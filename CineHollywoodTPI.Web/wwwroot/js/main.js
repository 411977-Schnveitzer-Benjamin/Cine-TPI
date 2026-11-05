/* =========================================================================
   main.js - Comportamiento común del sitio público:
   navbar, tema, login / registro con JWT, "Mis compras"
   Requiere: api.js
   ========================================================================= */

const ICONOS = {
    sol: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><circle cx="12" cy="12" r="5"/><path d="M12 1v2M12 21v2M4.2 4.2l1.4 1.4M18.4 18.4l1.4 1.4M1 12h2M21 12h2M4.2 19.8l1.4-1.4M18.4 5.6l1.4-1.4"/></svg>',
    luna: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8z"/></svg>',
    usuario: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>'
};

const Sesion = {
    alLoguearse: null, // callback opcional (ej. reserva.html continúa la compra)

    pedirLogin(callback) {
        this.alLoguearse = callback || null;
        abrirModal('loginModal');
    }
};

document.addEventListener('DOMContentLoaded', () => {
    inyectarModalesAuth();
    iniciarNavbar();
    renderAuthArea();

    // index.html?login=1 abre el login (lo usa admin.html cuando no hay sesión)
    if (new URLSearchParams(location.search).get('login') === '1') abrirModal('loginModal');
});

/* ---------------------------------------------------------------------
   NAVBAR
   --------------------------------------------------------------------- */
function iniciarNavbar() {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;

    // En el inicio la barra es transparente hasta que se scrollea
    const fija = navbar.classList.contains('scrolled');
    if (!fija) {
        const actualizar = () => navbar.classList.toggle('scrolled', window.scrollY > 40);
        window.addEventListener('scroll', actualizar, { passive: true });
        actualizar();
    }

    document.getElementById('mobileMenuBtn')?.addEventListener('click', () => navbar.classList.toggle('menu-open'));

    // Marca el link de la página actual
    const pagina = location.pathname.split('/').pop() || 'index.html';
    document.querySelectorAll('.nav-links .nav-link').forEach(a => {
        if (a.getAttribute('href') === pagina) a.classList.add('active');
    });
}

/* ---------------------------------------------------------------------
   ZONA DE LOGIN EN LA NAVBAR
   --------------------------------------------------------------------- */
function renderAuthArea() {
    const zona = document.getElementById('authArea');
    if (!zona) return;

    const s = Auth.sesion;
    const btnTema = `<button class="btn btn-icon" id="themeToggle" title="Cambiar tema" aria-label="Cambiar tema">${Tema.actual() === 'dark' ? ICONOS.sol : ICONOS.luna}</button>`;

    if (!s) {
        zona.innerHTML = `${btnTema}
            <button class="btn btn-outline btn-sm" id="btnLogin">Iniciar sesión</button>
            <button class="btn btn-primary btn-sm" id="btnRegister">Registrarse</button>`;
        document.getElementById('btnLogin').onclick = () => Sesion.pedirLogin();
        document.getElementById('btnRegister').onclick = () => abrirModal('registerModal');
    } else {
        zona.innerHTML = `${btnTema}
            <span class="user-chip" title="${esc(s.email)}">${ICONOS.usuario}${esc(s.nombre)}</span>
            ${s.idCliente ? '<button class="btn btn-outline btn-sm" id="btnMisCompras">Mis compras</button>' : ''}
            <button class="btn btn-outline btn-sm" id="btnLogout">Salir</button>`;
        document.getElementById('btnLogout').onclick = () => {
            Auth.cerrar();
            toast('Cerraste sesión.');
            renderAuthArea();
            document.dispatchEvent(new Event('sesion-cambiada'));
        };
        document.getElementById('btnMisCompras')?.addEventListener('click', abrirMisCompras);
    }

    document.getElementById('themeToggle').onclick = () => { Tema.alternar(); renderAuthArea(); };

    // Link al panel solo para administradores
    const navLinks = document.querySelector('.nav-links');
    navLinks?.querySelector('.nav-link--admin')?.remove();
    if (Auth.esAdmin() && navLinks) {
        navLinks.insertAdjacentHTML('beforeend', '<a href="admin.html" class="nav-link nav-link--admin">Panel de administrador</a>');
    }
}

