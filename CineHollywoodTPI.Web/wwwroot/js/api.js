const API_URL = 'http://localhost:5191/api';

const Auth = {
    CLAVE: 'cine_sesion',

    get sesion() {
        try {
            const s = JSON.parse(localStorage.getItem(this.CLAVE));
            if (!s || !s.token) return null;
            if (s.expira && new Date(s.expira) <= new Date()) {
                localStorage.removeItem(this.CLAVE);
                return null;
            }
            return s;
        } catch {
            return null;
        }
    },

    guardar(respuestaLogin) { localStorage.setItem(this.CLAVE, JSON.stringify(respuestaLogin)); },
    cerrar() { localStorage.removeItem(this.CLAVE); },

    get token() { return this.sesion?.token ?? null; },
    estaLogueado() { return !!this.sesion; },
    esAdmin() { return this.sesion?.rol === 'Administrador'; }
};

async function api(ruta, { method = 'GET', body } = {}) {
    const esArchivo = body instanceof FormData;
    const headers = { 'Accept': 'application/json' };
    if (body !== undefined && !esArchivo) headers['Content-Type'] = 'application/json';
    if (Auth.token) headers['Authorization'] = `Bearer ${Auth.token}`;

    const esperas = method === 'GET' ? [500, 1000, 2000, 3000, 4000, 5000] : [];
    let resp;
    for (let intento = 0; ; intento++) {
        try {
            resp = await fetch(`${API_URL}${ruta}`, {
                method,
                headers,
                body: body === undefined ? undefined : (esArchivo ? body : JSON.stringify(body))
            });
            break;
        } catch {
            if (intento >= esperas.length)
                throw new Error('No se pudo conectar con la API. ¿Está corriendo el proyecto CineHollywoodTPI.API?');
            await new Promise(r => setTimeout(r, esperas[intento]));
        }
    }

    const texto = await resp.text();
    let datos = null;
    try { datos = texto ? JSON.parse(texto) : null; } catch { datos = texto; }

    if (resp.ok) return datos;

    if (resp.status === 401 && Auth.estaLogueado()) {
        Auth.cerrar();
        throw new Error('Tu sesión expiró. Volvé a iniciar sesión.');
    }
    if (resp.status === 403 && !datos) throw new Error('No tenés permisos para realizar esta acción.');

    throw new Error(mensajeDeError(datos) || `Error ${resp.status}`);
}

function mensajeDeError(datos) {
    if (!datos) return null;
    if (typeof datos === 'string') return datos;
    if (datos.errors) return Object.values(datos.errors).flat().join(' ');
    return datos.mensaje || datos.title || null;
}

const fmtMoneda = n => (Number(n) || 0).toLocaleString('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 });
const htmlPrecio = (precio, precioSinDescuento) =>
    Number(precioSinDescuento) > Number(precio)
        ? `<s class="text-body-secondary fw-normal">${fmtMoneda(precioSinDescuento)}</s> ${fmtMoneda(precio)}`
        : fmtMoneda(precio);
const fmtNumero = n => (Number(n) || 0).toLocaleString('es-AR');

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

function esc(valor) {
    return String(valor ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
}

const API_ORIGEN = new URL(API_URL).origin;
function urlPoster(posterUrl) {
    if (!posterUrl) return null;
    return posterUrl.startsWith('/') ? API_ORIGEN + posterUrl : posterUrl;
}

function hueDesdeTexto(texto) {
    let h = 0;
    for (const c of String(texto)) h = (h * 31 + c.charCodeAt(0)) % 360;
    return h;
}

function posterHTML(pelicula, ribbon = '') {
    const titulo = esc(pelicula.titulo);
    const generado = `<div class="poster-generado" style="--h:${hueDesdeTexto(pelicula.titulo)}"><span>${titulo}</span></div>`;
    const src = urlPoster(pelicula.posterUrl);
    const img = src
        ? `<img src="${esc(src)}" alt="Póster de ${titulo}" loading="lazy" onerror="this.remove()">`
        : '';
    const cinta = ribbon ? `<span class="poster-ribbon badge rounded-pill text-bg-primary">${esc(ribbon)}</span>` : '';
    return `<div class="poster">${generado}${img}${cinta}</div>`;
}

function badgeClasificacion(desc) {
    if (!desc) return '<span class="badge text-bg-secondary">Sin calificar</span>';
    const clase = desc === 'ATP' ? 'text-bg-success' : (desc.includes('18') || desc.includes('16') ? 'text-bg-danger' : 'text-bg-warning');
    return `<span class="badge ${clase}">${esc(desc)}</span>`;
}

const ICONO_SWAL = { success: 'success', error: 'error', warning: 'warning', info: 'info' };

function opcionesSwal() {
    const cs = getComputedStyle(document.documentElement);
    return {
        target: document.querySelector('.modal.show') ?? 'body',
        background: cs.getPropertyValue('--cine-surface').trim(),
        color: cs.getPropertyValue('--bs-body-color').trim(),
        confirmButtonColor: cs.getPropertyValue('--cine-primary').trim(),
        cancelButtonColor: cs.getPropertyValue('--bs-tertiary-bg').trim()
    };
}

function toast(mensaje, tipo = 'info', ms = 4000) {
    if (!window.Swal) { alert(mensaje); return; }
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
}

async function alerta(titulo, texto = '', tipo = 'info') {
    if (!window.Swal) { alert(`${titulo}\n${texto}`); return; }
    await Swal.fire({ ...opcionesSwal(), icon: ICONO_SWAL[tipo] ?? 'info', title: titulo, html: esc(texto).replace(/\n/g, '<br>'), confirmButtonText: 'Aceptar' });
}

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
        confirmButtonColor: peligroso ? '#dc3545' : opciones.confirmButtonColor,
        customClass: peligroso ? { confirmButton: 'swal-peligro' } : {},
        reverseButtons: true,
        focusCancel: peligroso
    });
    return r.isConfirmed;
}

