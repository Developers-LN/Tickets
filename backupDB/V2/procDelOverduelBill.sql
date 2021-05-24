USE [Tickets]
GO
/****** Object:  StoredProcedure [dbo].[procDelOverduelBill]    Script Date: 10/03/2017 14:53:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[procDelOverduelBill]
as
DELETE  FROM OverduelBill  where exists
(select * from Invoice i where i.Id = OverduelBill.InvoiceId
and i.PaymentStatu = 2083)