
USE [Tickets]
GO
SET IDENTITY_INSERT dbo.Catalog ON;
GO

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
		   3092,
		   29,
           'AllocationStatu',
           4,
           'Impresa',
           'Asignacion de billetes impresa',
           1)
GO

SET IDENTITY_INSERT dbo.Catalog OFF;
GO