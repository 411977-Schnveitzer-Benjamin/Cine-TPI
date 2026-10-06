// Change navbar background on scroll
window.addEventListener('scroll', () => {
    const navbar = document.querySelector('.navbar');
    if (window.scrollY > 50) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});

// Auth buttons basic interaction
document.getElementById('btnLogin').addEventListener('click', () => {
    // In the future this will open a modal or navigate to login page
    alert('Abriendo el modal de Iniciar Sesión...');
});

document.getElementById('btnRegister').addEventListener('click', () => {
    // In the future this will open a modal or navigate to register page
    alert('Abriendo el modal de Registro...');
});
