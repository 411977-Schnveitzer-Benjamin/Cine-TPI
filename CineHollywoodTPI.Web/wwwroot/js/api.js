/* =========================================================================
   api.js - Configuración, sesión (JWT), llamadas a la API y utilidades
   Se carga en TODAS las páginas antes que cualquier otro script.
   ========================================================================= */

// Dirección de la API (proyecto CineHollywoodTPI.API, perfil "http" o "https")
const API_URL = 'http://localhost:5191/api';

/* ---------------------------------------------------------------------
   SESIÓN: guarda el token JWT y los datos del usuario en localStorage
   --------------------------------------------------------------------- */
const Auth = {
    CLAVE: 'cine_sesion',

    get sesion() {
        try {
            const s = JSON.parse(localStorage.getItem(this.CLAVE));
            if (!s || !s.token) return null;
            // Si el token venció lo descartamos
            if (s.expira && new Date(s.expira) <= new Date()) {
                localStorage.removeItem(this.CLAVE);
                return null;
            }
            return s;
        } catch {
            return null;
        }
    },

    guardar(respuestaLogin) {
        localStorage.setItem(this.CLAVE, JSON.stringify(respuestaLogin));
    },

    cerrar() {
        localStorage.removeItem(this.CLAVE);
    },

    get token() { return this.sesion?.token ?? null; },
    estaLogueado() { return !!this.sesion; },
    esAdmin() { return this.sesion?.rol === 'Administrador'; }
};

/* ---------------------------------------------------------------------
   FETCH a la API: agrega el token, convierte la respuesta y los errores
   --------------------------------------------------------------------- */
async function api(ruta, { method = 'GET', body } = {}) {
    const esArchivo = body instanceof FormData; // subida de archivos (multipart)
    const headers = { 'Accept': 'application/json' };
    if (body !== undefined && !esArchivo) headers['Content-Type'] = 'application/json';
    if (Auth.token) headers['Authorization'] = `Bearer ${Auth.token}`;

    let resp;
    try {
        resp = await fetch(`${API_URL}${ruta}`, {
            method,
            headers,
            body: body === undefined ? undefined : (esArchivo ? body : JSON.stringify(body))
        });
    } catch {
        throw new Error('No se pudo conectar con la API. ¿Está corriendo el proyecto CineHollywoodTPI.API?');
    }

    const texto = await resp.text();
    let datos = null;
    try { datos = texto ? JSON.parse(texto) : null; } catch { datos = texto; }

    if (resp.ok) return datos;

    if (resp.status === 401 && Auth.estaLogueado()) {
        Auth.cerrar();
        throw new Error('Tu sesión expiró. Volvé a iniciar sesión.');
    }
    if (resp.status === 403) throw new Error('No tenés permisos para realizar esta acción.');

    throw new Error(mensajeDeError(datos) || `Error ${resp.status}`);
}

// La API devuelve errores como texto plano o como ProblemDetails ({ title, errors })
function mensajeDeError(datos) {
    if (!datos) return null;
    if (typeof datos === 'string') return datos;
    if (datos.errors) return Object.values(datos.errors).flat().join(' ');
    return datos.mensaje || datos.title || null;
}

/* ---------------------------------------------------------------------
   UTILIDADES DE FORMATO
   --------------------------------------------------------------------- */
