# 🚀 GUÍA RÁPIDA DE CORRECCIONES

Este archivo contiene los comandos y pasos para ejecutar las correcciones identificadas en el análisis.

---

## ⚙️ CONFIGURACIÓN RÁPIDA

### Backend - Preparar Ambiente

```powershell
# Navegar al proyecto
cd c:\Users\user\Documents\visit\appvisitasnew\VisitApp\backend

# Restaurar NuGets
dotnet restore

# Navegar a proyecto principal
cd src\Visitapp

# Ver migraciones actuales
dotnet ef migrations list

# Ver estado de BD
dotnet ef database info

# Ver el script SQL de cambios
dotnet ef migrations script
```

---

## 🔴 FASE 1: CRÍTICOS (22.5 horas)

### 1. Consolidar Base de Datos (6 horas)

**Paso 1**: Crear migración para combinar tablas

```powershell
cd backend/src/Visitapp

# Crear migración
dotnet ef migrations add ConsolidateSchema
```

**Paso 2**: Editar `Migrations/[timestamp]_ConsolidateSchema.cs`

```csharp
// Remover todas las referencias a legacy tables
// Mantener solo Domain* tables
// Migrar datos si existen
```

**Paso 3**: Aplicar migración

```powershell
dotnet ef database update

# Verificar
dotnet ef database info
```

**Checklist**:
- [ ] Eliminar DbSet<Users> (legacy)
- [ ] Eliminar DbSet<Contacts> (legacy)
- [ ] Eliminar DbSet<Visits> (legacy)
- [ ] Renombrar DomainUsers → Users
- [ ] Renombrar DomainContacts → Contacts
- [ ] Renombrar DomainVisits → Visits
- [ ] Actualizar todos los controladores v1
- [ ] Migración de datos completada

---

### 2. Validación de Permisos (3 horas)

**Ubicación**: `Controllers/ContactsController.cs` (v1)

**Antes**:
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
{
    var contacts = await _context.Contacts.ToListAsync(); // ❌ Ve todos
}
```

**Después**:
```csharp
[HttpGet]
[Authorize]
public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    var contacts = await _context.Contacts
        .Where(c => c.UserId == userId) // ✅ Solo los del usuario
        .ToListAsync();
}
```

**Repetir para**:
- `VisitsController.cs` - GET
- `NotesController.cs` - GET
- Todos los DELETE/PUT

**Checklist**:
- [ ] ContactsController: Filtrar por UserId
- [ ] VisitsController: Filtrar por UserId
- [ ] NotesController: Filtrar por UserId
- [ ] Validar en todos los métodos GET, PUT, DELETE
- [ ] Testing manual completado

---

### 3. Validación de Roles (2.5 horas)

**Agregar atributos [Authorize]**:

```csharp
// Admin endpoints
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase { }

// Admin + Pastor
[Authorize(Roles = "Admin,Pastor")]
public class ReportsController : ControllerBase { }

// Cualquier rol autenticado
[Authorize]
public class UsersController : ControllerBase { }
```

**Ubicaciones a actualizar**:
- [ ] `RolesController.cs` → Admin only
- [ ] `ReportsController.cs` → Admin, Pastor
- [ ] `AuditLogController.cs` → Admin only
- [ ] `UserRolesController.cs` → Admin only
- [ ] `DistrictsController.cs` → Admin only
- [ ] `ChurchesController.cs` → Admin only
- [ ] `TemasController.cs` → Admin (CREATE/EDIT), All (READ)
- [ ] `PreguntasClavesController.cs` → Admin (CREATE/EDIT), All (READ)

**Checklist**:
- [ ] Todos los controladores administrativos protegidos
- [ ] Roles correctos asignados
- [ ] Testing con Postman (sin token, sin rol, con rol)

---

### 4. DTOs Completos (2 horas)

**Archivo**: `Application/DTOs/Auth/RegisterDto.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Visitapp.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [RegularExpression(@"^\+?1?\d{9,15}$", 
            ErrorMessage = "Teléfono inválido")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "Mínimo 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)", 
            ErrorMessage = "Debe incluir mayúscula, minúscula y número")]
        public string Password { get; set; } = string.Empty;
    }
}
```

**Crear DTOs faltantes**:
- [ ] CreateContactDto
- [ ] UpdateContactDto
- [ ] CreateVisitDto
- [ ] UpdateVisitDto
- [ ] UpdateProfileDto
- [ ] PaginatedResponseDto<T>

---

### 5. Email Service (4 horas)

**Archivo**: `appsettings.Development.json`

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "your-email@gmail.com",
    "Pass": "your-app-password",
    "From": "noreply@visitapp.com",
    "EnableSSL": true
  },
  "Jwt": {
    "Key": "tu-clave-super-segura-aqui-minimo-32-caracteres",
    "ExpirationDays": 7
  }
}
```

