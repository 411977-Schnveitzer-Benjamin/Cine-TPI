/* =========================================================================
   reserva.js - Compra web de entradas
   1) función  2) butacas + tipo de espectador  3) forma de pago -> POST /api/venta
   Requiere: api.js, main.js, butacas.js
   ========================================================================= */

const estado = {
    pelicula: null,
    funciones: [],
    funcion: null,
    formasPago: [],
    seleccion: [],
    entradasUI: { entradas: () => [], total: () => 0, detalle: () => [] } // lo arma renderDetalleEntradas
};

document.addEventListener('DOMContentLoaded', iniciarReserva);
document.addEventListener('sesion-cambiada', actualizarPaso3);

async function iniciarReserva() {
    const params = new URLSearchParams(location.search);
    const idPelicula = Number(params.get('id'));
    const info = document.getElementById('peliculaInfo');

    if (!idPelicula) {
        info.innerHTML = htmlVacio('No se indicó ninguna película.') ;
        info.insertAdjacentHTML('beforeend', '<a class="btn btn-primary" href="cartelera.html">Ir a la cartelera</a>');
        return;
    }

    try {
        const [pelicula, funciones, formas] = await Promise.all([
            api(`/pelicula/${idPelicula}`),
            api(`/funcion?idPelicula=${idPelicula}&soloProximas=true`),
            api('/catalogo/formas-pago')
        ]);
        Object.assign(estado, { pelicula, funciones, formasPago: formas });
    } catch (err) {
        info.innerHTML = htmlVacio(err.message);
        return;
    }

    const p = estado.pelicula;
    document.title = `${p.titulo} - Comprar entradas`;
    info.innerHTML = `
        <div class="reserva-poster rounded overflow-hidden">${posterHTML(p)}</div>
        <div>
            <h1 class="display-6 fw-bold mb-2">${esc(p.titulo)}</h1>
            <div class="d-flex align-items-center gap-2 text-body-secondary">${badgeClasificacion(p.clasificacionDescripcion)}<span>${p.duracionMinutos ?? '-'} min</span></div>
        </div>`;

    document.getElementById('flujoCompra').hidden = false;
    document.getElementById('formaPago').innerHTML = estado.formasPago
        .map(f => `<option value="${f.id}">${esc(f.descripcion)}</option>`).join('');
    document.getElementById('formaPago').addEventListener('change', renderResumen);
    document.getElementById('btnConfirmar').addEventListener('click', confirmarCompra);

    renderFunciones();

    // Si vino ?funcion=X (desde los horarios de la cartelera) la preseleccionamos
    const idFuncion = Number(params.get('funcion'));
    if (idFuncion && estado.funciones.some(f => f.idFuncion === idFuncion)) elegirFuncion(idFuncion);
}

/* ---------- Paso 1 ---------- */
function renderFunciones() {
    const cont = document.getElementById('funcionesLista');
    if (!estado.funciones.length) {
        cont.innerHTML = htmlVacio('Esta película no tiene funciones programadas por ahora.');
        return;
    }

    // Agrupamos por día
    const porDia = {};
    estado.funciones.forEach(f => (porDia[f.fecha] ??= []).push(f));

    cont.innerHTML = Object.entries(porDia).map(([fecha, lista]) => `
        <div class="funciones-dia mb-3">
            <h3 class="h6 text-body-secondary mb-2">${fmtFechaLarga(fecha)}</h3>
            <div class="d-flex flex-wrap gap-2">
                ${lista.map(f => `
                    <button type="button" class="btn btn-outline-secondary funcion-btn" data-funcion="${f.idFuncion}" aria-pressed="false">
                        <strong>${fmtHora(f.horaInicio)}</strong>
                        <small class="d-block text-body-secondary">${esc(f.salaNombre)} · ${esc(f.formatoDescripcion ?? '')}</small>
                        <small class="d-block">General ${fmtMoneda(f.precioEntrada)}</small>
                        ${f.promocionDescripcion ? `<span class="badge text-bg-primary mt-1">${esc(f.promocionDescripcion)}</span>` : ''}
                    </button>`).join('')}
            </div>
        </div>`).join('');

    cont.querySelectorAll('.funcion-btn').forEach(b => b.addEventListener('click', () => elegirFuncion(Number(b.dataset.funcion))));
}

async function elegirFuncion(idFuncion) {
    estado.funcion = estado.funciones.find(f => f.idFuncion === idFuncion);
    estado.seleccion = [];

    document.querySelectorAll('.funcion-btn').forEach(b => {
        const sel = Number(b.dataset.funcion) === idFuncion;
        b.classList.toggle('active', sel);
        b.setAttribute('aria-pressed', sel);
    });

    const f = estado.funcion;
    document.getElementById('funcionElegidaTexto').textContent =
        `${fmtFechaLarga(f.fecha)} · ${fmtHora(f.horaInicio)} hs · ${f.salaNombre} (${f.tipoSala ?? ''}) · ${f.formatoDescripcion ?? ''}`;

    const step2 = document.getElementById('step2');
    step2.hidden = false;
    document.getElementById('step3').hidden = true;
    const mapa = document.getElementById('mapaButacas');
    mapa.innerHTML = htmlCargando('Cargando butacas...');
    step2.scrollIntoView({ behavior: 'smooth', block: 'start' });

    await cargarMapa();
}

