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



