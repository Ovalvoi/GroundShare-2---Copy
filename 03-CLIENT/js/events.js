// כתובת הבסיס ל־API, תעדכן פורט לפי מה שרץ אצלך
const API_BASE_URL = 'https://localhost:7057/api'

// כשדף add-event נטען
document.addEventListener('DOMContentLoaded', () => {

    const form = document.querySelector('#addEventForm')
    const toggleNewLocationBtn = document.querySelector('#toggleNewLocationBtn')
    const addLocationBtn = document.querySelector('#addLocationBtn')

    // חיבור טופס יצירת אירוע
    if (form) {
        form.addEventListener('submit', onAddEventSubmit)
    }

    // חיבור כפתור פתיחת / סגירת טופס מיקום חדש
    if (toggleNewLocationBtn) {
        toggleNewLocationBtn.addEventListener('click', toggleNewLocationSection)
    }

    // חיבור כפתור "הוסף מיקום חדש"
    if (addLocationBtn) {
        addLocationBtn.addEventListener('click', onAddLocationClick)
    }

    // טעינת מיקומים קיימים ל־dropdown
    loadLocationsForSelect()
})

/* -------------------------------------------------------
   1, טעינת כל המיקומים מרשימת Locations (spGetAllLocations)
-------------------------------------------------------- */
function loadLocationsForSelect() {

    const select = document.querySelector('#locationSelect')
    const messageDiv = document.querySelector('#addEventMessage')

    if (!select) {
        return
    }

    // מצב טעינה
    select.innerHTML = `
        <option value="">טוען מיקומים מהמערכת</option>
    `
    select.disabled = true

    fetch(API_BASE_URL + '/Locations/all', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => {
            if (!res.ok) {
                throw new Error('שגיאה בטעינת מיקומים')
            }
            return res.json()
        })
        .then(locations => {

            select.innerHTML = `
                <option value="" disabled selected>בחר מיקום מהרשימה</option>
            `

            if (!locations || locations.length === 0) {
                const opt = document.createElement('option')
                opt.value = ''
                opt.textContent = 'לא נמצאו מיקומים במערכת'
                select.appendChild(opt)
                select.disabled = true
                return
            }

            locations.forEach(loc => {
                const option = document.createElement('option')

                // עבודה גם עם camelCase וגם עם PascalCase ליתר ביטחון
                const id = loc.locationsId ?? loc.LocationsId
                const city = loc.city ?? loc.City
                const street = loc.street ?? loc.Street
                const houseNumber = loc.houseNumber ?? loc.HouseNumber
                const houseType = loc.houseType ?? loc.HouseType
                const floor = loc.floor ?? loc.Floor

                option.value = id

                let label = city || ''
                if (street) {
                    label += `, ${street}`
                    if (houseNumber) {
                        label += ` ${houseNumber}`
                    }
                }
                if (houseType) {
                    label += ` (${houseType}`
                    if (floor !== null && floor !== undefined && floor !== '') {
                        label += `, קומה ${floor}`
                    }
                    label += `)`
                }

                option.textContent = label.trim()
                select.appendChild(option)
            })

            select.disabled = false
        })
        .catch(() => {
            select.innerHTML = `
                <option value="">שגיאה בטעינת מיקומים</option>
            `
            select.disabled = true

            if (messageDiv) {
                messageDiv.className = 'text-danger small mb-3'
                messageDiv.textContent = 'לא ניתן לטעון את רשימת המיקומים כרגע, אפשר לנסות שוב מאוחר יותר'
            }
        })
}

/* -------------------------------------------------------
   2, הצגת / הסתרת טופס "מיקום חדש"
-------------------------------------------------------- */
function toggleNewLocationSection() {
    const section = document.querySelector('#newLocationSection')
    if (!section) {
        return
    }

    // הוספה או הסרה של d-none
    if (section.classList.contains('d-none')) {
        section.classList.remove('d-none')
    } else {
        section.classList.add('d-none')
    }
}

