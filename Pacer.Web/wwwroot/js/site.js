// Purpose: JavaScript for the site.

// Intro Scroll JavaScript
const displaysections = Array.from(document.getElementsByClassName('section'));
let currentSection = 0;

function handleWheel(event) {
  event.preventDefault();
  const delta = event.deltaY;

  // Scroll down
  if (delta > 0 && currentSection < displaysections.length - 1) {
    currentSection++;
  }
  // Scroll up
  else if (delta < 0 && currentSection > 0) {
    currentSection--;
  }

  // Fade in the current section and fade out the others
  displaysections.forEach((section, index) => {
    if (index === currentSection) {
      section.classList.remove('fade-out');
      section.classList.add('fade-in');
    } else {
      section.classList.remove('fade-in');
      section.classList.add('fade-out');
    }
  });
}

if (document.querySelector('#index-page')) {
  window.addEventListener('wheel', handleWheel, { passive: false });
}

if (document.querySelector('#about-page')) {
  window.addEventListener('wheel', handleWheel, { passive: false });
}
// 5k time slider 
var sliderValue = 1800; // Default value if no savedFiveKTimeMinutes
if (typeof savedFiveKTimeMinutes !== 'undefined' && savedFiveKTimeMinutes > 0) {
  sliderValue = (savedFiveKTimeMinutes * 60) + savedFiveKTimeSeconds;
}
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


// Tool Tips
$(function () {
  $('[data-bs-toggle="tooltip"]').tooltip();
});


