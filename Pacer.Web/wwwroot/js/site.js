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
  
    if (minutes == 30) {
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
// Loading buttons
$(document).ready(function() {
  $(".loading-button").click(function() {
      var $this = $(this);

      // Change the button text to the loading GIF
      $this.html('<img src="/images/LoadingGif.gif" width="20px" alt="Loading..." />');
  });
});






