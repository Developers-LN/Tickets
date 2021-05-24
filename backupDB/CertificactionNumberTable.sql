USE [Tickets]
GO

/****** Object:  Table [dbo].[CertificationNumber]    Script Date: 12/1/2015 11:38:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CertificationNumber](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NOT NULL,
	[RaffleAwardId] [int] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CertificationNumber] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CertificationNumber]  WITH CHECK ADD  CONSTRAINT [FK_CertificationNumber_RaffleAward] FOREIGN KEY([RaffleAwardId])
REFERENCES [dbo].[RaffleAward] ([Id])
GO

ALTER TABLE [dbo].[CertificationNumber] CHECK CONSTRAINT [FK_CertificationNumber_RaffleAward]
GO

ALTER TABLE [dbo].[CertificationNumber]  WITH CHECK ADD  CONSTRAINT [FK_CertificationNumber_User] FOREIGN KEY([CreateUser])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[CertificationNumber] CHECK CONSTRAINT [FK_CertificationNumber_User]
GO


