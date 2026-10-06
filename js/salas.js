// js/salas.js

const defaultSalas = [
    {
        id: 1,
        nombre: "Sala 1",
        tipo: "2D Regular",
        capacidad: 150
    },
    {
        id: 2,
        nombre: "Sala 2",
        tipo: "3D",
        capacidad: 150
    },
    {
        id: 3,
        nombre: "Sala VIP",
        tipo: "Premium / VIP",
        capacidad: 80
    }
];

if (!localStorage.getItem('cine_salas')) {
    localStorage.setItem('cine_salas', JSON.stringify(defaultSalas));
}

function getSalas() {
    return JSON.parse(localStorage.getItem('cine_salas')) || [];
}

function saveSalas(salas) {
    localStorage.setItem('cine_salas', JSON.stringify(salas));
}
