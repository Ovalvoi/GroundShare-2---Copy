// --- GET & DISPLAY ---
async function renderEvents() {
    const container = document.getElementById("eventsContainer");
    
    // Add a random query param to prevent caching
    const timestamp = new Date().getTime(); 

    try {
        const response = await fetch(`${API_URL}/Events?t=${timestamp}`);
        if (!response.ok) throw new Error("Failed to fetch");
        
        const events = await response.json();
        container.innerHTML = "";

        if(events.length === 0) {
            container.innerHTML = `<h4 class="text-center text-muted">אין אירועים להציג כרגע</h4>`;
            return;
        }

        events.forEach(ev => {
            let imgUrl = 'https://via.placeholder.com/400x200?text=No+Image';
            
            if (ev.photoUrl) {
                if (ev.photoUrl.startsWith('http')) {
                    imgUrl = ev.photoUrl;
                } else {
                    const serverRoot = API_URL.replace('/api', '');
                    imgUrl = `${serverRoot}/${ev.photoUrl}`;
                }
            }
            
            const fullAddress = `${ev.city}, ${ev.street} ${ev.houseNumber || ''}`;

            // --- GENERATE STARS HTML ---
            const starHtml = getStarsHtml(ev.avgRating);
            const ratingText = ev.ratingCount > 0 
                ? `<span class="text-warning fw-bold me-2">${ev.avgRating.toFixed(1)}</span> <span class="text-muted small">(${ev.ratingCount} דירוגים)</span>`
                : `<span class="text-muted small">אין דירוגים עדיין</span>`;
            // --------------------------------

            const cardHtml = `
            <div class="col-md-4">
                <div class="card event-card h-100 shadow-sm border-0 rounded-3">
                    <span class="status-badge status-${ev.eventsStatus}">${ev.eventsStatus}</span>
                    <img src="${imgUrl}" class="event-img-top" alt="${ev.eventsType}" onerror="this.src='https://via.placeholder.com/400x200?text=Error';">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center mb-2">
                            <h5 class="card-title fw-bold mb-0 text-primary">${ev.eventsType}</h5>
                            <small class="text-muted"><i class="fas fa-map-marker-alt"></i> ${fullAddress}</small>
                        </div>
                        
                        <!-- RATING SECTION -->
                        <div class="mb-2 d-flex align-items-center">
                            <div class="me-2">${starHtml}</div>
                            <div>${ratingText}</div>
                        </div>

                        <p class="card-text text-secondary">${ev.description}</p>
                        
                        <div class="small text-muted mb-3 border-top pt-2">
                            <div><i class="fas fa-calendar-alt text-primary"></i> התחלה: ${new Date(ev.startDateTime).toLocaleDateString()}</div>
                            <div><i class="fas fa-building text-info"></i> רשות: ${ev.municipality || 'לא צוין'}</div>
                        </div>
                    </div>
                    <div class="card-footer bg-white border-0 pb-3">
                        <div class="d-grid gap-2">
                            <button class="btn btn-outline-warning text-dark fw-bold rounded-pill btn-sm" onclick="openRatingModal(${ev.eventsId})">
                                <i class="far fa-star"></i> דרג אירוע
                            </button>
                            <button class="btn btn-outline-secondary rounded-pill btn-sm" onclick="openReviewsModal(${ev.eventsId})">
                                <i class="fas fa-comments"></i> קרא תגובות
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            `;
            container.innerHTML += cardHtml;
        });

    } catch (error) {
        console.error(error);
        container.innerHTML = `<div class="alert alert-danger">שגיאה בטעינת הנתונים</div>`;
    }
}

// HELPER: Generate Star Icons
function getStarsHtml(rating) {
    let stars = '';
    for (let i = 1; i <= 5; i++) {
        if (i <= Math.round(rating)) {
            stars += '<i class="fas fa-star text-warning"></i>';
        } else {
            stars += '<i class="far fa-star text-muted"></i>';
        }
    }
    return stars;
}

