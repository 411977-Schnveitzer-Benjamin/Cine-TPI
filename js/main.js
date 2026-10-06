// Change navbar background on scroll
window.addEventListener('scroll', () => {
    const navbar = document.querySelector('.navbar');
    if (window.scrollY > 50) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});

// Auth modals interaction
const loginModal = document.getElementById('loginModal');
const registerModal = document.getElementById('registerModal');

document.getElementById('btnLogin').addEventListener('click', () => {
    loginModal.classList.add('active');
});

document.getElementById('btnRegister').addEventListener('click', () => {
    registerModal.classList.add('active');
});

document.getElementById('closeLogin').addEventListener('click', () => {
    loginModal.classList.remove('active');
});

document.getElementById('closeRegister').addEventListener('click', () => {
    registerModal.classList.remove('active');
});

// Close modals when clicking outside
window.addEventListener('click', (e) => {
    if (e.target === loginModal) {
        loginModal.classList.remove('active');
    }
    if (e.target === registerModal) {
        registerModal.classList.remove('active');
    }
});

// Validate passwords match on register
document.getElementById('registerForm').addEventListener('submit', (e) => {
    const pwd = document.getElementById('regPassword').value;
    const confirmPwd = document.getElementById('regConfirmPassword').value;
    const errorText = document.getElementById('passwordError');
    
    if (pwd !== confirmPwd) {
        e.preventDefault();
        errorText.style.display = 'block';
    } else {
        errorText.style.display = 'none';
        // Here you will handle the API fetch in the future
        e.preventDefault(); // Prevent reload for now
        alert('Validación exitosa. ¡Listo para enviar a la API!');
        registerModal.classList.remove('active');
    }
});

// Auth State and Login Fetch
const checkAuthState = () => {
    const token = localStorage.getItem('token');
    const rol = localStorage.getItem('id_rol');
    
    const btnLogin = document.getElementById('btnLogin');
    const btnRegister = document.getElementById('btnRegister');
    const btnLogout = document.getElementById('btnLogout');
    const navAdminPanel = document.getElementById('navAdminPanel');
    
    if (token) {
        if (btnLogin) btnLogin.style.display = 'none';
        if (btnRegister) btnRegister.style.display = 'none';
        if (btnLogout) btnLogout.style.display = 'inline-flex';
        
        if (rol == '2' && navAdminPanel) {
            navAdminPanel.style.display = 'inline-block';
        }
    } else {
        if (btnLogin) btnLogin.style.display = 'inline-flex';
        if (btnRegister) btnRegister.style.display = 'inline-flex';
        if (btnLogout) btnLogout.style.display = 'none';
        if (navAdminPanel) navAdminPanel.style.display = 'none';
    }
};

checkAuthState();

if (document.getElementById('btnLogout')) {
    document.getElementById('btnLogout').addEventListener('click', () => {
        localStorage.removeItem('token');
        localStorage.removeItem('id_rol');
        checkAuthState();
        window.location.reload();
    });
}

document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            localStorage.setItem('token', data.token);
            localStorage.setItem('id_rol', data.id_rol);
            loginModal.classList.remove('active');
            checkAuthState();
            alert('¡' + data.mensaje + '!');
        } else {
            alert('Error: ' + data.error);
        }
    } catch (error) {
        alert('Error conectando al servidor');
    }
});
// Theme Toggle
const themeToggle = document.getElementById('themeToggle');
const sunIcon = document.getElementById('sunIcon');
const moonIcon = document.getElementById('moonIcon');

// Load saved theme
const savedTheme = localStorage.getItem('cineTheme');
if (savedTheme === 'light') {
    document.documentElement.setAttribute('data-theme', 'light');
    if (sunIcon && moonIcon) {
        sunIcon.style.display = 'none';
        moonIcon.style.display = 'block';
    }
}

if (themeToggle) {
    const toggleTheme = () => {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        if (currentTheme === 'light') {
            document.documentElement.removeAttribute('data-theme');
            localStorage.setItem('cineTheme', 'dark');
            sunIcon.style.display = 'block';
            moonIcon.style.display = 'none';
        } else {
            document.documentElement.setAttribute('data-theme', 'light');
            localStorage.setItem('cineTheme', 'light');
            sunIcon.style.display = 'none';
            moonIcon.style.display = 'block';
        }
    };

    themeToggle.addEventListener('click', (e) => {
        if (!document.startViewTransition) {
            toggleTheme();
            return;
        }
        
        const x = e.clientX;
        const y = e.clientY;
        const endRadius = Math.hypot(Math.max(x, innerWidth - x), Math.max(y, innerHeight - y));
        
        const transition = document.startViewTransition(() => {
            toggleTheme();
        });
        
        transition.ready.then(() => {
            document.documentElement.animate(
                {
                    clipPath: [
                        `circle(0px at ${x}px ${y}px)`,
                        `circle(${endRadius}px at ${x}px ${y}px)`
                    ],
                },
                {
                    duration: 500,
                    easing: "ease-in-out",
                    pseudoElement: "::view-transition-new(root)",
                }
            );
        });
    });
}

// Carousel Logic - 3D Coverflow
const track = document.getElementById('carouselTrack');
const btnPrev = document.getElementById('prevSlide');
const btnNext = document.getElementById('nextSlide');

if (track && btnPrev && btnNext) {
    const items = Array.from(track.children);
    let currentIndex = 2; // Ítem central inicial

    const updateCarousel = () => {
        items.forEach((item, i) => {
            item.className = 'carousel-slide'; // reset class
            
            let offset = i - currentIndex;
            let absOffset = Math.abs(offset);
            
            let scale = 1 - absOffset * 0.15;
            let translateX = offset * 65; 
            let zIndex = 10 - absOffset;
            let opacity = absOffset > 2 ? 0 : 1 - (absOffset * 0.2);
            let filter = absOffset > 0 ? 'blur(3px) brightness(0.5)' : 'blur(0px) brightness(1)';

            item.style.transform = `translateX(${translateX}%) scale(${scale})`;
            item.style.zIndex = zIndex;
            item.style.opacity = opacity;
            item.style.filter = filter;
            
            if (offset === 0) item.classList.add('active');
        });
    };

    updateCarousel();

    btnNext.addEventListener('click', () => {
        if (currentIndex < items.length - 1) {
            currentIndex++;
            updateCarousel();
        }
    });

    btnPrev.addEventListener('click', () => {
        if (currentIndex > 0) {
            currentIndex--;
            updateCarousel();
        }
    });
    
    // Permitir clickear una película del fondo para traerla al frente
    items.forEach((item, i) => {
        item.addEventListener('click', () => {
            if (currentIndex !== i) {
                currentIndex = i;
                updateCarousel();
            }
        });
    });
}
