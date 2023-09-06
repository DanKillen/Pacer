// Purpose: JavaScript for the site.

// 5k Slider
$(function () {
  var slider = $("#slider-range");

  function updateMinutesAndSeconds() {
    var totalSeconds = slider.val();
    var minutes = Math.floor(totalSeconds / 60);
    var seconds = totalSeconds - (minutes * 60);

    $("#FiveKTimeMinutes").val(minutes);
    $("#FiveKTimeSeconds").val(seconds);

    if (totalSeconds == 1800) {
      $("#moreText").show();
    } else {
      $("#moreText").hide();
    }
  }

  function updateSlider() {
    var minutes = Number($("#FiveKTimeMinutes").val());
    var seconds = Number($("#FiveKTimeSeconds").val());

    var totalSeconds = minutes * 60 + seconds;

    if (totalSeconds >= 750 && totalSeconds <= 1800) {
      slider.val(totalSeconds);
    }
  }

  slider.on("input", updateMinutesAndSeconds);

  $("#FiveKTimeMinutes, #FiveKTimeSeconds").on("change", updateSlider);

  // Initialize
  updateMinutesAndSeconds();
});
// Confirm clear run
function confirmClear() {
  return confirm('Are you sure you want to clear this run?');
}
// Tool Tips
$(function () {
  $('[data-bs-toggle="tooltip"]').tooltip();
});


