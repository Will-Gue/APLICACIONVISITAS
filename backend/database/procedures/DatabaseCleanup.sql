DELETE FROM UserRoles WHERE UserId = 30; -- O el id que quieras
DELETE FROM Users WHERE UserId = 30;
DELETE FROM UserRoles;
-- 🔴 Paso 1: Borrar tablas intermedias primero (dependen de otras)
DELETE FROM UserRoles;
DELETE FROM UserDistricts;

-- 🔴 Paso 2: Borrar tablas hijas que dependen de Users y Contacts
DELETE FROM Notifications;
DELETE FROM Visits;
DELETE FROM Contacts;

-- 🔴 Paso 3: Borrar tablas principales
DELETE FROM Districts;
DELETE FROM Roles;
DELETE FROM Users;

select * from Users