const fmtMoneda = n => (Number(n) || 0).toLocaleString('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 });
const fmtNumero = n => (Number(n) || 0).toLocaleString('es-AR');

// "2026-11-20" -> Date local (sin corrimiento por zona horaria)
function parseFecha(iso) {
    if (!iso) return null;
    const [a, m, d] = iso.substring(0, 10).split('-').map(Number);
    return new Date(a, m - 1, d);
}
const fmtFecha = iso => iso ? parseFecha(iso).toLocaleDateString('es-AR') : '-';
const fmtFechaLarga = iso => iso ? parseFecha(iso).toLocaleDateString('es-AR', { weekday: 'long', day: 'numeric', month: 'long' }) : '-';
const fmtFechaCorta = iso => iso ? parseFecha(iso).toLocaleDateString('es-AR', { weekday: 'short', day: '2-digit', month: '2-digit' }) : '-';
const fmtHora = h => h ? h.substring(0, 5) : '-';
const fmtFechaHora = iso => iso ? new Date(iso).toLocaleString('es-AR', { dateStyle: 'short', timeStyle: 'short' }) : '-';
const hoyISO = () => {
    const d = new Date();
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
};

// Evita inyectar HTML con datos que vienen de la base
function esc(valor) {
    return String(valor ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
}

/* ---------------------------------------------------------------------
   PÓSTER: imagen real (PosterUrl) o uno generado con el título
   --------------------------------------------------------------------- */
function hueDesdeTexto(texto) {
    let h = 0;
    for (const c of String(texto)) h = (h * 31 + c.charCodeAt(0)) % 360;
    return h;
}

// Los pósters guardados en el servidor vienen como "/posters/x.jpg": se sirven desde la API
const API_ORIGEN = new URL(API_URL).origin;
function urlPoster(posterUrl) {
    if (!posterUrl) return null;
    return posterUrl.startsWith('/') ? API_ORIGEN + posterUrl : posterUrl;
}

function posterHTML(pelicula, ribbon = '') {
    const titulo = esc(pelicula.titulo);
    const generado = `<div class="poster-generado" style="--h:${hueDesdeTexto(pelicula.titulo)}"><span>${titulo}</span></div>`;
    const src = urlPoster(pelicula.posterUrl);
    const img = src
        ? `<img src="${esc(src)}" alt="Póster de ${titulo}" loading="lazy" onerror="this.remove()">`
        : '';
    const cinta = ribbon ? `<span class="poster-ribbon">${esc(ribbon)}</span>` : '';
    // El generado queda debajo: si la imagen falla se elimina y se ve el generado
    return `<div class="poster">${generado}${img}${cinta}</div>`;
}

function badgeClasificacion(desc) {
    if (!desc) return '<span class="badge">Sin calificar</span>';
    const clase = desc === 'ATP' ? 'badge-success' : (desc.includes('18') || desc.includes('16') ? 'badge-danger' : 'badge-warning');
    return `<span class="badge ${clase}">${esc(desc)}</span>`;
}

/* ---------------------------------------------------------------------
   ALERTAS con SweetAlert2 (https://sweetalert2.github.io)
   Si la librería no cargó (sin internet) se usa un aviso simple.
   --------------------------------------------------------------------- */
const ICONO_SWAL = { success: 'success', error: 'error', warning: 'warning', info: 'info' };

// Colores del tema actual para que las alertas combinen con el modo claro/oscuro
function opcionesSwal() {
    const cs = getComputedStyle(document.documentElement);
    return {
        background: cs.getPropertyValue('--clr-surface').trim(),
        color: cs.getPropertyValue('--clr-text').trim(),
        confirmButtonColor: cs.getPropertyValue('--clr-primary').trim(),
        cancelButtonColor: cs.getPropertyValue('--clr-surface-2').trim()
    };
}

// Notificación chica en la esquina (no bloquea)
function toast(mensaje, tipo = 'info', ms = 4000) {
    if (window.Swal) {
        Swal.fire({
            ...opcionesSwal(),
            toast: true,
            position: 'bottom-end',
            icon: ICONO_SWAL[tipo] ?? 'info',
            title: mensaje,
            showConfirmButton: false,
            timer: ms,
            timerProgressBar: true,
            didOpen: t => {
                t.addEventListener('mouseenter', Swal.stopTimer);
                t.addEventListener('mouseleave', Swal.resumeTimer);
            }
        });
        return;
    }
    toastSimple(mensaje, tipo, ms);
}

// Alerta centrada con botón "Aceptar"
async function alerta(titulo, texto = '', tipo = 'info') {
    if (!window.Swal) { alert(`${titulo}\n${texto}`); return; }
    await Swal.fire({ ...opcionesSwal(), icon: ICONO_SWAL[tipo] ?? 'info', title: titulo, html: esc(texto).replace(/\n/g, '<br>'), confirmButtonText: 'Aceptar' });
}

// Confirmación Sí/No. Devuelve true si el usuario confirma.
async function confirmar(titulo, texto = '', { textoConfirmar = 'Sí, confirmar', peligroso = false } = {}) {
    if (!window.Swal) return confirm(`${titulo}\n${texto}`);
    const opciones = opcionesSwal();
    const r = await Swal.fire({
        ...opciones,
        icon: peligroso ? 'warning' : 'question',
        title: titulo,
        html: esc(texto).replace(/\n/g, '<br>'),
        showCancelButton: true,
        confirmButtonText: textoConfirmar,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: peligroso ? '#ef4444' : opciones.confirmButtonColor,
        customClass: peligroso ? { confirmButton: 'swal-peligro' } : {},
        reverseButtons: true,
        focusCancel: peligroso
    });
    return r.isConfirmed;
}

function toastSimple(mensaje, tipo = 'info', ms = 4000) {
    let cont = document.querySelector('.toast-container');
    if (!cont) {
        cont = document.createElement('div');
        cont.className = 'toast-container';
        cont.setAttribute('role', 'status');
        cont.setAttribute('aria-live', 'polite');
        document.body.appendChild(cont);
    }
    const t = document.createElement('div');
    t.className = `toast toast-${tipo}`;
    t.textContent = mensaje;
    cont.appendChild(t);
    setTimeout(() => t.remove(), ms);
}

/* ---------------------------------------------------------------------
   MODALES
   --------------------------------------------------------------------- */
function abrirModal(id) {
    const m = document.getElementById(id);
    m.classList.add('active');
    m.querySelector('input:not([type=hidden]), select, textarea')?.focus();
}
function cerrarModal(id) {
    document.getElementById(id).classList.remove('active');
}
// Cerrar con Escape o clic en el fondo
document.addEventListener('keydown', e => {
    if (e.key === 'Escape') document.querySelectorAll('.modal.active').forEach(m => m.classList.remove('active'));
});
document.addEventListener('mousedown', e => {
    if (e.target.classList?.contains('modal')) e.target.classList.remove('active');
});
document.addEventListener('click', e => {
    const btn = e.target.closest('[data-cerrar-modal]');
    if (btn) cerrarModal(btn.dataset.cerrarModal);
});

/* ---------------------------------------------------------------------
   TEMA CLARO / OSCURO
   --------------------------------------------------------------------- */
const Tema = {
    actual() { return localStorage.getItem('cine_tema') || 'dark'; },
    aplicar(tema) {
        document.documentElement.dataset.theme = tema;
        localStorage.setItem('cine_tema', tema);
        document.dispatchEvent(new CustomEvent('tema-cambiado', { detail: tema }));
    },
    alternar() { this.aplicar(this.actual() === 'dark' ? 'light' : 'dark'); }
};
document.documentElement.dataset.theme = Tema.actual();

// Mensajes de "cargando" / "vacío" reutilizables
const htmlCargando = (texto = 'Cargando...') => `<div class="cargando"><div class="spinner"></div>${esc(texto)}</div>`;
const htmlVacio = texto => `<div class="estado-vacio">${esc(texto)}</div>`;

/* ---------------------------------------------------------------------
   DETALLE DE UNA VENTA (comprobante, "Mis compras" y panel de admin)
   --------------------------------------------------------------------- */
function htmlDetalleVenta(v) {
    return `
        <h3 style="margin-top:1.5rem">Venta #${v.idVenta} · ${fmtFechaHora(v.fechaHora)}</h3>
        <p class="text-muted">${esc(v.cliente ?? '')} · ${esc(v.canal ?? '')} · ${esc(v.formasPago ?? '')}</p>
        <div class="table-wrap"><table class="table">
            <thead><tr><th>Película</th><th>Función</th><th>Sala</th><th>Butaca</th><th>Tipo</th><th class="num">Precio</th></tr></thead>
            <tbody>${v.entradas.map(e => `
                <tr>
                    <td>${esc(e.pelicula)}</td>
                    <td>${fmtFechaCorta(e.fecha)} ${fmtHora(e.horaInicio)}</td>
                    <td>${esc(e.sala)}</td>
                    <td>${esc(e.butaca)}</td>
                    <td>${esc(e.tipoEspectador)}</td>
                    <td class="num">${fmtMoneda(e.precioFinal)}</td>
                </tr>`).join('')}
            </tbody>
            <tfoot><tr><th colspan="5">Total</th><th class="num">${fmtMoneda(v.total)}</th></tr></tfoot>
        </table></div>`;
}
