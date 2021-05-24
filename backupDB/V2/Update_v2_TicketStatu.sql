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
		   801,
		   21,
           'TicketStatus',
           5,
           'Revisado',
           'Billetes revisado',
           1)
GO

SET IDENTITY_INSERT dbo.Catalog OFF;
GO
