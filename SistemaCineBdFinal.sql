create database SistemaineBdFinal
go
use SistemaineBdFinal
go

CREATE TABLE PAIS (
    id_pais INT PRIMARY KEY,
    nombre VARCHAR(100)
);

CREATE TABLE PROVINCIA (
    id_provincia INT PRIMARY KEY,
    id_pais INT FOREIGN KEY REFERENCES PAIS(id_pais),
    nombre VARCHAR(100)
);

CREATE TABLE LOCALIDAD (
    id_localidad INT PRIMARY KEY,
    id_provincia INT FOREIGN KEY REFERENCES PROVINCIA(id_provincia),
    nombre VARCHAR(100)
);

CREATE TABLE SUCURSAL (
    id_sucursal INT PRIMARY KEY,
    id_localidad INT FOREIGN KEY REFERENCES LOCALIDAD(id_localidad),
    calle VARCHAR(200),
    numero INT
);

CREATE TABLE COMPLEJO (
    id_complejo INT PRIMARY KEY,
    id_sucursal INT FOREIGN KEY REFERENCES SUCURSAL(id_sucursal),
    nombre VARCHAR(100)
);

CREATE TABLE TIPO_SALA (
    id_tipo_sala INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE SALA (
    id_sala INT PRIMARY KEY,
    id_complejo INT FOREIGN KEY REFERENCES COMPLEJO(id_complejo),
    id_tipo_sala INT FOREIGN KEY REFERENCES TIPO_SALA(id_tipo_sala),
    nombre VARCHAR(100)
);

CREATE TABLE BUTACA (
    id_butaca INT PRIMARY KEY,
    id_sala INT FOREIGN KEY REFERENCES SALA(id_sala),
    fila VARCHAR(10),
    numero INT,
    habilitada BIT
);

CREATE TABLE PRECIO_TIPO_SALA (
    id_precio INT PRIMARY KEY,
    id_tipo_sala INT FOREIGN KEY REFERENCES TIPO_SALA(id_tipo_sala),
    precio DECIMAL(10, 2),
    fecha_desde DATE,
    fecha_hasta DATE
);

CREATE TABLE CLASIFICACION (
    id_clasificacion INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE GENERO (
    id_genero INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE IDIOMA (
    id_idioma INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE FORMATO (
    id_formato INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE PELICULA (
    id_pelicula INT PRIMARY KEY,
    id_clasificacion INT FOREIGN KEY REFERENCES CLASIFICACION(id_clasificacion),
    titulo VARCHAR(200),
    duracion_minutos INT
);

CREATE TABLE PELICULA_GENERO (
    id_pelicula INT FOREIGN KEY REFERENCES PELICULA(id_pelicula),
    id_genero INT FOREIGN KEY REFERENCES GENERO(id_genero),
    PRIMARY KEY (id_pelicula, id_genero)
);

CREATE TABLE PELICULA_IDIOMA (
    id_pelicula INT FOREIGN KEY REFERENCES PELICULA(id_pelicula),
    id_idioma INT FOREIGN KEY REFERENCES IDIOMA(id_idioma),
    es_doblada BIT,
    PRIMARY KEY (id_pelicula, id_idioma)
);

-- Nuevas tablas de Actores y Directores
CREATE TABLE ACTOR (
    id_actor INT PRIMARY KEY,
    nombre VARCHAR(100),
    apellido VARCHAR(100)
);

CREATE TABLE DIRECTOR (
    id_director INT PRIMARY KEY,
    nombre VARCHAR(100),
    apellido VARCHAR(100)
);

CREATE TABLE PELICULA_ACTOR (
    id_pelicula INT FOREIGN KEY REFERENCES PELICULA(id_pelicula),
    id_actor INT FOREIGN KEY REFERENCES ACTOR(id_actor),
    PRIMARY KEY (id_pelicula, id_actor)
);

CREATE TABLE PELICULA_DIRECTOR (
    id_pelicula INT FOREIGN KEY REFERENCES PELICULA(id_pelicula),
    id_director INT FOREIGN KEY REFERENCES DIRECTOR(id_director),
    PRIMARY KEY (id_pelicula, id_director)
);

CREATE TABLE PROMOCION (
    id_promocion INT PRIMARY KEY,
    descripcion VARCHAR(100),
    porcentaje_descuento DECIMAL(5, 2)
);

CREATE TABLE FUNCION (
    id_funcion INT PRIMARY KEY,
    id_pelicula INT FOREIGN KEY REFERENCES PELICULA(id_pelicula),
    id_sala INT FOREIGN KEY REFERENCES SALA(id_sala),
    id_formato INT FOREIGN KEY REFERENCES FORMATO(id_formato),
    id_promocion INT FOREIGN KEY REFERENCES PROMOCION(id_promocion),
    fecha DATE,
    hora_inicio TIME(7)
);

CREATE TABLE BENEFICIO (
    id_beneficio INT PRIMARY KEY,
    descripcion VARCHAR(200),
    aplica_a_jubilado BIT DEFAULT 0
);

CREATE TABLE TIPO_MEMBRESIA (
    id_tipo_membresia INT PRIMARY KEY,
    nombre VARCHAR(100),
    precio_mensual DECIMAL(10, 2),
    entradas_gratis_por_mes INT
);

CREATE TABLE MEMBRESIA_BENEFICIO (
    id_tipo_membresia INT FOREIGN KEY REFERENCES TIPO_MEMBRESIA(id_tipo_membresia),
    id_beneficio INT FOREIGN KEY REFERENCES BENEFICIO(id_beneficio),
    PRIMARY KEY (id_tipo_membresia, id_beneficio)
);

-- Persona sin la FK de domicilio
CREATE TABLE PERSONA (
    id_persona INT PRIMARY KEY,
    apellido VARCHAR(100),
    nombre VARCHAR(100),
    fecha_nacimiento DATE,
    nro_documento VARCHAR(50)
);

-- Cliente con calle y altura agregados
CREATE TABLE CLIENTE (
    id_cliente INT PRIMARY KEY FOREIGN KEY REFERENCES PERSONA(id_persona),
    es_jubilado BIT DEFAULT 0,
    calle VARCHAR(200),
    altura INT
);

CREATE TABLE CLIENTE_MEMBRESIA (
    id_cliente_membresia INT PRIMARY KEY,
    id_cliente INT FOREIGN KEY REFERENCES CLIENTE(id_cliente),
    id_tipo_membresia INT FOREIGN KEY REFERENCES TIPO_MEMBRESIA(id_tipo_membresia),
    fecha_inicio DATE,
    fecha_proximo_cobro DATE,
    estado BIT,
    entradas_gratis_usadas_mes INT DEFAULT 0
);

CREATE TABLE TIPO_CONTACTO (
    id_tipo_contacto INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE CONTACTO (
    id_contacto INT PRIMARY KEY,
    id_persona INT FOREIGN KEY REFERENCES PERSONA(id_persona),
    id_tipo_contacto INT FOREIGN KEY REFERENCES TIPO_CONTACTO(id_tipo_contacto),
    valor VARCHAR(255)
);

CREATE TABLE DEPARTAMENTO (
    id_departamento INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE PUESTO (
    id_puesto INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE EMPLEADO (
    id_empleado INT PRIMARY KEY FOREIGN KEY REFERENCES PERSONA(id_persona),
    id_puesto INT FOREIGN KEY REFERENCES PUESTO(id_puesto),
    id_departamento INT FOREIGN KEY REFERENCES DEPARTAMENTO(id_departamento),
    legajo VARCHAR(50),
    fecha_ingreso DATE
);

CREATE TABLE TURNO (
    id_turno INT PRIMARY KEY,
    descripcion VARCHAR(100),
    hora_desde TIME(7),
    hora_hasta TIME(7)
);

CREATE TABLE EMPLEADO_TURNO (
    id_empleado INT FOREIGN KEY REFERENCES EMPLEADO(id_empleado),
    id_turno INT FOREIGN KEY REFERENCES TURNO(id_turno),
    fecha DATE,
    PRIMARY KEY (id_empleado, id_turno, fecha)
);

CREATE TABLE CANAL_VENTA (
    id_canal INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

-- Compra renombrada a Venta
CREATE TABLE VENTA (
    id_venta INT PRIMARY KEY,
    id_cliente INT FOREIGN KEY REFERENCES CLIENTE(id_cliente),
    id_empleado INT FOREIGN KEY REFERENCES EMPLEADO(id_empleado),
    id_canal INT FOREIGN KEY REFERENCES CANAL_VENTA(id_canal),
    fecha_hora DATETIME
);

CREATE TABLE FORMA_PAGO (
    id_forma_pago INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

-- Detalle_Compra renombrado a VentaxFormaPago
CREATE TABLE VentaxFormaPago (
    id_ventaxformapago INT PRIMARY KEY,
    id_venta INT FOREIGN KEY REFERENCES VENTA(id_venta),
    id_forma_pago INT FOREIGN KEY REFERENCES FORMA_PAGO(id_forma_pago),
    monto DECIMAL(10, 2)
);

CREATE TABLE CATEGORIA_PRODUCTO (
    id_categoria_prod INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE PRODUCTO_CONFITERIA (
    id_producto INT PRIMARY KEY,
    id_categoria_prod INT FOREIGN KEY REFERENCES CATEGORIA_PRODUCTO(id_categoria_prod),
    nombre VARCHAR(150),
    precio_actual DECIMAL(10, 2),
    stock INT
);

-- Detalle_Compra_Confiteria ajustada a Venta
CREATE TABLE DETALLE_VENTA_CONFITERIA (
    id_detalle_confiteria INT PRIMARY KEY,
    id_venta INT FOREIGN KEY REFERENCES VENTA(id_venta),
    id_producto INT FOREIGN KEY REFERENCES PRODUCTO_CONFITERIA(id_producto),
    cantidad INT,
    precio_unitario DECIMAL(10, 2)
);

CREATE TABLE ESTADO_RESERVA (
    id_estado_reserva INT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE RESERVA (
    id_reserva INT PRIMARY KEY,
    id_cliente INT FOREIGN KEY REFERENCES CLIENTE(id_cliente),
    id_funcion INT FOREIGN KEY REFERENCES FUNCION(id_funcion),
    id_butaca INT FOREIGN KEY REFERENCES BUTACA(id_butaca),
    id_estado_reserva INT FOREIGN KEY REFERENCES ESTADO_RESERVA(id_estado_reserva),
    fecha_hora_reserva DATETIME,
    fecha_hora_vencimiento DATETIME
);

CREATE TABLE TIPO_ESPECTADOR (
    id_tipo_espectador INT PRIMARY KEY,
    descripcion VARCHAR(50)
);

CREATE TABLE ENTRADA (
    id_entrada INT PRIMARY KEY,
    id_venta INT FOREIGN KEY REFERENCES VENTA(id_venta),
    id_reserva INT FOREIGN KEY REFERENCES RESERVA(id_reserva),
    id_funcion INT FOREIGN KEY REFERENCES FUNCION(id_funcion),
    id_butaca INT FOREIGN KEY REFERENCES BUTACA(id_butaca),
    id_tipo_espectador INT FOREIGN KEY REFERENCES TIPO_ESPECTADOR(id_tipo_espectador),
    precio_final DECIMAL(10, 2)
);

CREATE TABLE ESTADO_USUARIO (
    id_estado_usuario INT PRIMARY KEY,
    descripcion VARCHAR(50)
);

CREATE TABLE ROL_USUARIO (
    id_rol INT PRIMARY KEY,
    nombre VARCHAR(50),
    descripcion VARCHAR(200)
);

CREATE TABLE USUARIO (
    id_usuario INT PRIMARY KEY,
    id_persona INT UNIQUE FOREIGN KEY REFERENCES PERSONA(id_persona),
    id_rol INT FOREIGN KEY REFERENCES ROL_USUARIO(id_rol),
    email VARCHAR(150) UNIQUE,
    password_hash VARCHAR(255),
    id_estado_usuario INT FOREIGN KEY REFERENCES ESTADO_USUARIO(id_estado_usuario),
    fecha_registro DATETIME
);

CREATE TABLE TOKEN_VERIFICACION (
    id_token INT PRIMARY KEY,
    id_usuario INT FOREIGN KEY REFERENCES USUARIO(id_usuario),
    token VARCHAR(255),
    fecha_expiracion DATETIME,
    usado BIT
);