// ייבוא פונקציות הולידציה
import { validateName, validatePhone, validatePassword } from './validators.js';

// הגדרת כתובת ה-API הבסיסית
const API_BASE_URL = 'https://localhost:7057/api';

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
        // 1. חיבור בדיקות ולידציה בזמן אמת (בעת הקלדה)
        document.getElementById('firstName').addEventListener('input', validateName);
        document.getElementById('lastName').addEventListener('input', validateName);
        document.getElementById('phoneNumber').addEventListener('input', validatePhone);
        document.getElementById('password').addEventListener('input', validatePassword);

        // 2. חיבור אירוע השליחה
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

    // בדיקה שהשדות אינם ריקים לפני השליחה
    if (!emailVal || !passwordVal) {
        if(msgDiv) msgDiv.textContent = 'אימייל או סיסמה שגויים';
        return; // עצירת הפונקציה אם אחד השדות ריק
    }

    // יצירת האובייקט לשליחה לשרת
    const loginData = {
        Email: emailVal,
        Password: passwordVal
    };

    // שליחת בקשת POST
    fetch(`${API_BASE_URL}/Users/login`, {
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
    // --- בדיקת ולידציה לפני שליחה ---
    // הפונקציה checkValidity בודקת אם יש setCustomValidity באחד השדות
    if (!e.target.checkValidity()) {
        e.preventDefault();
        e.stopPropagation();
        
        // הוספת מחלקה של בוטסטראפ שמציגה את השגיאות ויזואלית
        e.target.classList.add('was-validated');
        
        const msgDiv = document.querySelector('#registerMessage');
        if(msgDiv) msgDiv.textContent = 'אנא תקן את השגיאות בטופס לפני השליחה';
        return; 
    }

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
    fetch(`${API_BASE_URL}/Users/register`, {
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