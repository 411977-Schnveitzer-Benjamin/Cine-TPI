// js/movies.js

const defaultMovies = [
    {
        id: 1,
        title: "Inception",
        duration: "148 min",
        classification: "+13",
        status: "Cartelera",
        image: "https://via.placeholder.com/300x450/1e293b/ffffff?text=Inception"
    },
    {
        id: 2,
        title: "The Dark Knight",
        duration: "152 min",
        classification: "+16",
        status: "Cartelera",
        image: "https://via.placeholder.com/300x450/1e293b/ffffff?text=The+Dark+Knight"
    },
    {
        id: 3,
        title: "Interstellar",
        duration: "169 min",
        classification: "ATP",
        status: "Cartelera",
        image: "https://via.placeholder.com/300x450/1e293b/ffffff?text=Interstellar"
    },
    {
        id: 4,
        title: "Dune: Parte Dos",
        duration: "166 min",
        classification: "+13",
        status: "Próximamente",
        image: "https://via.placeholder.com/300x450/1e293b/ffffff?text=Dune+Parte+Dos"
    },
    {
        id: 5,
        title: "Deadpool & Wolverine",
        duration: "128 min",
        classification: "+16",
        status: "Próximamente",
        image: "https://via.placeholder.com/300x450/1e293b/ffffff?text=Deadpool+y+Wolverine"
    }
];

if (!localStorage.getItem('cine_movies')) {
    localStorage.setItem('cine_movies', JSON.stringify(defaultMovies));
}

function getMovies() {
    return JSON.parse(localStorage.getItem('cine_movies')) || [];
}

function saveMovies(movies) {
    localStorage.setItem('cine_movies', JSON.stringify(movies));
}

function createMovieCardHTML(movie) {
    let badgeHtml = '';
    if (movie.status === 'Próximamente') {
        badgeHtml = `<div style="position: absolute; top: 10px; right: 10px; background: var(--clr-primary); color: white; padding: 0.3rem 0.8rem; border-radius: 20px; font-weight: bold; font-size: 0.8rem; box-shadow: 0 4px 6px rgba(0,0,0,0.3);">Muy Pronto</div>`;
    }

    let badgeClass = 'badge-success';
    if (movie.classification === '+13') badgeClass = 'badge-warning';
    if (movie.classification === '+16' || movie.classification === '+18') badgeClass = 'badge-danger';

    const urlParams = new URLSearchParams(window.location.search);
    const promo = urlParams.get('promo');
    const promoQuery = promo ? `&promo=${promo}` : '';

    let actionBtn = movie.status === 'Próximamente' 
        ? '' 
        : `<button class="btn btn-primary" style="width: 100%;" onclick="window.location.href='reserva.html?id=${movie.id}${promoQuery}'">Ver Funciones</button>`;

    return `
        <div class="movie-card" style="background: var(--clr-surface); border: 1px solid var(--clr-border); border-radius: 12px; overflow: hidden; display: flex; flex-direction: column; position: relative;">
            ${badgeHtml}
            <img src="${movie.image}" alt="${movie.title}" style="width: 100%; height: 350px; object-fit: cover;">
            <div style="padding: 1.5rem;">
                <h3 style="margin-bottom: 0.5rem; font-size: 1.25rem;">${movie.title}</h3>
                <div style="display: flex; gap: 0.5rem; margin-bottom: 1rem;">
                    <span class="badge ${badgeClass}">${movie.classification}</span>
                    <span style="font-size: 0.9rem; color: var(--clr-text-muted);">${movie.duration}</span>
                </div>
                ${actionBtn}
            </div>
        </div>
    `;
}
