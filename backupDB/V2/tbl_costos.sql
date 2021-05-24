create table Costos(
Id int identity(1,1) primary key,
Detalles varchar(250) not null,
Cantidad int not null,
Precio Money not null)