// הגדרת כתובת ה-API של הדירוגים
const RATINGS_API_URL = 'https://localhost:7057/api/Ratings';

// ---------------------------------------------------------
// פונקציה להוספת דירוג חדש לאירוע
// מקבלת אובייקט המכיל את נתוני הדירוג (userId, eventId, ציונים וכו')
// ---------------------------------------------------------
function addRating(ratingData) {
    // מחזירה Promise כדי שהפונקציה הקוראת תוכל לחכות לתשובה (async/await)
    return fetch(`${RATINGS_API_URL}/add`, {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json' 
        },
        body: JSON.stringify(ratingData)
    })
    .then(res => {
        if (!res.ok) {
            // אם השרת מחזיר שגיאה, אנו זורקים שגיאה כדי לתפוס אותה ב-catch
            throw new Error('שגיאה בשמירת הדירוג');
        }
        return res.json();
    });
}

// ---------------------------------------------------------
// פונקציה לשליפת כל הדירוגים של אירוע ספציפי
// שימושית להצגת ביקורות במודל או ברשימה
// ---------------------------------------------------------
function getRatingsByEvent(eventId) {
    if (!eventId) return;

    fetch(`${RATINGS_API_URL}/byEvent/${eventId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(res => {
        if (!res.ok) throw new Error('שגיאה בקבלת דירוגים');
        return res.json();
    })
    .then(ratings => {
        // כאן ניתן לממש לוגיקה להצגת הדירוגים במסך
        console.log(`התקבלו ${ratings.length} דירוגים עבור אירוע ${eventId}`, ratings);
        return ratings;
    })
    .catch(err => {
        console.error('Error fetching ratings:', err);
    });
}

// ---------------------------------------------------------
// פונקציית עזר לחישוב ממוצע דירוג
// מקבלת את שלושת הציונים (רעש, תנועה, בטיחות) ומחזירה ממוצע מעוגל
// ---------------------------------------------------------
function calculateAverageScore(noise, traffic, safety) {
    const sum = Number(noise) + Number(traffic) + Number(safety);
    return Math.round(sum / 3);
}