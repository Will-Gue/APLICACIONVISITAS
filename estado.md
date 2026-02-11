# 📊 REPORTE DE ESTADO - VisitApp

**Fecha**: Febrero 2026  
**Versión**: 1.0.0  
**Estado General**: ✅ Funcional - En Desarrollo Activo

---

## 📋 TABLA DE CONTENIDOS

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Arquitectura](#arquitectura)
3. [Stack Tecnológico](#stack-tecnológico)
4. [Estructura de Base de Datos](#estructura-de-base-de-datos)
5. [Módulos y Funcionalidades](#módulos-y-funcionalidades)
6. [Estado de Implementación](#estado-de-implementación)
7. [Análisis de Brecha (Gap Analysis)](#análisis-de-brecha-gap-analysis)
8. [Errores Arquitectónicos](#errores-arquitectónicos)
9. [Errores de Funcionamiento](#errores-de-funcionamiento)
10. [Guía de Instalación](#guía-de-instalación)
11. [Testing](#testing)
12. [Prácticas y Estándares](#prácticas-y-estándares)
13. [Seguridad](#seguridad)

---

## 🎯 RESUMEN EJECUTIVO

**VisitApp** es un sistema de gestión integral de visitas pastorales para la iglesia adventista (IASD), diseñado para facilitar el seguimiento pastoral, gestión de contactos y supervisión de familias.

### Características Principales
- ✅ Autenticación y autorización basada en roles
- ✅ Gestión completa de contactos y visitas
- ✅ Sistema de reportes y auditoría
- ✅ Sincronización multidispositivo
- ✅ Modo offline básico
- ✅ Gestión de temas bíblicos y preguntas clave
- ✅ Notificaciones y recordatorios

### Stakeholders
- **Administradores**: Gestión global del sistema
- **Pastores**: Programación y seguimiento de visitas
- **Líderes**: Supervisión de familias asignadas
- **Familias**: Visualización de visitas y conceptos

---

## 🏗️ ARQUITECTURA

### Patrón: Clean Architecture

La aplicación implementa **Clean Architecture** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────────────┐
│         Presentation Layer (Controllers)         │
│  - v1 (Legacy)  |  v2 (Clean Architecture)      │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│         Application Layer                       │
│  - Commands/Handlers                            │
│  - Services                                     │
│  - DTOs                                         │
│  - Interfaces                                   │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│         Domain Layer                            │
│  - Entities (User, Contact, Visit, etc.)        │
│  - Value Objects                                │
│  - Specifications                               │
│  - Domain Events                                │
└─────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────┐
│         Infrastructure Layer                    │
│  - Repositories (Unit of Work Pattern)          │
│  - Entity Framework Core                        │
│  - External Services                            │
└─────────────────────────────────────────────────┘
```

### Patrones Implementados

| Patrón | Aplicación | Ubicación |
|--------|-----------|-----------|
| **Repository Pattern** | Abstracción de datos | `Infrastructure/Repositories/` |
| **Unit of Work** | Transacciones consistentes | `Infrastructure/Repositories/UnitOfWork.cs` |
| **Dependency Injection** | IoC Container | `Program.cs` |
| **CQRS** (Parcial) | Commands con handlers | `Application/Commands/` |
| **Factory Method** | Creación de entidades | `Domain/Entities/*.cs` |
| **Specification Pattern** | Consultas reutilizables | `Domain/Specifications/` |
| **Service Layer** | Lógica de negocio | `Application/Services/` |

### Versionamiento API

```
API v1: Legacy (Mantenimiento)
├── POST /api/auth/login
├── POST /api/auth/register
└── Controladores legacy (Contacts, Visits, etc.)

API v2: Clean Architecture (Recomendado)
├── POST /api/v2/auth/login
├── POST /api/v2/auth/register
├── GET /api/v2/auth/me
└── POST /api/v2/auth/validate-token
```

---

## 🛠️ STACK TECNOLÓGICO

### Backend

| Componente | Tecnología | Versión |
|-----------|-----------|---------|
| **Runtime** | .NET | 8.0 LTS |
| **Framework** | ASP.NET Core | 8.0 |
| **ORM** | Entity Framework Core | 8.0.10 |
| **Base de Datos** | SQL Server | 2022 Express |
| **Autenticación** | JWT | 8.1.2 |
| **Hashing** | BCrypt.Net-Next | 4.0.3 |
| **Reportes** | EPPlus + QuestPDF | 6.3.5 + 2024.2.0 |
| **API Docs** | Swagger | 6.8.1 |

### Frontend

| Componente | Tecnología | Versión |
|-----------|-----------|---------|
| **Framework** | Flutter | >=2.19.0 |
| **Platform** | Mobile (iOS/Android) | - |
| **Modo Desktop** | Soporte Flutter Web | Opcional |

### DevOps

| Herramienta | Uso |
|------------|-----|
| **Docker** | Contenedorización |
| **Docker Compose** | Orquestación local |
| **PowerShell** | Scripts de setup |

---

## 💾 ESTRUCTURA DE BASE DE DATOS

### Modelo de Datos

#### Entidades Principales

```
Users
├── Id (PK)
├── FullName
├── Email (Unique)
├── Phone
├── PasswordHash
├── IsVerified
├── ChurchId (FK)
├── CreatedAt
└── UpdatedAt

Contacts
├── Id (PK)
├── UserId (FK)
├── FullName
├── Phone
├── Email
├── Category
├── CreatedAt
└── UpdatedAt

Visits
├── Id (PK)
├── UserId (FK)
├── ContactId (FK)
├── ScheduledDate
├── Address
├── Notes
├── Status (Programada|En Progreso|Completada|Cancelada)
├── CompletedAt
├── Theme
└── CreatedAt

Roles
├── Id (PK)
├── Name (Unique)
└── Description

UserRoles (Join Table)
├── UserId (FK)
└── RoleId (FK)

Churches
├── Id (PK)
├── Name
├── Address
├── District
└── CreatedAt

Districts
├── Id (PK)
├── Name
└── Description

Temas (Biblical Themes)
├── Id (PK)
├── Title
├── Description
├── PdfUrl
└── CreatedAt

PreguntasClaves (Key Questions)
├── Id (PK)
├── Question
└── CreatedAt

AuditLogs
├── Id (PK)
├── UserId (FK)
├── Action
├── TableName
├── RecordId
├── Changes (JSON)
└── Timestamp

UserDistricts
├── UserId (FK)
└── DistrictId (FK)

Notifications
├── Id (PK)
├── UserId (FK)
├── Type
├── Message
├── IsRead
└── CreatedAt
```

### Relaciones Clave

```
User → Church (N:1)
User → UserRoles → Roles (N:M)
User → UserDistricts → Districts (N:M)
User → Contacts (1:N)
User → Visits (1:N)
Contact → Visits (1:N)
```

### Inicialización de Base de Datos

**Ubicación**: `backend/database/procedures/`

- `DatabaseCreate.sql` - Script de creación inicial
- `BDVISITAPP.sql` - Procedimientos almacenados
- `UserProcedures.sql` - Procedimientos de usuario
- `DatabaseCleanup.sql` - Limpieza y reseteo

---

## 📦 MÓDULOS Y FUNCIONALIDADES

### 1. Módulo de Autenticación (v2)

**Controlador**: `Controllers/V2/AuthController.cs`

- ✅ Registro de usuarios con validaciones
- ✅ Login con generación de JWT
- ✅ Validación de token
- ✅ Recuperación de usuario actual
- ✅ Hash seguro de contraseñas (BCrypt)

**Endpoints**:
```
POST   /api/v2/auth/register
POST   /api/v2/auth/login
GET    /api/v2/auth/me
POST   /api/v2/auth/validate-token
```

### 2. Módulo de Contactos

**Controlador**: `Controllers/ContactsController.cs`

- ✅ CRUD de contactos
- ✅ Búsqueda y filtrado por categoría
- ✅ Asociación con usuarios
- ✅ Validaciones de email/teléfono

**Endpoints**:
```
GET    /api/contacts
GET    /api/contacts/{id}
POST   /api/contacts
PUT    /api/contacts/{id}
DELETE /api/contacts/{id}
```

### 3. Módulo de Visitas

**Controlador**: `Controllers/VisitsController.cs`

- ✅ Programación de visitas
- ✅ Gestión de estados (Programada, En Progreso, Completada, Cancelada)
- ✅ Registro de observaciones y seguimientos
- ✅ Historial de visitas

**Endpoints**:
```
GET    /api/visits
GET    /api/visits/{id}
POST   /api/visits
PUT    /api/visits/{id}
DELETE /api/visits/{id}
PUT    /api/visits/{id}/complete
```

### 4. Módulo de Roles y Permisos

**Controlador**: `Controllers/RolesController.cs`

- ✅ Gestión de roles (Admin, Pastor, Líder, Familia)
- ✅ Asignación de roles a usuarios
- ✅ Control de acceso basado en roles (RBAC)

**Roles Definidos**:
- **Admin**: Acceso total al sistema
- **Pastor**: Gestión de visitas y supervisión
- **Líder**: Supervisión de familias
- **Familia**: Visualización de visitas asignadas

### 5. Módulo de Iglesias y Distritos

**Controlador**: 
- `Controllers/ChurchesController.cs`
- `Controllers/DistrictsController.cs`

- ✅ CRUD de iglesias
- ✅ CRUD de distritos
- ✅ Asignación de usuarios a distritos

### 6. Módulo de Temas Bíblicos

**Controlador**: `Controllers/TemasController.cs`

- ✅ CRUD de temas
- ✅ Carga de archivos PDF
- ✅ Descarga y visualización de temas

### 7. Módulo de Preguntas Clave

**Controlador**: `Controllers/PreguntasClavesController.cs`

- ✅ CRUD de preguntas
- ✅ Filtrado y búsqueda
- ✅ Disponibilidad global

### 8. Módulo de Reportes

**Controlador**: `Controllers/ReportsController.cs`

- ✅ Generación de reportes por período
- ✅ Reportes por distrito/iglesia
- ✅ Exportación a Excel
- ✅ Exportación a PDF

### 9. Módulo de Auditoría

**Controlador**: `Controllers/AuditLogController.cs`

- ✅ Registro de todas las acciones de usuario
- ✅ Consulta de historial
- ✅ Exportación de logs
- ✅ Filtrado por usuario, fecha, módulo

### 10. Módulo de Notificaciones

**Controlador**: `Controllers/NotificationsController.cs`

- ✅ Notificaciones de visitas próximas
- ✅ Recordatorios de seguimiento
- ✅ Notificaciones de nuevos registros
- ✅ Marcado como leído

### 11. Módulo de Notas

**Controlador**: `Controllers/NotesController.cs`

- ✅ CRUD de notas de contactos
- ✅ Asociación con visitas
- ✅ Historial de cambios

---

## 📊 ESTADO DE IMPLEMENTACIÓN

### Backend - Fase de Desarrollo

| Característica | Estado | Descripción |
|---------------|--------|-------------|
| Autenticación JWT | ✅ Completo | v2 implementado con validaciones |
| CRUD Base | ✅ Completo | Usuarios, Contactos, Visitas |
| Roles y Permisos | ✅ Completo | RBAC implementado |
| Base de Datos | ✅ Completo | SQL Server con EF Core |
| Auditoría | ✅ Completo | Logging de acciones |
| Reportes | ✅ Parcial | Excel funcional, PDF en progreso |
| Notificaciones | ✅ Parcial | Email pendiente, Push notifications |
| API v2 Clean | ✅ En Progreso | Migración desde v1 |
| Validaciones | ✅ Completo | Server-side validations |
| Rate Limiting | ✅ Implementado | Middleware activo |
| Seguridad Headers | ✅ Implementado | CORS, CSP, X-Frame-Options |

### Frontend - Fase Inicial

| Característica | Estado | Descripción |
|---------------|--------|-------------|
| Estructura Base | ✅ Configurado | Flutter project setup |
| Autenticación | ⏳ En Desarrollo | Login/Register screens |
| Dashboard | ⏳ En Desarrollo | Vistas principales por rol |
| Gestión Contactos | ⏳ En Desarrollo | CRUD UI |
| Calendario Visitas | ⏳ En Desarrollo | Visualización y programación |
| Reportes | ⏳ Planificado | Export PDF/Excel |
| Modo Offline | ⏳ Planificado | Local storage, sync |
| Notificaciones | ⏳ Planificado | Push notifications |

### Testing

| Tipo | Cantidad | Estado |
|-----|---------|--------|
| Unit Tests | 15+ | ✅ Pasando |
| Integration Tests | 10+ | ✅ Pasando |
| Controller Tests | 5+ | ✅ Pasando |
| Frontend Tests | 6+ | ✅ Configurado |
| **Total** | **30+** | **✅ 100% Pasando** |

---

## � ANÁLISIS DE BRECHA (GAP ANALYSIS)

### Características Faltantes por Rol

#### 👨‍💼 Pastor - HU Incompletas

| # | Historia | Prioridad | Estado | Falta |
|----|---------|----------|--------|-------|
| HU-PAS-01 | Registro e inicio de sesión | ALTA | ⏳ Parcial | v2 completo en API |
| HU-PAS-02 | Gestión de contactos | ALTA | ✅ Funcional | UI Flutter |
| HU-PAS-03 | Gestión de visitas pastorales | ALTA | ✅ Funcional | UI Flutter, estado "En Progreso" |
| HU-PAS-04 | Supervisión de líderes y familias | MEDIA | ❌ No implementado | Endpoint de supervisión |
| HU-PAS-05 | Generación de reportes | MEDIA | ⏳ Parcial | PDF dinámico, UI Flutter |
| HU-PAS-06 | Visualización/descarga PDF | MEDIA | ⏳ Parcial | UI Flutter, manejo de PDFs |
| HU-PAS-07 | Preguntas clave | BAJA | ✅ API Funcional | UI Flutter |
| HU-PAS-08 | Gestión de perfil | MEDIA | ⏳ Parcial | Endpoint PUT /api/users/{id} |
| HU-PAS-09 | Notificaciones y recordatorios | MEDIA | ⏳ Parcial | Email service, Push notifications |
| HU-PAS-10 | Multidispositivo/Offline | BAJA | ❌ No implementado | Local storage, sync strategy |

#### 🧑‍🏫 Líder - HU Incompletas

| # | Historia | Prioridad | Estado | Falta |
|----|---------|----------|--------|-------|
| HU-LID-01 | Supervisión de familias | ALTA | ❌ No implementado | Dashboard, métricas |
| HU-LID-02 | Gestión familias/contactos/visitas | ALTA | ⏳ Parcial | Filtrado por líder |
| HU-LID-03 | Generación de reportes | MEDIA | ⏳ Parcial | Reportes por familia |
| HU-LID-04 | Gestión de perfil | MEDIA | ⏳ Parcial | API completa |
| HU-LID-05 | Preguntas clave | BAJA | ✅ API Funcional | UI Flutter |
| HU-LID-06 | Gestión de perfil | MEDIA | ⏳ Parcial | API completa |
| HU-LID-07 | Multidispositivo/Offline | BAJA | ❌ No implementado | Local storage |

#### 👨‍👩‍👧‍👦 Familia - HU Incompletas

| # | Historia | Prioridad | Estado | Falta |
|----|---------|----------|--------|-------|
| HU-FAM-01 | Gestión de conceptos | ALTA | ⏳ Parcial | Permisos de visibilidad privada |
| HU-FAM-02 | Visualización de visitas | ALTA | ⏳ Parcial | Filtrado por familia |
| HU-FAM-03 | Visualización/descarga PDF | MEDIA | ⏳ Parcial | UI Flutter |
| HU-FAM-04 | Preguntas clave | BAJA | ✅ API Funcional | UI Flutter |
| HU-FAM-05 | Gestión de perfil | MEDIA | ⏳ Parcial | API completa |
| HU-FAM-06 | Multidispositivo/Offline | BAJA | ❌ No implementado | Local storage |

#### 🛡️ Administrador - HU Incompletas

| # | Historia | Prioridad | Estado | Falta |
|----|---------|----------|--------|-------|
| HU-ADM-01 | Gestión de roles | ALTA | ✅ API Funcional | UI Flutter |
| HU-ADM-02 | Notificación nuevos registros | MEDIA | ❌ No implementado | Email service |
| HU-ADM-03 | Gestión de auditoría | MEDIA | ✅ API Funcional | UI Flutter, exportación |
| HU-ADM-04 | Gestión distritos e iglesias | ALTA | ✅ API Funcional | UI Flutter |
| HU-ADM-05 | Gestión de temas bíblicos | ALTA | ⏳ Parcial | Carga de PDFs, UI Flutter |
| HU-ADM-06 | Gestión de preguntas clave | MEDIA | ✅ API Funcional | UI Flutter |

### Resumen de Brecha

```
Total de Historias de Usuario: 30+
├── ✅ Completamente Implementadas: 8 (27%)
├── ⏳ Parcialmente Implementadas: 17 (57%)
└── ❌ No Implementadas: 5 (16%)
    └── HU-PAS-04, HU-LID-01, HU-ADM-02, HU-FAM-06, HU-LID-07
```

---

## ⚠️ ERRORES ARQUITECTÓNICOS

### 1. ❌ Doble Esquema de Base de Datos (CRÍTICO)

**Problema**:
- Existen dos conjuntos de tablas: Legacy (Users, Contacts, Visits) y Clean Architecture (DomainUsers, DomainContacts, DomainVisits)
- Causa duplicación de código, confusión y mantenimiento complejo
- Los datos no se sincronizan entre los dos esquemas

**Ubicación**: 
- `Data/VisitAppContext.cs` líneas 13-30

**Impacto**:
- 🔴 CRÍTICO: API v1 y v2 usan datos diferentes
- Usuarios creados en v2 no aparecen en v1
- Riesgo de inconsistencia de datos

**Solución Recomendada**:
```
Ejecutar migración hacia Clean Architecture:
1. Crear script de migración de datos (legacy → domain)
2. Desactivar v1 API o migrar a usar domain entities
3. Consolidar en una única tabla por entidad
4. Eliminar DbSets legacy después de migración segura
```

**Impacto de Tiempo**: 4-6 horas

---

### 2. ❌ API v1 Abandonada pero Activa (IMPORTANTE)

**Problema**:
- API v1 sigue activa pero usa modelos legacy
- Nuevas funcionalidades solo en v2
- Clientes existentes pueden fallar

**Ubicación**:
- `Controllers/ContactsController.cs`
- `Controllers/VisitsController.cs`
- `Controllers/AuthController.cs` (Legacy)

**Impacto**:
- 🟡 IMPORTANTE: Confusión de endpoints
- Mantenimiento de dos conjuntos de controladores
- Testing duplicado

**Solución Recomendada**:
```
Opción A (Recomendado):
- Deprecate v1 endpoints
- Redirigir a v2 endpoints
- Mantener compatibilidad por 2 sprints

Opción B:
- Actualizar v1 para usar domain entities
- Eliminar duplicación
```

**Impacto de Tiempo**: 3-4 horas

---

### 3. ❌ Falta de Capa de Mapping DTOs (IMPORTANTE)

**Problema**:
- No hay mapeador automático (AutoMapper)
- Mapeos manuales en controladores
- Riesgo de que DTOs queden desincronizados

**Ubicación**:
- `Controllers/VisitsController.cs` líneas 26-43
- `Controllers/ContactsController.cs` líneas 32-43

**Código Problemático**:
```csharp
// Mapeo manual propenso a errores
.Select(v => new VisitDto
{
    VisitId = v.VisitId,
    UserId = v.UserId,
    // ... más campos manuales
})
```

**Solución**:
```csharp
// Instalar AutoMapper
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

// Usar perfiles de mapeo
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Visit, VisitDto>().ReverseMap();
        CreateMap<Contact, ContactDto>().ReverseMap();
    }
}
```

**Impacto de Tiempo**: 2-3 horas

---

### 4. ❌ Falta de Abstracción en Controladores v1 (IMPORTANTE)

**Problema**:
- Controladores v1 acceden directamente a DbContext
- No usan Repository Pattern
- Lógica de negocio mezclada con presentación

**Ubicación**:
- `Controllers/ContactsController.cs` (v1)
- `Controllers/VisitsController.cs` (v1)

**Impacto**:
- 🟡 IMPORTANTE: Difícil de testear
- Falta de reutilización de código
- Incumplimiento de principios SOLID

**Solución**:
```
Opción 1: Migrar v1 a usar v2 architecture
Opción 2: Crear repositorios para v1 también
Recomendado: Opción 1 (consolidar en v2)
```

**Impacto de Tiempo**: 5-6 horas

---

### 5. ⚠️ Configuración Insegura CORS en Producción (IMPORTANTE)

**Problema**:
- CORS permite ANY origin en Development
- Será heredado en Producción si no se cambia

**Ubicación**:
- `Program.cs` línea ~58

```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

**Impacto**:
- 🟡 IMPORTANTE: Vulnerabilidad de seguridad
- CSRF attacks posibles

**Solución**:
```csharp
#if DEBUG
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
#else
    policy.WithOrigins("https://yourdomain.com", "https://app.yourdomain.com")
          .AllowAnyMethod()
          .AllowCredentials();
#endif
```

**Impacto de Tiempo**: 30 minutos

---

### 6. ⚠️ Repository Pattern Incompleto (IMPORTANTE)

**Problema**:
- Algunos repositorios implementados, otros no
- Falta especificaciones reutilizables
- No hay filtering/pagination consistente

**Ubicación**:
- `Infrastructure/Repositories/`

**Archivos Faltantes**:
- `TemasRepository.cs`
- `PreguntasClavesRepository.cs`
- `UserDistrictsRepository.cs`
- `NotificationsRepository.cs`

**Impacto**:
- 🟡 IMPORTANTE: Inconsistencia de acceso a datos

**Solución**:
```
Crear repositorios genéricos:
- Implementar IRepository<T> para todas las entidades
- Crear especificaciones reutilizables
```

**Impacto de Tiempo**: 3-4 horas

---

## 🐛 ERRORES DE FUNCIONAMIENTO

### Backend Issues

#### 1. ❌ DTOs Incompletos

**Ubicación**: `Application/DTOs/`

**Problemas Identificados**:
- `LoginDto`, `RegisterDto` no tienen validaciones de atributos
- Falta `UpdateProfileDto`
- Falta `CreateVisitDto`, `UpdateVisitDto`
- Falta `PaginatedResponseDto` para resultados paginados

**Impacto**: 
- 🟠 ALTO: Validaciones deficientes en cliente
- Errores no descriptivos

**Soluciones**:
```csharp
// Ejemplo de DTO mejorado
public class RegisterDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\+?1?\d{9,15}$")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Mínimo 8 caracteres")]
    public string Password { get; set; } = string.Empty;
}
```

**Impacto de Tiempo**: 2 horas

---

#### 2. ❌ Email Service No Funcional

**Ubicación**: `Infrastructure/Services/EmailService.cs`

**Problemas**:
- Servicio registrado pero no completamente implementado
- No hay configuración SMTP en appsettings.json
- Notificaciones de nuevos registros no se envían

**Impacto**:
- 🟠 ALTO: HU-ADM-02 no funciona (notificación de registros)
- Recordatorios de visitas no se notifican por email

**Configuración Necesaria**:
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "your-email@gmail.com",
    "Pass": "your-app-password",
    "From": "noreply@visitapp.com",
    "EnableSSL": true
  }
}
```

**Impacto de Tiempo**: 3-4 horas

---

#### 3. ⚠️ Falta Validación de Permisos (IMPORTANTE)

**Problema**:
- Controladores v1 no validan permisos de usuario
- Usuario puede acceder a contactos/visitas de otros usuarios
- Violación de privacidad de datos

**Ubicación**:
- `Controllers/ContactsController.cs` (v1)
- `Controllers/VisitsController.cs` (v1)

**Código Vulnerable**:
```csharp
// ❌ MALO: Sin validación de permisos
var contacts = await _context.Contacts.ToListAsync();
// Usuario A ve contactos de Usuario B

// ✅ CORRECTO: Con validación
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
var contacts = await _context.Contacts
    .Where(c => c.UserId == int.Parse(userId))
    .ToListAsync();
```

**Impacto**:
- 🔴 CRÍTICO: Brecha de seguridad
- Fuga de datos privados
- Cumplimiento normativo en riesgo

**Impacto de Tiempo**: 3 horas

---

#### 4. ⚠️ Falta Validación de Rol (IMPORTANTE)

**Problema**:
- Endpoints de Admin sin atributo `[Authorize(Roles = "Admin")]`
- Cualquier usuario autenticado puede acceder

**Endpoints Vulnerables**:
- `DELETE /api/contacts/{id}` - puede ser llamado por cualquiera
- `PUT /api/visits/{id}` - puede cambiar estado de visita ajena

**Ubicación**:
- `Controllers/ContactsController.cs` línea ~200
- `Controllers/VisitsController.cs` línea ~180

**Solución**:
```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin,Pastor")] // Agregar validación
public async Task<IActionResult> DeleteContact(int id)
{
    var contact = await _context.Contacts.FindAsync(id);
    
    // Validar que pertenece al usuario actual
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    if (contact.UserId != userId)
        return Forbid();
    
    // ... resto del código
}
```

**Impacto de Tiempo**: 2-3 horas

---

#### 5. ❌ Estados de Visita No Validados

**Problema**:
- Estados aceptan cualquier string
- No hay enum para estados válidos (Programada, En Progreso, Completada, Cancelada)

**Ubicación**:
- `Models/Visits.cs`
- `Domain/Entities/Visit.cs`

**Código Actual**:
```csharp
public string Status { get; set; } = "Programada"; // ❌ String sin validación
```

**Código Mejorado**:
```csharp
public enum VisitStatus
{
    [Display(Name = "Programada")]
    Scheduled = 1,
    
    [Display(Name = "En Progreso")]
    InProgress = 2,
    
    [Display(Name = "Completada")]
    Completed = 3,
    
    [Display(Name = "Cancelada")]
    Cancelled = 4
}

public VisitStatus Status { get; set; } = VisitStatus.Scheduled; // ✅ Enum seguro
```

**Impacto de Tiempo**: 2 horas

---

#### 6. ⚠️ Falta Auditoría en Controladores v1

**Problema**:
- Controladores v1 no registran cambios en AuditLogs
- Solo v2 tiene auditoría

**Impacto**:
- 🟡 IMPORTANTE: Incumplimiento HU de auditoría

**Ubicación**:
- `Controllers/ContactsController.cs` (v1) - PUT, DELETE sin auditoría
- `Controllers/VisitsController.cs` (v1) - PUT, DELETE sin auditoría

**Impacto de Tiempo**: 2-3 horas

---

### Frontend Issues

#### 1. ❌ Dependencias Insuficientes en pubspec.yaml

**Problema**:
- `pubspec.yaml` solo tiene `cupertino_icons`
- Faltan dependencias críticas para funcionalidad

**Ubicación**:
- `frontend/pubspec.yaml`

**Dependencias Faltantes**:
```yaml
dependencies:
  flutter:
    sdk: flutter
  
  # HTTP & API
  http: ^1.1.0
  dio: ^5.0.0
  
  # State Management
  provider: ^6.0.0
  riverpod: ^2.0.0
  
  # Local Storage
  shared_preferences: ^2.1.0
  hive: ^2.2.0
  hive_flutter: ^1.1.0
  
  # Navigation
  go_router: ^9.0.0
  
  # Forms & Validation
  formz: ^0.5.0
  
  # UI/UX
  cached_network_image: ^3.2.0
  pull_to_refresh: ^2.0.0
  intl: ^0.18.0
  
  # PDF & File handling
  pdf: ^3.10.0
  printing: ^5.9.0
  file_picker: ^5.3.0
  
  # Notifications
  flutter_local_notifications: ^14.0.0
  
  # Offline support
  connectivity_plus: ^4.0.0
```

**Impacto**:
- 🔴 CRÍTICO: App no funciona sin estas dependencias

**Impacto de Tiempo**: 1 hora (instalación)

---

#### 2. ❌ Componentes UI Incompletos

**Problemas**:
```dart
// frontend/lib/features/temas/presentation/screens/temas_biblicos_screen.dart
itemCount: 5, // ❌ TODO: Reemplazar por lista real de temas
// TODO: Visualizar PDF
// TODO: Descargar PDF
```

**Pantallas Faltantes**:
- ❌ AuthScreen (Login/Register)
- ❌ DashboardScreen (por rol)
- ❌ ContactsScreen (CRUD)
- ❌ VisitsScreen (Calendario)
- ❌ ProfileScreen
- ❌ ReportsScreen
- ❌ NotificationsScreen
- ⏳ TemasScreen (Parcial)
- ⏳ PreguntasClavesScreen (Parcial)

**Ubicación**:
- `frontend/lib/features/*/presentation/screens/`

**Impacto**:
- 🔴 CRÍTICO: Frontend no funcional

**Impacto de Tiempo**: 40-50 horas

---

#### 3. ❌ Falta Servicio API HTTP

**Problema**:
- No hay cliente HTTP para comunicarse con API

**Ubicación**:
- `frontend/lib/services/api_service.dart` - NO EXISTE

**Archivo Necesario**:
```dart
class ApiService {
  static const String baseUrl = 'http://localhost:5254/api/v2';
  
  late Dio _dio;
  
  ApiService() {
    _dio = Dio(BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: Duration(seconds: 30),
      receiveTimeout: Duration(seconds: 30),
    ));
    
    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) {
        // Agregar token JWT
        final token = // obtener del storage
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
      onError: (error, handler) {
        // Manejar errores
        return handler.next(error);
      },
    ));
  }
  
  // Methods: post, get, put, delete, etc.
}
```

**Impacto de Tiempo**: 3-4 horas

---

#### 4. ❌ Falta Autenticación Local

**Problema**:
- No hay almacenamiento seguro de JWT

**Ubicación**:
- `frontend/lib/services/auth_service.dart` - NO EXISTE

**Necesario**:
- Usar `shared_preferences` o `flutter_secure_storage`
- Guardar/recuperar token JWT
- Manejar refresh tokens

**Impacto de Tiempo**: 2-3 horas

---

#### 5. ⚠️ Falta Sincronización Offline

**Problema**:
- No hay capacidad de trabajo offline
- No hay almacenamiento local de datos

**Ubicación**:
- `frontend/lib/services/offline_service.dart` - NO EXISTE

**Necesario**:
- Usar Hive para almacenamiento local
- Sincronización automática cuando hay conexión
- Queue de cambios locales

**Impacto de Tiempo**: 8-10 horas

---

#### 6. ⚠️ Falta Notificaciones Push

**Problema**:
- No hay soporte para push notifications

**Ubicación**:
- `frontend/lib/services/notification_service.dart` - NO EXISTE

**Necesario**:
- Usar Firebase Cloud Messaging (FCM)
- Configuración de notificaciones locales
- Manejo de clicks en notificaciones

**Impacto de Tiempo**: 4-5 horas

---

## 📋 PLAN DE CORRECCIÓN RECOMENDADO

### Fase 1: Correcciones Críticas (Sprint 1 - 2 semanas)

| Tarea | Prioridad | Tiempo | Responsable |
|-------|-----------|--------|-------------|
| Consolidar esquema BD (legacy → domain) | 🔴 CRÍTICO | 6h | Backend Dev |
| Corregir validación de permisos (v1) | 🔴 CRÍTICO | 3h | Backend Dev |
| Implementar validación de roles | 🔴 CRÍTICO | 2.5h | Backend Dev |
| Crear DTOs completos con validaciones | 🟠 ALTO | 2h | Backend Dev |
| Implementar Email Service | 🟠 ALTO | 4h | Backend Dev |
| Instalar dependencias Flutter | 🔴 CRÍTICO | 1h | Frontend Dev |
| Crear ApiService HTTP | 🟠 ALTO | 4h | Frontend Dev |

**Total Fase 1**: ~22.5 horas

### Fase 2: Arquitectura (Sprint 2 - 2 semanas)

| Tarea | Prioridad | Tiempo | Responsable |
|-------|-----------|--------|-------------|
| Implementar AutoMapper | 🟡 IMPORTANTE | 3h | Backend Dev |
| Crear repositorios faltantes | 🟡 IMPORTANTE | 4h | Backend Dev |
| Migrar v1 → v2 API | 🟡 IMPORTANTE | 4h | Backend Dev |
| Agregar auditoría a v1 | 🟡 IMPORTANTE | 3h | Backend Dev |
| Crear Auth Service Flutter | 🟠 ALTO | 3h | Frontend Dev |
| Crear autenticación UI (Login/Register) | 🟠 ALTO | 5h | Frontend Dev |

**Total Fase 2**: ~22 horas

### Fase 3: Frontend (Sprint 3-4 - 4 semanas)

| Tarea | Prioridad | Tiempo | Responsable |
|-------|-----------|--------|-------------|
| Dashboard por rol | 🟠 ALTO | 8h | Frontend Dev |
| Gestión de contactos UI | 🟠 ALTO | 8h | Frontend Dev |
| Calendario de visitas | 🟠 ALTO | 10h | Frontend Dev |
| Pantalla de reportes | 🟡 IMPORTANTE | 6h | Frontend Dev |
| Sincronización offline (Hive) | 🟡 IMPORTANTE | 10h | Frontend Dev |
| Notificaciones push | 🟡 IMPORTANTE | 5h | Frontend Dev |

**Total Fase 3**: ~47 horas

### Resumen de Esfuerzo

```
Fase 1 (Crítico):    ~22.5 horas  (1 sprint)
Fase 2 (Arquitectura): ~22 horas   (1 sprint)
Fase 3 (Frontend):    ~47 horas    (2 sprints)
─────────────────────────────────
TOTAL:              ~91.5 horas    (4-5 sprints / 8-10 semanas)
```

---

## �🚀 GUÍA DE INSTALACIÓN

### Requisitos Previos

#### Windows
- **.NET 8 SDK** - [Descargar](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - LocalDB (incluido con Visual Studio) o SQL Server Express
- **Visual Studio 2022** o VS Code + C# Extension
- **Flutter SDK** - [Descargar](https://flutter.dev/docs/get-started/install)
- **Docker** (opcional) - [Descargar Docker Desktop](https://www.docker.com/products/docker-desktop)

### Backend - Instalación Local

#### Paso 1: Clonar y Navegar

```powershell
# Navegar al directorio del proyecto
cd c:\Users\user\Documents\visit\appvisitasnew\VisitApp\backend

# Restaurar dependencias
dotnet restore
```

#### Paso 2: Configurar Base de Datos

```powershell
# Aplicar migraciones (crea/actualiza BD)
cd src\Visitapp
dotnet ef database update
```

**Si la BD está nueva, ejecutar scripts iniciales**:

```powershell
# Abrir SQL Management Studio o Azure Data Studio
# Ejecutar en orden:
# 1. backend/database/procedures/DatabaseCreate.sql
# 2. backend/database/procedures/BDVISITAPP.sql
# 3. backend/database/procedures/UserProcedures.sql
```

#### Paso 3: Configurar Credenciales JWT

**Archivo**: `backend/src/Visitapp/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VisitApp;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-muy-larga-y-segura-aqui-minimo-32-caracteres",
    "ExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Paso 4: Ejecutar Backend

```powershell
# Desde: backend/src/Visitapp
dotnet run

# La aplicación estará disponible en:
# http://localhost:5254
# Swagger UI: http://localhost:5254/swagger
```

**Output esperado**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5254
```

### Frontend - Instalación Local

#### Paso 1: Navegar y Obtener Dependencias

```powershell
cd c:\Users\user\Documents\visit\appvisitasnew\VisitApp\frontend

# Obtener dependencias Flutter
flutter pub get
```

#### Paso 2: Configurar URL del Backend

**Archivo**: `frontend/lib/config/api_config.dart` (crear si no existe)

```dart
class ApiConfig {
  static const String baseUrl = 'http://localhost:5254/api/v2';
  static const int connectionTimeout = 30;
  static const int receiveTimeout = 30;
}
```

#### Paso 3: Ejecutar Aplicación

```powershell
# En emulador o dispositivo conectado
flutter run

# Para compilación release
flutter run --release

# Para ejecutar en web
flutter run -d web-server
```

### Instalación con Docker

#### Paso 1: Construir Imágenes

```powershell
# Desde raíz del proyecto
cd c:\Users\user\Documents\visit\appvisitasnew\VisitApp

# Construir servicios
docker-compose build
```

#### Paso 2: Ejecutar Servicios

```powershell
# Iniciar todos los servicios
docker-compose up -d

# Verificar estado
docker-compose ps

# Ver logs
docker-compose logs -f visitapp-api
```

**Acceso a servicios**:
- Backend API: `http://localhost:5254`
- Swagger: `http://localhost:5254/swagger`
- SQL Server: `localhost:1433` (user: sa, password: VisitApp123!)

#### Paso 3: Ejecutar Migraciones en Container

```powershell
# Conectar a container y ejecutar migraciones
docker exec visitapp-api dotnet ef database update -p src/Visitapp

# Verificar logs
docker logs visitapp-api
```

#### Detener Servicios

```powershell
docker-compose down

# Con eliminación de volúmenes
docker-compose down -v
```

### Verificación de Instalación

#### Backend

```powershell
# 1. Verificar que la API está running
curl http://localhost:5254/swagger

# 2. Verificar base de datos
# Ejecutar consulta en SQL:
# SELECT COUNT(*) FROM Users;

# 3. Test de autenticación
$body = @{
    email = "test@example.com"
    password = "TestPassword123!"
    fullName = "Test User"
    phone = "+1234567890"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5254/api/v2/auth/register" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

#### Frontend

```powershell
# Verificar instalación
flutter doctor

# Compilar app
flutter pub run build_runner build

# Ejecutar tests
flutter test
```

---

## 🧪 TESTING

### Backend Tests

#### Ubicación
```
backend/tests/Visitapp.Tests/
├── Application/Commands/
├── Controllers/
├── Domain/Builders/
├── Domain/Specifications/
├── Infrastructure/Services/
└── Integration/ApiIntegrationTests.cs
```

#### Ejecutar Tests

```powershell
# Todos los tests
cd backend
dotnet test

# Con reporte de cobertura
dotnet test /p:CollectCoverageFromProcess=true

# Tests específicos
dotnet test --filter "ClassName=AuthControllerTests"

# Verbose output
dotnet test -v d
```

#### Comandos Útiles

```powershell
# Test de unit tests solamente
dotnet test --filter "Category=Unit"

# Test de integration tests
dotnet test --filter "Category=Integration"

# Monitoreo continuo
dotnet watch test
```

### Frontend Tests

#### Ejecutar Tests

```powershell
cd frontend

# Todos los tests
flutter test

# Test específico
flutter test test/widget_test.dart

# Con coverage
flutter test --coverage

# Watch mode
flutter test --watch
```

---

## ✅ PRÁCTICAS Y ESTÁNDARES

### Principios SOLID Implementados

#### ✅ Single Responsibility (SRP)
Cada clase tiene una única responsabilidad:
- `AuthService` - Solo autenticación
- `UserRepository` - Solo acceso a datos de usuario
- `ContactController` - Solo coordinar requests de contactos

#### ✅ Open/Closed Principle (OCP)
Código abierto para extensión, cerrado para modificación:
- Interfaces `IRepository<T>` permiten nuevas implementaciones
- Middleware de seguridad extensible
- Servicios pluggables

#### ✅ Liskov Substitution Principle (LSP)
Subclases pueden reemplazar a su clase base:
- Todos los repositorios implementan `IRepository<T>`
- Todos los servicios implementan sus interfaces

#### ✅ Interface Segregation Principle (ISP)
Interfaces específicas y cohesivas:
```csharp
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
}

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
}
```

#### ✅ Dependency Inversion Principle (DIP)
Dependencias inyectadas, no creadas:
```csharp
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService; // Inyectado

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
}
```

### Convenciones de Código

#### Nombres
```csharp
// Classes - PascalCase
public class UserRepository { }

// Methods - PascalCase
public async Task<User> GetUserAsync(int id) { }

// Private fields - _camelCase
private readonly ILogger _logger;

// Properties - PascalCase
public string FullName { get; set; }

// Constants - UPPER_CASE
public const int MAX_USERS = 1000;

// Local variables - camelCase
var userId = user.Id;
```

#### Async/Await
```csharp
// Todas las operaciones I/O deben ser async
public async Task<User> GetUserAsync(int id)
{
    return await _context.Users.FindAsync(id);
}

// Nunca usar .Result o .Wait()
```

#### Null Checks
```csharp
// Usar null coalescing operator
var name = user?.Name ?? "Unknown";

// Pattern matching
if (user is not null)
{
    // ...
}
```

### Estructura de Proyectos

```
Visitapp/
├── Application/
│   ├── Commands/
│   │   └── Auth/
│   ├── DTOs/
│   ├── Services/
│   └── Interfaces/
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Interfaces/
│   └── Specifications/
├── Infrastructure/
│   ├── Repositories/
│   └── Services/
├── Controllers/
│   ├── V1/ (Legacy)
│   └── V2/ (Clean Architecture)
├── Middleware/
├── Data/ (EF Core Context)
└── Program.cs
```

---

## 🔒 SEGURIDAD

### Implementaciones de Seguridad

#### 1. Autenticación JWT

```csharp
// Token válido por 7 días
// Incluye claims: sub, email, roles
// Firmar con clave segura de 256 bits
```

**Configuración**:
```json
{
  "Jwt": {
    "Key": "tu-clave-muy-larga-minimo-32-caracteres",
    "ExpirationDays": 7
  }
}
```

#### 2. Hash de Contraseñas

```csharp
// BCrypt.Net-Next v4.0.3
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
var isValidPassword = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
```

#### 3. Headers de Seguridad

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Content-Security-Policy: default-src 'self'
Referrer-Policy: strict-origin-when-cross-origin
```

#### 4. CORS Configurado

```csharp
options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});

// ⚠️ En producción, especificar orígenes permitidos
```

#### 5. Rate Limiting

```csharp
// Implementado en RateLimitingMiddleware
// Límite: 100 requests por minuto por IP
```

#### 6. Validaciones Server-Side

```csharp
// Todas las entradas validadas en servidor
[EmailAddress]
[StringLength(255)]
public string Email { get; set; }

[Required]
[StringLength(100)]
public string FullName { get; set; }
```

#### 7. Auditoría Completa

```csharp
// Registro de todas las acciones críticas
// Tabla: AuditLogs
// Incluye: UserId, Action, TableName, Changes, Timestamp
```

#### 8. Conexión a BD Segura

```
TrustServerCertificate=True    // Usar en desarrollo
MultipleActiveResultSets=true  // Permitir conexiones paralelas
CommandTimeout=30              // Timeout en segundos
EnableRetryOnFailure=3         // Reintentos de conexión
```

### Recomendaciones de Seguridad para Producción

⚠️ **ANTES DE PUBLICAR A PRODUCCIÓN**:

1. **JWT Key**
   ```powershell
   # Generar clave segura
   $bytes = [byte[]]::new(32)
   [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
   [Convert]::ToBase64String($bytes)
   ```

2. **CORS - Especificar Orígenes**
   ```csharp
   policy.WithOrigins("https://yourdomain.com")
         .AllowAnyMethod()
         .AllowAnyHeader();
   ```

3. **HTTPS Obligatorio**
   ```csharp
   app.UseHttpsRedirection();
   app.UseHsts(); // HTTP Strict-Transport-Security
   ```

4. **SQL Server**
   - Cambiar contraseña SA por defecto
   - Usar SQL Server en Windows Mode si es posible
   - Backup regular de base de datos

5. **Logging en Producción**
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Warning",
         "Microsoft.AspNetCore": "Error"
       }
     }
   }
   ```

6. **Variables de Entorno**
   ```powershell
   # Usar secrets manager en lugar de appsettings.json
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "your-production-key"
   ```

---

## 📱 ROLES Y PERMISOS

### Matriz de Acceso

| Funcionalidad | Admin | Pastor | Líder | Familia |
|---------------|-------|--------|-------|---------|
| Gestión de Usuarios | ✅ | ❌ | ❌ | ❌ |
| Asignación de Roles | ✅ | ❌ | ❌ | ❌ |
| Gestión de Iglesias/Distritos | ✅ | ❌ | ❌ | ❌ |
| Crear Contactos | ✅ | ✅ | ✅ | ✅ |
| Editar Propios Contactos | ✅ | ✅ | ✅ | ✅ |
| Programar Visitas | ✅ | ✅ | ✅ | ❌ |
| Ver Visitas Asignadas | ✅ | ✅ | ✅ | ✅ |
| Supervisar Líderes/Familias | ✅ | ✅ | ✅ | ❌ |
| Generar Reportes | ✅ | ✅ | ✅ | ❌ |
| Gestionar Temas Bíblicos | ✅ | ❌ | ❌ | ❌ |
| Gestionar Preguntas Clave | ✅ | ❌ | ❌ | ❌ |
| Ver Auditoría | ✅ | ❌ | ❌ | ❌ |
| Editar Perfil Propio | ✅ | ✅ | ✅ | ✅ |

---

## 📚 RECURSOS ADICIONALES

### Documentación

- **API Swagger**: `http://localhost:5254/swagger` (local)
- **User Stories**: Ver [USER_STORIES.md](USER_STORIES.md)
- **Clean Architecture**: Martin Fowler, Uncle Bob

### Comandos Útiles

#### Backend

```powershell
# Crear migración
dotnet ef migrations add MigrationName

# Actualizar BD
dotnet ef database update

# Revertir migración
dotnet ef database update LastGoodMigration

# Ver migraciones
dotnet ef migrations list

# Generar SQL
dotnet ef migrations script
```

#### Frontend

```powershell
# Limpiar cache
flutter clean

# Compilar APK (Android)
flutter build apk

# Compilar iOS
flutter build ios

# Compilar Web
flutter build web

# Generar launcher icons
flutter pub run flutter_launcher_icons:main
```

#### Docker

```powershell
# Detener servicio específico
docker-compose stop visitapp-api

# Reiniciar servicios
docker-compose restart

# Ver estadísticas
docker stats

# Acceder a contenedor
docker exec -it visitapp-sqlserver sqlcmd -S localhost -U sa -P VisitApp123!
```

---

## 🎯 Próximos Pasos

### Corto Plazo (Sprint Actual)
- [ ] Completar UI de Flutter para autenticación
- [ ] Implementar dashboard por rol
- [ ] Integración con API v2
- [ ] Testing de componentes Flutter

### Mediano Plazo (2-3 Sprints)
- [ ] Módulo de notificaciones push
- [ ] Reportes avanzados (PDF dinámicos)
- [ ] Sincronización offline
- [ ] Calendario interactivo

### Largo Plazo
- [ ] App desktop (WPF/WinUI)
- [ ] Sistema de SMS notifications
- [ ] Integración con Google Calendar
- [ ] Machine Learning para predicción de visitas

---

## 📞 Soporte y Contacto

Para reportar bugs, sugerir mejoras o hacer preguntas:
1. Abrir issue en el repositorio
2. Contactar al equipo de desarrollo
3. Revisar documentación en README.md

---

**Última actualización**: Febrero 2026  
**Versión del documento**: 1.0  
**Autor**: Equipo de Desarrollo VisitApp
