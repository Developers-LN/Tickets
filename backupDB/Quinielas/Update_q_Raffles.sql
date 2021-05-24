
ALTER TABLE Raffle
ADD PoolsProspectId INT NULL
GO

ALTER TABLE [dbo].[Raffle]  WITH CHECK ADD  CONSTRAINT [FK_Solteo_PoolsProspect] FOREIGN KEY([PoolsProspectId])
REFERENCES [dbo].[Prospect] ([Id])
GO

ALTER TABLE [dbo].[Raffle] CHECK CONSTRAINT [FK_Solteo_PoolsProspect]
GO