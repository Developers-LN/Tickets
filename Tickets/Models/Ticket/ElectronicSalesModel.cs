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

        internal RequestResponseModel GetElectronicSalesDetails(int AllocationId)
        {
            var context = new TicketsEntities();
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

        internal RequestResponseModel GetList(int raffleId, int clientId = 0, int statu = 0)
        {
            var context = new TicketsEntities();

            var Clients = context.Clients.Where(w => w.GroupId == (int)ClientGroupEnum.DistribuidorElectronico).Select(s => s.Id).ToList();

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
            var TotalVentaElectroica = 0;
            if (context.ElectronicTicketSales.Where(w => w.TicketAllocationId == AllocationId).Any())
            {
                TotalVentaElectroica = context.ElectronicTicketSales.Where(w => w.TicketAllocationId == AllocationId).Select(s => s.FractionTo - s.FractionFrom + 1).Sum();
            }

            var allocation = new TicketAllocationModel()
            {
                ClientId = model.ClientId,
                AllocationId = model.Id,
                ClientDesc = model.Client.Name,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Statu).NameDetail,
                CreateDate = model.CreateDate,
                FractionQuantity = TotalVentaElectroica,
                StatuId = model.Statu,
                Agente = model.Agente,
                Grupo = context.Clients.FirstOrDefault(c => c.Id == model.ClientId).GroupId,
                AnyReturn = model.Statu,
                AllocationFractionQuantity = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == model.Id).Select(s => s.FractionTo - s.FractionFrom + 1).Sum()
            };

            allocation.TotalRest = allocation.AllocationFractionQuantity - allocation.FractionQuantity;

            return allocation;
        }

        internal ElectronicSalesModel ToObject(List<ElectronicTicketSale> electronicTickets, bool hasNumber = false)
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
        }

        internal ElectronicSalesModel ElectronicSaleObject(ElectronicTicketSale electronicTicketSale, bool hasNumber = true)
        {
            var context = new TicketsEntities();
            var Allocation = electronicTicketSale.TicketAllocationId;

            var electronicTickets = new ElectronicSalesModel()
            {
                ClientId = electronicTicketSale.ClientId,
                ClientDesc = electronicTicketSale.Client.Name,
                RaffleDesc = electronicTicketSale.Raffle.Name,
                RaffleId = electronicTicketSale.Raffle.Id,
                AllocationId = (int)electronicTicketSale.TicketAllocationId
            };
            if (hasNumber)
            {
                var ElectronicTicketsList = context.ElectronicTicketSales.Where(w => w.TicketAllocationId == Allocation).ToList();
                foreach (var item in ElectronicTicketsList)
                {
                    var electronicSalesNumberModel = new ElectronicSalesNumberModel()
                    {
                        NumberDesc = item.TicketAllocationNumber.Number,
                        FractionFrom = item.FractionFrom,
                        FractionTo = item.FractionTo
                    };
                    electronicTickets.ElectronicTickets.Add(electronicSalesNumberModel);
                }

            }

            return electronicTickets;
        }
    }
}
