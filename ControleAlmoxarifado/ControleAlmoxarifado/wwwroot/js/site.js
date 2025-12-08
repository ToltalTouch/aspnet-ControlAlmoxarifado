document.addEventListener("DOMContentLoaded", function() {
    const entryForm = document.getElementById("entryForm");
    const exitForm = document.getElementById("exitForm");

    if (entryForm) {
        entryForm.addEventListener("submit", function(event) {
            event.preventDefault();
            const formData = new FormData(entryForm);
            fetch('/Inventory/Entry', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                alert(data.message);
                entryForm.reset();
            })
            .catch(error => console.error('Error:', error));
        });
    }

    if (exitForm) {
        exitForm.addEventListener("submit", function(event) {
            event.preventDefault();
            const formData = new FormData(exitForm);
            fetch('/Inventory/Exit', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                alert(data.message);
                exitForm.reset();
            })
            .catch(error => console.error('Error:', error));
        });
    }
});