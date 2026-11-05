/* =========================================================================
   admin.js - Panel de administrador
   Dashboard (reportes) + ABM Películas / Funciones / Clientes + Ventas
   Requiere: api.js, butacas.js, feather-icons, Chart.js
   ========================================================================= */

const MESES = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
const TITULOS = { dashboard: 'Dashboard', peliculas: 'Películas', funciones: 'Funciones', salas: 'Salas', precios: 'Precios', clientes: 'Clientes', ventas: 'Ventas' };

const Admin = {
    catalogos: { clasificaciones: [], salas: [], formatos: [], promociones: [], tiposEspectador: [], formasPago: [], tiposSala: [] },
    precios: [],
    peliculas: [],
    funciones: [],
    clientes: [],
    ventas: [],
    charts: {},
    datosTop: [],
    datosMensual: []
};

const $ = id => document.getElementById(id);
const refrescarIconos = () => window.feather?.replace({ 'aria-hidden': 'true' });

/* =====================================================================
   INICIO
   ===================================================================== */
document.addEventListener('DOMContentLoaded', async () => {
    if (!Auth.esAdmin()) {
        location.replace('index.html?login=1');
        return;
    }

    const s = Auth.sesion;
    $('perfilNombre').textContent = s.nombre;
    $('perfilAvatar').textContent = (s.nombre || 'A').charAt(0).toUpperCase();

    iniciarLayout();
    iniciarFormularios();
    refrescarIconos();

    try {
        await cargarCatalogos();
    } catch (err) {
        manejarError(err);
    }

    window.addEventListener('hashchange', () => mostrarSeccion(location.hash.slice(1)));
    mostrarSeccion(location.hash.slice(1));
});

function manejarError(err) {
    toast(err.message, 'error', 6000);
    if (!Auth.estaLogueado()) setTimeout(() => location.replace('index.html?login=1'), 1500);
}

function iniciarLayout() {
    $('adminLogout').onclick = () => {
        Auth.cerrar();
        location.href = 'index.html';
    };

    // Tema
    const iconoTema = () => {
        $('themeToggleAdmin').innerHTML = `<i data-feather="${Tema.actual() === 'dark' ? 'sun' : 'moon'}"></i>`;
        refrescarIconos();
    };
    $('themeToggleAdmin').onclick = () => Tema.alternar();
    document.addEventListener('tema-cambiado', () => {
        iconoTema();
        dibujarGraficoTop();
        dibujarGraficoMensual();
    });
    iconoTema();
    // El menú lateral en celulares lo abre/cierra Bootstrap (offcanvas): no hace falta JS propio
}

function mostrarSeccion(nombre) {
    if (!TITULOS[nombre]) nombre = 'dashboard';

    document.querySelectorAll('.admin-section').forEach(s => s.hidden = s.id !== `section-${nombre}`);
    document.querySelectorAll('.sidebar .nav-link').forEach(a => {
        const activo = a.dataset.target === nombre;
        a.classList.toggle('active', activo);
        if (activo) a.setAttribute('aria-current', 'page'); else a.removeAttribute('aria-current');
    });
    $('tituloSeccion').textContent = TITULOS[nombre];
    document.title = `${TITULOS[nombre]} - Panel Cines Hollywood`;

    // En celulares, al elegir una sección se cierra el menú
    bootstrap.Offcanvas.getInstance($('sidebar'))?.hide();

    // Cada vez que se entra a una sección se recargan sus datos
    ({
        dashboard: cargarDashboard,
        peliculas: cargarPeliculas,
        funciones: cargarFunciones,
        salas: cargarSalas,
        precios: cargarPrecios,
        clientes: cargarClientes,
        ventas: cargarVentas
    })[nombre]();
}

async function cargarCatalogos() {
    const [clasificaciones, salas, formatos, promociones, tiposEspectador, formasPago, tiposSala] = await Promise.all([
        api('/catalogo/clasificaciones'),
        api('/catalogo/salas'),
        api('/catalogo/formatos'),
        api('/catalogo/promociones'),
        api('/catalogo/tipos-espectador'),
        api('/catalogo/formas-pago'),
        api('/catalogo/tipos-sala')
    ]);
    Object.assign(Admin.catalogos, { clasificaciones, salas, formatos, promociones, tiposEspectador, formasPago, tiposSala });

    llenarSelect($('precioTipoSala'), tiposSala, t => t.id, t => t.descripcion);
    llenarSelect($('salaTipo'), tiposSala, t => t.id, t => t.descripcion);

    llenarSelect($('peliculaClasificacion'), clasificaciones, c => c.id, c => c.descripcion, 'Sin clasificación');
    llenarSelect($('funcionSala'), salas, s => s.idSala, s => `${s.nombre} (${s.tipoSala ?? ''} · ${s.cantidadButacas} butacas)`);
    llenarSelect($('funcionFormato'), formatos, f => f.id, f => f.descripcion);
    llenarSelect($('funcionPromocion'), promociones, p => p.idPromocion, p => `${p.descripcion} (-${Number(p.porcentajeDescuento)}%)`, 'Sin promoción');
    llenarSelect($('ventaFormaPago'), formasPago, f => f.id, f => f.descripcion);
}

function llenarSelect(select, items, valor, texto, opcionVacia) {
    select.innerHTML = (opcionVacia !== undefined ? `<option value="">${esc(opcionVacia)}</option>` : '') +
        items.map(i => `<option value="${valor(i)}">${esc(texto(i))}</option>`).join('');
}

function filaVacia(colspan, texto) {
    return `<tr><td colspan="${colspan}" class="text-center text-body-secondary py-4">${esc(texto)}</td></tr>`;
}

// Botones de acción de cada fila (botones de Bootstrap con íconos de Feather)
function botonesAccion(id, { editar = true, borrar = true, ver = false } = {}) {
    return `<div class="d-flex justify-content-end gap-1">
        ${ver ? `<button class="btn btn-outline-secondary btn-sm btn-icono" data-ver="${id}" title="Ver detalle" aria-label="Ver detalle"><i data-feather="eye"></i></button>` : ''}
        ${editar ? `<button class="btn btn-outline-secondary btn-sm btn-icono" data-editar="${id}" title="Editar" aria-label="Editar"><i data-feather="edit-2"></i></button>` : ''}
        ${borrar ? `<button class="btn btn-outline-danger btn-sm btn-icono" data-borrar="${id}" title="Dar de baja" aria-label="Dar de baja"><i data-feather="trash-2"></i></button>` : ''}
    </div>`;
}

