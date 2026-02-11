---------------------------------------------------------------------
-- VisitApp - Procedimientos para tabla Users
-- INSERT / GET ALL / GET BY ID / UPDATE / DELETE (con opción FORCE)
---------------------------------------------------------------------
SET NOCOUNT ON;
GO

-- Insert: devuelve el NewUserId como resultset (fácil de capturar)
CREATE OR ALTER PROCEDURE sp_InsertUser
    @FullName NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(20),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
            THROW 50000, 'Email already registered.', 1;

        IF EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone)
            THROW 50000, 'Phone already registered.', 1;

        INSERT INTO Users (FullName, Email, Phone, PasswordHash)
        VALUES (@FullName, @Email, @Phone, @PasswordHash);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


-- Get all (opcional ver PasswordHash)
CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, FullName, Email, Phone, IsVerified, CreatedAt
    FROM Users;
END
GO


-- Get by id
CREATE OR ALTER PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, FullName, Email, Phone, IsVerified, CreatedAt
    FROM Users
    WHERE UserId = @UserId;
END
GO


-- Update (parámetros opcionales)
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @UserId INT,
    @FullName NVARCHAR(100) = NULL,
    @Email NVARCHAR(100) = NULL,
    @Phone NVARCHAR(20) = NULL,
    @PasswordHash NVARCHAR(255) = NULL,
    @IsVerified BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Users
        SET FullName = COALESCE(@FullName, FullName),
            Email = COALESCE(@Email, Email),
            Phone = COALESCE(@Phone, Phone),
            PasswordHash = COALESCE(@PasswordHash, PasswordHash),
            IsVerified = COALESCE(@IsVerified, IsVerified)
        WHERE UserId = @UserId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


-- Delete (con ForceDelete)
CREATE OR ALTER PROCEDURE sp_DeleteUser
    @UserId INT,
    @ForceDelete BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF @ForceDelete = 1
        BEGIN
            DELETE FROM UserRoles WHERE UserId = @UserId;
            DELETE FROM UserDistricts WHERE UserId = @UserId;
            DELETE FROM Notifications WHERE UserId = @UserId;
            DELETE FROM Visits WHERE UserId = @UserId;
            DELETE FROM Contacts WHERE UserId = @UserId;
        END
        ELSE
        BEGIN
            IF EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @UserId) OR
               EXISTS (SELECT 1 FROM UserDistricts WHERE UserId = @UserId) OR
               EXISTS (SELECT 1 FROM Notifications WHERE UserId = @UserId) OR
               EXISTS (SELECT 1 FROM Visits WHERE UserId = @UserId) OR
               EXISTS (SELECT 1 FROM Contacts WHERE UserId = @UserId)
            BEGIN
                THROW 50000, 'User has related records. Use @ForceDelete = 1 to force delete.', 1;
            END
        END

        DELETE FROM Users WHERE UserId = @UserId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

--PROCEDIMIENTOS TABLA ROL
-- =============================================
-- Insertar Rol
-- =============================================
CREATE OR ALTER PROCEDURE sp_InsertRole
    @RoleName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM Roles WHERE RoleName = @RoleName)
            THROW 50000, 'Role name already exists.', 1;

        INSERT INTO Roles (RoleName)
        VALUES (@RoleName);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


-- =============================================
-- Consultar todos los Roles
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetAllRoles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RoleId, RoleName
    FROM Roles;
END
GO


-- =============================================
-- Consultar Rol por Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetRoleById
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RoleId, RoleName
    FROM Roles
    WHERE RoleId = @RoleId;
END
GO


-- =============================================
-- Actualizar Rol
-- =============================================
CREATE OR ALTER PROCEDURE sp_UpdateRole
    @RoleId INT,
    @RoleName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Roles
        SET RoleName = @RoleName
        WHERE RoleId = @RoleId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


-- =============================================
-- Eliminar Rol
-- =============================================
CREATE OR ALTER PROCEDURE sp_DeleteRole
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- First, delete associations
        DELETE FROM UserRoles WHERE RoleId = @RoleId;

        -- Then, delete the role
        DELETE FROM Roles WHERE RoleId = @RoleId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


