-- Insertar roles básicos
INSERT INTO Roles (RoleName, Description, IsActive, CreatedAt) VALUES 
('admin', 'Administrador del sistema', 1, GETDATE()),
('pastor', 'Pastor de distrito', 1, GETDATE()),
('lider', 'Líder de grupo', 1, GETDATE()),
('familia', 'Miembro de familia', 1, GETDATE());

-- Insertar distritos
INSERT INTO Districts (DistrictName, IsActive, CreatedAt) VALUES 
('BOYACA OCCIDENTE', 1, GETDATE()),
('TUNDAMA', 1, GETDATE()),
('SABANA OCCIDENTE', 1, GETDATE()),
('ENGATIVA NOROCCIDENTE', 1, GETDATE());

-- Insertar iglesias
INSERT INTO Churches (ChurchName, DistrictId, IsActive, CreatedAt) VALUES 
('CHIQUINQUIRA', 1, 1, GETDATE()),
('DUITAMA CENTRAL', 2, 1, GETDATE()),
('MOSQUERA CENTRAL', 3, 1, GETDATE()),
('OASIS DE AMOR', 4, 1, GETDATE());

-- Insertar usuario de prueba
INSERT INTO Users (FullName, Email, Phone, PasswordHash, IsVerified, CreatedAt, ChurchId) VALUES 
('Test User', 'test@example.com', '1234567890', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 1, GETDATE(), 1);

-- Asignar rol al usuario
INSERT INTO UserRoles (UserId, RoleId, AssignedDate) VALUES 
(1, 4, GETDATE()); -- Asignar rol 'familia' al usuario 1