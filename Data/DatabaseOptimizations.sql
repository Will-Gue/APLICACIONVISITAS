-- Database Performance Optimizations for VisitApp
-- Execute these after migrations to improve query performance

-- Índices para consultas frecuentes en tablas legacy
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Visits_Date_Status')
CREATE INDEX IX_Visits_Date_Status ON Visits(VisitDate, Status);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contacts_Category')
CREATE INDEX IX_Contacts_Category ON Contacts(Category);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contacts_UserId')
CREATE INDEX IX_Contacts_UserId ON Contacts(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Visits_UserId')
CREATE INDEX IX_Visits_UserId ON Visits(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Visits_ContactId')
CREATE INDEX IX_Visits_ContactId ON Visits(ContactId);

-- Índices para tablas de Clean Architecture (Domain)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DomainVisits_Date_Status')
CREATE INDEX IX_DomainVisits_Date_Status ON DomainVisits(ScheduledDate, Status);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DomainContacts_Category')
CREATE INDEX IX_DomainContacts_Category ON DomainContacts(Category);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DomainContacts_UserId')
CREATE INDEX IX_DomainContacts_UserId ON DomainContacts(UserId);

-- Estadísticas para optimización del query planner
UPDATE STATISTICS Users;
UPDATE STATISTICS Contacts;
UPDATE STATISTICS Visits;
UPDATE STATISTICS DomainUsers;
UPDATE STATISTICS DomainContacts;
UPDATE STATISTICS DomainVisits;