window.manifestInterop = {
    getManifest: async function () {
        const response = await fetch('/manifest.webmanifest?k=3');        
        if (!response.ok) {
            throw new Error('Manifest not found');
        }
        return await response.json();
    },
    initDatePicker: function (elementId) {
        flatpickr("#" + elementId, {
            locale: "it",
            dateFormat: "Y-m-d",
            maxDate: "today",
            disableMobile: "true", // Force custom picker even on mobile for consistent UI
            theme: "airbnb",
            allowInput: true,
            onChange: function (selectedDates, dateStr, instance) {
                let element = document.getElementById(elementId);
                element.value = dateStr;
                element.dispatchEvent(new Event('change'));
            }
        });
    }
};