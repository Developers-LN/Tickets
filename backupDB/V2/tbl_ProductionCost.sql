  use Tickets
  go
  -- drop table ProductionCostHistory
  -- drop table ProductionCost
  create table ProductionCost (
  Id int identity(1,1) primary key,
  RaffleId int not null references Raffle(id),
  Detalle varchar(250) not null,
  Cantidad int not null,
  Monto money not null,
  Created datetime not null,
  Status bit not null
  );
 

  --posiblemente un historico para esta tabla es innecesario
  --create table ProductionCostHistory (
  --Id int identity(1,1) primary key,
  --ProductCostId int not null references ProductionCost(Id),
  --RaffleId int not null references Raffle(id),
  --Detalle varchar(250) not null,
  --Cantidad int not null,
  --Total money not null,
  --Created datetime ,
  --status bit not null
  --);


 

