// js/admin.js

document.addEventListener('DOMContentLoaded', () => {
    // Theme logic
    const themeToggleAdmin = document.getElementById('themeToggleAdmin');
    const sunIcon = document.querySelector('.sun-icon');
    const moonIcon = document.querySelector('.moon-icon');

    // Inicializar tema desde localStorage
    const savedTheme = localStorage.getItem('theme') || 'dark';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeIcons(savedTheme);

    themeToggleAdmin.addEventListener('click', () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const targetTheme = currentTheme === 'light' ? 'dark' : 'light';
        
        // Efecto de barrido (View Transitions API)
        if (!document.startViewTransition) {
            setTheme(targetTheme);
            return;
        }

        document.startViewTransition(() => {
            setTheme(targetTheme);
        });
    });

    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        updateThemeIcons(theme);
    }

    function updateThemeIcons(theme) {
        if (theme === 'light') {
            sunIcon.style.display = 'none';
            moonIcon.style.display = 'block';
        } else {
            sunIcon.style.display = 'block';
            moonIcon.style.display = 'none';
        }
    }

    // Logout logic
    const btnLogout = document.getElementById('adminLogout');
    if (btnLogout) {
        btnLogout.addEventListener('click', () => {
            // Eliminar rol y redirigir
            localStorage.removeItem('id_rol');
            window.location.href = 'index.html';
        });
    }

    // Protect admin page
    const idRol = localStorage.getItem('id_rol');
    if (idRol !== '2') {
        // Redirigir a inicio si no es admin
        window.location.href = 'index.html';
    }
});
