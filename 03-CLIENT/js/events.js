// ייבוא הולידציה לכותרת
import { validateEventTitle, validateLocationCity, validateLocationStreet, validateLocationHouseNumber, validateLocationFloor} from './validators.js';

var API_BASE_URL = 'https://localhost:7057/api';

document.addEventListener('DOMContentLoaded', () => {
    
    // חיבור ולידציה לשדות של מיקום חדש (כדי שהם יקבלו צבע אדום תוך כדי הקלדה)
    const cityInput = document.getElementById('newCity');
    if(cityInput) {
        cityInput.addEventListener('input', validateLocationCity);
    }
    const streetInput = document.getElementById('newStreet');
    if(streetInput) {
        streetInput.addEventListener('input', validateLocationStreet);
    }
    const houseNumberInput = document.getElementById('newHouseNumber');
    if(houseNumberInput) {
        houseNumberInput.addEventListener('input', validateLocationHouseNumber);
    }
    const floorInput = document.getElementById('newFloor');
    if(floorInput) {
        floorInput.addEventListener('input', validateLocationFloor);
    }

    const user = JSON.parse(localStorage.getItem('user'));
    if (!user) {
        alert('עליך להתחבר כדי להוסיף אירוע');
        window.location.href = 'login.html';
        return;
    }

    // Call the local update function directly to ensure it runs
    updateNavbarState();

    if (typeof loadLocationsForSelect === 'function') {
        loadLocationsForSelect();
    }

    const form = document.querySelector('#addEventForm');
    if (form) {
        // חיבור ולידציה לכותרת
        const titleInput = document.getElementById('eventTitle');
        if(titleInput) {
            titleInput.addEventListener('input', validateEventTitle);
        }

        form.addEventListener('submit', onAddEventSubmit);
    }

    const toggleLocBtn = document.querySelector('#toggleNewLocationBtn');
    const addLocBtn = document.querySelector('#addLocationBtn');

    if (toggleLocBtn) {
        toggleLocBtn.addEventListener('click', () => {
            document.querySelector('#newLocationSection').classList.toggle('d-none');
        });
    }

    if (addLocBtn && typeof addNewLocation === 'function') {
        addLocBtn.addEventListener('click', addNewLocation);
    }
});

