// Base URL for API - Change Port if needed (usually 5xxx in VS)
const API_URL = "https://localhost:7057/api"; 

// Global State
let currentUser = null;

document.addEventListener("DOMContentLoaded", () => {
    // Check if user is logged in (mock persistence)
    const storedUser = localStorage.getItem("groundShareUser");
    if (storedUser) {
        currentUser = JSON.parse(storedUser);
        updateNav();
        showSection('feedSection');
        renderEvents(); // From events.js
    } else {
        updateNav();
        showSection('loginSection');
    }

    // --- Event Listeners ---

    // Nav Links
    document.getElementById("logoutBtn").addEventListener("click", logout);
    document.getElementById("goRegister").addEventListener("click", () => showSection("registerSection"));
    document.getElementById("goLogin").addEventListener("click", () => showSection("loginSection"));
    
    // Buttons
    const addEventBtn = document.getElementById("showAddEventBtn");
    if(addEventBtn) addEventBtn.addEventListener("click", () => {
        showSection("addEventSection");
        loadLocationsToSelect(); // From locations.js
    });

    document.getElementById("cancelAddEvent").addEventListener("click", () => showSection("feedSection"));

    // Forms
    document.getElementById("loginForm").addEventListener("submit", handleLogin);
    document.getElementById("registerForm").addEventListener("submit", handleRegister);
    document.getElementById("addEventForm").addEventListener("submit", handleAddEvent);
    
    // NEW: Location Form
    const newLocForm = document.getElementById("newLocationForm");
    if (newLocForm) {
        newLocForm.addEventListener("submit", handleSaveLocation);
    }
});

// Navigation & View Manager
function showSection(sectionId) {
    // Hide all sections
    document.querySelectorAll(".section-view").forEach(el => el.classList.add("d-none"));
    // Show requested
    document.getElementById(sectionId).classList.remove("d-none");
}

function updateNav() {
    const navList = document.getElementById("navLinks");
    const userDisplay = document.getElementById("userInfo");
    const logoutBtn = document.getElementById("logoutBtn");

    navList.innerHTML = "";

    if (currentUser) {
        userDisplay.textContent = `שלום, ${currentUser.firstName}`;
        logoutBtn.classList.remove("d-none");
        
        // Add nav links
        const li = document.createElement("li");
        li.className = "nav-item";
        li.innerHTML = `<a class="nav-link active" href="#" onclick="showSection('feedSection'); renderEvents()">דף הבית</a>`;
        navList.appendChild(li);
    } else {
        userDisplay.textContent = "";
        logoutBtn.classList.add("d-none");
    }
}

function logout() {
    currentUser = null;
    localStorage.removeItem("groundShareUser");
    updateNav();
    showSection("loginSection");
}