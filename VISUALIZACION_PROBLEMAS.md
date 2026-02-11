# 📊 VISUALIZACIÓN DEL ANÁLISIS

## Árbol de Problemas Encontrados

```
VISITAPP (Febrero 2026)
│
├── 🔴 CRÍTICOS (Bloqueantes)
│   ├── Doble esquema BD (legacy + domain)
│   │   └── Impacto: v1 y v2 datos inconsistentes
│   ├── Falta validación permisos (Usuario A ve datos Usuario B)
│   │   └── Impacto: Brecha de seguridad CRÍTICA
│   └── Frontend no funcional (50+ pantallas faltantes)
│       └── Impacto: App no se puede usar
│
├── 🟠 IMPORTANTES (Alta Prioridad)
│   ├── Email Service incompleto
│   │   └── Impacto: Notificaciones no funcionan
│   ├── DTOs sin validaciones
│   │   └── Impacto: Validaciones deficientes
│   ├── API v1 abandonada pero activa
│   │   └── Impacto: Confusión, duplicación
│   ├── Falta abstracción en v1 (AccesoDirecto a DbContext)
│   │   └── Impacto: No testeable, SOLID violado
│   └── CORS inseguro en producción
│       └── Impacto: Vulnerabilidad CSRF
│
├── 🟡 MEJORAS (Importante)
│   ├── Falta AutoMapper
│   │   └── Impacto: Mapeos manuales propenso a errores
│   ├── Repository Pattern incompleto
│   │   └── Impacto: Inconsistencia de acceso a datos
│   ├── Estados de visita no validados (string)
│   │   └── Impacto: Datos inválidos en BD
│   └── Falta auditoría en v1
│       └── Impacto: Incumplimiento HU
│
└── ⚪ FRONTEND - 90% FALTANTE
    ├── Sin componentes UI principales
    ├── Sin servicio de autenticación local
    ├── Sin sincronización offline
    ├── Sin notificaciones push
    ├── Sin gestión de estado
    └── Compilación imposible por dependencias faltantes
```

---

## Estado de Implementación por Componente

### Backend Components

```
Autenticación JWT
├── Generación          ✅✅✅ (bien implementado)
├── Validación          ✅✅ (funcional)
├── Refresh tokens      ⚠️ (no implementado)
└── OAuth 2.0          ⚪ (no implementado)

API Endpoints
├── Auth v2             ✅ (completo)
├── Contacts v1         ⚠️ (sin permisos)
├── Contacts v2         ✅ (con permisos)
├── Visits v1           ⚠️ (sin permisos)
├── Visits v2           ✅ (con permisos)
├── Reports             ✅ (parcial, falta PDF)
├── Audit Logs          ✅ (v2 solamente)
└── Roles/Permissions   ⚠️ (no validados en v1)

Seguridad
├── JWT                 ✅✅✅
├── BCrypt hashing      ✅✅✅
├── CORS                ⚠️ (inseguro en prod)
├── Rate Limiting       ✅
├── Security Headers    ✅
├── Validación roles    ⚠️ (parcial)
└── Validación permisos ⚪ (faltante)

Base de Datos
├── Schema Design       ✅✅✅
├── Relaciones          ✅✅✅
├── Migraciones         ✅ (funcional)
├── Consolidación       ⚪ (legacy + domain)
└── Optimizaciones      ⚠️ (faltarían índices)
```

### Frontend Components

```
UI Screens
├── Login               ⚪ (no existe)
├── Dashboard           ⚪ (no existe)
├── Contacts CRUD       ⚪ (no existe)
├── Visits Calendar     ⚪ (no existe)
├── Profile            ⚪ (no existe)
├── Reports            ⚪ (no existe)
├── Temas              ⏳ (mock/placeholder)
└── Preguntas Clave    ⏳ (mock/placeholder)

Services
├── API HTTP           ⚪ (no existe)
├── Authentication     ⚪ (no existe)
├── Local Storage      ⚪ (no existe)
├── Offline Sync       ⚪ (no existe)
├── Push Notifications ⚪ (no existe)
└── State Management   ⚪ (no existe)

Dependencias
├── HTTP Client        ⚪ (falta Dio/http)
├── State Mgmt         ⚪ (falta Provider)
├── Storage            ⚪ (falta Hive/Prefs)
├── Navigation         ⚪ (falta go_router)
├── PDF Handling       ⚪ (falta pdf/printing)
└── Notifications      ⚪ (falta flutter_local_notifications)
```

---

## Matriz de Impacto vs Esfuerzo

```
ESFUERZO
  ALTO  │                              
        │  Offline Sync (10h)  Calendario (10h)
        │        █                    █
        │
        │    Email (4h)           Reportes (6h)
        │       █                      █
MEDIO   │   AutoMapper (3h)  Validar Roles (2.5h)
        │        █                █
        │
BAJO    │  Instalar Deps (1h)
        │         █
        │_________________________
          BAJO    IMPACTO    ALTO
        
        Prioridad: Superior derecha (Alto impacto, Bajo esfuerzo)
        Después: Superior izquierda (Alto impacto, Alto esfuerzo)
```

---

## Línea de Tiempo Recomendada

