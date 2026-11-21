-- =====================================================================
-- SistemaCineBdFinal - estructura y datos actuales (generado desde la base real)
-- Ejecutar completo en SQL Server Management Studio.
-- =====================================================================
IF DB_ID('SistemaCineBdFinal') IS NULL CREATE DATABASE SistemaCineBdFinal;
GO
USE SistemaCineBdFinal;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PERSONA](
	[id_persona] [int] IDENTITY(1,1) NOT NULL,
	[apellido] [varchar](100) NULL,
	[nombre] [varchar](100) NULL,
	[fecha_nacimiento] [date] NULL,
	[nro_documento] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_persona] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IDIOMA](
	[id_idioma] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_idioma] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GENERO](
	[id_genero] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_genero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAIS](
	[id_pais] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pais] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ROL_USUARIO](
	[id_rol] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](50) NULL,
	[descripcion] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PUESTO](
	[id_puesto] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_puesto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PROMOCION](
	[id_promocion] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
	[porcentaje_descuento] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_promocion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FORMATO](
	[id_formato] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_formato] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FORMA_PAGO](
	[id_forma_pago] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_forma_pago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ESTADO_USUARIO](
	[id_estado_usuario] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_estado_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ESTADO_RESERVA](
	[id_estado_reserva] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_estado_reserva] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DIRECTOR](
	[id_director] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NULL,
	[apellido] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_director] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DEPARTAMENTO](
	[id_departamento] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_departamento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CLASIFICACION](
	[id_clasificacion] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_clasificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CATEGORIA_PRODUCTO](
	[id_categoria_prod] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_categoria_prod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CANAL_VENTA](
	[id_canal] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_canal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BENEFICIO](
	[id_beneficio] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](200) NULL,
	[aplica_a_jubilado] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_beneficio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ACTOR](
	[id_actor] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NULL,
	[apellido] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_actor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TIPO_SALA](
	[id_tipo_sala] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tipo_sala] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TIPO_MEMBRESIA](
	[id_tipo_membresia] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](100) NULL,
	[precio_mensual] [decimal](10, 2) NULL,
	[entradas_gratis_por_mes] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tipo_membresia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TIPO_ESPECTADOR](
	[id_tipo_espectador] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tipo_espectador] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TIPO_CONTACTO](
	[id_tipo_contacto] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tipo_contacto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TURNO](
	[id_turno] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [varchar](100) NULL,
	[hora_desde] [time](7) NULL,
	[hora_hasta] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_turno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[USUARIO](
	[id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[id_persona] [int] NULL,
	[id_rol] [int] NULL,
	[email] [varchar](150) NULL,
	[password_hash] [varchar](255) NULL,
	[id_estado_usuario] [int] NULL,
	[fecha_registro] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CLIENTE](
	[id_cliente] [int] NOT NULL,
	[es_jubilado] [bit] NULL,
	[calle] [varchar](200) NULL,
	[altura] [int] NULL,
	[activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CONTACTO](
	[id_contacto] [int] IDENTITY(1,1) NOT NULL,
	[id_persona] [int] NULL,
	[id_tipo_contacto] [int] NULL,
	[valor] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_contacto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMPLEADO](
	[id_empleado] [int] NOT NULL,
	[id_puesto] [int] NULL,
	[id_departamento] [int] NULL,
	[legajo] [varchar](50) NULL,
	[fecha_ingreso] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_empleado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PRODUCTO_CONFITERIA](
	[id_producto] [int] IDENTITY(1,1) NOT NULL,
	[id_categoria_prod] [int] NULL,
	[nombre] [varchar](150) NULL,
	[precio_actual] [decimal](10, 2) NULL,
	[stock] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PRECIO_TIPO_SALA](
	[id_precio] [int] IDENTITY(1,1) NOT NULL,
	[id_tipo_sala] [int] NULL,
	[precio] [decimal](10, 2) NULL,
	[fecha_desde] [date] NULL,
	[fecha_hasta] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_precio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PELICULA](
	[id_pelicula] [int] IDENTITY(1,1) NOT NULL,
	[id_clasificacion] [int] NULL,
	[titulo] [varchar](200) NULL,
	[duracion_minutos] [int] NULL,
	[activo] [bit] NOT NULL,
	[PosterUrl] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pelicula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PROVINCIA](
	[id_provincia] [int] IDENTITY(1,1) NOT NULL,
	[id_pais] [int] NULL,
	[nombre] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_provincia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MEMBRESIA_BENEFICIO](
	[id_tipo_membresia] [int] NOT NULL,
	[id_beneficio] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_tipo_membresia] ASC,
	[id_beneficio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PELICULA_IDIOMA](
	[id_pelicula] [int] NOT NULL,
	[id_idioma] [int] NOT NULL,
	[es_doblada] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pelicula] ASC,
	[id_idioma] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PELICULA_GENERO](
	[id_pelicula] [int] NOT NULL,
	[id_genero] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pelicula] ASC,
	[id_genero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PELICULA_DIRECTOR](
	[id_pelicula] [int] NOT NULL,
	[id_director] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pelicula] ASC,
	[id_director] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PELICULA_ACTOR](
	[id_pelicula] [int] NOT NULL,
	[id_actor] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pelicula] ASC,
	[id_actor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOCALIDAD](
	[id_localidad] [int] IDENTITY(1,1) NOT NULL,
	[id_provincia] [int] NULL,
	[nombre] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_localidad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VENTA](
	[id_venta] [int] IDENTITY(1,1) NOT NULL,
	[id_cliente] [int] NULL,
	[id_empleado] [int] NULL,
	[id_canal] [int] NULL,
	[fecha_hora] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_venta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TOKEN_VERIFICACION](
	[id_token] [int] IDENTITY(1,1) NOT NULL,
	[id_usuario] [int] NULL,
	[token] [varchar](255) NULL,
	[fecha_expiracion] [datetime] NULL,
	[usado] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMPLEADO_TURNO](
	[id_empleado] [int] NOT NULL,
	[id_turno] [int] NOT NULL,
	[fecha] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_empleado] ASC,
	[id_turno] ASC,
	[fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CLIENTE_MEMBRESIA](
	[id_cliente_membresia] [int] IDENTITY(1,1) NOT NULL,
	[id_cliente] [int] NULL,
	[id_tipo_membresia] [int] NULL,
	[fecha_inicio] [date] NULL,
	[fecha_proximo_cobro] [date] NULL,
	[estado] [bit] NULL,
	[entradas_gratis_usadas_mes] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_cliente_membresia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VentaxFormaPago](
	[id_ventaxformapago] [int] IDENTITY(1,1) NOT NULL,
	[id_venta] [int] NULL,
	[id_forma_pago] [int] NULL,
	[monto] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_ventaxformapago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SUCURSAL](
	[id_sucursal] [int] IDENTITY(1,1) NOT NULL,
	[id_localidad] [int] NULL,
	[calle] [varchar](200) NULL,
	[numero] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_sucursal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DETALLE_VENTA_CONFITERIA](
	[id_detalle_confiteria] [int] IDENTITY(1,1) NOT NULL,
	[id_venta] [int] NULL,
	[id_producto] [int] NULL,
	[cantidad] [int] NULL,
	[precio_unitario] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_detalle_confiteria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[COMPLEJO](
	[id_complejo] [int] IDENTITY(1,1) NOT NULL,
	[id_sucursal] [int] NULL,
	[nombre] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_complejo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SALA](
	[id_sala] [int] IDENTITY(1,1) NOT NULL,
	[id_complejo] [int] NULL,
	[id_tipo_sala] [int] NULL,
	[nombre] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_sala] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BUTACA](
	[id_butaca] [int] IDENTITY(1,1) NOT NULL,
	[id_sala] [int] NULL,
	[fila] [varchar](10) NULL,
	[numero] [int] NULL,
	[habilitada] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_butaca] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FUNCION](
	[id_funcion] [int] IDENTITY(1,1) NOT NULL,
	[id_pelicula] [int] NULL,
	[id_sala] [int] NULL,
	[id_formato] [int] NULL,
	[id_promocion] [int] NULL,
	[fecha] [date] NULL,
	[hora_inicio] [time](7) NULL,
	[activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_funcion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RESERVA](
	[id_reserva] [int] IDENTITY(1,1) NOT NULL,
	[id_cliente] [int] NULL,
	[id_funcion] [int] NULL,
	[id_butaca] [int] NULL,
	[id_estado_reserva] [int] NULL,
	[fecha_hora_reserva] [datetime] NULL,
	[fecha_hora_vencimiento] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_reserva] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ENTRADA](
	[id_entrada] [int] IDENTITY(1,1) NOT NULL,
	[id_venta] [int] NULL,
	[id_reserva] [int] NULL,
	[id_funcion] [int] NULL,
	[id_butaca] [int] NULL,
	[id_tipo_espectador] [int] NULL,
	[precio_final] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id_entrada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)

