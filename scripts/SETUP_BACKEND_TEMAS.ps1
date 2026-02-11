# =====================================================
# SCRIPT DE CONFIGURACIÓN COMPLETA PARA TEMAS CON PDF
# Sistema de Visitas Pastorales
# Fecha: 24 de Noviembre 2025
# =====================================================

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CONFIGURACIÓN DE TEMAS CON PDF    " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Función para mostrar mensajes con colores
function Write-Success { param($message) Write-Host "✅ $message" -ForegroundColor Green }
function Write-Info { param($message) Write-Host "ℹ️  $message" -ForegroundColor Cyan }
function Write-Warning { param($message) Write-Host "⚠️  $message" -ForegroundColor Yellow }
function Write-Error-Custom { param($message) Write-Host "❌ $message" -ForegroundColor Red }

# Paso 1: Verificar archivos necesarios
Write-Info "Paso 1: Verificando archivos del backend..."

$archivosNecesarios = @(
    "TemasController_COMPLETO_CON_PDF.cs",
    "backend_tema_model.cs",
    "backend_tema_dtos.cs",
    "SETUP_TEMAS_COMPLETO.sql"
)

$todoExiste = $true
foreach ($archivo in $archivosNecesarios) {
    if (Test-Path $archivo) {
        Write-Success "Archivo encontrado: $archivo"
    } else {
        Write-Error-Custom "Archivo faltante: $archivo"
        $todoExiste = $false
    }
}

if (-not $todoExiste) {
    Write-Error-Custom "Faltan archivos necesarios. Abortando..."
    exit 1
}

Write-Host ""

# Paso 2: Solicitar información del proyecto backend
Write-Info "Paso 2: Configuración del proyecto backend"
Write-Host ""

$backendPath = Read-Host "Ingresa la ruta completa de tu proyecto backend (ej: C:\Projects\VisitApp\Backend)"

if (-not (Test-Path $backendPath)) {
    Write-Error-Custom "La ruta del backend no existe: $backendPath"
    exit 1
}

Write-Success "Ruta del backend válida: $backendPath"

# Paso 3: Crear directorio para uploads
Write-Info "Paso 3: Creando directorio para PDFs..."

$uploadsPath = Join-Path $backendPath "wwwroot\uploads\temas"

if (-not (Test-Path $uploadsPath)) {
    New-Item -ItemType Directory -Path $uploadsPath -Force | Out-Null
    Write-Success "Directorio creado: $uploadsPath"
} else {
    Write-Warning "El directorio ya existe: $uploadsPath"
}

# Crear un archivo .gitkeep para mantener la carpeta en git
$gitkeepPath = Join-Path $uploadsPath ".gitkeep"
if (-not (Test-Path $gitkeepPath)) {
    New-Item -ItemType File -Path $gitkeepPath -Force | Out-Null
    Write-Success "Archivo .gitkeep creado"
}

Write-Host ""

# Paso 4: Copiar archivos del backend
Write-Info "Paso 4: Copiando archivos al proyecto backend..."

$controllersPath = Join-Path $backendPath "Controllers"
$modelsPath = Join-Path $backendPath "Models"
$dtosPath = Join-Path $backendPath "Dtos"

# Crear directorios si no existen
if (-not (Test-Path $controllersPath)) {
    New-Item -ItemType Directory -Path $controllersPath -Force | Out-Null
}
if (-not (Test-Path $modelsPath)) {
    New-Item -ItemType Directory -Path $modelsPath -Force | Out-Null
}
if (-not (Test-Path $dtosPath)) {
    New-Item -ItemType Directory -Path $dtosPath -Force | Out-Null
}

# Copiar TemasController
Copy-Item "TemasController_COMPLETO_CON_PDF.cs" "$controllersPath\TemasController.cs" -Force
Write-Success "TemasController.cs copiado"

# Copiar Modelo
Copy-Item "backend_tema_model.cs" "$modelsPath\Tema.cs" -Force
Write-Success "Tema.cs copiado"

# Copiar DTOs
Copy-Item "backend_tema_dtos.cs" "$dtosPath\TemaDtos.cs" -Force
Write-Success "TemaDtos.cs copiado"

Write-Host ""

# Paso 5: Configuración de la base de datos
Write-Info "Paso 5: Configuración de la base de datos"
Write-Host ""