```
SEMANA  FASE           TAREAS                                   HORAS
────────────────────────────────────────────────────────────────────
  1     CRÍTICOS       • Consolidar BD
                       • Validar permisos
                       • Validar roles                           11.5
                       
  2     CRÍTICOS       • DTOs completos
       (Continuación)  • Email Service
                       • Deps + ApiService Flutter               11
                       
  3     ARQUITECTURA   • AutoMapper
                       • Repositorios
                       • Migrar v1→v2                            11
                       
  4     ARQUITECTURA   • Auditoría en v1
       (Continuación)  • Auth Service Flutter
                       • Login UI                                11
                       
  5     FRONTEND       • Dashboard (Pastor)
                       • Contactos CRUD
                       • Tests básicos                           12
                       
  6     FRONTEND       • Dashboard (Líder/Admin)
       (Continuación)  • Visitas Calendario
                       • Reportes                                12
                       
  7     OFFLINE/NTF    • Sincronización offline (Hive)
                       • Notificaciones push
                       • Integración total                       12
                       
  8     REFINAMIENTO   • Testing completo
       & QA            • Bug fixes
                       • Documentación                           10
────────────────────────────────────────────────────────────────────
TOTAL: ~91.5 horas (8 semanas, 1-2 desarrolladores)
```

---

## Índice de Salud de Componentes

```
Componente              Antes   Después  Delta   Prioridad
─────────────────────────────────────────────────────────
BD Consolidación        20%  →  100%   +80%    🔴 CRÍTICO
Validación Permisos     0%   →   95%   +95%    🔴 CRÍTICO
Validación Roles        20%  →   95%   +75%    🔴 CRÍTICO
Email Service           30%  →   90%   +60%    🟠 ALTO
DTOs Validación         40%  →   95%   +55%    🟠 ALTO
Frontend UI             0%   →   85%   +85%    🟠 ALTO
AutoMapper              0%   →   95%   +95%    🟡 IMPORTANTE
Repository Pattern      60%  →   95%   +35%    🟡 IMPORTANTE
─────────────────────────────────────────────────────────
PROMEDIO GENERAL        19%  →   82%   +63%
```

---

## Resumen Ejecutivo en Números

| Métrica | Valor | Tendencia |
|---------|-------|-----------|
| **Historias de Usuario Completas** | 8/30 (27%) | 📈 Esperar 25/30 (83%) |
| **Errores Críticos** | 3 | 📉 Esperar 0 |
| **Errores Arquitectónicos** | 6 | 📉 Esperar 1-2 |
| **Brechas de Seguridad** | 3 | 📉 Esperar 0 |
| **Pantallas Implementadas** | 2/10 | 📈 Esperar 10/10 |
| **Tests Pasando** | 30+ | ✅ Mantener 100% |
| **Cobertura de Código (Backend)** | 60% | 📈 Esperar 80%+ |
| **Cobertura de Código (Frontend)** | 0% | 📈 Esperar 70%+ |
| **Documentación Completitud** | 80% | 📈 Esperar 95% |
| **Deuda Técnica** | ALTA | 📉 Esperar BAJA |

---

## Dependencias de Tareas (Camino Crítico)

```
START
  │
  ├─→ [Consolidar BD] ────────────────────┐
  │        (6h)                           │
  │                                       ▼
  ├─→ [Validar Permisos] ────────────────[Tests BD]
  │        (3h)                           │
  │                                       │
  ├─→ [Validar Roles] ───────────────────┤
  │        (2.5h)                         │
  │                                       ▼
  ├─→ [DTOs Completos] ──────────────────[Backend Seguro]
  │        (2h)                           │
  │                                       │
  ├─→ [Email Service] ────────────────────┤
  │        (4h)                           │
  │                                       ▼
  ├─→ [Flutter Deps] ────────────────────[Frontend Ready]
  │        (1h)
  │
  └─→ [ApiService] ──────────────────────[Integración]
         (4h)                            │
                                         ▼
                                    [FASE 1 COMPLETA]
                                    (22.5h trabajo)
                                    [Pasar a Fase 2]
```

---

## Escenarios de Testing Recomendados

### Backend
```gherkin
Scenario: Usuario sin token no puede ver sus contactos
  Given usuario sin autenticación
  When intenta GET /api/contacts
  Then recibe 401 Unauthorized

Scenario: Usuario ve solo sus contactos
  Given usuario autenticado con ID 1
  When realiza GET /api/contacts
  Then recibe solo contactos where UserId = 1

Scenario: Usuario no puede acceder endpoint Admin
  Given usuario con rol "Pastor"
  When intenta POST /api/roles
  Then recibe 403 Forbidden
```

### Frontend
```gherkin
Scenario: Login válido guarda token
  Given usuario en LoginScreen
  When ingresa credenciales correctas
  Then token se guarda en SharedPreferences
  And navega a HomeScreen

Scenario: App funciona offline
  Given sin conexión
  When accede a contactos guardados
  Then ve lista cached
  And puede hacer cambios
  When recupera conexión
  Then cambios se sincronizan

Scenario: Notificación push llega
  Given app con FCM configurado
  When servidor envía notificación
  Then usuario ve notificación local
  And puede clickearla
```

---

## Recursos Necesarios

### Herramientas
- ✅ Visual Studio 2022 / VS Code
- ✅ Android Studio (emulador)
- ✅ SQL Server Management Studio
- ✅ Postman (testing API)
- ✅ Git

### Dependencias Críticas
```
Backend:
✅ .NET 8 SDK
✅ Entity Framework Core 8
✅ AutoMapper
✅ BCrypt

Frontend:
✅ Flutter SDK 3.0+
✅ Dio (HTTP)
✅ Provider (State Mgmt)
✅ Hive (Storage)
✅ firebase_messaging (Push)
```

### Conocimientos Requeridos
- Clean Architecture (.NET)
- Flutter/Dart
- Entity Framework Core
- RESTful APIs
- JWT Authentication
- SQL Server

---

**Documento Completo**: [estado.md](estado.md)  
**Guía de Correcciones**: [GUIA_CORRECCIONES.md](GUIA_CORRECCIONES.md)  
**Resumen Ejecutivo**: [ANALISIS_RESUMEN.md](ANALISIS_RESUMEN.md)
