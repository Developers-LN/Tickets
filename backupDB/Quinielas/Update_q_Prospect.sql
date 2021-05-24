
USE [Tickets]
GO
SET IDENTITY_INSERT dbo.Catalog ON;
GO


Update Catalog  SET IdGroup = 56 where IdGroup > 36
go

INSERT INTO [dbo].[Catalog]
           (
		   [Id]
		   ,[IdGroup]
           ,[NameGroup]
           ,[IdDetail]
           ,[NameDetail]
           ,[Description]
           ,[Statu])
     VALUES
           (
		   5801,
		   36,
           'ProspectType',
           4,
           'Billetes',
           'Prospecto de tipo billetes',
           1),
		   (
		   5802,
		   36,
           'ProspectType',
           4,
           'Quinielas',
           'Prospecto de tipos quinielas',
           1)
GO

SET IDENTITY_INSERT dbo.Catalog OFF;
GO

ALTER TABLE Prospect 
ADD Type INT NOT NULL
CONSTRAINT ProspectType DEFAULT 5801
