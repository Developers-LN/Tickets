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
		   5821,
		   37,
           'AllocationType',
           1,
           'Billetes',
           'Asignaciones de tipo billetes',
           1),
		   (
		   5822,
		   37,
           'AllocationType',
           2,
           'Quinielas',
           'Asignaciones de tipos quinielas',
           1)
GO

SET IDENTITY_INSERT dbo.Catalog OFF;
GO

Update TicketAllocation SET Type = 5821 where Id > 0