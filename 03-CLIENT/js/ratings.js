document.addEventListener("DOMContentLoaded", () => {
    // Initialize Star Clicks
    document.querySelectorAll(".star-rating i").forEach(star => {
        star.addEventListener("click", handleStarClick);
    });

    // Handle Form Submit
    const ratingForm = document.getElementById("ratingForm");
    if (ratingForm) {
        ratingForm.addEventListener("submit", submitRating);
    }
});

function openRatingModal(eventId) {
    // 1. Check if user is logged in
    if (!currentUser) {
        Swal.fire({ icon: 'warning', title: 'יש להתחבר', text: 'רק משתמשים רשומים יכולים לדרג אירועים' });
        return;
    }

    // 2. Set Event ID in hidden field
    document.getElementById("ratingEventId").value = eventId;

    // 3. Reset Form Visuals
    document.getElementById("ratingForm").reset();
    resetStars();

    // 4. Show Modal
    const modal = new bootstrap.Modal(document.getElementById('ratingModal'));
    modal.show();
}

function handleStarClick(e) {
    const star = e.target;
    const ratingValue = parseInt(star.getAttribute("data-val"));
    const parent = star.parentElement;
    const inputId = parent.getAttribute("data-input");

    // Update Hidden Input
    document.getElementById(inputId).value = ratingValue;

    // Update Visuals (Empty all, then fill up to clicked)
    const stars = parent.querySelectorAll("i");
    stars.forEach((s, index) => {
        if (index < ratingValue) {
            s.classList.remove("far"); // Remove outline
            s.classList.add("fas");    // Add filled
            s.style.color = "#ffc107";
        } else {
            s.classList.remove("fas");
            s.classList.add("far");
            s.style.color = "#ccc";
        }
    });
}

function resetStars() {
    document.querySelectorAll(".star-rating i").forEach(s => {
        s.classList.remove("fas");
        s.classList.add("far");
        s.style.color = "#ccc";
    });
    // Reset hidden inputs
    document.getElementById("rateOverall").value = "";
    document.getElementById("rateNoise").value = "";
    document.getElementById("rateTraffic").value = "";
    document.getElementById("rateSafety").value = "";
}

async function submitRating(e) {
    e.preventDefault();

    // Validate
    const overall = document.getElementById("rateOverall").value;
    if (!overall) {
        Swal.fire({ icon: 'error', title: 'חסר דירוג', text: 'חובה למלא לפחות ציון כללי' });
        return;
    }

    const ratingData = {
        userId: currentUser.userId,
        eventsId: parseInt(document.getElementById("ratingEventId").value),
        overallScore: parseInt(overall),
        noiseScore: parseInt(document.getElementById("rateNoise").value) || 0,
        trafficScore: parseInt(document.getElementById("rateTraffic").value) || 0,
        safetyScore: parseInt(document.getElementById("rateSafety").value) || 0,
        comment: document.getElementById("rateComment").value
    };

    try {
        const response = await fetch(`${API_URL}/Ratings`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(ratingData)
        });

        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'הדירוג נקלט!', timer: 1500, showConfirmButton: false });
            
            // Close Modal
            const modalEl = document.getElementById('ratingModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            modalInstance.hide();
        } else {
            Swal.fire({ icon: 'error', title: 'שגיאה', text: 'לא ניתן לשמור את הדירוג' });
        }
    } catch (error) {
        console.error(error);
        Swal.fire({ icon: 'error', title: 'שגיאה', text: 'תקלה בתקשורת' });
    }
}