**Implementar**: `Infrastructure/Services/EmailService.cs`

```csharp
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpSettings = _config.GetSection("Smtp");
        
        using (var client = new SmtpClient(smtpSettings["Host"]))
        {
            client.Port = int.Parse(smtpSettings["Port"] ?? "587");
            client.EnableSsl = bool.Parse(smtpSettings["EnableSSL"] ?? "true");
            client.Credentials = new NetworkCredential(
                smtpSettings["User"],
                smtpSettings["Pass"]
            );
            
            var message = new MailMessage(
                smtpSettings["From"],
                to,
                subject,
                body
            ) { IsBodyHtml = true };
            
            await client.SendMailAsync(message);
        }
    }
}
```

**Usar en RegisterCommand**:
```csharp
if (userCreated)
{
    var emailBody = $@"
        <h1>¡Bienvenido a VisitApp!</h1>
        <p>Tu cuenta ha sido creada exitosamente.</p>
        <p>Tu email: {registerCommand.Email}</p>
    ";
    await _emailService.SendEmailAsync(
        registerCommand.Email,
        "Bienvenido a VisitApp",
        emailBody
    );
}
```

**Checklist**:
- [ ] SMTP configurado
- [ ] EmailService implementado
- [ ] Integrado con registro de usuarios
- [ ] Testing manual de envío

---

### 6. Dependencias Flutter (1 hora)

**Archivo**: `frontend/pubspec.yaml`

```yaml
name: visit_app
description: VisitApp - Sistema de Gestión de Visitas Pastorales

publish_to: 'none'

version: 1.0.0+1

environment:
  sdk: '>=2.19.0 <4.0.0'

dependencies:
  flutter:
    sdk: flutter
  
  # HTTP & API Communication
  http: ^1.1.0
  dio: ^5.0.0
  
  # State Management
  provider: ^6.0.0
  
  # Local Storage
  shared_preferences: ^2.1.0
  hive: ^2.2.0
  hive_flutter: ^1.1.0
  
  # Navigation
  go_router: ^9.0.0
  
  # UI Components
  cached_network_image: ^3.2.0
  intl: ^0.18.0
  
  # PDF & File handling
  pdf: ^3.10.0
  printing: ^5.9.0
  file_picker: ^5.3.0
  
  # Notifications
  flutter_local_notifications: ^14.0.0
  
  # Connectivity
  connectivity_plus: ^4.0.0
  
  # Misc
  cupertino_icons: ^1.0.2

dev_dependencies:
  flutter_test:
    sdk: flutter
  flutter_lints: ^2.0.0

flutter:
  uses-material-design: true
```

**Instalar**:
```powershell
cd frontend
flutter pub get

# Ver si hay problemas
flutter pub outdated

# Compilar para verificar
flutter compile kernel
```

**Checklist**:
- [ ] pubspec.yaml actualizado
- [ ] flutter pub get sin errores
- [ ] No hay conflictos de versiones

---

### 7. ApiService HTTP (4 horas)

**Crear**: `frontend/lib/services/api_service.dart`

