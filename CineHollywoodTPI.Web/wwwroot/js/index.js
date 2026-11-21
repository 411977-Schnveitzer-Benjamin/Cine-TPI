document.addEventListener('DOMContentLoaded', cargarCarrusel);

async function cargarCarrusel() {
    const track = document.getElementById('carouselTrack');
    track.style.height = '';
    track.innerHTML = htmlCargando('Cargando cartelera...');

    let peliculas;
    try {
        peliculas = await api('/pelicula/cartelera');
    } catch (err) {
        track.style.height = 'auto';
        track.innerHTML = `
            <div class="col-12 text-center text-body-secondary border border-dashed rounded-3 py-5 px-3">
                <p class="mb-3">${esc(err.message)}</p>
                <button type="button" class="btn btn-outline-primary rounded-pill" id="btnReintentarCartelera">Reintentar</button>
            </div>`;
        document.getElementById('btnReintentarCartelera').addEventListener('click', cargarCarrusel);
        return;
    }

    if (!peliculas.length) {
        track.style.height = 'auto';
        track.innerHTML = htmlVacio('No hay películas en cartelera por el momento.');
        document.getElementById('carouselControles').hidden = true;
        return;
    }

    track.innerHTML = peliculas.map((p, i) => `
        <button type="button" class="carousel-slide" data-index="${i}" data-id="${p.idPelicula}" aria-label="Comprar entradas para ${esc(p.titulo)}">
            ${posterHTML(p)}
            <span class="carousel-caption-3d">${esc(p.titulo)}</span>
        </button>`).join('');

    const slides = [...track.querySelectorAll('.carousel-slide')];
    let actual = 0;

    function actualizar() {
        slides.forEach((slide, i) => {
            let offset = i - actual;
            const n = slides.length;
            if (offset > n / 2) offset -= n;
            if (offset < -n / 2) offset += n;

            const abs = Math.abs(offset);
            const dir = Math.sign(offset);
            const config = [
                { x: 0, s: 1, o: 1, b: 0, z: 5 },
                { x: 110, s: .8, o: .7, b: 2, z: 4 },
                { x: 190, s: .62, o: .4, b: 4, z: 3 }
            ][abs] ?? { x: 260, s: .4, o: 0, b: 6, z: 1 };

            slide.style.transform = `translateX(${dir * config.x}%) scale(${config.s})`;
            slide.style.opacity = config.o;
            slide.style.zIndex = config.z;
            slide.style.filter = config.b ? `blur(${config.b}px)` : 'none';
            slide.tabIndex = abs === 0 ? 0 : -1;
        });
    }

    slides.forEach((slide, i) => slide.addEventListener('click', () => {
        if (i === actual) location.href = `reserva.html?id=${slide.dataset.id}`;
        else { actual = i; actualizar(); }
    }));

    document.getElementById('prevSlide').addEventListener('click', () => { actual = (actual - 1 + slides.length) % slides.length; actualizar(); });
    document.getElementById('nextSlide').addEventListener('click', () => { actual = (actual + 1) % slides.length; actualizar(); });

    actualizar();
}