GO
SET IDENTITY_INSERT [dbo].[PERSONA] ON 

GO
INSERT [dbo].[PERSONA] ([id_persona], [apellido], [nombre], [fecha_nacimiento], [nro_documento]) VALUES (1, N'Pérez', N'Juan', CAST(N'1995-05-12' AS Date), N'38555444')
GO
INSERT [dbo].[PERSONA] ([id_persona], [apellido], [nombre], [fecha_nacimiento], [nro_documento]) VALUES (2, N'Gómez', N'María', CAST(N'1988-10-20' AS Date), N'31222333')
GO
INSERT [dbo].[PERSONA] ([id_persona], [apellido], [nombre], [fecha_nacimiento], [nro_documento]) VALUES (3, N'Sistema', N'Admin', NULL, NULL)
GO
INSERT [dbo].[PERSONA] ([id_persona], [apellido], [nombre], [fecha_nacimiento], [nro_documento]) VALUES (5, N'toma', N'matias', CAST(N'2003-04-11' AS Date), N'42154124')
GO
SET IDENTITY_INSERT [dbo].[PERSONA] OFF
GO
SET IDENTITY_INSERT [dbo].[IDIOMA] ON 

GO
INSERT [dbo].[IDIOMA] ([id_idioma], [descripcion]) VALUES (1, N'Inglés')
GO
INSERT [dbo].[IDIOMA] ([id_idioma], [descripcion]) VALUES (2, N'Español')
GO
SET IDENTITY_INSERT [dbo].[IDIOMA] OFF
GO
SET IDENTITY_INSERT [dbo].[GENERO] ON 

GO
INSERT [dbo].[GENERO] ([id_genero], [descripcion]) VALUES (1, N'Acción')
GO
INSERT [dbo].[GENERO] ([id_genero], [descripcion]) VALUES (2, N'Ciencia Ficción')
GO
SET IDENTITY_INSERT [dbo].[GENERO] OFF
GO
SET IDENTITY_INSERT [dbo].[PAIS] ON 

GO
INSERT [dbo].[PAIS] ([id_pais], [nombre]) VALUES (1, N'Argentina')
GO
SET IDENTITY_INSERT [dbo].[PAIS] OFF
GO
SET IDENTITY_INSERT [dbo].[ROL_USUARIO] ON 

GO
INSERT [dbo].[ROL_USUARIO] ([id_rol], [nombre], [descripcion]) VALUES (1, N'Administrador', N'Acceso total')
GO
INSERT [dbo].[ROL_USUARIO] ([id_rol], [nombre], [descripcion]) VALUES (2, N'Cliente', N'Usuario estándar')
GO
SET IDENTITY_INSERT [dbo].[ROL_USUARIO] OFF
GO
SET IDENTITY_INSERT [dbo].[PUESTO] ON 

GO
INSERT [dbo].[PUESTO] ([id_puesto], [descripcion]) VALUES (1, N'Boletero / Candy Bar')
GO
SET IDENTITY_INSERT [dbo].[PUESTO] OFF
GO
SET IDENTITY_INSERT [dbo].[PROMOCION] ON 