// --- NEW: REVIEWS LOGIC ---
async function openReviewsModal(eventId) {
    const reviewsList = document.getElementById("reviewsList");
    reviewsList.innerHTML = `<div class="text-center py-3"><div class="spinner-border text-secondary" role="status"></div></div>`;
    
    // Open Modal immediately
    const modal = new bootstrap.Modal(document.getElementById('reviewsModal'));
    modal.show();

    try {
        // Fetch reviews from server
        // Requires GET endpoint: api/Ratings/event/{id}
        const response = await fetch(`${API_URL}/Ratings/event/${eventId}`);
        
        if (!response.ok) {
            // Graceful fallback if endpoint doesn't exist yet
            if(response.status === 404) {
                 reviewsList.innerHTML = "<li class='list-group-item text-center text-muted'>עדיין לא ניתן לצפות בתגובות (חסר Endpoint בשרת)</li>";
                 return;
            }
            throw new Error("Failed to fetch reviews");
        }
        
        const reviews = await response.json();
        reviewsList.innerHTML = "";
        
        if (reviews.length === 0) {
            reviewsList.innerHTML = "<li class='list-group-item text-center text-muted py-4'>עדיין אין תגובות לאירוע זה. היה הראשון לדרג!</li>";
        } else {
            reviews.forEach(r => {
                const stars = getStarsHtml(r.overallScore); 
                // Format date nicely
                const date = new Date(r.createdAt).toLocaleDateString('he-IL');
                
                reviewsList.innerHTML += `
                    <li class="list-group-item">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <span class="small text-warning">${stars}</span>
                            <small class="text-muted" style="font-size: 0.8em;">${date}</small>
                        </div>
                        <p class="mb-0 text-dark small">${r.comment ? r.comment : '<i class="text-muted">ללא הערה מילולית</i>'}</p>
                    </li>
                `;
            });
        }
    } catch (error) {
        console.error(error);
        reviewsList.innerHTML = `<li class="list-group-item text-danger text-center">שגיאה בטעינת הנתונים</li>`;
    }
}

// --- KEEP YOUR EXISTING handleAddEvent CODE BELOW ---
async function handleAddEvent(e) {
    e.preventDefault();

    const fileInput = document.getElementById("eventImage");
    const file = fileInput.files[0];

    if (!file) {
        Swal.fire({ icon: 'warning', title: 'שים לב', text: 'חובה להעלות תמונה' });
        return;
    }

    // 1. Upload Image First
    const formData = new FormData();
    formData.append("file", file);

    try {
        Swal.fire({ title: 'מעלה תמונה...', didOpen: () => Swal.showLoading() });
        
        const uploadRes = await fetch(`${API_URL}/Events/upload`, {
            method: "POST",
            body: formData
        });

        if (!uploadRes.ok) {
            const errorText = await uploadRes.text();
            throw new Error(`שרת דחה את הקובץ: ${uploadRes.status} - ${errorText}`);
        }
        
        const uploadData = await uploadRes.json();
        const photoPath = uploadData.path; 

        // 2. Create Event Object
        const newEvent = {
            eventsType: document.getElementById("eventType").value,
            startDateTime: document.getElementById("eventStart").value,
            endDateTime: document.getElementById("eventEnd").value || null,
            locationsId: parseInt(document.getElementById("eventLocation").value),
            description: document.getElementById("eventDesc").value,
            municipality: document.getElementById("eventMuni").value,
            responsibleBody: document.getElementById("eventBody").value,
            eventsStatus: document.getElementById("eventStatus").value,
            photoUrl: photoPath,
            // WORKAROUND: Server validation requires these strings to be non-null.
            city: "",
            street: "",
            houseNumber: ""
        };

        // 3. Send Event Data
        const eventRes = await fetch(`${API_URL}/Events`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newEvent)
        });

        if (eventRes.ok) {
            Swal.fire({ 
                icon: 'success', 
                title: 'האירוע נוצר בהצלחה!', 
                timer: 1500, 
                showConfirmButton: false 
            });

            await new Promise(resolve => setTimeout(resolve, 1500));
            
            document.getElementById("addEventForm").reset();
            showSection("feedSection");
            await renderEvents();
        } else {
            const errText = await eventRes.text();
            throw new Error("Failed to save event data: " + errText);
        }

    } catch (error) {
        console.error("DEBUG ERROR:", error);
        Swal.fire({ icon: 'error', title: 'שגיאה', text: error.message });
    }
}