/* =========================================================================
   main.js - Comportamiento común del sitio público:
   navbar (Bootstrap), tema, login / registro con JWT, "Mis compras"
   Requiere: Bootstrap, SweetAlert2, api.js
   ========================================================================= */

const ICONOS = {
    logo: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" aria-hidden="true"><rect x="2" y="2" width="20" height="20" rx="2.5"/><path d="M7 2v20M17 2v20M2 12h20M2 7h5M2 17h5M17 17h5M17 7h5"/></svg>',
    sol: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><circle cx="12" cy="12" r="5"/><path d="M12 1v2M12 21v2M4.2 4.2l1.4 1.4M18.4 18.4l1.4 1.4M1 12h2M21 12h2M4.2 19.8l1.4-1.4M18.4 5.6l1.4-1.4"/></svg>',
    luna: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8z"/></svg>',
    usuario: '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>'
};

const PAGINAS = [
    { href: 'cartelera.html', texto: 'Cartelera' },
    { href: 'estrenos.html', texto: 'Próximos estrenos' },
    { href: 'promociones.html', texto: 'Promociones' }
];

const Sesion = {
    alLoguearse: null, // callback opcional (ej. reserva.html continúa la compra)

    pedirLogin(callback) {
        this.alLoguearse = callback || null;
        abrirModal('loginModal');
    }
};

document.addEventListener('DOMContentLoaded', () => {
    renderNavbar();
    inyectarModalesAuth();
    renderAuthArea();

    // index.html?login=1 abre el login (lo usa admin.html cuando no hay sesión)
    if (new URLSearchParams(location.search).get('login') === '1') abrirModal('loginModal');
});

/* ---------------------------------------------------------------------
   NAVBAR de Bootstrap (se dibuja dentro de <header id="navbarCine">)
   --------------------------------------------------------------------- */
function renderNavbar() {
    const header = document.getElementById('navbarCine');
    if (!header) return;

    const pagina = location.pathname.split('/').pop() || 'index.html';
    const transparente = header.hasAttribute('data-transparente');

    header.innerHTML = `
        <nav class="navbar navbar-expand-lg fixed-top navbar-cine" aria-label="Principal">
            <div class="container">
                <a class="navbar-brand d-flex align-items-center gap-2 fw-bold" href="index.html">
                    <span class="logo-mark">${ICONOS.logo}</span><span>Cines <b>Hollywood</b></span>
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menuPrincipal"
                        aria-controls="menuPrincipal" aria-expanded="false" aria-label="Abrir menú">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="menuPrincipal">
                    <ul class="navbar-nav me-auto mb-2 mb-lg-0" id="navLinks">
                        ${PAGINAS.map(p => `
                            <li class="nav-item">
                                <a class="nav-link ${p.href === pagina ? 'active' : ''}" ${p.href === pagina ? 'aria-current="page"' : ''} href="${p.href}">${p.texto}</a>
                            </li>`).join('')}
                    </ul>
                    <div class="d-flex flex-wrap align-items-center gap-2 pb-2 pb-lg-0" id="authArea"></div>
                </div>
            </div>
        </nav>`;

    // En el inicio la barra es transparente hasta que se scrollea
    if (transparente) {
        const nav = header.querySelector('nav');
        const actualizar = () => nav.classList.toggle('navbar-transparente', window.scrollY < 40);
        window.addEventListener('scroll', actualizar, { passive: true });
        actualizar();
    }
}

/* ---------------------------------------------------------------------
   ZONA DE LOGIN EN LA NAVBAR
   --------------------------------------------------------------------- */
function renderAuthArea() {
    const zona = document.getElementById('authArea');
    if (!zona) return;

    const s = Auth.sesion;
    const btnTema = `<button class="btn btn-outline-secondary btn-sm" id="themeToggle" title="Cambiar tema" aria-label="Cambiar tema">${Tema.actual() === 'dark' ? ICONOS.sol : ICONOS.luna}</button>`;

    if (!s) {
        zona.innerHTML = `${btnTema}
            <button class="btn btn-outline-secondary btn-sm" id="btnLogin">Iniciar sesión</button>
            <button class="btn btn-primary btn-sm" id="btnRegister">Registrarse</button>`;
        document.getElementById('btnLogin').addEventListener('click', () => Sesion.pedirLogin());
        document.getElementById('btnRegister').addEventListener('click', () => abrirModal('registerModal'));
    } else {
        zona.innerHTML = `${btnTema}
            <span class="usuario-nav d-inline-flex align-items-center gap-1 fw-semibold small text-truncate" style="max-width:180px" title="${esc(s.email)}">${ICONOS.usuario}${esc(s.nombre)}</span>
            ${s.idCliente ? '<button class="btn btn-outline-secondary btn-sm" id="btnMisCompras">Mis compras</button>' : ''}
            <button class="btn btn-outline-secondary btn-sm" id="btnLogout">Salir</button>`;
        document.getElementById('btnLogout').addEventListener('click', () => {
            Auth.cerrar();
            toast('Cerraste sesión.');
            renderAuthArea();
            document.dispatchEvent(new Event('sesion-cambiada'));
        });
        document.getElementById('btnMisCompras')?.addEventListener('click', abrirMisCompras);
    }

    document.getElementById('themeToggle').addEventListener('click', () => { Tema.alternar(); renderAuthArea(); });

    // Link al panel solo para administradores
    const links = document.getElementById('navLinks');
    links?.querySelector('.item-admin')?.remove();
    if (Auth.esAdmin() && links) {
        links.insertAdjacentHTML('beforeend', '<li class="nav-item item-admin"><a class="nav-link nav-link-admin" href="admin.html">Panel de administrador</a></li>');
    }
}

