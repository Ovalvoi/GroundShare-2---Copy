// =========================================================
// הגדרת קבועים (Constants) לכתובות השרת
// =========================================================
const API_URL_ALL_EVENTS = 'https://localhost:7057/api/Events/all';
const API_URL_DELETE = 'https://localhost:7057/api/Events/delete';

// =========================================================
// אירוע טעינת הדף (DOMContentLoaded)
// הפונקציה הראשית שרצה אוטומטית כשהדף עולה
// =========================================================
document.addEventListener('DOMContentLoaded', () => {
    // בדיקה אם קיימת פונקציה חיצונית לעדכון ה-Navbar, ואם לא - משתמשים במקומית
    if (typeof updateNavbarState === 'function') {
        updateNavbarState();
    } else {
        localUpdateNavbar();
    }
    // קריאה לפונקציה שטוענת את רשימת האירועים מהשרת
    loadAllEvents();
});

// =========================================================
// פונקציה לטעינת כל האירועים מהשרת (GET Request)
// =========================================================
function loadAllEvents() {
    const container = document.querySelector('#eventsContainer');
    if (!container) return;

    // שלב 1: הצגת ספינר (Loading) למשתמש עד שהמידע יגיע
    container.innerHTML = `
        <div class="col-12 text-center py-5">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">טוען אירועים...</p>
        </div>
    `;

    // שלב 2: שליחת בקשה לשרת לקבלת רשימת האירועים
    fetch(API_URL_ALL_EVENTS)
        .then(res => {
            if (!res.ok) throw new Error('שגיאה בטעינת הנתונים');
            return res.json();
        })
        .then(events => {
            // שלב 3: שליחת המידע שהתקבל לפונקציה שמציירת אותו על המסך
            renderEvents(events);
        })
        .catch(err => {
            // במקרה של שגיאה: הצגת הודעה אדומה למשתמש
            container.innerHTML = `
                <div class="col-12 alert alert-danger text-center">
                    שגיאה בטעינת אירועים: ${err.message}
                </div>
            `;
        });
}

// =========================================================
// פונקציה להצגת האירועים על המסך (Render)
// מקבלת מערך של אירועים ומייצרת HTML עבור כל אחד
// =========================================================
function renderEvents(events) {
    const container = document.querySelector('#eventsContainer');
    // ניקוי הקונטיינר מתוכן קודם (כמו הספינר)
    container.innerHTML = '';

    // בדיקה אם המערך ריק (אין אירועים)
    if (!events || events.length === 0) {
        container.innerHTML = '<div class="col-12 text-center">לא נמצאו אירועים במערכת</div>';
        return;
    }

    // לולאה שעוברת על כל אירוע ובונה לו כרטיס
    events.forEach(event => {
        // טיפול בתמונה: אם הנתיב הוא יחסי (images/...), מוסיפים לו את כתובת השרת
        let imageUrl = event.photoUrl || 'https://placehold.co/600x400?text=No+Image';
        if (imageUrl.startsWith('images/')) {
            imageUrl = `https://localhost:7057/${imageUrl}`;
        }

        // עיצוב הדירוג (נקודה עשרונית אחת)
        const rating = event.avgRating ? event.avgRating.toFixed(1) : '0.0';

        // המרת תאריכים לפורמט קריא (יום/חודש/שנה)
        const startDate = new Date(event.startDateTime).toLocaleDateString('he-IL');
        const endDate = event.endDateTime ? new Date(event.endDateTime).toLocaleDateString('he-IL') : 'לא ידוע';

        // בניית ה-HTML של הכרטיס (Card)
        const cardHtml = `
            <div class="col-md-6 col-lg-4">
                <div class="card h-100 shadow-sm event-card">
                    <div class="position-relative">
                        <img src="${imageUrl}" class="card-img-top event-img-top" alt="תמונת אירוע">
                        <span class="status-badge status-${event.eventsStatus}">${event.eventsStatus}</span>
                    </div>
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title fw-bold">${event.eventsType}</h5>
                        <h6 class="card-subtitle mb-2 text-muted">
                            <i class="fa-solid fa-map-pin me-1"></i>
                            ${event.city}, ${event.street} ${event.houseNumber}
                        </h6>
                        
                        <p class="card-text flex-grow-1">
                            ${event.description}
                        </p>
                        
                        <div class="text-muted small mb-3">
                            <div><i class="fa-regular fa-calendar me-1"></i> התחלה: ${startDate}</div>
                            <div><i class="fa-regular fa-calendar-check me-1"></i> סיום: ${endDate}</div>
                        </div>

                        <div class="d-flex gap-2 mt-2 pt-2 border-top">
                             <button onclick="deleteEvent('${event.eventsId}')" 
                                class="btn btn-sm btn-outline-danger flex-fill">
                                <i class="fa-solid fa-trash"></i> מחיקה
                             </button>
                        </div>

                        <div class="d-flex justify-content-between align-items-center mt-3 pt-2 border-top">
                             <div class="text-warning fw-bold">
                                <i class="fa-solid fa-star"></i> ${rating}
                                <span class="text-muted small fw-normal">(${event.ratingCount})</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // הוספת הכרטיס החדש לרשימה בדף
        container.innerHTML += cardHtml;
    });
}

// =========================================================
// פונקציה למחיקת אירוע (DELETE Request)
// מופעלת בלחיצה על כפתור המחיקה בכרטיס
// =========================================================
function deleteEvent(id) {
    // וידוא מול המשתמש שהוא באמת רוצה למחוק
    if (!confirm("האם אתה בטוח שברצונך למחוק אירוע זה?")) return;

    // שליחת בקשת מחיקה לשרת
    fetch(`${API_URL_DELETE}/${id}`, {
        method: 'DELETE'
    })
        .then(res => {
            if (res.ok) {
                alert('האירוע נמחק בהצלחה');
                // טעינה מחדש של האירועים כדי לעדכן את המסך
                loadAllEvents();
            } else {
                alert('שגיאה במחיקת האירוע');
            }
        })
        .catch(err => console.error(err));
}

// =========================================================
// פונקציה לניהול סרגל הניווט (Navbar)
// בודקת אם המשתמש מחובר ומציגה כפתור יציאה ושם משתמש
// =========================================================
function localUpdateNavbar() {
    // בדיקה ב-LocalStorage אם יש משתמש שמור
    const user = JSON.parse(localStorage.getItem('user'));
    const navList = document.querySelector('.navbar-nav');
    if (!navList) return;

    if (user) {
        // הסתרת הלינקים של "התחברות" ו"הרשמה" כי המשתמש כבר מחובר
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

            // אירוע לחיצה על כפתור התנתקות - מוחק את המשתמש ומעביר לדף ההתחברות
            document.getElementById('logoutBtn').addEventListener('click', () => {
                localStorage.removeItem('user');
                window.location.href = 'login.html';
            });
        }
    }
}