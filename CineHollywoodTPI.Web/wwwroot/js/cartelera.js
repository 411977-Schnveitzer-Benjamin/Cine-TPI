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
                `<a class="chip" href="reserva.html?id=${p.idPelicula}&funcion=${f.idFuncion}">${fmtFechaCorta(f.fecha)} ${fmtHora(f.horaInicio)}</a>`
            ).join('');
            const mas = susFunciones.length > 6 ? `<span class="chip">+${susFunciones.length - 6}</span>` : '';
            const promo = susFunciones.some(f => f.idPromocion) ? 'Promo' : '';

            return `
                <article class="movie-card">
                    ${posterHTML(p, promo)}
                    <div class="movie-card-body">
                        <h2 class="movie-card-title">${esc(p.titulo)}</h2>
                        <div class="movie-meta">
                            ${badgeClasificacion(p.clasificacionDescripcion)}
                            <span>${p.duracionMinutos ?? '-'} min</span>
                        </div>
                        <div class="chips">${chips}${mas}</div>
                        <a class="btn btn-primary btn-block" href="reserva.html?id=${p.idPelicula}">Comprar entradas</a>
                    </div>
                </article>`;
        }).join('');
    } catch (err) {
        cont.innerHTML = htmlVacio(err.message);
    }
});