async function cargarMapa() {
    const mapa = document.getElementById('mapaButacas');
    try {
        const butacas = await api(`/funcion/${estado.funcion.idFuncion}/butacas`);
        crearMapaButacas(mapa, butacas, { max: 10, onChange: sel => { estado.seleccion = sel; onSeleccionCambiada(); } });
        onSeleccionCambiada();
    } catch (err) {
        mapa.innerHTML = htmlVacio(err.message);
    }
}

/* ---------- Paso 2 ---------- */
function onSeleccionCambiada() {
    const detalle = document.getElementById('entradasDetalle');
    // Cada entrada elige su tipo (General / Jubilado / Menor); el precio de cada uno viene del servidor
    estado.entradasUI = renderDetalleEntradas(detalle, estado.seleccion, estado.funcion, renderResumen);

    document.getElementById('step3').hidden = estado.seleccion.length === 0;
    actualizarPaso3();
}

/* ---------- Paso 3 ---------- */
function actualizarPaso3() {
    if (!estado.funcion) return;
    const esAdmin = Auth.esAdmin();
    const logueado = Auth.estaLogueado();

    document.getElementById('avisoAdmin').hidden = !esAdmin;
    const btn = document.getElementById('btnConfirmar');
    btn.disabled = esAdmin || estado.seleccion.length === 0;
    btn.textContent = logueado ? 'Confirmar compra' : 'Iniciar sesión y comprar';
    document.getElementById('hintLogin').textContent = logueado
        ? `Comprás como ${Auth.sesion.nombre}.`
        : 'Necesitás una cuenta para comprar. Si no tenés, registrate en un minuto.';
    renderResumen();
}

function renderResumen() {
    if (!estado.funcion) return;
    const forma = estado.formasPago.find(f => f.id === Number(document.getElementById('formaPago').value));

    // Agrupamos las entradas por tipo: "2 x General $4.000", "1 x Jubilado $3.400"
    const grupos = {};
    estado.entradasUI.detalle().forEach(d => {
        const g = (grupos[d.tipo] ??= { cant: 0, precio: d.precio });
        g.cant++;
    });
    // Cada línea es un item de una "list-group" de Bootstrap
    const fila = (izq, der) => `<li class="list-group-item d-flex justify-content-between gap-3"><span>${izq}</span><span class="text-end">${der}</span></li>`;
    const lineas = Object.entries(grupos).map(([tipo, g]) => fila(`${g.cant} x ${esc(tipo)}`, `${fmtMoneda(g.precio)} c/u`)).join('');

    document.getElementById('resumen').innerHTML = `
        ${fila('Película', `<strong>${esc(estado.pelicula.titulo)}</strong>`)}
        ${fila('Función', `${fmtFechaCorta(estado.funcion.fecha)} ${fmtHora(estado.funcion.horaInicio)} · ${esc(estado.funcion.salaNombre)}`)}
        ${fila('Butacas', estado.seleccion.map(b => esc(b.fila) + b.numero).join(', ') || '-')}
        ${lineas}
        ${estado.funcion.promocionDescripcion ? fila('Promoción', `${esc(estado.funcion.promocionDescripcion)} (-${Number(estado.funcion.porcentajeDescuento)}%)`) : ''}
        ${fila('Forma de pago', esc(forma?.descripcion ?? '-'))}
        <li class="list-group-item d-flex justify-content-between fs-4 fw-bold text-primary"><span>Total</span><span>${fmtMoneda(estado.entradasUI.total())}</span></li>`;
}

async function confirmarCompra() {
    if (!Auth.estaLogueado()) {
        Sesion.pedirLogin(() => actualizarPaso3());
        return;
    }

    const entradas = estado.entradasUI.entradas();
    if (!entradas.length) return toast('Elegí al menos una butaca.', 'warning');

    const btn = document.getElementById('btnConfirmar');
    btn.disabled = true;
    btn.textContent = 'Procesando...';

    try {
        // Maestro (venta) + detalle (entradas) + forma de pago: la API lo guarda en UNA transacción
        const resp = await api('/venta', {
            method: 'POST',
            body: {
                entradas,
                formasPago: [{ idFormaPago: Number(document.getElementById('formaPago').value) }]
            }
        });
        await mostrarComprobante(resp.idVenta);
    } catch (err) {
        toast(err.message, 'error', 6000);
        // Si alguien ganó la butaca, refrescamos el mapa
        if (/ocupada|acaba de vender/i.test(err.message)) {
            estado.seleccion = [];
            await cargarMapa();
        }
        actualizarPaso3();
    }
}

async function mostrarComprobante(idVenta) {
    document.getElementById('flujoCompra').hidden = true;
    const comp = document.getElementById('comprobante');
    comp.hidden = false;
    comp.innerHTML = htmlCargando();
    window.scrollTo({ top: 0, behavior: 'smooth' });

    let detalle = '';
    try {
        detalle = htmlDetalleVenta(await api(`/venta/${idVenta}`));
    } catch { /* si falla el detalle igual mostramos el ok */ }

    comp.innerHTML = `
        <div class="card-body p-4">
            <div class="comprobante-icono mb-3"><svg width="34" height="34" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" aria-hidden="true"><polyline points="20 6 9 17 4 12"/></svg></div>
            <h2>¡Compra confirmada!</h2>
            <p class="text-body-secondary">Número de operación <strong>#${idVenta}</strong>. Presentá este número en la boletería.</p>
            <div class="text-start">${detalle}</div>
            <div class="d-flex flex-wrap justify-content-center gap-2 mt-4">
                <a class="btn btn-outline-secondary" href="cartelera.html">Volver a la cartelera</a>
                <a class="btn btn-primary" href="index.html">Ir al inicio</a>
            </div>
        </div>`;
}
