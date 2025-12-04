const API_URL_ALL_EVENTS = 'https://localhost:7057/api/Events/all';

// ---------------------------------------------------------
// אתחול דף הבית (Home Page)
// ---------------------------------------------------------
document.addEventListener('DOMContentLoaded', () => {
    
    // 1. עדכון התפריט העליון (הצגת שם משתמש אם מחובר)
    // הערה: פונקציה זו הוגדרה ב-events.js, אם הקובץ לא נטען כאן יש להעתיקה או לייבא אותה
    // לצורך הדגמה, נניח שהיא זמינה או שנוסיף אותה גם כאן אם צריך.
    if (typeof updateNavbarState === 'function') {
        updateNavbarState();
    } else {
        // מימוש מקומי במידה והפונקציה לא קיימת
        localUpdateNavbar();
    }

    // 2. טעינת רשימת האירועים מהשרת
    loadAllEvents();
});

// ---------------------------------------------------------
// שליפת כל האירועים מהשרת והצגתם במסך
// ---------------------------------------------------------
function loadAllEvents() {
    const container = document.querySelector('#eventsContainer');
    if (!container) return;

    // הצגת אנימציית טעינה (Spinner) למשתמש
    container.innerHTML = `
        <div class="col-12 text-center py-5">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">טוען אירועים...</p>
        </div>
    `;

    // ביצוע בקשת GET
    fetch(API_URL_ALL_EVENTS)
        .then(res => {
            if (!res.ok) throw new Error('שגיאה בטעינת הנתונים');
            return res.json();
        })
        .then(events => {
            // שליחת הנתונים לפונקציית הרינדור
            renderEvents(events);
        })
        .catch(err => {
            // הצגת הודעת שגיאה במקרה של כישלון
            container.innerHTML = `
                <div class="col-12 alert alert-danger text-center">
                    שגיאה בטעינת אירועים: ${err.message}
                </div>
            `;
        });
}

// ---------------------------------------------------------
// פונקציה לבניית ה-HTML של כרטיסי האירועים (Render)
// ---------------------------------------------------------
function renderEvents(events) {
    const container = document.querySelector('#eventsContainer');
    container.innerHTML = ''; // ניקוי תוכן קודם

    // בדיקה אם אין אירועים כלל
    if (!events || events.length === 0) {
        container.innerHTML = '<div class="col-12 text-center">לא נמצאו אירועים במערכת</div>';
        return;
    }

    // לולאה על כל אירוע ליצירת כרטיס
    events.forEach(event => {
        // טיפול בכתובת התמונה:
        // אם הכתובת מתחילה ב-"images/", זה קובץ מקומי בשרת ויש להוסיף לו את הדומיין
        let imageUrl = event.photoUrl || 'https://placehold.co/600x400?text=No+Image';
        if (imageUrl.startsWith('images/')) {
            imageUrl = `https://localhost:7057/${imageUrl}`;
        }

        // עיצוב הדירוג (ספרה אחת אחרי הנקודה)
        const rating = event.avgRating ? event.avgRating.toFixed(1) : '0.0';
        
        // יצירת ה-HTML של הכרטיס (שימוש ב-Template Literals)
        const cardHtml = `
            <div class="col-md-6 col-lg-4">
                <div class="card h-100 shadow-sm event-card">
                    <div class="position-relative">
                        <img src="${imageUrl}" class="card-img-top event-img-top" alt="תמונת אירוע">
                        <span class="status-badge status-${event.eventsStatus}">${event.eventsStatus}</span>
                    </div>
                    <div class="card-body">
                        <h5 class="card-title fw-bold">${event.eventsType}</h5>
                        <h6 class="card-subtitle mb-2 text-muted">
                            <i class="fa-solid fa-map-pin me-1"></i>
                            ${event.city}, ${event.street} ${event.houseNumber}
                        </h6>
                        <p class="card-text text-truncate" title="${event.description}">
                            ${event.description}
                        </p>
                        <div class="d-flex justify-content-between align-items-center mt-3">
                            <small class="text-muted">
                                <i class="fa-regular fa-calendar me-1"></i>
                                ${new Date(event.startDateTime).toLocaleDateString('he-IL')}
                            </small>
                            <div class="text-warning fw-bold">
                                <i class="fa-solid fa-star"></i> ${rating}
                                <span class="text-muted small fw-normal">(${event.ratingCount})</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // הוספת הכרטיס לקונטיינר
        container.innerHTML += cardHtml;
    });
}

// ---------------------------------------------------------
// פונקציה מקומית לעדכון ה-Navbar (במקרה ואין קובץ משותף)
// ---------------------------------------------------------
function localUpdateNavbar() {
    const user = JSON.parse(localStorage.getItem('user'));
    const navList = document.querySelector('.navbar-nav');
    if (!navList) return;

    if (user) {
        // הסתרת כפתורי אורח
        const guestLinks = document.querySelectorAll('a[href="login.html"], a[href="register.html"]');
        guestLinks.forEach(link => {
            if(link.parentElement) link.parentElement.style.display = 'none';
        });

        // הוספת כפתור התנתקות
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

            document.getElementById('logoutBtn').addEventListener('click', () => {
                localStorage.removeItem('user');
                window.location.href = 'login.html';
            });
        }
    }
}