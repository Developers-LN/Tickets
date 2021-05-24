/*UPDATE CLIENTES*/

ALTER TABLE Client
DROP COLUMN CheckIn
GO

ALTER TABLE Client
ADD AmountDeposit decimal null

ALTER TABLE Client
ADD DepositDocument text null

ALTER TABLE Client
ADD AdminDocument text null
GO