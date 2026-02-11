-- Crear la base de datos
CREATE DATABASE VisitApp
GO

USE VisitApp
GO

-- Tablas principales
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(20) UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsVerified BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
)

CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
)

CREATE TABLE Districts (
    DistrictId INT IDENTITY(1,1) PRIMARY KEY,
    DistrictName NVARCHAR(100) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
)

CREATE TABLE Contacts (
    ContactId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Category NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
)

CREATE TABLE Visits (
    VisitId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ContactId INT NOT NULL,
    ScheduledDate DATETIME NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending',
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ContactId) REFERENCES Contacts(ContactId)
)

CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    VisitId INT,
    Type NVARCHAR(50) NOT NULL,
    Message NVARCHAR(MAX),
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (VisitId) REFERENCES Visits(VisitId)
)

-- Tablas intermedias para relaciones N:M
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    AssignedDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
)

CREATE TABLE UserDistricts (
    UserId INT NOT NULL,
    DistrictId INT NOT NULL,
    AssignedDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserId, DistrictId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (DistrictId) REFERENCES Districts(DistrictId)
)

-- Índices para mejorar el rendimiento
CREATE INDEX IX_Contacts_UserId ON Contacts(UserId)
CREATE INDEX IX_Visits_UserId ON Visits(UserId)
CREATE INDEX IX_Visits_ContactId ON Visits(ContactId)
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId)
CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId)
CREATE INDEX IX_UserDistricts_DistrictId ON UserDistricts(DistrictId)

 


