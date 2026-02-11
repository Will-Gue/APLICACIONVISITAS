# VisitApp - Sistema de Gestión de Visitas Pastorales

## 🏗️ Arquitectura

**Clean Architecture** implementada con:
- **Domain Layer**: Entidades, interfaces, enums
- **Application Layer**: Servicios de aplicación, DTOs, interfaces
- **Infrastructure Layer**: Repositorios, servicios externos
- **Presentation Layer**: Controladores API

## 🛠️ Tecnologías

- **.NET 8** - Framework backend
- **Entity Framework Core** - ORM
- **SQL Server LocalDB** - Base de datos
- **JWT** - Autenticación
- **Flutter** - Frontend móvil
- **Swagger** - Documentación API

## 📋 Funcionalidades

### Autenticación (v2 - Clean Architecture)
- Registro de usuarios con validaciones
- Login con JWT tokens
- Gestión de perfiles
- Validación de tokens

### Gestión de Contactos
- CRUD completo de contactos
- Categorización de feligreses
- Búsqueda y filtrado

### Programación de Visitas
- Calendario de visitas
- Estados de visita (Programada, En Progreso, Completada, Cancelada)
- Notas y seguimiento

## 🚀 Inicio Rápido

### Backend
```bash
cd visitApp/Visitapp
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend
```bash
cd Visit_app/visit_app_flutter
flutter pub get
flutter run
```

## 📊 Testing

- **Backend**: 25+ tests (unitarios + integración)
- **Frontend**: 6 tests (widget + unitarios)
- **Total**: 30+ tests - 100% pasando
- **Cobertura**: 85%+ estimada

```bash
# Backend tests
dotnet test

# Frontend tests
flutter test
```

## 🔗 API Endpoints

### v2 (Clean Architecture)
- `POST /api/v2/auth/login` - Autenticación
- `POST /api/v2/auth/register` - Registro
- `GET /api/v2/auth/me` - Usuario actual
- `POST /api/v2/auth/validate-token` - Validar token

### v1 (Legacy)
- `POST /api/auth/login` - Login legacy
- `POST /api/auth/register` - Registro legacy

## 📖 Documentación

- **Swagger UI**: `http://localhost:5254/swagger`
- **API v1**: Endpoints legacy
- **API v2**: Clean Architecture endpoints

## 🏛️ Principios SOLID

✅ **Single Responsibility**: Cada clase tiene una responsabilidad única  
✅ **Open/Closed**: Extensible sin modificar código existente  
✅ **Liskov Substitution**: Interfaces correctamente implementadas  
✅ **Interface Segregation**: Interfaces específicas y cohesivas  
✅ **Dependency Inversion**: Dependencias invertidas con DI  

## 🎯 Patrones Implementados

- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work**: Transacciones consistentes
- **Service Layer**: Lógica de negocio separada
- **Factory Pattern**: Creación de entidades
- **DTO Pattern**: Transferencia de datos
- **Dependency Injection**: Inversión de dependencias
- **Command Pattern**: Comandos con handlers
- **Mediator Pattern**: Desacoplamiento de controladores

## 🔒 Seguridad

✅ **Autenticación JWT** con tokens seguros  
✅ **Rate Limiting** para prevenir ataques de fuerza bruta  
✅ **Security Headers** (XSS, CSRF, Clickjacking protection)  
✅ **Password Hashing** con BCrypt  
✅ **Validación robusta** de entrada  
✅ **CORS** configurado apropiadamente  

## ⚡ Rendimiento

✅ **Memory Caching** para datos frecuentes  
✅ **Response Compression** (Gzip)  
✅ **Database Indexing** para consultas optimizadas  
✅ **Connection Pooling** mejorado  
✅ **Kestrel optimizado** para mejor throughput  

## 📱 Responsividad

✅ **Diseño adaptativo** para móvil, tablet y desktop  
✅ **Breakpoints responsivos** (600px, 1024px)  
✅ **Layouts flexibles** que se adaptan al dispositivo  
✅ **Tipografía escalable** según tamaño de pantalla  
✅ **Touch targets** apropiados para dispositivos táctiles  

## 📁 Estructura del Proyecto

```
visitApp/
├── Visitapp/
│   ├── Domain/
│   │   ├── Entities/          # Entidades de dominio
│   │   ├── Interfaces/        # Contratos del dominio
│   │   ├── Specifications/    # Specification Pattern
│   │   └── Builders/          # Builder Pattern
│   ├── Application/
│   │   ├── Commands/          # Command Pattern
│   │   ├── DTOs/             # Objetos de transferencia
│   │   ├── Interfaces/       # Contratos de aplicación
│   │   └── Services/         # Servicios de aplicación
│   ├── Infrastructure/
│   │   ├── Repositories/     # Implementación de repositorios
│   │   ├── Services/         # Servicios de infraestructura
│   │   └── Common/           # Mediator implementation
│   ├── Controllers/
│   │   ├── V2/              # Controladores Clean Architecture
│   │   └── AuthController.cs # Controlador legacy
│   ├── Middleware/           # Security & Performance middleware
│   └── Data/                # Contexto de base de datos
└── Visitapp.Tests/          # Tests unitarios e integración
    ├── Application/         # Tests de capa de aplicación
    ├── Domain/             # Tests de dominio
    ├── Infrastructure/     # Tests de infraestructura
    └── Controllers/        # Tests de controladores
```

## 🔧 Configuración

### Base de Datos
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VisitAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### JWT
```json
{
  "Jwt": {
    "Key": "your-secret-key-here-make-it-long-and-secure-123456789",
    "ExpirationDays": 7
  }
}
```

## 📈 Métricas de Calidad

- **Cobertura de Tests**: 85%+
- **SOLID Compliance**: ✅ Completo
- **Clean Architecture**: ✅ Implementada
- **Patrones de Diseño**: 8 patrones implementados
- **Seguridad**: Nivel empresarial
- **Rendimiento**: Optimizado
- **Responsividad**: Multi-dispositivo
- **Documentación API**: Swagger completo

## 🤝 Contribución

1. Fork del proyecto
2. Crear feature branch
3. Implementar con tests
4. Seguir principios SOLID
5. Documentar cambios
6. Pull request

## 📄 Licencia

MIT License - Ver archivo LICENSE para detalles.

## 📱 Multidispositivo y Modo Offline

### Multidispositivo
- Todos los endpoints REST permiten acceso desde móvil, tablet y desktop.
- Autenticación JWT válida en cualquier dispositivo.
- Sincronización automática: los datos se actualizan en tiempo real al consultar los endpoints desde cualquier dispositivo.

### Modo Offline
- Recomendado para Flutter: usar Hive/Secure Storage para cache local.
- Endpoints para sincronización offline:
  - `GET /api/contacts/user/{userId}`: contactos de la familia
  - `GET /api/visits/user/{userId}`: visitas de la familia
  - `GET /api/notes/user/{userId}`: notas de la familia
  - `GET /api/temas`: temas bíblicos
  - `GET /api/preguntasclaves`: preguntas clave
- Al recuperar conexión, sincronizar cambios locales con los endpoints POST/PUT/DELETE.
- Los endpoints devuelven toda la información necesaria para reconstruir el estado local.

### Ejemplo de flujo offline:
1. La app consulta los endpoints y guarda datos localmente.
2. El usuario puede ver y editar datos sin conexión.
3. Al volver a estar online, la app sincroniza los cambios con el backend.

> Todos los endpoints están documentados en Swagger y permiten integración multiplataforma/offline.

---