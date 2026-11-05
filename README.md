# 🎬 Cines Hollywood — TPI

Sistema web para un cine: cartelera pública, compra de entradas online eligiendo butaca, y un panel de administración con ABM de películas, funciones, salas, precios y clientes, ventas en boletería y reportes.

- **Backend:** ASP.NET Core 8 Web API + Entity Framework Core (SQL Server)
- **Frontend:** HTML + CSS + JavaScript puro (sin frameworks), servido por un proyecto ASP.NET aparte
- **Seguridad:** login con **JWT** y roles (`Administrador` / `Cliente`)
- **Librerías del front (por CDN):** [Chart.js](https://www.chartjs.org/) (gráficos), [SweetAlert2](https://sweetalert2.github.io/) (alertas), [Feather Icons](https://feathericons.com/) (íconos del panel)

---

## Índice

1. [Cómo levantar el proyecto](#1-cómo-levantar-el-proyecto)
2. [Arquitectura general](#2-arquitectura-general)
3. [Estructura de carpetas](#3-estructura-de-carpetas)
4. [Base de datos y modelo](#4-base-de-datos-y-modelo)
5. [Proyecto Data (repositorios y DTOs)](#5-proyecto-data-repositorios-y-dtos)
6. [Proyecto API (controladores y servicios)](#6-proyecto-api-controladores-y-servicios)
7. [Autenticación con JWT](#7-autenticación-con-jwt)
8. [Lista completa de endpoints](#8-lista-completa-de-endpoints)
9. [Reglas de negocio importantes](#9-reglas-de-negocio-importantes)
10. [Frontend (proyecto Web)](#10-frontend-proyecto-web)
11. [Flujos completos paso a paso](#11-flujos-completos-paso-a-paso)
12. [Problemas comunes](#12-problemas-comunes)
13. [Cómo agregar algo nuevo](#13-cómo-agregar-algo-nuevo)

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
┌──────────────────────────────┐      fetch + JWT       ┌───────────────────────────────┐
│  CineHollywoodTPI.Web        │ ─────────────────────▶ │  CineHollywoodTPI.API         │
│  (puerto 5020)               │ ◀───────────────────── │  (puerto 5191)                │
│  HTML / CSS / JS estáticos   │        JSON            │  Controllers + Services       │
└──────────────────────────────┘                        └──────────────┬────────────────┘
                                                                       │ usa
                                                        ┌──────────────▼────────────────┐
                                                        │  CineHollywoodTPI.Data        │
                                                        │  Models (EF Core) + DTOs      │
                                                        │  Interfaces + Repositories    │
                                                        └──────────────┬────────────────┘
                                                                       │ EF Core
                                                        ┌──────────────▼────────────────┐
                                                        │  SQL Server                   │
                                                        │  SistemaCineBdFinal           │
                                                        └───────────────────────────────┘
```

**Patrón Repositorio en 3 capas:**

1. El **frontend** (JS) llama a un endpoint de la API con `fetch`.
2. El **controlador** valida los datos de entrada, revisa permisos (JWT/rol) y llama al repositorio.
3. El **repositorio** habla con la base usando Entity Framework Core (consultas LINQ, transacciones).
4. El controlador convierte el resultado a un **DTO** (objeto liviano) y lo devuelve como JSON.

¿Por qué DTOs? Los modelos de EF tienen navegaciones circulares (Venta → Entrada → Venta…) y campos que el navegador no necesita. Los DTOs devuelven solo lo justo y evitan errores de serialización.

¿Por qué interfaces (`IPeliculaRepository`, etc.)? Porque se registran con **inyección de dependencias** en `Program.cs` (`AddScoped<IPeliculaRepository, PeliculaRepository>()`). El controlador recibe la interfaz en el constructor y no le importa cómo está implementada.

---

## 3. Estructura de carpetas

```
CineHollywoodTPI.slnx                  ← la solución (3 proyectos)
│
├── CineHollywoodTPI/                  ← PROYECTO API
│   ├── Program.cs                     ← configuración: BD, DI, JWT, CORS, Swagger, admin inicial
│   ├── appsettings.json               ← conexión a la BD, clave JWT, admin inicial
│   ├── Controllers/
│   │   ├── AuthController.cs          ← login / registro (JWT)
│   │   ├── PeliculaController.cs      ← ABM películas + subida de pósters
│   │   ├── FuncionController.cs       ← ABM funciones + mapa de butacas
│   │   ├── SalaController.cs          ← ABM salas (genera butacas)
│   │   ├── PrecioController.cs        ← precios por tipo de sala
│   │   ├── ClienteController.cs       ← ABM clientes
│   │   ├── VentaController.cs         ← ventas (transacción maestro-detalle)
│   │   ├── ReporteController.cs       ← reportes del dashboard
│   │   └── CatalogoController.cs      ← listas para los <select>
│   ├── Services/
│   │   ├── TokenService.cs            ← arma y firma el JWT
│   │   └── PosterService.cs           ← guarda/descarga imágenes de pósters
│   └── wwwroot/posters/               ← acá se guardan los pósters subidos
│
├── CineHollywoodTPI.Data/             ← PROYECTO DE DATOS (biblioteca de clases)
│   ├── Models/                        ← entidades generadas por scaffold de EF + SistemaCineContext
│   │   ├── SistemaCineContext.Partial.cs ← mapeo extra (PosterUrl) sin tocar el scaffold
│   │   └── Pelicula.Partial.cs           ← propiedad PosterUrl
│   ├── DTOs/                          ← objetos que viajan como JSON
│   ├── Interfaces/                    ← contratos de los repositorios
│   └── Repositories/                  ← implementación con EF Core (+ PrecioHelper)
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
| `CineHollywoodTPI.Controllers/VentaController.cs` | Archivo vacío de una versión vieja; no pertenece a ningún proyecto y se puede borrar |

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
| `TIPO_ESPECTADOR`, `FORMA_PAGO`, `CANAL_VENTA`, `COMPLEJO` | Tablas de referencia |

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

### Repositorios

| Repositorio | Qué hace |
|---|---|
| `PeliculaRepository` | Listado de activas, búsqueda por nombre, **cartelera** (activas con funciones desde hoy), **estrenos** (activas sin funciones), alta, modificación, baja lógica |
| `FuncionRepository` | Funciones activas (filtro por película y "desde hoy"), **mapa de butacas** de una función (libre/ocupada), validación de **superposición de horarios** en la misma sala, ABM |
| `SalaRepository` | ABM de salas. Al crear una sala **genera las butacas** (filas A…Z × números). No deja cambiar la cantidad de butacas si ya hay entradas vendidas; no deja borrar una sala con funciones |
| `PrecioRepository` | Lista de precios con estado (Vigente / Programado / Histórico), alta de precio nuevo (cierra el anterior), baja de precios futuros |
| `ClienteRepository` | ABM de clientes (crea PERSONA + CLIENTE juntos), control de DNI repetido |
| `VentaRepository` | Listado y detalle de ventas, totales, y la **transacción de venta** (ver [sección 9](#9-reglas-de-negocio-importantes)) |
| `ReporteRepository` | Películas más vendidas y ventas mes a mes |
| `UsuarioRepository` | Buscar usuario por email (login), registro de clientes, creación del admin inicial |
| `CatalogoRepository` | Listas cortas para combos: clasificaciones, formatos, salas, tipos de sala, tipos de espectador, formas de pago, canales, promociones |
| `PrecioHelper` (clase estática) | Calcula el precio de la entrada de una función (ver abajo) |

### Cálculo del precio (`PrecioHelper.CalcularPrecioEntrada`)

```
precio = precio vigente del TIPO DE SALA para la FECHA de la función
         × (1 − % de descuento de la PROMOCIÓN de la función)
```

- Se busca en `PRECIO_TIPO_SALA` el registro cuya vigencia incluye la fecha de la función.
- Si no hay ninguno, se usa el más reciente cargado. Si no hay precios para ese tipo de sala, el precio es $0 (por eso el panel avisa "Sin precio").
- **El precio siempre lo calcula el servidor**, nunca se confía en lo que manda el navegador.

### DTOs principales

| Archivo | Contenido |
|---|---|
| `PeliculaDTO`, `PeliculaInsertDTO`, `PeliculaUpdateDTO` (+ `PosterUrlDTO`) | Datos de película |
| `FuncionDTO`, `FuncionInsertDTO`, `FuncionUpdateDTO` | La DTO de lectura trae además título, sala, tipo de sala, formato, promoción y **precio calculado** |
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

## 6. Proyecto API (controladores y servicios)

### `Program.cs` (en orden)
1. **CORS** `PermitirTodo`: deja que el sitio (puerto 5020) llame a la API (puerto 5191).
2. **DbContext** con la cadena `DefaultConnection`.
3. **Inyección de dependencias**: todos los repositorios, `TokenService`, `PosterService`, `PasswordHasher<Usuario>` y un `HttpClient` llamado `"posters"`.
4. **Autenticación JWT**: valida emisor, audiencia, vencimiento y firma con la clave de `appsettings.json`.
5. **Controllers + Swagger** con botón *Authorize* para pegar el token.
6. **Admin inicial**: si no existe ningún usuario con rol Administrador, lo crea.
7. Middleware: `UseStaticFiles` (pósters) → `UseCors` → `UseAuthentication` → `UseAuthorization` → `MapControllers`.

> En desarrollo **no** se usa `UseHttpsRedirection` porque el front llama por HTTP y la redirección rompe CORS.

### Patrón de los controladores
Todos siguen la misma forma:

```csharp
[HttpPost]
[Authorize(Roles = "Administrador")]        // solo admin
public async Task<IActionResult> Crear([FromBody] XInsertDTO dto)
{
    try
    {
        // 1. validar → BadRequest("mensaje claro")
        // 2. llamar al repositorio
        // 3. devolver Ok(new { mensaje = "...", id })
    }
    catch (InvalidOperationException ex) { return BadRequest(ex.Message); } // error de negocio
    catch (Exception ex) { return StatusCode(500, $"Error interno: {ex.Message}"); }
}
```

Los repositorios lanzan `InvalidOperationException` con un mensaje en español cuando una regla de negocio no se cumple (butaca ocupada, sala con ventas, etc.). El controlador la convierte en un **400** y el front muestra ese mismo mensaje.

### Servicios
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

### Transacción de venta (maestro-detalle) — `VentaRepository.RegistrarVentaAsync`
Todo ocurre dentro de **una transacción `Serializable`** (si algo falla, `ROLLBACK` y no se guarda nada):

1. Valida cada entrada:
   - la función existe, está activa y no pasó;
   - la butaca existe, **pertenece a la sala de esa función** y está habilitada;
   - la butaca **no está vendida** para esa función (ni repetida dentro de la misma compra);
   - el tipo de espectador es válido.
2. **Calcula el precio** de cada entrada en el servidor (`PrecioHelper`).
3. Formas de pago: si viene una sola sin monto, paga el total. Si vienen varias, la suma tiene que dar el total.
4. Inserta la **VENTA** (maestro) → obtiene el `id_venta`.
5. Inserta las **ENTRADAs** (detalle) y las **VentaxFormaPago** con ese id.
6. `COMMIT`.

El aislamiento `Serializable` evita que dos personas compren la misma butaca al mismo tiempo.

### Otras reglas
- **Bajas lógicas**: películas, funciones y clientes no se borran, se marcan `activo = 0` (así las ventas viejas siguen teniendo sus datos).
- **Funciones**: no se pueden crear en fecha pasada ni **superponer** con otra función de la misma sala (se usa la duración de la película).
- **Cartelera vs. estrenos**: no hay un campo "estado"; se deduce. Con funciones desde hoy → cartelera. Sin funciones → próximo estreno.
- **Precios**: al cargar un precio nuevo desde una fecha, el que estaba vigente se cierra el día anterior. Las ventas ya hechas no cambian (cada ENTRADA guarda su `precio_final`).
- **Salas**: el **tipo de sala** define el precio. Para tener funciones 3D hay que crear una sala de tipo 3D (y cargarle precio a ese tipo).
- **Clientes**: no se permite repetir DNI.

---

## 10. Frontend (proyecto Web)

No usa frameworks: cada página HTML carga sus scripts en este orden:

```html
<script src="...sweetalert2.all.min.js"></script>  <!-- alertas -->
<script src="js/api.js"></script>     <!-- SIEMPRE primero: API, sesión, utilidades -->
<script src="js/main.js"></script>    <!-- páginas públicas: navbar y login -->
<script src="js/xxx.js"></script>     <!-- lógica propia de la página -->
```

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
| `abrirModal()` / `cerrarModal()` | Modales propios (cierran con Escape o clic afuera) |
| `Tema` | Modo claro / oscuro (se guarda en `localStorage`) |
| `htmlDetalleVenta()` | Tabla con el detalle de una venta (comprobante, "Mis compras" y panel) |

### `main.js` — páginas públicas
- Navbar: transparente en el inicio y sólida al scrollear, menú en celulares, link activo.
- Inyecta los **modales de Login, Registro y "Mis compras"** en todas las páginas (están en un solo lugar).
- Muestra "Iniciar sesión / Registrarse" o "Hola, nombre / Mis compras / Salir" según la sesión.
- Agrega el link **Panel de administrador** solo si el rol es admin.

### `butacas.js` — componente reutilizable
- `crearMapaButacas(contenedor, butacas, { max, onChange })`: dibuja la sala (pantalla, filas, butacas libres, ocupadas y seleccionadas).
- `renderDetalleEntradas(...)`: lista las butacas elegidas con un `<select>` de tipo de espectador cada una y devuelve una función que arma el array `entradas` para la API.

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

## 12. Problemas comunes

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

## 13. Cómo agregar algo nuevo

Ejemplo: un ABM de **Promociones**.

1. **Data/DTOs**: crear `PromocionInsertDTO` (y usar `PromocionDTO` para lectura).
2. **Data/Interfaces**: `IPromocionRepository` con los métodos.
3. **Data/Repositories**: `PromocionRepository` implementando la interfaz con `_context.Promociones`. Si algo no es válido, `throw new InvalidOperationException("mensaje")`.
4. **API/Program.cs**: `builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();`
5. **API/Controllers**: `PromocionController` con `[Authorize(Roles = "Administrador")]` siguiendo el patrón del punto 6.
6. **Web/admin.html**: agregar un `<li>` en el menú con `data-target="promociones"`, una `<section id="section-promociones" class="admin-section" hidden>` y el modal.
7. **Web/js/admin.js**: agregar `promociones` en `TITULOS` y en el mapa de `mostrarSeccion`, y las funciones `cargar…`, `render…`, `abrirForm…`, `guardar…`, `borrar…` copiando el patrón de Clientes.

> ⚠️ **Seguridad**: la clave JWT y la contraseña del admin están en `appsettings.json` porque es un trabajo práctico. En un sistema real irían en *User Secrets* o variables de entorno, y la clave se cambiaría.