```dart
import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {
  static const String _baseUrl = 'http://localhost:5254/api/v2';
  
  late Dio _dio;
  late SharedPreferences _prefs;

  ApiService._();
  static final ApiService _instance = ApiService._();

  factory ApiService() => _instance;

  Future<void> init() async {
    _prefs = await SharedPreferences.getInstance();
    _dio = Dio(
      BaseOptions(
        baseUrl: _baseUrl,
        connectTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
      ),
    );

    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) {
        final token = _prefs.getString('auth_token');
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
      onError: (error, handler) {
        if (error.response?.statusCode == 401) {
          // Token expirado, limpiar
          _prefs.remove('auth_token');
        }
        return handler.next(error);
      },
    ));
  }

  // Auth endpoints
  Future<Map<String, dynamic>> register({
    required String fullName,
    required String email,
    required String phone,
    required String password,
  }) async {
    try {
      final response = await _dio.post(
        '/auth/register',
        data: {
          'fullName': fullName,
          'email': email,
          'phone': phone,
          'password': password,
        },
      );
      return response.data;
    } catch (e) {
      rethrow;
    }
  }

  Future<Map<String, dynamic>> login({
    required String email,
    required String password,
  }) async {
    try {
      final response = await _dio.post(
        '/auth/login',
        data: {
          'email': email,
          'password': password,
        },
      );
      
      final token = response.data['token'];
      await _prefs.setString('auth_token', token);
      
      return response.data;
    } catch (e) {
      rethrow;
    }
  }

  // Contacts endpoints
  Future<List<Map<String, dynamic>>> getContacts() async {
    try {
      final response = await _dio.get('/contacts');
      return List<Map<String, dynamic>>.from(response.data);
    } catch (e) {
      rethrow;
    }
  }

  Future<Map<String, dynamic>> createContact({
    required String fullName,
    required String phone,
    required String email,
    required String category,
  }) async {
    try {
      final response = await _dio.post(
        '/contacts',
        data: {
          'fullName': fullName,
          'phone': phone,
          'email': email,
          'category': category,
        },
      );
      return response.data;
    } catch (e) {
      rethrow;
    }
  }

  // Visits endpoints
  Future<List<Map<String, dynamic>>> getVisits() async {
    try {
      final response = await _dio.get('/visits');
      return List<Map<String, dynamic>>.from(response.data);
    } catch (e) {
      rethrow;
    }
  }

  Future<Map<String, dynamic>> createVisit({
    required int contactId,
    required DateTime scheduledDate,
    required String address,
    required String notes,
  }) async {
    try {
      final response = await _dio.post(
        '/visits',
        data: {
          'contactId': contactId,
          'scheduledDate': scheduledDate.toIso8601String(),
          'address': address,
          'notes': notes,
          'status': 'Programada',
        },
      );
      return response.data;
    } catch (e) {
      rethrow;
    }
  }

  // Utility
  void setToken(String token) {
    _prefs.setString('auth_token', token);
  }

  String? getToken() {
    return _prefs.getString('auth_token');
  }

  Future<void> logout() async {
    await _prefs.remove('auth_token');
  }

  bool isAuthenticated() {
    return _prefs.getString('auth_token') != null;
  }
}
```

**Inicializar en main.dart**:
```dart
void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await ApiService().init();
  runApp(const MyApp());
}
```

**Checklist**:
- [ ] ApiService creado y completo
- [ ] Todos los endpoints implementados
- [ ] Token management funcional
- [ ] Error handling implementado

---

## 📋 VERIFICACIÓN DE FASE 1

### Backend
```bash
# Tests
dotnet test

# Swagger
curl http://localhost:5254/swagger
```

### Frontend
```bash
# Ver si compila
flutter pub get
flutter analyze
flutter pub global activate devtools

# Compilar
flutter build apk --debug   # Android
flutter build ios --debug   # iOS (solo en Mac)
```

---

## 🟡 FASE 2: ARQUITECTURA (22 horas)

Se comenzaría después de Fase 1. Ver sección "Plan de Corrección Recomendado" en `estado.md`

---

## ⏱️ TRACKING DE PROGRESO

### Checklist Fase 1

**Backend (15 horas)**
- [ ] Consolidar BD (6h)
- [ ] Validar permisos (3h)
- [ ] Validar roles (2.5h)
- [ ] DTOs (2h)
- [ ] Email Service (4h) — pero solo login/registro, pushes después
- Subtotal: ~17.5h (ajustar según necesidades)

**Frontend (7.5 horas)**
- [ ] Instalar deps (1h)
- [ ] ApiService (4h)
- [ ] Testing (2.5h)
- Subtotal: ~7.5h

**TOTAL FASE 1: ~25 horas** (puede variar ±3h)

---

## 💡 TIPS DE DESARROLLO

### Backend
- Usar Postman para testing de endpoints
- Ejecutar `dotnet watch run` para recompilación automática
- Ver logs en consola durante desarrollo

### Frontend
- Usar `flutter run` en terminal separada
- `flutter hot reload` (Ctrl+S) para cambios rápidos
- DevTools con `flutter pub global run devtools`

---

## 🆘 TROUBLESHOOTING

### Backend
```bash
# Si hay problema con migraciones
dotnet ef migrations remove
dotnet ef migrations add MigrationName

# Si BD está corrupta
dotnet ef database drop
dotnet ef database update

# Ver SQL generado
dotnet ef migrations script
```

### Frontend
```bash
# Si hay error de deps
flutter clean
flutter pub get
flutter pub upgrade

# Si hay problema de compilación
flutter clean
flutter pub get
flutter analyze

# Ver qué device está disponible
flutter devices
```
---

**Próxima revisión después de completar Fase 1**