/* ---------------------------------------------------------------------
   MODALES DE LOGIN / REGISTRO / MIS COMPRAS (Bootstrap)
   --------------------------------------------------------------------- */
function inyectarModalesAuth() {
    document.body.insertAdjacentHTML('beforeend', `
    <div class="modal fade" id="loginModal" tabindex="-1" aria-labelledby="loginTitulo" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <form id="loginForm" novalidate>
                    <div class="modal-header">
                        <h2 class="modal-title fs-4" id="loginTitulo">Iniciar sesión</h2>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-danger py-2" id="loginError" hidden></div>
                        <div class="mb-3">
                            <label class="form-label" for="loginEmail">Correo electrónico</label>
                            <input type="email" class="form-control" id="loginEmail" autocomplete="username" placeholder="ejemplo@correo.com" required>
                        </div>
                        <div class="mb-3">
                            <label class="form-label" for="loginPassword">Contraseña</label>
                            <input type="password" class="form-control" id="loginPassword" autocomplete="current-password" required>
                        </div>
                        <button type="submit" class="btn btn-primary w-100">Ingresar</button>
                        <p class="text-center text-body-secondary small mt-3 mb-0">
                            ¿No tenés cuenta? <button type="button" class="btn btn-link btn-sm p-0 align-baseline" id="irARegistro">Registrate</button>
                        </p>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <div class="modal fade" id="registerModal" tabindex="-1" aria-labelledby="regTitulo" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <form id="registerForm" novalidate>
                    <div class="modal-header">
                        <h2 class="modal-title fs-4" id="regTitulo">Crear cuenta</h2>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-danger py-2" id="registerError" hidden></div>
                        <div class="row g-3">
                            <div class="col-sm-6">
                                <label class="form-label" for="regNombre">Nombre</label>
                                <input type="text" class="form-control" id="regNombre" autocomplete="given-name" required>
                            </div>
                            <div class="col-sm-6">
                                <label class="form-label" for="regApellido">Apellido</label>
                                <input type="text" class="form-control" id="regApellido" autocomplete="family-name" required>
                            </div>
                            <div class="col-sm-6">
                                <label class="form-label" for="regDni">DNI</label>
                                <input type="text" class="form-control" id="regDni" inputmode="numeric" required>
                            </div>
                            <div class="col-sm-6">
                                <label class="form-label" for="regNacimiento">Fecha de nacimiento</label>
                                <input type="date" class="form-control" id="regNacimiento">
                            </div>
                            <div class="col-12">
                                <label class="form-label" for="regEmail">Correo electrónico</label>
                                <input type="email" class="form-control" id="regEmail" autocomplete="email" required>
                            </div>
                            <div class="col-sm-6">
                                <label class="form-label" for="regPassword">Contraseña</label>
                                <input type="password" class="form-control" id="regPassword" autocomplete="new-password" minlength="6" required>
                                <div class="form-text">Mínimo 6 caracteres</div>
                            </div>
                            <div class="col-sm-6">
                                <label class="form-label" for="regConfirmPassword">Repetir contraseña</label>
                                <input type="password" class="form-control" id="regConfirmPassword" autocomplete="new-password" required>
                            </div>
                        </div>
                        <button type="submit" class="btn btn-primary w-100 mt-4">Registrarme</button>
                        <p class="text-center text-body-secondary small mt-3 mb-0">
                            ¿Ya tenés cuenta? <button type="button" class="btn btn-link btn-sm p-0 align-baseline" id="irALogin">Iniciá sesión</button>
                        </p>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <div class="modal fade" id="misComprasModal" tabindex="-1" aria-labelledby="mcTitulo" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h2 class="modal-title fs-4" id="mcTitulo">Mis compras</h2>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <div class="modal-body" id="misComprasBody"></div>
            </div>
        </div>
    </div>`);

    document.getElementById('irARegistro').addEventListener('click', () => { cerrarModal('loginModal'); abrirModal('registerModal'); });
    document.getElementById('irALogin').addEventListener('click', () => { cerrarModal('registerModal'); abrirModal('loginModal'); });
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

    // Validaciones en el cliente (la API las vuelve a validar)
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
            <div class="table-responsive"><table class="table table-hover align-middle">
                <thead><tr><th>N°</th><th>Fecha</th><th class="text-end">Entradas</th><th>Pago</th><th class="text-end">Total</th><th></th></tr></thead>
                <tbody>${ventas.map(v => `
                    <tr>
                        <td>#${v.idVenta}</td>
                        <td>${fmtFechaHora(v.fechaHora)}</td>
                        <td class="text-end">${v.cantidadEntradas}</td>
                        <td>${esc(v.formasPago)}</td>
                        <td class="text-end">${fmtMoneda(v.total)}</td>
                        <td><button class="btn btn-link btn-sm" data-venta="${v.idVenta}">Ver detalle</button></td>
                    </tr>`).join('')}
                </tbody>
            </table></div>
            <div id="misComprasDetalle"></div>`;

        body.querySelectorAll('[data-venta]').forEach(b => b.addEventListener('click', async () => {
            const cont = document.getElementById('misComprasDetalle');
            cont.innerHTML = htmlCargando();
            try {
                cont.innerHTML = htmlDetalleVenta(await api(`/venta/${b.dataset.venta}`));
            } catch (err) {
                cont.innerHTML = htmlVacio(err.message);
            }
        }));
    } catch (err) {
        body.innerHTML = htmlVacio(err.message);
    }
}