async function onAddEventSubmit(e) {
    e.preventDefault();
    e.stopPropagation();

    // 1. Validation
    const titleInput = document.getElementById('eventTitle');
    if (titleInput && titleInput.value) {
        validateEventTitle({ target: titleInput });
    }

    if (!e.target.checkValidity()) {
        e.target.classList.add('was-validated');
        const msgDiv = document.querySelector('#addEventMessage');
        if (msgDiv) {
            msgDiv.textContent = 'נא לתקן את השגיאות בטופס לפני השליחה';
            msgDiv.className = 'text-danger small mb-3 fw-bold';
        }
        return;
    }

    // 2. Gather Elements
    const msgDiv = document.querySelector('#addEventMessage');
    if (!msgDiv) return;

    const user = JSON.parse(localStorage.getItem('user'));
    const userId = user ? (user.userId || user.UserId) : null;

    if (!userId) {
        msgDiv.textContent = 'שגיאה: לא ניתן לזהות את המשתמש. נא להתחבר מחדש.';
        msgDiv.className = 'text-danger small mb-3';
        return;
    }

    // Select inputs
    const locationEl = document.querySelector('#locationSelect');
    const titleEl = document.querySelector('#eventTitle');
    const typeEl = document.querySelector('#eventType');
    const startEl = document.querySelector('#startDate');
    const endEl = document.querySelector('#endDate');
    const municipalityEl = document.querySelector('#municipality');
    const responsibleEl = document.querySelector('#responsibleBody');
    const statusEl = document.querySelector('#eventStatus');
    const descEl = document.querySelector('#description');
    
    const noiseEl = document.querySelector('#noiseScore');
    const trafficEl = document.querySelector('#trafficScore');
    const safetyEl = document.querySelector('#safetyScore');
    const imageInput = document.querySelector('#eventImage');

    // 3. Image Check
    if (!imageInput.files || imageInput.files.length === 0) {
        msgDiv.textContent = 'חובה להעלות תמונה';
        msgDiv.className = 'text-danger small mb-3';
        return;
    }

    try {
        msgDiv.textContent = 'שומר נתונים...';
        msgDiv.className = 'text-info small mb-3';
        
        // --- Step A: Upload Image ---
        let finalPhotoUrl = null;
        const formData = new FormData();
        formData.append('file', imageInput.files[0]);

        const uploadRes = await fetch(`${API_BASE_URL}/Events/upload`, {
            method: 'POST',
            body: formData
        });

        if (!uploadRes.ok) throw new Error(`Image upload failed: ${uploadRes.statusText}`);
        const uploadData = await uploadRes.json();
        finalPhotoUrl = uploadData.path;
        
        // --- Step B: Create Event ---
        const locId = parseInt(locationEl.value);
        if (isNaN(locId)) {
            throw new Error("Invalid Location ID selected");
        }

        const eventData = {
            StartDateTime: startEl.value,
            EndDateTime: endEl.value ? endEl.value : null,
            EventsType: typeEl.value,
            PhotoUrl: finalPhotoUrl, 
            Description: titleEl.value ? `${titleEl.value} - ${descEl.value}` : descEl.value,
            Municipality: municipalityEl.value,
            ResponsibleBody: responsibleEl.value,
            EventsStatus: statusEl.value,
            LocationsId: locId
        };

        const createRes = await fetch(`${API_BASE_URL}/Events/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eventData)
        });

        if (!createRes.ok) {
            const errorText = await createRes.text(); 
            throw new Error(`Event creation failed (${createRes.status}): ${errorText}`);
        }

        const createData = await createRes.json();
        const newEventId = createData.eventsId || createData.EventsId;

        // --- Step C: Add Rating ---
        const avgScore = Math.round((Number(noiseEl.value) + Number(trafficEl.value) + Number(safetyEl.value)) / 3);
        
        const ratingData = {
            UserId: userId,
            EventsId: newEventId,
            OverallScore: avgScore,
            NoiseScore: parseInt(noiseEl.value),
            TrafficScore: parseInt(trafficEl.value),
            SafetyScore: parseInt(safetyEl.value),
            Comment: 'אין תיאור'
        };

        const ratingRes = await fetch(`${API_BASE_URL}/Ratings/add`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(ratingData)
        });

        if (!ratingRes.ok) throw new Error('Event created, but rating failed.');

        // --- Success ---
        msgDiv.textContent = 'האירוע והדירוג נוספו בהצלחה! מעביר לדף הבית...';
        msgDiv.className = 'text-success small mb-3 fw-bold';
        window.location.href = 'index.html';

    } catch (error) {
        console.error(error); // Keep actual errors in console for debugging crashes
        msgDiv.textContent = `שגיאה: ${error.message}`;
        msgDiv.className = 'text-danger small mb-3 fw-bold';
    }
}

// =========================================================
// פונקציה לניהול סרגל הניווט (Navbar)
// בודקת אם המשתמש מחובר ומציגה כפתור יציאה ושם משתמש
// =========================================================
function updateNavbarState() {
    const user = JSON.parse(localStorage.getItem('user'));
    const navList = document.querySelector('.navbar-nav');
    if (!navList) return;

    if (user) {
        // הסתרת לינקים של אורח (התחברות/הרשמה)
        const guestLinks = document.querySelectorAll('a[href="login.html"], a[href="register.html"]');
        guestLinks.forEach(link => {
            if (link.parentElement) link.parentElement.style.display = 'none';
        });

        // יצירת אזור "שלום משתמש" וכפתור התנתקות
        if (!document.getElementById('logoutBtn')) {
            const li = document.createElement('li');
            li.className = 'nav-item d-flex align-items-center ms-lg-3';
            const userName = user.FirstName || user.firstName || 'משתמש';
            li.innerHTML = `
                <div class="d-flex align-items-center gap-2">
                    <span class="fw-bold text-primary">שלום, ${userName}</span>
                    <button id="logoutBtn" class="btn btn-sm btn-outline-danger">
                        <i class="fa-solid fa-right-from-bracket"></i>
                    </button>
                </div>
            `;
            navList.appendChild(li);

            // אירוע לחיצה על כפתור התנתקות
            document.getElementById('logoutBtn').addEventListener('click', () => {
                localStorage.removeItem('user');
                window.location.href = 'login.html';
            });
        }
    }
}