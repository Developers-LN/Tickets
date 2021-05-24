declare @raffleId int
set @raffleId = 3745

select 
	r.ControlNumber, 
	count(r.Id) as 'duplicado',
	STUFF(
	(SELECT ' ),( ' + a1.Name
	FROM RaffleAward s
	inner join Award a1 on a1.Id = s.AwardId
	WHERE r.ControlNumber = s.ControlNumber and (a1.TypesAwardId <> 9 and a1.TypesAwardId <> 7 and a1.TypesAwardId <> 8 and a1.TypesAwardId <> 10 and a1.TypesAwardId <> 6)
	FOR XML PATH('')),1,1,'') AS CSV

from RaffleAward r
inner join Award a on a.Id = r.AwardId
where r.RaffleId = @raffleId and (a.TypesAwardId <> 9 and a.TypesAwardId <> 7 and a.TypesAwardId <> 8 and a.TypesAwardId <> 10 and a.TypesAwardId <> 6)
group by r.ControlNumber
having count(r.Id) > 1