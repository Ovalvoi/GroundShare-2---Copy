const API_BASE_URL = 'https://localhost:7057/api';

// טוען את כל האירועים כשדף הבית נטען
document.addEventListener('DOMContentLoaded', () => {
    loadAllEvents()
})

// שליפת כל האירועים מהשרת בשיטת then כמו בדוגמה של המרצה
function loadAllEvents() {

    const container = document.querySelector('#eventsContainer')

    if (!container) {
        return
    }

    // מצב טעינה
    container.innerHTML = `
        <div class="text-center py-5">
            <div class="spinner-border" role="status"></div>
            <p class="mt-3 mb-0">טוען את רשימת האירועים</p>
        </div>
    `

    fetch(API_BASE_URL + '/Events/all', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => {
            if (!res.ok) {
                throw new Error('שגיאה בטעינת האירועים מהשרת')
            }
            return res.json()
        })
        .then(events => {
            renderEventsList(events)
        })
        .catch(() => {
            container.innerHTML = `
                <div class="alert alert-danger mt-4 mb-0" role="alert">
                    לא ניתן לטעון את רשימת האירועים כרגע,
                    אנא נסה שוב מאוחר יותר
                </div>
            `
        })
}

// מציירת רשימה של כרטיסי אירועים
function renderEventsList(events) {

    const container = document.querySelector('#eventsContainer')

    if (!container) {
        return
    }

    container.innerHTML = ''

    if (!events || events.length === 0) {
        container.innerHTML = `
            <div class="text-center py-5">
                <p class="mb-0">לא נמצאו אירועים להצגה</p>
            </div>
        `
        return
    }

    events.forEach(ev => {
        const card = createEventCard(ev)
        container.appendChild(card)
    })
}

// יוצרת כרטיס DOM אחד מאובייקט אירוע בודד
function createEventCard(event) {

    const startDate = event.startDateTime
        ? new Date(event.startDateTime).toLocaleString('he-IL')
        : 'לא ידוע'

    const endDate = event.endDateTime
        ? new Date(event.endDateTime).toLocaleString('he-IL')
        : 'לא ידוע'

    const locationText = [
        event.city,
        event.street && event.houseNumber
            ? `${event.street} ${event.houseNumber}`
            : event.street || ''
    ]
        .filter(x => x && x.trim() !== '')
        .join(', ')

    const avgRating = typeof event.avgRating === 'number'
        ? event.avgRating.toFixed(1)
        : '0.0'

    const ratingCount = event.ratingCount || 0

    const photoUrl =
        event.photoUrl && event.photoUrl.trim() !== ''
            ? event.photoUrl
            : 'https://placehold.co/600x400/cccccc/000000?text=No+Image'

    const wrapper = document.createElement('div')
    wrapper.className = 'col-12 col-md-6 col-lg-4'

    wrapper.innerHTML = `
        <div class="card shadow-sm h-100">
            <img src="${photoUrl}" class="card-img-top" alt="תמונת אירוע">

            <div class="card-body d-flex flex-column">
                <h5 class="card-title mb-2">
                    ${event.eventsType || 'אירוע'}
                </h5>

                <p class="card-text mb-2 text-muted">
                    ${event.description || 'ללא תיאור'}
                </p>

                <p class="card-text mb-1">
                    <strong>סטטוס,</strong>
                    ${event.eventsStatus || 'לא ידוע'}
                </p>

                <p class="card-text mb-1">
                    <strong>מועצה / עירייה,</strong>
                    ${event.municipality || 'לא ידוע'}
                </p>

                <p class="card-text mb-1">
                    <strong>גורם אחראי,</strong>
                    ${event.responsibleBody || 'לא ידוע'}
                </p>

                <p class="card-text mb-1">
                    <strong>מיקום,</strong>
                    ${locationText || 'לא ידוע'}
                </p>

                <p class="card-text mb-1">
                    <strong>התחלה,</strong>
                    ${startDate}
                </p>

                <p class="card-text mb-2">
                    <strong>סיום משוער,</strong>
                    ${endDate}
                </p>

                <p class="card-text mb-3">
                    <strong>דירוג ממוצע,</strong>
                    ${avgRating}
                    <span class="text-warning ms-1">
                        <i class="fa-solid fa-star"></i>
                    </span>
                    <span class="text-muted small">
                        (${ratingCount} דירוגים)
                    </span>
                </p>
            </div>
        </div>
    `
    return wrapper
}