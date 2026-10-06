// js/confiteria.js

const defaultConfiteria = [
    {
        id: 1,
        nombre: "Combo Familiar",
        descripcion: "2 Pochoclos Grandes + 4 Gaseosas",
        categoria: "Combos",
        precio: 15000,
        stock: 50,
        image: "https://via.placeholder.com/300x200/1e293b/ffffff?text=Combo+Familiar"
    },
    {
        id: 2,
        nombre: "Pochoclos Grandes",
        descripcion: "Balde de pochoclos salados o dulces",
        categoria: "Snacks",
        precio: 5000,
        stock: 100,
        image: "https://via.placeholder.com/300x200/1e293b/ffffff?text=Pochoclos"
    },
    {
        id: 3,
        nombre: "Gaseosa Grande",
        descripcion: "Vaso de 1 Litro (Coca, Sprite, Fanta)",
        categoria: "Bebidas",
        precio: 3500,
        stock: 120,
        image: "https://via.placeholder.com/300x200/1e293b/ffffff?text=Gaseosa"
    }
];

if (!localStorage.getItem('cine_confiteria')) {
    localStorage.setItem('cine_confiteria', JSON.stringify(defaultConfiteria));
}

function getConfiteria() {
    return JSON.parse(localStorage.getItem('cine_confiteria')) || [];
}

function saveConfiteria(productos) {
    localStorage.setItem('cine_confiteria', JSON.stringify(productos));
}
