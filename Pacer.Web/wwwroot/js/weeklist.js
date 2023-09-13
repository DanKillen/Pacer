// Form validation for the submit workout button
function validateForm(workoutId) {

    if (!validateDistance(workoutId)) {
        return false;
    }
    if (!validateDuration(workoutId)) {
        return false;
    }
    return true;


    function validateDistance(workoutId) {
        var distanceField = document.getElementById("actualDistance-" + workoutId);
        var warningField = document.getElementById("distanceWarning-" + workoutId);
        if (distanceField.value == 0) {
            warningField.innerText = 'Actual distance ran must be greater than zero.';
            warningField.style.display = 'inline'; // Show the warning
            distanceField.focus();
            return false;
        } else {
            warningField.style.display = 'none'; // Hide the warning if value is valid
        }
        return true;
    }

    function validateDuration(workoutId) {
        var hours = parseInt(document.getElementById("actualHours-" + workoutId).value);
        var minutes = parseInt(document.getElementById("actualMinutes-" + workoutId).value);
        var seconds = parseInt(document.getElementById("actualSeconds-" + workoutId).value);
        var warningField = document.getElementById("durationWarning-" + workoutId);

        if (hours === 0 && minutes === 0 && seconds === 0) {
            warningField.innerText = 'Duration cannot be zero. Please enter a valid duration.';
            warningField.style.display = 'inline'; // Show the warning
            return false;
        } else {
            warningField.style.display = 'none'; // Hide the warning if value is valid
        }

        return true;
    }
}
// Function to show the week that is passed in
function showWeek(weekNumber) {
    $(".week-content").hide();
    $("#week-" + weekNumber).show();
}
// Function for button to show the previous week
$(document).on('click', '#prevWeek', function () {
    if (currentWeek > 1) {
        currentWeek--;
        showWeek(currentWeek);
    }
});
// Function for button to show the next week
$(document).on('click', '#nextWeek', function () {
    if (currentWeek < totalWeeks) {
        currentWeek++;
        showWeek(currentWeek);
    }
});
// Function that handles whole process of chaning the date of a workout
$(document).ready(function () {
    $('[id^=changeDateModal-]').on('show.bs.modal', function (e) {
        const workoutId = e.target.id.split('-')[1];
        showChangeDateModal(workoutId);
    });

    $('[id^=confirmChange-]').click(function () {
        const workoutId = this.id.split('-')[1];
        const newDate = $(`#newDate-${workoutId}`).val();
        updateWorkoutDate(workoutId, newDate);
    });
    $('.close-modal').click(function () {
        $('.modal').modal('hide');
    });
});
// Function to show the change date modal and populate it with available dates
function showChangeDateModal(workoutId) {
    // Show the loading icon
    $(`#loadingIcon-${workoutId}`).show();

    $.ajax({
        url: '/TrainingPlan/GetAvailableDates',
        method: 'GET',
        data: { workoutId: workoutId },
        success: function (availableDates) {
            availableDates = availableDates.filter(date => date !== window.raceDate);

            // Populate the modal body with available dates
            let options = '';
            availableDates.forEach(function (date) {
                options += `<option value="${date}">${date}</option>`;
            });
            const newBodyHtml = `
                <div>Please choose a new day for this workout:</div>
                <select id="newDate-${workoutId}">${options}</select>
            `;
            $(`#changeDateModal-${workoutId} .modal-body`).html(newBodyHtml);

            // Hide the loading icon
            $(`#loadingIcon-${workoutId}`).hide();
        }
    });
}

// Function to update the workout date on the server
function updateWorkoutDate(workoutId, newDate) {
    $.ajax({
        url: '/TrainingPlan/UpdateWorkoutDate',
        method: 'POST',
        data: { workoutId: workoutId, newDate: newDate },
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                alert('Failed to update date');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error updating workout date:', error);
        }
    });
}

