IF OBJECT_ID('dbo.RRentabilidaSorteo', 'U') IS NOT NULL 
  DROP TABLE dbo.RRentabilidaSorteo; 

GO

create table RRentabilidaSorteo(
Id int identity(1,1) primary key,
RaffleId int not null references Raffle(Id),
CantBilletes int not null,
CantSeries int not null,
TotalHojas int not null,
AsignFracciones int not null,
AsignFraMonto money not null,
AsignFraFacturas int not null,
MontoFraFacturas money not null,
AsignFraNoFacturadas int not null,
MontoFraNoFacturadas money not null,
FraDevueltasPorClientes int not null,
MontoDevueltoPorClientes money not null,
VentaFraccionesCliente int not null,
MontoVentaFraCliente money not null
)

