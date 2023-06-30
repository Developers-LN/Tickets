using AttributeRouting.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tickets.Models.Enums;
using Tickets.Models.Procedures;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketInvoiceModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "agencyId")]
        public int AgencyId { get; set; }

        [JsonProperty(PropertyName = "agencyDesc")]
        public string AgencyDesc { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "agente")]
        public string Agente { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

        [JsonProperty(PropertyName = "condition")]
        public int Condition { get; set; }

        [JsonProperty(PropertyName = "taxReceipt")]
        public int TaxReceipt { get; set; }

        [JsonProperty(PropertyName = "taxReceiptName")]
        public string TaxReceiptName { get; set; }

        [JsonProperty(PropertyName = "conditionDesc")]
        public string ConditionDesc { get; set; }

        [JsonProperty(PropertyName = "paymentStatu")]
        public int PaymentStatu { get; set; }

        [JsonProperty(PropertyName = "paymentStatuDesc")]
        public string PaymentStatuDesc { get; set; }

        [JsonProperty(PropertyName = "paymentType")]
        public int PaymentType { get; set; }

        [JsonProperty(PropertyName = "paymentTypeDesc")]
        public string PaymentTypeDesc { get; set; }

        [JsonProperty(PropertyName = "invoiceExpredDay")]
        public int InvoiceExpredDay { get; set; }

        [JsonProperty(PropertyName = "invoiceExpredDate")]
        public DateTime InvoiceExpredDate { get; set; }

        [JsonProperty(PropertyName = "craeteDate")]
        public DateTime CraeteDate { get; set; }

        [JsonProperty(PropertyName = "craeteDateLong")]
        public long CraeteDateLong { get; set; }

        [JsonProperty(PropertyName = "invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [JsonProperty(PropertyName = "invoiceDateLong")]
        public long InvoiceDateLong { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public int Statu { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        [JsonProperty(PropertyName = "createUser")]
        public int CreateUser { get; set; }

        [JsonProperty(PropertyName = "createUserDesc")]
        public string CreateUserDesc { get; set; }

        [JsonProperty(PropertyName = "ticketInvoiceNumbers")]
        public List<TicketInvoiceNumberModel> TicketInvoiceNumbers { get; set; }

        [JsonProperty(PropertyName = "ticketAllocations")]
        public List<TicketAllocationModel> TicketAllocations { get; set; }

        [JsonProperty(PropertyName = "ticketPrice")]
        public decimal TicketPrice { get; set; }

        [JsonProperty(PropertyName = "poolPrice")]
        public decimal PoolPrice { get; set; }

        [JsonProperty(PropertyName = "fractionCount")]
        public int FractionCount { get; set; }

        [JsonProperty(PropertyName = "taxReceiptHistory")]
        public int TaxReceiptHistory { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "invoiceStatusProv")]
        public string InvoiceStatusProv { get; set; }

        internal TicketInvoiceModel InvoiceDetails(Invoice model, bool hasAllocaation = false, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            int expiredDay = model.InvoiceExpredDay ?? 14;
            var expiredDate = model.InvoiceDate.AddDays(expiredDay);
            var xpiredDate = model.InvoiceDate.AddDays(model.InvoiceExpredDay.Value);
            //int productType = 5821; // model.InvoiceTickets.FirstOrDefault().TicketAllocationNumber.TicketAllocation.Type;
            var invoice = new TicketInvoiceModel()
            {
                Id = model.Id,
                AgencyId = model.AgencyId,
                AgencyDesc = context.Agencies.Where(r => r.Id == model.AgencyId).Select(c => c.Name).FirstOrDefault(),
                ClientId = model.ClientId,
                ClientDesc = context.Clients.Where(r => r.Id == model.ClientId).Select(c => c.Name).FirstOrDefault(),
                Agente = context.Clients.Where(r => r.Id == model.ClientId).Select(c => c.TicketAllocations.Select(a => a.Agente).FirstOrDefault()).FirstOrDefault(),
                RaffleId = model.RaffleId,
                RaffleDesc = context.Raffles.Where(r => r.Id == model.RaffleId).Select(c => c.Name).FirstOrDefault(),
                PaymentType = model.PaymentType,
                PaymentTypeDesc = context.Catalogs.Where(r => r.Id == model.PaymentType).Select(c => c.NameDetail).FirstOrDefault(),
                InvoiceExpredDay = expiredDay,
                InvoiceExpredDate = expiredDate,
                CraeteDate = model.CreateDate,
                CraeteDateLong = model.CreateDate.ToUnixTime(),
                InvoiceDate = model.InvoiceDate,
                InvoiceDateLong = model.InvoiceDate.ToUnixTime(),
                Condition = model.Condition,
                TaxReceipt = (int)((model.TaxReceipt == null || model.TaxReceipt == 0) ? (int)TaxReceiptTypeEnum.NotReceipt : model.TaxReceipt),
                TaxReceiptName = (model.TaxReceipt == null || model.TaxReceipt == 0) ? "SIN COMPROBANTE" : context.Catalogs.Where(w => w.Id == model.TaxReceipt).Select(s => s.NameDetail).FirstOrDefault(),
                InvoiceStatusProv = (xpiredDate.Date < DateTime.Now.Date && model.PaymentStatu == 2082) ? "Caducada" : "",
                ConditionDesc = context.Catalogs.Where(r => r.Id == model.Condition).Select(c => c.NameDetail).FirstOrDefault(),
                PaymentStatu = model.PaymentStatu,
                PaymentStatuDesc = context.Catalogs.Where(r => r.Id == model.PaymentStatu).Select(c => c.NameDetail).FirstOrDefault(),
                Statu = model.Statu,
                StatuDesc = context.Catalogs.Where(r => r.Id == model.Statu).Select(c => c.NameDetail).FirstOrDefault(),
                CreateUser = model.CreateUser,
                CreateUserDesc = context.Users.Where(r => r.Id == model.CreateUser).Select(c => c.Name).FirstOrDefault(),
                //TicketAllocations = new List<TicketAllocationModel>(),
                //TicketInvoiceNumbers = new List<TicketInvoiceNumberModel>(),
                FractionCount = model.InvoiceTickets.Select(s => s.TicketAllocationNumber.FractionTo - s.TicketAllocationNumber.FractionFrom - 1).Sum()
            };
            if (hasAllocaation == true)
            {
                var ticketAllocationModel = new TicketAllocationModel();
                invoice.TicketAllocations = model.InvoiceTickets.GroupBy(a => a.TicketAllocationNumber.TicketAllocationId).Select(a => ticketAllocationModel.ToObject(a.FirstOrDefault().TicketAllocationNumber.TicketAllocation, false, false)).ToList();
            }
            return invoice;
        }

        internal TicketInvoiceModel ToObject(Invoice model, bool hasAllocaation = false, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            int expiredDay = model.InvoiceExpredDay ?? 14;
            var expiredDate = model.InvoiceDate.AddDays(expiredDay);
            var xpiredDate = model.InvoiceDate.AddDays(model.InvoiceExpredDay.Value);
            //int productType = 5821; // model.InvoiceTickets.FirstOrDefault().TicketAllocationNumber.TicketAllocation.Type;
            var invoice = new TicketInvoiceModel()
            {
                Id = model.Id,
                AgencyId = model.AgencyId,
                AgencyDesc = context.Agencies.Where(r => r.Id == model.AgencyId).Select(c => c.Name).FirstOrDefault(),
                ClientId = model.ClientId,
                ClientDesc = context.Clients.Where(r => r.Id == model.ClientId).Select(c => c.Name).FirstOrDefault(),
                Agente = context.Clients.Where(r => r.Id == model.ClientId).Select(c => c.TicketAllocations.Select(a => a.Agente).FirstOrDefault()).FirstOrDefault(),
                RaffleId = model.RaffleId,
                RaffleDesc = context.Raffles.Where(r => r.Id == model.RaffleId).Select(c => c.Name).FirstOrDefault(),
                PaymentType = model.PaymentType,
                PaymentTypeDesc = context.Catalogs.Where(r => r.Id == model.PaymentType).Select(c => c.NameDetail).FirstOrDefault(),
                InvoiceExpredDay = expiredDay,
                InvoiceExpredDate = expiredDate,
                CraeteDate = model.CreateDate,
                CraeteDateLong = model.CreateDate.ToUnixTime(),
                InvoiceDate = model.InvoiceDate,
                InvoiceDateLong = model.InvoiceDate.ToUnixTime(),
                Condition = model.Condition,
                InvoiceStatusProv = (xpiredDate.Date < DateTime.Now.Date && model.PaymentStatu == 2082) ? "Caducada" : "",
                ConditionDesc = context.Catalogs.Where(r => r.Id == model.Condition).Select(c => c.NameDetail).FirstOrDefault(),
                PaymentStatu = model.PaymentStatu,
                PaymentStatuDesc = context.Catalogs.Where(r => r.Id == model.PaymentStatu).Select(c => c.NameDetail).FirstOrDefault(),
                Statu = model.Statu,
                StatuDesc = context.Catalogs.Where(r => r.Id == model.Statu).Select(c => c.NameDetail).FirstOrDefault(),
                CreateUser = model.CreateUser,
                CreateUserDesc = context.Users.Where(r => r.Id == model.CreateUser).Select(c => c.Name).FirstOrDefault(),
                TaxReceiptHistory = model.TaxReceiptNumbersHistories.Any() ? model.TaxReceiptNumbersHistories.OrderByDescending(o => o.Id).FirstOrDefault().TaxReceiptAssignmentDate.Value.Date > model.CreateDate.Date ? 1 : 0 : 0,
                TicketAllocations = new List<TicketAllocationModel>(),
                TicketInvoiceNumbers = new List<TicketInvoiceNumberModel>()
            };
            if (hasNumber == true)
            {
                var ticketInvoiceNumberModel = new TicketInvoiceNumberModel();
                invoice.TicketInvoiceNumbers = model.InvoiceTickets.Select(i => ticketInvoiceNumberModel.ToObject(i)).ToList();
            }
            if (hasAllocaation == true)
            {
                var ticketAllocationModel = new TicketAllocationModel();
                invoice.TicketAllocations = model.InvoiceTickets.GroupBy(a => a.TicketAllocationNumber.TicketAllocationId).Select(a => ticketAllocationModel.ToObject(a.FirstOrDefault().TicketAllocationNumber.TicketAllocation, false, false)).ToList();
            }
            return invoice;
        }

        internal RequestResponseModel GetInvoiceList(int raffleId, int clientId = 0, int statu = 0)
        {
            var context = new TicketsEntities();
            var invoices = context.Invoices.AsEnumerable().Where(i =>
                    i.RaffleId == raffleId
                    && (i.ClientId == clientId || clientId == 0)
                    && (i.PaymentStatu == statu || statu == 0)
                ).Select(dt => this.ToObject(dt)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = invoices
            };
        }

        internal RequestResponseModel GetInvoice(int id)
        {
            var context = new TicketsEntities();

            var invoice = context.Invoices.Where(i => i.Id == id).AsEnumerable()
                .Select(dt => this.InvoiceDetails(dt, true, true)).FirstOrDefault();

            if (invoice == null)
            {
                invoice = new TicketInvoiceModel()
                {
                    Id = 0,
                    Condition = 0,
                    InvoiceExpredDay = 14,
                    InvoiceDateLong = DateTime.Now.ToUnixTime(),
                    TicketAllocations = new List<TicketAllocationModel>()
                };
            }

            return new RequestResponseModel()
            {
                Result = true,
                Object = invoice
            };
        }

        internal RequestResponseModel Save(TicketInvoiceModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        Invoice invoice;
                        if (model.Id == 0)
                        {
                            var client = context.Clients.FirstOrDefault(c => c.Id == model.ClientId);

                            var expiredInvoices = (from i in context.Invoices
                                                   join it in context.InvoiceTickets on i.Id equals it.InvoiceId
                                                   where i.ClientId == model.ClientId
                                                     && i.Condition == (int)InvoiceConditionEnum.Credit
                                                     && i.PaymentStatu == (int)InvoicePaymentStatuEnum.Pendient
                                                   select new
                                                   {
                                                       i.ClientId,
                                                       i.Condition,
                                                       i.Id,
                                                       i.PaymentStatu,
                                                       i.InvoiceDate,
                                                       i.InvoiceExpredDay,
                                                       it.Quantity,
                                                       it.PricePerFraction
                                                   }).AsEnumerable().GroupBy(i => i.Id).Select(ig => new
                                                   {
                                                       ig.FirstOrDefault().ClientId,
                                                       ig.FirstOrDefault().Condition,
                                                       ig.FirstOrDefault().Id,
                                                       ig.FirstOrDefault().PaymentStatu,
                                                       ig.FirstOrDefault().InvoiceDate,
                                                       ig.FirstOrDefault().InvoiceExpredDay,
                                                       InvoiceTickets = ig.Select(it => new
                                                       {
                                                           it.Quantity,
                                                           it.PricePerFraction
                                                       }).ToList()
                                                   }).ToList();


                            if (expiredInvoices.Where(i => DateTime.Now > i.InvoiceDate.AddDays(i.InvoiceExpredDay ?? 45)).ToList().Count > 0)
                            {
                                tx.Rollback();
                                return new RequestResponseModel()
                                {
                                    Result = false,
                                    Message = "El cliente tiene facturas pendiente."
                                };
                            }

                            decimal totalInvoiceData = 0;
                            var invoice_ids = expiredInvoices.Select(i => i.Id).ToArray();
                            var payments = (from rp in context.ReceiptPayments
                                            join nc in context.NoteCreditReceiptPayments on rp.Id equals nc.ReceiptPaymentId into ncj
                                            from nc in ncj.DefaultIfEmpty()
                                            where invoice_ids.Contains(rp.InvoiceId)
                                            select new
                                            {
                                                rp.Id,
                                                rp.InvoiceId,
                                                rp.TotalCash,
                                                rp.TotalCheck,
                                                rp.TotalCredit,
                                                nc_totalCash = nc == null ? 0 : nc.TotalCash
                                            }).AsEnumerable().GroupBy(rp => rp.Id).Select(rpg => new
                                            {
                                                rpg.FirstOrDefault().Id,
                                                rpg.FirstOrDefault().InvoiceId,
                                                rpg.FirstOrDefault().TotalCash,
                                                rpg.FirstOrDefault().TotalCheck,
                                                rpg.FirstOrDefault().TotalCredit,
                                                NoteCreditReceiptPayments = rpg.Select(nc => new
                                                {
                                                    nc.TotalCash
                                                }).ToList()
                                            }).ToList();

                            foreach (var expiredInvoice in expiredInvoices)
                            {
                                foreach (var invoiceTicket in expiredInvoice.InvoiceTickets)
                                {
                                    totalInvoiceData += (invoiceTicket.Quantity * invoiceTicket.PricePerFraction);
                                }

                                var totalCreditNote = 0.0M;
                                var totalRequestCash = 0.0M;

                                var receiptPayments = payments.Select(rp => new
                                {
                                    rp.InvoiceId,
                                    rp.TotalCash,
                                    rp.TotalCheck,
                                    rp.TotalCredit,
                                    rp.NoteCreditReceiptPayments
                                }).Where(r => r.InvoiceId == expiredInvoice.Id).ToList();

                                receiptPayments.ForEach(r => r.NoteCreditReceiptPayments.ToList().ForEach(rn => totalCreditNote += rn.TotalCash));

                                receiptPayments.Select(r => r.TotalCash + r.TotalCheck + r.TotalCredit).ToList().ForEach(t => totalRequestCash += t);
                                totalInvoiceData -= (totalCreditNote + totalRequestCash);
                            }

                            var allocation_ids = model.TicketAllocations.Select(a => a.Id).ToArray();

                            var allocatList = context.TicketAllocations.Where(a => allocation_ids.Contains(a.Id)).ToList();

                            allocatList.ForEach(a => a.TicketAllocationNumbers.ToList().ForEach(n => totalInvoiceData += (n.FractionTo - n.FractionFrom + 1) * model.TicketPrice));

                            if ((totalInvoiceData - ((totalInvoiceData * client.Discount) / 100)) >= client.CreditLimit)
                            {
                                tx.Rollback();
                                return new RequestResponseModel()
                                {
                                    Result = false,
                                    Message = "El cliente agoto el limite de credito."
                                };
                            }
                            var user = context.Users.Where(u => u.Id == WebSecurity.CurrentUserId).FirstOrDefault();

                            int? taxReceiptNumber = 0;

                            if (model.TaxReceipt != (int)TaxReceiptTypeEnum.NotReceipt)
                            {
                                taxReceiptNumber = context.TaxReceipts
                                .Where(w => w.DueDate >= DateTime.Now && w.Statu == (int)TaxReceiptStatuEnum.Activo && w.Type == model.TaxReceipt)
                                .Select(f => f.TaxReceiptNumbers.Where(w2 => w2.Status == (int)TaxReceiptNumberStatuEnum.Disponible)
                                .Select(f2 => f2.Id).FirstOrDefault()).FirstOrDefault();
                            }

                            invoice = new Invoice()
                            {
                                PaymentType = model.PaymentType,
                                InvoiceDate = model.InvoiceDate,
                                Condition = model.Condition,
                                Discount = client.Discount,
                                InvoiceExpredDay = model.InvoiceExpredDay <= 0 ? 30 : model.InvoiceExpredDay,
                                ClientId = model.ClientId,
                                RaffleId = model.RaffleId,
                                AgencyId = user.Employee.AgencyId,
                                Statu = (int)GeneralStatusEnum.Active,
                                OutstandingBalance = 0.0M,
                                CreateUser = WebSecurity.CurrentUserId,
                                CreateDate = DateTime.Now,
                                TaxReceipt = taxReceiptNumber.Value.HasNoValue() ? 0 : taxReceiptNumber.Value,
                                TaxReceiptAssignmentDate = DateTime.Now
                            };

                            if (model.Condition == (int)InvoiceConditionEnum.Credit)
                            {
                                invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Pendient;
                            }
                            else
                            {
                                invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Pendient;
                                //invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Payed;
                            }

                            if (taxReceiptNumber.Value != 0 || taxReceiptNumber != 0 || taxReceiptNumber > 1 || taxReceiptNumber.Value > 1)
                            {
                                var lasTaxReceiptNumber = context.TaxReceiptNumbers.Where(w => w.Id == taxReceiptNumber).FirstOrDefault();
                                lasTaxReceiptNumber.Status = (int)TaxReceiptNumberStatuEnum.Ocupado;

                                var taxReceiptOut = context.TaxReceipts.Where(w => w.Id == lasTaxReceiptNumber.TaxReceiptId).FirstOrDefault();

                                if (context.TaxReceiptNumbers.Where(w => w.TaxReceiptId == taxReceiptOut.Id).Any(a => a.Status == (int)TaxReceiptNumberStatuEnum.Disponible) == false)
                                {
                                    taxReceiptOut.Statu = (int)TaxReceiptStatuEnum.Agotado;
                                }
                                if (context.TaxReceipts.Where(w => w.Id == lasTaxReceiptNumber.TaxReceiptId && w.DueDate < DateTime.Now).Any() == true)
                                {
                                    taxReceiptOut.Statu = (int)TaxReceiptStatuEnum.Caducado;
                                }
                            }

                            context.Invoices.Add(invoice);
                            context.SaveChanges();

                            if (taxReceiptNumber.Value != 0 || taxReceiptNumber != 0 || taxReceiptNumber > 1 || taxReceiptNumber.Value > 1)
                            {
                                var taxReceiptHistory = new TaxReceiptNumbersHistory()
                                {
                                    InvoiceId = invoice.Id,
                                    TaxReceiptId = taxReceiptNumber.Value,
                                    TaxReceiptAssignmentDate = DateTime.Now
                                };

                                context.TaxReceiptNumbersHistories.Add(taxReceiptHistory);
                                context.SaveChanges();
                            }

                            var allocations = allocatList;//context.TicketAllocations.AsEnumerable().Where(a => allocation_ids.Contains( a.Id) ).ToList();

                            List<InvoiceTicket> invoiceTicketList = new List<InvoiceTicket>();
                            foreach (var allocation in allocations)
                            {
                                Procedure_AvailableTicketToInvoice availableTicketToInvoice = new Procedure_AvailableTicketToInvoice();
                                var Resultado = availableTicketToInvoice.AvailableTicketsToInvoice(model.RaffleId, allocation.Id);

                                var clientType = context.TicketAllocations.Where(w => w.Id == allocation.Id).Select(s => s.Client.GroupId).FirstOrDefault();

                                if (clientType == (int)ClientGroupEnum.DistribuidorElectronico)
                                {
                                    foreach (var number in allocation.TicketAllocationNumbers)
                                    {
                                        if (Resultado.Any(a => a.AllocationNumberId == number.Id))
                                        {
                                            invoiceTicketList.Add(new InvoiceTicket()
                                            {
                                                InvoiceId = invoice.Id,
                                                PricePerFraction = model.TicketPrice,
                                                Quantity = Resultado.Where(w => w.AllocationNumberId == number.Id).FirstOrDefault().AvailableFractions,
                                                TicketNumberAllocationId = number.Id,
                                            });
                                            number.Invoiced = true;
                                            number.Statu = (int)TicketStatusEnum.Factured;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var number in allocation.TicketAllocationNumbers)
                                    {
                                        if (Resultado.Any(a => a.AllocationNumberId == number.Id))
                                        {
                                            invoiceTicketList.Add(new InvoiceTicket()
                                            {
                                                InvoiceId = invoice.Id,
                                                PricePerFraction = model.TicketPrice,
                                                Quantity = Resultado.Where(w => w.AllocationNumberId == number.Id).FirstOrDefault().AvailableFractions,
                                                TicketNumberAllocationId = number.Id,
                                            });
                                            number.Invoiced = true;
                                            number.Statu = (int)TicketStatusEnum.Factured;
                                        }
                                        else
                                        {
                                            number.Invoiced = false;
                                            number.Statu = (int)TicketStatusEnum.Returned;
                                        }
                                    }
                                }

                                allocation.Statu = (int)AllocationStatuEnum.Invoiced;
                            }
                            context.InvoiceTickets.AddRange(invoiceTicketList);
                        }
                        else
                        {
                            invoice = context.Invoices.FirstOrDefault(i => i.Id == model.Id);
                            invoice.InvoiceExpredDay = model.InvoiceExpredDay;
                            invoice.Condition = model.Condition;
                            if (model.Condition == (int)InvoiceConditionEnum.Credit)
                            {
                                invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Pendient;
                            }
                            else
                            {
                                invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Pendient;
                                //invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Payed;
                            }
                        }
                        context.SaveChanges();
                        tx.Commit();
                        model.Id = invoice.Id;
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Creación de Factura", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Entrega de Billetes Completada.",
                Object = model
            };
        }

        internal RequestResponseModel Suspend(TicketInvoiceModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var invoice = context.Invoices.Where(i => i.Id == model.Id).FirstOrDefault();
                        if (invoice == null)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "No se encontró esta factura."
                            };
                        }
                        if (invoice.PaymentStatu == (int)InvoicePaymentStatuEnum.Payed)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "Esta factura ya fue pagada."
                            };
                        }
                        if (invoice.PaymentStatu == (int)GeneralStatusEnum.Delete)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "Esta factura ya fue eliminada."
                            };
                        }

                        var receiptPayment = context.ReceiptPayments.Where(i => i.InvoiceId == invoice.Id).FirstOrDefault();
                        if (receiptPayment != null)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "Esta factura ya tiene pagos realizados"
                            };
                        }

                        var AllocationList = (from it in context.InvoiceTickets
                                              join tan in context.TicketAllocationNumbers on it.TicketNumberAllocationId equals tan.Id
                                              join ta in context.TicketAllocations on tan.TicketAllocationId equals ta.Id
                                              where it.InvoiceId == invoice.Id
                                              group ta by ta.Id into tal
                                              select new { id = tal.Key }).ToList();

                        var invoiceTicket = context.InvoiceTickets.Where(i => i.InvoiceId == invoice.Id).ToList();
                        if (invoiceTicket != null)
                        {
                            context.InvoiceTickets.RemoveRange(invoice.InvoiceTickets);
                            context.SaveChanges();
                        }

                        if (AllocationList != null)
                        {
                            foreach (var allocationList in AllocationList)
                            {
                                var Allocation = context.TicketAllocations.Where(a => a.Id == allocationList.id).FirstOrDefault();
                                Allocation.Statu = (int)AllocationStatuEnum.Review;
                                context.SaveChanges();
                            }
                        }

                        invoice.DeleteUser = WebSecurity.CurrentUserId;
                        invoice.Statu = (int)GeneralStatusEnum.Delete;
                        invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Suspended;

                        context.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.Message
                        };
                    }
                }
            }
            return new RequestResponseModel()
            {
                Result = true,
                Message = "Factura suspendida correctamente."
            };
        }
    }
}
