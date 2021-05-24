use Tickets
go

ALTER TABLE Client 
ADD PreviousDebt decimal(18, 2) not null default 0;
GO

ALTER TABLE IdentifyBachPayment
ADD DiscountPercent int not null default 0
GO

ALTER TABLE NoteCredit
ADD DiscountPercent int not null default 0
GO

ALTER TABLE CertificationNumber
ADD Fractions int not null default 0
GO
