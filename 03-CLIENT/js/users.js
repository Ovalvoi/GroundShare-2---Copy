async function handleLogin(e) {
    e.preventDefault();
    
    const email = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;

    const loginData = {
        email: email,
        password: password,
        // Empty fields to satisfy model
        firstName: "", lastName: "", dateOfBirth: "2000-01-01", phoneNumber: "", address: ""
    };

    try {
        const response = await fetch(`${API_URL}/Users/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(loginData)
        });

        if (response.ok) {
            const user = await response.json();
            currentUser = user;
            localStorage.setItem("groundShareUser", JSON.stringify(user));
            
            Swal.fire({
                icon: 'success',
                title: 'התחברת בהצלחה!',
                timer: 1500,
                showConfirmButton: false
            });

            updateNav();
            showSection("feedSection");
            renderEvents();
        } else {
            Swal.fire({ icon: 'error', title: 'שגיאה', text: 'אימייל או סיסמה שגויים' });
        }
    } catch (error) {
        console.error("Login Error:", error);
        Swal.fire({ icon: 'error', title: 'שגיאה', text: 'תקלה בתקשורת עם השרת' });
    }
}

async function handleRegister(e) {
    e.preventDefault();

    const newUser = {
        firstName: document.getElementById("regFirst").value,
        lastName: document.getElementById("regLast").value,
        email: document.getElementById("regEmail").value,
        password: document.getElementById("regPass").value,
        dateOfBirth: document.getElementById("regDob").value,
        phoneNumber: document.getElementById("regPhone").value,
        address: document.getElementById("regAddress").value
    };

    try {
        const response = await fetch(`${API_URL}/Users/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newUser)
        });

        if (response.ok) {
            Swal.fire({ icon: 'success', title: 'נרשמת בהצלחה!', text: 'כעת ניתן להתחבר' });
            showSection("loginSection");
        } else if (response.status === 409) {
            Swal.fire({ icon: 'warning', title: 'שגיאה', text: 'האימייל כבר קיים במערכת' });
        } else {
            Swal.fire({ icon: 'error', title: 'שגיאה', text: 'משהו השתבש בהרשמה' });
        }
    } catch (error) {
        console.error(error);
    }
}