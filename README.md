# 🎬 Cines Hollywood — TPI

Sistema web para un cine: cartelera pública, compra de entradas online eligiendo butaca, y un panel de administración con ABM de películas, funciones, salas, precios y clientes, ventas en boletería y reportes.

- **Backend:** ASP.NET Core 8 Web API + Entity Framework Core (SQL Server), arquitectura en capas Controller → Service → Repository
- **Frontend:** HTML + CSS + JavaScript (sin frameworks de JS) con **Bootstrap 5.3** para el diseño responsivo, servido por un proyecto ASP.NET aparte
- **Seguridad:** login con **JWT** y roles (`Administrador` / `Cliente`)
- **Librerías del front (por CDN):** [Bootstrap 5.3](https://getbootstrap.com/) (diseño responsivo, componentes), [Chart.js](https://www.chartjs.org/) (gráficos), [SweetAlert2](https://sweetalert2.github.io/) (alertas), [Feather Icons](https://feathericons.com/) (íconos del panel)
- **Testing de la API:** colección de **Postman** en `postman/` (60 requests con tests)

---

## Índice

1. [Cómo levantar el proyecto](#1-cómo-levantar-el-proyecto)
2. [Arquitectura general](#2-arquitectura-general)
3. [Estructura de carpetas](#3-estructura-de-carpetas)
4. [Base de datos y modelo](#4-base-de-datos-y-modelo)
5. [Proyecto Data (repositorios y DTOs)](#5-proyecto-data-repositorios-y-dtos)
6. [Proyecto API (controladores, servicios y middleware)](#6-proyecto-api-controladores-servicios-y-middleware)
7. [Autenticación con JWT](#7-autenticación-con-jwt)
8. [Lista completa de endpoints](#8-lista-completa-de-endpoints)
9. [Reglas de negocio importantes](#9-reglas-de-negocio-importantes)
10. [Frontend (proyecto Web)](#10-frontend-proyecto-web)
11. [Flujos completos paso a paso](#11-flujos-completos-paso-a-paso)
12. [Probar la API con Postman](#12-probar-la-api-con-postman)
13. [Temas de la materia: dónde está cada uno](#13-temas-de-la-materia-dónde-está-cada-uno)
14. [Problemas comunes](#14-problemas-comunes)
15. [Cómo agregar algo nuevo](#15-cómo-agregar-algo-nuevo)

---

## 1. Cómo levantar el proyecto

### Requisitos
- Visual Studio 2022 (o superior) con el SDK de **.NET 8**
- **SQL Server Express** con la base `SistemaCineBdFinal` (instancia `localhost\SQLEXPRESS`)
- Conexión a internet (las fuentes y las librerías JS se cargan por CDN)

### Crear la base de datos
Ejecutar **`SistemaCineBdFinal_actual.sql`** (en la raíz del repo) en SQL Server Management Studio. Crea la base `SistemaCineBdFinal` con todas las tablas, claves, columnas autoincrementales y los datos de prueba (películas, sala, butacas, precios, roles, etc.).

> ⚠️ El otro script, `SistemaCineBdFinal.sql`, es la **versión vieja**: no tiene columnas IDENTITY (autoincrementales), ni las columnas `activo` y `PosterUrl`, ni datos. Con ese script el sistema **no funciona**. Usar siempre `SistemaCineBdFinal_actual.sql`.
>
> Si ya tenés una base creada con el script viejo, borrala y volvé a crearla con el nuevo.

### Pasos
1. Abrir `CineHollywoodTPI.slnx` en Visual Studio.
2. Revisar la cadena de conexión en `CineHollywoodTPI/appsettings.json` → `ConnectionStrings:DefaultConnection`. Si tu instancia de SQL Server tiene otro nombre, cambiala ahí.
3. Configurar **inicio múltiple**: clic derecho en la solución → *Configurar proyectos de inicio* → *Varios proyectos de inicio* → poner en **Iniciar** a:
   - `CineHollywoodTPI.API` (la API, puerto **5191**)
   - `CineHollywoodTPI.Web` (el sitio, puerto **5020**)
4. Ejecutar (F5). Se abre Swagger de la API y el sitio web.

### Usuario administrador
La primera vez que arranca, la API crea un administrador **si no existe ninguno** (ver `Program.cs`, sección 6):

| Email | Contraseña |
|---|---|
| `admin@cinehollywood.com` | `Admin123!` |

Se puede cambiar en `appsettings.json` → `AdminInicial`. Los clientes se registran desde el sitio con el botón **Registrarse**.

### Direcciones
| Qué | URL |
|---|---|
| Sitio público | http://localhost:5020 |
| Panel de administración | http://localhost:5020/admin.html (requiere login de admin) |
| Swagger (probar la API) | http://localhost:5191/swagger |
| Pósters guardados | http://localhost:5191/posters/<archivo> |

> El frontend llama siempre a `http://localhost:5191/api`. Si cambiás el puerto de la API, cambialo también en `CineHollywoodTPI.Web/wwwroot/js/api.js` (constante `API_URL`).

---

## 2. Arquitectura general

```
┌───────────────────────────┐   fetch + JWT   ┌─────────────────────────────────────────────┐
│  CineHollywoodTPI.Web     │ ──────────────▶ │  CineHollywoodTPI.API  (puerto 5191)        │
│  (puerto 5020)            │ ◀────────────── │                                             │
│  HTML / CSS / JS          │      JSON       │  Controllers   → solo HTTP                  │
└───────────────────────────┘                 │      │                                      │
                                              │      ▼                                      │
                                              │  Services      → REGLAS DE NEGOCIO          │
                                              │      │           (validaciones, descuentos) │
                                              └──────┼──────────────────────────────────────┘
                                                     ▼
                                              ┌─────────────────────────────────────────────┐
                                              │  CineHollywoodTPI.Data                      │
                                              │  Repositories  → solo acceso a datos (EF)   │
                                              │  Models + DTOs + Interfaces                 │
                                              └──────┬──────────────────────────────────────┘
                                                     ▼  EF Core
                                              ┌─────────────────────────────────────────────┐
                                              │  SQL Server — SistemaCineBdFinal            │
                                              └─────────────────────────────────────────────┘
```

### Arquitectura en capas: cada una tiene **una sola responsabilidad**

| Capa | Dónde | Qué hace | Qué **no** hace |
|---|---|---|---|
| **Controller** | `CineHollywoodTPI/Controllers` | Recibe el pedido HTTP, revisa permisos (`[Authorize]`), llama al servicio y devuelve `Ok(...)` | No tiene `if` de negocio ni `try/catch` |
| **Service** | `CineHollywoodTPI/Services` | **Todas las reglas de negocio**: validaciones, descuentos, permisos de negocio, cálculo de precios, armado de DTOs | No escribe SQL ni usa el `DbContext` |
| **Repository** | `CineHollywoodTPI.Data/Repositories` | Consultas y guardado con Entity Framework (incluidas las transacciones) | No decide reglas de negocio |
| **Middleware** | `CineHollywoodTPI/Middleware` | Convierte las excepciones de los servicios en respuestas HTTP (400, 404, 409…) | — |

Ejemplo, **vender entradas**:

```
VentaController.RegistrarVenta(dto)
   └─▶ VentaService.RegistrarAsync(dto, usuario)
         ├─ ¿la función existe y no empezó?           (FuncionService.YaEmpezo)
         ├─ ¿la butaca es de la sala y está libre?     (FuncionRepository.GetButacasAsync)
         ├─ ¿película +18 y entrada de Menor?          → NegocioException("...") → 400
         ├─ precio de cada entrada (promo o -15%)      (PrecioService.PrecioEntrada)
         └─▶ VentaRepository.RegistrarVentaAsync(...)  → transacción VENTA + ENTRADAs + pago
```

**¿Por qué así?**
- Si mañana cambia una regla (por ejemplo, el descuento pasa a 20%), se toca **un solo lugar**: el servicio (o `appsettings.json`).
- Los servicios se reutilizan entre sí: `VentaService` usa las reglas de `FuncionService` y el cálculo de `PrecioService`, en lugar de repetirlos.
- Los controllers quedan de 1 o 2 líneas por endpoint y son fáciles de leer.

**¿Por qué DTOs?** Los modelos de EF tienen navegaciones circulares (Venta → Entrada → Venta…) y campos que el navegador no necesita. Los DTOs devuelven solo lo justo y evitan errores de serialización.

**¿Por qué interfaces (`IVentaService`, `IVentaRepository`…)?** Porque se registran con **inyección de dependencias** en `Program.cs` (`AddScoped<IVentaService, VentaService>()`). Cada capa recibe la interfaz de la capa de abajo en el constructor y no le importa cómo está implementada.

---

## 3. Estructura de carpetas

```
CineHollywoodTPI.slnx                  ← la solución (3 proyectos)
│
├── CineHollywoodTPI/                  ← PROYECTO API
│   ├── Program.cs                     ← configuración: BD, DI, JWT, CORS, Swagger, middleware, admin inicial
│   ├── appsettings.json               ← conexión a la BD, clave JWT, REGLAS DE NEGOCIO, admin inicial
│   ├── Controllers/                   ← CAPA HTTP (finitos, sin lógica)
│   │   ├── AuthController.cs          ← login / registro (JWT)
│   │   ├── PeliculaController.cs      ← ABM películas + pósters
│   │   ├── FuncionController.cs       ← ABM funciones + mapa de butacas
│   │   ├── SalaController.cs          ← ABM salas
│   │   ├── PrecioController.cs        ← precios por tipo de sala
│   │   ├── ClienteController.cs       ← ABM clientes
│   │   ├── VentaController.cs         ← ventas
│   │   ├── ReporteController.cs       ← reportes del dashboard
│   │   └── CatalogoController.cs      ← listas para los <select>
│   ├── Services/                      ← CAPA DE NEGOCIO
│   │   ├── Interfaces/                ← IPeliculaService, IVentaService, ...
│   │   ├── PeliculaService.cs         ← reglas de películas y pósters
│   │   ├── FuncionService.cs          ← reglas de funciones (fecha, superposición, funciones con ventas)
│   │   ├── SalaService.cs             ← reglas de salas y generación de butacas
│   │   ├── PrecioService.cs           ← ABM de precios + CÁLCULO del precio de cada entrada (promo / -15%)
│   │   ├── ClienteService.cs          ← reglas de clientes (DNI, fechas)
│   │   ├── VentaService.cs            ← reglas de la venta (butacas, +18, función empezada, pagos)
│   │   ├── AuthService.cs             ← login, registro, admin inicial
│   │   ├── ReporteService.cs          ← validación de filtros de reportes
│   │   ├── CatalogoService.cs         ← catálogos (sin reglas, mantiene la arquitectura pareja)
│   │   ├── Excepciones.cs             ← NegocioException, NoEncontradoException, ConflictoException...
│   │   ├── ReglasNegocioOptions.cs    ← parámetros de las reglas (se leen de appsettings.json)
│   │   ├── UsuarioActual.cs           ← quién hace el pedido (sale del token)
│   │   ├── TokenService.cs            ← (infraestructura) arma y firma el JWT
│   │   └── PosterService.cs           ← (infraestructura) guarda/descarga imágenes
│   ├── Middleware/
│   │   └── ManejoErroresMiddleware.cs ← excepción de negocio → respuesta HTTP 400/401/403/404/409
│   └── wwwroot/posters/               ← acá se guardan los pósters subidos
│
├── CineHollywoodTPI.Data/             ← CAPA DE DATOS (biblioteca de clases)
│   ├── Models/                        ← entidades generadas por scaffold de EF + SistemaCineContext
│   │   ├── SistemaCineContext.Partial.cs ← mapeo extra (PosterUrl) sin tocar el scaffold
│   │   └── Pelicula.Partial.cs           ← propiedad PosterUrl
│   ├── DTOs/                          ← objetos que viajan como JSON
│   ├── Interfaces/                    ← contratos de los repositorios
│   └── Repositories/                  ← acceso a datos con EF Core (sin reglas de negocio)
│
└── CineHollywoodTPI.Web/              ← PROYECTO WEB (solo sirve archivos estáticos)
    ├── Program.cs                     ← UseDefaultFiles + UseStaticFiles
    └── wwwroot/
        ├── index.html                 ← inicio (carrusel de cartelera)
        ├── cartelera.html             ← películas con funciones y horarios
        ├── estrenos.html              ← películas sin funciones todavía
        ├── promociones.html           ← promociones y funciones adheridas
        ├── reserva.html               ← compra de entradas (3 pasos)
        ├── admin.html                 ← panel de administración
        ├── css/style.css              ← estilos generales (tema claro/oscuro)
        ├── css/admin.css              ← estilos del panel
        └── js/
            ├── api.js                 ← fetch a la API, sesión JWT, utilidades, alertas
            ├── main.js                ← navbar, login/registro, "Mis compras"
            ├── butacas.js             ← mapa de butacas reutilizable
            ├── index.js               ← carrusel del inicio
            ├── cartelera.js           ← página de cartelera
            ├── reserva.js             ← flujo de compra
            └── admin.js               ← todo el panel de administración
```

**Otros archivos en la raíz del repo:**

| Archivo | Qué es |
|---|---|
| `SistemaCineBdFinal_actual.sql` | ✅ Script de la base **actual** (estructura + datos). Es el que hay que usar |
| `SistemaCineBdFinal.sql` | Script viejo, desactualizado (ver sección 1) |
| `postman/CineHollywood.postman_collection.json` | Colección de Postman para probar la API (ver [sección 12](#12-probar-la-api-con-postman)) |

---

## 4. Base de datos y modelo

Los modelos de `CineHollywoodTPI.Data/Models` se generaron con **scaffold** de EF Core a partir de la base existente. Las tablas que usa el sistema:

| Tabla | Para qué se usa |
|---|---|
| `PELICULA` | Películas (`activo` = baja lógica, `PosterUrl` = imagen) |
| `CLASIFICACION` | ATP, +13, etc. |
| `FUNCION` | Una proyección: película + sala + formato + fecha + hora (+ promoción opcional) |
| `SALA` / `BUTACA` | Salas y sus butacas (fila + número) |
| `TIPO_SALA` | 2D Estándar, 3D XD… **define el precio** |
| `PRECIO_TIPO_SALA` | Precio de la entrada por tipo de sala, con vigencia `fecha_desde` / `fecha_hasta` |
| `PROMOCION` | Descuento en % que se aplica a las funciones adheridas |
| `FORMATO` | Subtitulado / Doblada |
| `PERSONA` | Datos personales (nombre, apellido, DNI, nacimiento) |
| `CLIENTE` | Extiende a PERSONA (mismo id). `activo` = baja lógica |
| `USUARIO` | Cuenta de login: email + hash de contraseña + rol. Apunta a una PERSONA |
| `ROL_USUARIO` | `Administrador` / `Cliente` |
| `VENTA` | **Maestro** de la venta: cliente, canal, fecha |
| `ENTRADA` | **Detalle** de la venta: función, butaca, tipo de espectador, precio final |
| `VentaxFormaPago` | Cómo se pagó la venta (puede haber más de una forma) |
| `TIPO_ESPECTADOR` | General / **Jubilado** / **Menor** (Jubilado y Menor tienen descuento; Menor no puede ver +18) |
| `FORMA_PAGO`, `CANAL_VENTA`, `COMPLEJO` | Tablas de referencia |

Las tablas de confitería, membresías, empleados, reservas, etc. existen en la base pero **no se usan** en esta versión.

**Relaciones clave:**

```
PERSONA 1──1 CLIENTE 1──* VENTA 1──* ENTRADA *──1 FUNCION *──1 PELICULA
   │                        │           │            │
   1                        *           *            *──1 SALA *──1 TIPO_SALA 1──* PRECIO_TIPO_SALA
   │                  VentaxFormaPago   BUTACA *──1 SALA
USUARIO *──1 ROL_USUARIO
```

---

## 5. Proyecto Data (repositorios y DTOs)

### Repositorios (solo acceso a datos)
Los repositorios **no tienen reglas de negocio**: solo consultan y guardan. Las decisiones las toman los servicios.

| Repositorio | Qué hace |
|---|---|
| `PeliculaRepository` | Activas, búsqueda por nombre, cartelera (con funciones desde una fecha), estrenos (sin funciones), alta, modificación, baja lógica |
| `FuncionRepository` | Funciones activas (con película, clasificación, sala, precios y promo), **mapa de butacas** (libre/ocupada), funciones de una sala en una fecha, si tiene entradas vendidas, ABM |
| `SalaRepository` | Salas con sus datos, existencia de tipo/complejo/nombre, si tiene butacas usadas o funciones, alta (con butacas), modificación, eliminación |
| `PrecioRepository` | Precios por tipo de sala; alta y baja en transacción |
| `ClienteRepository` | ABM de clientes (PERSONA + CLIENTE), si existe un DNI |
| `VentaRepository` | Listado, detalle y totales de ventas, y la **transacción de venta** (ver [sección 9](#9-reglas-de-negocio-importantes)) |
| `ReporteRepository` | Películas más vendidas y ventas mes a mes |
| `UsuarioRepository` | Usuario por email, persona por DNI, roles, alta de usuario en transacción |
| `CatalogoRepository` | Listas cortas para combos: clasificaciones, formatos, salas, tipos de sala, tipos de espectador, formas de pago, canales, promociones |

### Cálculo del precio (`PrecioService`)

```
precio base = precio vigente del TIPO DE SALA para la FECHA de la función (PRECIO_TIPO_SALA)

si la función tiene PROMOCIÓN        → precio = base × (1 − % promo)      (para todos)
si no, y la entrada es Jubilado/Menor → precio = base × (1 − 15%)
si no                                 → precio = base
```

- La promoción y el 15% **no se acumulan**: si la función tiene promo, se aplica solo la promo.
- Si no hay precio para la fecha se usa el más reciente cargado; si el tipo de sala no tiene precios, es $0 (el panel avisa "Sin precio").
- **El precio siempre lo calcula el servidor**. Cada función viene con su lista `precios` (uno por tipo de espectador) para que el front los muestre.

### DTOs principales

| Archivo | Contenido |
|---|---|
| `PeliculaDTO`, `PeliculaInsertDTO`, `PeliculaUpdateDTO` (+ `PosterUrlDTO`) | Datos de película |
| `FuncionDTO`, `FuncionInsertDTO`, `FuncionUpdateDTO` | La DTO de lectura trae además título, clasificación, sala, formato, promoción, si tiene ventas y los **precios por tipo de espectador** (`PrecioPorTipoDTO`) |
| `ClienteDTO`, `ClienteInsertDTO`, `ClienteUpdateDTO` | Datos de cliente + persona |
| `VentaCompletaDTO` | Lo que manda el front para vender: cliente, entradas (función + butaca + tipo espectador), formas de pago |
| `VentaConsultaDTOs` | `VentaListadoDTO` (fila de la tabla) y `VentaDetalleDTO` (con la lista de entradas) |
| `ReporteDTOs` | `PeliculaMasVendidaDTO`, `VentaMensualDTO` |
| `SalaDTOs` | `SalaDetalleDTO`, `SalaGuardarDTO` |
| `PrecioDTOs` | `PrecioDTO`, `PrecioInsertDTO` |
| `CatalogoDTOs` | `ItemDTO` (id + descripción), `SalaDTO`, `PromocionDTO`, `ButacaEstadoDTO` |
| `AuthDTOs` | `LoginDTO`, `RegistroDTO`, `LoginRespuestaDTO` |

### Archivos `.Partial.cs`
La columna `PosterUrl` existía en la tabla `PELICULA` pero el scaffold no la mapeó. En lugar de editar los archivos generados, se agregó en `Pelicula.Partial.cs` (propiedad) y `SistemaCineContext.Partial.cs` (configuración). Así, si se vuelve a hacer scaffold, no se pierde.

---

## 6. Proyecto API (controladores, servicios y middleware)

### `Program.cs` (en orden)
1. **CORS** `PermitirTodo`: deja que el sitio (puerto 5020) llame a la API (puerto 5191).
2. **DbContext** con la cadena `DefaultConnection`.
3. **Inyección de dependencias**, en tres grupos: **repositorios** (datos), **servicios** (negocio) e infraestructura (`TokenService`, `PosterService`, `PasswordHasher`, `HttpClient`). También se cargan las `ReglasNegocioOptions` desde `appsettings.json`.
4. **Autenticación JWT**: valida emisor, audiencia, vencimiento y firma con la clave de `appsettings.json`.
5. **Controllers + Swagger** con botón *Authorize* para pegar el token.
6. **Admin inicial** (`AuthService.CrearAdminInicialAsync`): si no existe ningún usuario Administrador, lo crea.
7. Pipeline: `UseStaticFiles` (pósters) → `UseCors` → **`ManejoErroresMiddleware`** → `UseAuthentication` → `UseAuthorization` → `MapControllers`.

> En desarrollo **no** se usa `UseHttpsRedirection` porque el front llama por HTTP y la redirección rompe CORS.

### Cómo es un controller
Solo HTTP, una o dos líneas por endpoint:

```csharp
[Authorize(Roles = "Administrador")]
[HttpPut("{id:int}")]
public async Task<IActionResult> Modificar(int id, [FromBody] FuncionUpdateDTO dto)
{
    await _funcionService.ModificarAsync(id, dto);          // las reglas están en el servicio
    return Ok(new { mensaje = "Función modificada con éxito!" });
}
```

### Cómo es un servicio
Valida y, si algo no se cumple, **lanza una excepción de negocio** con un mensaje claro:

```csharp
public async Task ModificarAsync(int id, FuncionUpdateDTO dto)
{
    var funcion = await _funcionRepository.GetByIdAsync(id)
        ?? throw new NoEncontradoException($"No existe la función con ID {id}.");   // → 404

    if (await _funcionRepository.TieneEntradasVendidasAsync(id) && cambiaFechaOHora)
        throw new NegocioException("La función ya tiene entradas vendidas...");      // → 400
    ...
    await _funcionRepository.UpdateAsync(funcion);
}
```

### Excepciones y middleware (`Services/Excepciones.cs` + `Middleware/ManejoErroresMiddleware.cs`)
El middleware atrapa las excepciones y responde con el código HTTP y el mensaje, que el front muestra tal cual. Por eso **ningún controller tiene `try/catch`**.

| Excepción | HTTP | Cuándo |
|---|---|---|
| `NegocioException` | 400 | No se cumple una regla (datos inválidos, función con ventas, +18…) |
| `NoAutorizadoException` | 401 | Email o contraseña incorrectos |
| `AccesoDenegadoException` | 403 | Un cliente quiere ver una compra que no es suya |
| `NoEncontradoException` | 404 | No existe lo pedido |
| `ConflictoException` | 409 | Choca con algo existente (DNI/email repetido, butaca ocupada, horario superpuesto) |
| cualquier otra | 500 | Error inesperado (se registra en el log) |

### Parámetros de las reglas (`appsettings.json` → `ReglasNegocio`)
```json
"ReglasNegocio": {
  "PorcentajeDescuentoJubiladoMenor": 15,
  "IdsTipoEspectadorConDescuento": [ 2, 3 ],   // Jubilado, Menor
  "IdTipoEspectadorMenor": 3,
  "TextoClasificacionMayores": "18",           // clasificación que se considera +18
  "EdadMayoria": 18,
  "MaxEntradasPorCompra": 10,
  "IdCanalBoleteria": 1,
  "IdCanalWeb": 2
}
```
Para cambiar el descuento o el tope de entradas no hace falta tocar código.

### Servicios de negocio
| Servicio | Reglas principales |
|---|---|
| `PeliculaService` | Título obligatorio (máx. 200), duración 1–600, clasificación existente; pósters |
| `FuncionService` | Campos obligatorios, película activa, **no en el pasado**, **no superponer** en la sala, **con ventas no se mueve ni se cancela**; arma el DTO con los precios por tipo |
| `SalaService` | Nombre único por complejo, 1–26 filas y 1–40 butacas por fila, genera las butacas; no cambia butacas si hay ventas; no elimina si tiene funciones |
| `PrecioService` | Precio > 0, fecha ≥ hoy, cierra el precio anterior; **calcula el precio de cada entrada** |
| `ClienteService` | Nombre/apellido obligatorios, DNI numérico y no repetido, nacimiento no futuro |
| `VentaService` | Cliente/canal según rol, 1–10 entradas, función activa y **no empezada**, butaca de la sala y libre, **+18**, precios, formas de pago, ver solo compras propias |
| `AuthService` | Login con hash, usuario activo, registro con validaciones y reutilización de persona por DNI |
| `ReporteService` | Top 1–50, "desde" ≤ "hasta", año válido |
| `CatalogoService` | Sin reglas (solo lectura) |

### Servicios de infraestructura
- **`TokenService`**: arma el JWT con los *claims* del usuario (id, nombre, email, rol, idCliente) y lo firma con HMAC-SHA256. Devuelve `LoginRespuestaDTO` con el token y los datos que el front guarda.
- **`PosterService`**: guarda los pósters en `CineHollywoodTPI/wwwroot/posters/`.
  - **Subir archivo**: valida tamaño (5 MB) y que sea una imagen real mirando los primeros bytes (JPG, PNG, GIF, WEBP), no solo la extensión.
  - **Desde un link**: la API descarga la imagen. Si el link es una **página web** (no una imagen), busca la etiqueta `<meta property="og:image">` de esa página y descarga esa imagen.
  - En la base se guarda la ruta relativa (`/posters/abc.jpg`). Cuando se reemplaza un póster se borra el archivo anterior.

---

## 7. Autenticación con JWT

```
1. Usuario completa el login  ──POST /api/auth/login { email, password }──▶ API
2. API busca el USUARIO por email y verifica la contraseña con PasswordHasher (PBKDF2)
3. API devuelve { token, expira, nombre, rol, idCliente }
4. El front guarda eso en localStorage (clave "cine_sesion")
5. En cada fetch, api.js agrega el header:  Authorization: Bearer <token>
6. La API valida el token; [Authorize(Roles = "Administrador")] chequea el rol
```

- **Contraseñas**: nunca se guardan en texto plano; se usa `PasswordHasher<Usuario>` de ASP.NET Identity.
- **Registro** (`POST /api/auth/registro`): crea PERSONA + CLIENTE + USUARIO (rol Cliente) en una transacción. Si ya existe una persona con ese DNI (por ejemplo, cargada antes en boletería), la reutiliza.
- **Vencimiento**: 120 minutos (`Jwt:ExpiraMinutos`). Si vence, la API responde 401 y el front cierra la sesión.
- **Roles**:
  - `Administrador`: todo el panel.
  - `Cliente`: comprar entradas por la web y ver **solo sus** compras.
- **Protección del panel**: `admin.html` revisa la sesión antes de dibujarse y redirige al inicio si no sos admin. Igual, **la seguridad real está en la API**: aunque alguien fuerce la página, sin token de admin los endpoints responden 401/403.

---

## 8. Lista completa de endpoints

🔓 = público · 👤 = cualquier usuario logueado · 🛡️ = solo Administrador

### Códigos de respuesta que usa la API
| Código | Cuándo |
|---|---|
| **200 OK** | Consultas (GET) y modificaciones/bajas exitosas (PUT, DELETE) — devuelven `{ mensaje }` |
| **201 Created** | Altas exitosas (POST): película, función, cliente, venta, sala, precio, registro. Devuelven `{ mensaje, id }` y, cuando existe un GET por id, el header `Location` con la URL del recurso nuevo (`CreatedAtAction`) |
| **400 Bad Request** | Datos inválidos o regla de negocio no cumplida (`NegocioException`) |
| **401 Unauthorized** | Sin token, token vencido o login incorrecto |
| **403 Forbidden** | Logueado pero sin permiso (ej. un Cliente quiere usar un endpoint de Administrador) |
| **404 Not Found** | No existe el recurso pedido |
| **409 Conflict** | Choca con algo existente: email/DNI repetido, butaca ya vendida, horario superpuesto |
| **500 Internal Server Error** | Error inesperado (se registra en el log) |

### Auth
| Método | Ruta | | Descripción |
|---|---|---|---|
| POST | `/api/auth/login` | 🔓 | Devuelve el JWT |
| POST | `/api/auth/registro` | 🔓 | Crea una cuenta de cliente y devuelve el JWT |
| GET | `/api/auth/yo` | 👤 | Datos del usuario del token |

### Películas
| Método | Ruta | | Descripción |
|---|---|---|---|
| GET | `/api/pelicula` | 🔓 | Películas activas |
| GET | `/api/pelicula/{id}` | 🔓 | Una película |
| GET | `/api/pelicula/buscar?nombre=` | 🔓 | Búsqueda por título |
| GET | `/api/pelicula/cartelera` | 🔓 | Activas con funciones desde hoy |
| GET | `/api/pelicula/estrenos` | 🔓 | Activas sin funciones programadas |
| POST | `/api/pelicula` | 🛡️ | Alta |
| PUT | `/api/pelicula/{id}` | 🛡️ | Modificación |
| DELETE | `/api/pelicula/{id}` | 🛡️ | Baja lógica |
| POST | `/api/pelicula/{id}/poster` | 🛡️ | Subir imagen (multipart, campo `archivo`) |
| POST | `/api/pelicula/{id}/poster-url` | 🛡️ | Descargar imagen desde un link `{ url }` |

### Funciones
| Método | Ruta | | Descripción |
|---|---|---|---|
| GET | `/api/funcion?idPelicula=&soloProximas=true` | 🔓 | Funciones activas (con precio calculado) |
| GET | `/api/funcion/{id}` | 🔓 | Una función |
| GET | `/api/funcion/{id}/butacas` | 🔓 | Butacas de la sala con `ocupada` true/false |
| POST | `/api/funcion` | 🛡️ | Alta (valida fecha no pasada y superposición) |
| PUT | `/api/funcion/{id}` | 🛡️ | Modificación |
| DELETE | `/api/funcion/{id}` | 🛡️ | Baja lógica (cancelar función) |

### Salas
| Método | Ruta | | Descripción |
|---|---|---|---|
| GET | `/api/sala` | 🛡️ | Salas con tipo, complejo, butacas y funciones |
| GET | `/api/sala/complejos` | 🛡️ | Complejos para el combo |
| POST | `/api/sala` | 🛡️ | Alta + generación de butacas `{ nombre, idTipoSala, idComplejo, filas, butacasPorFila }` |
| PUT | `/api/sala/{id}` | 🛡️ | Modificación (regenera butacas solo si no hay ventas) |
| DELETE | `/api/sala/{id}` | 🛡️ | Eliminación (solo si no tiene funciones) |

### Precios
| Método | Ruta | | Descripción |
|---|---|---|---|
| GET | `/api/precio` | 🛡️ | Todos los precios con estado |
| POST | `/api/precio` | 🛡️ | Nuevo precio `{ idTipoSala, precio, fechaDesde }` |
| DELETE | `/api/precio/{id}` | 🛡️ | Elimina un precio que todavía no empezó |

### Clientes (todo 🛡️)
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/cliente` | Clientes activos |
| GET | `/api/cliente/{id}` | Un cliente |
| POST | `/api/cliente` | Alta (PERSONA + CLIENTE) |
| PUT | `/api/cliente/{id}` | Modificación |
| DELETE | `/api/cliente/{id}` | Baja lógica |

### Ventas
| Método | Ruta | | Descripción |
|---|---|---|---|
| GET | `/api/venta` | 👤 | Admin: todas. Cliente: solo las suyas |
| GET | `/api/venta/{id}` | 👤 | Detalle con entradas (un cliente solo ve las suyas) |
| GET | `/api/venta/totales` | 🛡️ | Totales para el dashboard |
| POST | `/api/venta` | 👤 | **Registrar venta** (transacción maestro-detalle) |

Ejemplo de body para `POST /api/venta`:
```json
{
  "idCliente": 1,
  "idCanal": 1,
  "entradas": [
    { "idFuncion": 3, "idButaca": 10, "idTipoEspectador": 1 },
    { "idFuncion": 3, "idButaca": 11, "idTipoEspectador": 2 }
  ],
  "formasPago": [ { "idFormaPago": 1 } ]
}
```
Si quien compra es un **cliente**, `idCliente` y `idCanal` se ignoran: se usan el cliente del token y el canal Web. Si es **admin**, `idCliente` es obligatorio (venta en boletería).

### Reportes (todo 🛡️)
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/reporte/peliculas-mas-vendidas?top=5&desde=&hasta=` | Ranking por entradas vendidas |
| GET | `/api/reporte/ventas-mensuales?anio=2026` | 12 meses con ventas, entradas y recaudación |
| GET | `/api/reporte/anios` | Años con ventas (para el selector) |

### Catálogos (todo 🔓)
`/api/catalogo/clasificaciones`, `/formatos`, `/tipos-espectador`, `/formas-pago`, `/canales`, `/salas`, `/tipos-sala`, `/promociones`

---

## 9. Reglas de negocio importantes

### Venta (maestro-detalle)

**1) `VentaService.RegistrarAsync` — reglas de negocio**
1. Quién compra: un **cliente** compra a su nombre por el canal Web; un **admin** vende en boletería y tiene que elegir un cliente activo.
2. Entre 1 y 10 entradas.
3. Para cada entrada:
   - la función existe, está activa y **no empezó** (se compara fecha **y hora**);
   - la butaca **pertenece a la sala de esa función**, está habilitada y **libre** (ni repetida en la misma compra);
   - el tipo de espectador es válido;
   - **película +18**: no se venden entradas de tipo **Menor** ni a un **cliente menor de edad** (según su fecha de nacimiento);
   - **precio** calculado por `PrecioService`: promo de la función, o **15% para Jubilado / Menor** (no se acumulan).
4. Formas de pago: si viene una sola sin monto, paga el total. Si vienen varias, la suma tiene que dar el total.

**2) `VentaRepository.RegistrarVentaAsync` — transacción `Serializable`**
1. Vuelve a verificar, dentro de la transacción, que ninguna butaca se haya vendido en ese instante (dos compras simultáneas). Si pasó, `ROLLBACK` y el servicio responde 409.
2. Inserta la **VENTA** (maestro) → obtiene el `id_venta`.
3. Inserta las **ENTRADAs** (detalle) y las **VentaxFormaPago** con ese id.
4. `COMMIT`. Si cualquier paso falla: `ROLLBACK` y no se guarda nada.

### Descuentos
| Caso | Precio |
|---|---|
| Función **con promoción** (ej. 2x1 = 50%) | Todos pagan con el descuento de la promo (el 15% no se suma) |
| Función sin promoción, entrada **General** | Precio base |
| Función sin promoción, entrada **Jubilado** o **Menor** | Precio base − 15% |

### Otras reglas
- **Bajas lógicas**: películas, funciones y clientes no se borran, se marcan `activo = 0` (así las ventas viejas siguen teniendo sus datos).
- **Funciones**: no se pueden crear en fecha/hora pasada ni **superponer** con otra función de la misma sala (se usa la duración de la película). Las que ya empezaron no aparecen como "próximas".
- **Funciones con entradas vendidas**: no se puede cambiar película, sala, fecha ni hora, ni cancelarlas. Solo formato y promoción.
- **Cartelera vs. estrenos**: no hay un campo "estado"; se deduce. Con funciones desde hoy → cartelera. Sin funciones → próximo estreno.
- **Precios**: al cargar un precio nuevo desde una fecha, el que estaba vigente se cierra el día anterior. Las ventas ya hechas no cambian (cada ENTRADA guarda su `precio_final`).
- **Salas**: el **tipo de sala** define el precio. Para tener funciones 3D hay que crear una sala de tipo 3D (y cargarle precio a ese tipo).
- **Clientes**: no se permite repetir DNI.

---

## 10. Frontend (proyecto Web)

HTML + CSS + JavaScript, con **Bootstrap 5.3** para el diseño responsivo y los componentes. Cada página carga:

```html
<link href=".../bootstrap.min.css" rel="stylesheet">     <!-- Bootstrap -->
<link rel="stylesheet" href="css/style.css">            <!-- personalización (después de Bootstrap) -->
...
<script src=".../bootstrap.bundle.min.js"></script>     <!-- JS de Bootstrap: navbar, modales, offcanvas -->
<script src=".../sweetalert2.all.min.js"></script>      <!-- alertas -->
<script src="js/api.js"></script>     <!-- SIEMPRE antes que los demás: API, sesión, utilidades -->
<script src="js/main.js"></script>    <!-- páginas públicas: navbar y login -->
<script src="js/xxx.js"></script>     <!-- lógica propia de la página -->
```

### Bootstrap: qué se usa y dónde
| Componente / utilidad | Dónde |
|---|---|
| **Grilla** (`container`, `row`, `row-cols-*`, `col-md-*`, `g-*`) | Cartelera y estrenos (1 → 2 → 3 → 4 columnas según el ancho), formularios, dashboard |
| **Navbar** con `navbar-expand-lg` + `collapse` | Barra superior: en celular se colapsa en el botón ☰ |
| **Offcanvas** (`offcanvas-lg`) | Menú lateral del panel: fijo en pantallas grandes, deslizable en celular |
| **Cards** | Tarjetas de películas, pasos de la compra, métricas y secciones del panel |
| **Modales** (`modal`, `modal-dialog`, `data-bs-dismiss`) | Login, registro, mis compras y todos los formularios del panel |
| **Formularios** (`form-label`, `form-control`, `form-select`, `form-check`, `form-text`) | Todos los formularios |
| **Botones, badges, alerts, list-group, tables, spinners** | En todo el sitio |
| **Utilidades** (`d-flex`, `gap-*`, `mb-*`, `p-*`, `text-*`, `fw-*`, `rounded-*`) | En todo el sitio |
| **Color modes** (`data-bs-theme="dark"`/`"light"`) | Botón de tema claro/oscuro |

### CSS propio (`style.css` y `admin.css`)
- **Variables CSS**: los colores del cine (`--cine-primary`…) y se **sobrescriben variables de Bootstrap** (`--bs-primary`, `--bs-body-bg`…) para cada tema.
- **Selectores**: de clase (`.poster`), descendientes (`.navbar-transparente .btn`), de atributo (`[data-bs-theme="dark"]`, `[id^="step"]`), pseudo-clases (`:hover`, `:disabled`, `:focus-visible`, `:has()`), pseudo-elementos (`::before`, `::first-letter`).
- **Box model**: `padding`, `border`, `margin`, `border-radius` y `box-shadow` en pósters, butacas, tarjetas.
- Lo que Bootstrap no trae: hero, pósters generados, **carrusel 3D**, **mapa de butacas**.
- **Responsive propio**: `@media` para el carrusel y el póster de la compra.

### `api.js` — la base de todo
| Función / objeto | Para qué |
|---|---|
| `API_URL` | Dirección de la API |
| `Auth` | Sesión en `localStorage`: `Auth.sesion`, `Auth.token`, `Auth.esAdmin()`, `Auth.guardar()`, `Auth.cerrar()` |
| `api(ruta, { method, body })` | `fetch` que agrega el token, convierte a JSON y **lanza un Error con el mensaje de la API** si falla. Acepta `FormData` para subir archivos |
| `toast()`, `alerta()`, `confirmar()` | Alertas con **SweetAlert2** (con estilos del tema). `confirmar()` devuelve `true`/`false` |
| `fmtMoneda`, `fmtFecha`, `fmtFechaLarga`, `fmtHora`, `hoyISO`… | Formatos en español (Argentina) |
| `esc()` | Escapa HTML para no inyectar código con datos de la base |
| `posterHTML()` | Dibuja el póster: imagen real, o uno generado con el título si no hay imagen |
| `abrirModal()` / `cerrarModal()` | Abren/cierran un **modal de Bootstrap** (`bootstrap.Modal`) |
| `Tema` | Modo claro / oscuro con el `data-bs-theme` de Bootstrap (se guarda en `localStorage`) |
| `htmlDetalleVenta()` | Tabla con el detalle de una venta (comprobante, "Mis compras" y panel) |

### `main.js` — páginas públicas
- Dibuja la **navbar de Bootstrap** en `<header id="navbarCine">` (así no se repite en cada HTML): transparente en el inicio y sólida al scrollear, colapsable en celulares, link activo.
- Inyecta los **modales de Login, Registro y "Mis compras"** en todas las páginas (están en un solo lugar).
- Muestra "Iniciar sesión / Registrarse" o "Hola, nombre / Mis compras / Salir" según la sesión.
- Agrega el link **Panel de administrador** solo si el rol es admin.

### `butacas.js` — componente reutilizable
- `crearMapaButacas(contenedor, butacas, { max, onChange })`: dibuja la sala (pantalla, filas, butacas libres, ocupadas y seleccionadas).
- `renderDetalleEntradas(contenedor, seleccion, funcion, onCambio)`: lista las butacas elegidas con un `<select>` de tipo de espectador (General / Jubilado / Menor, con el precio que calculó el servidor) y devuelve `{ entradas(), total(), detalle() }`.
- **Closures**: las dos funciones crean variables internas (`seleccionadas`, la lista de selects) y devuelven funciones que las siguen usando después de terminar. Desde afuera no se pueden modificar directamente.

Se usa en **reserva.html** (compra web) y en **admin.html** (venta en boletería).

### Páginas
| Página | Script | Qué hace |
|---|---|---|
| `index.html` | `index.js` | Hero + carrusel 3D de la cartelera |
| `cartelera.html` | `cartelera.js` | Tarjetas con horarios (chips) que llevan directo a la compra |
| `estrenos.html` | inline | Películas sin funciones |
| `promociones.html` | inline | Promociones y funciones adheridas |
| `reserva.html` | `reserva.js` + `butacas.js` | Compra: 1) función → 2) butacas y tipo de espectador → 3) pago → comprobante |
| `admin.html` | `admin.js` + `butacas.js` + Chart.js | Panel completo |

### `admin.js` — panel de administración
Navegación por **hash** (`admin.html#ventas`): cada vez que entrás a una sección se recargan sus datos.

| Sección | Qué tiene |
|---|---|
| **Dashboard** | 4 indicadores (recaudado, entradas, ventas, funciones próximas) + **Películas más vendidas** (gráfico de barras horizontal + tabla, filtro de fechas y Top 5/10) + **Ventas mes a mes** (gráfico de barras + tabla, selector de año). Gráficos con **Chart.js**, se redibujan al cambiar el tema |
| **Películas** | ABM con búsqueda; póster subiendo archivo o pegando un link |
| **Funciones** | ABM con filtro por película y "incluir pasadas"; muestra la fecha leída en palabras para evitar confusiones dd/mm vs mm/dd |
| **Salas** | ABM con generación de butacas |
| **Precios** | Tarjeta con el precio actual por tipo de sala + historial + nuevo precio programado |
| **Clientes** | ABM con búsqueda por nombre o DNI |
| **Ventas** | Listado, detalle y **Nueva venta** (cliente + función + mapa de butacas + forma de pago) |

---

## 11. Flujos completos paso a paso

### Compra web de un cliente
1. `cartelera.html` → `GET /api/pelicula/cartelera` y `GET /api/funcion?soloProximas=true`.
2. Clic en un horario → `reserva.html?id=2&funcion=5`.
3. `reserva.js` carga la película, sus funciones, tipos de espectador y formas de pago.
4. Al elegir la función → `GET /api/funcion/5/butacas` → `crearMapaButacas`.
5. Elige butacas y tipo de espectador → se arma el resumen.
6. "Confirmar compra": si no hay sesión, abre el login y después continúa.
7. `POST /api/venta` con el token → transacción → devuelve `idVenta`.
8. `GET /api/venta/{id}` → se muestra el comprobante.

### Venta en boletería (admin)
Panel → Ventas → **Nueva venta** → elegir cliente y función → butacas → forma de pago → **Registrar venta** (`POST /api/venta` con `idCliente` y canal Boletería).

### Habilitar funciones 3D
Panel → **Precios** (cargar precio a "3D XD") → **Salas** (crear una sala tipo 3D XD) → **Funciones** (crear la función en esa sala).

---

## 12. Probar la API con Postman

En `postman/CineHollywood.postman_collection.json` hay una colección lista para importar.

1. Levantar la API (`CineHollywoodTPI.API`).
2. En Postman: **Import → File →** elegir el archivo.
3. Correr **`1. Auth > Login admin`**: guarda el token en la variable `{{token}}`. La colección tiene **Auth = Bearer {{token}}**, así que todas las requests lo usan solas.
4. Para correr todo: botón **Run** sobre la colección (Collection Runner). Las requests están **en orden** y se pasan los ids por variables (`{{idPelicula}}`, `{{idFuncion}}`, `{{idButaca}}`, `{{idVenta}}`…).

| Carpeta | Qué prueba |
|---|---|
| 1. Auth | Login (200), login incorrecto (401), registro (201), email repetido (409) |
| 2. Catálogos | Listas públicas (200) |
| 3. Películas | Listar, crear (**201** + `Location`), obtener, modificar, título vacío (400), inexistente (404), sin token (401), como cliente (403) |
| 4. Salas | Crear (201), nombre repetido (409), 30 filas (400) |
| 5. Precios | Programar precio (201), fecha pasada (400) |
| 6. Funciones | Crear (201), superpuesta (409), fecha pasada (400), **precio con 15% para Jubilado**, butacas |
| 7. Clientes | Crear (201), modificar, DNI repetido (409), DNI inválido (400) |
| 8. Ventas | **Venta (transacción maestro-detalle) 201**, misma butaca (409), sin entradas (400), detalle, venta ajena (403), cancelar/mover función con ventas (400) |
| 9. Reportes | Más vendidas, mes a mes (12 meses), top inválido (400), como cliente (403) |
| 10. Limpieza | Baja física del precio y bajas lógicas de película y cliente; dar de baja dos veces (404) |

Cada request tiene **tests** (pestaña *Tests*) que verifican el código HTTP y algunos datos de la respuesta, y un script que guarda los ids para las siguientes. Se probó de punta a punta: **60/60 OK**.

> Correr la colección completa **crea datos de prueba** (película, sala, función, cliente, venta). La carpeta 10 da de baja lo que se puede; la sala y la función quedan porque tienen una venta (regla de negocio).

---

## 13. Temas de la materia: dónde está cada uno

| Tema | Dónde verlo en el proyecto |
|---|---|
| **Arquitectura cliente-servidor** | Cliente: `CineHollywoodTPI.Web` (navegador, puerto 5020). Servidor: `CineHollywoodTPI.API` (puerto 5191). Se comunican por **HTTP** intercambiando **JSON** |
| **Sistemas basados en HTTP / peticiones y respuestas** | `api.js` → función `api()` arma la petición (método, headers, body) con `fetch`; la API responde con código + JSON |
| **Códigos de estado** | Ver [sección 8](#códigos-de-respuesta-que-usa-la-api): 200, 201, 400, 401, 403, 404, 409, 500. Los errores salen de `ManejoErroresMiddleware` |
| **Web API con C#, controladores, POST/PUT/DELETE** | `CineHollywoodTPI/Controllers/*` con `[HttpGet]`, `[HttpPost]`, `[HttpPut("{id}")]`, `[HttpDelete("{id}")]`, `[FromBody]`, `[FromQuery]` |
| **Mensajes de respuesta** | `Ok(...)`, `CreatedAtAction(...)` (201), excepciones → 400/404/409 con mensaje en español |
| **Postman** | `postman/CineHollywood.postman_collection.json` ([sección 12](#12-probar-la-api-con-postman)) |
| **Entity Framework Core** | `CineHollywoodTPI.Data/Models` (scaffold), `SistemaCineContext` |
| **DbContext / Unit of Work** | El `SistemaCineContext` se inyecta por request (*scoped*): todos los cambios se acumulan y se confirman juntos con **`SaveChangesAsync()`**. Ej.: `ClienteRepository.AddAsync` guarda PERSONA + CLIENTE en un solo `SaveChanges`; `SalaRepository.CrearAsync` guarda la sala con todas sus butacas |
| **Manejo de entidades / LINQ** | Repositorios: `Where`, `Include`/`ThenInclude`, `Select`, `GroupBy`, `OrderBy`, `Any`, `Count`, `Sum` (ej. `ReporteRepository.GetPeliculasMasVendidasAsync`) |
| **async / await / manejo de errores** | Todo el backend es `async Task<...>`; errores con excepciones propias (`Services/Excepciones.cs`) + middleware. En el front, `async/await` con `try/catch` en cada llamada a la API |
| **Transacciones con EF** | `BeginTransactionAsync` / `CommitAsync` / `RollbackAsync` en `VentaRepository.RegistrarVentaAsync` (maestro-detalle, `Serializable`), `SalaRepository`, `PrecioRepository`, `UsuarioRepository` |
| **Patrón Repository** | Interfaces en `Data/Interfaces`, implementaciones en `Data/Repositories`, inyectadas con DI. A diferencia de un **DAO** (un objeto por tabla con CRUD básico), los repositorios exponen métodos del dominio (`GetEnCarteleraAsync`, `TieneEntradasVendidasAsync`) y pueden abarcar varias tablas |
| **Bajas lógicas y físicas** | Lógicas (`activo = 0`): películas, funciones, clientes. Físicas (`DELETE`): salas sin funciones, precios que todavía no empezaron |
| **Formularios HTML** | Login, registro y todos los ABM del panel (`<form>`, `<label for>`, `<input type="date/number/email/file">`, `<select>`, `required`) |
| **CSS: selectores, propiedades, variables** | `css/style.css` (ver [sección 10](#css-propio-stylecss-y-admincss)) |
| **Box model** | Butacas, pósters, tarjetas (`padding`, `border`, `margin`, `border-radius`) |
| **Responsive con Bootstrap** | Grilla `row-cols-*`, navbar colapsable, offcanvas del panel, `table-responsive` ([sección 10](#bootstrap-qué-se-usa-y-dónde)) |
| **JavaScript: sintaxis, DOM** | Todo el front arma el HTML con template strings y `innerHTML`, `querySelector`, `classList`, `dataset` |
| **Eventos** | `addEventListener('click' / 'submit' / 'change' / 'input' / 'scroll' / 'DOMContentLoaded')`, eventos propios (`sesion-cambiada`, `tema-cambiado`) y de Bootstrap (`shown.bs.modal`) |
| **Funciones y closures** | `butacas.js` (`crearMapaButacas`, `renderDetalleEntradas`), `index.js` (el carrusel guarda `actual` en un closure) |

---

## 14. Problemas comunes

| Síntoma | Causa / solución |
|---|---|
| "No se pudo conectar con la API" | La API no está corriendo. Iniciá **los dos** proyectos. |
| `admin.html` te manda al inicio | No iniciaste sesión como **Administrador** (o te logueaste con una cuenta de cliente). Cerrá sesión y entrá con el admin. |
| Error 404 en una sección nueva del panel | La API que está corriendo es una versión vieja: detené y volvé a compilar. |
| Cambios de CSS/JS no se ven | El navegador cachea: **Ctrl+F5**. |
| "Una directiva de Control de aplicaciones bloqueó este archivo" | Windows (Smart App Control) bloquea el `.exe` recién compilado. Se puede ejecutar con `dotnet bin\Debug\net8.0\<Proyecto>.dll` o desactivar Smart App Control. |
| Error de compilación "el archivo está siendo usado por otro proceso" | La app está corriendo: detenela antes de compilar. |
| El póster no se ve | Usá **subir archivo** o pegá el link en el formulario (la API lo descarga). |
| "Ya hay otra función en esa sala que se superpone" | Elegí otro horario/sala: se tiene en cuenta la duración de la película. |
| El precio de las funciones 3D es $0 | Falta cargar precio a ese tipo de sala en **Precios**. |

---

## 15. Cómo agregar algo nuevo

Ejemplo: un ABM de **Promociones**.

1. **Data/DTOs**: crear `PromocionInsertDTO` (y usar `PromocionDTO` para lectura).
2. **Data/Interfaces + Repositories**: `IPromocionRepository` / `PromocionRepository` con `_context.Promociones`. **Solo consultas y guardado**, sin reglas.
3. **API/Services/Interfaces + Services**: `IPromocionService` / `PromocionService` con **las reglas** (por ejemplo, porcentaje entre 1 y 100, descripción obligatoria). Si algo no se cumple: `throw new NegocioException("mensaje")` (o `NoEncontradoException`, `ConflictoException`…).
4. **API/Program.cs**: registrar los dos:
   ```csharp
   builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();
   builder.Services.AddScoped<IPromocionService, PromocionService>();
   ```
5. **API/Controllers**: `PromocionController` con `[Authorize(Roles = "Administrador")]` que **solo llama al servicio** (sin `try/catch`, el middleware se encarga de los errores).
6. **Web/admin.html**: agregar un `<li>` en el menú con `data-target="promociones"`, una `<section id="section-promociones" class="admin-section" hidden>` y el modal.
7. **Web/js/admin.js**: agregar `promociones` en `TITULOS` y en el mapa de `mostrarSeccion`, y las funciones `cargar…`, `render…`, `abrirForm…`, `guardar…`, `borrar…` copiando el patrón de Clientes.

> ⚠️ **Seguridad**: la clave JWT y la contraseña del admin están en `appsettings.json` porque es un trabajo práctico. En un sistema real irían en *User Secrets* o variables de entorno, y la clave se cambiaría.
