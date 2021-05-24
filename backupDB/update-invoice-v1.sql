
USE [Tickets]
GO
SET IDENTITY_INSERT dbo.Catalog ON;
GO

INSERT INTO [dbo].[Catalog]
           (
		   [IdGroup]
           ,[NameGroup]
           ,[IdDetail]
           ,[NameDetail]
           ,[Description]
           ,[Statu])
     VALUES
           (
		   32,
           'TicketReturnedStatu',
           1,
           'Creado',
           'Devoluci�n de billetes creada',
           1),
		   (
		   32,
           'TicketReturnedStatu',
           2,
           'Facturada',
           'Devoluci�n de billetes facturada',
           1),
		   (
		   32,
           'TicketReturnedStatu',
           3,
           'Cancelada',
           'Devoluci�n de billetes cancelada',
           1);
GO

ALTER TABLE TicketReturn
ADD Statu int not null DEFAULT 1;
GO