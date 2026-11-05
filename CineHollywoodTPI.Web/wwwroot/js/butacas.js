/* =========================================================================
   butacas.js - Selector de butacas + detalle de entradas (tipo de espectador)
   Lo usan reserva.html (compra web) y admin.html (venta en boletería).
   Requiere: api.js
   ========================================================================= */

/**
 * Dibuja el mapa de la sala dentro de `contenedor`.
 * @param {HTMLElement} contenedor
 * @param {Array} butacas  respuesta de GET /api/funcion/{id}/butacas
 * @param {{max:number, onChange:function}} opciones
 * @returns {{seleccion: () => Array}}
 */
function crearMapaButacas(contenedor, butacas, { max = 10, onChange = () => {} } = {}) {
    const seleccionadas = new Map(); // idButaca -> butaca

    if (!butacas.length) {
        contenedor.innerHTML = htmlVacio('Esta sala no tiene butacas cargadas.');
        return { seleccion: () => [] };
    }

    // Agrupamos por fila (A, B, C...) y ordenamos por número
    const filas = {};
    butacas.forEach(b => (filas[b.fila ?? '?'] ??= []).push(b));
    Object.values(filas).forEach(f => f.sort((a, b) => (a.numero ?? 0) - (b.numero ?? 0)));

    contenedor.innerHTML = `
        <div class="sala-mapa">
            <div class="pantalla">PANTALLA</div>
            ${Object.keys(filas).sort().map(fila => `
                <div class="fila-butacas">
                    <span class="fila-label">${esc(fila)}</span>
                    ${filas[fila].map(b => {
                        const noDisponible = b.ocupada || !b.habilitada;
                        const clase = b.ocupada ? 'seat--ocupada' : (!b.habilitada ? 'seat--deshabilitada' : '');
                        const estado = b.ocupada ? 'ocupada' : (!b.habilitada ? 'no habilitada' : 'libre');
                        return `<button type="button" class="seat ${clase}" data-id="${b.idButaca}"
                                    ${noDisponible ? 'disabled' : ''}
                                    aria-pressed="false"
                                    aria-label="Fila ${esc(b.fila)} butaca ${b.numero} (${estado})"
                                    title="${esc(b.fila)}${b.numero} - ${estado}">${b.numero}</button>`;
                    }).join('')}
                    <span class="fila-label">${esc(fila)}</span>
                </div>`).join('')}
            <div class="leyenda">
                <span><i style="background:var(--seat-free)"></i>Libre</span>
                <span><i style="background:var(--seat-taken)"></i>Ocupada</span>
                <span><i style="background:var(--seat-selected)"></i>Tu selección</span>
            </div>
        </div>`;

    contenedor.querySelectorAll('.seat:not(:disabled)').forEach(btn => {
        btn.addEventListener('click', () => {
            const id = Number(btn.dataset.id);
            if (seleccionadas.has(id)) {
                seleccionadas.delete(id);
            } else {
                if (seleccionadas.size >= max) {
                    toast(`Podés elegir hasta ${max} butacas por compra.`, 'warning');
                    return;
                }
                seleccionadas.set(id, butacas.find(b => b.idButaca === id));
            }
            const marcada = seleccionadas.has(id);
            btn.classList.toggle('seat--seleccionada', marcada);
            btn.setAttribute('aria-pressed', marcada);
            onChange([...seleccionadas.values()]);
        });
    });

    return { seleccion: () => [...seleccionadas.values()] };
}

/**
 * Lista las butacas elegidas con un <select> de tipo de espectador cada una.
 * Devuelve una función que arma el array de entradas para la API.
 */
function renderDetalleEntradas(contenedor, seleccion, tiposEspectador, precioUnitario, idFuncion) {
    if (!seleccion.length) {
        contenedor.innerHTML = '<p class="text-muted">Elegí tus butacas en el mapa.</p>';
        return () => [];
    }

    // Conservamos el tipo elegido si se vuelve a dibujar la lista
    const previos = {};
    contenedor.querySelectorAll('select[data-butaca]').forEach(s => previos[s.dataset.butaca] = s.value);

    const opciones = tiposEspectador.map(t => `<option value="${t.id}">${esc(t.descripcion)}</option>`).join('');

    contenedor.innerHTML = seleccion
        .slice()
        .sort((a, b) => `${a.fila}${String(a.numero).padStart(3, '0')}`.localeCompare(`${b.fila}${String(b.numero).padStart(3, '0')}`))
        .map(b => `
            <div class="entrada-item">
                <strong>Butaca ${esc(b.fila)}${b.numero}</strong>
                <select class="form-control" data-butaca="${b.idButaca}" aria-label="Tipo de espectador para la butaca ${esc(b.fila)}${b.numero}">${opciones}</select>
                <span class="entrada-precio">${fmtMoneda(precioUnitario)}</span>
            </div>`).join('');

    contenedor.querySelectorAll('select[data-butaca]').forEach(s => {
        if (previos[s.dataset.butaca]) s.value = previos[s.dataset.butaca];
    });

    return () => [...contenedor.querySelectorAll('select[data-butaca]')].map(s => ({
        idFuncion,
        idButaca: Number(s.dataset.butaca),
        idTipoEspectador: Number(s.value)
    }));
}
