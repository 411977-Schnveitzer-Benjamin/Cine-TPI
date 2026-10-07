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

const btnLogin = document.getElementById('btnLogin');
if (btnLogin) {
    btnLogin.addEventListener('click', () => loginModal.classList.add('active'));
}

const btnRegister = document.getElementById('btnRegister');
if (btnRegister) {
    btnRegister.addEventListener('click', () => registerModal.classList.add('active'));
}

const closeLogin = document.getElementById('closeLogin');
if (closeLogin) {
    closeLogin.addEventListener('click', () => loginModal.classList.remove('active'));
}

const closeRegister = document.getElementById('closeRegister');
if (closeRegister) {
    closeRegister.addEventListener('click', () => registerModal.classList.remove('active'));
}

// Close modals when clicking outside
window.addEventListener('click', (e) => {
    if (loginModal && e.target === loginModal) {
        loginModal.classList.remove('active');
    }
    if (registerModal && e.target === registerModal) {
        registerModal.classList.remove('active');
    }
});

// Validate passwords match on register
const registerForm = document.getElementById('registerForm');
if (registerForm) {
    registerForm.addEventListener('submit', (e) => {
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
}

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
    
    renderCartIcon();
};

// --- CART LOGIC ---
function renderCartIcon() {
    let authButtons = document.querySelector('.auth-buttons');
    if (!authButtons) return;
    
    let cartBtn = document.getElementById('btnCart');
    const token = localStorage.getItem('token');
    
    if (token) {
        if (!cartBtn) {
            cartBtn = document.createElement('button');
            cartBtn.id = 'btnCart';
            cartBtn.className = 'btn btn-icon';
            cartBtn.title = 'Carrito de Compras';
            cartBtn.innerHTML = `
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path></svg>
                <span id="cartCount" style="position: absolute; top: -5px; right: -5px; background: var(--clr-primary); color: white; border-radius: 50%; padding: 2px 6px; font-size: 0.7rem; font-weight: bold;">0</span>
            `;
            cartBtn.style.position = 'relative';
            cartBtn.addEventListener('click', () => {
                document.getElementById('cartModal').classList.add('active');
                renderCartItems();
            });
            authButtons.appendChild(cartBtn);
        } else {
            cartBtn.style.display = 'inline-flex';
        }
        injectCartModal();
        updateCartBadge();
    } else {
        if (cartBtn) cartBtn.style.display = 'none';
    }
}

function injectCartModal() {
    if (document.getElementById('cartModal')) return;
    const modalHTML = `
    <div class="modal" id="cartModal">
        <div class="modal-content" style="max-width: 600px;">
            <div class="modal-header">
                <h2 class="modal-title">Tu Carrito</h2>
                <button class="close-modal" id="closeCart">&times;</button>
            </div>
            <div id="cartItemsContainer" style="max-height: 400px; overflow-y: auto; margin-bottom: 1rem; padding-right: 1rem;">
                <!-- Cart items -->
            </div>
            <div style="border-top: 1px solid var(--clr-border); padding-top: 1rem;">
                <h3 style="text-align: right; color: var(--clr-primary);">Total: $<span id="cartTotal">0</span></h3>
                <button class="btn btn-primary" style="width: 100%; margin-top: 1rem;" onclick="checkoutCart()">Finalizar Pago</button>
            </div>
        </div>
    </div>
    `;
    document.body.insertAdjacentHTML('beforeend', modalHTML);
    document.getElementById('closeCart').addEventListener('click', () => {
        document.getElementById('cartModal').classList.remove('active');
    });
    
    // Close cart when clicking outside
    window.addEventListener('click', (e) => {
        const cartM = document.getElementById('cartModal');
        if (cartM && e.target === cartM) {
            cartM.classList.remove('active');
        }
    });
}

function getCart() {
    return JSON.parse(localStorage.getItem('cine_cart')) || [];
}

function saveCart(cart) {
    localStorage.setItem('cine_cart', JSON.stringify(cart));
    updateCartBadge();
}

function updateCartBadge() {
    const cart = getCart();
    const badge = document.getElementById('cartCount');
    if (badge) {
        const count = cart.reduce((acc, item) => acc + (item.type === 'reserva' ? 1 : item.qty), 0);
        badge.innerText = count;
        badge.style.display = count > 0 ? 'inline-block' : 'none';
    }
}

function addToCart(item) {
    const cart = getCart();
    if (item.type === 'confiteria') {
        const existing = cart.find(i => i.type === 'confiteria' && i.itemId === item.itemId);
        if (existing) {
            existing.qty += item.qty;
        } else {
            cart.push(item);
        }
    } else {
        cart.push(item);
    }
    saveCart(cart);
    alert('Añadido al carrito con éxito.');
}

window.removeCartItem = function(index) {
    window.showCustomModal('¿Seguro que desea eliminar?', true, () => {
        const cart = getCart();
        cart.splice(index, 1);
        saveCart(cart);
        renderCartItems();
    });
};

window.updateCartConfiteriaQty = function(index, change) {
    const cart = getCart();
    if (cart[index].type === 'confiteria') {
        cart[index].qty += change;
        if (cart[index].qty <= 0) {
            cart.splice(index, 1);
        }
        saveCart(cart);
        renderCartItems();
    }
};

function renderCartItems() {
    const cart = getCart();
    const container = document.getElementById('cartItemsContainer');
    const totalEl = document.getElementById('cartTotal');
    
    if (cart.length === 0) {
        container.innerHTML = '<p style="text-align: center; color: var(--clr-text-muted); padding: 2rem 0;">Tu carrito está vacío.</p>';
        totalEl.innerText = '0';
        return;
    }
    
    let total = 0;
    container.innerHTML = cart.map((item, index) => {
        if (item.type === 'reserva') {
            const subtotal = item.tickets * item.price;
            total += subtotal;
            return `
            <div style="display: flex; justify-content: space-between; align-items: center; background: rgba(0,0,0,0.2); padding: 1rem; border-radius: 8px; margin-bottom: 0.5rem; border: 1px solid var(--clr-border);">
                <div>
                    <h4 style="color: var(--clr-primary); margin-bottom: 0.2rem;">🎬 ${item.movieTitle}</h4>
                    <p style="font-size: 0.9rem; color: var(--clr-text-muted);">${item.tickets} Entrada(s) - Butacas: ${item.seats.length}</p>
                    <p style="font-weight: bold; margin-top: 0.5rem;">$ ${subtotal.toLocaleString('es-AR')}</p>
                </div>
                <button class="btn btn-outline" style="border-color: #ef4444; color: #ef4444;" onclick="removeCartItem(${index})" title="Eliminar entradas">
                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                </button>
            </div>
            `;
        } else if (item.type === 'confiteria') {
            const subtotal = item.qty * item.price;
            total += subtotal;
            return `
            <div style="display: flex; justify-content: space-between; align-items: center; background: rgba(0,0,0,0.2); padding: 1rem; border-radius: 8px; margin-bottom: 0.5rem; border: 1px solid var(--clr-border);">
                <div>
                    <h4 style="color: var(--clr-primary); margin-bottom: 0.2rem;">🍿 ${item.nombre}</h4>
                    <p style="font-weight: bold; margin-top: 0.5rem;">$ ${subtotal.toLocaleString('es-AR')}</p>
                </div>
                <div style="display: flex; align-items: center; gap: 0.5rem;">
                    <div style="display: flex; align-items: center; gap: 1rem; background: var(--clr-bg); border-radius: 20px; padding: 0.2rem;">
                        <button class="btn btn-outline" style="padding: 0; width: 30px; height: 30px; border-radius: 50%; display: flex; align-items: center; justify-content: center;" onclick="updateCartConfiteriaQty(${index}, -1)">-</button>
                        <span style="font-weight: bold; width: 20px; text-align: center;">${item.qty}</span>
                        <button class="btn btn-outline" style="padding: 0; width: 30px; height: 30px; border-radius: 50%; display: flex; align-items: center; justify-content: center;" onclick="updateCartConfiteriaQty(${index}, 1)">+</button>
                    </div>
                </div>
            </div>
            `;
        }
    }).join('');
    
    totalEl.innerText = total.toLocaleString('es-AR');
}

window.checkoutCart = function() {
    const cart = getCart();
    if (cart.length === 0) return alert('El carrito está vacío.');
    
    // Simular flujo de pago (reutilizando el form que vamos a mover aquí si es necesario, o un simple alert)
    const cardHtml = `
        <div id="pagoForm" style="margin-top: 1rem;">
            <hr style="border: 0; border-top: 1px solid var(--clr-border); margin: 1rem 0;">
            <h3 style="margin-bottom: 1rem;">Datos de Pago</h3>
            <div style="display: grid; grid-template-columns: 1fr; gap: 1rem; max-width: 400px; margin: 0 auto;">
                <input type="text" class="form-control" placeholder="Número de Tarjeta" required id="pagoNum">
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                    <input type="text" class="form-control" placeholder="MM/AA" required id="pagoVenc">
                    <input type="text" class="form-control" placeholder="CVC" required id="pagoCvc">
                </div>
                <input type="text" class="form-control" placeholder="Nombre en la Tarjeta" required id="pagoNom">
                <button class="btn btn-primary" style="margin-top: 1rem;" onclick="processPayment()">Confirmar Compra</button>
            </div>
        </div>
    `;
    const container = document.getElementById('cartItemsContainer');
    if (!document.getElementById('pagoForm')) {
        container.insertAdjacentHTML('beforeend', cardHtml);
        container.scrollTop = container.scrollHeight;
    }
}

window.processPayment = function() {
    if(!document.getElementById('pagoNum').value || !document.getElementById('pagoVenc').value || !document.getElementById('pagoCvc').value) {
        return alert('Por favor, complete todos los campos de la tarjeta.');
    }
    alert('¡Pago procesado con éxito! Recibirás los tickets en tu correo.');
    localStorage.removeItem('cine_cart');
    document.getElementById('cartModal').classList.remove('active');
    updateCartBadge();
    window.location.href = 'index.html';
}

if (document.getElementById('btnLogout')) {
    document.getElementById('btnLogout').addEventListener('click', () => {
        localStorage.removeItem('token');
        localStorage.removeItem('id_rol');
        checkAuthState();
        window.location.reload();
    });
}

const loginForm = document.getElementById('loginForm');
if (loginForm) {
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const email = document.getElementById('loginEmail').value;
        const password = document.getElementById('loginPassword').value;
        
        // Simulate login using localStorage if backend doesn't exist
        const users = JSON.parse(localStorage.getItem('cine_users')) || [];
        const user = users.find(u => 
            u.email.toLowerCase() === email.toLowerCase() || 
            u.nombre.toLowerCase() === email.toLowerCase() || 
            (email.toLowerCase() === 'admin' && u.tipo === 'Admin')
        );
        
        if (user) {
            let id_rol = '1';
            if (user.tipo === 'Socio VIP') id_rol = '3';
            if (user.tipo === 'Admin' || user.rol === 'Administrador') id_rol = '2';
            
            localStorage.setItem('token', 'fake-jwt-token-123');
            localStorage.setItem('id_rol', id_rol);
            loginModal.classList.remove('active');
            checkAuthState();
            alert('¡Inicio de sesión exitoso!');
        } else {
            alert('Error: Usuario o contraseña incorrectos');
        }
    });
}

// Ejecutar al cargar la página
checkAuthState();
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
