// js/users.js

const defaultUsers = [
    {
        id: 1,
        nombre: "Administrador",
        email: "admin@cineshollywood.com",
        tipo: "Admin",
        rol: "Administrador"
    },
    {
        id: 2,
        nombre: "Juan Pérez",
        email: "juanperez@ejemplo.com",
        tipo: "Socio VIP",
        rol: "Usuario"
    },
    {
        id: 3,
        nombre: "Maria Gómez",
        email: "mariag@ejemplo.com",
        tipo: "Normal",
        rol: "Usuario"
    }
];

if (!localStorage.getItem('cine_users')) {
    localStorage.setItem('cine_users', JSON.stringify(defaultUsers));
}

function getUsers() {
    return JSON.parse(localStorage.getItem('cine_users')) || [];
}

function saveUsers(users) {
    localStorage.setItem('cine_users', JSON.stringify(users));
}