/* ---------------------------------------------------------------------
   MODALES DE LOGIN / REGISTRO (se agregan a todas las páginas)
   --------------------------------------------------------------------- */
function inyectarModalesAuth() {
    document.body.insertAdjacentHTML('beforeend', `
    <div class="modal" id="loginModal" role="dialog" aria-modal="true" aria-labelledby="loginTitulo">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title" id="loginTitulo">Iniciar sesión</h2>
                <button class="close-modal" data-cerrar-modal="loginModal" aria-label="Cerrar">&times;</button>
            </div>
            <form id="loginForm" novalidate>
                <div class="form-error" id="loginError" hidden></div>
                <div class="form-group">
                    <label class="form-label" for="loginEmail">Correo electrónico</label>
                    <input type="email" class="form-control" id="loginEmail" autocomplete="username" placeholder="ejemplo@correo.com" required>
                </div>
                <div class="form-group">
                    <label class="form-label" for="loginPassword">Contraseña</label>
                    <input type="password" class="form-control" id="loginPassword" autocomplete="current-password" required>
                </div>
                <button type="submit" class="btn btn-primary btn-block">Ingresar</button>
                <p class="form-switch-text">¿No tenés cuenta? <button type="button" class="link-btn" id="irARegistro">Registrate</button></p>
            </form>
        </div>
    </div>

    <div class="modal" id="registerModal" role="dialog" aria-modal="true" aria-labelledby="regTitulo">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title" id="regTitulo">Crear cuenta</h2>
                <button class="close-modal" data-cerrar-modal="registerModal" aria-label="Cerrar">&times;</button>
            </div>
            <form id="registerForm" novalidate>
                <div class="form-error" id="registerError" hidden></div>
                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label" for="regNombre">Nombre</label>
                        <input type="text" class="form-control" id="regNombre" autocomplete="given-name" required>
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="regApellido">Apellido</label>
                        <input type="text" class="form-control" id="regApellido" autocomplete="family-name" required>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label" for="regDni">DNI</label>
                        <input type="text" class="form-control" id="regDni" inputmode="numeric" required>
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="regNacimiento">Fecha de nacimiento</label>
                        <input type="date" class="form-control" id="regNacimiento">
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label" for="regEmail">Correo electrónico</label>
                    <input type="email" class="form-control" id="regEmail" autocomplete="email" required>
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <label class="form-label" for="regPassword">Contraseña</label>
                        <input type="password" class="form-control" id="regPassword" autocomplete="new-password" minlength="6" required>
                        <span class="form-hint">Mínimo 6 caracteres</span>
                    </div>
                    <div class="form-group">
                        <label class="form-label" for="regConfirmPassword">Repetir contraseña</label>
                        <input type="password" class="form-control" id="regConfirmPassword" autocomplete="new-password" required>
                    </div>
                </div>
                <button type="submit" class="btn btn-primary btn-block">Registrarme</button>
                <p class="form-switch-text">¿Ya tenés cuenta? <button type="button" class="link-btn" id="irALogin">Iniciá sesión</button></p>
            </form>
        </div>
    </div>

    <div class="modal" id="misComprasModal" role="dialog" aria-modal="true" aria-labelledby="mcTitulo">
        <div class="modal-content modal-lg">
            <div class="modal-header">
                <h2 class="modal-title" id="mcTitulo">Mis compras</h2>
                <button class="close-modal" data-cerrar-modal="misComprasModal" aria-label="Cerrar">&times;</button>
            </div>
            <div id="misComprasBody"></div>
        </div>
    </div>`);

    document.getElementById('irARegistro').onclick = () => { cerrarModal('loginModal'); abrirModal('registerModal'); };
    document.getElementById('irALogin').onclick = () => { cerrarModal('registerModal'); abrirModal('loginModal'); };
    document.getElementById('loginForm').addEventListener('submit', onLogin);
    document.getElementById('registerForm').addEventListener('submit', onRegistro);
}

function mostrarError(id, msg) {
    const el = document.getElementById(id);
    el.textContent = msg || '';
    el.hidden = !msg;
}

