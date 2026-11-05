/* =========================================================================
   cartelera.js - Películas con funciones próximas y sus horarios
   ========================================================================= */

document.addEventListener('DOMContentLoaded', async () => {
    const cont = document.getElementById('peliculas-container');
    cont.innerHTML = htmlCargando('Cargando cartelera...');

    try {
        const [peliculas, funciones] = await Promise.all([
            api('/pelicula/cartelera'),
            api('/funcion?soloProximas=true')
        ]);

        if (!peliculas.length) {
            cont.innerHTML = htmlVacio('No hay películas en cartelera por el momento.');
            return;
        }

        cont.innerHTML = peliculas.map(p => {
            const susFunciones = funciones.filter(f => f.idPelicula === p.idPelicula);
            const chips = susFunciones.slice(0, 6).map(f =>
                `<a class="btn btn-sm btn-outline-secondary rounded-pill" href="reserva.html?id=${p.idPelicula}&funcion=${f.idFuncion}">${fmtFechaCorta(f.fecha)} ${fmtHora(f.horaInicio)}</a>`
            ).join('');
            const mas = susFunciones.length > 6 ? `<span class="btn btn-sm btn-outline-secondary rounded-pill disabled">+${susFunciones.length - 6}</span>` : '';
            const promo = susFunciones.some(f => f.idPromocion) ? 'Promo' : '';

            return `
                <div class="col">
                    <article class="card h-100 movie-card">
                        ${posterHTML(p, promo)}
                        <div class="card-body d-flex flex-column gap-2">
                            <h2 class="card-title h5 mb-0">${esc(p.titulo)}</h2>
                            <div class="d-flex align-items-center gap-2 small text-body-secondary">
                                ${badgeClasificacion(p.clasificacionDescripcion)}
                                <span>${p.duracionMinutos ?? '-'} min</span>
                            </div>
                            <div class="d-flex flex-wrap gap-1">${chips}${mas}</div>
                            <a class="btn btn-primary w-100 mt-auto" href="reserva.html?id=${p.idPelicula}">Comprar entradas</a>
                        </div>
                    </article>
                </div>`;
        }).join('');
    } catch (err) {
        cont.innerHTML = htmlVacio(err.message);
    }
});
