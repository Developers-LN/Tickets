USE [Tickets]
GO

/****** Object:  Table [dbo].[ReceivablePayment]    Script Date: 11/27/2015 12:58:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReceivablePayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ReceivablePayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ReceivablePayment]  WITH CHECK ADD  CONSTRAINT [FK_ReceivablePayment_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
GO

ALTER TABLE [dbo].[ReceivablePayment] CHECK CONSTRAINT [FK_ReceivablePayment_Invoice]
GO

ALTER TABLE [dbo].[ReceivablePayment]  WITH CHECK ADD  CONSTRAINT [FK_ReceivablePayment_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[ReceivablePayment] CHECK CONSTRAINT [FK_ReceivablePayment_User]
GO