Write-Warning "Debes ejecutar el script SQL manualmente:"
Write-Host "   1. Abre SQL Server Management Studio" -ForegroundColor Yellow
Write-Host "   2. Conéctate a tu base de datos VisitApp" -ForegroundColor Yellow
Write-Host "   3. Abre el archivo: SETUP_TEMAS_COMPLETO.sql" -ForegroundColor Yellow
Write-Host "   4. Ejecuta el script completo (F5)" -ForegroundColor Yellow
Write-Host ""

$ejecutarSQL = Read-Host "¿Ya ejecutaste el script SQL? (S/N)"

if ($ejecutarSQL -ne "S" -and $ejecutarSQL -ne "s") {
    Write-Warning "Recuerda ejecutar el script SQL antes de probar la app"
}

Write-Host ""

# Paso 6: Configurar appsettings.json
Write-Info "Paso 6: Verificando appsettings.json..."

$appsettingsPath = Join-Path $backendPath "appsettings.json"

if (Test-Path $appsettingsPath) {
    Write-Success "appsettings.json encontrado"
    Write-Info "Verifica que contenga:"
    Write-Host '  "FileUpload": {' -ForegroundColor Yellow
    Write-Host '    "MaxSizeInBytes": 10485760,' -ForegroundColor Yellow
    Write-Host '    "AllowedExtensions": [".pdf"],' -ForegroundColor Yellow
    Write-Host '    "UploadPath": "wwwroot/uploads/temas"' -ForegroundColor Yellow
    Write-Host '  }' -ForegroundColor Yellow
} else {
    Write-Warning "appsettings.json no encontrado en $backendPath"
}

Write-Host ""

# Paso 7: Configurar Program.cs para límite de tamaño
Write-Info "Paso 7: Verificando Program.cs..."

$programPath = Join-Path $backendPath "Program.cs"

if (Test-Path $programPath) {
    Write-Success "Program.cs encontrado"
    Write-Info "Asegúrate de tener esta configuración:"
    Write-Host '  builder.Services.Configure<FormOptions>(options =>' -ForegroundColor Yellow
    Write-Host '  {' -ForegroundColor Yellow
    Write-Host '      options.MultipartBodyLengthLimit = 10485760; // 10 MB' -ForegroundColor Yellow
    Write-Host '  });' -ForegroundColor Yellow
} else {
    Write-Warning "Program.cs no encontrado"
}

Write-Host ""

# Paso 8: Resumen final
Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "  ✅ CONFIGURACIÓN COMPLETADA       " -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

Write-Success "Archivos copiados al proyecto backend"
Write-Success "Directorio de uploads creado: $uploadsPath"
Write-Host ""

Write-Info "PRÓXIMOS PASOS:"
Write-Host "   1. ✅ Ejecutar SETUP_TEMAS_COMPLETO.sql en SQL Server" -ForegroundColor Cyan
Write-Host "   2. ✅ Compilar el proyecto backend (.NET)" -ForegroundColor Cyan
Write-Host "   3. ✅ Ejecutar el backend (dotnet run o IIS)" -ForegroundColor Cyan
Write-Host "   4. ✅ Ejecutar la app Flutter (flutter run)" -ForegroundColor Cyan
Write-Host "   5. ✅ Probar creación de tema con PDF" -ForegroundColor Cyan
Write-Host ""

Write-Info "ENDPOINTS DISPONIBLES:"
Write-Host "   POST   /api/Temas          - Crear tema con PDF" -ForegroundColor Yellow
Write-Host "   GET    /api/Temas          - Listar todos los temas" -ForegroundColor Yellow
Write-Host "   GET    /api/Temas/{id}     - Obtener tema por ID" -ForegroundColor Yellow
Write-Host "   PUT    /api/Temas/{id}     - Actualizar tema con PDF" -ForegroundColor Yellow
Write-Host "   PATCH  /api/Temas/{id}/estado - Activar/Archivar tema" -ForegroundColor Yellow
Write-Host "   DELETE /api/Temas/{id}     - Eliminar tema (soft delete)" -ForegroundColor Yellow
Write-Host "   GET    /api/Temas/stats    - Estadísticas" -ForegroundColor Yellow
Write-Host ""

Write-Success "¡Todo listo! Ahora puedes crear temas y subir PDFs desde la app Flutter 🎉"
Write-Host ""

# Opción de abrir el directorio
$abrirDirectorio = Read-Host "¿Deseas abrir el directorio del backend? (S/N)"
if ($abrirDirectorio -eq "S" -or $abrirDirectorio -eq "s") {
    explorer $backendPath
}
