document.getElementById('request-geo-button').addEventListener('click', function () {
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(function (position) {
            document.getElementById('latitude').value = position.coords.latitude;
            document.getElementById('longitude').value = position.coords.longitude;

            // Enable the "Proceed with Geolocation Data" button
            document.getElementById('geo-button').disabled = false;

        }, function (error) {
            console.error('Error obtaining geolocation: ', error);
        });
    } else {
        console.error('Geolocation not available in this browser.');
    }
});

document.getElementById('location-form').addEventListener('submit', function (event) {
    var location = document.getElementById('location').value;
    var latitude = document.getElementById('latitude').value;
    var longitude = document.getElementById('longitude').value;

    if (location) {
        event.preventDefault(); // prevent the default form submission
        window.location.href = `/Weather/WeatherByLocation?location=${location}`;
    }
    // If latitude and longitude are available, it will use the form's default action, i.e., post to Weather/Weather
    else if (!(latitude && longitude)) {
        console.error('No location data available.');
        event.preventDefault(); // prevent the form submission if no data available
    }
});
