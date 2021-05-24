
USE Tickets;
GO

DECLARE @rafleId int;
set @rafleId = 6048;

DELETE  r FROM TicketReturn AS r
WHERE   r.RaffleId = @rafleId;

DELETE  r FROM ReturnedOpen AS r
WHERE   r.RaffleId = @rafleId;

DELETE  ra FROM RaffleAward AS ra
WHERE   ra.RaffleId = @rafleId;

DELETE  ta
FROM    TicketAllocationNumber AS ta 
LEFT JOIN
        TicketAllocation AS a 
ON      a.Id = ta.TicketAllocationId
WHERE   a.RaffleId = @rafleId;

DELETE  a FROM TicketAllocation AS a 
WHERE   a.RaffleId = @rafleId;

DELETE r FROM Raffle as r WHERE r.Id = @rafleId;

GO

