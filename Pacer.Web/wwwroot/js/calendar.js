var workoutTypeNames = {
    0: 'Recovery Run',
    1: 'Easy Run',
    2: 'Long Run',
    3: 'Race Pace',
    4: 'Interval Training',
    5: 'Tempo Run',
    6: 'VO2 Max'
};
// Function to get the day suffix
function getDaySuffix(day) {
    let suffix = '';
    if (day > 3 && day < 21) return 'th';
    switch (day % 10) {
        case 1: return 'st';
        case 2: return 'nd';
        case 3: return 'rd';
        default: return 'th';
    }
}
// Function to handle calendar population
$(function () {
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    var workouts = viewModel.Workouts;
    var raceDate = viewModel.RaceDate.slice(0, 10);


    var workoutMap = {};
    workouts.forEach(w => {
        if (isNaN(Date.parse(w.DateString))) {
            console.error('Invalid date:', w.DateString);
        } else {
            var date = new Date(w.DateString);
            var dateString = date.toISOString().slice(0, 10);
            workoutMap[dateString] = w;
        }
    });

    var calendar = new jsCalendar(document.querySelector('#calendar'));
    var startMonth = viewModel.Month;  // replace with the actual month
    var startYear = viewModel.Year;  // replace with the actual year

    // Code to add classes to dates that have workouts 
    calendar.onDateRender(function (date, element, instance) {
        var localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
        var dateString = localDate.toISOString().slice(0, 10);

        if (dateString === raceDate) {
            element.classList.add('jsCalendar-RaceDay');
        }

        // Check if there's a workout on the date
        if (workoutMap[dateString]) {
            var workout = workoutMap[dateString];
            switch (workout.Type) {
                case 0:
                    element.classList.add('jsCalendar-RecoveryRun');
                    break;
                case 1:
                    element.classList.add('jsCalendar-EasyRun');
                    break;
                case 2:
                    element.classList.add('jsCalendar-LongRun');
                    break;
                case 3:
                    element.classList.add('jsCalendar-MarathonPace');
                    break;
                case 4:
                    element.classList.add('jsCalendar-IntervalTraining');
                    break;
                case 5:
                    element.classList.add('jsCalendar-TempoRun');
                    break;
                case 6:
                    element.classList.add('jsCalendar-VO2Max');
                    break;
                default:
                    break;
            }
        }
    });
    // Choosing today or the date the training plan starts, whichever is later
    var today = new Date();
    var startDate = new Date(startYear, startMonth - 1);
    var laterDate = new Date(Math.max(today, startDate));

    calendar.refresh(laterDate);

    var selectedCell = null;

    calendar.onDateClick(function (event, date) {
        if (selectedCell) {
            selectedCell.classList.remove('selected-date');  // Remove the class from the previously selected cell
        }

        selectedCell = event.target;  // Update the selected cell
        selectedCell.classList.add('selected-date');  // Add the class to the newly selected cell

        var localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
        var dateString = localDate.toISOString().slice(0, 10);

        var workout = workoutMap[dateString];

        // get the day suffix
        var day = localDate.getDate();
        var suffix = getDaySuffix(day);

        var formattedDate = days[localDate.getDay()] + ' ' + day + suffix + ' ' + months[localDate.getMonth()] + ' ' + localDate.getFullYear();
        $('#workout-date').text(formattedDate);

        if (dateString === raceDate) {
            let raceParts = targetRaceDisplayName.split(' ');
            var raceName = '';
            if (raceParts[0] == "Beginner") {
                raceParts.shift();
            }
            raceName = raceParts.join(' ');
            $('#workout-date').text("Race Day!");
            $('#workout-type').text(targetTime + " " + raceName);
            $('#workout-pace').text(targetPace + " min/mile");
            $('#workout-description').text("Run the " + raceName + " at " + targetPace + " per mile. Good luck!");
            $('#entry-form').hide();
            $('#previous-results').hide();
            return;
        }

        if (workout) {
            resetWorkoutInputs();
            // Update the workout details on the page
            var workoutDate = new Date(workout.DateString); // Use DateString here
            var workoutDistance = workout.ActualDistance;

            // get the day suffix
            var day = workoutDate.getDate();
            var suffix = getDaySuffix(day);

            var formattedDate = days[workoutDate.getDay()] + ' ' + day + suffix + ' ' + months[workoutDate.getMonth()] + ' ' + workoutDate.getFullYear();
            $('#workout-date').text(formattedDate);
            $('#workout-type').text(workout.TargetDistance + ' mile ' + workoutTypeNames[workout.Type]);
            $('#workout-pace').text(workout.TargetPace + ' min/mile');
            $('#workout-description').text(workout.WorkoutDescription);
            $('#workoutId').val(workout.Id);
            $('#clearWorkoutId').val(workout.Id);
            $('#actualDistance').val(workout.TargetDistance);

            if (workoutDistance > 0) {
                $('#actualDistance').val(workoutDistance);
                $('#prev-distance').text("Distance Ran: " + workoutDistance + " miles");
                $('#previous-results').show();
                $('#entry-form').hide();
            }
            else {
                $('#previous-results').hide();
                $('#entry-form').show();
                $('#workout-details').removeClass('completed-workout');
            }

            if (workout.ActualTime) {
                var actualTimeParts = workout.ActualTime.split(':');
                var hours = parseInt(actualTimeParts[0]);
                var minutes = parseInt(actualTimeParts[1]);
                var seconds = parseInt(actualTimeParts[2]);

                if (hours > 0 || minutes > 0 || seconds > 0) {
                    $('#actualHours').val(hours);
                    $('#actualMinutes').val(minutes);
                    $('#actualSeconds').val(seconds);

                    $('#prev-time').text("Duration: " + hours + "h " + minutes + "m " + seconds + "s");
                    $('#prev-pace').text("Pace: " + workout.ActualPace + " min/mile");
                    $('#previous-results').show();
                    $('#entry-form').hide();
                    $('#workout-details').addClass('completed-workout');

                }
            }

        }
        else {
            $('#workout-type').text("");
            $('#workout-pace').text("");
            $('#workout-description').text("No workout scheduled for this date.");
            $('#entry-form').hide();
            $('#previous-results').hide();
            $('#workout-details').removeClass('completed-workout');
        }
    });
    // Clearing the form
    function resetWorkoutInputs() {
        $('#actualHours').val(0);
        $('#actualMinutes').val(0);
        $('#actualSeconds').val(0);
        $('#warningField').text('');
    }
    // Ensuring correct data is input
    $(document).ready(function () {
        $('#entry-form').on('submit', function (e) {
            var actualDistance = parseFloat($('#actualDistance').val());
            var actualHours = parseInt($('#actualHours').val());
            var actualMinutes = parseInt($('#actualMinutes').val());
            var actualSeconds = parseInt($('#actualSeconds').val());

            if (actualDistance === 0 || (actualHours === 0 && actualMinutes === 0 && actualSeconds === 0)) {
                warningField.innerText = 'Neither duration or distance can be zero. Please enter valid values.';
                e.preventDefault();
            }
        });
    });

});