async function onLogin(e) {
    e.preventDefault();
    const email = document.getElementById('loginEmail').value.trim();
    const password = document.getElementById('loginPassword').value;
    if (!email || !password) return mostrarError('loginError', 'Completá el email y la contraseña.');

    const btn = e.submitter;
    btn.disabled = true;
    try {
        const resp = await api('/auth/login', { method: 'POST', body: { email, password } });
        despuesDeLoguear(resp, 'loginModal', `¡Hola, ${resp.nombre}!`);
        e.target.reset();
        mostrarError('loginError', null);
    } catch (err) {
        mostrarError('loginError', err.message);
    } finally {
        btn.disabled = false;
    }
}

async function onRegistro(e) {
    e.preventDefault();
    const v = id => document.getElementById(id).value.trim();
    const datos = {
        nombre: v('regNombre'),
        apellido: v('regApellido'),
        nroDocumento: v('regDni'),
        fechaNacimiento: v('regNacimiento') || null,
        email: v('regEmail'),
        password: document.getElementById('regPassword').value
    };

    if (!datos.nombre || !datos.apellido || !datos.nroDocumento || !datos.email)
        return mostrarError('registerError', 'Completá todos los campos obligatorios.');
    if (!/^\d{6,}$/.test(datos.nroDocumento))
        return mostrarError('registerError', 'El DNI tiene que tener solo números (mínimo 6).');
    if (!/^\S+@\S+\.\S+$/.test(datos.email))
        return mostrarError('registerError', 'El email no es válido.');
    if (datos.password.length < 6)
        return mostrarError('registerError', 'La contraseña tiene que tener al menos 6 caracteres.');
    if (datos.password !== document.getElementById('regConfirmPassword').value)
        return mostrarError('registerError', 'Las contraseñas no coinciden.');

    const btn = e.submitter;
    btn.disabled = true;
    try {
        const resp = await api('/auth/registro', { method: 'POST', body: datos });
        despuesDeLoguear(resp, 'registerModal', '¡Cuenta creada! Ya estás logueado.');
        e.target.reset();
        mostrarError('registerError', null);
    } catch (err) {
        mostrarError('registerError', err.message);
    } finally {
        btn.disabled = false;
    }
}

function despuesDeLoguear(resp, modalId, mensaje) {
    Auth.guardar(resp);
    cerrarModal(modalId);
    toast(mensaje, 'success');
    renderAuthArea();
    document.dispatchEvent(new Event('sesion-cambiada'));

    if (Sesion.alLoguearse) {
        const cb = Sesion.alLoguearse;
        Sesion.alLoguearse = null;
        cb();
    } else if (Auth.esAdmin() && new URLSearchParams(location.search).get('login') === '1') {
        location.href = 'admin.html';
    }
}

/* ---------------------------------------------------------------------
   MIS COMPRAS (cliente logueado)
   --------------------------------------------------------------------- */
async function abrirMisCompras() {
    const body = document.getElementById('misComprasBody');
    body.innerHTML = htmlCargando();
    abrirModal('misComprasModal');
    try {
        const ventas = await api('/venta');
        if (!ventas.length) {
            body.innerHTML = htmlVacio('Todavía no hiciste ninguna compra.');
            return;
        }
        body.innerHTML = `
            <div class="table-wrap"><table class="table">
                <thead><tr><th>N°</th><th>Fecha</th><th class="num">Entradas</th><th>Pago</th><th class="num">Total</th><th></th></tr></thead>
                <tbody>${ventas.map(v => `
                    <tr>
                        <td>#${v.idVenta}</td>
                        <td>${fmtFechaHora(v.fechaHora)}</td>
                        <td class="num">${v.cantidadEntradas}</td>
                        <td>${esc(v.formasPago)}</td>
                        <td class="num">${fmtMoneda(v.total)}</td>
                        <td><button class="link-btn" data-venta="${v.idVenta}">Ver detalle</button></td>
                    </tr>`).join('')}
                </tbody>
            </table></div>
            <div id="misComprasDetalle"></div>`;

        body.querySelectorAll('[data-venta]').forEach(b => b.onclick = async () => {
            const cont = document.getElementById('misComprasDetalle');
            cont.innerHTML = htmlCargando();
            try {
                cont.innerHTML = htmlDetalleVenta(await api(`/venta/${b.dataset.venta}`));
            } catch (err) {
                cont.innerHTML = htmlVacio(err.message);
            }
        });
    } catch (err) {
        body.innerHTML = htmlVacio(err.message);
    }
}

