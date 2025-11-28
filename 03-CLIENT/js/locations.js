// Fetch and populate the dropdown
async function loadLocationsToSelect() {
    const select = document.getElementById("eventLocation");
    
    // Clear current options except the first "loading/select" one, or reset completely
    select.innerHTML = '<option value="">בחר מיקום...</option>';

    try {
        const response = await fetch(`${API_URL}/Locations`);
        if (!response.ok) throw new Error("Failed");

        const locations = await response.json();
        
        locations.forEach(loc => {
            const option = document.createElement("option");
            option.value = loc.locationsId;
            option.text = `${loc.city} - ${loc.street} ${loc.houseNumber}`;
            select.appendChild(option);
        });

    } catch (error) {
        console.error("Error loading locations:", error);
        select.innerHTML = `<option>שגיאה בטעינת מיקומים</option>`;
    }
}

// Handle the New Location Modal Submit
async function handleSaveLocation(e) {
    e.preventDefault();

    const newLocation = {
        city: document.getElementById("locCity").value,
        street: document.getElementById("locStreet").value,
        houseNumber: document.getElementById("locNum").value,
        houseType: document.getElementById("locType").value,
        floor: document.getElementById("locFloor").value ? parseInt(document.getElementById("locFloor").value) : null
    };

    try {
        const response = await fetch(`${API_URL}/Locations`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newLocation)
        });

        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'המיקום נוסף!', timer: 1500, showConfirmButton: false });
            
            // Close Modal
            const modalEl = document.getElementById('addLocationModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            modalInstance.hide();

            // Clear Form
            document.getElementById("newLocationForm").reset();

            // Reload the dropdown in the main form
            loadLocationsToSelect();
        } else {
            Swal.fire({ icon: 'error', title: 'שגיאה', text: 'לא ניתן לשמור את המיקום' });
        }
    } catch (error) {
        console.error(error);
        Swal.fire({ icon: 'error', title: 'שגיאה', text: 'תקלה בתקשורת' });
    }
}