function crearMapaButacas(contenedor, butacas, { max = 10, onChange = () => {} } = {}) {
    const seleccionadas = new Map();

    if (!butacas.length) {
        contenedor.innerHTML = htmlVacio('Esta sala no tiene butacas cargadas.');
        return { seleccion: () => [] };
    }

    const filas = {};
    butacas.forEach(b => (filas[b.fila ?? '?'] ??= []).push(b));
    Object.values(filas).forEach(f => f.sort((a, b) => (a.numero ?? 0) - (b.numero ?? 0)));

    contenedor.innerHTML = `
        <div class="sala-mapa bg-body-tertiary border rounded-3 px-3 py-4">
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
            <div class="leyenda d-flex flex-wrap justify-content-center gap-4 mt-3 small text-body-secondary">
                <span class="d-inline-flex align-items-center gap-2"><i style="background:var(--seat-free)"></i>Libre</span>
                <span class="d-inline-flex align-items-center gap-2"><i style="background:var(--seat-taken)"></i>Ocupada</span>
                <span class="d-inline-flex align-items-center gap-2"><i style="background:var(--seat-selected)"></i>Tu selección</span>
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

function renderDetalleEntradas(contenedor, seleccion, funcion, onCambio = () => {}) {
    const vacio = { entradas: () => [], total: () => 0, ahorro: () => 0, detalle: () => [] };
    if (!seleccion.length) {
        contenedor.innerHTML = '<p class="text-body-secondary">Elegí tus butacas en el mapa.</p>';
        return vacio;
    }

    const precios = (funcion.precios ?? []).filter(p => p.permitido);
    const precioDe = idTipo => precios.find(p => p.idTipoEspectador === Number(idTipo));

    const previos = {};
    contenedor.querySelectorAll('select[data-butaca]').forEach(s => previos[s.dataset.butaca] = s.value);

    const opciones = precios.map(p =>
        `<option value="${p.idTipoEspectador}">${esc(p.tipoEspectador)} — ${fmtMoneda(p.precio)}${p.descuento ? ` (${esc(p.descuento)}, antes ${fmtMoneda(p.precioSinDescuento)})` : ''}</option>`
    ).join('');

    const aviso = funcion.esParaMayores
        ? `<div class="alert alert-warning py-2 small">Película ${esc(funcion.clasificacion)}: no se venden entradas de Menor.</div>`
        : '';

    contenedor.innerHTML = aviso + seleccion
        .slice()
        .sort((a, b) => `${a.fila}${String(a.numero).padStart(3, '0')}`.localeCompare(`${b.fila}${String(b.numero).padStart(3, '0')}`))
        .map(b => `
            <div class="row g-2 align-items-center border rounded-3 bg-body-tertiary mx-0 mb-2 p-2">
                <div class="col-4 col-sm-3 fw-semibold">Butaca ${esc(b.fila)}${b.numero}</div>
                <div class="col-8 col-sm-6">
                    <select class="form-select form-select-sm" data-butaca="${b.idButaca}" data-nombre="${esc(b.fila)}${b.numero}" aria-label="Tipo de espectador para la butaca ${esc(b.fila)}${b.numero}">${opciones}</select>
                </div>
                <div class="col-12 col-sm-3 text-end fw-semibold" data-precio-butaca="${b.idButaca}"></div>
            </div>`).join('');

    const selects = () => [...contenedor.querySelectorAll('select[data-butaca]')];

    const actualizarPrecios = () => {
        selects().forEach(s => {
            const p = precioDe(s.value);
            contenedor.querySelector(`[data-precio-butaca="${s.dataset.butaca}"]`).innerHTML =
                htmlPrecio(p?.precio ?? 0, p?.precioSinDescuento);
        });
    };

    selects().forEach(s => {
        if (previos[s.dataset.butaca] && precioDe(previos[s.dataset.butaca])) s.value = previos[s.dataset.butaca];
        s.addEventListener('change', () => { actualizarPrecios(); onCambio(); });
    });
    actualizarPrecios();

    const detalle = () => selects().map(s => {
        const p = precioDe(s.value);
        const precio = p?.precio ?? 0;
        return {
            butaca: s.dataset.nombre, idButaca: Number(s.dataset.butaca), idTipoEspectador: Number(s.value), tipo: p?.tipoEspectador,
            precio, precioSinDescuento: p?.precioSinDescuento ?? precio
        };
    });

    return {
        entradas: () => detalle().map(d => ({ idFuncion: funcion.idFuncion, idButaca: d.idButaca, idTipoEspectador: d.idTipoEspectador })),
        total: () => detalle().reduce((s, d) => s + d.precio, 0),
        ahorro: () => detalle().reduce((s, d) => s + Math.max(0, d.precioSinDescuento - d.precio), 0),
        detalle
    };
}