/* -------------------------------------------------------
   3, הוספת מיקום חדש ל־DB (spAddLocation, POST)
-------------------------------------------------------- */
function onAddLocationClick() {

    const city = document.querySelector('#newCity')?.value.trim() || ''
    const street = document.querySelector('#newStreet')?.value.trim() || ''
    const houseNumber = document.querySelector('#newHouseNumber')?.value.trim() || ''
    const houseType = document.querySelector('#newBuildingType')?.value || ''
    const floorValue = document.querySelector('#newFloor')?.value.trim() || ''

    const newLocationMessage = document.querySelector('#newLocationMessage')
    const locationSelect = document.querySelector('#locationSelect')

    if (!newLocationMessage || !locationSelect) {
        return
    }

    newLocationMessage.textContent = ''
    newLocationMessage.className = 'small mb-2'

    // ולידציה לפי הטבלה, City, Street, HouseNumber, HouseType חייבים
    if (!city || !street || !houseNumber || !houseType) {
        newLocationMessage.className = 'text-danger small mb-2'
        newLocationMessage.textContent = 'נא למלא עיר, רחוב, מספר בית וסוג מבנה כדי להוסיף מיקום'
        return
    }

    const locationToSend = {
        city: city,
        street: street,
        houseNumber: houseNumber,
        houseType: houseType,
        floor: floorValue === '' ? null : Number(floorValue)
    }

    fetch(API_BASE_URL + '/Locations/add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(locationToSend)
    })
        .then(res => {
            if (!res.ok) {
                throw new Error('שגיאה בהוספת המיקום')
            }
            return res.json()
        })
        .then(data => {
            // מניחים שהשרת מחזיר את המזהה החדש, בצורה:
            //  { locationsId: 123 } או { id: 123 } או אפילו מספר בלבד
            const newId = data.locationsId ?? data.id ?? data

            if (!newId) {
                throw new Error('לא התקבל מזהה מיקום חדש מהשרת')
            }

            // בניית טקסט יפה ל־option
            let label = city
            if (street) {
                label += `, ${street} ${houseNumber}`
            }
            if (houseType) {
                label += ` (${houseType}`
                if (floorValue !== '') {
                    label += `, קומה ${floorValue}`
                }
                label += `)`
            }

            const option = document.createElement('option')
            option.value = newId
            option.textContent = label.trim()

            // מוסיפים ל־select ובוחרים את המיקום החדש
            locationSelect.appendChild(option)
            locationSelect.value = String(newId)

            newLocationMessage.className = 'text-success small mb-2'
            newLocationMessage.textContent = 'המיקום נוסף בהצלחה ונבחר בטופס האירוע'

        })
        .catch(() => {
            newLocationMessage.className = 'text-danger small mb-2'
            newLocationMessage.textContent = 'לא ניתן להוסיף את המיקום כרגע, אפשר לנסות שוב מאוחר יותר'
        })
}

/* -------------------------------------------------------
   4, שליחת טופס אירוע חדש (spCreateEvent, POST)
-------------------------------------------------------- */
function onAddEventSubmit(e) {
    e.preventDefault()
    addEvent()
}

function addEvent() {

    const messageDiv = document.querySelector('#addEventMessage')
    const form = document.querySelector('#addEventForm')

    if (!messageDiv || !form) {
        return
    }

    messageDiv.textContent = ''
    messageDiv.className = 'small mb-3'

    // קריאת שדות
    const locationId = document.querySelector('#locationSelect')?.value || ''
    const title = document.querySelector('#eventTitle')?.value.trim() || ''
    const eventType = document.querySelector('#eventType')?.value || ''
    const startDate = document.querySelector('#startDate')?.value || ''
    const endDate = document.querySelector('#endDate')?.value || ''
    const municipality = document.querySelector('#municipality')?.value.trim() || ''
    const responsibleBody = document.querySelector('#responsibleBody')?.value.trim() || ''
    const eventStatus = document.querySelector('#eventStatus')?.value || ''

    const noiseScore = document.querySelector('#noiseScore')?.value || ''
    const trafficScore = document.querySelector('#trafficScore')?.value || ''
    const safetyScore = document.querySelector('#safetyScore')?.value || ''

    const description = document.querySelector('#description')?.value.trim() || ''

    // ולידציות בסיסיות לפי הטבלה, כל השדות NOT NULL
    if (!locationId || !eventType || !startDate || !municipality || !responsibleBody || !eventStatus || !description) {
        messageDiv.className = 'text-danger small mb-3'
        messageDiv.textContent = 'נא למלא מיקום, סוג אירוע, תאריך התחלה, רשות מקומית, גורם אחראי, סטטוס ותיאור'
        return
    }

    if (!noiseScore || !trafficScore || !safetyScore) {
        messageDiv.className = 'text-danger small mb-3'
        messageDiv.textContent = 'נא למלא את כל דירוגי האירוע, רעש, הפרעה לתנועה ובטיחות'
        return
    }

    // מחברים כותרת לתיאור, כי בטבלת Events אין שדה Title
    const finalDescription = title
        ? `${title} - ${description}`
        : description

    const eventToSend = {
        startDateTime: startDate,              // DATETIME2
        endDateTime: endDate || null,         // יכול להיות null
        eventsType: eventType,                // NVARCHAR(100)
        photoUrl: null,                       // כרגע אין תמונה
        description: finalDescription,        // NVARCHAR(1000)
        municipality: municipality,           // NOT NULL
        responsibleBody: responsibleBody,     // NOT NULL
        eventsStatus: eventStatus,            // 'קרה' / 'קורה' / 'יקרה'
        locationsId: Number(locationId)       // FK למיקום
    }

    fetch(API_BASE_URL + '/Events/create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(eventToSend)
    })
        .then(res => {
            if (!res.ok) {
                throw new Error('שגיאה ביצירת האירוע')
            }
            return res.json()
        })
        .then(data => {
            // כאן האירוע כבר נשמר בדאטה בייס דרך spCreateEvent

            messageDiv.className = 'text-success small mb-3'
            messageDiv.textContent = 'האירוע נוצר בהצלחה במערכת'

            // איפוס הטופס
            form.reset()

            // השארת רשימת מיקומים, החזרת טופס מיקום חדש למצב סגור
            const newLocationSection = document.querySelector('#newLocationSection')
            if (newLocationSection && !newLocationSection.classList.contains('d-none')) {
                newLocationSection.classList.add('d-none')
            }
        })
        .catch(() => {
            messageDiv.className = 'text-danger small mb-3'
            messageDiv.textContent = 'לא ניתן ליצור את האירוע כרגע, אפשר לנסות שוב מאוחר יותר'
        })
}