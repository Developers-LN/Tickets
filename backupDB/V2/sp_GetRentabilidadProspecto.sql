create procedure sp_GetRentabilidadProspecto
@raffleId int 
as
select sum( a.totalValue) totalProspecto from Award a
join Prospect p on p.Id = a.ProspectId join
Raffle r on r.ProspectId = p.Id
where r.id = @raffleId