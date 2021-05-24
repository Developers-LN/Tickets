USE [Tickets]
GO

insert into ReportByEmail values('Notificacion de facturas vencidas',1,	  'Notificacion de facturas vencidas',	'1028,1029,1030','	Esta notificación se le he enviada con el motivo de indicarle que las siguientes facturas se encuentran vencidas y el pago correspondiente a la misma no ha sido realizado');
insert into ReportByEmail values('Notificacion de cliente nuevo', 2,	  'Se ha dado ingreso a un nuevo cliente',	'1,1017,1028,1029,1030',	'Esta es una notificacion automatica para indicarle que se ha creado un nuevo cliente');
insert into ReportByEmail values('Notificacion de cliente modificado',3	, 'Se han realizado cambios al perfil de un cliente','1028,1029,1030',	'Esta es una notificacion automatica para indicarle que se ha modificado el perfil de un cliente');
insert into ReportByEmail values('Notificacion de facturas vencidas', 4,  'Reporte de Resumen Devoluciones',	'1028,1029,1030',	'El reporte de devoluciones se ha creado, verifique el archivo adjunto');
insert into ReportByEmail values('Notificacion de reportes generados', 5, 'Notificacion de reportes generados',	'1028',	'Mensaje de notificacion de generacion de reporte');