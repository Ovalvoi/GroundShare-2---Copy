// ---------------------------------------------------------
// פונקציית עזר לניהול הולידציה (הצגת שגיאה או אישור)
// ---------------------------------------------------------
function validateInput(event, regex, errorMsg) {
    const value = event.target.value;
    
    // .test() בודק האם הערך תואם לביטוי הרגולרי
    if (!regex.test(value)) {
        // 1. הגדרת הודעת שגיאה פנימית של הדפדפן (מונע את השליחה)
        event.target.setCustomValidity(errorMsg);
        
        // 2. הוספת עיצוב של בוטסטראפ (מסגרת אדומה)
        event.target.classList.add('is-invalid');
        event.target.classList.remove('is-valid');
        
        //console.log(`Invalid input: ${value}`);
    } else {
        // 1. ניקוי השגיאה
        event.target.setCustomValidity('');
        
        // 2. הוספת עיצוב של בוטסטראפ (מסגרת ירוקה)
        event.target.classList.remove('is-invalid');
        event.target.classList.add('is-valid');
        
        //console.log('Valid input');
    }
}

// ---------------------------------------------------------
// ולידציות להרשמה (Registration)
// ---------------------------------------------------------

// שם: אותיות בעברית או באנגלית, רווחים, בין 2 ל-30 תווים
export function validateName(event) {
    const regex = /^[\u0590-\u05FFa-zA-Z\s]{2,30}$/;
    validateInput(event, regex, 'יש להזין שם תקין (אותיות בלבד, לפחות 2 תווים)');
}

// טלפון: פורמט ישראלי (05X-XXXXXXX או 05XXXXXXXX)
export function validatePhone(event) {
    const regex = /^05\d-?\d{7}$/;
    validateInput(event, regex, 'מספר טלפון לא תקין (חייב להתחיל ב-05)');
}

// סיסמה: לפחות 4 תווים, חייבת להכיל גם אות וגם מספר
export function validatePassword(event) {
    // Lookahead for letter, Lookahead for number, min 4 chars total
    const regex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{4,}$/;
    validateInput(event, regex, 'סיסמה חייבת להכיל לפחות 4 תווים, כולל אות ומספר');
}

// ---------------------------------------------------------
// ולידציות להוספת אירוע (Add Event)
// ---------------------------------------------------------

// כותרת אירוע: אותיות, מספרים, רווחים וסימני פיסוק בסיסיים, 3-60 תווים
export function validateEventTitle(event) {
    const regex = /^[\u0590-\u05FFa-zA-Z0-9\s\.,'-]{3,60}$/;
    validateInput(event, regex, 'כותרת חייבת להכיל 3-60 תווים (אותיות ומספרים בלבד)');
}

// ---------------------------------------------------------
// ולידציות להוספת מיקום (Add location)
// ---------------------------------------------------------
// עיר: אותיות בעברית או באנגלית, רווחים, בין 2 ל-50 תווים
export function validateLocationCity(event) {
    const regex = /^[\u0590-\u05FFa-zA-Z\s]{2,50}$/;
    validateInput(event, regex, 'יש להזין עיר תקינה (אותיות בלבד, לפחות 2 תווים)');
}
// רחוב: אותיות בעברית או באנגלית, רווחים, בין 2 ל-50 תווים
export function validateLocationStreet(event) {
    const regex = /^[\u0590-\u05FFa-zA-Z\s]{2,50}$/;
    validateInput(event, regex, 'יש להזין רחוב תקין (אותיות בלבד, לפחות 2 תווים)');
}
// מספר בית: מספרים בלבד, בין 1 ל-5 תווים
export function validateLocationHouseNumber(event) {
    const regex = /^\d{1,5}$/;
    validateInput(event, regex, 'יש להזין מספר בית תקין (מספרים בלבד)');
}
// קומה: מספרים בלבד, בין 1 ל-3 תווים (לא חובה)
export function validateLocationFloor(event) {
    const regex = /^\d{1,3}$/;
    validateInput(event, regex, 'יש להזין קומה תקינה (מספרים בלבד)');
}