function errorForm(id, msg) {
    $(id).textContent = msg || '';
    $(id).hidden = !msg;
}

// Ejecuta una acción del formulario deshabilitando el botón mientras tanto
async function enviando(form, accion) {
    const btn = form.querySelector('[type=submit]');
    btn.disabled = true;
    try { await accion(); } finally { btn.disabled = false; }
}

function iniciarFormularios() {
    // Películas
    $('btnNuevaPelicula').onclick = () => abrirFormPelicula();
    $('buscarPelicula').oninput = renderPeliculas;
    $('peliculaPoster').oninput = () => { $('peliculaArchivo').value = ''; previewPoster(); };
    $('peliculaArchivo').onchange = () => { $('peliculaPoster').value = ''; previewPoster(); };
    $('peliculaTitulo').oninput = previewPoster;
    $('btnQuitarPoster').onclick = quitarPoster;
    $('formPelicula').onsubmit = guardarPelicula;
    $('tbodyPeliculas').onclick = e => {
        const ed = e.target.closest('[data-editar]'); if (ed) abrirFormPelicula(Number(ed.dataset.editar));
        const bo = e.target.closest('[data-borrar]'); if (bo) borrarPelicula(Number(bo.dataset.borrar));
    };

    // Funciones
    $('btnNuevaFuncion').onclick = () => abrirFormFuncion();
    $('filtroFuncionPelicula').onchange = renderFunciones;
    $('filtroFuncionPasadas').onchange = renderFunciones;
    $('formFuncion').onsubmit = guardarFuncion;
    $('funcionFecha').oninput = mostrarFechaLeida;
    $('tbodyFunciones').onclick = e => {
        const ed = e.target.closest('[data-editar]'); if (ed) abrirFormFuncion(Number(ed.dataset.editar));
        const bo = e.target.closest('[data-borrar]'); if (bo) borrarFuncion(Number(bo.dataset.borrar));
    };

    // Salas
    $('btnNuevaSala').onclick = () => abrirFormSala();
    $('formSala').onsubmit = guardarSala;
    $('salaFilas').oninput = resumenSala;
    $('salaPorFila').oninput = resumenSala;
    $('tbodySalas').onclick = e => {
        const ed = e.target.closest('[data-editar]'); if (ed) abrirFormSala(Number(ed.dataset.editar));
        const bo = e.target.closest('[data-borrar]'); if (bo) borrarSala(Number(bo.dataset.borrar));
    };

    // Precios
    $('btnNuevoPrecio').onclick = () => abrirFormPrecio();
    $('filtroPreciosHistoricos').onchange = renderPrecios;
    $('precioTipoSala').onchange = mostrarPrecioActual;
    $('formPrecio').onsubmit = guardarPrecio;
    $('tbodyPrecios').onclick = e => {
        const bo = e.target.closest('[data-borrar]'); if (bo) borrarPrecio(Number(bo.dataset.borrar));
    };
    $('preciosVigentes').onclick = e => {
        const b = e.target.closest('[data-nuevo-precio]'); if (b) abrirFormPrecio(Number(b.dataset.nuevoPrecio));
    };

    // Clientes
    $('btnNuevoCliente').onclick = () => abrirFormCliente();
    $('buscarCliente').oninput = renderClientes;
    $('formCliente').onsubmit = guardarCliente;
    $('tbodyClientes').onclick = e => {
        const ed = e.target.closest('[data-editar]'); if (ed) abrirFormCliente(Number(ed.dataset.editar));
        const bo = e.target.closest('[data-borrar]'); if (bo) borrarCliente(Number(bo.dataset.borrar));
    };

    // Ventas
    $('btnNuevaVenta').onclick = abrirNuevaVenta;
    $('buscarVenta').oninput = renderVentas;
    $('ventaFuncion').onchange = cargarButacasVenta;
    $('formVenta').onsubmit = registrarVenta;
    $('tbodyVentas').onclick = e => {
        const ver = e.target.closest('[data-ver]'); if (ver) verVenta(Number(ver.dataset.ver));
    };

    // Dashboard
    $('topDesde').onchange = cargarTop;
    $('topHasta').onchange = cargarTop;
    $('topCantidad').onchange = cargarTop;
    $('mensualAnio').onchange = cargarMensual;
}

/* =====================================================================
   DASHBOARD
   ===================================================================== */
async function cargarDashboard() {
    try {
        const [totales, proximas, anios] = await Promise.all([
            api('/venta/totales'),
            api('/funcion?soloProximas=true'),
            api('/reporte/anios')
        ]);
        $('mRecaudado').textContent = fmtMoneda(totales.totalRecaudado);
        $('mEntradas').textContent = fmtNumero(totales.totalEntradas);
        $('mVentas').textContent = fmtNumero(totales.totalVentas);
        $('mFunciones').textContent = fmtNumero(proximas.length);

        const anioSel = $('mensualAnio').value || String(new Date().getFullYear());
        $('mensualAnio').innerHTML = anios.map(a => `<option value="${a}">${a}</option>`).join('');
        $('mensualAnio').value = anios.map(String).includes(anioSel) ? anioSel : String(anios[0]);
    } catch (err) {
        manejarError(err);
    }
    cargarTop();
    cargarMensual();
}

async function cargarTop() {
    const params = new URLSearchParams({ top: $('topCantidad').value });
    if ($('topDesde').value) params.set('desde', $('topDesde').value);
    if ($('topHasta').value) params.set('hasta', $('topHasta').value);

    if ($('topDesde').value && $('topHasta').value && $('topDesde').value > $('topHasta').value) {
        toast('La fecha "desde" no puede ser mayor que "hasta".', 'warning');
        return;
    }

    try {
        Admin.datosTop = await api(`/reporte/peliculas-mas-vendidas?${params}`);
    } catch (err) {
        manejarError(err);
        Admin.datosTop = [];
    }

    $('tablaTop').innerHTML = Admin.datosTop.length
        ? Admin.datosTop.map((r, i) => `
            <tr>
                <td>${i + 1}</td>
                <td class="fw-semibold">${esc(r.titulo)}</td>
                <td class="num">${fmtNumero(r.cantidadEntradas)}</td>
                <td class="num">${fmtMoneda(r.totalRecaudado)}</td>
            </tr>`).join('')
        : filaVacia(4, 'No hay ventas en el período seleccionado.');

    dibujarGraficoTop();
}

