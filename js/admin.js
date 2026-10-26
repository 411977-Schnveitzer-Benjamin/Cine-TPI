// Custom Modal Alert & Confirm System
window.customAlerts = new Set();
window.showCustomModal = function(message, isConfirm = false, onConfirm = null) {
    if (!isConfirm && window.customAlerts.has(message)) return; 
    if (!isConfirm) window.customAlerts.add(message);

    const overlay = document.createElement('div');
    overlay.className = 'modal';
    overlay.style.zIndex = '99999';

    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';
    modalContent.style.maxWidth = '450px';
    modalContent.style.textAlign = 'center';
    
    const title = document.createElement('h3');
    title.style.marginBottom = '1.5rem';
    title.innerText = message;
    
    const btnContainer = document.createElement('div');
    btnContainer.style.display = 'flex';
    btnContainer.style.gap = '1rem';
    btnContainer.style.justifyContent = 'center';
    
    const btnOk = document.createElement('button');
    btnOk.className = 'btn btn-primary';
    btnOk.innerText = 'Aceptar';
    
    if (isConfirm) {
        const btnCancel = document.createElement('button');
        btnCancel.className = 'btn btn-outline';
        btnCancel.innerText = 'Cancelar';
        btnCancel.onclick = () => {
            overlay.classList.remove('active');
            setTimeout(() => overlay.remove(), 400);
        };
        btnContainer.appendChild(btnOk);
        btnContainer.appendChild(btnCancel);
        
        btnOk.onclick = () => {
            overlay.classList.remove('active');
            setTimeout(() => overlay.remove(), 400);
            if(onConfirm) onConfirm();
        };
    } else {
        btnContainer.appendChild(btnOk);
        btnOk.onclick = () => {
            overlay.classList.remove('active');
            setTimeout(() => {
                overlay.remove();
                window.customAlerts.delete(message);
            }, 400);
        };
    }
    
    modalContent.appendChild(title);
    modalContent.appendChild(btnContainer);
    overlay.appendChild(modalContent);
    document.body.appendChild(overlay);
    
    // Trigger animation
    void overlay.offsetWidth;
    overlay.classList.add('active');
};

// Override native alert globally
window.alert = function(message) {
    window.showCustomModal(message, false);
};

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

    updateDashboardStats();
});

function updateDashboardStats() {
    // Check if elements exist
    const elIngresos = document.getElementById('dash-ingresos');
    const elEntradas = document.getElementById('dash-entradas');
    const elFunciones = document.getElementById('dash-funciones');
    const elSocios = document.getElementById('dash-socios');
    const elChart = document.getElementById('dash-chart');
    
    if (!elIngresos) return;

    // Actual Data Sync
    let activeMovies = 0;
    if (typeof getMovies === 'function') {
        activeMovies = getMovies().filter(m => m.status === 'Cartelera').length;
    }
    
    let vipUsers = 0;
    let totalUsers = 0;
    if (typeof getUsers === 'function') {
        const users = getUsers();
        totalUsers = users.length;
        vipUsers = users.filter(u => u.tipo.includes('VIP')).length;
    }

    // Assign actual exact counts
    elFunciones.innerText = activeMovies;
    elSocios.innerText = vipUsers;

    // Generate realistic related data for sales
    const entradasVendidas = (activeMovies * 50) + (totalUsers * 4);
    const ingresosDia = entradasVendidas * 4500;

    elEntradas.innerText = entradasVendidas.toLocaleString('es-AR');
    elIngresos.innerText = '$' + ingresosDia.toLocaleString('es-AR');

    // Chart Sync (Stable for the day)
    if (elChart) {
        let chartData = JSON.parse(localStorage.getItem('cine_chart_data'));
        if (!chartData || chartData.date !== new Date().toDateString()) {
            chartData = {
                date: new Date().toDateString(),
                heights: [
                    Math.floor(Math.random() * 40) + 30, // Lun
                    Math.floor(Math.random() * 50) + 40, // Mar
                    Math.floor(Math.random() * 30) + 20, // Mie
                    Math.floor(Math.random() * 70) + 30, // Jue
                    Math.floor(Math.random() * 80) + 40, // Vie
                    Math.floor(Math.random() * 90) + 50, // Sab
                    100 // Dom is usually peak
                ]
            };
            localStorage.setItem('cine_chart_data', JSON.stringify(chartData));
        }

        const displayDays = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
        elChart.innerHTML = displayDays.map((day, i) => 
            `<div class="bar-group"><div class="bar" style="height: ${chartData.heights[i]}%;"></div><span class="bar-label">${day}</span></div>`
        ).join('');
    }
}

// Make it globally available so admin.html can call it when items are deleted/saved
window.updateDashboardStats = updateDashboardStats;

