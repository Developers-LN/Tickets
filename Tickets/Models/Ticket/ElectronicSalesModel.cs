using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;

namespace Tickets.Models.Ticket
{
	public class ElectronicSalesModel
	{
		[JsonProperty(PropertyName = "raffleId")]
		public int RaffleId { get; set; }

		[JsonProperty(PropertyName = "raffleDesc")]
		public string RaffleDesc { get; set; }

		[JsonProperty(PropertyName = "clientId")]
		public int ClientId { get; set; }

		[JsonProperty(PropertyName = "clientDesc")]
		public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "raffleNomenclature")]
        public string RaffleNomenclature { get; set; }

        [JsonProperty(PropertyName = "fractionQuantity")]
		public int FractionQuantity { get; set; }

		[JsonProperty(PropertyName = "allocationFractionQuantity")]
		public int AllocationFractionQuantity { get; set; }

		[JsonProperty(PropertyName = "allocationId")]
		public int AllocationId { get; set; }

		[JsonProperty(PropertyName = "totalRest")]
		public int TotalRest { get; set; }

		[JsonProperty(PropertyName = "electronicTickets")]
		public List<ElectronicSalesNumberModel> ElectronicTickets { get; set; }

		[JsonProperty(PropertyName = "statusId")]
		public int StatusId { get; set; }

		[JsonProperty(PropertyName = "raffleSequence")]
		public int? RaffleSequence { get; set; }

		internal RequestResponseModel GetElectronicSalesDetails(int AllocationId)
		{
			var context = new TicketsEntities();

			var Allocation = context.TicketAllocations.FirstOrDefault(f => f.Id == AllocationId);

			if (Allocation.Client.GroupId == (int)ClientGroupEnum.DistribuidorXML)
			{
				var electronicTickets = context.ElectronicTicketSales
				.Where(w => w.TicketAllocationId == AllocationId).AsEnumerable()
				.Select(s => this.ElectronicSaleObject(s)).FirstOrDefault();

				if (electronicTickets == null)
				{
					electronicTickets = new ElectronicSalesModel()
					{
						ClientId = 0,
						RaffleId = 0,
						ElectronicTickets = new List<ElectronicSalesNumberModel>()
					};
				}
				return new RequestResponseModel()
				{
					Result = true,
					Object = electronicTickets
				};
			}
			else
			{
				var electronicTickets = context.ElectronicTicketSales
				.Where(w => w.TicketAllocationId == AllocationId).AsEnumerable()
				.Select(s => this.ElectronicSaleObject(s)).FirstOrDefault();

				if (electronicTickets == null)
				{
					electronicTickets = new ElectronicSalesModel()
					{
						ClientId = 0,
						RaffleId = 0,
						ElectronicTickets = new List<ElectronicSalesNumberModel>()
					};
				}
				return new RequestResponseModel()
				{
					Result = true,
					Object = electronicTickets
				};
			}
		}

		internal RequestResponseModel GetList(int raffleId, int clientId = 0, int statu = 0)
		{
			var context = new TicketsEntities();

			var Clients = context.Clients
				.Where(w => w.GroupId == (int)ClientGroupEnum.DistribuidorElectronico ||
				w.GroupId == (int)ClientGroupEnum.DistribuidorXML).Select(s => s.Id).ToList();

			var electronicTickets = context.TicketAllocations
				.Where(a => a.RaffleId == raffleId
					&& Clients.Contains(a.ClientId)).AsEnumerable()
				.Select(a => this.ListadoAsignaciones(a)).ToList();

			return new RequestResponseModel()
			{
				Result = true,
				Object = electronicTickets
			};
		}

		internal TicketAllocationModel ListadoAsignaciones(TicketAllocation model)
		{
			var context = new TicketsEntities();
			var AllocationId = model.Id;
			var allocation = new TicketAllocationModel();
			var TotalVentaElectroica = 0;
			var client = context.Clients.FirstOrDefault(f => f.Id == model.ClientId);

			if (client.GroupId == (int)ClientGroupEnum.DistribuidorXML)
			{
				if (context.ElectronicTicketSales.Where(w => w.TicketAllocationId == AllocationId).Any())
				{
					TotalVentaElectroica = context.ElectronicTicketSales.Where(w => w.TicketAllocationId == AllocationId).Select(s => s.FractionTo - s.FractionFrom + 1).Sum();
				}

				allocation = new TicketAllocationModel()
				{
					ClientId = model.ClientId,
					AllocationId = model.Id,
					ClientDesc = model.Client.Name,
					StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Statu).NameDetail,
					CreateDate = model.CreateDate,
					ProspectFraction = model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber,
					FractionQuantity = TotalVentaElectroica % (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber),
					TicketsQuantity = TotalVentaElectroica / (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber),
					StatuId = model.Statu,
					Agente = model.Agente,
					Grupo = context.Clients.FirstOrDefault(c => c.Id == model.ClientId).GroupId,
					AnyReturn = model.Statu,
					AllocationFractionQuantity = context.TicketAllocationNumbers.Any(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated) ? context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated).Select(s => s.FractionTo - s.FractionFrom + 1).Sum() % (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber) : 0,
					AllocationTicketsQuantity = context.TicketAllocationNumbers.Any(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated) ? context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated).Select(s => s.FractionTo - s.FractionFrom + 1).Sum() / (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber) : 0,
				};

				if (context.TicketReturns.Where(w => w.ClientId == model.ClientId && w.RaffleId == model.RaffleId && w.TicketAllocationNumber.TicketAllocation.Id == model.Id).Any())
				{
					var TotalAllocated = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id).Select(s => s.Id).ToList();
					allocation.TotalRest = context.TicketReturns.Where(w => TotalAllocated.Contains(w.TicketAllocationNimberId)).Select(s => s.FractionFrom - s.FractionTo + 1).Sum();
				}
				else
				{
					allocation.TotalRest = 0;
				}
			}
			else if (client.GroupId == (int)ClientGroupEnum.DistribuidorElectronico)
			{
				TotalVentaElectroica = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id).Select(s => s.FractionTo - s.FractionFrom + 1).Sum();

				allocation = new TicketAllocationModel()
				{
					ClientId = model.ClientId,
					AllocationId = model.Id,
					ClientDesc = model.Client.Name,
					StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Statu).NameDetail,
					CreateDate = model.CreateDate,
					ProspectFraction = model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber,
					FractionQuantity = TotalVentaElectroica % (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber),
					TicketsQuantity = TotalVentaElectroica / (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber),
					StatuId = model.Statu,
					Agente = model.Agente,
					Grupo = context.Clients.FirstOrDefault(c => c.Id == model.ClientId).GroupId,
					AnyReturn = model.Statu,
					AllocationFractionQuantity = context.TicketAllocationNumbers.Any(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated) ? context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated).Select(s => s.FractionTo - s.FractionFrom + 1).Sum() % (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber) : 0,
					AllocationTicketsQuantity = context.TicketAllocationNumbers.Any(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated) ? context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id && w.Statu != (int)TicketStatusEnum.Anulated).Select(s => s.FractionTo - s.FractionFrom + 1).Sum() / (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber) : 0,
					TotalRest = context.TicketAllocationNumbers.Any(w => w.TicketAllocationId == model.Id && w.Statu == (int)TicketStatusEnum.Anulated) ? context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id && w.Statu == (int)TicketStatusEnum.Anulated).Select(s => s.FractionTo - s.FractionFrom + 1).Sum() % (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber) : 0,
					TotalRestTickets = context.TicketAllocationNumbers.Any(w => w.TicketAllocationId == model.Id && w.Statu == (int)TicketStatusEnum.Anulated) ? context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id && w.Statu == (int)TicketStatusEnum.Anulated).Select(s => s.FractionTo - s.FractionFrom + 1).Sum() / (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber) : 0
				};
			}

			return allocation;
		}

		/*internal ElectronicSalesModel ToObject(List<ElectronicTicketSale> electronicTickets, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var Allocation = electronicTickets.FirstOrDefault().TicketAllocationId;

            var model = new ElectronicSalesModel()
            {
                RaffleId = electronicTickets.FirstOrDefault().RaffleId,
                RaffleDesc = electronicTickets.FirstOrDefault().Raffle.Name,
                ClientId = electronicTickets.FirstOrDefault().ClientId,
                ClientDesc = electronicTickets.FirstOrDefault().Client.Name,
                FractionQuantity = electronicTickets.Select(t => t.FractionTo - t.FractionFrom + 1).Sum(),
                StatusId = electronicTickets.FirstOrDefault().Statu,
                AllocationId = (int)Allocation,
                AllocationFractionQuantity = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == Allocation).Select(s => s.FractionTo - s.FractionFrom + 1).Sum()
            };

            model.TotalRest = model.AllocationFractionQuantity - model.FractionQuantity;

            if (hasNumber == true)
            {
                var electronicSalesNumberModel = new ElectronicSalesNumberModel();
                model.ElectronicTickets = electronicTickets.Select(t => electronicSalesNumberModel.ToObject(t)).ToList();
            }
            return model;
        }*/

		internal ElectronicSalesModel ElectronicSaleObject(ElectronicTicketSale electronicTicketSale, bool hasNumber = true)
		{
			var context = new TicketsEntities();
			var Allocation = electronicTicketSale.TicketAllocationId;

			var electronicTickets = new ElectronicSalesModel()
			{
				ClientId = electronicTicketSale.ClientId,
				ClientDesc = electronicTicketSale.Client.Name,
                //RaffleDesc = electronicTicketSale.Raffle.Name,
                RaffleNomenclature = electronicTicketSale.Raffle.Symbol + electronicTicketSale.Raffle.Separator + electronicTicketSale.Raffle.Id,
                RaffleDesc = electronicTicketSale.Raffle.Symbol + electronicTicketSale.Raffle.Separator + electronicTicketSale.Raffle.Id + " " + electronicTicketSale.Raffle.Name + " " + electronicTicketSale.Raffle.DateSolteo.ToShortDateString(),
                RaffleId = electronicTicketSale.Raffle.Id,
				RaffleSequence = electronicTicketSale.Raffle.RaffleSequence,
				AllocationId = (int)electronicTicketSale.TicketAllocationId
			};
			if (hasNumber)
			{
				var ElectronicTicketsList = context.ElectronicTicketSales.Where(w => w.TicketAllocationId == Allocation).ToList();

				var GroupByDate = ElectronicTicketsList.GroupBy(g => g.SaleDate.ToShortDateString()).Select(s => new
				{
					date = s.Key,
					totalSold = s.Count()
				}).ToList();

				foreach (var item in GroupByDate)
				{
					var dataElectronicSale = new ElectronicSalesNumberModel()
					{
						DateSold = item.date,
						TotalSold = item.totalSold
					};
					electronicTickets.ElectronicTickets.Add(dataElectronicSale);
				}

			}

			return electronicTickets;
		}
	}
}
