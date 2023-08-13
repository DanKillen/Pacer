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
        var warningField = document.getElementById("durationWarning-" + workoutId); // Assuming you've added a warning field for time

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

function showWeek(weekNumber) {
    $(".week-content").hide();
    $("#week-" + weekNumber).show();
}

function checkForIncompleteWorkouts(weekElement) {
    let totalWorkouts = $(weekElement).find(".list-group-item").length;
    let notFullyCompleteWorkouts = $(weekElement).find(".not-fully-complete").length;

    if (notFullyCompleteWorkouts > totalWorkouts / 2) { // More than half are not fully complete
        alert("You have several workouts that are not fully complete this week. Consider adjusting your race target time to make workouts easier.");
    }
}

$(document).on('click', '#prevWeek', function () {
    if (currentWeek > 1) {
        currentWeek--;
        showWeek(currentWeek);
    }
});

$(document).on('click', '#nextWeek', function () {
    if (currentWeek < totalWeeks) {
        currentWeek++;
        showWeek(currentWeek);
    }
});