async function cargarMensual() {
    const anio = $('mensualAnio').value || new Date().getFullYear();
    try {
        Admin.datosMensual = await api(`/reporte/ventas-mensuales?anio=${anio}`);
    } catch (err) {
        manejarError(err);
        Admin.datosMensual = [];
    }

    const d = Admin.datosMensual;
    const totalAnio = d.reduce((s, m) => s + m.totalRecaudado, 0);
    const entradasAnio = d.reduce((s, m) => s + m.cantidadEntradas, 0);
    $('mensualResumen').textContent = `${anio}: ${fmtMoneda(totalAnio)} recaudados · ${fmtNumero(entradasAnio)} entradas`;

    $('tablaMensual').innerHTML = d.filter(m => m.cantidadVentas > 0).map(m => `
        <tr>
            <td>${MESES[m.mes - 1]}</td>
            <td class="num">${fmtNumero(m.cantidadVentas)}</td>
            <td class="num">${fmtNumero(m.cantidadEntradas)}</td>
            <td class="num">${fmtMoneda(m.totalRecaudado)}</td>
        </tr>`).join('') || filaVacia(4, `No hay ventas en ${anio}.`);

    dibujarGraficoMensual();
}

function coloresGrafico() {
    const cs = getComputedStyle(document.documentElement);
    // Colores tomados de las variables CSS (cambian con el tema claro/oscuro)
    return {
        primario: cs.getPropertyValue('--cine-primary').trim(),
        texto: cs.getPropertyValue('--bs-secondary-color').trim(),
        grilla: cs.getPropertyValue('--bs-border-color').trim(),
        fuente: 'Inter, system-ui, sans-serif'
    };
}

function opcionesBase(c) {
    return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        font: { family: c.fuente }
    };
}

function dibujarGraficoTop() {
    if (!window.Chart) return; // si no cargó la librería quedan las tablas
    Admin.charts.top?.destroy();
    const c = coloresGrafico();
    const datos = Admin.datosTop;

    Admin.charts.top = new Chart($('chartTop'), {
        type: 'bar',
        data: {
            labels: datos.map(d => d.titulo),
            datasets: [{
                label: 'Entradas vendidas',
                data: datos.map(d => d.cantidadEntradas),
                backgroundColor: c.primario,
                borderRadius: 4,
                maxBarThickness: 28
            }]
        },
        options: {
            ...opcionesBase(c),
            indexAxis: 'y',
            scales: {
                x: { beginAtZero: true, ticks: { color: c.texto, precision: 0 }, grid: { color: c.grilla } },
                y: { ticks: { color: c.texto }, grid: { display: false } }
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => ` ${fmtNumero(ctx.raw)} entradas · ${fmtMoneda(datos[ctx.dataIndex].totalRecaudado)}`
                    }
                }
            }
        }
    });
}

function dibujarGraficoMensual() {
    if (!window.Chart) return;
    Admin.charts.mensual?.destroy();
    const c = coloresGrafico();
    const datos = Admin.datosMensual;

    Admin.charts.mensual = new Chart($('chartMensual'), {
        type: 'bar',
        data: {
            labels: MESES,
            datasets: [{
                label: 'Recaudado',
                data: datos.map(d => d.totalRecaudado),
                backgroundColor: c.primario,
                borderRadius: 4,
                maxBarThickness: 32
            }]
        },
        options: {
            ...opcionesBase(c),
            scales: {
                x: { ticks: { color: c.texto }, grid: { display: false } },
                y: {
                    beginAtZero: true,
                    ticks: { color: c.texto, callback: v => fmtMoneda(v) },
                    grid: { color: c.grilla }
                }
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => {
                            const m = datos[ctx.dataIndex];
                            return ` ${fmtMoneda(m.totalRecaudado)} · ${m.cantidadVentas} ventas · ${m.cantidadEntradas} entradas`;
                        }
                    }
                }
            }
        }
    });
}

/* =====================================================================
   ABM PELÍCULAS
   ===================================================================== */
async function cargarPeliculas() {
    $('tbodyPeliculas').innerHTML = filaVacia(5, 'Cargando...');
    try {
        Admin.peliculas = await api('/pelicula');
        renderPeliculas();
    } catch (err) {
        $('tbodyPeliculas').innerHTML = filaVacia(5, err.message);
        manejarError(err);
    }
}

function renderPeliculas() {
    const q = $('buscarPelicula').value.trim().toLowerCase();
    const lista = Admin.peliculas.filter(p => p.titulo.toLowerCase().includes(q));

    $('tbodyPeliculas').innerHTML = lista.length
        ? lista.map(p => `
            <tr>
                <td><div class="table-poster">${posterHTML(p)}</div></td>
                <td class="fw-semibold">${esc(p.titulo)}</td>
                <td class="num">${p.duracionMinutos ?? '-'} min</td>
                <td>${badgeClasificacion(p.clasificacionDescripcion)}</td>
                <td>${botonesAccion(p.idPelicula)}</td>
            </tr>`).join('')
        : filaVacia(5, q ? 'No hay películas que coincidan con la búsqueda.' : 'No hay películas cargadas.');
    refrescarIconos();
}

// Estado del póster en el formulario
const posterForm = { actual: null, quitar: false, objectUrl: null };

function abrirFormPelicula(id) {
    const p = Admin.peliculas.find(x => x.idPelicula === id);
    $('formPelicula').reset();
    errorForm('errorPelicula', null);
    $('tituloModalPelicula').textContent = p ? 'Editar película' : 'Nueva película';
    $('peliculaId').value = p?.idPelicula ?? '';
    $('peliculaTitulo').value = p?.titulo ?? '';
    $('peliculaDuracion').value = p?.duracionMinutos ?? '';
    $('peliculaClasificacion').value = p?.idClasificacion ?? '';
    posterForm.actual = p?.posterUrl ?? null;
    posterForm.quitar = false;
    previewPoster();
    abrirModal('modalPelicula');
}

// Vista previa: archivo elegido > link pegado > póster actual > generado
function previewPoster() {
    if (posterForm.objectUrl) URL.revokeObjectURL(posterForm.objectUrl);
    posterForm.objectUrl = null;

    const archivo = $('peliculaArchivo').files[0];
    let src = null;
    if (archivo) src = posterForm.objectUrl = URL.createObjectURL(archivo);
    else if ($('peliculaPoster').value.trim()) src = $('peliculaPoster').value.trim();
    else if (!posterForm.quitar) src = posterForm.actual;

    $('peliculaPreview').innerHTML = posterHTML({ titulo: $('peliculaTitulo').value || 'Título', posterUrl: src });
    $('btnQuitarPoster').hidden = !src;
}

