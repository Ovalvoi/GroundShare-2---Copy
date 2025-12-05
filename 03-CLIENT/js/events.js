// הגדרת קבועים לכתובות ה-API
const API_PORT = 7057;
const API_BASE_URL = `https://localhost:${API_PORT}/api`;
const API_URL_EVENTS = `${API_BASE_URL}/Events`;
const API_URL_RATINGS = `${API_BASE_URL}/Ratings`;

// ---------------------------------------------------------
// פונקציית אתחול - רצה כשהדף (DOM) נטען במלואו
// ---------------------------------------------------------
document.addEventListener('DOMContentLoaded', () => {
    
    // 1. בדיקה האם יש משתמש מחובר
    const user = JSON.parse(localStorage.getItem('user'));
    if (!user) {
        alert('עליך להתחבר כדי להוסיף אירוע');
        window.location.href = 'login.html';
        return;
    }

    // 2. עדכון ה-Navbar (הצגת שם משתמש וכפתור התנתקות)
    if (typeof updateNavbarState === 'function') {
        updateNavbarState();
    }

    // 3. טעינת רשימת המיקומים לתוך ה-Dropdown (מ-locations.js)
    if (typeof loadLocationsForSelect === 'function') {
        loadLocationsForSelect();
    }

    // 4. חיבור פונקציית השליחה לטופס
    const form = document.querySelector('#addEventForm');
    if (form) {
        form.addEventListener('submit', onAddEventSubmit);
    }

    // 5. ניהול כפתורי הוספת מיקום (הצגה/הסתרה של הטופס המשני)
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

// ---------------------------------------------------------
// לוגיקה ראשית: שליחת אירוע (תהליך של 3 שלבים)
// 1. העלאת תמונה -> 2. יצירת אירוע -> 3. הוספת דירוג ראשוני
// ---------------------------------------------------------
async function onAddEventSubmit(e) {
    e.preventDefault(); // עצירת ברירת המחדל של הדפדפן
    
    const msgDiv = document.querySelector('#addEventMessage');
    const user = JSON.parse(localStorage.getItem('user'));

    // וידוא מזהה משתמש (תמיכה באות גדולה/קטנה בהתאם לשרת)
    const userId = user ? (user.userId || user.UserId) : null;

    if (!userId) {
        msgDiv.textContent = 'שגיאה: לא ניתן לזהות את המשתמש. נא להתחבר מחדש.';
        msgDiv.className = 'text-danger small mb-3';
        return;
    }

    // איסוף נתונים מהשדות בטופס
    const locationEl = document.querySelector('#locationSelect');
    const titleEl = document.querySelector('#eventTitle');
    const typeEl = document.querySelector('#eventType');
    const startEl = document.querySelector('#startDate');
    const endEl = document.querySelector('#endDate');
    const municipalityEl = document.querySelector('#municipality');
    const responsibleEl = document.querySelector('#responsibleBody');
    const statusEl = document.querySelector('#eventStatus');
    const descEl = document.querySelector('#description');
    
    // איסוף נתוני דירוג
    const noiseEl = document.querySelector('#noiseScore');
    const trafficEl = document.querySelector('#trafficScore');
    const safetyEl = document.querySelector('#safetyScore');

    // אלמנט קובץ התמונה
    const imageInput = document.querySelector('#eventImage');

    // ---------------------------------------------------------
    // בדיקות ולידציה + סימון באדום (Red Highlight)
    // ---------------------------------------------------------
    
    // ניקוי סימונים קודמים (הסרת המחלקה is-invalid מכל השדות)
    const allInputs = document.querySelectorAll('.form-control, .form-select');
    allInputs.forEach(el => el.classList.remove('is-invalid'));

    let hasError = false;

    // פונקציית עזר לבדיקה וסימון שדה
    function validateField(element) {
        if (!element || !element.value || element.value === "") {
            if (element) element.classList.add('is-invalid'); // הוספת Class של Bootstrap למסגרת אדומה
            hasError = true;
        }
    }

    // בדיקת שדות החובה (בהתאם ללוגיקה המקורית)
    validateField(locationEl);
    validateField(typeEl);
    validateField(startEl);
    validateField(municipalityEl);
    validateField(statusEl);
    
    // בדיקת דירוגים
    validateField(noiseEl);
    validateField(trafficEl);
    validateField(safetyEl);

    // בדיקת תמונה בנפרד (כי זה file input)
    if (!imageInput.files || imageInput.files.length === 0) {
        imageInput.classList.add('is-invalid');
        hasError = true;
    }

    // אם נמצאה שגיאה, מציגים הודעה ועוצרים
    if (hasError) {
        msgDiv.textContent = 'נא למלא את כל שדות החובה המסומנים באדום';
        msgDiv.className = 'text-danger small mb-3 fw-bold';
        return;
    }

    // ---------------------------------------------------------
    // שליחת הנתונים לשרת (רק אם הכל תקין)
    // ---------------------------------------------------------
    try {
        // הצגת חיווי למשתמש שהתהליך התחיל
        msgDiv.textContent = 'שומר נתונים...';
        msgDiv.className = 'text-info small mb-3';

        // --- שלב 1: העלאת תמונה לשרת ---
        let finalPhotoUrl = null;
        const formData = new FormData();
        formData.append('file', imageInput.files[0]);

        const uploadRes = await fetch(`${API_URL_EVENTS}/upload`, {
            method: 'POST',
            body: formData
        });

        if (!uploadRes.ok) throw new Error('שגיאה בהעלאת התמונה');
        const uploadData = await uploadRes.json();
        finalPhotoUrl = uploadData.path; // השרת מחזיר את הנתיב לשמירה ב-DB

        // --- שלב 2: יצירת רשומת האירוע ב-DB ---
        const eventData = {
            StartDateTime: startEl.value,
            EndDateTime: endEl.value ? endEl.value : null,
            EventsType: typeEl.value,
            PhotoUrl: finalPhotoUrl, 
            Description: titleEl.value ? `${titleEl.value} - ${descEl.value}` : descEl.value,
            Municipality: municipalityEl.value,
            ResponsibleBody: responsibleEl.value,
            EventsStatus: statusEl.value,
            LocationsId: parseInt(locationEl.value)
        };

        const createRes = await fetch(`${API_URL_EVENTS}/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eventData)
        });

        if (!createRes.ok) {
            const errText = await createRes.text();
            console.error('Event Creation Error:', errText);
            throw new Error('שגיאה ביצירת האירוע בשרת');
        }

        const createData = await createRes.json();
        const newEventId = createData.eventsId || createData.EventsId;

        // --- שלב 3: הוספת דירוג ראשוני לאירוע ---
        const avgScore = Math.round((Number(noiseEl.value) + Number(trafficEl.value) + Number(safetyEl.value)) / 3);
        
        const ratingData = {
            UserId: userId,
            EventsId: newEventId,
            OverallScore: avgScore,
            NoiseScore: parseInt(noiseEl.value),
            TrafficScore: parseInt(trafficEl.value),
            SafetyScore: parseInt(safetyEl.value),
            Comment: 'אין תיאור' // תיאור ברירת מחדל
        };

        const ratingRes = await fetch(`${API_URL_RATINGS}/add`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(ratingData)
        });

        if (!ratingRes.ok) {
            const errText = await ratingRes.text();
            console.error('Rating Creation Error:', errText);
            throw new Error('האירוע נוצר אך הייתה שגיאה בשמירת הדירוג: ' + errText);
        }

        // --- סיום מוצלח ---
        msgDiv.textContent = 'האירוע והדירוג נוספו בהצלחה! מעביר לדף הבית...';
        msgDiv.className = 'text-success small mb-3';
        
        // המתנה קצרה ומעבר לדף הבית
        setTimeout(() => {
            window.location.href = 'index.html';
        }, 0);

    } catch (error) {
        console.error(error);
        msgDiv.textContent = error.message;
        msgDiv.className = 'text-danger small mb-3';
    }
}

// ---------------------------------------------------------
// פונקציית עזר לעדכון ה-Navbar
// בודקת אם יש משתמש ב-localStorage ומציגה את שמו וכפתור התנתקות
// ---------------------------------------------------------
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

        // הוספת אלמנט "שלום משתמש" וכפתור התנתקות
        if (!document.querySelector('#logoutBtn')) {
            const li = document.createElement('li');
            li.className = 'nav-item d-flex align-items-center';
            // תמיכה באות גדולה/קטנה לשם המשתמש
            const userName = user.FirstName || user.firstName || 'משתמש';
            li.innerHTML = `
                <span class="nav-link text-dark fw-bold">שלום, ${userName}</span>
                <button id="logoutBtn" class="btn btn-outline-danger btn-sm ms-2">התנתק</button>
            `;
            navList.appendChild(li);

            // אירוע לחיצה על התנתקות
            document.querySelector('#logoutBtn').addEventListener('click', () => {
                localStorage.removeItem('user');
                window.location.href = 'login.html';
            });
        }
    }
}