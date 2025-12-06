// =========================================================
// הגדרת קבועים (Constants) לכתובות השרת
// =========================================================
const API_BASE_URL = 'https://localhost:7057/api';

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

    // 1. טעינת הסטטיסטיקות (Ad-Hoc)
    loadEventStats();

    // 2. טעינת רשימת האירועים מהשרת
    loadAllEvents();
});

// =========================================================
// פונקציה לטעינת הסטטיסטיקות (Ad-Hoc Feature)
// =========================================================
function loadEventStats() {
    fetch(`${API_BASE_URL}/Events/stats`)
        .then(res => {
            if (!res.ok) return null; // אם נכשל, פשוט לא נציג סטטיסטיקות
            return res.json();
        })
        .then(stats => {
            if (stats && stats.length > 0) {
                renderStatsBox(stats);
            }
        })
        .catch(err => console.error("Error loading stats:", err));
}

// =========================================================
// פונקציה לציור קופסת הסטטיסטיקות
// =========================================================
function renderStatsBox(stats) {
    const container = document.querySelector('#eventsContainer');
    if (!container) return;

    // Remove existing stats box if present to prevent duplication
    const existingStats = document.getElementById('statsBoxContainer');
    if (existingStats) {
        existingStats.remove();
    }

    // יצירת האלמנט של הסטטיסטיקות
    const statsDiv = document.createElement('div');
    statsDiv.id = 'statsBoxContainer'; // Assign an ID to easily find and remove it later
    statsDiv.className = 'col-12 mb-1';

    // חישוב סה"כ אירועים
    //the calculation of total events from stats array
    //example: [{ type: "חשמל", amount: 5 }, { type: "כביש", amount: 3 }] => totalEvents = 8
    const totalEvents = stats.reduce((sum, item) => sum + item.amount, 0);

    // בניית ה-HTML הפנימי של הקופסה
    let statsHtml = `
        <div class="card shadow-sm border-0 bg-white">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                <h5 class="mb-0"><i class="fa-solid fa-chart-pie me-2"></i>תמונת מצב בשימוש Ad-Hoc</h5>
                <span class="badge bg-light text-primary rounded-pill">${totalEvents} אירועים במערכת</span>
            </div>
            <div class="card-body">
                <div class="row text-center justify-content-center g-3">
    `;

    // הוספת פריט עבור כל סוג אירוע
    stats.forEach(item => {
        // בחירת אייקון לפי סוג האירוע
        let icon = 'fa-circle-info';
        if (item.type.includes('חשמל')) icon = 'fa-bolt';
        else if (item.type.includes('כביש') || item.type.includes('תשתית')) icon = 'fa-road';
        else if (item.type.includes('בנייה') || item.type.includes('תמ"א')) icon = 'fa-helmet-safety';

        statsHtml += `
            <div class="col-6 col-sm-4 col-md-2">
                <div class="p-2 border rounded bg-light h-100">
                    <i class="fa-solid ${icon} text-secondary mb-2 fs-5"></i>
                    <h4 class="h5 fw-bold text-primary mb-0">${item.amount}</h4>
                    <small class="text-muted">${item.type}</small>
                </div>
            </div>
        `;
    });

    statsHtml += `
                </div>
            </div>
        </div>
    `;

    statsDiv.innerHTML = statsHtml;

    // הוספת הקופסה לפני רשימת האירועים (insert before)
    container.parentNode.insertBefore(statsDiv, container);
}

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
    fetch(`${API_BASE_URL}/Events/all`)
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
    const statsBox = document.getElementById('statsBoxContainer');
    container.innerHTML = '';
    if (statsBox) {
        container.appendChild(statsBox);
    }

    // בדיקה אם המערך ריק (אין אירועים)
    if (!events || events.length === 0) {
        const msg = document.createElement('div');
        msg.className = 'col-12 text-center';
        msg.textContent = 'לא נמצאו אירועים במערכת';
        container.appendChild(msg);
        return;
    }

    // לולאה שעוברת על כל אירוע ובונה לו כרטיס
    events.forEach(event => {
        // טיפול בתמונה: אם הנתיב הוא יחסי (images/...), מוסיפים לו את כתובת השרת
        let imageUrl = event.photoUrl || 'https://placehold.co/600x400?text=No+Image';
        // אם התמונה לא מתחילה ב-http (כלומר היא נתיב מקומי), נוסיף לה את כתובת השרת
        // הערה: שיניתי את הבדיקה כדי שתהיה חכמה יותר ותתמוך גם בתמונות חיצוניות
        if (!imageUrl.startsWith('http')) {
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
                        <img src="${imageUrl}" class="card-img-top event-img-top" alt="תמונת אירוע" onerror="this.src='https://placehold.co/600x400?text=Error'">
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
                        <!-- כפתור עדכון סטטוס -->
                             <button onclick="markToEdit('${event.eventsId}')" class="btn btn-sm btn-outline-success flex-fill ms-1">
                                <i class="fa-solid fa-pencil"></i> סטטוס
                             </button>
                             <!-- כפתור מחיקה אירוע -->
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
        // שימוש ב-insertAdjacentHTML כדי לא להרוס את ה-DOM הקיים (סטטיסטיקות)
        container.insertAdjacentHTML('beforeend', cardHtml);
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
    fetch(`${API_BASE_URL}/Events/delete/${id}`, {
        method: 'DELETE'
    })
        .then(res => {
            if (res.ok) {
                alert('האירוע נמחק בהצלחה');
                // טעינה מחדש של האירועים כדי לעדכן את המסך
                loadAllEvents();
                // עדכון גם של הסטטיסטיקות
                loadEventStats();
            } else {
                alert('שגיאה במחיקת האירוע');
            }
        })
        .catch(err => console.error(err));
}

// =========================================================
// פונקציה לעדכון סטטוס אירוע (PUT Request)
// =========================================================
function markToEdit(id) {
    // בדיקה אם המודל כבר קיים בדף, ואם לא - יצירה שלו דינאמית
    if (!document.getElementById('statusUpdateModal')) {
        const modalHtml = `
            <div class="modal fade" id="statusUpdateModal" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header bg-success text-white">
                            <h5 class="modal-title"><i class="fa fa-pencil me-2"></i>עדכון סטטוס אירוע</h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <input type="hidden" id="statusUpdateEventId">
                            <div class="mb-3">
                                <label for="statusSelect" class="form-label fw-bold">בחר את הסטטוס החדש:</label>
                                <select class="form-select" id="statusSelect">
                                    <option value="קרה">קרה (הסתיים)</option>
                                    <option value="קורה">קורה (מתרחש כעת)</option>
                                    <option value="יקרה">יקרה (עתידי)</option>
                                </select>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ביטול</button>
                            <button type="button" class="btn btn-success" onclick="submitStatusUpdate()">שמור שינויים</button>
                        </div>
                    </div>
                </div>
            </div>`;
        
        // הוספת המודל לגוף הדף
        document.body.insertAdjacentHTML('beforeend', modalHtml);
    }

    // שמירת ה-ID של האירוע הנוכחי בתוך שדה נסתר במודל
    document.getElementById('statusUpdateEventId').value = id;

    // איפוס הבחירה לברירת מחדל (אופציונלי)
    document.getElementById('statusSelect').value = 'קורה';

    // פתיחת המודל באמצעות Bootstrap
    const modalElement = document.getElementById('statusUpdateModal');
    const modal = new bootstrap.Modal(modalElement);
    modal.show();
}

// פונקציה לשליחת העדכון מהמודל
function submitStatusUpdate() {
    const id = document.getElementById('statusUpdateEventId').value;
    const newStatus = document.getElementById('statusSelect').value;

    // שליחת בקשת PUT לשרת
    fetch(`${API_BASE_URL}/Events/updateStatus/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newStatus)
    })
    .then(res => {
        if (res.ok) {
            // סגירת המודל
            const modalElement = document.getElementById('statusUpdateModal');
            const modal = bootstrap.Modal.getInstance(modalElement);
            modal.hide();

            // הודעת הצלחה ורענון
            alert("הסטטוס עודכן בהצלחה!");
            loadAllEvents(); // רענון רשימת האירועים
            loadEventStats(); // רענון הסטטיסטיקות
        } else {
            alert("שגיאה בעדכון הסטטוס");
        }
    })
    .catch(err => {
        console.error(err);
        alert("שגיאה בתקשורת עם השרת");
    });
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