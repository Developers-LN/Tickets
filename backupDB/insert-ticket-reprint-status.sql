USE [Tickets]
GO
SET IDENTITY_INSERT dbo.Catalog ON;
GO

INSERT INTO [dbo].[Catalog]
           (
		   [Id],
		   [IdGroup]
           ,[NameGroup]
           ,[IdDetail]
           ,[NameDetail]
           ,[Description]
           ,[Statu])
     VALUES
           (
		   2092,
		   31,
           'TicketReprintStatu',
           1,
           'Creado',
           'Reimpresion de billetes creada',
           1),
		   (
		   2093,
		   31,
           'TicketReprintStatu',
           2,
           'Rechazado',
           'Reimpresion de billetes rechazada',
           1),
		   (
		   2094,
		   31,
           'TicketReprintStatu',
           3,
           'Aprobada',
           'Reimpresion de billetes aprobada',
           1),
		   (
		   2095,
		   31,
           'TicketReprintStatu',
           4,
           'Impresa',
           'Reimpresion de billetes impresa',
           1),
		   
		   (
		   2096,
		   31,
           'TicketReprintStatu',
           5,
           'En proceso',
           'Reimpresion de billetes en proceso',
           1);
GO

SET IDENTITY_INSERT dbo.WorkflowType ON;
GO

INSERT INTO [dbo].[WorkflowType]
           (
		   [Id]
		   ,[Name]
           ,[Description]
           ,[Statu]
           ,[CreateDate]
           ,[CreateUser])
     VALUES
           (
		   4
		   ,'Aprobación de Reimpresion'
		   ,'Flojo para aprobar la reimpresion'
		   ,1
		   ,'2015-10-20 14:36:46.403'
		   ,2)
GO

SET IDENTITY_INSERT dbo.WorkflowType OFF;
GO