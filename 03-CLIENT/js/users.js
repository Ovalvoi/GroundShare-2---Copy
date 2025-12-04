// הגדרת כתובת ה-API של המשתמשים
const API_URL_USERS = 'https://localhost:7057/api/Users';

// ---------------------------------------------------------
// פונקציה ראשית שרצה בטעינת הדף
// מזהה איזה טופס קיים בדף (התחברות או הרשמה) ומחברת אליו את האירוע המתאים
// ---------------------------------------------------------
document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.querySelector('#loginForm');
    const registerForm = document.querySelector('#registerForm');

    // אם אנחנו בדף התחברות - חבר את פונקציית הלוגין
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    // אם אנחנו בדף הרשמה - חבר את פונקציית ההרשמה
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }
});

// ---------------------------------------------------------
// לוגיקה להתחברות (Login)
// ---------------------------------------------------------
function handleLogin(e) {
    e.preventDefault(); // מניעת רענון הדף האוטומטי של הדפדפן

    // שליפת הערכים מהשדות
    const emailVal = document.querySelector('#email').value;
    const passwordVal = document.querySelector('#password').value;
    const msgDiv = document.querySelector('#loginMessage');

    // איפוס הודעות שגיאה קודמות
    if(msgDiv) msgDiv.textContent = '';

    // יצירת האובייקט לשליחה לשרת
    const loginData = {
        Email: emailVal,
        Password: passwordVal
    };

    // שליחת בקשת POST
    fetch(`${API_URL_USERS}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(loginData)
    })
    .then(res => {
        // טיפול בשגיאות נפוצות
        if (res.status === 401) throw new Error('אימייל או סיסמה שגויים');
        if (!res.ok) throw new Error('שגיאה בהתחברות לשרת');
        return res.json();
    })
    .then(user => {
        // התחברות מוצלחת: שמירת פרטי המשתמש ב-localStorage
        localStorage.setItem('user', JSON.stringify(user));
        // העברה לדף הבית
        window.location.href = 'index.html';
    })
    .catch(err => {
        // הצגת שגיאה למשתמש
        if(msgDiv) msgDiv.textContent = err.message;
    });
}

// ---------------------------------------------------------
// לוגיקה להרשמה (Register)
// ---------------------------------------------------------
function handleRegister(e) {
    e.preventDefault(); // מניעת רענון הדף

    const msgDiv = document.querySelector('#registerMessage');
    if(msgDiv) msgDiv.textContent = '';

    // איסוף כל הנתונים מהטופס
    const firstName = document.querySelector('#firstName').value;
    const lastName = document.querySelector('#lastName').value;
    const email = document.querySelector('#email').value;
    const password = document.querySelector('#password').value;
    const confirmPassword = document.querySelector('#confirmPassword').value;
    const dateOfBirth = document.querySelector('#dateOfBirth').value;
    const phoneNumber = document.querySelector('#phoneNumber').value;
    const address = document.querySelector('#address').value;

    // בדיקה ששתי הסיסמאות שהוזנו זהות
    if (password !== confirmPassword) {
        if(msgDiv) msgDiv.textContent = 'הסיסמאות אינן תואמות';
        return;
    }

    // בניית האובייקט לשליחה לשרת (חייב להתאים למבנה User ב-C#)
    const newUser = {
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        Password: password,
        DateOfBirth: dateOfBirth,
        PhoneNumber: phoneNumber,
        Address: address,
        IsAdmin: false // ברירת מחדל למשתמש רגיל
    };

    // שליחת בקשת POST
    fetch(`${API_URL_USERS}/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(newUser)
    })
    .then(res => {
        // אם האימייל תפוס, השרת יחזיר 409
        if (res.status === 409) throw new Error('האימייל כבר קיים במערכת');
        if (!res.ok) throw new Error('שגיאה בהרשמה');
        return res.json();
    })
    .then(data => {
        // הרשמה מוצלחת
        alert('נרשמת בהצלחה! כעת ניתן להתחבר');
        window.location.href = 'login.html';
    })
    .catch(err => {
        if(msgDiv) msgDiv.textContent = err.message;
    });
}

// ---------------------------------------------------------
// פונקציית התנתקות (Logout)
// ---------------------------------------------------------
function logout() {
    // מחיקת המשתמש מהזיכרון המקומי
    localStorage.removeItem('user');
    // חזרה לדף ההתחברות
    window.location.href = 'login.html';
}