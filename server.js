require('dotenv').config();
const express = require('express');
const cors = require('cors');
const sql = require('mssql');
const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');

const app = express();

// Middleware
app.use(cors());
app.use(express.json());

// Servir la carpeta del frontend
const path = require('path');
app.use(express.static(__dirname));

// Configuración de base de datos (puedes ajustar las variables en un archivo .env)
const dbConfig = {
    user: process.env.DB_USER || 'sa',
    password: process.env.DB_PASSWORD || 'tu_password',
    server: process.env.DB_SERVER || 'localhost',
    database: process.env.DB_NAME || 'CineTPI',
    options: {
        encrypt: true, // para azure
        trustServerCertificate: true // útil para desarrollo local
    }
};

// ==========================================
// 1. Autenticación y Usuarios (El patovica) 🛑
// ==========================================

// Middleware para verificar JWT
const verificarToken = (req, res, next) => {
    const token = req.header('Authorization');
    if (!token) return res.status(401).json({ error: 'Acceso denegado. Token no proporcionado.' });

    try {
        const verified = jwt.verify(token.replace('Bearer ', ''), process.env.JWT_SECRET || 'secreto_super_seguro');
        req.usuario = verified;
        next();
    } catch (error) {
        res.status(400).json({ error: 'Token inválido' });
    }
};

app.post('/api/auth/registro', async (req, res) => {
    try {
        const { nombre, apellido, email, password } = req.body;
        // Acá se haría la transacción para crear PERSONA, CLIENTE y USUARIO
        
        // Encriptamos la contraseña
        const salt = await bcrypt.genSalt(10);
        const hashedPassword = await bcrypt.hash(password, salt);

        // Simulamos la inserción en DB y generación de token de verificación
        // await sql.connect(dbConfig);
        // ... Lógica SQL ...

        res.status(201).json({ mensaje: 'Usuario creado exitosamente. Se envió token de verificación.', hashedPassword });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.post('/api/auth/login', async (req, res) => {
    try {
        const { email, password } = req.body;

        // Validar si es el administrador hardcodeado
        if (email === 'admin' && password === 'admin123') {
            const token = jwt.sign({ id: 999, id_rol: 2 }, process.env.JWT_SECRET || 'secreto_super_seguro', { expiresIn: '2h' });
            return res.json({ token, id_rol: 2, mensaje: 'Login exitoso como Administrador' });
        }

        // Simulación: usuario normal
        const id_rol = 1; // 1 = Cliente, 2 = Admin
        const hashEnDb = await bcrypt.hash('1234', 10); // Simulación de contraseña correcta '1234'
        const passwordValido = await bcrypt.compare(password, hashEnDb);

        if (!passwordValido) return res.status(400).json({ error: 'Contraseña incorrecta' });

        // Generar JWT
        const token = jwt.sign({ id: 1, id_rol: id_rol }, process.env.JWT_SECRET || 'secreto_super_seguro', { expiresIn: '2h' });

        res.json({ token, id_rol, mensaje: 'Login exitoso' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get('/api/usuarios/perfil', verificarToken, async (req, res) => {
    try {
        // req.usuario.id tiene el ID del token
        // Hacemos query a la BD para traer datos, si es_jubilado, membresia VIP, etc.
        res.json({
            id_usuario: req.usuario.id,
            nombre: 'Juan',
            apellido: 'Pérez',
            es_jubilado: false,
            membresia_activa: 'VIP'
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ==========================================
// 2. Cartelera y Películas (La carnada) 🍿
// ==========================================

app.get('/api/peliculas', async (req, res) => {
    try {
        const { estado, genero } = req.query;
        // Filtros SQL dependiendo de los query parameters
        // ej: SELECT * FROM PELICULA WHERE estado = @estado...
        
        res.json({
            mensaje: 'Lista de películas',
            filtros_aplicados: { estado, genero },
            data: [] // Acá iría el resultado del pool.query
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get('/api/peliculas/:id_pelicula', async (req, res) => {
    try {
        const { id_pelicula } = req.params;
        // SELECT * FROM PELICULA p JOIN CLASIFICACION c ... JOIN GENERO ...
        
        res.json({
            mensaje: `Detalle de la película ${id_pelicula}`,
            data: { id: id_pelicula, titulo: 'Ejemplo 3D', clasificacion: '+13', idioma: 'Subtitulada' }
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ==========================================
// 3. Funciones y Reservas (La magia) 🎬
// ==========================================

app.get('/api/funciones', async (req, res) => {
    try {
        const { id_pelicula, fecha } = req.query;
        // Búsqueda de horarios y salas disponibles
        
        res.json({
            mensaje: 'Funciones disponibles',
            filtros: { id_pelicula, fecha },
            data: [
                { id_funcion: 101, sala: 3, formato: '3D', horario: '20:30' }
            ]
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get('/api/funciones/:id_funcion/butacas', async (req, res) => {
    try {
        const { id_funcion } = req.params;
        // Traer mapa de SALA cruzando con RESERVA y ENTRADA
        
        res.json({
            mensaje: `Mapa de butacas para la función ${id_funcion}`,
            butacas: [
                { fila: 'A', asiento: 1, estado: 'libre' },
                { fila: 'A', asiento: 2, estado: 'ocupada' }
            ]
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ==========================================
// 4. Confitería y Membresías (Facturación) 🌭
// ==========================================

app.get('/api/membresias', async (req, res) => {
    try {
        res.json({
            mensaje: 'Planes disponibles',
            data: [
                { id_membresia: 1, nombre: 'Socio Classic', descuento: 10 },
                { id_membresia: 2, nombre: 'Socio VIP', descuento: 25 }
            ]
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

app.get('/api/confiteria/productos', async (req, res) => {
    try {
        res.json({
            mensaje: 'Menú de confitería',
            data: [
                { id_producto: 1, categoria: 'Snacks', nombre: 'Pochoclos Grandes', precio: 3500 },
                { id_producto: 2, categoria: 'Bebidas', nombre: 'Gaseosa Grande', precio: 2500 }
            ]
        });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// ==========================================
// Inicializar el Servidor
// ==========================================

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Servidor de Cine TPI corriendo en el puerto ${PORT}`);
});
