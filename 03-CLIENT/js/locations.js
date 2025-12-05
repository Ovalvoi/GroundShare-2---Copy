// שימוש ב-var כדי לאפשר טעינה חוזרת במסכים המשלבים מספר קבצים
var API_BASE_URL = 'https://localhost:7057/api';

// ---------------------------------------------------------
// פונקציה לשליפת כל המיקומים מהשרת
// מטרה: למלא את רשימת הבחירה (Select) בדף הוספת אירוע
// ---------------------------------------------------------
function loadLocationsForSelect() {
    const select = document.querySelector('#locationSelect');
    
    // בדיקה שהאלמנט קיים בדף כדי למנוע שגיאות אם אנחנו בדף אחר
    if (!select) return;

    // ביצוע קריאת GET לשרת
    fetch(`${API_BASE_URL}/Locations/all`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(res => {
        if (!res.ok) throw new Error('שגיאה בטעינת מיקומים');
        return res.json();
    })
    .then(locations => {
        // איפוס הרשימה והוספת אופציית ברירת מחדל
        select.innerHTML = '<option value="" disabled selected>בחר מיקום</option>';
        
        // מעבר על כל מיקום שהתקבל ויצירת אופציה (Option) ברשימה
        locations.forEach(loc => {
            const opt = document.createElement('option');
            opt.value = loc.locationsId; // ה-ID של המיקום (נשלח לשרת בבחירה)
            opt.textContent = `${loc.city}, ${loc.street} ${loc.houseNumber}`; // הטקסט שרואה המשתמש
            select.appendChild(opt);
        });
    })
    .catch(err => console.error('Error loading locations:', err));
}

// ---------------------------------------------------------
// פונקציה להוספת מיקום חדש למערכת
// מופעלת בלחיצה על כפתור "הוסף מיקום חדש" בטופס
// ---------------------------------------------------------
function addNewLocation() {
    // שליפת הערכים משדות הטופס של המיקום החדש
    const city = document.querySelector('#newCity').value;
    const street = document.querySelector('#newStreet').value;
    const houseNumber = document.querySelector('#newHouseNumber').value;
    const houseType = document.querySelector('#newBuildingType').value;
    const floor = document.querySelector('#newFloor').value;

    // ולידציה בסיסית - בדיקה שכל שדות החובה מלאים
    if (!city || !street || !houseNumber || !houseType) {
        alert('נא למלא את כל שדות המיקום (עיר, רחוב, מספר וסוג)');
        return;
    }

    // יצירת אובייקט מיקום לשליחה לשרת (תואם למחלקת Location ב-C#)
    const locationData = { 
        city: city, 
        street: street, 
        houseNumber: houseNumber, 
        houseType: houseType, 
        floor: floor ? parseInt(floor) : null // המרה למספר אם קיים, אחרת null
    };

    // שליחת בקשת POST לשרת
    fetch(`${API_BASE_URL}/Locations/add`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(locationData)
    })
    .then(res => {
        if (!res.ok) throw new Error('שגיאה בהוספת מיקום');
        return res.json();
    })
    .then(data => {
        // קבלת ה-ID החדש מהתשובה של השרת
        // תמיכה באות גדולה או קטנה בתשובה (Case Insensitive)
        const newId = data.locationsId || data.LocationsId;
        
        // הוספת המיקום החדש לרשימה הנגללת ובחירתו באופן אוטומטי
        const select = document.querySelector('#locationSelect');
        if (select) {
            const opt = document.createElement('option');
            opt.value = newId;
            opt.textContent = `${city}, ${street} ${houseNumber}`;
            select.appendChild(opt);
            select.value = newId; // סימון המיקום החדש כבחור
        }
        
        // הסתרת טופס המיקום החדש וניקוי השדות למקרה שירצו להוסיף עוד
        document.querySelector('#newLocationSection').classList.add('d-none');
        document.querySelector('#newCity').value = '';
        document.querySelector('#newStreet').value = '';
        document.querySelector('#newHouseNumber').value = '';
        
        alert('מיקום נוסף בהצלחה!');
    })
    .catch(err => {
        console.error(err);
        alert('שגיאה בהוספת מיקום לשרת');
    });
}