GO
INSERT [dbo].[PROMOCION] ([id_promocion], [descripcion], [porcentaje_descuento]) VALUES (1, N'2x1 Lunes y Martes', CAST(50.00 AS Decimal(5, 2)))
GO
SET IDENTITY_INSERT [dbo].[PROMOCION] OFF
GO
SET IDENTITY_INSERT [dbo].[FORMATO] ON 

GO
INSERT [dbo].[FORMATO] ([id_formato], [descripcion]) VALUES (1, N'Subtitulado')
GO
INSERT [dbo].[FORMATO] ([id_formato], [descripcion]) VALUES (2, N'Doblada')
GO
SET IDENTITY_INSERT [dbo].[FORMATO] OFF
GO
SET IDENTITY_INSERT [dbo].[FORMA_PAGO] ON 

GO
INSERT [dbo].[FORMA_PAGO] ([id_forma_pago], [descripcion]) VALUES (1, N'Efectivo')
GO
INSERT [dbo].[FORMA_PAGO] ([id_forma_pago], [descripcion]) VALUES (2, N'Tarjeta de Crédito')
GO
SET IDENTITY_INSERT [dbo].[FORMA_PAGO] OFF
GO
SET IDENTITY_INSERT [dbo].[ESTADO_USUARIO] ON 

GO
INSERT [dbo].[ESTADO_USUARIO] ([id_estado_usuario], [descripcion]) VALUES (1, N'Activo')
GO
INSERT [dbo].[ESTADO_USUARIO] ([id_estado_usuario], [descripcion]) VALUES (2, N'Inactivo')
GO
SET IDENTITY_INSERT [dbo].[ESTADO_USUARIO] OFF
GO
SET IDENTITY_INSERT [dbo].[ESTADO_RESERVA] ON 

GO
INSERT [dbo].[ESTADO_RESERVA] ([id_estado_reserva], [descripcion]) VALUES (1, N'Pendiente')
GO
INSERT [dbo].[ESTADO_RESERVA] ([id_estado_reserva], [descripcion]) VALUES (2, N'Confirmada')
GO
INSERT [dbo].[ESTADO_RESERVA] ([id_estado_reserva], [descripcion]) VALUES (3, N'Cancelada')
GO
SET IDENTITY_INSERT [dbo].[ESTADO_RESERVA] OFF
GO
SET IDENTITY_INSERT [dbo].[DIRECTOR] ON 

GO
INSERT [dbo].[DIRECTOR] ([id_director], [nombre], [apellido]) VALUES (1, N'Lana', N'Wachowski')
GO
SET IDENTITY_INSERT [dbo].[DIRECTOR] OFF
GO
SET IDENTITY_INSERT [dbo].[DEPARTAMENTO] ON 

GO
INSERT [dbo].[DEPARTAMENTO] ([id_departamento], [descripcion]) VALUES (1, N'Atención al Cliente')
GO
SET IDENTITY_INSERT [dbo].[DEPARTAMENTO] OFF
GO
SET IDENTITY_INSERT [dbo].[CLASIFICACION] ON 

GO
INSERT [dbo].[CLASIFICACION] ([id_clasificacion], [descripcion]) VALUES (1, N'+13')
GO
INSERT [dbo].[CLASIFICACION] ([id_clasificacion], [descripcion]) VALUES (2, N'ATP')
GO
INSERT [dbo].[CLASIFICACION] ([id_clasificacion], [descripcion]) VALUES (3, N'+16')
GO
INSERT [dbo].[CLASIFICACION] ([id_clasificacion], [descripcion]) VALUES (4, N'+18')
GO
SET IDENTITY_INSERT [dbo].[CLASIFICACION] OFF
GO
SET IDENTITY_INSERT [dbo].[CATEGORIA_PRODUCTO] ON 

GO
INSERT [dbo].[CATEGORIA_PRODUCTO] ([id_categoria_prod], [descripcion]) VALUES (1, N'Bebidas')
GO
INSERT [dbo].[CATEGORIA_PRODUCTO] ([id_categoria_prod], [descripcion]) VALUES (2, N'Golosinas y Pochoclos')
GO
SET IDENTITY_INSERT [dbo].[CATEGORIA_PRODUCTO] OFF
GO
SET IDENTITY_INSERT [dbo].[CANAL_VENTA] ON 

GO
INSERT [dbo].[CANAL_VENTA] ([id_canal], [descripcion]) VALUES (1, N'Boletería Presencial')
GO
INSERT [dbo].[CANAL_VENTA] ([id_canal], [descripcion]) VALUES (2, N'Web / App Móvil')
GO
SET IDENTITY_INSERT [dbo].[CANAL_VENTA] OFF
GO
SET IDENTITY_INSERT [dbo].[BENEFICIO] ON 

GO
INSERT [dbo].[BENEFICIO] ([id_beneficio], [descripcion], [aplica_a_jubilado]) VALUES (1, N'Entradas gratis mensuales', 1)
GO
SET IDENTITY_INSERT [dbo].[BENEFICIO] OFF
GO
SET IDENTITY_INSERT [dbo].[ACTOR] ON 

GO
INSERT [dbo].[ACTOR] ([id_actor], [nombre], [apellido]) VALUES (1, N'Keanu', N'Reeves')
GO
INSERT [dbo].[ACTOR] ([id_actor], [nombre], [apellido]) VALUES (2, N'Carrie-Anne', N'Moss')
GO
SET IDENTITY_INSERT [dbo].[ACTOR] OFF
GO
SET IDENTITY_INSERT [dbo].[TIPO_SALA] ON 

GO
INSERT [dbo].[TIPO_SALA] ([id_tipo_sala], [descripcion]) VALUES (1, N'2D Estándar')
GO
INSERT [dbo].[TIPO_SALA] ([id_tipo_sala], [descripcion]) VALUES (2, N'3D XD')
GO
SET IDENTITY_INSERT [dbo].[TIPO_SALA] OFF
GO
SET IDENTITY_INSERT [dbo].[TIPO_MEMBRESIA] ON 

