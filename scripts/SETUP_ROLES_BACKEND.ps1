# Script para configurar los controladores de Roles y UserRoles en el backend

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "CONFIGURACIÓN DE CONTROLADORES DE ROLES" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Ruta del proyecto backend (ajustar según tu configuración)
$backendPath = "C:\Users\Usuario\Documents\API VISITAPP\Visitapp\Visitapp\Controllers"

# Verificar si existe la carpeta del backend
if (!(Test-Path $backendPath)) {
    Write-Host "❌ ERROR: No se encuentra la carpeta del backend en: $backendPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor, ajusta la variable `$backendPath en este script con la ruta correcta." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "✅ Carpeta del backend encontrada: $backendPath" -ForegroundColor Green
Write-Host ""

# Copiar RolesController.cs
Write-Host "📋 Copiando RolesController.cs..." -ForegroundColor Cyan
$rolesController = "backend_roles_controller.cs"
if (Test-Path $rolesController) {
    $destinationRoles = Join-Path $backendPath "RolesController.cs"
    Copy-Item $rolesController $destinationRoles -Force
    Write-Host "✅ RolesController.cs copiado exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ ERROR: No se encuentra $rolesController" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Copiar UserRolesController.cs
Write-Host "📋 Copiando UserRolesController.cs..." -ForegroundColor Cyan
$userRolesController = "backend_user_roles_controller.cs"
if (Test-Path $userRolesController) {
    $destinationUserRoles = Join-Path $backendPath "UserRolesController.cs"
    Copy-Item $userRolesController $destinationUserRoles -Force
    Write-Host "✅ UserRolesController.cs copiado exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ ERROR: No se encuentra $userRolesController" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "✅ CONTROLADORES COPIADOS EXITOSAMENTE" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

Write-Host "📝 PASOS SIGUIENTES:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Abre Visual Studio con tu proyecto backend" -ForegroundColor White
Write-Host "2. Verifica que los archivos estén en la carpeta Controllers:" -ForegroundColor White
Write-Host "   - RolesController.cs" -ForegroundColor White
Write-Host "   - UserRolesController.cs" -ForegroundColor White
Write-Host ""
Write-Host "3. Compila el proyecto (Ctrl + Shift + B)" -ForegroundColor White
Write-Host "4. Ejecuta el backend (F5 o Ctrl + F5)" -ForegroundColor White
Write-Host ""
Write-Host "5. Verifica que los siguientes endpoints estén disponibles:" -ForegroundColor White
Write-Host "   - GET    /api/Roles" -ForegroundColor Cyan
Write-Host "   - GET    /api/Roles/{id}" -ForegroundColor Cyan
Write-Host "   - POST   /api/Roles" -ForegroundColor Cyan
Write-Host "   - GET    /api/UserRoles" -ForegroundColor Cyan
Write-Host "   - GET    /api/UserRoles/user/{userId}" -ForegroundColor Cyan
Write-Host "   - POST   /api/UserRoles" -ForegroundColor Cyan
Write-Host "   - DELETE /api/UserRoles/{userId}/{roleId}" -ForegroundColor Cyan
Write-Host ""
Write-Host "6. Asegúrate de tener los roles creados en la base de datos:" -ForegroundColor White
Write-Host "   - Administrador (roleId = 2)" -ForegroundColor White
Write-Host "   - Pastor (roleId = 13)" -ForegroundColor White
Write-Host "   - Líder (roleId = 14)" -ForegroundColor White
Write-Host "   - Familia (roleId = 15)" -ForegroundColor White
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

Read-Host "Presiona Enter para salir"
