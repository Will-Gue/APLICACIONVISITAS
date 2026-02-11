-- Consultar todos los roles
SELECT RoleId, RoleName FROM Roles;

-- Consultar usuarios con sus roles
SELECT 
    u.UserId,
    u.FullName,
    u.Email,
    r.RoleName
FROM Users u
JOIN UserRoles ur ON u.UserId = ur.UserId
JOIN Roles r ON ur.RoleId = r.RoleId;

-- Consultar usuarios y sus distritos
SELECT 
    u.FullName,
    d.DistrictName,
    ud.AssignedDate
FROM Users u
JOIN UserDistricts ud ON u.UserId = ud.UserId
JOIN Districts d ON ud.DistrictId = d.DistrictId;

-- Consultar contactos y sus visitas programadas
SELECT 
    c.FullName AS ContactName,
    v.ScheduledDate,
    v.Status,
    u.FullName AS AssignedTo
FROM Contacts c
JOIN Visits v ON c.ContactId = v.ContactId
JOIN Users u ON v.UserId = u.UserId;

-- Consultar notificaciones activas
SELECT 
    u.FullName,
    n.Type,
    n.Message,
    n.IsRead,
    n.CreatedAt
FROM Notifications n
JOIN Users u ON n.UserId = u.UserId
WHERE n.IsRead = 0;