function abrirModal(id) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(id)).show();
}
function cerrarModal(id) {
    bootstrap.Modal.getOrCreateInstance(document.getElementById(id)).hide();
}
document.addEventListener('shown.bs.modal', e => {
    e.target.querySelector('input:not([type=hidden]):not([disabled]), select:not([disabled]), textarea')?.focus();
});

const Tema = {
    actual() { return localStorage.getItem('cine_tema') || 'dark'; },
    aplicar(tema) {
        document.documentElement.setAttribute('data-bs-theme', tema);
        localStorage.setItem('cine_tema', tema);
        document.dispatchEvent(new CustomEvent('tema-cambiado', { detail: tema }));
    },
    alternar() { this.aplicar(this.actual() === 'dark' ? 'light' : 'dark'); }
};
document.documentElement.setAttribute('data-bs-theme', Tema.actual());

const htmlCargando = (texto = 'Cargando...') =>
    `<div class="col-12 text-center text-body-secondary py-4">
        <div class="spinner-border text-primary mb-2" role="status"></div>
        <div>${esc(texto)}</div>
    </div>`;

const htmlVacio = texto =>
    `<div class="col-12"><div class="text-center text-body-secondary border border-dashed rounded-3 py-5 px-3">${esc(texto)}</div></div>`;

function htmlDetalleVenta(v) {
    return `
        <h5 class="mt-3">Venta #${v.idVenta} · ${fmtFechaHora(v.fechaHora)}</h5>
        <p class="text-body-secondary">${esc(v.cliente ?? '')} · ${esc(v.canal ?? '')} · ${esc(v.formasPago ?? '')}</p>
        <div class="table-responsive"><table class="table table-sm align-middle tabla-apilable">
            <thead><tr><th>Película</th><th>Función</th><th>Sala</th><th>Butaca</th><th>Tipo</th><th class="text-end">Precio</th></tr></thead>
            <tbody>${v.entradas.map(e => `
                <tr>
                    <td data-label="Película" class="fw-semibold">${esc(e.pelicula)}</td>
                    <td data-label="Función">${fmtFechaCorta(e.fecha)} ${fmtHora(e.horaInicio)}</td>
                    <td data-label="Sala">${esc(e.sala)}</td>
                    <td data-label="Butaca">${esc(e.butaca)}</td>
                    <td data-label="Tipo">${esc(e.tipoEspectador)}</td>
                    <td data-label="Precio" class="text-end">${fmtMoneda(e.precioFinal)}</td>
                </tr>`).join('')}
            </tbody>
            <tfoot><tr><th colspan="5">Total</th><th class="text-end">${fmtMoneda(v.total)}</th></tr></tfoot>
        </table></div>`;
}

async function verificarSesion() {
    if (!Auth.estaLogueado()) return;
    try {
        await api('/auth/yo');
    } catch {
        if (!Auth.estaLogueado()) document.dispatchEvent(new Event('sesion-expirada'));
    }
}
document.addEventListener('DOMContentLoaded', verificarSesion);

const FORMATO_FECHA = 'd-m-Y';

function parseFechaTexto(texto, formato) {
    if (formato !== FORMATO_FECHA) return flatpickr.parseDate(texto, formato);
    const m = /^\s*(\d{1,2})[-\/.](\d{1,2})[-\/.](\d{4})\s*$/.exec(texto || '');
    if (!m) return undefined;
    const [d, mes, a] = [Number(m[1]), Number(m[2]), Number(m[3])];
    const fecha = new Date(a, mes - 1, d);
    return fecha.getDate() === d && fecha.getMonth() === mes - 1 ? fecha : undefined;
}

function campoFecha(id) {
    const input = typeof id === 'string' ? document.getElementById(id) : id;
    if (!input || typeof flatpickr === 'undefined') return null;
    if (input._flatpickr) return input._flatpickr;

    const fp = flatpickr(input, {
        locale: 'es',
        dateFormat: 'Y-m-d',
        altInput: true,
        altFormat: FORMATO_FECHA,
        allowInput: true,
        parseDate: parseFechaTexto,
        static: true,
        disableMobile: true
    });

    const visible = fp.altInput;
    visible.id = `${input.id}Visible`;
    visible.placeholder = 'dd-mm-aaaa';
    ['title', 'aria-label', 'aria-describedby'].forEach(a => input.hasAttribute(a) && visible.setAttribute(a, input.getAttribute(a)));
    const label = document.querySelector(`label[for="${input.id}"]`);
    if (label) label.htmlFor = visible.id;
    return fp;
}

function activarCamposFecha(raiz = document) {
    raiz.querySelectorAll('input[type="date"]').forEach(campoFecha);
}

function setFecha(id, iso) {
    const fp = campoFecha(id);
    if (!fp) { document.getElementById(id).value = iso || ''; return; }
    if (iso) fp.setDate(iso, false); else fp.clear(false);
}

function setLimitesFecha(id, { min = null, max = null } = {}) {
    const fp = campoFecha(id);
    if (!fp) { const el = document.getElementById(id); el.min = min || ''; el.max = max || ''; return; }
    fp.set('minDate', min);
    fp.set('maxDate', max);
}

function habilitarFecha(id, habilitado) {
    const fp = campoFecha(id);
    document.getElementById(id).disabled = !habilitado;
    if (fp) fp.altInput.disabled = !habilitado;
}

document.addEventListener('reset', e => {
    e.target.querySelectorAll?.('input').forEach(i => i._flatpickr?.clear(false));
});
