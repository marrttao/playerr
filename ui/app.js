// Create shimmer overlay
let shimmer = document.querySelector('.shimmer-bg');
if (!shimmer) {
  shimmer = document.createElement('div');
  shimmer.className = 'shimmer-bg';
  document.body.appendChild(shimmer);
}

// Smooth shimmer movement with requestAnimationFrame
let targetX = 50, targetY = 50;
let currentX = 50, currentY = 50;
function animateShimmer() {
  currentX += (targetX - currentX) * 0.08;
  currentY += (targetY - currentY) * 0.08;
  shimmer.style.setProperty('--shimmer-x', currentX + 'vw');
  shimmer.style.setProperty('--shimmer-y', currentY + 'vh');
  requestAnimationFrame(animateShimmer);
}
animateShimmer();

document.addEventListener('mousemove', function(e) {
  targetX = Math.round((e.clientX / window.innerWidth) * 100);
  targetY = Math.round((e.clientY / window.innerHeight) * 100);
});
document.addEventListener('mouseleave', function() {
  targetX = 50;
  targetY = 50;
});
// Interactive background movement (linear gradient angle)
document.addEventListener('mousemove', function(e) {
  const x = e.clientX / window.innerWidth;
  const y = e.clientY / window.innerHeight;
  // Angle from 90deg (left) to 270deg (right)
  const angle = 90 + (x * 180);
  document.body.style.setProperty('--angle', angle + 'deg');
  document.body.classList.add('bg-move');
});
document.addEventListener('mouseleave', function() {
  document.body.style.setProperty('--angle', '135deg');
});
const loginForm = document.getElementById('login-form');
const registerForm = document.getElementById('register-form');
const toRegister = document.getElementById('to-register');
const toLogin = document.getElementById('to-login');
const formWrapper = document.querySelector('.form-wrapper');

function setWrapperHeight(form) {
  // Get the height of the form and set it to the wrapper
  formWrapper.style.height = form.offsetHeight + 'px';
}

function showRegister() {
  setWrapperHeight(registerForm);
  formWrapper.classList.remove('show');
  formWrapper.classList.add('hide');
  setTimeout(() => {
    loginForm.style.display = 'none';
    registerForm.style.display = 'block';
    setWrapperHeight(registerForm);
    formWrapper.classList.remove('hide');
    formWrapper.classList.add('show');
  }, 600);
}

function showLogin() {
  setWrapperHeight(loginForm);
  formWrapper.classList.remove('show');
  formWrapper.classList.add('hide');
  setTimeout(() => {
    registerForm.style.display = 'none';
    loginForm.style.display = 'block';
    setWrapperHeight(loginForm);
    formWrapper.classList.remove('hide');
    formWrapper.classList.add('show');
  }, 600);
}

toRegister.addEventListener('click', (e) => {
  e.preventDefault();
  showRegister();
});

toLogin.addEventListener('click', (e) => {
  e.preventDefault();
  showLogin();
});

// Initial state
const psaMessage = document.getElementById('psa-message');
const psaToLogin = document.getElementById('psa-to-login');

loginForm.style.display = 'block';
registerForm.style.display = 'none';
psaMessage.style.display = 'none';
formWrapper.classList.add('show');
setWrapperHeight(loginForm);

// Show PSA after registration
registerForm.addEventListener('submit', function(e) {
  e.preventDefault();
  formWrapper.classList.remove('show');
  formWrapper.classList.add('hide');
  setTimeout(() => {
    registerForm.style.display = 'none';
    psaMessage.style.display = 'block';
    setWrapperHeight(psaMessage);
    formWrapper.classList.remove('hide');
    formWrapper.classList.add('show');
  }, 600);
});

psaToLogin.addEventListener('click', function() {
  psaMessage.style.display = 'none';
  showLogin();
});
