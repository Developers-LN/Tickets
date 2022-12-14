USE [Tickets]
GO
/****** Object:  StoredProcedure [dbo].[procPopulateOveduelBill]    Script Date: 10/03/2017 14:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[procPopulateOveduelBill]
AS
insert into overduelBill
select i.Id InvoiceId, i.InvoiceDate, i.ClientId,
c.Name, c.Tradename, i.RaffleId, r.Name,
i.AgencyId, GETDATE() OverduelBillAddedDate,
 DATEADD(d,i.InvoiceExpredDay,i.InvoiceDate) overduelBill
FROM Invoice i join Client c on i.ClientId = c.Id
join Raffle r on i.RaffleId = r.Id	WHERE i.PaymentStatu = 2082
and GETDATE() > DATEADD(d,i.InvoiceExpredDay,i.InvoiceDate) 
and not exists (select  * from overduelBill bill where bill.InvoiceId = i.Id)