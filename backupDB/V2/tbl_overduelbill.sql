USE [Tickets]
GO

/****** Object:  Table [dbo].[OverduelBill]    Script Date: 10/03/2017 14:52:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[OverduelBill](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[InvoiceDate] [date] NOT NULL,
	[ClientId] [int] NOT NULL,
	[ClientName] [varchar](450) NULL,
	[ClientTradeName] [varchar](450) NULL,
	[RaffleId] [int] NOT NULL,
	[RaffleName] [varchar](max) NULL,
	[AgencyId] [int] NOT NULL,
	[OverduelBillAddedDate] [date] NOT NULL,
	[OverduelBillDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[OverduelBill]  WITH CHECK ADD FOREIGN KEY([AgencyId])
REFERENCES [dbo].[Agency] ([Id])
GO

ALTER TABLE [dbo].[OverduelBill]  WITH CHECK ADD FOREIGN KEY([ClientId])
REFERENCES [dbo].[Client] ([Id])
GO

ALTER TABLE [dbo].[OverduelBill]  WITH CHECK ADD FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
GO

ALTER TABLE [dbo].[OverduelBill]  WITH CHECK ADD FOREIGN KEY([RaffleId])
REFERENCES [dbo].[Raffle] ([Id])
GO