function quitarPoster() {
    posterForm.quitar = true;
    $('peliculaArchivo').value = '';
    $('peliculaPoster').value = '';
    previewPoster();
}

async function guardarPelicula(e) {
    e.preventDefault();
    let id = Number($('peliculaId').value) || null;
    const archivo = $('peliculaArchivo').files[0];
    const link = $('peliculaPoster').value.trim();
    const datos = {
        titulo: $('peliculaTitulo').value.trim(),
        duracionMinutos: Number($('peliculaDuracion').value) || null,
        idClasificacion: Number($('peliculaClasificacion').value) || null,
        posterUrl: posterForm.quitar ? null : posterForm.actual // el póster nuevo se sube aparte
    };

    if (!datos.titulo) return errorForm('errorPelicula', 'El título es obligatorio.');
    if (!datos.duracionMinutos || datos.duracionMinutos < 1 || datos.duracionMinutos > 600)
        return errorForm('errorPelicula', 'La duración tiene que estar entre 1 y 600 minutos.');
    if (archivo && archivo.size > 5 * 1024 * 1024)
        return errorForm('errorPelicula', 'La imagen no puede superar los 5 MB.');
    if (link && !/^https?:\/\//i.test(link))
        return errorForm('errorPelicula', 'El link tiene que empezar con http:// o https://');

    await enviando(e.target, async () => {
        // 1) Datos de la película
        try {
            const resp = id
                ? await api(`/pelicula/${id}`, { method: 'PUT', body: { idPelicula: id, ...datos } })
                : await api('/pelicula', { method: 'POST', body: datos });
            if (!id) {
                id = resp.id;
                $('peliculaId').value = id; // si falla el póster, el reintento edita en vez de duplicar
                $('tituloModalPelicula').textContent = 'Editar película';
            }
        } catch (err) {
            return errorForm('errorPelicula', err.message);
        }

        // 2) Póster: se guarda en el servidor (archivo subido o descargado desde el link)
        try {
            if (archivo) {
                const fd = new FormData();
                fd.append('archivo', archivo);
                await api(`/pelicula/${id}/poster`, { method: 'POST', body: fd });
            } else if (link) {
                await api(`/pelicula/${id}/poster-url`, { method: 'POST', body: { url: link } });
            }
        } catch (err) {
            cargarPeliculas();
            return errorForm('errorPelicula', `La película se guardó, pero el póster no: ${err.message}`);
        }

        toast('Película guardada.', 'success');
        cerrarModal('modalPelicula');
        cargarPeliculas();
    });
}

async function borrarPelicula(id) {
    const p = Admin.peliculas.find(x => x.idPelicula === id);
    if (!await confirmar(`¿Dar de baja "${p?.titulo}"?`, 'La película dejará de verse en la cartelera.', { textoConfirmar: 'Sí, dar de baja', peligroso: true })) return;
    try {
        const resp = await api(`/pelicula/${id}`, { method: 'DELETE' });
        toast(resp.mensaje, 'success');
        cargarPeliculas();
    } catch (err) {
        manejarError(err);
    }
}

/* =====================================================================
   ABM FUNCIONES
   ===================================================================== */
async function cargarFunciones() {
    $('tbodyFunciones').innerHTML = filaVacia(8, 'Cargando...');
    try {
        const [funciones, peliculas] = await Promise.all([api('/funcion'), api('/pelicula')]);
        Admin.funciones = funciones;
        Admin.peliculas = peliculas;

        const filtroActual = $('filtroFuncionPelicula').value;
        llenarSelect($('filtroFuncionPelicula'), peliculas, p => p.idPelicula, p => p.titulo, 'Todas las películas');
        $('filtroFuncionPelicula').value = peliculas.some(p => String(p.idPelicula) === filtroActual) ? filtroActual : '';
        llenarSelect($('funcionPelicula'), peliculas, p => p.idPelicula, p => `${p.titulo} (${p.duracionMinutos ?? '?'} min)`, 'Seleccioná una película');

        renderFunciones();
    } catch (err) {
        $('tbodyFunciones').innerHTML = filaVacia(8, err.message);
        manejarError(err);
    }
}

function renderFunciones() {
    const idPeli = Number($('filtroFuncionPelicula').value) || null;
    const incluirPasadas = $('filtroFuncionPasadas').checked;
    const hoy = hoyISO();

    const lista = Admin.funciones.filter(f =>
        (!idPeli || f.idPelicula === idPeli) && (incluirPasadas || f.fecha >= hoy));

    $('tbodyFunciones').innerHTML = lista.length
        ? lista.map(f => `
            <tr class="${f.fecha < hoy ? 'fila-pasada' : ''}">
                <td>${fmtFechaCorta(f.fecha)}</td>
                <td class="fw-semibold">${fmtHora(f.horaInicio)}</td>
                <td>${esc(f.peliculaTitulo)} ${f.esParaMayores ? `<span class="badge text-bg-danger">${esc(f.clasificacion)}</span>` : ''}
                    ${f.tieneEntradasVendidas ? '<span class="badge text-bg-success" title="No se puede mover ni cancelar">Con ventas</span>' : ''}</td>
                <td>${esc(f.salaNombre)}</td>
                <td>${esc(f.formatoDescripcion ?? '-')}</td>
                <td>${f.promocionDescripcion ? `<span class="badge text-bg-primary">${esc(f.promocionDescripcion)}</span>` : '-'}</td>
                <td class="num">${fmtMoneda(f.precioEntrada)}</td>
                <td>${botonesAccion(f.idFuncion, { borrar: !f.tieneEntradasVendidas })}</td>
            </tr>`).join('')
        : filaVacia(8, 'No hay funciones para mostrar.');
    refrescarIconos();
}

function abrirFormFuncion(id) {
    const f = Admin.funciones.find(x => x.idFuncion === id);
    $('formFuncion').reset();
    errorForm('errorFuncion', null);
    $('tituloModalFuncion').textContent = f ? 'Editar función' : 'Nueva función';
    $('funcionId').value = f?.idFuncion ?? '';
    $('funcionPelicula').value = f?.idPelicula ?? ($('filtroFuncionPelicula').value || '');
    $('funcionSala').value = f?.idSala ?? Admin.catalogos.salas[0]?.idSala ?? '';
    $('funcionFormato').value = f?.idFormato ?? Admin.catalogos.formatos[0]?.id ?? '';
    $('funcionPromocion').value = f?.idPromocion ?? '';
    $('funcionFecha').value = f?.fecha ?? hoyISO();
    $('funcionFecha').min = f ? '' : hoyISO();
    $('funcionHora').value = f ? fmtHora(f.horaInicio) : '20:00';

    // Regla de negocio (la valida FuncionService): con entradas vendidas no se puede mover la función
    const bloqueada = !!f?.tieneEntradasVendidas;
    ['funcionPelicula', 'funcionSala', 'funcionFecha', 'funcionHora'].forEach(c => $(c).disabled = bloqueada);
    errorForm('errorFuncion', null);
    if (bloqueada) {
        $('errorFuncion').textContent = 'Esta función ya tiene entradas vendidas: solo se puede cambiar el formato y la promoción.';
        $('errorFuncion').hidden = false;
    }

    mostrarFechaLeida();
    abrirModal('modalFuncion');
}

// Muestra en palabras la fecha que leyó el campo (evita confusiones dd/mm vs mm/dd o años de 2 dígitos)
function mostrarFechaLeida() {
    const v = $('funcionFecha').value;
    $('funcionFechaTexto').textContent = v ? `Se va a guardar: ${fmtFechaLarga(v)} de ${v.substring(0, 4)}` : '';
}

async function guardarFuncion(e) {
    e.preventDefault();
    const id = Number($('funcionId').value) || null;
    const datos = {
        idPelicula: Number($('funcionPelicula').value) || null,
        idSala: Number($('funcionSala').value) || null,
        idFormato: Number($('funcionFormato').value) || null,
        idPromocion: Number($('funcionPromocion').value) || null,
        fecha: $('funcionFecha').value || null,
        horaInicio: $('funcionHora').value ? `${$('funcionHora').value}:00` : null
    };

    if (!datos.idPelicula || !datos.idSala || !datos.idFormato || !datos.fecha || !datos.horaInicio)
        return errorForm('errorFuncion', 'Completá película, sala, formato, fecha y hora.');
    if (Number(datos.fecha.substring(0, 4)) < 2000)
        return errorForm('errorFuncion', `El año ${Number(datos.fecha.substring(0, 4))} no es válido. Escribí el año con 4 dígitos (ej. 2026).`);
    if (!id && datos.fecha < hoyISO())
        return errorForm('errorFuncion', `La fecha ingresada es ${fmtFechaLarga(datos.fecha)} de ${datos.fecha.substring(0, 4)}, que ya pasó (hoy es ${fmtFecha(hoyISO())}). Revisá si el campo está en formato mes/día/año.`);

    await enviando(e.target, async () => {
        try {
            const resp = id
                ? await api(`/funcion/${id}`, { method: 'PUT', body: { idFuncion: id, activo: true, ...datos } })
                : await api('/funcion', { method: 'POST', body: datos });
            toast(resp.mensaje, 'success');
            cerrarModal('modalFuncion');
            cargarFunciones();
        } catch (err) {
            errorForm('errorFuncion', err.message);
        }
    });
}

async function borrarFuncion(id) {
    const f = Admin.funciones.find(x => x.idFuncion === id);
    if (!await confirmar('¿Cancelar la función?', `${f?.peliculaTitulo}\n${fmtFechaLarga(f?.fecha)} a las ${fmtHora(f?.horaInicio)} · ${f?.salaNombre}`, { textoConfirmar: 'Sí, cancelar función', peligroso: true })) return;
    try {
        const resp = await api(`/funcion/${id}`, { method: 'DELETE' });
        toast(resp.mensaje, 'success');
        cargarFunciones();
    } catch (err) {
        manejarError(err);
    }
}

/* =====================================================================
   ABM SALAS (las butacas se generan automáticamente)
   ===================================================================== */
async function cargarSalas() {
    $('tbodySalas').innerHTML = filaVacia(6, 'Cargando...');
    try {
        const [salas, complejos] = await Promise.all([api('/sala'), api('/sala/complejos')]);
        Admin.salas = salas;
        llenarSelect($('salaComplejo'), complejos, c => c.id, c => c.descripcion, 'Sin complejo');
        renderSalas();
    } catch (err) {
        $('tbodySalas').innerHTML = filaVacia(6, err.message);
        manejarError(err);
    }
}

function renderSalas() {
    $('tbodySalas').innerHTML = Admin.salas.length
        ? Admin.salas.map(s => `
            <tr>
                <td class="fw-semibold">${esc(s.nombre)}</td>
                <td><span class="badge text-bg-info">${esc(s.tipoSala ?? 'Sin tipo')}</span></td>
                <td>${esc(s.complejo ?? '-')}</td>
                <td class="num">${s.cantidadButacas} <span class="text-body-secondary">(${s.cantidadFilas} filas)</span></td>
                <td class="num">${s.cantidadFunciones}</td>
                <td>${botonesAccion(s.idSala)}</td>
            </tr>`).join('')
        : filaVacia(6, 'No hay salas cargadas.');
    refrescarIconos();
}

function abrirFormSala(id) {
    const s = (Admin.salas ?? []).find(x => x.idSala === id);
    $('formSala').reset();
    errorForm('errorSala', null);
    $('tituloModalSala').textContent = s ? 'Editar sala' : 'Nueva sala';
    $('salaId').value = s?.idSala ?? '';
    $('salaNombre').value = s?.nombre ?? '';
    $('salaTipo').value = s?.idTipoSala ?? Admin.catalogos.tiposSala[0]?.id ?? '';
    $('salaComplejo').value = s?.idComplejo ?? ($('salaComplejo').options[1]?.value ?? '');
    $('salaFilas').value = s ? s.cantidadFilas : 8;
    $('salaPorFila').value = s ? s.butacasPorFila : 12;

    // Si ya vendió entradas no se puede cambiar la distribución
    const bloquear = !!s?.tieneEntradasVendidas;
    $('salaFilas').disabled = bloquear;
    $('salaPorFila').disabled = bloquear;
    $('salaFilas').dataset.bloqueada = bloquear ? '1' : '';
    resumenSala();
    abrirModal('modalSala');
}

function resumenSala() {
    const filas = Number($('salaFilas').value) || 0;
    const porFila = Number($('salaPorFila').value) || 0;
    const ultima = filas >= 1 && filas <= 26 ? String.fromCharCode(64 + filas) : '?';
    $('salaResumen').textContent = $('salaFilas').dataset.bloqueada
        ? 'Esta sala ya tiene entradas vendidas: la cantidad de butacas no se puede modificar.'
        : `Se generan ${filas * porFila} butacas: filas A a ${ultima}, numeradas del 1 al ${porFila}.`;
}

async function guardarSala(e) {
    e.preventDefault();
    const id = Number($('salaId').value) || null;
    const datos = {
        nombre: $('salaNombre').value.trim(),
        idTipoSala: Number($('salaTipo').value),
        idComplejo: Number($('salaComplejo').value) || null,
        filas: Number($('salaFilas').value),
        butacasPorFila: Number($('salaPorFila').value)
    };

    if (!datos.nombre) return errorForm('errorSala', 'El nombre es obligatorio.');
    if (!datos.idTipoSala) return errorForm('errorSala', 'Elegí el tipo de sala.');
    if (!(datos.filas >= 1 && datos.filas <= 26)) return errorForm('errorSala', 'Las filas tienen que estar entre 1 y 26.');
    if (!(datos.butacasPorFila >= 1 && datos.butacasPorFila <= 40)) return errorForm('errorSala', 'Las butacas por fila tienen que estar entre 1 y 40.');

    const original = (Admin.salas ?? []).find(x => x.idSala === id);
    if (original && !original.tieneEntradasVendidas &&
        (original.cantidadFilas !== datos.filas || original.butacasPorFila !== datos.butacasPorFila) &&
        !await confirmar('¿Regenerar las butacas?', `Se van a reemplazar las ${original.cantidadButacas} butacas actuales por ${datos.filas * datos.butacasPorFila} nuevas.`, { textoConfirmar: 'Sí, regenerar' }))
        return;

    await enviando(e.target, async () => {
        try {
            const resp = id
                ? await api(`/sala/${id}`, { method: 'PUT', body: datos })
                : await api('/sala', { method: 'POST', body: datos });
            toast(resp.mensaje, 'success');
            cerrarModal('modalSala');
            cargarSalas();
            cargarCatalogos(); // actualiza el combo de salas de las funciones
        } catch (err) {
            errorForm('errorSala', err.message);
        }
    });
}

async function borrarSala(id) {
    const s = Admin.salas.find(x => x.idSala === id);
    if (!await confirmar(`¿Eliminar "${s?.nombre}"?`, `Se eliminan también sus ${s?.cantidadButacas} butacas.`, { textoConfirmar: 'Sí, eliminar', peligroso: true })) return;
    try {
        const resp = await api(`/sala/${id}`, { method: 'DELETE' });
        toast(resp.mensaje, 'success');
        cargarSalas();
        cargarCatalogos();
    } catch (err) {
        alerta('No se pudo eliminar la sala', err.message, 'error');
    }
}

/* =====================================================================
   PRECIOS DE ENTRADAS (por tipo de sala)
   ===================================================================== */
async function cargarPrecios() {
    $('tbodyPrecios').innerHTML = filaVacia(6, 'Cargando...');
    try {
        Admin.precios = await api('/precio');
        renderPrecios();
    } catch (err) {
        $('tbodyPrecios').innerHTML = filaVacia(6, err.message);
        manejarError(err);
    }
}

const precioVigente = idTipo => Admin.precios.find(p => p.idTipoSala === idTipo && p.estado === 'Vigente');
const precioProgramado = idTipo => Admin.precios.filter(p => p.idTipoSala === idTipo && p.estado === 'Programado')
    .sort((a, b) => a.fechaDesde.localeCompare(b.fechaDesde))[0];

function renderPrecios() {
    // Tarjetas: precio actual de cada tipo de sala
    $('preciosVigentes').innerHTML = Admin.catalogos.tiposSala.map(t => {
        const v = precioVigente(t.id);
        const prox = precioProgramado(t.id);
        return `
            <div class="col-sm-6 col-xl-4">
                <div class="card h-100"><div class="card-body d-flex align-items-center gap-3">
                    <div class="metric-icon text-primary" style="background: rgba(234,179,8,.12)"><i data-feather="tag"></i></div>
                    <div class="flex-grow-1">
                        <div class="small text-body-secondary">${esc(t.descripcion)}</div>
                        <p class="metric-value">${v ? fmtMoneda(v.precio) : '<span class="badge text-bg-danger fs-6">Sin precio</span>'}</p>
                        ${prox ? `<div class="form-text m-0">Desde ${fmtFecha(prox.fechaDesde)}: ${fmtMoneda(prox.precio)}</div>` : ''}
                    </div>
                    <button class="btn btn-outline-secondary btn-sm btn-icono" data-nuevo-precio="${t.id}" title="Cambiar precio" aria-label="Cambiar precio de ${esc(t.descripcion)}"><i data-feather="edit-2"></i></button>
                </div></div>
            </div>`;
    }).join('');

    const verHistoricos = $('filtroPreciosHistoricos').checked;
    const lista = Admin.precios.filter(p => verHistoricos || p.estado !== 'Histórico');
    const badge = { 'Vigente': 'text-bg-success', 'Programado': 'text-bg-info', 'Histórico': 'text-bg-secondary' };

    $('tbodyPrecios').innerHTML = lista.length
        ? lista.map(p => `
            <tr class="${p.estado === 'Histórico' ? 'fila-pasada' : ''}">
                <td class="fw-semibold">${esc(p.tipoSala)}</td>
                <td class="num">${fmtMoneda(p.precio)}</td>
                <td>${fmtFecha(p.fechaDesde)}</td>
                <td>${p.fechaHasta ? fmtFecha(p.fechaHasta) : 'Sin fecha de fin'}</td>
                <td><span class="badge ${badge[p.estado]}">${esc(p.estado)}</span></td>
                <td>${p.sePuedeEliminar ? botonesAccion(p.idPrecio, { editar: false }) : ''}</td>
            </tr>`).join('')
        : filaVacia(6, 'No hay precios cargados.');
    refrescarIconos();
}

function abrirFormPrecio(idTipoSala) {
    $('formPrecio').reset();
    errorForm('errorPrecio', null);
    if (idTipoSala) $('precioTipoSala').value = idTipoSala;
    $('precioDesde').value = hoyISO();
    $('precioDesde').min = hoyISO();
    mostrarPrecioActual();
    abrirModal('modalPrecio');
}

function mostrarPrecioActual() {
    const v = precioVigente(Number($('precioTipoSala').value));
    $('precioActualTexto').textContent = v
        ? `Precio actual: ${fmtMoneda(v.precio)} (desde ${fmtFecha(v.fechaDesde)})`
        : 'Este tipo de sala todavía no tiene precio vigente.';
    if (v && !$('precioMonto').value) $('precioMonto').placeholder = String(v.precio);
}

async function guardarPrecio(e) {
    e.preventDefault();
    const datos = {
        idTipoSala: Number($('precioTipoSala').value),
        precio: Number($('precioMonto').value),
        fechaDesde: $('precioDesde').value
    };

    if (!datos.idTipoSala) return errorForm('errorPrecio', 'Elegí el tipo de sala.');
    if (!datos.precio || datos.precio <= 0) return errorForm('errorPrecio', 'Ingresá un precio mayor a cero.');
    if (!datos.fechaDesde || datos.fechaDesde < hoyISO()) return errorForm('errorPrecio', 'La fecha no puede ser anterior a hoy.');

    const tipo = Admin.catalogos.tiposSala.find(t => t.id === datos.idTipoSala)?.descripcion;
    const ok = await confirmar('¿Confirmás el nuevo precio?',
        `${tipo}: ${fmtMoneda(datos.precio)}\nVigente desde el ${fmtFechaLarga(datos.fechaDesde)}`,
        { textoConfirmar: 'Sí, guardar' });
    if (!ok) return;

    await enviando(e.target, async () => {
        try {
            const resp = await api('/precio', { method: 'POST', body: datos });
            toast(resp.mensaje, 'success');
            cerrarModal('modalPrecio');
            cargarPrecios();
        } catch (err) {
            errorForm('errorPrecio', err.message);
        }
    });
}

async function borrarPrecio(id) {
    const p = Admin.precios.find(x => x.idPrecio === id);
    if (!await confirmar('¿Eliminar este precio?',
        `${p?.tipoSala}: ${fmtMoneda(p?.precio)} desde el ${fmtFecha(p?.fechaDesde)}\nEl precio anterior vuelve a quedar vigente.`,
        { textoConfirmar: 'Sí, eliminar', peligroso: true })) return;
    try {
        const resp = await api(`/precio/${id}`, { method: 'DELETE' });
        toast(resp.mensaje, 'success');
        cargarPrecios();
    } catch (err) {
        manejarError(err);
    }
}

/* =====================================================================
   ABM CLIENTES
   ===================================================================== */
async function cargarClientes() {
    $('tbodyClientes').innerHTML = filaVacia(6, 'Cargando...');
    try {
        Admin.clientes = await api('/cliente');
        renderClientes();
    } catch (err) {
        $('tbodyClientes').innerHTML = filaVacia(6, err.message);
        manejarError(err);
    }
}

function renderClientes() {
    const q = $('buscarCliente').value.trim().toLowerCase();
    const lista = Admin.clientes.filter(c =>
        `${c.apellido} ${c.nombre} ${c.nroDocumento}`.toLowerCase().includes(q));

    $('tbodyClientes').innerHTML = lista.length
        ? lista.map(c => `
            <tr>
                <td class="fw-semibold">${esc(c.apellido)}, ${esc(c.nombre)}</td>
                <td>${esc(c.nroDocumento ?? '-')}</td>
                <td>${fmtFecha(c.fechaNacimiento)}</td>
                <td>${c.esJubilado ? '<span class="badge text-bg-info">Sí</span>' : 'No'}</td>
                <td>${c.calle ? `${esc(c.calle)} ${c.altura ?? ''}` : '-'}</td>
                <td>${botonesAccion(c.idCliente)}</td>
            </tr>`).join('')
        : filaVacia(6, q ? 'No hay clientes que coincidan con la búsqueda.' : 'No hay clientes cargados.');
    refrescarIconos();
}

function abrirFormCliente(id) {
    const c = Admin.clientes.find(x => x.idCliente === id);
    $('formCliente').reset();
    errorForm('errorCliente', null);
    $('tituloModalCliente').textContent = c ? 'Editar cliente' : 'Nuevo cliente';
    $('clienteId').value = c?.idCliente ?? '';
    $('clienteNombre').value = c?.nombre ?? '';
    $('clienteApellido').value = c?.apellido ?? '';
    $('clienteDni').value = c?.nroDocumento ?? '';
    $('clienteNacimiento').value = c?.fechaNacimiento ?? '';
    $('clienteNacimiento').max = hoyISO();
    $('clienteCalle').value = c?.calle ?? '';
    $('clienteAltura').value = c?.altura ?? '';
    $('clienteJubilado').checked = !!c?.esJubilado;
    abrirModal('modalCliente');
}

async function guardarCliente(e) {
    e.preventDefault();
    const id = Number($('clienteId').value) || null;
    const datos = {
        nombre: $('clienteNombre').value.trim(),
        apellido: $('clienteApellido').value.trim(),
        nroDocumento: $('clienteDni').value.trim(),
        fechaNacimiento: $('clienteNacimiento').value || null,
        esJubilado: $('clienteJubilado').checked,
        calle: $('clienteCalle').value.trim() || null,
        altura: $('clienteAltura').value === '' ? null : Number($('clienteAltura').value)
    };

    if (!datos.nombre || !datos.apellido) return errorForm('errorCliente', 'Nombre y apellido son obligatorios.');
    if (!/^\d{6,}$/.test(datos.nroDocumento)) return errorForm('errorCliente', 'El DNI tiene que tener solo números (mínimo 6).');
    if (datos.fechaNacimiento && datos.fechaNacimiento > hoyISO()) return errorForm('errorCliente', 'La fecha de nacimiento no puede ser futura.');

    await enviando(e.target, async () => {
        try {
            const resp = id
                ? await api(`/cliente/${id}`, { method: 'PUT', body: { idCliente: id, activo: true, ...datos } })
                : await api('/cliente', { method: 'POST', body: datos });
            toast(resp.mensaje, 'success');
            cerrarModal('modalCliente');
            cargarClientes();
        } catch (err) {
            errorForm('errorCliente', err.message);
        }
    });
}

async function borrarCliente(id) {
    const c = Admin.clientes.find(x => x.idCliente === id);
    if (!await confirmar('¿Dar de baja al cliente?', `${c?.apellido}, ${c?.nombre} (DNI ${c?.nroDocumento ?? '-'})`, { textoConfirmar: 'Sí, dar de baja', peligroso: true })) return;
    try {
        const resp = await api(`/cliente/${id}`, { method: 'DELETE' });
        toast(resp.mensaje, 'success');
        cargarClientes();
    } catch (err) {
        manejarError(err);
    }
}

/* =====================================================================
   VENTAS (listado, detalle y venta en boletería)
   ===================================================================== */
async function cargarVentas() {
    $('tbodyVentas').innerHTML = filaVacia(8, 'Cargando...');
    try {
        Admin.ventas = await api('/venta');
        renderVentas();
    } catch (err) {
        $('tbodyVentas').innerHTML = filaVacia(8, err.message);
        manejarError(err);
    }
}

function renderVentas() {
    const q = $('buscarVenta').value.trim().toLowerCase().replace('#', '');
    const lista = Admin.ventas.filter(v =>
        !q || String(v.idVenta) === q || (v.cliente ?? '').toLowerCase().includes(q));

    $('tbodyVentas').innerHTML = lista.length
        ? lista.map(v => `
            <tr>
                <td class="fw-semibold">#${v.idVenta}</td>
                <td>${fmtFechaHora(v.fechaHora)}</td>
                <td>${esc(v.cliente || '-')}</td>
                <td>${esc(v.canal ?? '-')}</td>
                <td class="num">${v.cantidadEntradas}</td>
                <td>${esc(v.formasPago || '-')}</td>
                <td class="num fw-semibold">${fmtMoneda(v.total)}</td>
                <td>${botonesAccion(v.idVenta, { editar: false, borrar: false, ver: true })}</td>
            </tr>`).join('')
        : filaVacia(8, q ? 'No hay ventas que coincidan con la búsqueda.' : 'Todavía no hay ventas registradas.');
    refrescarIconos();
}

async function verVenta(id) {
    $('ventaDetalleBody').innerHTML = htmlCargando();
    abrirModal('modalVentaDetalle');
    try {
        $('ventaDetalleBody').innerHTML = htmlDetalleVenta(await api(`/venta/${id}`));
    } catch (err) {
        $('ventaDetalleBody').innerHTML = htmlVacio(err.message);
    }
}

// --- Nueva venta en boletería: maestro (cliente, pago) + detalle (entradas) ---
const entradasVacias = () => ({ entradas: () => [], total: () => 0, detalle: () => [] });
const ventaNueva = { funcion: null, seleccion: [], entradasUI: entradasVacias() };

async function abrirNuevaVenta() {
    errorForm('errorVenta', null);
    $('ventaMapa').innerHTML = '<p class="text-body-secondary">Elegí una función para ver las butacas.</p>';
    $('ventaEntradas').innerHTML = '';
    $('ventaTotal').textContent = fmtMoneda(0);
    Object.assign(ventaNueva, { funcion: null, seleccion: [], entradasUI: entradasVacias() });
    abrirModal('modalVenta');

    try {
        const [clientes, funciones] = await Promise.all([api('/cliente'), api('/funcion?soloProximas=true')]);
        Admin.clientes = clientes;
        Admin.funcionesVenta = funciones;

        llenarSelect($('ventaCliente'), clientes, c => c.idCliente, c => `${c.apellido}, ${c.nombre} - DNI ${c.nroDocumento ?? '-'}`, 'Seleccioná un cliente');
        llenarSelect($('ventaFuncion'), funciones,
            f => f.idFuncion,
            f => `${f.peliculaTitulo} · ${fmtFechaCorta(f.fecha)} ${fmtHora(f.horaInicio)} · ${f.salaNombre} · ${fmtMoneda(f.precioEntrada)}`,
            funciones.length ? 'Seleccioná una función' : 'No hay funciones próximas');
    } catch (err) {
        errorForm('errorVenta', err.message);
    }
}

async function cargarButacasVenta() {
    const id = Number($('ventaFuncion').value);
    ventaNueva.funcion = (Admin.funcionesVenta ?? []).find(f => f.idFuncion === id) ?? null;
    ventaNueva.seleccion = [];
    ventaNueva.entradasUI = entradasVacias();
    $('ventaEntradas').innerHTML = '';
    actualizarTotalVenta();

    if (!ventaNueva.funcion) {
        $('ventaMapa').innerHTML = '<p class="text-body-secondary">Elegí una función para ver las butacas.</p>';
        return;
    }

    $('ventaMapa').innerHTML = htmlCargando('Cargando butacas...');
    try {
        const butacas = await api(`/funcion/${id}/butacas`);
        crearMapaButacas($('ventaMapa'), butacas, {
            max: 10,
            onChange: sel => {
                ventaNueva.seleccion = sel;
                // Precio por tipo de espectador calculado por el servidor (promo o 15% jubilado/menor)
                ventaNueva.entradasUI = renderDetalleEntradas($('ventaEntradas'), sel, ventaNueva.funcion, actualizarTotalVenta);
                actualizarTotalVenta();
            }
        });
    } catch (err) {
        $('ventaMapa').innerHTML = htmlVacio(err.message);
    }
}

function actualizarTotalVenta() {
    $('ventaTotal').textContent = `${fmtMoneda(ventaNueva.entradasUI.total())} (${ventaNueva.seleccion.length} entradas)`;
}

async function registrarVenta(e) {
    e.preventDefault();
    const idCliente = Number($('ventaCliente').value) || null;
    const entradas = ventaNueva.entradasUI.entradas();

    if (!idCliente) return errorForm('errorVenta', 'Seleccioná el cliente.');
    if (!ventaNueva.funcion) return errorForm('errorVenta', 'Seleccioná la función.');
    if (!entradas.length) return errorForm('errorVenta', 'Elegí al menos una butaca.');
    errorForm('errorVenta', null);

    await enviando(e.target, async () => {
        try {
            // Una sola llamada: la API guarda VENTA + ENTRADAs + forma de pago en una transacción
            const resp = await api('/venta', {
                method: 'POST',
                body: {
                    idCliente,
                    idCanal: 1, // Boletería presencial
                    entradas,
                    formasPago: [{ idFormaPago: Number($('ventaFormaPago').value) }]
                }
            });
            toast(`${resp.mensaje} (N° ${resp.idVenta})`, 'success');
            cerrarModal('modalVenta');
            await cargarVentas();
            verVenta(resp.idVenta);
        } catch (err) {
            errorForm('errorVenta', err.message);
            if (/ocupada|acaba de vender/i.test(err.message)) cargarButacasVenta();
        }
    });
}
