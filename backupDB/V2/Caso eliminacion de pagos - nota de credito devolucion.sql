Select * from Invoice  i where i.Id = 1381

update Invoice set PaymentStatu = 2082 where Id = 1381

-- paso 1
create table temporal 
(
ReceiptPaymentId int,
NoteCreditId int,
TotalCasht decimal(18, 2)
)

--paso 2
alter procedure SP_UpdateCreditNote
@idnotecredit int
as
begin
declare @totalCash  int;
declare @totalRest int;
select @totalCash =  totalCash from NoteCreditReceiptPayment where NoteCreditId = @idnotecredit and (ReceiptPaymentId = 813 or ReceiptPaymentId = 735)
select @totalRest = totalRest from NoteCredit where id = @idnotecredit
update NoteCredit set TotalRest = isNull(@totalRest,0) + isnull( @totalCash,0) where Id = @idnotecredit
Update NoteCredit set Statu = 1 where Id = @idnotecredit
Delete NoteCreditReceiptPayment where NoteCreditId = @idnotecredit and (ReceiptPaymentId = 813 or ReceiptPaymentId = 735)
end

--paso 3
declare @index int = 1;
declare @noteId int = 0;
while(@index <= 68)
begin
select top 1 @noteId = NoteCreditId from  NoteCreditReceiptPayment nrp where nrp.ReceiptPaymentId = 735 or nrp.ReceiptPaymentId = 813
select @noteId;
exec SP_UpdateCreditNote  @noteId
set @index = @index + 1
end