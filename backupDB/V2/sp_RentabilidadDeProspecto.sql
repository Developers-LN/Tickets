--- Rentabilidad sorteo de billetes ---
alter PROCEDURE sp_ReporteRentabilidadProspecto
@raffleId int 
as
if(not exists (select * from RRentabilidadSorteo where RaffleId = @raffleId))
begin
INSERT INTO RRentabilidadSorteo VALUES (@raffleId,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
end

begin
--prospecto de premios

 UPDATE RRentabilidadSorteo
 SET CantBilletes = isNull(A.CantidadDeBillestes,0),
 CantSeries = isNull(A.Series,0),
 TotalHojas = isNull(A.TotalHojas,0)
 FROM RRentabilidadSorteo r inner join
 (select r.Id,
 p.Production CantidadDeBillestes, 
 p.leafNumber Series, 
 (p.Production * p.LeafNumber) TotalHojas
 FROM dbo.Raffle r JOIN dbo.Prospect p ON r.ProspectId = p.Id
 WHERE r.id = @raffleId)A on r.RaffleId = A.Id


 UPDATE RRentabilidadSorteo
 SET AsignFracciones = isNull(C.TotalFracciones,0),
 AsignFraFacturas = isNull(C.FraccionesFacturadas,0),
 AsignFraNoFacturadas = isNull(c.NoFacturados,0)
 FROM  RRentabilidadSorteo r inner join
	(select @raffleId Id,
	sum(case  when tan.Invoiced = 1 then 1 else 0 end) FraccionesFacturadas,
	sum(case  when tan.Invoiced = 0 then 1 else 0 end) NoFacturados,
	count(tan.Id) TotalFracciones
    from TicketAllocationNumber tan
    join TicketAllocation ta on tan.TicketAllocationId = ta.Id 
	and ta.RaffleId = @raffleId--@raffleId
	 )C on r.RaffleId = C.Id

	 -- monto facturadas
 Update RRentabilidadSorteo
 SET RRentabilidadSorteo.MontoFraFacturas  = isNull(qq.MontoFacturadas,0)
 FROM RRentabilidadSorteo r inner join
 (select @raffleId Id,
 sum(q.total) MontoFacturadas from (
	select  it.Quantity, it.PricePerFraction, (it.Quantity * it.PricePerFraction) total---count(it.quantity)  counts, it.Quantity valor,
	 ---(it.Quantity *  counts) totalporFraction 
	 from Invoice i join 
	 InvoiceTicket it on i.Id = it.InvoiceId
	 where i.RaffleId = @raffleId
	 )q)qq on r.RaffleId = qq.id

--- no facturadas
 Update RRentabilidadSorteo
 SET RRentabilidadSorteo.MontoFraNoFacturadas  = isNull(qab.FraccionesNoFacturadas,0)
 from RRentabilidadSorteo r inner join
     (select @raffleId Id, sum(qb.totalByFraction) FraccionesNoFacturadas from(
	 select (tan.FractionTo - tan.FractionFrom) + 1 TotalFracciones,
	  ta.ClientId, c.PriceId, ca.NameDetail, pp.FactionPrice,
	  ((tan.FractionTo - tan.FractionFrom) + 1) * pp.FactionPrice totalByFraction
	 from 
 TicketAllocation ta  join 
 TicketAllocationNumber tan
 on ta.Id = tan.TicketAllocationId
 join Client c on c.Id = ta.ClientId
 join Catalog ca on c.PriceId = ca.Id
 join Prospect_Price pp on pp.PriceId = c.PriceId
 where Invoiced = 0
 and ta.RaffleId = @raffleId
 )qb)qab on r.RaffleId = qab.Id

	 --asignacion de billetes
 Update RRentabilidadSorteo
 SET RRentabilidadSorteo.asignfraMonto = isNull(q.AsignFraMonto,0)
 from RRentabilidadSorteo r  inner join (select @raffleId RaffleId, (isNull(MontoFraFacturas,0) + isNull(MontoFraNoFacturadas,0)) asignFraMonto
 from RRentabilidadSorteo where RaffleId = @raffleId) q on  r.RaffleId = q.RaffleId


	 --devolucion premiada
  Update RRentabilidadSorteo SET RRentabilidadSorteo.DevolucionPremiada  = isNull(qq.devoluciones,0)
  from RRentabilidadSorteo r inner join 
  ( 
  select sum(isnull(q.totalDevolucion,0)) devoluciones, @raffleId raffleId from(
  select @raffleId Id, tr.TicketAllocationNimberId, tr.FractionFrom, tr.FractionTo, ta.Number, ra.AwardId,  aw.Value,
  (aw.Value /(pr.LeafFraction * pr.LeafNumber)) PrecioFraccion,
  ((tr.FractionTo - tr.FractionFrom +1) * (aw.Value /(pr.LeafFraction * pr.LeafNumber))) totalDevolucion,
  Pr.Name, pr.LeafFraction, pr.LeafNumber
  from TicketReturn tr join TicketAllocationNumber ta
  on tr.TicketAllocationNimberId = ta.Id
  join RaffleAward ra on ta.Number = ra.ControlNumber
  join Award aw on aw.Id = ra.AwardId
  join Raffle raf on raf.Id = tr.RaffleId
  join Prospect Pr on pr.Id = raf.ProspectId
  where tr.RaffleId = @raffleId and tr.RaffleId = ra.RaffleId
  and raf.id = @raffleId and ta.Invoiced = 1 )q)qq on qq.raffleId = r.RaffleId


--Ventas en hojas clientes 

Update RRentabilidadSorteo
SET RRentabilidadSorteo.VentaFraccionesCliente  = isNull(q.ventaFracciones,0),
RRentabilidadSorteo.MontoVentaFraCliente = isNull(q.MontoVentaFraCliente,0)
from RRentabilidadSorteo r inner join 
	(select @raffleId RaffleId, (asignFraFacturas) ventaFracciones ,
	(MontoFraFacturas) MontoVentaFraCliente from RRentabilidadSorteo where raffleId = @raffleId)q
	on r.RaffleId = q.RaffleId 


	 ----devolucion premiada
  --Update RRentabilidadSorteo SET RRentabilidadSorteo.DevolucionPremiada  = isNull(qq.totalDevolucion,0)
  --from RRentabilidadSorteo r inner join 
  --( select sum(q.totalDevolucion) devoluciones, @raffleId raffleId from(
  --select @raffleId Id, tr.TicketAllocationNimberId, tr.FractionFrom, tr.FractionTo, ta.Number, ra.AwardId,  aw.Value,
  --(aw.Value /(pr.LeafFraction * pr.LeafNumber)) PrecioFraccion,
  --((tr.FractionTo - tr.FractionFrom +1) * (aw.Value /(pr.LeafFraction * pr.LeafNumber))) totalDevolucion,
  --Pr.Name, pr.LeafFraction, pr.LeafNumber
  --from TicketReturn tr join TicketAllocationNumber ta
  --on tr.TicketAllocationNimberId = ta.Id
  --join RaffleAward ra on ta.Number = ra.ControlNumber
  --join Award aw on aw.Id = ra.AwardId
  --join Raffle raf on raf.Id = tr.RaffleId
  --join Prospect Pr on pr.Id = raf.ProspectId
  --where tr.RaffleId = @raffleId and tr.RaffleId = ra.RaffleId
  --and raf.id = @raffleId and ta.Invoiced = 1 )q)qq on qq.raffleId = r.RaffleId


	-- ganadores publicos premiados - premiados devueltos

 Update RRentabilidadSorteo SET RRentabilidadSorteo.GanadoresPublicos = (isNull(qq.ganadoresPublico,0) - r.DevolucionPremiada)
  from RRentabilidadSorteo r inner join
  (
  select sum(q.Value) ganadoresPublico, @raffleId RaffleId from(
  select  distinct  raf.id raffleId ,ta.Number, ra.AwardId,  aw.Value,
  (aw.Value /(pr.LeafFraction * pr.LeafNumber)) PrecioFraccion,
  (ta.FractionTo - ta.FractionFrom + 1 ) CantidadDeFracciones,
  Pr.Name, pr.LeafFraction, pr.LeafNumber
  from  TicketAllocationNumber ta
  join RaffleAward ra on ta.Number = ra.ControlNumber
 
  join Award aw on aw.Id = ra.AwardId
  join Raffle raf on raf.Id = ra.RaffleId
  join Prospect Pr on pr.Id = raf.ProspectId
  where ra.RaffleId = @raffleId and ra.RaffleId = @raffleId
  and raf.id = @raffleId and ta.Invoiced = 1 and ta.Id not in( select tr.TicketAllocationNimberId  from TicketReturn tr
   where tr.RaffleId = @raffleId) )q
   )qq on qq.raffleId = r.RaffleId

   -- premios pagados
 Update RRentabilidadSorteo SET RRentabilidadSorteo.Pagado = isNull(qq.premiosPagados,0)
 from RRentabilidadSorteo r inner join
 (
 select @raffleId raffleId, sum(q.pagadoefectivo) premiosPagados from (
 SELECT  @raffleId raffleId,sum(ibp.value) pagadoefectivo FROM IdentifyBach IB 
 JOIN IdentifyBachPayment IBP ON IB.Id = IBP.IdentifyBachId
 WHERE IB.RaffleId = @raffleId
 union all
 SELECT 1 raffleId, sum(NC.TotalCash - NC.TotalRest) pagadoNotaCredit FROM IdentifyBach IB 
 JOIN IdentifyBach_CreditNote INC ON INC.IdentifyBachid = IB.Id
 JOIN NoteCredit NC ON NC.Id = INC.CreditNoteId
 WHERE IB.RaffleId = @raffleId)q)qq on r.RaffleId = qq.raffleId

 --premios pendiente de pago
 Update RRentabilidadSorteo SET RRentabilidadSorteo.PendientePago = isNull(rr.pendientePago,0)
 from RRentabilidadSorteo r inner join (select @raffleId raffleId, (GanadoresPublicos - Pagado) pendientePago
 from RRentabilidadSorteo where RaffleId = @raffleId) rr on r.RaffleId = rr.raffleId


 
-- total devuelto
 Update RRentabilidadSorteo SET RRentabilidadSorteo.FraDevueltasPorClientes = isNull(qq.cantidadDevolucion,0),
 RRentabilidadSorteo.MontoDevueltoPorClientes = ISNULL(qq.MontoDevolucion,0)
 from RRentabilidadSorteo r inner join  (
 --Total devoluciones
SELECT @raffleId raffleId, sum(q.totalDevolucion) MontoDevolucion, count(q.totalDevolucion) cantidadDevolucion from(
SELECT TAN.Number, TR.FractionFrom, TR.FractionTo, TR.ClientId, C.Id, 
PP.PriceId, PP.FactionPrice, ((TR.FractionTo - TR.FractionFrom +1) * (PP.FactionPrice)) totalDevolucion 
FROM TicketReturn TR
JOIN Client C ON C.Id = TR.ClientId
JOIN TicketAllocationNumber TAN ON TAN.Id = TR.TicketAllocationNimberId
JOIN Raffle R ON R.Id = TR.RaffleId
JOIN Prospect P ON P.Id = R.ProspectId
JOIN Prospect_Price PP ON PP.ProspectId = P.Id AND PP.PriceId = C.PriceId
WHERE TR.RaffleId = @raffleId AND r.Id = @raffleId)q)qq on r.RaffleId = qq.raffleId






--select distinct * from TicketReturn tr  join TicketAllocationNumber ta
--  on tr.TicketAllocationNimberId = ta.Id 
--  where tr.RaffleId = 3742 and ta.Invoiced = 1 and
--  ta.Number not in( select ra.ControlNumber from RaffleAward ra where ra.RaffleId = 3742)


 -- select * from RaffleAward where RaffleId = 3742 and ControlNumber = 35323
-- otros
--Select isnull(sum(q.suma),0) otros from (
--select 
--isnull(rp.TotalCredit,0) + isnull(rp.TotalCheck,0) suma
--from invoice i
--inner join ReceiptPayment rp
--on i.id = rp.InvoiceId  where i.raffleId = @raffleId
--and i.paymenttype in (2069,2070)
--)q

-- no impreso
 Update RRentabilidadSorteo
 SET RRentabilidadSorteo.NoImpreso  = isNull(dpq.noImpreso,0)
 From RRentabilidadSorteo r inner join (
 select rr.raffleId, pp.totalProspecto - (rr.GanadoresPublicos +  rr.DevolucionPremiada)  noImpreso
  from RRentabilidadSorteo rr 
  inner join (select @raffleId raffleId, sum( a.totalValue) totalProspecto from Award a
join Prospect p on p.Id = a.ProspectId join
Raffle r on r.ProspectId = p.Id
where r.id = @raffleId)pp on rr.raffleId = pp.raffleId)dpq on r.RaffleId = dpq.RaffleId

-- pagados en efectivo
  Update RRentabilidadSorteo
 SET RRentabilidadSorteo.Efectivo  = isNull(qq.PagadosEnEfectivo,0)
 From RRentabilidadSorteo r inner join (
 --  pagado en efectivo
 select @raffleId raffleId, sum(q.pe) PagadosEnEfectivo from (
 SELECT  @raffleId raffleId,sum(ibp.value) pe FROM IdentifyBach IB 
 JOIN IdentifyBachPayment IBP ON IB.Id = IBP.IdentifyBachId
 WHERE IB.RaffleId = @raffleId)q)qq on r.RaffleId = qq.raffleId

 
 -- pagado en nota de credito
  Update RRentabilidadSorteo
 SET RRentabilidadSorteo.PagadoConPremios  = isNull(qq.PagadoConPremios,0)
 From RRentabilidadSorteo r inner join (
 SELECT @raffleId raffleId, sum(isnull(NC.TotalCash,0) - isnull(NC.TotalRest,0)) PagadoConPremios FROM IdentifyBach IB 
 JOIN IdentifyBach_CreditNote INC ON INC.IdentifyBachid = IB.Id
 JOIN NoteCredit NC ON NC.Id = INC.CreditNoteId
 WHERE IB.RaffleId = @raffleId) qq on r.RaffleId = qq.raffleId


 --otros
 Update RRentabilidadSorteo
 SET RRentabilidadSorteo.Otros  = isNull(qq.otros,0)
 From RRentabilidadSorteo r inner join (
 Select @raffleId raffleId, isnull(sum(q.suma),0) otros from (
select 
isnull(rp.TotalCredit,0) + isnull(rp.TotalCheck,0) suma
from invoice i
inner join ReceiptPayment rp
on i.id = rp.InvoiceId  where i.raffleId = @raffleId
and i.paymenttype in (2069,2070)
)q)qq on r.RaffleId = qq.raffleId

end