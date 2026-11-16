# 📘 Guía para entender Cines Hollywood a fondo

Esta guía está pensada para alguien que **sabe algo de backend y nada de frontend**. Todo lo que se explica sale del código real del repo; 
cuando algo no está claro en el código, se avisa con **⚠️ No está claro en el código**.

El [README](../README.md) ya describe la arquitectura del backend, los endpoints y las reglas de negocio. Esta guía **no repite eso**: 
se enfoca en las tecnologías, en JWT y, sobre todo, en el frontend.

> Las rutas son relativas a la raíz del repo. Los números de línea (`archivo:línea`) corresponden al código al momento de escribir la guía;
si el código cambia pueden correrse un poco.

---

## Índice

1. [El proyecto en 1 minuto](#1-el-proyecto-en-1-minuto)
2. [Tecnologías y librerías](#2-tecnologías-y-librerías)
3. [JWT completo](#3-jwt-completo)
4. [Frontend desde cero](#4-frontend-desde-cero)
   - 4.1 [Cómo llega una página al navegador](#41-cómo-llega-una-página-al-navegador)
   - 4.2 [HTML](#42-html)
   - 4.3 [CSS](#43-css)
   - 4.4 [JavaScript](#44-javascript)
   - 4.5 [Nuestros archivos JS, uno por uno](#45-nuestros-archivos-js-uno-por-uno)
   - 4.6 [Cómo se conectan HTML, CSS y JS](#46-cómo-se-conectan-html-css-y-js)
5. [Recorridos paso a paso](#5-recorridos-paso-a-paso)
   - 5.1 [Login](#51-recorrido-1-login)
   - 5.2 [Elegir una película y una butaca](#52-recorrido-2-de-la-cartelera-al-mapa-de-butacas)
   - 5.3 [Cargar un gráfico del dashboard](#53-recorrido-3-cargar-el-gráfico-películas-más-vendidas)
6. [Cosas que no quedan claras en el código](#6-cosas-que-no-quedan-claras-en-el-código)
7. [Glosario](#7-glosario)

---

## 1. El proyecto en 1 minuto

Hay **dos programas que corren al mismo tiempo**:

| Proyecto | Puerto | Qué hace |
|---|---|---|
| `CineHollywoodTPI` (la **API**) | `http://localhost:5191` | Recibe pedidos HTTP, aplica reglas de negocio, lee/escribe SQL Server y responde **JSON** |
| `CineHollywoodTPI.Web` (el **sitio**) | `http://localhost:5020` | **Solo entrega archivos** (HTML, CSS, JS). No tiene lógica: todo lo hace el JavaScript
que corre en el navegador |

Y una biblioteca de clases, `CineHollywoodTPI.Data`, con los modelos de EF, los DTOs y los repositorios, que usa la API.

```
 Navegador (Chrome, Edge…)
 ┌──────────────────────────────────────────────┐
 │ 1. Pide index.html, style.css, api.js…  ─────┼──▶ CineHollywoodTPI.Web (5020)
 │    (archivos estáticos)              ◀───────┼──  devuelve los archivos tal cual
 │                                              │
 │ 2. El JS de la página llama a la API ────────┼──▶ CineHollywoodTPI (API, 5191)
 │    fetch('http://localhost:5191/api/...')    │      Controller → Service → Repository → SQL Server
 │    con el header Authorization: Bearer <JWT> │
 │                                       ◀──────┼──  JSON
 │ 3. El JS arma el HTML con esos datos         │
 └──────────────────────────────────────────────┘
```

La idea clave para alguien de backend: **el servidor Web no arma HTML con datos** (no es Razor ni MVC con vistas). 
Entrega HTML "vacío" y el JavaScript, ya en el navegador, le pide los datos a la API y completa la página.

---

## 2. Tecnologías y librerías

### 2.1 Backend

| Tecnología | Qué es | Para qué la usamos acá | Dónde se usa |
|---|---|---|---|
| **.NET 8 / ASP.NET Core** | Framework de Microsoft para hacer aplicaciones web en C# | Los dos proyectos web (API y sitio) | `CineHollywoodTPI/CineHollywoodTPI.API.csproj`, `CineHollywoodTPI.Web/CineHollywoodTPI.Web.csproj` (ambos `net8.0`, SDK `Microsoft.NET.Sdk.Web`) |
| **ASP.NET Core Web API** (controllers) | La parte de ASP.NET para exponer endpoints HTTP que devuelven JSON | Todos los endpoints `/api/...` | `CineHollywoodTPI/Controllers/*.cs`, registrado con `AddControllers()` y `MapControllers()` en `CineHollywoodTPI/Program.cs` |
| **Entity Framework Core 8** + proveedor **SqlServer** | ORM: mapea tablas de SQL a clases de C# y permite consultar con LINQ | Todo el acceso a datos | Paquetes en `CineHollywoodTPI.Data/CineHollywoodTPI.Data.csproj`. Contexto: `CineHollywoodTPI.Data/Models/SistemaCineContext.cs` (+ `SistemaCineContext.Partial.cs`). Consultas: `CineHollywoodTPI.Data/Repositories/*.cs`. Registro: `CineHollywoodTPI/Program.cs:33` (`AddDbContext` + `UseSqlServer`) |
| **EF Core Tools / Design** | Herramientas de línea de comandos de EF (scaffold, migraciones) | Se usaron para **generar los modelos desde la base** (scaffold). No se usan en tiempo de ejecución | `Microsoft.EntityFrameworkCore.Tools` en `CineHollywoodTPI.Data.csproj`, `Microsoft.EntityFrameworkCore.Design` en `CineHollywoodTPI.API.csproj` |
| **SQL Server Express** | Motor de base de datos | La base `SistemaCineBdFinal` | Script: `SistemaCineBdFinal_actual.sql`. Cadena de conexión: `CineHollywoodTPI/appsettings.json` → `ConnectionStrings:DefaultConnection` |
| **Microsoft.AspNetCore.Authentication.JwtBearer** | Middleware que lee el header `Authorization: Bearer ...` y valida el JWT | Autenticación de cada request | Paquete en `CineHollywoodTPI.API.csproj`. Configuración en `CineHollywoodTPI/Program.cs:81-98` |
| **System.IdentityModel.Tokens.Jwt** | Clases para **crear** y firmar JWT (`JwtSecurityToken`, `JwtSecurityTokenHandler`) | Generar el token al hacer login | `CineHollywoodTPI/Services/TokenService.cs`. ⚠️ No está como `PackageReference` propio en el `.csproj`: llega como dependencia del paquete JwtBearer |
| **PasswordHasher de ASP.NET Identity** | Clase que hashea contraseñas con PBKDF2 + sal | Guardar y verificar contraseñas (nunca en texto plano) | Registro: `CineHollywoodTPI/Program.cs:68`. Uso: `CineHollywoodTPI/Services/AuthService.cs` (`HashPassword`, `VerifyHashedPassword`). Viene incluida en ASP.NET Core, no hay paquete aparte |
| **Swashbuckle (Swagger)** | Genera una página web para ver y probar los endpoints | `http://localhost:5191/swagger`, con botón **Authorize** para pegar el JWT | Paquete en `CineHollywoodTPI.API.csproj`. Config: `CineHollywoodTPI/Program.cs:105-127` y `UseSwagger()/UseSwaggerUI()` |
| **Inyección de dependencias** (de ASP.NET Core) | El framework crea los objetos y se los pasa por constructor | Controllers reciben servicios, servicios reciben repositorios | `CineHollywoodTPI/Program.cs`, sección 3 (`AddScoped<...>`) |
| **Options pattern** (`IOptions<T>`) | Leer una sección de `appsettings.json` como una clase | Parámetros de reglas de negocio (descuento, edad, topes) | `CineHollywoodTPI/Services/ReglasNegocioOptions.cs`, registrado con `Configure<ReglasNegocioOptions>` en `Program.cs` |
| **IHttpClientFactory** | Fábrica de `HttpClient` configurados | Descargar pósters desde un link | `AddHttpClient("posters", ...)` en `Program.cs`; uso en `CineHollywoodTPI/Services/PosterService.cs` |
| **CORS** (de ASP.NET Core) | Permiso para que una página de **otro origen** (otro puerto) llame a la API | El sitio (5020) llama a la API (5191) | `CineHollywoodTPI/Program.cs:20` (`AddCors`, política `PermitirTodo`) y `:170` (`UseCors`) |
| **Static files** (de ASP.NET Core) | Servir archivos tal cual desde `wwwroot` | API: sirve los pósters (`/posters/x.jpg`). Web: sirve **todo el frontend** | `CineHollywoodTPI/Program.cs:167`, `CineHollywoodTPI.Web/Program.cs:13-14` (`UseDefaultFiles` + `UseStaticFiles`) |

### 2.2 Frontend

Ninguna librería del front se instala: **todas se cargan por CDN** (un servidor público) con una etiqueta `<link>` o `<script>` en cada HTML. Por eso el sitio necesita internet.

| Librería | Versión | Qué es | Para qué la usamos acá | Dónde se carga | Dónde se consume |
|---|---|---|---|---|---|
| **Bootstrap** (CSS) | 5.3.3 | Framework de estilos: trae clases listas (`container`, `row`, `btn`, `card`, `modal`…) | Toda la maquetación responsiva y los componentes visuales | `<link>` en el `<head>` de los 6 HTML (ej. `index.html:11`) | Clases en todos los HTML y en el HTML que arman los JS |
| **Bootstrap** (JS, "bundle") | 5.3.3 | La parte de Bootstrap que da **comportamiento**: abrir modales, colapsar el menú, offcanvas | Modales de login/registro/formularios, menú ☰ en celular, menú lateral del panel | `<script>` al final del `<body>` (ej. `index.html:52`) | `api.js:210-215` (`bootstrap.Modal`), `admin.js` (`bootstrap.Offcanvas`), atributos `data-bs-toggle` / `data-bs-dismiss` en `main.js` y `admin.html` |
| **SweetAlert2** | 11.14.5 | Librería de alertas lindas (reemplaza a `alert()` / `confirm()`) | Toasts, alertas y confirmaciones "¿Seguro?" | `<script>` en los 6 HTML (ej. `index.html:53`) | Solo se llama desde `api.js` (`toast`, `alerta`, `confirmar`, líneas 151-205). El resto del código usa esas tres funciones |
| **Chart.js** | 4.4.1 | Librería para dibujar gráficos en un `<canvas>` | Los 2 gráficos de barras del dashboard | `<script>` solo en `admin.html:552` | `admin.js:329` (`dibujarGraficoTop`) y `admin.js:366` (`dibujarGraficoMensual`) |
| **Feather Icons** | 4.29.2 | Set de íconos. Se escribe `<i data-feather="film"></i>` y la librería lo reemplaza por un SVG | Íconos del panel de administración | `<script>` solo en `admin.html:551` (desde `unpkg.com`) | `admin.js:23` (`refrescarIconos` → `feather.replace()`); los `<i data-feather>` están en `admin.html` y en el HTML que arma `admin.js` |
| **Google Fonts** (Inter y Outfit) | — | Tipografías servidas por Google | Inter para el texto, Outfit para los títulos | `<link>` en el `<head>` (ej. `index.html:8-10`) | `css/style.css`: variables `--bs-body-font-family` y `--font-title` |

> En las páginas públicas los íconos (logo, sol/luna, usuario) **no** son de Feather: son SVG escritos a mano en el objeto `ICONOS` de `main.js:7-12`.

### 2.3 Herramientas

| Herramienta | Para qué | Dónde |
|---|---|---|
| **Postman** | Probar la API con requests guardadas y tests automáticos | `postman/CineHollywood.postman_collection.json` (ver README §12) |
| **Visual Studio 2022** | IDE; levanta los dos proyectos a la vez | `CineHollywoodTPI.slnx`, `CineHollywoodTPI.slnLaunch.user` |
| **SQL Server Management Studio** | Correr el script de la base | `SistemaCineBdFinal_actual.sql` |

---

## 3. JWT completo

### 3.1 Qué es

Un **JWT** (*JSON Web Token*) es un texto que el servidor le da al usuario después de loguearse y que funciona como una **credencial firmada**: "el portador de esto es el usuario 7, rol Cliente, y vale hasta tal hora". En cada pedido siguiente, el navegador lo manda y la API **no necesita consultar la base ni guardar sesiones** para saber quién es: le alcanza con verificar la firma.

Es **stateless** (sin estado): la API no guarda en ningún lado "qué tokens entregó". Toda la información viaja dentro del token.

### 3.2 Sus 3 partes, con un token como el nuestro

Un JWT son tres textos en **Base64URL** separados por puntos: `HEADER.PAYLOAD.FIRMA`.

Este es un token de ejemplo **armado con la misma estructura, los mismos claims y la misma clave** que usa `TokenService` (los datos del usuario son inventados: un cliente "Juan Perez" con `idUsuario` 7 e `idCliente` 12):

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
.
eyJzdWIiOiI3IiwianRpIjoiM2YyYjhjMWUtNWE0ZC00ZTBmLTliN2EtMWMyZDNlNGY1YTZiIiwidW5pcXVlX25hbWUiOiJKdWFuIFBlcmV6IiwiZW1haWwiOiJqdWFuQG1haWwuY29tIiwicm9sZSI6IkNsaWVudGUiLCJpZFVzdWFyaW8iOiI3IiwiaWRDbGllbnRlIjoiMTIiLCJleHAiOjE3OTUxMDE2MDAsImlzcyI6IkNpbmVIb2xseXdvb2RUUEkuQVBJIiwiYXVkIjoiQ2luZUhvbGx5d29vZFRQSS5XZWIifQ
.
hBF9xA_06gzhGOXsNfNWkySATNNQcT4385WGKnTrtAI
```

(En la realidad va todo en una sola línea; acá se cortó para que se vean las partes.)

**1) Header** — decodificado:

```json
{ "alg": "HS256", "typ": "JWT" }
```

Dice con qué algoritmo está firmado. `HS256` = HMAC-SHA256, que sale de `SecurityAlgorithms.HmacSha256` en `TokenService.cs:47`.

**2) Payload** (los *claims*, o sea, los datos) — decodificado:

```json
{
  "sub": "7",
  "jti": "3f2b8c1e-5a4d-4e0f-9b7a-1c2d3e4f5a6b",
  "unique_name": "Juan Perez",
  "email": "juan@mail.com",
  "role": "Cliente",
  "idUsuario": "7",
  "idCliente": "12",
  "exp": 1795101600,
  "iss": "CineHollywoodTPI.API",
  "aud": "CineHollywoodTPI.Web"
}
```

De dónde sale cada claim en `CineHollywoodTPI/Services/TokenService.cs`:

| Claim en el token | Línea | Qué es |
|---|---|---|
| `sub` | 35 | *Subject*: el id del usuario (`JwtRegisteredClaimNames.Sub`) |
| `jti` | 36 | Id único del token (un GUID nuevo cada vez) |    
            
| `unique_name` | 37 | Nombre y apellido. En C# se escribe `ClaimTypes.Name`; la librería lo guarda con el nombre corto `unique_name` |
| `email` | 38 | `ClaimTypes.Email` → `email` |
| `role` | 39 | `"Administrador"` o `"Cliente"` (`ClaimTypes.Role` → `role`). **Es el que usa `[Authorize(Roles = ...)]`** |
| `idUsuario` | 40 | Claim propio (`ClaimsCine.IdUsuario`, definido en la línea 13) |
| `idCliente` | 43-44 | Claim propio. **Solo existe si el usuario es un cliente**: el admin inicial no tiene `CLIENTE` asociado, así que su token no lo trae |
| `exp` | 48, 54 | Vencimiento, en segundos desde 1970 (UTC). `1795101600` = 19/11/2026 15:20 UTC en este ejemplo |
| `iss` | 51 | *Issuer*: quién emitió el token (`Jwt:Issuer` de `appsettings.json`) |
| `aud` | 52 | *Audience*: para quién es (`Jwt:Audience`) |

> ⚠️ El **orden** exacto de los claims y los nombres cortos (`unique_name`, `role`, `email`) los decide la librería `JwtSecurityTokenHandler` al serializar; el ejemplo reproduce su comportamiento por defecto. Si querés ver un token real, logueate, abrí las herramientas del navegador (F12) → *Application* → *Local Storage* → `cine_sesion`, copiá el `token` y pegalo en [jwt.io](https://jwt.io) (es una herramienta pública: no lo hagas con tokens de un sistema real).

**Importante:** el payload **no está encriptado**, solo codificado en Base64. Cualquiera que tenga el token puede leerlo. Por eso **nunca** se pone la contraseña ni datos sensibles adentro.

**3) Firma**

```
firma = HMAC-SHA256( base64url(header) + "." + base64url(payload),  clave secreta )
```

La clave es `Jwt:Key` de `CineHollywoodTPI/appsettings.json`. Si alguien modifica el payload (por ejemplo, cambia `"role":"Cliente"` por `"Administrador"`), la firma deja de coincidir y la API rechaza el token con **401**. Solo quien tiene la clave puede fabricar tokens válidos, y la clave vive solamente en el servidor.

### 3.3 Cómo se genera

```
POST /api/auth/login  { "email": "...", "password": "..." }
        │
        ▼
AuthController.Login                         CineHollywoodTPI/Controllers/AuthController.cs:24
        │
        ▼
AuthService.LoginAsync                       CineHollywoodTPI/Services/AuthService.cs:32
  ├─ valida que vengan email y contraseña     → si no, NegocioException (400)
  ├─ UsuarioRepository.GetByEmailAsync        (trae USUARIO + ROL + PERSONA + CLIENTE con Include)
  ├─ PasswordHasher.VerifyHashedPassword      → si falla, NoAutorizadoException (401)
  ├─ usuario activo?                          → si no, NoAutorizadoException (401)
  └─ TokenService.GenerarToken(usuario)       CineHollywoodTPI/Services/TokenService.cs:26
        ├─ arma la lista de claims
        ├─ crea la clave simétrica con Jwt:Key y la firma HS256
        ├─ calcula expira = ahora (UTC) + Jwt:ExpiraMinutos (120)
        ├─ new JwtSecurityToken(issuer, audience, claims, expires, firma)
        └─ devuelve LoginRespuestaDTO
```

La respuesta (`LoginRespuestaDTO`, en `CineHollywoodTPI.Data/DTOs/AuthDTOs.cs`) llega al navegador como JSON. ASP.NET pasa los nombres de propiedad a *camelCase* automáticamente (`Token` → `token`):

```json
{
  "token": "eyJhbGciOi...",
  "expira": "2026-11-19T15:20:00Z",
  "idUsuario": 7,
  "email": "juan@mail.com",
  "nombre": "Juan Perez",
  "rol": "Cliente",
  "idCliente": 12
}
```

Fijate que `nombre`, `rol` e `idCliente` vienen **repetidos afuera del token**. Es para que el front no tenga que decodificar el JWT: los lee directo de este objeto.

El **registro** (`POST /api/auth/registro`, `AuthService.RegistrarAsync`) termina igual: crea el usuario y llama a `GenerarToken`, así que el usuario queda logueado sin tener que volver a escribir la contraseña.

### 3.4 Dónde se valida

En `CineHollywoodTPI/Program.cs:81-98`:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,            // iss == "CineHollywoodTPI.API"
            ValidateAudience = true,          // aud == "CineHollywoodTPI.Web"
            ValidateLifetime = true,          // exp no pasó
            ValidateIssuerSigningKey = true,  // la firma coincide con Jwt:Key
            ...
            RoleClaimType = ClaimTypes.Role,  // de dónde sale el rol para [Authorize(Roles=...)]
            NameClaimType = ClaimTypes.Name,  // de dónde sale User.Identity.Name
            ClockSkew = TimeSpan.FromMinutes(1) // tolerancia de 1 minuto por diferencias de reloj
        };
    });
```

Y en el pipeline (`Program.cs:175-176`), en este orden:

```csharp
app.UseAuthentication();  // lee el header, valida el token y arma HttpContext.User
app.UseAuthorization();   // revisa [Authorize] del endpoint contra ese User
```

Qué pasa en cada request:

1. `UseAuthentication` busca el header `Authorization: Bearer <token>`. Si está y es válido, carga los claims en `User` (un `ClaimsPrincipal`). Si no hay header, el usuario queda **anónimo** (no es error todavía).
2. `UseAuthorization` mira el atributo del endpoint:
   - Sin `[Authorize]` (ej. `GET /api/pelicula`) → pasa igual, aunque sea anónimo.
   - `[Authorize]` (ej. todo `VentaController`, línea 12) → anónimo o token inválido/vencido → **401**.
   - `[Authorize(Roles = "Administrador")]` (ej. todo `ReporteController`, línea 10) → logueado pero con otro rol → **403**.
3. Si pasa, el controller puede leer los claims. Por ejemplo `VentaController` arma un `UsuarioActual` con `UsuarioActual.Desde(User)` (`CineHollywoodTPI/Services/UsuarioActual.cs`), que saca `idUsuario`, si es admin (`User.IsInRole("Administrador")`) e `idCliente`. Así el servicio sabe **a nombre de quién** registrar la venta sin confiar en lo que mande el front.

### 3.5 Dónde se guarda en el cliente

En el **`localStorage`** del navegador, bajo la clave `cine_sesion`. `localStorage` es un pequeño almacenamiento clave → texto que cada navegador tiene por sitio y que **sobrevive a cerrar la pestaña**.

Todo eso lo maneja el objeto `Auth` en `CineHollywoodTPI.Web/wwwroot/js/api.js:13-37`:

```js
const Auth = {
    CLAVE: 'cine_sesion',

    get sesion() {                                   // lee y valida la sesión
        const s = JSON.parse(localStorage.getItem(this.CLAVE));
        if (!s || !s.token) return null;
        if (s.expira && new Date(s.expira) <= new Date()) {   // ¿venció?
            localStorage.removeItem(this.CLAVE);              // la borra
            return null;
        }
        return s;
    },
    guardar(respuestaLogin) { localStorage.setItem(this.CLAVE, JSON.stringify(respuestaLogin)); },
    cerrar() { localStorage.removeItem(this.CLAVE); },
    get token() { return this.sesion?.token ?? null; },
    estaLogueado() { return !!this.sesion; },
    esAdmin() { return this.sesion?.rol === 'Administrador'; }
};
```

Se guarda **el objeto entero** que devolvió el login (token + expira + nombre + rol + idCliente). `Auth.guardar` se llama desde `despuesDeLoguear` en `main.js:285-286`.

### 3.6 Cómo viaja en cada request

Toda llamada a la API pasa por la función `api()` (`api.js:43`), que agrega el header si hay sesión:

```js
if (Auth.token) headers['Authorization'] = `Bearer ${Auth.token}`;   // api.js:47
```

Un pedido HTTP queda así:

```http
GET /api/venta HTTP/1.1
Host: localhost:5191
Accept: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI3Ii...
```

Como **todas** las llamadas usan `api()`, ningún otro archivo tiene que acordarse del token.

### 3.7 Expiración

Se controla en varios lugares:

| Dónde | Qué hace |
|---|---|
| **API** — `TokenService.cs:48` | Pone `exp` = ahora + `Jwt:ExpiraMinutos` (120 minutos, en `appsettings.json`) |
| **API** — `ValidateLifetime = true` en `Program.cs` | Si el token venció (con 1 minuto de tolerancia), lo rechaza → **401** |
| **Front** — getter `Auth.sesion` en `api.js:16-29` | Antes de usar la sesión compara `expira` con la hora actual; si venció, la borra. Así ni siquiera se manda un token vencido |
| **Front** — `verificarSesion()` en `api.js:276-285` | **Al cargar cada página**, si hay sesión guardada, llama a `GET /api/auth/yo` para que **la API** confirme que el token sigue valiendo (ver abajo) |
| **Front** — `api.js:66-69` | Si cualquier pedido recibe un **401** estando logueado, cierra la sesión y lanza el error *"Tu sesión expiró. Volvé a iniciar sesión."* |
| **Front** — `admin.html:7-16` | Un `<script>` en el `<head>` revisa la sesión **antes de dibujar el panel** y, si no hay sesión de admin o está vencida, redirige a `index.html?login=1` |

No hay *refresh token*: cuando vence, hay que loguearse de nuevo.

#### Verificación con `GET /api/auth/yo` al cargar la página

El chequeo de `Auth.sesion` solo compara la fecha `expira` **guardada en el navegador** con el reloj **de la PC**. Hay casos en que eso dice "válido" pero la API va a rechazar el token:

- el reloj de la PC está atrasado (el navegador cree que no venció, el servidor sí);
- en el servidor cambió `Jwt:Key`, `Jwt:Issuer` o `Jwt:Audience` (los tokens viejos ya no pasan la validación);
- el contenido de `localStorage` se modificó o se rompió.

Por eso, en todas las páginas, `api.js` registra `verificarSesion` en `DOMContentLoaded`:

```js
async function verificarSesion() {                        // api.js:276
    if (!Auth.estaLogueado()) return;                     // sin sesión no hay nada que verificar
    try {
        await api('/auth/yo');                            // [Authorize]: 200 si el token vale, 401 si no
    } catch {
        // Otros errores (ej. la API apagada) no cierran la sesión: solo el 401
        if (!Auth.estaLogueado()) document.dispatchEvent(new Event('sesion-expirada'));
    }
}
document.addEventListener('DOMContentLoaded', verificarSesion);
```

- Si la API responde **401**, la propia función `api()` ya borró la sesión (`api.js:66-69`). `verificarSesion` lo detecta porque `Auth.estaLogueado()` pasó a `false`, y dispara el evento propio **`sesion-expirada`**.
- Si falla por otro motivo (API apagada, error 500), la sesión **se mantiene**: no tiene sentido desloguear a alguien porque el servidor no respondió.
- Quién escucha `sesion-expirada`:
  - `main.js:347` (páginas públicas): muestra un toast, redibuja la navbar con "Iniciar sesión / Registrarse" y dispara `sesion-cambiada` (así, por ejemplo, `reserva.js` cambia el botón a "Iniciar sesión y comprar").
  - `admin.js:1135` (panel): muestra un toast y, a los 1,5 segundos, redirige a `index.html?login=1`.

El endpoint es el que ya existía en `AuthController` (`[Authorize] [HttpGet("yo")]`): devuelve los datos del token y no consulta la base, así que es un pedido liviano.

### 3.8 Logout: por qué solo borra el token del navegador

**Qué hace hoy.** El botón "Salir" (`main.js:100-105` en las páginas públicas, `adminLogout` en `admin.js:58-61`) llama a `Auth.cerrar()`, que hace `localStorage.removeItem('cine_sesion')`. **No hay ningún pedido a la API.**

**Por qué alcanza (casi).** Sin el token guardado, el navegador deja de mandar el header `Authorization`, así que para ese navegador la sesión terminó.

**Por qué no se invalida en el servidor.** El JWT es **stateless**: para aceptar un token, la API solo verifica la firma, el emisor, la audiencia y el vencimiento (ver [3.4](#34-dónde-se-valida)). **No guarda en ningún lado qué tokens entregó**, así que no tiene una lista contra la cual decir "este token ya no vale". La consecuencia: si alguien hubiera copiado el token antes del logout (por ejemplo, con un ataque XSS o desde las herramientas del navegador de una PC compartida), **ese token seguiría funcionando hasta su `exp`**, hasta 120 minutos.

Es el costo de la ventaja principal de JWT: la API no consulta la base para saber quién sos en cada request.

**Qué haría falta para invalidarlo en el servidor.** Hay que **agregar estado** del lado de la API. Las opciones habituales:

| Opción | Cómo funcionaría en este proyecto | Costo |
|---|---|---|
| **Lista negra por `jti`** | El token ya trae un `jti` único (`TokenService.cs:36`). Un endpoint `POST /api/auth/logout` con `[Authorize]` guardaría ese `jti` y su `exp` en una tabla nueva (o en una caché como `IMemoryCache`/Redis). En `Program.cs`, dentro de `AddJwtBearer`, se configuraría `options.Events = new JwtBearerEvents { OnTokenValidated = ... }` para buscar el `jti` y llamar a `context.Fail(...)` si está revocado → 401. Los registros vencidos se pueden borrar periódicamente | Una consulta por request. Con caché en memoria, la lista se pierde al reiniciar la API y no se comparte entre varios servidores |
| **Versión de token por usuario** | Columna nueva en `USUARIO` (ej. `version_token`) que se copia como claim al generar el token. En el logout se incrementa; en `OnTokenValidated` se compara el claim con la columna y, si no coincide, se rechaza | Una consulta por request. Invalida **todos** los tokens de ese usuario (cierra la sesión en todos sus dispositivos) |
| **Access token corto + refresh token** | El JWT dura pocos minutos (ej. 10). Además se entrega un *refresh token* que se guarda en una tabla y sirve para pedir un JWT nuevo (`POST /api/auth/refresh`). El logout borra el refresh token de la base | No agrega consultas por request, pero el JWT que ya estaba emitido sigue valiendo hasta que vence (por eso se hace corto). Requiere más código en el front: renovar el token antes de que venza o al recibir un 401 |

Cualquiera de estas opciones también implicaría que el botón "Salir" del front llame a la API (`await api('/auth/logout', { method: 'POST' })`) antes de `Auth.cerrar()`.

**Nada de esto está implementado** en el proyecto: el logout queda como un cierre de sesión local, y la verificación con `/api/auth/yo` solo sirve para detectar tokens que la API **ya** rechaza (vencidos o con firma inválida), no para revocarlos.

### 3.9 Diagrama: login → request protegida

```
 Navegador (main.js / api.js)                      API (5191)                          SQL Server
 ─────────────────────────────                     ──────────                          ──────────
 Usuario completa el modal de login
 y aprieta "Ingresar"
        │
        │ onLogin()  (main.js:227)
        │ api('/auth/login', {method:'POST', body:{email,password}})
        │──── POST /api/auth/login ──────────────────▶ AuthController.Login
        │     Content-Type: application/json            └▶ AuthService.LoginAsync
        │     { "email": "...", "password": "..." }          ├▶ UsuarioRepository ───── SELECT USUARIO ──▶
        │                                                    │                   ◀──── fila + rol ──────
        │                                                    ├▶ PasswordHasher.Verify...
        │                                                    └▶ TokenService.GenerarToken (firma HS256)
        │◀─── 200 OK { token, expira, nombre, rol, idCliente } ─
        │
        │ despuesDeLoguear()  (main.js:285)
        │   Auth.guardar(resp) → localStorage["cine_sesion"]
        │   cierra el modal, muestra "¡Hola, Juan!", redibuja la navbar
        │
        ⋮   (más tarde) el usuario abre "Mis compras"
        │
        │ abrirMisCompras()  (main.js:304)
        │ api('/venta')  → agrega Authorization: Bearer <token>
        │──── GET /api/venta ────────────────────────▶ UseAuthentication
        │     Authorization: Bearer eyJ...                 ├ ¿firma OK? ¿iss/aud OK? ¿no venció?
        │                                                  │   NO → 401 ──────────────┐
        │                                                  └ SÍ → User = claims       │
        │                                              UseAuthorization               │
        │                                                  └ [Authorize] → OK         │
        │                                              VentaController                │
        │                                                  └ UsuarioActual.Desde(User)│
        │                                                    (idCliente = 12)         │
        │                                                  └▶ VentaService ─▶ VentaRepository ── SELECT ventas del cliente 12 ─▶
        │◀─── 200 OK [ {idVenta, fechaHora, total, ...}, ... ] ◀──────────────────────────────────────────
        │                                                                             │
        │ arma la tabla en el modal                                                   │
        │◀────────────────────────────────────────────────────────────────────────────┘
        │ si fue 401: api.js borra la sesión y lanza "Tu sesión expiró..."
```

### 3.10 Seguridad: qué conviene saber

- **La protección real está en la API.** Que `admin.html` redirija si no sos admin es solo para la experiencia de usuario; aunque alguien edite el `localStorage` y ponga `"rol":"Administrador"`, el **token** sigue diciendo `Cliente` (y no lo puede modificar sin romper la firma), así que la API le responde 403.
- **`localStorage` es legible por cualquier JavaScript de la página.** Si un atacante lograra inyectar código JS (ataque XSS), podría robar el token. Para reducir ese riesgo, el código escapa todos los datos que vienen de la base antes de meterlos en el HTML con la función `esc()` (`api.js:106`).
- La clave `Jwt:Key` está en `appsettings.json` porque es un TP; en producción iría en variables de entorno o *User Secrets* (lo aclara también el README).

---

## 4. Frontend desde cero

### 4.1 Cómo llega una página al navegador

1. Escribís `http://localhost:5020` en el navegador.
2. El proyecto `CineHollywoodTPI.Web` tiene solo dos líneas importantes (`CineHollywoodTPI.Web/Program.cs:13-14`):
   ```csharp
   app.UseDefaultFiles();  // si piden "/", responder con index.html
   app.UseStaticFiles();   // entregar cualquier archivo de wwwroot tal cual
   ```
   Entonces devuelve `wwwroot/index.html`.
3. El navegador lee ese HTML de arriba hacia abajo. Cada vez que encuentra un `<link href="...">` o `<script src="...">`, **hace otro pedido** para traer ese archivo (a nuestro servidor o al CDN).
4. Con el HTML arma la estructura, con el CSS la pinta, y ejecuta el JS. El JS llama a la API y completa la página.

Podés ver todo esto en vivo con **F12 → pestaña Network (Red)**: aparece `index.html`, `bootstrap.min.css`, `style.css`, `api.js`… y después los `fetch` a `localhost:5191/api/...`.

Las tres piezas, con una analogía:

| Lenguaje | Analogía | Responde a… |
|---|---|---|
| **HTML** | El esqueleto / los ladrillos | ¿**Qué** hay en la página? (un título, un botón, una lista) |
| **CSS** | La pintura y la decoración | ¿**Cómo se ve**? (color, tamaño, posición) |
| **JavaScript** | La electricidad / el cerebro | ¿**Qué hace**? (al hacer clic, traer datos, mostrar/ocultar) |

### 4.2 HTML

#### Qué es

HTML (*HyperText Markup Language*) **no es un lenguaje de programación**: no tiene variables ni `if`. Es un lenguaje de **marcado**: describe el contenido con **etiquetas**.

```html
<a href="cartelera.html" class="btn btn-primary">Ver cartelera</a>
│  └────── atributos ──────────────────────┘  └ contenido ┘ └ cierre
└ etiqueta de apertura (a = "anchor", un link)
```

- Una **etiqueta** abre (`<a>`) y cierra (`</a>`). Algunas no tienen contenido y no se cierran (`<meta>`, `<link>`, `<input>`, `<br>`).
- Los **atributos** dan datos extra: `href` (a dónde lleva el link), `class` (para CSS), `id` (nombre único para encontrar el elemento desde JS).
- Las etiquetas se **anidan**, formando un árbol: un `<main>` contiene `<section>`, que contiene `<div>`, que contiene un `<h1>`…
- `<!-- esto es un comentario -->`

#### La estructura de una página

```html
<!DOCTYPE html>          ← "esto es HTML moderno (HTML5)"
<html>                   ← la raíz: todo va adentro
  <head>                 ← información SOBRE la página (no se ve): título, CSS, configuración
  </head>
  <body>                 ← lo que SE VE
    <header>  ← cabecera: logo y menú
      <nav>   ← bloque de navegación (links a otras páginas)
    <main>    ← el contenido principal (debería haber uno solo por página)
    <footer>  ← pie de página
  </body>
</html>
```

`<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<aside>` y `<footer>` se llaman **etiquetas semánticas**: visualmente son iguales a un `<div>` (una caja), pero le dicen al navegador, a Google y a los lectores de pantalla para ciegos **qué rol cumple** cada parte. En el proyecto se usan todas: `<header>` y `<nav>` en la navbar, `<main>` en cada página, `<section>` en los bloques del inicio y en los pasos de la compra, `<article>` en cada tarjeta de película (`cartelera.js`), `<aside>` en el menú lateral del panel (`admin.html`), `<footer>` al pie.

`<div>` y `<span>` son cajas **genéricas** sin significado: `<div>` ocupa todo el ancho (es de "bloque"), `<span>` va dentro de una línea de texto.

#### `index.html` línea por línea

Archivo: `CineHollywoodTPI.Web/wwwroot/index.html` (58 líneas).

```html
1  <!DOCTYPE html>
```
Le dice al navegador que use el modo HTML5 estándar.

```html
2  <html lang="es" data-bs-theme="dark">
```
Raíz del documento. `lang="es"`: la página está en español (ayuda a lectores de pantalla y traductores). `data-bs-theme="dark"`: atributo que lee **Bootstrap 5.3** para elegir colores claros u oscuros. Arranca en oscuro, pero `api.js:233` lo cambia apenas carga según lo que eligió el usuario la última vez.

```html
3  <head>
4      <meta charset="UTF-8">
```
Inicio de la "info sobre la página". `charset="UTF-8"`: cómo están codificados los caracteres; sin esto las tildes y la ñ se verían como `Ã±`.

```html
5      <meta name="viewport" content="width=device-width, initial-scale=1.0">
```
**Clave para celulares.** Sin esto, el celular simula una pantalla de computadora (~980 px) y achica todo. Con esto, el ancho de la página es el ancho real del dispositivo y el diseño responsivo de Bootstrap funciona.

```html
6      <title>Cines Hollywood - La mejor experiencia</title>
7      <meta name="description" content="Cines Hollywood: cartelera, horarios y compra de entradas online.">
```
`<title>`: el texto de la pestaña del navegador. `description`: el resumen que muestra Google en los resultados (solo está en `index.html`).

```html
8      <link rel="preconnect" href="https://fonts.googleapis.com">
9      <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
```
Optimización: le dice al navegador "vas a necesitar estos servidores, empezá a conectarte ya", así las fuentes llegan antes.

```html
10     <link href="https://fonts.googleapis.com/css2?family=Inter:...&family=Outfit:...&display=swap" rel="stylesheet">
```
Descarga las tipografías **Inter** y **Outfit** de Google Fonts. `display=swap`: mientras carga, muestra el texto con una fuente del sistema (no deja la pantalla en blanco).

```html
11     <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.3/css/bootstrap.min.css" rel="stylesheet">
12     <link rel="stylesheet" href="css/style.css">
13 </head>
```
Las hojas de estilo. **El orden importa**: primero Bootstrap, después la nuestra. Cuando dos reglas igual de específicas chocan, gana la última, así que `style.css` puede **pisar** lo que define Bootstrap (por ejemplo, el color amarillo de `btn-primary`). `css/style.css` es una ruta relativa: el navegador la pide a `http://localhost:5020/css/style.css`.

```html
14 <body>
15     <!-- La navbar la dibuja main.js. data-transparente: transparente sobre el hero -->
16     <header id="navbarCine" data-transparente></header>
```
Empieza lo visible. El `<header>` está **vacío a propósito**: `main.js` (`renderNavbar`, línea 41) lo busca por su `id` y le mete adentro el HTML de la barra. Así la barra se escribe una sola vez para las 6 páginas. `data-transparente` es un atributo inventado por nosotros (los `data-*` están permitidos para guardar datos propios): `main.js:46` lo detecta y hace la barra transparente mientras estás arriba de todo.

```html
18     <main>
19         <section class="hero">
20             <div class="container">
```
Contenido principal. La sección "hero" (el bloque grande de bienvenida) usa la clase **nuestra** `.hero` (`style.css:164`: fondo con degradés, alto mínimo 82% de la pantalla). `container` es de **Bootstrap**: centra el contenido y le pone un ancho máximo según la pantalla.

```html
21                 <span class="badge rounded-pill hero-badge px-3 py-2 mb-3">Estrenos 2026</span>
```
Una etiqueta tipo "pastilla". `badge`, `rounded-pill`, `px-3` (padding horizontal), `py-2` (vertical), `mb-3` (margen abajo) son de Bootstrap; `hero-badge` es nuestra (`style.css:185`, color amarillo translúcido).

```html
22                 <h1 class="display-3 fw-bold mb-3">Viví la mejor<br>experiencia del cine</h1>
23                 <p class="lead mb-4">Elegí tu película, tu función y tu butaca...</p>
```
`<h1>`: título principal (hay niveles `h1`…`h6`). `display-3`: tamaño enorme de Bootstrap; `fw-bold`: negrita. `<br>`: salto de línea. `<p>`: párrafo; `lead` lo hace un poco más grande.

```html
24                 <div class="d-flex flex-wrap gap-3">
25                     <a href="cartelera.html" class="btn btn-primary btn-lg rounded-pill px-4">Ver cartelera</a>
26                     <a href="promociones.html" class="btn btn-outline-light btn-lg rounded-pill px-4">Ver promociones</a>
27                 </div>
```
Dos links con forma de botón (`btn`). `d-flex flex-wrap gap-3` = "poné los hijos en fila, que bajen de línea si no entran, con espacio entre ellos" (flexbox, ver CSS). Al hacer clic, el navegador va a `cartelera.html`: es navegación normal, sin JS.

```html
31         <section class="py-5">
32             <div class="container">
33                 <div class="d-flex justify-content-between align-items-center mb-4">
34                     <h2 class="fw-bold m-0">En cartelera</h2>
35                     <div class="d-flex gap-2" id="carouselControles">
36                         <button class="btn btn-outline-secondary rounded-circle" id="prevSlide" aria-label="Anterior">&#8249;</button>
37                         <button class="btn btn-outline-secondary rounded-circle" id="nextSlide" aria-label="Siguiente">&#8250;</button>
38                     </div>
39                 </div>
40                 <div class="carousel-3d" id="carouselTrack"></div>
```
Segunda sección: título "En cartelera" a la izquierda y dos botones redondos a la derecha (`justify-content-between` los separa a los extremos). `&#8249;` y `&#8250;` son los caracteres ‹ y ›. `aria-label` es el texto que lee un lector de pantalla (porque el botón solo tiene un símbolo). Los `id` (`prevSlide`, `nextSlide`, `carouselTrack`) existen para que `index.js` los encuentre. El `<div id="carouselTrack">` está **vacío**: `index.js` lo llena con los pósters que trae de la API.

```html
45     <footer class="border-top py-4 text-body-secondary small">
46         <div class="container d-flex flex-wrap justify-content-between gap-2">
47             <span>© 2026 Cines Hollywood - TPI</span>
48             <span>Horarios y precios sujetos a cambios.</span>
```
Pie de página: línea arriba (`border-top`), texto gris (`text-body-secondary`) y chico (`small`).

```html
52     <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.3/js/bootstrap.bundle.min.js"></script>
53     <script src="https://cdnjs.cloudflare.com/ajax/libs/limonte-sweetalert2/11.14.5/sweetalert2.all.min.js"></script>
54     <script src="js/api.js"></script>
55     <script src="js/main.js"></script>
56     <script src="js/index.js"></script>
57 </body>
58 </html>
```
Los scripts van **al final del `<body>`** para que, cuando se ejecuten, el HTML de arriba ya exista. Se ejecutan **en orden**, y el orden es obligatorio:

1. Bootstrap JS y SweetAlert2 → crean las variables globales `bootstrap` y `Swal`.
2. `api.js` → define `api()`, `Auth`, `esc()`, `toast()`… (usa `bootstrap` y `Swal`).
3. `main.js` → dibuja la navbar y el login (usa lo de `api.js`).
4. `index.js` → lógica propia de esta página (usa todo lo anterior).

Si cambiaras el orden, por ejemplo `index.js` antes que `api.js`, fallaría con *"api is not defined"*.

Las demás páginas públicas siguen **el mismo molde**: mismo `<head>`, `<header id="navbarCine">`, un `<main>` propio, mismo footer y los mismos scripts más el de la página (`cartelera.js`, `reserva.js` + `butacas.js`). `estrenos.html` y `promociones.html` no tienen archivo JS propio: su código está **dentro** de un `<script>` en el mismo HTML (ej. `estrenos.html:31-56`).

#### Formularios

Los formularios usan `<form>`, `<label for="...">` (el texto del campo; `for` apunta al `id` del input, y al hacer clic en el label se enfoca el campo), `<input>` con distintos `type` (`email`, `password`, `date`, `number`, `file`), `<select>` con `<option>` y `<button type="submit">`. Ejemplo real: el modal de login que inyecta `main.js:127-147`.

`required` y `minlength` son validaciones del navegador, pero el formulario tiene `novalidate` (las desactiva) porque las validaciones las hace nuestro JS (`onRegistro`, `main.js:259-269`) y, sobre todo, **las vuelve a hacer la API**.

### 4.3 CSS

#### Qué es y cómo se aplica

CSS (*Cascading Style Sheets*) define **cómo se ve** cada elemento. Una regla tiene un **selector** (a quién se aplica) y **declaraciones** `propiedad: valor;`:

```css
.seat {                         /* selector: todos los elementos con class="seat" */
    width: 34px;                /* propiedad: valor; */
    height: 30px;
    background: var(--seat-free);
}
```

Hay tres formas de aplicar CSS, y el proyecto usa las tres:

| Forma | Ejemplo en el proyecto |
|---|---|
| **Archivo externo** con `<link>` (la recomendada) | `<link rel="stylesheet" href="css/style.css">` en todos los HTML |
| **Bloque `<style>`** dentro del HTML | No se usa |
| **Atributo `style=""`** en el elemento (inline) | `reserva.html:16`: `<main class="container page" style="max-width: 900px">`. También el JS lo usa para el carrusel: `index.js:50` (`slide.style.transform = ...`) |

"Cascada" significa que **varias reglas pueden aplicarse al mismo elemento** y el navegador decide cuál gana: la más **específica** (un `#id` le gana a una `.clase`, que le gana a una etiqueta), y entre iguales, **la última** que aparece. `!important` fuerza a ganar (se usa poco, ej. `style.css:185`).

#### Selectores

| Selector | Significa | Ejemplo real |
|---|---|---|
| `etiqueta` | Todos los elementos de ese tipo | `h1, h2, h3, h4, h5 { font-family: var(--font-title); }` (`style.css:58`): todos los títulos con la fuente Outfit |
| `.clase` | Todos los que tengan esa clase | `.poster { ... }` (`style.css:188`) |
| `#id` | El único elemento con ese id | **En nuestros CSS no hay ninguno.** Los `id` del HTML se usan solo desde JS (`getElementById`). Es una buena práctica: el estilo va por clase |
| `A B` (descendiente) | B que esté **dentro** de A | `.navbar-transparente .btn-outline-secondary` (`style.css:150`): los botones, pero solo dentro de la barra transparente |
| `A > B` (hijo directo) | B que sea hijo **inmediato** de A | `.hero > .container` (`style.css:183`) |
| `.a.b` (las dos clases) | Elemento que tenga **ambas** clases | `.seat.seat--seleccionada` (`style.css:291`) |
| `[atributo="valor"]` | Por atributo | `[data-bs-theme="dark"]` (`style.css:41`): reglas que valen solo en modo oscuro. `[id^="step"]` (`style.css:252`): ids que **empiezan** con "step" |
| `:pseudo-clase` | Un **estado** | `.movie-card:hover` (mouse encima), `.seat:hover:not(:disabled)`, `.seat:focus-visible` (foco con teclado) |
| `:has()` | Elemento que **contiene** algo | `.navbar-transparente:has(.navbar-collapse.show)` (`style.css:154`): si el menú del celular está abierto, la barra deja de ser transparente |
| `::pseudo-elemento` | Una parte "virtual" | `.hero::before` (una capa decorativa de rayitas), `.funciones-dia h3::first-letter` (primera letra en mayúscula) |

#### Propiedades comunes (con ejemplos nuestros)

| Propiedad | Para qué | Ejemplo real |
|---|---|---|
| `color` | Color del texto | `.hero { color: #fff; }` → texto blanco en el hero |
| `background` | Fondo (color, degradé o imagen) | `.seat { background: var(--seat-free); }` → butaca azul |
| `width` / `height` | Ancho / alto | `.seat { width: 34px; height: 30px; }` |
| `margin` | Espacio **afuera** del borde | `.pantalla { margin: 0 auto 2rem; }` → centrada (`auto` a los costados) y separada 2rem de las butacas |
| `padding` | Espacio **adentro** del borde | `.poster-generado { padding: 1.1rem; }` → el título no queda pegado al borde del póster |
| `border` / `border-radius` | Borde / esquinas redondeadas | `.seat { border-radius: 8px 8px 4px 4px; }` → forma de asiento (arriba más redondo) |
| `display` | Cómo se ubica la caja | `block` (ocupa todo el ancho), `inline` (dentro del texto), `none` (oculto), `flex`, `grid` |
| `position` | Posicionamiento | `.poster img { position: absolute; inset: 0; }` → la imagen se superpone exactamente sobre el póster generado |
| `transition` / `transform` | Animaciones | `.movie-card:hover { transform: translateY(-4px); }` + `transition` → la tarjeta "sube" suavemente al pasar el mouse |
| Unidades | `px` (píxeles), `rem` (relativo al tamaño de letra base, 1rem ≈ 16px), `%`, `vh` (% del alto de la ventana) | `.hero { min-height: 82vh; }` |

#### Flexbox (`display: flex`)

Pone los hijos **en fila** (o columna) y permite alinearlos fácilmente. Ejemplo real, cada fila del mapa de butacas (`style.css:274`):

```css
.fila-butacas {
    display: flex;            /* los hijos (letra de fila + butacas) van uno al lado del otro */
    align-items: center;      /* centrados verticalmente */
    justify-content: center;  /* el grupo centrado horizontalmente */
    gap: .4rem;               /* espacio entre butacas */
    margin-bottom: .4rem;     /* separación con la fila de abajo */
}
```

En pantalla: `A [1][2][3][4][5][6] A`, centrado bajo la "PANTALLA".

Bootstrap trae lo mismo como clases: `d-flex` = `display:flex`, `justify-content-between`, `align-items-center`, `gap-2`, `flex-wrap`, `flex-column`. Por eso en el HTML vas a ver mucho `class="d-flex align-items-center gap-2"` en lugar de reglas en el CSS.

#### El modelo de caja

**Todo elemento es una caja** con cuatro capas, de adentro hacia afuera:

```
┌──────────────────── margin (afuera, transparente) ─────────────────┐
│   ┌──────────────── border (el borde visible) ──────────────────┐  │
│   │   ┌──────────── padding (relleno interno) ───────────────┐  │  │
│   │   │                                                      │  │  │
│   │   │               contenido (width × height)             │  │  │
│   │   │                                                      │  │  │
│   │   └──────────────────────────────────────────────────────┘  │  │
│   └─────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────┘
```

Ejemplo real, la pantalla del cine en el mapa de butacas (`style.css:263-273`):

```css
.pantalla {
    width: min(80%, 520px);                 /* CONTENIDO: 80% del ancho, máximo 520px */
    padding-top: .4rem;                     /* PADDING: separa el texto "PANTALLA" de la línea */
    border-top: 4px solid var(--cine-primary); /* BORDER: la línea amarilla curva de arriba */
    border-radius: 50% 50% 0 0 / 12px 12px 0 0; /* curva el borde, como una pantalla */
    margin: 0 auto 2rem;                    /* MARGIN: centrada y con 2rem de aire abajo */
    ...
}
```

Bootstrap, como casi todos los frameworks, usa `box-sizing: border-box`: el `width` **incluye** padding y borde (así una caja de 34px mide 34px en total, no 34 + padding).

#### Variables CSS

Son valores con nombre que se definen una vez y se reusan con `var(--nombre)`. Están al principio de `style.css`:

```css
:root, [data-bs-theme="light"] {       /* style.css:11 — tema claro (y valores por defecto) */
    --cine-primary: #eab308;           /* el amarillo del cine */
    --cine-surface: #ffffff;           /* fondo de tarjetas y modales */
    --seat-free: #3b82f6;              /* butaca libre: azul */
    --seat-taken: #cbd5e1;             /* butaca ocupada: gris claro */
    --bs-primary: #eab308;             /* ← pisamos una variable DE BOOTSTRAP */
    --bs-body-bg: #f5f6f8;             /* ← fondo de la página según Bootstrap */
    ...
}
[data-bs-theme="dark"] {               /* style.css:41 — tema oscuro: solo lo que cambia */
    --cine-surface: #141821;
    --seat-taken: #475569;
    --bs-body-bg: #0b0d12;
    ...
}
```

**Así funciona el modo claro/oscuro:** el botón de sol/luna llama a `Tema.alternar()` (`api.js:224-232`), que cambia el atributo `data-bs-theme` de `<html>`. Eso activa el otro bloque de variables y **toda la página cambia de color sin tocar ninguna otra regla**, porque todas usan `var(...)`.

#### Qué viene de Bootstrap y qué es nuestro

**Regla práctica:** si una clase aparece en el HTML pero **no la encontrás en `css/style.css` ni en `css/admin.css`, es de Bootstrap.**

| De **Bootstrap** (no están en nuestros CSS) | Qué hacen |
|---|---|
| `container` | Centra el contenido con un ancho máximo que depende de la pantalla |
| `row`, `col-md-6`, `row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4`, `g-4` | **Grilla** de 12 columnas. Ej. `cartelera.html:21`: 1 película por fila en celular, 2 en `sm` (≥576px), 3 en `md` (≥768px), 4 en `xl` (≥1200px); `g-4` = espacio entre tarjetas |
| `btn`, `btn-primary`, `btn-outline-secondary`, `btn-sm`, `btn-lg` | Botones |
| `card`, `card-body`, `card-title` | Tarjetas (películas, pasos de la compra, panel) |
| `modal`, `modal-dialog`, `modal-content`… | Ventanas emergentes (login, formularios) |
| `navbar`, `navbar-expand-lg`, `collapse`, `navbar-toggler` | Barra de navegación que se colapsa en un botón ☰ en pantallas chicas |
| `offcanvas-lg` | Menú lateral del panel: fijo en pantallas grandes, deslizable en celular |
| `form-control`, `form-select`, `form-label` | Campos de formulario |
| `badge`, `alert`, `table`, `list-group`, `spinner-border` | Etiquetas, avisos, tablas, listas, ruedita de "cargando" |
| `d-flex`, `gap-2`, `mb-3`, `p-4`, `px-3`, `py-5`, `text-end`, `fw-bold`, `rounded-pill`, `w-100`, `text-body-secondary` | **Utilidades**: una clase = una propiedad (`mb-3` = `margin-bottom: 1rem`, `fw-bold` = `font-weight: bold`) |

| **Nuestras** (`css/style.css`) | Qué parte de la pantalla cambian |
|---|---|
| Variables `--cine-*` y las `--bs-*` pisadas (líneas 11-56) | Colores de todo el sitio y el modo claro/oscuro |
| `.btn-primary` (línea 65) | No es una clase nueva: **reconfigura** la de Bootstrap pisando sus variables `--bs-btn-*`, para que el botón principal sea amarillo con letra oscura en vez del azul por defecto |
| `.navbar-cine`, `.navbar-transparente`, `.logo-mark` (126-158) | La barra superior: fondo, borde, transparencia sobre el hero, el cuadradito amarillo del logo |
| `.page` (161) | Deja espacio arriba en las páginas internas para que la navbar fija no tape el título |
| `.hero`, `.hero-badge` (164-185) | El bloque grande de bienvenida del inicio |
| `.poster`, `.poster-generado`, `.poster-ribbon` (188-216) | Los pósters: proporción 2:3, la imagen real encima del póster "generado" con el título y un degradé de color |
| `.movie-card` (219-220) | La tarjeta que sube y se ilumina al pasar el mouse en Cartelera / Estrenos |
| `.carousel-3d`, `.carousel-slide`, `.carousel-caption-3d` (223-249) | El carrusel 3D del inicio (Bootstrap tiene carrusel, pero no con este efecto) |
| `.funcion-btn` (253-255) | Los botones de horario en la compra |
| `.sala-mapa`, `.pantalla`, `.fila-butacas`, `.seat`, `.seat--ocupada`, `.seat--seleccionada` (262-292) | El **mapa de butacas** completo |
| `.comprobante-icono` (295) | El círculo verde con el tilde al confirmar la compra |
| `.swal2-*` (304-312) | Ajusta las alertas de SweetAlert2 para que combinen con el sitio |

| **Nuestras** (`css/admin.css`, solo en el panel) | Qué cambian |
|---|---|
| `.sidebar` (8-31) | El menú lateral: ancho, fijo con `position: sticky` en pantallas ≥992px (`@media`), colores de los links |
| `.profile-avatar` | El círculo amarillo con la inicial del admin |
| `.top-header` | La barra superior del contenido del panel |
| `.metric-icon`, `.metric-value` | Las 4 tarjetas de indicadores del dashboard |
| `.chart-box` (67) | **Contenedor de los gráficos con alto fijo de 280px**: Chart.js con `maintainAspectRatio: false` necesita que el contenedor tenga alto, si no el gráfico crece sin control |
| `.btn-icono`, `.table-poster`, `.fila-pasada`, `.num` | Botones cuadrados de editar/borrar, pósters miniatura en tablas, funciones pasadas en gris, números alineados a la derecha |

#### Responsive (que se adapte al celular)

Se logra de dos formas:

1. **Clases de Bootstrap con breakpoint** (`-sm-`, `-md-`, `-lg-`, `-xl-`): `row-cols-md-3`, `navbar-expand-lg`, `offcanvas-lg`, `col-sm-6`. Significan "a partir de ese ancho, hacé esto".
2. **`@media` propios**: bloques de CSS que solo valen en ciertos anchos. Ej. `style.css:245`:
   ```css
   @media (max-width: 575.98px) {      /* solo en celulares */
       .carousel-3d { height: 400px; }
       .carousel-slide { width: 210px; margin-left: -105px; }   /* pósters del carrusel más chicos */
   }
   ```

### 4.4 JavaScript

#### Qué es y qué hace en el navegador

JavaScript es un **lenguaje de programación** que el navegador ejecuta. En este proyecto hace todo lo dinámico:

- **Pide datos** a la API (`fetch`).
- **Arma HTML** con esos datos y lo inserta en la página.
- **Reacciona** a lo que hace el usuario (clics, formularios, scroll).
- **Guarda** la sesión en `localStorage`.

No usamos ningún framework (ni React, ni Angular, ni Vue): es JavaScript "a pelo" (*vanilla JS*), y los archivos **no son módulos**: todo lo que se declara arriba de un archivo (`const Auth`, `function api`) queda **global**, y por eso `main.js` puede usar lo que definió `api.js`. Por eso importa el orden de los `<script>`.

#### Variables

```js
const API_URL = 'http://localhost:5191/api';   // api.js:8 — const: no se puede reasignar
let actual = 0;                                 // index.js:32 — let: se puede cambiar (actual = 1)
```

No se declaran tipos: una variable puede tener un número, un texto, un objeto, un array, una función… (Es un lenguaje de tipado dinámico, a diferencia de C#.)

Tipos que vas a ver:

```js
'texto'  o  "texto"                       // string
`Hola ${resp.nombre}`                     // template string: permite meter variables con ${...} y varias líneas
42, 3.5                                   // number (no hay int/decimal separados)
true / false                              // boolean
null / undefined                          // "sin valor"
[1, 2, 3]                                 // array
{ email, password }                       // objeto (parecido a un diccionario o un objeto anónimo de C#)
```

`{ email, password }` es un atajo de `{ email: email, password: password }`.

#### Funciones

Tres formas de escribirlas, todas en el proyecto:

```js
// 1) Declaración clásica — main.js:221
function mostrarError(id, msg) {
    const el = document.getElementById(id);
    el.textContent = msg || '';
    el.hidden = !msg;
}

// 2) Arrow function (función flecha), como las lambdas de C# — api.js:86
const fmtMoneda = n => (Number(n) || 0).toLocaleString('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 });
// fmtMoneda(4000) → "$ 4.000"

// 3) async: función que puede esperar operaciones lentas con await — main.js:227
async function onLogin(e) { ... }
```

Los arrays tienen métodos muy parecidos a LINQ:

| JavaScript | LINQ (C#) | Ejemplo real |
|---|---|---|
| `.map(x => ...)` | `.Select` | `peliculas.map(p => \`<div>...${p.titulo}...</div>\`)` — `cartelera.js:20` |
| `.filter(x => ...)` | `.Where` | `funciones.filter(f => f.idPelicula === p.idPelicula)` — `cartelera.js:21` |
| `.find(x => ...)` | `.FirstOrDefault` | `estado.funciones.find(f => f.idFuncion === idFuncion)` — `reserva.js:94` |
| `.some(x => ...)` | `.Any` | `susFunciones.some(f => f.idPromocion)` — `cartelera.js:26` |
| `.reduce((acc, x) => ..., 0)` | `.Aggregate` / `.Sum` | `d.reduce((s, m) => s + m.totalRecaudado, 0)` — `admin.js` (`cargarMensual`) |
| `.join('')` | `string.Join("", ...)` | Une los pedazos de HTML que devolvió `.map` |

Otros detalles de sintaxis que aparecen seguido:

- `===` compara valor **y** tipo (`'5' === 5` es `false`). Se usa siempre en lugar de `==`.
- `a?.b` (*optional chaining*): si `a` es `null`, devuelve `undefined` en vez de romper. Igual que en C#. Ej.: `this.sesion?.token`.
- `a ?? b`: si `a` es `null`/`undefined`, usa `b`. Igual que en C#.
- `...objeto` (*spread*): copia las propiedades de un objeto dentro de otro. Ej.: `{ ...opcionesSwal(), toast: true }`.
- `const [a, b] = await Promise.all([...])` (*desestructuración*): saca los elementos de un array en variables.

#### El DOM: cómo JS busca y modifica el HTML

Cuando el navegador lee el HTML, arma en memoria un **árbol de objetos**, el **DOM** (*Document Object Model*). Cada etiqueta es un objeto que JS puede leer y modificar; cuando JS lo modifica, **la pantalla se actualiza sola**.

```
document
 └ html
    ├ head ...
    └ body
       ├ header#navbarCine
       ├ main
       │  ├ section.hero ...
       │  └ section
       │     └ div.container
       │        └ div#carouselTrack.carousel-3d     ← index.js lo llena
       └ footer
```

**Buscar elementos:**

| Método | Qué devuelve | Ejemplo real |
|---|---|---|
| `document.getElementById('x')` | El elemento con `id="x"` | `document.getElementById('carouselTrack')` — `index.js:7`. En `admin.js` hay un atajo: `const $ = id => document.getElementById(id);` (línea 22), y se usa `$('chartTop')` |
| `document.querySelector('selector CSS')` | El **primero** que coincide | `header.querySelector('nav')` — `main.js:72` |
| `document.querySelectorAll('selector CSS')` | **Todos** los que coinciden | `contenedor.querySelectorAll('.seat:not(:disabled)')` — `butacas.js:56` (todas las butacas clickeables) |

Fijate que `querySelector` usa **los mismos selectores que CSS**.

**Modificar elementos:**

| Qué | Ejemplo real |
|---|---|
| Reemplazar todo el HTML interno | `track.innerHTML = htmlCargando('Cargando cartelera...');` — `index.js:8` |
| Agregar HTML al final | `document.body.insertAdjacentHTML('beforeend', \`<div class="modal"...\`)` — `main.js:123` |
| Cambiar solo texto (seguro, no interpreta HTML) | `$('mRecaudado').textContent = fmtMoneda(totales.totalRecaudado);` — `admin.js:239` |
| Agregar / quitar / alternar una clase | `btn.classList.toggle('seat--seleccionada', marcada);` — `butacas.js:69`. **Así JS cambia el aspecto: no toca el CSS, cambia la clase, y el CSS ya tiene la regla para esa clase** |
| Mostrar / ocultar | `document.getElementById('step2').hidden = false;` — `reserva.js:108` |
| Leer atributos `data-*` | `<button data-id="15">` → `btn.dataset.id` vale `"15"` — `butacas.js:58` |
| Leer lo que escribió el usuario | `document.getElementById('loginEmail').value` — `main.js:229` |
| Estilo directo | `slide.style.transform = \`translateX(...) scale(...)\`;` — `index.js:50` |

> **`innerHTML` y seguridad:** cuando se arma HTML con datos de la base, siempre se pasan por `esc()` (`api.js:106`), que convierte `<` en `&lt;`, etc. Si una película se llamara `<script>robar()</script>`, se vería como texto en lugar de ejecutarse.

#### Eventos

Un **evento** es algo que pasa: un clic, enviar un formulario, cambiar un `<select>`, scrollear, que termine de cargar la página. Se "escucha" con `addEventListener(tipo, función)`: la función se ejecuta **cada vez** que ocurre.

```js
// Clic en un botón — index.js:63
document.getElementById('prevSlide').addEventListener('click', () => {
    actual = (actual - 1 + slides.length) % slides.length;   // slide anterior (circular)
    actualizar();                                             // reacomoda los pósters
});
```

Eventos que usa el proyecto:

| Evento | Cuándo | Ejemplo |
|---|---|---|
| `DOMContentLoaded` | El HTML terminó de cargarse | Todos los archivos arrancan así: `document.addEventListener('DOMContentLoaded', ...)` (`index.js:6`, `main.js:29`, `admin.js:28`…) |
| `click` | Clic | Butacas (`butacas.js:57`), botones de horario (`reserva.js:90`) |
| `submit` | Se envía un formulario (botón submit o Enter) | `loginForm` → `onLogin` (`main.js:217`) |
| `change` | Cambia un `<select>` / `<input>` | Tipo de espectador (`butacas.js:133`), filtros del dashboard (`admin.js:223`, con la forma alternativa `elemento.onchange = función`) |
| `scroll` | El usuario scrollea | Navbar transparente (`main.js:74`) |
| `hashchange` | Cambia lo que va después de `#` en la URL | Navegación del panel (`admin.js:48`) |
| `shown.bs.modal` | Evento **de Bootstrap**: terminó de abrirse un modal | Pone el foco en el primer campo (`api.js:217`) |
| `sesion-cambiada`, `tema-cambiado`, `sesion-expirada` | Eventos **inventados por nosotros** con `dispatchEvent` | Al loguearse, `main.js:290` avisa y `reserva.js:17` actualiza el botón "Confirmar compra". Al cambiar el tema, `api.js:229` avisa y `admin.js:69` redibuja los gráficos. Si la API rechaza el token guardado al cargar la página, `api.js:282` avisa y lo escuchan `main.js:347` y `admin.js:1135` |

En un `submit`, lo primero es `e.preventDefault()` (`main.js:228`): el comportamiento por defecto de un formulario es **recargar la página** enviando los datos, y nosotros queremos manejarlo con JS.

#### Llamar al backend: `fetch`, promesas, `async`/`await` y JSON

`fetch(url, opciones)` hace un pedido HTTP desde el navegador. Como la red es lenta, **no devuelve la respuesta, sino una *promesa*** (algo parecido a un `Task<T>` de C#). Con `await` se espera a que se resuelva, **sin congelar la página**. Exactamente igual que `async`/`await` en C#.

**JSON** es el formato de texto con el que viajan los datos. `JSON.stringify(objeto)` lo convierte a texto para mandarlo; `JSON.parse(texto)` lo convierte en objeto al recibirlo. Del lado de C#, ASP.NET hace lo mismo automáticamente con `[FromBody]` y `Ok(...)`, y pasa los nombres a *camelCase*: la propiedad `IdButaca` del DTO llega a JS como `idButaca`.

Nadie en el proyecto llama a `fetch` directamente: todos usan la función **`api()`** de `api.js:43-73`, que hace todo el trabajo repetitivo:

```js
async function api(ruta, { method = 'GET', body } = {}) {
    const esArchivo = body instanceof FormData;                 // ¿subida de archivo (póster)?
    const headers = { 'Accept': 'application/json' };
    if (body !== undefined && !esArchivo) headers['Content-Type'] = 'application/json';
    if (Auth.token) headers['Authorization'] = `Bearer ${Auth.token}`;   // ← el JWT

    let resp;
    try {
        resp = await fetch(`${API_URL}${ruta}`, {               // ← el pedido HTTP
            method,
            headers,
            body: body === undefined ? undefined : (esArchivo ? body : JSON.stringify(body))
        });
    } catch {                                                   // ← no hubo respuesta (API apagada)
        throw new Error('No se pudo conectar con la API. ¿Está corriendo el proyecto CineHollywoodTPI.API?');
    }

    const texto = await resp.text();
    let datos = null;
    try { datos = texto ? JSON.parse(texto) : null; } catch { datos = texto; }

    if (resp.ok) return datos;                                  // 2xx → devuelve los datos ya parseados

    if (resp.status === 401 && Auth.estaLogueado()) {           // token vencido o inválido
        Auth.cerrar();
        throw new Error('Tu sesión expiró. Volvé a iniciar sesión.');
    }
    if (resp.status === 403 && !datos) throw new Error('No tenés permisos para realizar esta acción.');

    throw new Error(mensajeDeError(datos) || `Error ${resp.status}`);   // 400/404/409... → Error con el mensaje de la API
}
```

Puntos importantes:

- Un `fetch` **no falla** con un 400 o un 404: solo falla si no hay conexión. Por eso `api()` revisa `resp.ok` y **convierte los códigos de error en excepciones** (`throw new Error(...)`).
- El mensaje del error es **el texto que mandó la API**. Recordá que `ManejoErroresMiddleware` responde con texto plano, por ejemplo *"Ya existe una cuenta con ese email."* con un 409. Ese texto termina en `err.message` y se muestra tal cual.
- Quien llama hace simplemente:
  ```js
  try {
      const peliculas = await api('/pelicula/cartelera');   // GET
      // usar peliculas...
  } catch (err) {
      cont.innerHTML = htmlVacio(err.message);             // mostrar el error
  }
  ```
  y para un POST: `await api('/auth/login', { method: 'POST', body: { email, password } })`.
- **Varios pedidos en paralelo**: `Promise.all` (como `Task.WhenAll`). En `cartelera.js:10`:
  ```js
  const [peliculas, funciones] = await Promise.all([
      api('/pelicula/cartelera'),
      api('/funcion?soloProximas=true')
  ]);
  ```
  Los dos pedidos salen a la vez y se espera a que terminen ambos.

#### Closures

Una *closure* es una función que **recuerda las variables del lugar donde fue creada**, incluso después de que esa función externa terminó. Ejemplo real en `butacas.js:18-76`:

```js
function crearMapaButacas(contenedor, butacas, { max = 10, onChange = () => {} } = {}) {
    const seleccionadas = new Map();    // variable "privada"
    ...
    btn.addEventListener('click', () => { ... seleccionadas.set(id, ...) ... });  // la usa después
    ...
    return { seleccion: () => [...seleccionadas.values()] };    // y la expone solo para leer
}
```

`crearMapaButacas` termina enseguida, pero los manejadores de clic y la función `seleccion` siguen accediendo a `seleccionadas`. Desde afuera nadie puede modificarla directamente: es como un campo `private` de una clase.

### 4.5 Nuestros archivos JS, uno por uno

Carpeta: `CineHollywoodTPI.Web/wwwroot/js/`.

#### `api.js` — la base de todo (se carga en todas las páginas)

| Función / objeto | Línea | Qué hace |
|---|---|---|
| `API_URL` | 8 | `'http://localhost:5191/api'`. Si cambia el puerto de la API, se cambia acá |
| `Auth` | 13 | Sesión JWT en `localStorage` (ver [3.5](#35-dónde-se-guarda-en-el-cliente)) |
| `api(ruta, opciones)` | 43 | El `fetch` con token y manejo de errores (ver arriba) |
| `mensajeDeError(datos)` | 76 | Saca el texto de error, sea texto plano (nuestro middleware) o un `ProblemDetails` de ASP.NET (`{ title, errors }`, el formato que usa ASP.NET cuando falla la validación automática del modelo) |
| `fmtMoneda`, `fmtNumero`, `fmtFecha`, `fmtFechaLarga`, `fmtFechaCorta`, `fmtHora`, `fmtFechaHora`, `hoyISO` | 86-103 | Formatos argentinos: `$ 4.000`, `19/11/2026`, `jueves, 19 de noviembre`. `parseFecha` (90) arma la fecha "a mano" para evitar que `"2026-11-20"` se corra un día por la zona horaria |
| `esc(valor)` | 106 | Escapa HTML (seguridad) |
| `urlPoster`, `posterHTML(pelicula, ribbon)` | 115, 126 | Arma el HTML del póster. Si la película tiene `posterUrl`, pone la imagen **encima** de un póster "generado" con el título y un color calculado del texto (`hueDesdeTexto`). Si la imagen falla al cargar, `onerror="this.remove()"` la borra y queda el generado |
| `badgeClasificacion(desc)` | 138 | Etiqueta de color según la clasificación: verde ATP, rojo +16/+18, amarilla el resto |
| `opcionesSwal`, `toast`, `alerta`, `confirmar` | 151-205 | Envoltorios de **SweetAlert2**. Toman los colores de las variables CSS para respetar el tema. `confirmar()` devuelve `true`/`false` |
| `abrirModal(id)`, `cerrarModal(id)` | 210-215 | Abren/cierran un modal de **Bootstrap** por JS |
| `Tema` | 224 | Modo claro/oscuro: cambia `data-bs-theme` en `<html>`, lo guarda en `localStorage` (`cine_tema`) y dispara el evento `tema-cambiado` |
| `htmlCargando`, `htmlVacio`, `htmlDetalleVenta` | 238-266 | Pedazos de HTML reutilizables: spinner, mensaje de vacío/error, tabla del detalle de una venta |
| `verificarSesion()` | 276-285 | Al cargar cada página, si hay sesión, llama a `GET /auth/yo`. Si la API responde 401, la sesión ya quedó cerrada y dispara el evento `sesion-expirada` (ver [3.7](#37-expiración)) |

#### `main.js` — páginas públicas (navbar, login, registro, mis compras)

| Función | Línea | Qué hace |
|---|---|---|
| Arranque | 29 | Al cargar la página: dibuja la navbar, inyecta los modales de login/registro/mis compras y la zona de usuario. Si la URL tiene `?login=1`, abre el login (lo usa `admin.html` para mandarte a loguearte) |
| `renderNavbar()` | 41 | Escribe la navbar de Bootstrap dentro de `<header id="navbarCine">`, marca como `active` el link de la página actual y, si el header tiene `data-transparente`, escucha el `scroll` para alternar la clase `navbar-transparente` |
| `renderAuthArea()` | 82 | Según `Auth.sesion` muestra "Iniciar sesión / Registrarse" o "Hola, Nombre / Mis compras / Salir", el botón de tema, y agrega el link **Panel de administrador** solo si `Auth.esAdmin()` |
| `inyectarModalesAuth()` | 122 | Mete en el `<body>` el HTML de los tres modales y conecta los eventos `submit` |
| `onLogin(e)` | 227 | Login: lee email y contraseña, `POST /auth/login`, y si sale bien llama a `despuesDeLoguear` |
| `onRegistro(e)` | 247 | Registro: valida en el cliente (la API vuelve a validar) y `POST /auth/registro` |
| `despuesDeLoguear(resp, modalId, mensaje)` | 285 | **Guarda el JWT** (`Auth.guardar`), cierra el modal, muestra un toast, redibuja la navbar, dispara `sesion-cambiada` y, si había una acción pendiente (`Sesion.alLoguearse`, ej. "seguir con la compra"), la ejecuta |
| `abrirMisCompras()` | 304 | `GET /venta` (con token: la API devuelve solo las del cliente del token) y arma la tabla; "Ver detalle" llama a `GET /venta/{id}` |
| Escucha de `sesion-expirada` | 347 | Si la verificación de `api.js` recibió un 401: toast "Tu sesión expiró", redibuja la navbar sin sesión y dispara `sesion-cambiada` |

#### `index.js` — carrusel 3D del inicio

Al cargar: muestra un spinner, pide `GET /pelicula/cartelera`, crea un `<button class="carousel-slide">` por película (con `posterHTML`) y define `actualizar()` (línea 34), que calcula para cada póster su distancia al del centro y le aplica `transform`, `opacity`, `zIndex` y `blur` desde una tabla de configuraciones. Clic en un póster lateral → pasa al centro; clic en el del centro → `reserva.html?id=...`. Las flechas cambian `actual` y llaman a `actualizar()`. El movimiento suave lo hace el CSS (`transition: transform .5s` en `.carousel-slide`): JS solo cambia los valores.

#### `cartelera.js` — página de cartelera

Pide en paralelo películas en cartelera y funciones próximas, y por cada película arma una tarjeta Bootstrap (`card`) con el póster, la clasificación, la duración y hasta 6 "chips" de horario. Cada chip es un link a `reserva.html?id=<película>&funcion=<función>`.

#### `butacas.js` — componente reutilizable (compra web y boletería)

| Función | Qué hace |
|---|---|
| `crearMapaButacas(contenedor, butacas, { max, onChange })` (línea 18) | Recibe la respuesta de `GET /funcion/{id}/butacas` (lista de `{ idButaca, fila, numero, habilitada, ocupada }`), agrupa por fila, dibuja la pantalla y un `<button class="seat">` por butaca (deshabilitado si está ocupada). Al hacer clic: agrega/quita la butaca del `Map` `seleccionadas` (máximo `max`), alterna la clase `seat--seleccionada` y llama a `onChange` con la selección |
| `renderDetalleEntradas(contenedor, seleccion, funcion, onCambio)` (línea 87) | Por cada butaca elegida muestra un `<select>` de tipo de espectador con **los precios que calculó el servidor** (`funcion.precios`, uno por tipo; los no permitidos, como "Menor" en una +18, no se ofrecen). Devuelve `{ entradas(), total(), detalle() }` para que la página arme el body del POST y el resumen |

#### `reserva.js` — compra en 3 pasos

| Función | Qué hace |
|---|---|
| `iniciarReserva()` (19) | Lee `?id=` y `?funcion=` de la URL, pide película + funciones + formas de pago en paralelo, muestra el encabezado y el paso 1. Si vino `?funcion=`, la preselecciona |
| `renderFunciones()` (65) | Paso 1: botones de horario agrupados por día |
| `elegirFuncion(id)` (93) | Marca el botón elegido, muestra el paso 2 y scrollea hasta él |
| `cargarMapa()` (117) | `GET /funcion/{id}/butacas` → `crearMapaButacas` |
| `onSeleccionCambiada()` (129) | Redibuja la lista de entradas y muestra/oculta el paso 3 |
| `actualizarPaso3()` (139) | Ajusta el botón según la sesión: "Iniciar sesión y comprar" o "Confirmar compra"; si sos admin, lo deshabilita (los admin venden desde el panel) |
| `renderResumen()` (154) | Arma el resumen (película, función, butacas, cantidad por tipo, total) con un `list-group` de Bootstrap |
| `confirmarCompra()` (178) | Si no hay sesión, abre el login y deja pendiente seguir. Si hay, `POST /venta` con `{ entradas, formasPago }` (el cliente y el canal los saca la API **del token**). Si la butaca se ocupó mientras tanto (409), recarga el mapa |
| `mostrarComprobante(idVenta)` (212) | Oculta el flujo y muestra el comprobante con `GET /venta/{id}` |

#### `admin.js` — panel de administración (1130 líneas)

**Estructura general**

- **Protección** (línea 28): si `!Auth.esAdmin()`, redirige a `index.html?login=1`. (Antes ya lo hizo el `<script>` del `<head>` de `admin.html`.) Además, si la verificación de `api.js` con `GET /auth/yo` recibe un 401, `admin.js:1135` escucha el evento `sesion-expirada`, avisa con un toast y redirige al login.
- **Estado global** en el objeto `Admin` (línea 10): catálogos, listas cargadas, los gráficos (`charts`) y los datos de los reportes.
- **Navegación por hash** (`mostrarSeccion`, línea 78): los links del menú son `href="#peliculas"`, `#ventas`… Cambiar el hash **no recarga la página**, dispara `hashchange`. `mostrarSeccion` oculta todas las `<section class="admin-section">` menos `section-<nombre>`, marca el link activo, cierra el menú en celular (`bootstrap.Offcanvas`) y llama a la función de carga de esa sección.
- **Patrón de cada ABM** (Películas, Funciones, Salas, Precios, Clientes): `cargarX()` (GET a la API) → `renderX()` (arma la tabla) → `abrirFormX(id)` (llena el modal para alta o edición) → `guardarX(e)` (POST o PUT) → `borrarX(id)` (`confirmar()` y DELETE).

**Cómo se arman los gráficos con Chart.js**

Chart.js dibuja sobre un `<canvas>` (un elemento HTML que es un "lienzo" de píxeles). En `admin.html` hay dos:

```html
<div class="chart-box"><canvas id="chartTop" ...></canvas></div>       <!-- admin.html:126 -->
<div class="chart-box"><canvas id="chartMensual" ...></canvas></div>   <!-- admin.html:146 -->
```

El flujo para "Películas más vendidas":

1. `cargarTop()` (línea 254) arma los parámetros con los filtros (`top`, `desde`, `hasta`), llama a `GET /reporte/peliculas-mas-vendidas?top=5&...` y guarda el resultado en `Admin.datosTop`. La API devuelve una lista de `PeliculaMasVendidaDTO`:
   ```json
   [ { "idPelicula": 2, "titulo": "...", "cantidadEntradas": 35, "totalRecaudado": 140000 }, ... ]
   ```
2. Llena la tabla de abajo (`tablaTop`) y llama a `dibujarGraficoTop()`.
3. `dibujarGraficoTop()` (línea 329):
   ```js
   if (!window.Chart) return;            // si el CDN de Chart.js no cargó, quedan solo las tablas
   Admin.charts.top?.destroy();          // si ya había un gráfico en ese canvas, se destruye primero
   const c = coloresGrafico();           // colores leídos de las variables CSS (cambian con el tema)
   const datos = Admin.datosTop;

   Admin.charts.top = new Chart($('chartTop'), {     // ← crea el gráfico sobre el <canvas>
       type: 'bar',                                  // gráfico de barras
       data: {
           labels: datos.map(d => d.titulo),         // eje: un título por barra
           datasets: [{
               label: 'Entradas vendidas',
               data: datos.map(d => d.cantidadEntradas),   // largo de cada barra
               backgroundColor: c.primario,                // amarillo del cine
               borderRadius: 4,
               maxBarThickness: 28
           }]
       },
       options: {
           ...opcionesBase(c),        // responsive: true, maintainAspectRatio: false, sin leyenda
           indexAxis: 'y',            // ← barras HORIZONTALES (los títulos en el eje Y)
           scales: { x: { beginAtZero: true, ticks: { precision: 0 } ... }, y: { ... } },
           plugins: {
               tooltip: { callbacks: {
                   label: ctx => ` ${fmtNumero(ctx.raw)} entradas · ${fmtMoneda(datos[ctx.dataIndex].totalRecaudado)}`
               } }                    // texto que aparece al pasar el mouse por una barra
           }
       }
   });
   ```

En resumen, a Chart.js se le pasa: **el canvas**, **el tipo** (`'bar'`), **los datos** (`labels` = nombres, `datasets[].data` = valores) y **opciones** (ejes, colores, tooltip).

El gráfico mensual (`dibujarGraficoMensual`, línea 366) es igual pero vertical, con `labels: MESES` (`['Ene', 'Feb', ...]`, línea 7) y `data` = recaudación de cada mes; el eje Y muestra pesos con `ticks.callback: v => fmtMoneda(v)`. La API siempre devuelve los 12 meses (con 0 los que no tienen ventas), así que las barras coinciden con las etiquetas.

Detalles:

- **`destroy()` antes de redibujar**: Chart.js no permite dos gráficos en el mismo canvas.
- **Redibujo al cambiar el tema**: `admin.js:69` escucha `tema-cambiado` y vuelve a llamar a `dibujarGraficoTop()` y `dibujarGraficoMensual()`, porque los colores del gráfico se leen de las variables CSS en el momento de crearlo (`coloresGrafico`, línea 309) y no se actualizan solos.
- **Alto fijo**: `.chart-box { height: 280px; }` en `admin.css:67` + `maintainAspectRatio: false` = el gráfico ocupa todo el ancho de la tarjeta y siempre 280px de alto.
- **Filtros**: `admin.js:223-226` conecta `topDesde`, `topHasta`, `topCantidad` y `mensualAnio` (evento `change`) a `cargarTop` / `cargarMensual`.

### 4.6 Cómo se conectan HTML, CSS y JS

```
                   ┌───────────────────────────────┐
                   │          reserva.html          │
                   │                                │
  <link> ─────────▶│  bootstrap.min.css  style.css  │──▶ el CSS pinta TODO lo que haya en la página,
                   │                                │    incluido el HTML que el JS agrega después
                   │  <div id="mapaButacas"></div>  │◀── "ganchos" vacíos con id
                   │                                │
  <script> ───────▶│  api.js → main.js → butacas.js │──▶ el JS busca los ganchos por id,
                   │         → reserva.js           │    les mete HTML con clases (seat, card, btn…)
                   └───────────────────────────────┘    y escucha eventos
```

Las tres reglas que conectan todo:

1. **HTML → CSS, por clases.** El HTML dice `class="seat"`; el CSS tiene `.seat { ... }`. Funciona igual si el `<button class="seat">` estaba en el archivo o lo creó JS después.
2. **HTML → JS, por `id` y `data-*`.** El HTML dice `id="mapaButacas"`; el JS hace `getElementById('mapaButacas')`. Los `data-id="15"` llevan datos (ids de la base) del HTML al JS.
3. **JS → CSS, cambiando clases.** JS no cambia colores directamente: agrega o saca clases (`seat--seleccionada`, `active`, `navbar-transparente`) o el atributo `data-bs-theme`, y el CSS ya tiene la regla para cada caso.

Organización de los archivos del front:

```
CineHollywoodTPI.Web/wwwroot/
├── index.html, cartelera.html, estrenos.html,   ← una página = un archivo HTML
│   promociones.html, reserva.html, admin.html
├── css/
│   ├── style.css     ← lo cargan TODAS las páginas (después de Bootstrap)
│   └── admin.css     ← solo admin.html (después de style.css)
└── js/
    ├── api.js        ← TODAS las páginas, siempre primero de los nuestros
    ├── main.js       ← páginas públicas (admin.html no lo usa: tiene su propio layout)
    ├── butacas.js    ← reserva.html y admin.html
    ├── index.js      ← index.html
    ├── cartelera.js  ← cartelera.html
    ├── reserva.js    ← reserva.html
    └── admin.js      ← admin.html
```

| Página | CSS | Librerías CDN | Nuestros JS (en orden) |
|---|---|---|---|
| `index.html` | style | Bootstrap, SweetAlert2 | api → main → index |
| `cartelera.html` | style | Bootstrap, SweetAlert2 | api → main → cartelera |
| `estrenos.html` | style | Bootstrap, SweetAlert2 | api → main → `<script>` en el HTML |
| `promociones.html` | style | Bootstrap, SweetAlert2 | api → main → `<script>` en el HTML |
| `reserva.html` | style | Bootstrap, SweetAlert2 | api → main → butacas → reserva |
| `admin.html` | style + admin | Bootstrap, SweetAlert2, **Feather**, **Chart.js** | api → butacas → admin |

---

## 5. Recorridos paso a paso

### 5.1 Recorrido 1: Login

**Situación:** estás en `cartelera.html` sin sesión y hacés clic en **Iniciar sesión**.

| # | Qué pasa | HTML | CSS | JS |
|---|---|---|---|---|
| 1 | Carga la página | `<header id="navbarCine">` vacío | — | `main.js:29` (`DOMContentLoaded`) llama a `renderNavbar()`, `inyectarModalesAuth()` y `renderAuthArea()` |
| 2 | Aparecen los botones de la barra | `renderAuthArea` (`main.js:89-94`) mete `<button id="btnLogin" class="btn btn-outline-secondary btn-sm">` | `btn-outline-secondary` de Bootstrap, reconfigurado en `style.css` para que se lea bien en modo oscuro | Conecta `click` en `btnLogin` → `Sesion.pedirLogin()` |
| 3 | Clic en "Iniciar sesión" | — | — | `pedirLogin` (`main.js:23`) → `abrirModal('loginModal')` (`api.js:210`) → `bootstrap.Modal...show()` |
| 4 | Se ve la ventana | El modal (`main.js:124-150`): `<div class="modal fade">` con `<form id="loginForm">`, dos `<input>` y un `<button type="submit">` | Bootstrap: fondo oscurecido, centrado (`modal-dialog-centered`), animación (`fade`). Nuestro: fondo del modal con `--cine-surface` (`style.css:61`) | Bootstrap dispara `shown.bs.modal`; `api.js:217` pone el foco en el campo de email |
| 5 | Escribís y apretás "Ingresar" (o Enter) | El `<form>` dispara `submit` | — | `onLogin(e)` (`main.js:227`): `e.preventDefault()` (no recarga), lee los `.value`, deshabilita el botón |
| 6 | Pedido a la API | — | — | `api('/auth/login', { method:'POST', body:{email, password} })` → `fetch` con `Content-Type: application/json`. Todavía no hay token, así que no va el header `Authorization` |
| 7 | La API responde | — | — | Backend: `AuthController.Login` → `AuthService.LoginAsync` → `TokenService.GenerarToken` (ver [3.3](#33-cómo-se-genera)). Devuelve `200 { token, expira, nombre, rol, idCliente }` |
| 7b | Si la contraseña es incorrecta | `<div id="loginError" class="alert alert-danger" hidden>` deja de estar oculto | `alert-danger` de Bootstrap: recuadro rojo | La API responde **401** con el texto "Email o contraseña incorrectos." → `api()` lanza `Error` con ese texto → el `catch` de `onLogin` llama a `mostrarError('loginError', err.message)` |
| 8 | Login OK | — | — | `despuesDeLoguear` (`main.js:285`): `Auth.guardar(resp)` → **el JWT queda en `localStorage["cine_sesion"]`**; `cerrarModal`; `toast('¡Hola, Juan!')` |
| 9 | Cambia la barra | `renderAuthArea` reemplaza los botones por "👤 Juan Perez · Mis compras · Salir" (y "Panel de administrador" si el rol es admin) | `.usuario-nav`, `.nav-link-admin` (amarillo, `style.css:158`) | Dispara el evento `sesion-cambiada` para que otras partes (ej. `reserva.js`) se enteren |
| 10 | Desde ahora | — | — | Cada `api(...)` agrega `Authorization: Bearer <token>` (`api.js:47`), aunque recargues la página o cambies de página, porque el token está en `localStorage`. Y en cada carga de página, `verificarSesion()` (`api.js:276`) le pregunta a la API con `GET /auth/yo` si el token sigue valiendo; si responde 401, se cierra la sesión y la navbar vuelve a "Iniciar sesión" |

### 5.2 Recorrido 2: de la cartelera al mapa de butacas

**Situación:** entrás a `cartelera.html`, hacés clic en un horario y elegís una butaca.

| # | Qué pasa | HTML | CSS | JS |
|---|---|---|---|---|
| 1 | Carga la cartelera | `cartelera.html:21`: `<div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4 g-4" id="peliculas-container">` vacío | La grilla de Bootstrap decide cuántas tarjetas por fila según el ancho | `cartelera.js`: pone un spinner (`htmlCargando`) y hace `Promise.all` de `GET /pelicula/cartelera` y `GET /funcion?soloProximas=true` (públicos: no necesitan token) |
| 2 | Aparecen las tarjetas | Por cada película: `<div class="col"><article class="card h-100 movie-card">` con `posterHTML(p)`, título, badge y chips `<a class="btn btn-sm btn-outline-secondary rounded-pill" href="reserva.html?id=2&funcion=5">` | `.poster` (proporción 2:3), `.movie-card:hover` (sube y borde amarillo) son nuestros; `card`, `btn`, `badge` son de Bootstrap | Arma todo con `.map(...)` + template strings y lo asigna a `innerHTML` |
| 3 | Clic en un horario | Es un link normal (`<a href>`): el navegador navega a `reserva.html?id=2&funcion=5` | — | Ninguno (navegación nativa del navegador) |
| 4 | Carga la reserva | `reserva.html`: `#peliculaInfo` con un spinner, `#flujoCompra` con `hidden`, tres `<section class="card" id="step1/2/3">` (2 y 3 ocultas) | — | `iniciarReserva()` (`reserva.js:19`): lee `id` y `funcion` con `URLSearchParams`; `Promise.all` de película, funciones de esa película y formas de pago |
| 5 | Paso 1 visible | Encabezado con póster y título; botones de horario `<button class="btn btn-outline-secondary funcion-btn" data-funcion="5">` | `.funcion-btn` (nuestro): ancho mínimo, hora grande | `renderFunciones()` agrupa por día. Como vino `?funcion=5`, llama directo a `elegirFuncion(5)` |
| 6 | Se elige la función | El botón queda con clase `active`; `#step2` deja de estar `hidden` | `.funcion-btn.active` (`style.css:255`): fondo amarillo translúcido y borde amarillo. `[id^="step"] { scroll-margin-top }` evita que la navbar tape el título al scrollear | `elegirFuncion` (`reserva.js:93`): `classList.toggle('active', ...)`, `step2.hidden = false`, `scrollIntoView` |
| 7 | Se pide el estado de la sala | — | — | `cargarMapa()` → `GET /funcion/5/butacas` → lista de `{ idButaca, fila, numero, habilitada, ocupada }` |
| 8 | Se dibuja el mapa | `crearMapaButacas` (`butacas.js:31-54`): `div.sala-mapa` → `div.pantalla` "PANTALLA" → por fila un `div.fila-butacas` con `<button class="seat" data-id="..">` (con `disabled` y `seat--ocupada` si está vendida) → leyenda | `.pantalla` (línea amarilla curva), `.fila-butacas` (flexbox centrado), `.seat` (34×30px, azul `--seat-free`), `.seat--ocupada` (gris, `cursor: not-allowed`) | Conecta `click` solo a las butacas habilitadas (`.seat:not(:disabled)`) |
| 9 | Clic en la butaca B5 | El botón pasa a tener `class="seat seat--seleccionada"` y `aria-pressed="true"` | `.seat.seat--seleccionada` (`style.css:291`): fondo amarillo, letra oscura, `scale(1.08)`. La transición de 0.15s la hace suave | Agrega la butaca al `Map` `seleccionadas` (closure), `classList.toggle(...)`, llama a `onChange` |
| 10 | Aparece "Tus entradas" y el paso 3 | Una fila con "Butaca B5", un `<select>` con "General — $ 4.000 / Jubilado — $ 3.400 (15%)…" y el precio; `#step3` visible con el resumen | Grilla de Bootstrap (`row`, `col-4 col-sm-3`…) para que en celular se apile | `onSeleccionCambiada` → `renderDetalleEntradas` (los precios vienen **calculados por el servidor** en `funcion.precios`) → `actualizarPaso3` → `renderResumen` |
| 11 | "Confirmar compra" | Botón deshabilitado y con texto "Procesando..." | — | `confirmarCompra` (`reserva.js:178`): si no hay sesión, abre el login (Recorrido 1) y después continúa; si hay, `POST /venta` **con el JWT**. La API usa el `idCliente` del token |

### 5.3 Recorrido 3: cargar el gráfico "Películas más vendidas"

**Situación:** sos admin, entrás a `admin.html` y cambiás el filtro a "Top 10".

| # | Qué pasa | HTML | CSS | JS |
|---|---|---|---|---|
| 1 | Antes de dibujar nada | `admin.html:7-16`: `<script>` en el `<head>` | — | Lee `localStorage["cine_sesion"]`; si no hay sesión, venció o el rol no es `Administrador`, `location.replace('index.html?login=1')` |
| 2 | Carga el panel | Layout: `<aside class="offcanvas-lg sidebar">` (menú) + contenido. Dashboard: 4 tarjetas de métricas y dos `card` con `<canvas id="chartTop">` y `<canvas id="chartMensual">` dentro de `div.chart-box` | `admin.css`: `.sidebar` fijo a la izquierda en pantallas ≥992px, `.chart-box` con 280px de alto. Grilla: `col-xl-6` (dos gráficos lado a lado en pantallas grandes, uno abajo del otro en las chicas) | `admin.js:28`: vuelve a chequear `Auth.esAdmin()`, muestra el nombre, `feather.replace()` convierte los `<i data-feather>` en íconos SVG, carga catálogos y llama a `mostrarSeccion('dashboard')` (no hay `#` en la URL) |
| 3 | Pedidos del dashboard | — | — | `cargarDashboard()` (`admin.js:232`): `Promise.all` de `GET /venta/totales`, `GET /funcion?soloProximas=true` y `GET /reporte/anios`, **todos con `Authorization: Bearer`**. Llena las 4 métricas con `textContent` y llama a `cargarTop()` y `cargarMensual()` |
| 4 | Pedido del reporte | — | — | `cargarTop()` (`admin.js:254`): arma `?top=5` (+ `desde`/`hasta` si están cargados) → `GET /reporte/peliculas-mas-vendidas?top=5` |
| 5 | Backend | — | — | `[Authorize(Roles = "Administrador")]` en `ReporteController` valida el token **y** el rol (un cliente recibiría 403) → `ReporteService` valida el top (1-50) y las fechas → `ReporteRepository.GetPeliculasMasVendidasAsync` hace un `GroupBy` por película con LINQ/EF → lista de `PeliculaMasVendidaDTO` en JSON |
| 6 | Tabla | `<tbody id="tablaTop">` se llena con `<tr>` | `.num` (`admin.css`): números a la derecha | Template string por fila con `fmtNumero` y `fmtMoneda` |
| 7 | Gráfico | Chart.js dibuja dentro del `<canvas>` | El canvas ocupa el alto del `.chart-box`. Colores: el amarillo de `--cine-primary` y los grises de `--bs-secondary-color` / `--bs-border-color` | `dibujarGraficoTop()` (`admin.js:329`): `destroy()` del anterior, `coloresGrafico()` lee las variables CSS, `new Chart(canvas, { type: 'bar', data, options: { indexAxis: 'y', ... } })` |
| 8 | Elegís "Top 10" | `<select id="topCantidad">` cambia de valor | — | Evento `change` (`admin.js:225`) → `cargarTop()` otra vez con `?top=10` → tabla y gráfico nuevos |
| 9 | Pasás el mouse por una barra | Chart.js muestra un tooltip | — | El `callback` del tooltip arma "35 entradas · $ 140.000" |
| 10 | Cambiás a tema claro | `<html data-bs-theme="light">` | Se activan las variables del bloque claro | `Tema.aplicar` dispara `tema-cambiado` → `admin.js:69` redibuja los dos gráficos con los colores nuevos |

---

## 6. Cosas que no quedan claras en el código

Puntos donde el código no alcanza para afirmar algo con seguridad, o donde hay detalles que conviene saber:

1. **`System.IdentityModel.Tokens.Jwt`** no está declarado como paquete en ningún `.csproj`: llega de forma transitiva con `Microsoft.AspNetCore.Authentication.JwtBearer`.
2. **Nombres de los claims dentro del token:** en C# se usan `ClaimTypes.Name/Email/Role`, pero en el JWT se escriben como `unique_name`, `email`, `role`, y al validar se vuelven a convertir. Ese mapeo lo hace la librería por defecto; no hay código nuestro que lo configure explícitamente. Se comprobó levantando la API: el token de ejemplo de la sección 3.2 fue aceptado por `GET /api/auth/yo`, que devolvió `nombre`, `email` y `rol` leídos con `ClaimTypes.*`.
3. **Cerrar sesión no invalida el token en el servidor** (es solo borrar el `localStorage`). Es una decisión consciente: el motivo y lo que haría falta para invalidarlo están en la [sección 3.8](#38-logout-por-qué-solo-borra-el-token-del-navegador).
4. **Perfil HTTPS:** los `launchSettings.json` tienen perfiles `https` (puertos 7181 y 7096), pero el front tiene la API escrita con `http://localhost:5191` en `api.js:8`. Si se levantara solo con el perfil HTTPS, igual escucha en 5191 por HTTP, pero en producción (`UseHttpsRedirection`) habría que revisar esa URL.

---

## 7. Glosario

| Término | Significado |
|---|---|
| **API** | Programa que expone operaciones por HTTP para que otros programas (acá, el JS del navegador) las usen |
| **Atributo `data-*`** | Atributo HTML inventado por nosotros para guardar datos (`data-id="5"`); en JS se lee con `elemento.dataset.id` |
| **Base64URL** | Forma de escribir datos binarios con letras, números, `-` y `_`. No es encriptación |
| **Breakpoint** | Ancho de pantalla a partir del cual cambia el diseño (`sm` 576px, `md` 768px, `lg` 992px, `xl` 1200px en Bootstrap) |
| **CDN** | Servidor público que aloja librerías (Bootstrap, Chart.js…) para cargarlas con una URL |
| **Claim** | Un dato dentro del JWT ("rol = Cliente") |
| **Closure** | Función que recuerda variables del lugar donde se creó |
| **CORS** | Regla del navegador: una página de un origen (puerto 5020) solo puede llamar a otro origen (5191) si este lo permite con headers especiales |
| **DOM** | Árbol de objetos que representa el HTML en memoria; JS lo modifica y la pantalla se actualiza |
| **DTO** | Clase que define la forma del JSON que viaja entre front y API |
| **Evento** | Algo que ocurre en la página (clic, submit, scroll) y que JS puede escuchar |
| **`fetch`** | Función del navegador para hacer pedidos HTTP desde JS |
| **Flexbox** | Modo de CSS (`display: flex`) para alinear elementos en fila o columna |
| **JSON** | Formato de texto para datos: `{ "clave": "valor", "lista": [1, 2] }` |
| **JWT** | Token firmado que identifica al usuario en cada pedido |
| **`localStorage`** | Almacenamiento clave→texto del navegador, por sitio, que persiste al cerrar la pestaña |
| **Modal** | Ventana emergente sobre la página (login, formularios) |
| **Promesa** | Objeto que representa un resultado que todavía no llegó (como `Task<T>`); se espera con `await` |
| **Responsive** | Diseño que se adapta al tamaño de la pantalla |
| **Selector** | La parte de una regla CSS que dice a qué elementos se aplica |
| **Template string** | Texto entre comillas invertidas `` `...${variable}...` `` que permite insertar valores y varias líneas |
| **Toast** | Notificación chica que aparece en una esquina y desaparece sola |
