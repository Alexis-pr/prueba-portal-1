# 🎭 Andromeda API

Backend del sistema de gestión de boletería para teatro **Andromeda**, construido con **ASP.NET Core Web API** y **PostgreSQL**.

---

## 📋 Tabla de contenidos

- [Descripción del proyecto](#descripción-del-proyecto)
- [Tecnologías utilizadas](#tecnologías-utilizadas)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Requisitos previos](#requisitos-previos)
- [Configuración del entorno](#configuración-del-entorno)
- [Cambiar contraseña de PostgreSQL](#cambiar-contraseña-de-postgresql)
- [Migraciones](#migraciones)
- [Endpoints disponibles](#endpoints-disponibles)
- [Despliegue en VPS](#despliegue-en-vps)

---

## 📌 Descripción del proyecto

Andromeda es un sistema propio de venta y gestión de boletas para teatro, dividido en 4 portales:

| Portal | Dominio | Descripción |
|--------|---------|-------------|
| 🎟️ Taquilla Virtual | `andromeda.andrescortes.dev` | Portal público para clientes |
| ⚙️ Administrador | `admin.andromeda.andrescortes.dev` | Gestión de eventos y empleados |
| 🏷️ Taquilla Física | `tickets.andromeda.andrescortes.dev` | Ventas presenciales |
| 🚪 Control de Acceso | `access.andromeda.andrescortes.dev` | Escaneo de boletas en puerta |

Este repositorio corresponde al backend del **Portal 1 — Taquilla Virtual**.

---

## 🛠️ Tecnologías utilizadas

- [.NET 10](https://dotnet.microsoft.com/) — Framework principal
- [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core) — API REST
- [Entity Framework Core 8](https://learn.microsoft.com/ef/core/) — ORM
- [PostgreSQL 16+](https://www.postgresql.org/) — Base de datos
- [Npgsql](https://www.npgsql.org/) — Driver PostgreSQL para .NET
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) — Hash de contraseñas
- [JWT Bearer](https://jwt.io/) — Autenticación con tokens
- [Google.Apis.Auth](https://github.com/googleapis/google-api-dotnet-client) — Login con Google

---

## 📁 Estructura del proyecto

```
Andromeda.API/
├── Controllers/
│   └── AuthController.cs         # Endpoints de autenticación
├── Data/
│   └── AppDbContext.cs           # Contexto de base de datos
├── DTOs/
│   └── Auth/
│       ├── AuthResponseDto.cs
│       ├── GoogleAuthRequestDto.cs
│       ├── LoginRequestDto.cs
│       └── RegisterRequestDto.cs
├── Entities/
│   ├── Favorite.cs
│   ├── Order.cs
│   ├── Pqr.cs
│   ├── PqrResponse.cs
│   └── User.cs
├── Helpers/
├── Migrations/                   # Migraciones de EF generadas automáticamente
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── ITokenService.cs
│   ├── AuthService.cs
│   └── TokenService.cs
├── appsettings.json              # Configuración base (sin secretos)
├── appsettings.Development.json  # Configuración local (NO subir a Git)
└── Program.cs                    # Punto de entrada y configuración de servicios
```

---

## ✅ Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [dotnet-ef tools](https://learn.microsoft.com/ef/core/cli/dotnet)

Instalar las herramientas de EF globalmente:

```bash
dotnet tool install --global dotnet-ef
```

---

## ⚙️ Configuración del entorno

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/andromeda-api.git
cd andromeda-api
```

### 2. Crear el archivo de configuración local

Crea el archivo `appsettings.Development.json` en la raíz del proyecto (este archivo **no se sube a Git**):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tickets;Username=cosmos;Password=TU_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "ClaveSuperSecretaDe64CaracteresComoMinimo1234567890ABCDEF",
    "Issuer": "AndromedaAPI",
    "Audience": "AndromedaClient",
    "ExpirationHours": 24
  },
  "GoogleAuth": {
    "ClientId": "TU_GOOGLE_CLIENT_ID"
  }
}
```

> ⚠️ La `SecretKey` debe tener mínimo 32 caracteres para que JWT funcione correctamente.

### 3. Crear el usuario y base de datos en PostgreSQL

Conéctate a PostgreSQL y ejecuta:

```sql
CREATE USER cosmos WITH PASSWORD 'TU_PASSWORD';
CREATE DATABASE tickets OWNER cosmos;
GRANT ALL PRIVILEGES ON DATABASE tickets TO cosmos;
```

---

## 🔑 Cambiar contraseña de PostgreSQL

### En Windows

**1. Abrir PowerShell como administrador**

**2. Editar el archivo `pg_hba.conf` para deshabilitar autenticación temporalmente:**

```powershell
notepad "C:\Program Files\PostgreSQL\18\data\pg_hba.conf"
```

Busca las líneas con `scram-sha-256` y cámbialas a `trust`:

```
# Antes:
host    all    all    127.0.0.1/32    scram-sha-256
host    all    all    ::1/128         scram-sha-256

# Después:
host    all    all    127.0.0.1/32    trust
host    all    all    ::1/128         trust
```

**3. Reiniciar el servicio:**

```powershell
Stop-Service postgresql*
Start-Service postgresql*
```

**4. Conectarse sin contraseña y cambiarla:**

```powershell
cd "C:\Program Files\PostgreSQL\18\bin"
.\psql.exe -U postgres
```

Dentro de psql:

```sql
ALTER USER postgres WITH PASSWORD 'nueva_password';
\q
```

**5. Volver a habilitar autenticación** — edita `pg_hba.conf` de nuevo y cambia `trust` de vuelta a `scram-sha-256`, luego reinicia:

```powershell
Stop-Service postgresql*
Start-Service postgresql*
```

---

### En Linux (VPS)

**1. Cambiar al usuario postgres del sistema:**

```bash
sudo -u postgres psql
```

**2. Dentro de psql cambiar la contraseña:**

```sql
ALTER USER postgres WITH PASSWORD 'nueva_password';
\q
```

> En Linux no necesitas editar `pg_hba.conf` porque el usuario `postgres` del sistema puede conectarse directamente sin contraseña via socket local (`peer` auth).

**Si por alguna razón no puedes entrar**, edita el archivo de configuración:

```bash
sudo nano /etc/postgresql/16/main/pg_hba.conf
```

Busca la línea:
```
local   all   postgres   peer
```

Cámbiala temporalmente a:
```
local   all   postgres   trust
```

Reinicia PostgreSQL:

```bash
sudo systemctl restart postgresql
```

Conéctate y cambia la contraseña:

```bash
psql -U postgres
```

```sql
ALTER USER postgres WITH PASSWORD 'nueva_password';
\q
```

Luego vuelve a cambiar `trust` por `peer` y reinicia de nuevo.

---

## 🗃️ Migraciones

### Crear una nueva migración

```bash
dotnet ef migrations add NombreDeLaMigracion
```

### Aplicar migraciones a la base de datos

```bash
dotnet ef database update
```

### Revertir la última migración

```bash
dotnet ef migrations remove
```

---

## 🚀 Correr el proyecto

```bash
dotnet run
```

La API queda disponible en: `http://localhost:5010`

---

## 📡 Endpoints disponibles

### Autenticación

| Método | Endpoint | Descripción | Auth requerida |
|--------|----------|-------------|----------------|
| `POST` | `/api/auth/register` | Registro con email y contraseña | No |
| `POST` | `/api/auth/login` | Login con email y contraseña | No |
| `POST` | `/api/auth/google` | Login / Registro con Google | No |
| `GET` | `/api/auth/me` | Obtener perfil del usuario autenticado | ✅ Sí |

### Ejemplos de uso

**Registro:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "Andrés Cortés",
  "email": "andres@test.com",
  "password": "Test1234!"
}
```

**Login:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "andres@test.com",
  "password": "Test1234!"
}
```

**Perfil (requiere JWT):**
```http
GET /api/auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Respuesta exitosa de registro/login:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "andres@test.com",
  "fullName": "Andrés Cortés",
  "userType": "customer"
}
```

---

## 🌐 Despliegue en VPS

### 1. Asegurarse de que PostgreSQL en Docker acepte conexiones externas

El contenedor debe estar mapeado a `0.0.0.0` en lugar de `127.0.0.1`:

```bash
docker run -d \
  --name cosmos \
  -e POSTGRES_USER=cosmos \
  -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=tickets \
  -p 0.0.0.0:5432:5432 \
  postgres:16-alpine
```

### 2. Configurar variables de entorno en el servidor

En producción los secretos van como variables de entorno, nunca en archivos:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=tickets;Username=cosmos;Password=password"
export JwtSettings__SecretKey="TuClaveSecretaDe64CaracteresComoMinimo"
```

### 3. Publicar y correr la API

```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet Andromeda.API.dll
```

### 4. Aplicar migraciones en el servidor

```bash
dotnet ef database update
```

---

## 🔒 Seguridad

- Las contraseñas se almacenan hasheadas con **BCrypt**
- Los tokens JWT expiran en **24 horas** (configurable)
- El archivo `appsettings.Development.json` está en `.gitignore` y nunca se sube al repositorio
- En producción todos los secretos van en **variables de entorno**

---

## 📄 Licencia

MIT — Andromeda Theater System © 2026