--PROCEDIMIENTOS TABLA DISTRICTS
-- =============================================
-- Insertar Distrito
-- =============================================
CREATE OR ALTER PROCEDURE sp_InsertDistrict
    @DistrictName NVARCHAR(100),
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar duplicados
        IF EXISTS (SELECT 1 FROM Districts WHERE DistrictName = @DistrictName)
        BEGIN
            RAISERROR('El nombre del distrito ya existe.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO Districts (DistrictName, IsActive)
        VALUES (@DistrictName, @IsActive);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- Consultar todos los Distritos
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetAllDistricts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DistrictId, DistrictName, IsActive, CreatedAt
    FROM Districts
    ORDER BY DistrictName;
END
GO

-- =============================================
-- Consultar Distrito por Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetDistrictById
    @DistrictId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DistrictId, DistrictName, IsActive, CreatedAt
    FROM Districts
    WHERE DistrictId = @DistrictId;
END
GO

-- =============================================
-- Actualizar Distrito
-- =============================================
CREATE OR ALTER PROCEDURE sp_UpdateDistrict
    @DistrictId INT,
    @DistrictName NVARCHAR(100),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que no exista otro distrito con el mismo nombre
        IF EXISTS (SELECT 1 FROM Districts WHERE DistrictName = @DistrictName AND DistrictId <> @DistrictId)
        BEGIN
            RAISERROR('Ya existe un distrito con este nombre.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Districts
        SET DistrictName = @DistrictName,
            IsActive = @IsActive
        WHERE DistrictId = @DistrictId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- Eliminar Distrito
-- =============================================
CREATE OR ALTER PROCEDURE sp_DeleteDistrict
    @DistrictId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar dependencias en UserDistricts
        IF EXISTS (SELECT 1 FROM UserDistricts WHERE DistrictId = @DistrictId)
        BEGIN
            RAISERROR('No se puede eliminar: el distrito está asignado a usuarios.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM Districts
        WHERE DistrictId = @DistrictId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

--PROCEDIMIENTOS TABLA CONTACT

-- =============================================
-- Insertar Contacto
-- =============================================
CREATE OR ALTER PROCEDURE sp_InsertContact
    @UserId INT,
    @FullName NVARCHAR(100),
    @Phone NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL,
    @Category NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que exista el usuario
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId)
        BEGIN
            RAISERROR('El usuario no existe.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO Contacts (UserId, FullName, Phone, Email, Category)
        VALUES (@UserId, @FullName, @Phone, @Email, @Category);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- Consultar todos los Contactos
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetAllContacts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT C.ContactId, C.UserId, U.FullName AS UserName,
           C.FullName, C.Phone, C.Email, C.Category, C.CreatedAt
    FROM Contacts C
    INNER JOIN Users U ON C.UserId = U.UserId
    ORDER BY C.FullName;
END
GO

-- =============================================
-- Consultar Contacto por Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetContactById
    @ContactId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT C.ContactId, C.UserId, U.FullName AS UserName,
           C.FullName, C.Phone, C.Email, C.Category, C.CreatedAt
    FROM Contacts C
    INNER JOIN Users U ON C.UserId = U.UserId
    WHERE C.ContactId = @ContactId;
END
GO

-- =============================================
-- Consultar Contactos por Usuario
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetContactsByUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT C.ContactId, C.UserId, U.FullName AS UserName,
           C.FullName, C.Phone, C.Email, C.Category, C.CreatedAt
    FROM Contacts C
    INNER JOIN Users U ON C.UserId = U.UserId
    WHERE C.UserId = @UserId
    ORDER BY C.FullName;
END
GO

-- =============================================
-- Actualizar Contacto
-- =============================================
CREATE OR ALTER PROCEDURE sp_UpdateContact
    @ContactId INT,
    @FullName NVARCHAR(100),
    @Phone NVARCHAR(20) = NULL,
    @Email NVARCHAR(100) = NULL,
    @Category NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Contacts
        SET FullName = @FullName,
            Phone = @Phone,
            Email = @Email,
            Category = @Category
        WHERE ContactId = @ContactId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- Eliminar Contacto
-- =============================================
CREATE OR ALTER PROCEDURE sp_DeleteContact
    @ContactId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar dependencias en Visits
        IF EXISTS (SELECT 1 FROM Visits WHERE ContactId = @ContactId)
        BEGIN
            RAISERROR('No se puede eliminar: el contacto tiene visitas registradas.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM Contacts
        WHERE ContactId = @ContactId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

--PROCEDIMIENTOS TABLA VISIT

-- ============================================
-- CRUD para la tabla Visits
-- ============================================

-- Insertar visita
CREATE PROCEDURE sp_InsertVisit
    @UserId INT,
    @ContactId INT,
    @ScheduledDate DATETIME,
    @Status NVARCHAR(20) = 'Pending',
    @Notes NVARCHAR(MAX) = NULL
AS
BEGIN
    INSERT INTO Visits (UserId, ContactId, ScheduledDate, Status, Notes)
    VALUES (@UserId, @ContactId, @ScheduledDate, @Status, @Notes)
END
GO

-- Consultar todas las visitas
CREATE PROCEDURE sp_GetAllVisits
AS
BEGIN
    SELECT v.VisitId, v.UserId, u.FullName AS UserName,
           v.ContactId, c.FullName AS ContactName,
           v.ScheduledDate, v.Status, v.Notes, v.CreatedAt
    FROM Visits v
    INNER JOIN Users u ON v.UserId = u.UserId
    INNER JOIN Contacts c ON v.ContactId = c.ContactId
    ORDER BY v.CreatedAt DESC
END
GO

-- Consultar visita por ID
CREATE PROCEDURE sp_GetVisitById
    @VisitId INT
AS
BEGIN
    SELECT v.VisitId, v.UserId, u.FullName AS UserName,
           v.ContactId, c.FullName AS ContactName,
           v.ScheduledDate, v.Status, v.Notes, v.CreatedAt
    FROM Visits v
    INNER JOIN Users u ON v.UserId = u.UserId
    INNER JOIN Contacts c ON v.ContactId = c.ContactId
    WHERE v.VisitId = @VisitId
END
GO

-- Actualizar visita
CREATE PROCEDURE sp_UpdateVisit
    @VisitId INT,
    @UserId INT,
    @ContactId INT,
    @ScheduledDate DATETIME,
    @Status NVARCHAR(20),
    @Notes NVARCHAR(MAX)
AS
BEGIN
    UPDATE Visits
    SET UserId = @UserId,
        ContactId = @ContactId,
        ScheduledDate = @ScheduledDate,
        Status = @Status,
        Notes = @Notes
    WHERE VisitId = @VisitId
END
GO

-- Eliminar visita
CREATE PROCEDURE sp_DeleteVisit
    @VisitId INT
AS
BEGIN
    DELETE FROM Visits WHERE VisitId = @VisitId
END
GO

-- ============================================
-- Procedimientos para la tabla UserRoles
-- ============================================

-- Asignar un rol a un usuario
CREATE PROCEDURE sp_AddUserRole
    @UserId INT,
    @RoleId INT
AS
BEGIN
    INSERT INTO UserRoles (UserId, RoleId)
    VALUES (@UserId, @RoleId)
END
GO

-- Consultar roles de un usuario
CREATE PROCEDURE sp_GetUserRoles
    @UserId INT
AS
BEGIN
    SELECT ur.UserId, u.FullName, ur.RoleId, r.RoleName, ur.AssignedDate
    FROM UserRoles ur
    INNER JOIN Users u ON ur.UserId = u.UserId
    INNER JOIN Roles r ON ur.RoleId = r.RoleId
    WHERE ur.UserId = @UserId
END
GO

-- Quitar un rol a un usuario
CREATE PROCEDURE sp_RemoveUserRole
    @UserId INT,
    @RoleId INT
AS
BEGIN
    DELETE FROM UserRoles
    WHERE UserId = @UserId AND RoleId = @RoleId
END
GO
-- ============================================
-- Procedimientos para la tabla UserDistricts
-- ============================================

-- Asignar un distrito a un usuario
CREATE PROCEDURE sp_AddUserDistrict
    @UserId INT,
    @DistrictId INT
AS
BEGIN
    INSERT INTO UserDistricts (UserId, DistrictId)
    VALUES (@UserId, @DistrictId)
END
GO

-- Consultar distritos de un usuario
CREATE PROCEDURE sp_GetUserDistricts
    @UserId INT
AS
BEGIN
    SELECT ud.UserId, u.FullName, ud.DistrictId, d.DistrictName, ud.AssignedDate
    FROM UserDistricts ud
    INNER JOIN Users u ON ud.UserId = u.UserId
    INNER JOIN Districts d ON ud.DistrictId = d.DistrictId
    WHERE ud.UserId = @UserId
END
GO

-- Quitar un distrito a un usuario
CREATE PROCEDURE sp_RemoveUserDistrict
    @UserId INT,
    @DistrictId INT
AS
BEGIN
    DELETE FROM UserDistricts
    WHERE UserId = @UserId AND DistrictId = @DistrictId
END
GO

-- ============================================
-- Procedimientos para la tabla Notifications
-- ============================================

-- Insertar notificación
CREATE PROCEDURE sp_InsertNotification
    @UserId INT,
    @VisitId INT = NULL,
    @Type NVARCHAR(50),
    @Message NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Notifications (UserId, VisitId, Type, Message)
    VALUES (@UserId, @VisitId, @Type, @Message)
END
GO

-- Consultar todas las notificaciones
CREATE PROCEDURE sp_GetNotifications
AS
BEGIN
    SELECT n.NotificationId, n.UserId, u.FullName, n.VisitId, n.Type, n.Message, 
           n.IsRead, n.CreatedAt
    FROM Notifications n
    INNER JOIN Users u ON n.UserId = u.UserId
END
GO

-- Consultar notificación por ID
CREATE PROCEDURE sp_GetNotificationById
    @NotificationId INT
AS
BEGIN
    SELECT n.NotificationId, n.UserId, u.FullName, n.VisitId, n.Type, n.Message, 
           n.IsRead, n.CreatedAt
    FROM Notifications n
    INNER JOIN Users u ON n.UserId = u.UserId
    WHERE n.NotificationId = @NotificationId
END
GO

-- Actualizar notificación
CREATE PROCEDURE sp_UpdateNotification
    @NotificationId INT,
    @Type NVARCHAR(50),
    @Message NVARCHAR(MAX),
    @IsRead BIT
AS
BEGIN
    UPDATE Notifications
    SET Type = @Type,
        Message = @Message,
        IsRead = @IsRead
    WHERE NotificationId = @NotificationId
END
GO

-- Eliminar notificación
CREATE PROCEDURE sp_DeleteNotification
    @NotificationId INT
AS
BEGIN
    DELETE FROM Notifications
    WHERE NotificationId = @NotificationId
END
GO