GO
INSERT [dbo].[TIPO_MEMBRESIA] ([id_tipo_membresia], [nombre], [precio_mensual], [entradas_gratis_por_mes]) VALUES (1, N'Club Cine Premium', CAST(4500.00 AS Decimal(10, 2)), 2)
GO
SET IDENTITY_INSERT [dbo].[TIPO_MEMBRESIA] OFF
GO
SET IDENTITY_INSERT [dbo].[TIPO_ESPECTADOR] ON 

GO
INSERT [dbo].[TIPO_ESPECTADOR] ([id_tipo_espectador], [descripcion]) VALUES (1, N'General')
GO
INSERT [dbo].[TIPO_ESPECTADOR] ([id_tipo_espectador], [descripcion]) VALUES (2, N'Jubilado')
GO
INSERT [dbo].[TIPO_ESPECTADOR] ([id_tipo_espectador], [descripcion]) VALUES (3, N'Menor')
GO
SET IDENTITY_INSERT [dbo].[TIPO_ESPECTADOR] OFF
GO
SET IDENTITY_INSERT [dbo].[USUARIO] ON 

GO
INSERT [dbo].[USUARIO] ([id_usuario], [id_persona], [id_rol], [email], [password_hash], [id_estado_usuario], [fecha_registro]) VALUES (1, 1, 2, N'juan.perez@email.com', N'HASH_FAKE_123', 1, CAST(N'2026-10-27T13:21:00.490' AS DateTime))
GO
INSERT [dbo].[USUARIO] ([id_usuario], [id_persona], [id_rol], [email], [password_hash], [id_estado_usuario], [fecha_registro]) VALUES (2, 3, 1, N'admin@cinehollywood.com', N'AQAAAAIAAYagAAAAEBrffW2FXbZ3X56sTx8W78or83nvrK1KQaNr3L0pb/ud31Bckabx76djnc2jDL8ISQ==', 1, CAST(N'2026-11-05T11:08:53.990' AS DateTime))
GO
INSERT [dbo].[USUARIO] ([id_usuario], [id_persona], [id_rol], [email], [password_hash], [id_estado_usuario], [fecha_registro]) VALUES (4, 5, 2, N'mtomatis77@gmail.com', N'AQAAAAIAAYagAAAAEK1Zob77FMLkmxBQrcDCR8iA+5FnsYI/koT1/b87bSL3HgTV3nkJ6E8T6N4PV45Q/Q==', 1, CAST(N'2026-11-05T11:24:11.563' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[USUARIO] OFF
GO
INSERT [dbo].[CLIENTE] ([id_cliente], [es_jubilado], [calle], [altura], [activo]) VALUES (1, 0, N'San Jerónimo', 500, 1)
GO
INSERT [dbo].[CLIENTE] ([id_cliente], [es_jubilado], [calle], [altura], [activo]) VALUES (5, 0, NULL, NULL, 1)
GO
INSERT [dbo].[EMPLEADO] ([id_empleado], [id_puesto], [id_departamento], [legajo], [fecha_ingreso]) VALUES (2, 1, 1, N'EMP-001', CAST(N'2023-01-15' AS Date))
GO
SET IDENTITY_INSERT [dbo].[PRODUCTO_CONFITERIA] ON 

GO
INSERT [dbo].[PRODUCTO_CONFITERIA] ([id_producto], [id_categoria_prod], [nombre], [precio_actual], [stock]) VALUES (1, 1, N'Coca Cola 500ml', CAST(2500.00 AS Decimal(10, 2)), 100)
GO
INSERT [dbo].[PRODUCTO_CONFITERIA] ([id_producto], [id_categoria_prod], [nombre], [precio_actual], [stock]) VALUES (2, 2, N'Pochoclo Mediano', CAST(3500.00 AS Decimal(10, 2)), 50)
GO
SET IDENTITY_INSERT [dbo].[PRODUCTO_CONFITERIA] OFF
GO
SET IDENTITY_INSERT [dbo].[PRECIO_TIPO_SALA] ON 

GO
INSERT [dbo].[PRECIO_TIPO_SALA] ([id_precio], [id_tipo_sala], [precio], [fecha_desde], [fecha_hasta]) VALUES (1, 1, CAST(4000.00 AS Decimal(10, 2)), CAST(N'2026-01-01' AS Date), CAST(N'2026-11-04' AS Date))
GO
INSERT [dbo].[PRECIO_TIPO_SALA] ([id_precio], [id_tipo_sala], [precio], [fecha_desde], [fecha_hasta]) VALUES (4, 1, CAST(6000.00 AS Decimal(10, 2)), CAST(N'2026-11-05' AS Date), NULL)
GO
INSERT [dbo].[PRECIO_TIPO_SALA] ([id_precio], [id_tipo_sala], [precio], [fecha_desde], [fecha_hasta]) VALUES (5, 2, CAST(10000.00 AS Decimal(10, 2)), CAST(N'2026-11-05' AS Date), NULL)
GO
SET IDENTITY_INSERT [dbo].[PRECIO_TIPO_SALA] OFF
GO
SET IDENTITY_INSERT [dbo].[PELICULA] ON 

GO
INSERT [dbo].[PELICULA] ([id_pelicula], [id_clasificacion], [titulo], [duracion_minutos], [activo], [PosterUrl]) VALUES (1, 1, N'Matrix Resurrecciones', 148, 1, N'/posters/b63cee5853b6432fab3f6d9b0b839142.jpg')
GO
INSERT [dbo].[PELICULA] ([id_pelicula], [id_clasificacion], [titulo], [duracion_minutos], [activo], [PosterUrl]) VALUES (2, 1, N'SHREK', 120, 0, NULL)
GO
INSERT [dbo].[PELICULA] ([id_pelicula], [id_clasificacion], [titulo], [duracion_minutos], [activo], [PosterUrl]) VALUES (3, 2, N'Cars 2', 116, 1, N'/posters/d651c447daac4e909ad4bf46bd8c4295.jpg')
GO
INSERT [dbo].[PELICULA] ([id_pelicula], [id_clasificacion], [titulo], [duracion_minutos], [activo], [PosterUrl]) VALUES (4, 1, N'Shrek 2', 136, 1, N'https://images.contentstack.io/v3/assets/blt13adb7e2033fcee5/blt5a0b43bd1a6f24fb/6ae4f16daaf3510bf82d9d80/Shrek_2000x3000_keyart_Poster.jpg?width=300')
GO
INSERT [dbo].[PELICULA] ([id_pelicula], [id_clasificacion], [titulo], [duracion_minutos], [activo], [PosterUrl]) VALUES (6, 2, N'El Gato con Botas', 140, 1, N'/posters/a3fc4f2c3d51423aa10830854f19e295.jpg')
GO
SET IDENTITY_INSERT [dbo].[PELICULA] OFF
GO
SET IDENTITY_INSERT [dbo].[PROVINCIA] ON 

GO
INSERT [dbo].[PROVINCIA] ([id_provincia], [id_pais], [nombre]) VALUES (1, 1, N'Córdoba')
GO
SET IDENTITY_INSERT [dbo].[PROVINCIA] OFF
GO
INSERT [dbo].[MEMBRESIA_BENEFICIO] ([id_tipo_membresia], [id_beneficio]) VALUES (1, 1)
GO
INSERT [dbo].[PELICULA_IDIOMA] ([id_pelicula], [id_idioma], [es_doblada]) VALUES (1, 1, 0)
GO
INSERT [dbo].[PELICULA_GENERO] ([id_pelicula], [id_genero]) VALUES (1, 1)
GO
INSERT [dbo].[PELICULA_GENERO] ([id_pelicula], [id_genero]) VALUES (1, 2)
GO
INSERT [dbo].[PELICULA_DIRECTOR] ([id_pelicula], [id_director]) VALUES (1, 1)
GO
INSERT [dbo].[PELICULA_ACTOR] ([id_pelicula], [id_actor]) VALUES (1, 1)
GO
INSERT [dbo].[PELICULA_ACTOR] ([id_pelicula], [id_actor]) VALUES (1, 2)
GO
SET IDENTITY_INSERT [dbo].[LOCALIDAD] ON 

GO
INSERT [dbo].[LOCALIDAD] ([id_localidad], [id_provincia], [nombre]) VALUES (1, 1, N'Córdoba Capital')
GO
SET IDENTITY_INSERT [dbo].[LOCALIDAD] OFF
GO
SET IDENTITY_INSERT [dbo].[VENTA] ON 

GO
INSERT [dbo].[VENTA] ([id_venta], [id_cliente], [id_empleado], [id_canal], [fecha_hora]) VALUES (1, 1, 2, 1, CAST(N'2026-10-27T13:21:00.497' AS DateTime))
GO
INSERT [dbo].[VENTA] ([id_venta], [id_cliente], [id_empleado], [id_canal], [fecha_hora]) VALUES (3, 5, NULL, 2, CAST(N'2026-11-05T11:33:23.543' AS DateTime))
GO
INSERT [dbo].[VENTA] ([id_venta], [id_cliente], [id_empleado], [id_canal], [fecha_hora]) VALUES (4, 5, NULL, 2, CAST(N'2026-11-05T11:35:21.040' AS DateTime))
GO
INSERT [dbo].[VENTA] ([id_venta], [id_cliente], [id_empleado], [id_canal], [fecha_hora]) VALUES (5, 5, NULL, 2, CAST(N'2026-11-05T12:19:30.460' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[VENTA] OFF
GO
SET IDENTITY_INSERT [dbo].[CLIENTE_MEMBRESIA] ON 

GO
INSERT [dbo].[CLIENTE_MEMBRESIA] ([id_cliente_membresia], [id_cliente], [id_tipo_membresia], [fecha_inicio], [fecha_proximo_cobro], [estado], [entradas_gratis_usadas_mes]) VALUES (1, 1, 1, CAST(N'2026-10-01' AS Date), CAST(N'2026-11-01' AS Date), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[CLIENTE_MEMBRESIA] OFF
GO
SET IDENTITY_INSERT [dbo].[VentaxFormaPago] ON 

GO
INSERT [dbo].[VentaxFormaPago] ([id_ventaxformapago], [id_venta], [id_forma_pago], [monto]) VALUES (1, 1, 2, CAST(9000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[VentaxFormaPago] ([id_ventaxformapago], [id_venta], [id_forma_pago], [monto]) VALUES (3, 3, 1, CAST(4000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[VentaxFormaPago] ([id_ventaxformapago], [id_venta], [id_forma_pago], [monto]) VALUES (4, 4, 1, CAST(8000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[VentaxFormaPago] ([id_ventaxformapago], [id_venta], [id_forma_pago], [monto]) VALUES (5, 5, 1, CAST(20000.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[VentaxFormaPago] OFF
GO
SET IDENTITY_INSERT [dbo].[SUCURSAL] ON 

GO
INSERT [dbo].[SUCURSAL] ([id_sucursal], [id_localidad], [calle], [numero]) VALUES (1, 1, N'Av. Vélez Sarsfield', 350)
GO
SET IDENTITY_INSERT [dbo].[SUCURSAL] OFF
GO
SET IDENTITY_INSERT [dbo].[DETALLE_VENTA_CONFITERIA] ON 

GO
INSERT [dbo].[DETALLE_VENTA_CONFITERIA] ([id_detalle_confiteria], [id_venta], [id_producto], [cantidad], [precio_unitario]) VALUES (1, 1, 1, 2, CAST(2500.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[DETALLE_VENTA_CONFITERIA] OFF
GO
SET IDENTITY_INSERT [dbo].[COMPLEJO] ON 

GO
INSERT [dbo].[COMPLEJO] ([id_complejo], [id_sucursal], [nombre]) VALUES (1, 1, N'CineCenter Patio Olmos')
GO
SET IDENTITY_INSERT [dbo].[COMPLEJO] OFF
GO
SET IDENTITY_INSERT [dbo].[SALA] ON 

GO
INSERT [dbo].[SALA] ([id_sala], [id_complejo], [id_tipo_sala], [nombre]) VALUES (1, 1, 1, N'Sala 1')
GO
INSERT [dbo].[SALA] ([id_sala], [id_complejo], [id_tipo_sala], [nombre]) VALUES (3, 1, 2, N'Sala 2')
GO
SET IDENTITY_INSERT [dbo].[SALA] OFF
GO
SET IDENTITY_INSERT [dbo].[BUTACA] ON 

GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (1, 1, N'A', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (2, 1, N'A', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (3, 1, N'B', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (26, 3, N'A', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (27, 3, N'A', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (28, 3, N'A', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (29, 3, N'A', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (30, 3, N'A', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (31, 3, N'A', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (32, 3, N'A', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (33, 3, N'A', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (34, 3, N'A', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (35, 3, N'A', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (36, 3, N'A', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (37, 3, N'A', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (38, 3, N'B', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (39, 3, N'B', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (40, 3, N'B', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (41, 3, N'B', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (42, 3, N'B', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (43, 3, N'B', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (44, 3, N'B', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (45, 3, N'B', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (46, 3, N'B', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (47, 3, N'B', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (48, 3, N'B', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (49, 3, N'B', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (50, 3, N'C', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (51, 3, N'C', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (52, 3, N'C', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (53, 3, N'C', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (54, 3, N'C', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (55, 3, N'C', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (56, 3, N'C', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (57, 3, N'C', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (58, 3, N'C', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (59, 3, N'C', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (60, 3, N'C', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (61, 3, N'C', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (62, 3, N'D', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (63, 3, N'D', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (64, 3, N'D', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (65, 3, N'D', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (66, 3, N'D', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (67, 3, N'D', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (68, 3, N'D', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (69, 3, N'D', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (70, 3, N'D', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (71, 3, N'D', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (72, 3, N'D', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (73, 3, N'D', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (74, 3, N'E', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (75, 3, N'E', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (76, 3, N'E', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (77, 3, N'E', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (78, 3, N'E', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (79, 3, N'E', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (80, 3, N'E', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (81, 3, N'E', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (82, 3, N'E', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (83, 3, N'E', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (84, 3, N'E', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (85, 3, N'E', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (86, 3, N'F', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (87, 3, N'F', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (88, 3, N'F', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (89, 3, N'F', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (90, 3, N'F', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (91, 3, N'F', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (92, 3, N'F', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (93, 3, N'F', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (94, 3, N'F', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (95, 3, N'F', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (96, 3, N'F', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (97, 3, N'F', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (98, 3, N'G', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (99, 3, N'G', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (100, 3, N'G', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (101, 3, N'G', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (102, 3, N'G', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (103, 3, N'G', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (104, 3, N'G', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (105, 3, N'G', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (106, 3, N'G', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (107, 3, N'G', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (108, 3, N'G', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (109, 3, N'G', 12, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (110, 3, N'H', 1, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (111, 3, N'H', 2, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (112, 3, N'H', 3, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (113, 3, N'H', 4, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (114, 3, N'H', 5, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (115, 3, N'H', 6, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (116, 3, N'H', 7, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (117, 3, N'H', 8, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (118, 3, N'H', 9, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (119, 3, N'H', 10, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (120, 3, N'H', 11, 1)
GO
INSERT [dbo].[BUTACA] ([id_butaca], [id_sala], [fila], [numero], [habilitada]) VALUES (121, 3, N'H', 12, 1)
GO
SET IDENTITY_INSERT [dbo].[BUTACA] OFF
GO
SET IDENTITY_INSERT [dbo].[FUNCION] ON 

GO
INSERT [dbo].[FUNCION] ([id_funcion], [id_pelicula], [id_sala], [id_formato], [id_promocion], [fecha], [hora_inicio], [activo]) VALUES (1, 1, 1, 1, 1, CAST(N'2026-11-01' AS Date), CAST(N'18:30:00' AS Time), 1)
GO
INSERT [dbo].[FUNCION] ([id_funcion], [id_pelicula], [id_sala], [id_formato], [id_promocion], [fecha], [hora_inicio], [activo]) VALUES (3, 3, 1, 2, NULL, CAST(N'2026-11-11' AS Date), CAST(N'20:00:00' AS Time), 1)
GO
INSERT [dbo].[FUNCION] ([id_funcion], [id_pelicula], [id_sala], [id_formato], [id_promocion], [fecha], [hora_inicio], [activo]) VALUES (4, 1, 1, 1, 1, CAST(N'2026-11-09' AS Date), CAST(N'20:00:00' AS Time), 1)
GO
INSERT [dbo].[FUNCION] ([id_funcion], [id_pelicula], [id_sala], [id_formato], [id_promocion], [fecha], [hora_inicio], [activo]) VALUES (5, 4, 3, 2, 1, CAST(N'2026-11-05' AS Date), CAST(N'20:00:00' AS Time), 1)
GO
SET IDENTITY_INSERT [dbo].[FUNCION] OFF
GO
SET IDENTITY_INSERT [dbo].[RESERVA] ON 

GO
INSERT [dbo].[RESERVA] ([id_reserva], [id_cliente], [id_funcion], [id_butaca], [id_estado_reserva], [fecha_hora_reserva], [fecha_hora_vencimiento]) VALUES (1, 1, 1, 1, 2, CAST(N'2026-10-27T13:21:00.497' AS DateTime), CAST(N'2026-10-27T15:21:00.497' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[RESERVA] OFF
GO
SET IDENTITY_INSERT [dbo].[ENTRADA] ON 

GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (1, 1, 1, 1, 1, 1, CAST(4000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (4, 3, NULL, 3, 1, 2, CAST(4000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (5, 4, NULL, 3, 2, 2, CAST(4000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (6, 4, NULL, 3, 3, 1, CAST(4000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (7, 5, NULL, 5, 54, 1, CAST(5000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (8, 5, NULL, 5, 55, 1, CAST(5000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (9, 5, NULL, 5, 56, 1, CAST(5000.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[ENTRADA] ([id_entrada], [id_venta], [id_reserva], [id_funcion], [id_butaca], [id_tipo_espectador], [precio_final]) VALUES (10, 5, NULL, 5, 57, 1, CAST(5000.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[ENTRADA] OFF
GO
ALTER TABLE [dbo].[USUARIO] ADD UNIQUE NONCLUSTERED 
(
	[id_persona] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
GO
SET ANSI_PADDING ON

GO
ALTER TABLE [dbo].[USUARIO] ADD UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
GO
ALTER TABLE [dbo].[BENEFICIO] ADD  DEFAULT ((0)) FOR [aplica_a_jubilado]
GO
ALTER TABLE [dbo].[CLIENTE] ADD  DEFAULT ((0)) FOR [es_jubilado]
GO
ALTER TABLE [dbo].[CLIENTE] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[PELICULA] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[CLIENTE_MEMBRESIA] ADD  DEFAULT ((0)) FOR [entradas_gratis_usadas_mes]
GO
ALTER TABLE [dbo].[FUNCION] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[USUARIO]  WITH CHECK ADD FOREIGN KEY([id_estado_usuario])
REFERENCES [dbo].[ESTADO_USUARIO] ([id_estado_usuario])
GO
ALTER TABLE [dbo].[USUARIO]  WITH CHECK ADD FOREIGN KEY([id_persona])
REFERENCES [dbo].[PERSONA] ([id_persona])
GO
ALTER TABLE [dbo].[USUARIO]  WITH CHECK ADD FOREIGN KEY([id_rol])
REFERENCES [dbo].[ROL_USUARIO] ([id_rol])
GO
ALTER TABLE [dbo].[CLIENTE]  WITH CHECK ADD FOREIGN KEY([id_cliente])
REFERENCES [dbo].[PERSONA] ([id_persona])
GO
ALTER TABLE [dbo].[CONTACTO]  WITH CHECK ADD FOREIGN KEY([id_persona])
REFERENCES [dbo].[PERSONA] ([id_persona])
GO
ALTER TABLE [dbo].[CONTACTO]  WITH CHECK ADD FOREIGN KEY([id_tipo_contacto])
REFERENCES [dbo].[TIPO_CONTACTO] ([id_tipo_contacto])
GO
ALTER TABLE [dbo].[EMPLEADO]  WITH CHECK ADD FOREIGN KEY([id_departamento])
REFERENCES [dbo].[DEPARTAMENTO] ([id_departamento])
GO
ALTER TABLE [dbo].[EMPLEADO]  WITH CHECK ADD FOREIGN KEY([id_empleado])
REFERENCES [dbo].[PERSONA] ([id_persona])
GO
ALTER TABLE [dbo].[EMPLEADO]  WITH CHECK ADD FOREIGN KEY([id_puesto])
REFERENCES [dbo].[PUESTO] ([id_puesto])
GO
ALTER TABLE [dbo].[PRODUCTO_CONFITERIA]  WITH CHECK ADD FOREIGN KEY([id_categoria_prod])
REFERENCES [dbo].[CATEGORIA_PRODUCTO] ([id_categoria_prod])
GO
ALTER TABLE [dbo].[PRECIO_TIPO_SALA]  WITH CHECK ADD FOREIGN KEY([id_tipo_sala])
REFERENCES [dbo].[TIPO_SALA] ([id_tipo_sala])
GO
ALTER TABLE [dbo].[PELICULA]  WITH CHECK ADD FOREIGN KEY([id_clasificacion])
REFERENCES [dbo].[CLASIFICACION] ([id_clasificacion])
GO
ALTER TABLE [dbo].[PROVINCIA]  WITH CHECK ADD FOREIGN KEY([id_pais])
REFERENCES [dbo].[PAIS] ([id_pais])
GO
ALTER TABLE [dbo].[MEMBRESIA_BENEFICIO]  WITH CHECK ADD FOREIGN KEY([id_beneficio])
REFERENCES [dbo].[BENEFICIO] ([id_beneficio])
GO
ALTER TABLE [dbo].[MEMBRESIA_BENEFICIO]  WITH CHECK ADD FOREIGN KEY([id_tipo_membresia])
REFERENCES [dbo].[TIPO_MEMBRESIA] ([id_tipo_membresia])
GO
ALTER TABLE [dbo].[PELICULA_IDIOMA]  WITH CHECK ADD FOREIGN KEY([id_idioma])
REFERENCES [dbo].[IDIOMA] ([id_idioma])
GO
ALTER TABLE [dbo].[PELICULA_IDIOMA]  WITH CHECK ADD FOREIGN KEY([id_pelicula])
REFERENCES [dbo].[PELICULA] ([id_pelicula])
GO
ALTER TABLE [dbo].[PELICULA_GENERO]  WITH CHECK ADD FOREIGN KEY([id_genero])
REFERENCES [dbo].[GENERO] ([id_genero])
GO
ALTER TABLE [dbo].[PELICULA_GENERO]  WITH CHECK ADD FOREIGN KEY([id_pelicula])
REFERENCES [dbo].[PELICULA] ([id_pelicula])
GO
ALTER TABLE [dbo].[PELICULA_DIRECTOR]  WITH CHECK ADD FOREIGN KEY([id_director])
REFERENCES [dbo].[DIRECTOR] ([id_director])
GO
ALTER TABLE [dbo].[PELICULA_DIRECTOR]  WITH CHECK ADD FOREIGN KEY([id_pelicula])
REFERENCES [dbo].[PELICULA] ([id_pelicula])
GO
ALTER TABLE [dbo].[PELICULA_ACTOR]  WITH CHECK ADD FOREIGN KEY([id_actor])
REFERENCES [dbo].[ACTOR] ([id_actor])
GO
ALTER TABLE [dbo].[PELICULA_ACTOR]  WITH CHECK ADD FOREIGN KEY([id_pelicula])
REFERENCES [dbo].[PELICULA] ([id_pelicula])
GO
ALTER TABLE [dbo].[LOCALIDAD]  WITH CHECK ADD FOREIGN KEY([id_provincia])
REFERENCES [dbo].[PROVINCIA] ([id_provincia])
GO
ALTER TABLE [dbo].[VENTA]  WITH CHECK ADD FOREIGN KEY([id_canal])
REFERENCES [dbo].[CANAL_VENTA] ([id_canal])
GO
ALTER TABLE [dbo].[VENTA]  WITH CHECK ADD FOREIGN KEY([id_cliente])
REFERENCES [dbo].[CLIENTE] ([id_cliente])
GO
ALTER TABLE [dbo].[VENTA]  WITH CHECK ADD FOREIGN KEY([id_empleado])
REFERENCES [dbo].[EMPLEADO] ([id_empleado])
GO
ALTER TABLE [dbo].[TOKEN_VERIFICACION]  WITH CHECK ADD FOREIGN KEY([id_usuario])
REFERENCES [dbo].[USUARIO] ([id_usuario])
GO
ALTER TABLE [dbo].[EMPLEADO_TURNO]  WITH CHECK ADD FOREIGN KEY([id_empleado])
REFERENCES [dbo].[EMPLEADO] ([id_empleado])
GO
ALTER TABLE [dbo].[EMPLEADO_TURNO]  WITH CHECK ADD FOREIGN KEY([id_turno])
REFERENCES [dbo].[TURNO] ([id_turno])
GO
ALTER TABLE [dbo].[CLIENTE_MEMBRESIA]  WITH CHECK ADD FOREIGN KEY([id_cliente])
REFERENCES [dbo].[CLIENTE] ([id_cliente])
GO
ALTER TABLE [dbo].[CLIENTE_MEMBRESIA]  WITH CHECK ADD FOREIGN KEY([id_tipo_membresia])
REFERENCES [dbo].[TIPO_MEMBRESIA] ([id_tipo_membresia])
GO
ALTER TABLE [dbo].[VentaxFormaPago]  WITH CHECK ADD FOREIGN KEY([id_forma_pago])
REFERENCES [dbo].[FORMA_PAGO] ([id_forma_pago])
GO
ALTER TABLE [dbo].[VentaxFormaPago]  WITH CHECK ADD FOREIGN KEY([id_venta])
REFERENCES [dbo].[VENTA] ([id_venta])
GO
ALTER TABLE [dbo].[SUCURSAL]  WITH CHECK ADD FOREIGN KEY([id_localidad])
REFERENCES [dbo].[LOCALIDAD] ([id_localidad])
GO
ALTER TABLE [dbo].[DETALLE_VENTA_CONFITERIA]  WITH CHECK ADD FOREIGN KEY([id_producto])
REFERENCES [dbo].[PRODUCTO_CONFITERIA] ([id_producto])
GO
ALTER TABLE [dbo].[DETALLE_VENTA_CONFITERIA]  WITH CHECK ADD FOREIGN KEY([id_venta])
REFERENCES [dbo].[VENTA] ([id_venta])
GO
ALTER TABLE [dbo].[COMPLEJO]  WITH CHECK ADD FOREIGN KEY([id_sucursal])
REFERENCES [dbo].[SUCURSAL] ([id_sucursal])
GO
ALTER TABLE [dbo].[SALA]  WITH CHECK ADD FOREIGN KEY([id_complejo])
REFERENCES [dbo].[COMPLEJO] ([id_complejo])
GO
ALTER TABLE [dbo].[SALA]  WITH CHECK ADD FOREIGN KEY([id_tipo_sala])
REFERENCES [dbo].[TIPO_SALA] ([id_tipo_sala])
GO
ALTER TABLE [dbo].[BUTACA]  WITH CHECK ADD FOREIGN KEY([id_sala])
REFERENCES [dbo].[SALA] ([id_sala])
GO
ALTER TABLE [dbo].[FUNCION]  WITH CHECK ADD FOREIGN KEY([id_formato])
REFERENCES [dbo].[FORMATO] ([id_formato])
GO
ALTER TABLE [dbo].[FUNCION]  WITH CHECK ADD FOREIGN KEY([id_pelicula])
REFERENCES [dbo].[PELICULA] ([id_pelicula])
GO
ALTER TABLE [dbo].[FUNCION]  WITH CHECK ADD FOREIGN KEY([id_promocion])
REFERENCES [dbo].[PROMOCION] ([id_promocion])
GO
ALTER TABLE [dbo].[FUNCION]  WITH CHECK ADD FOREIGN KEY([id_sala])
REFERENCES [dbo].[SALA] ([id_sala])
GO
ALTER TABLE [dbo].[RESERVA]  WITH CHECK ADD FOREIGN KEY([id_butaca])
REFERENCES [dbo].[BUTACA] ([id_butaca])
GO
ALTER TABLE [dbo].[RESERVA]  WITH CHECK ADD FOREIGN KEY([id_cliente])
REFERENCES [dbo].[CLIENTE] ([id_cliente])
GO
ALTER TABLE [dbo].[RESERVA]  WITH CHECK ADD FOREIGN KEY([id_estado_reserva])
REFERENCES [dbo].[ESTADO_RESERVA] ([id_estado_reserva])
GO
ALTER TABLE [dbo].[RESERVA]  WITH CHECK ADD FOREIGN KEY([id_funcion])
REFERENCES [dbo].[FUNCION] ([id_funcion])
GO
ALTER TABLE [dbo].[ENTRADA]  WITH CHECK ADD FOREIGN KEY([id_butaca])
REFERENCES [dbo].[BUTACA] ([id_butaca])
GO
ALTER TABLE [dbo].[ENTRADA]  WITH CHECK ADD FOREIGN KEY([id_funcion])
REFERENCES [dbo].[FUNCION] ([id_funcion])
GO
ALTER TABLE [dbo].[ENTRADA]  WITH CHECK ADD FOREIGN KEY([id_reserva])
REFERENCES [dbo].[RESERVA] ([id_reserva])
GO
ALTER TABLE [dbo].[ENTRADA]  WITH CHECK ADD FOREIGN KEY([id_tipo_espectador])
REFERENCES [dbo].[TIPO_ESPECTADOR] ([id_tipo_espectador])
GO
ALTER TABLE [dbo].[ENTRADA]  WITH CHECK ADD FOREIGN KEY([id_venta])
REFERENCES [dbo].[VENTA] ([id_venta])
GO
