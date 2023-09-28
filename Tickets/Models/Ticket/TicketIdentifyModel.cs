using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.AuxModels;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketIdentifyModel
    {
        internal object GetIdentifyListToPay(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();

            var raffles = new List<object>();
            var clients = new List<object>();
            var identifyBachs = new List<object>();
            if (raffleId == 0 && clientId == 0)
            {
                raffles = context.Raffles.Where(s => s.Statu == (int)RaffleStatusEnum.Generated).Select(r => new
                {
                    r.Id,
                    r.RaffleSequence,
                    r.Name
                }).ToList<object>();


                clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                {
                    r.Id,
                    r.Name,
                }).ToList<object>();
            }
            else
            {
                identifyBachs = context.IdentifyBaches.Where(a =>
                    (a.RaffleId == raffleId || raffleId == 0)
                    && (a.ClientId == clientId || clientId == 0)
                    && a.Statu == (int)BachIdentifyStatuEnum.Approved)
                    .AsEnumerable()
                    .Select(t => this.ListaIdentificacionPremios(t)).ToList();
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Lotes de Billetes");

            return new { result = true, identifyBachs, raffles, clients };
        }

        internal object ObtenerListaIdentificacionPremios(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();

            var raffles = new List<object>();
            var clients = new List<object>();
            var identifyBachs = new List<object>();
            if (raffleId == 0 && clientId == 0)
            {
                raffles = context.Raffles.Where(s => s.Statu == (int)RaffleStatusEnum.Generated).Select(r => new
                {
                    r.Id,
                    r.RaffleSequence,
                    r.Name
                }).ToList<object>();


                clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                {
                    r.Id,
                    r.Name
                }).ToList<object>();
            }
            else
            {
                //var awards = context.RaffleAwards.Where(r => r.RaffleId == raffleId).ToList();
                identifyBachs = context.IdentifyBaches.Where(a =>
                    (a.RaffleId == raffleId || raffleId == 0)
                    && (a.ClientId == clientId || clientId == 0)
                    && a.IdentifyType == (int)IdentifyBachTypeEnum.Gamers)
                    .AsEnumerable()
                    .Select(t => this.ListaIdentificacionPremios(t)).ToList();
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Lotes de Billetes");

            return new { result = true, identifyBachs, raffles, clients };
        }

        internal object ObtenerListaIdentificacionPremiosVendedor(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();

            var raffles = new List<object>();
            var clients = new List<object>();
            var identifyBachs = new List<object>();
            if (raffleId == 0 && clientId == 0)
            {
                raffles = context.Raffles.Where(s => s.Statu == (int)RaffleStatusEnum.Generated).Select(r => new
                {
                    r.Id,
                    r.RaffleSequence,
                    r.Name
                }).ToList<object>();


                clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                {
                    r.Id,
                    r.Name
                }).ToList<object>();
            }
            else
            {
                //var awards = context.RaffleAwards.Where(r => r.RaffleId == raffleId).ToList();
                identifyBachs = context.IdentifyBaches.Where(a =>
                    (a.RaffleId == raffleId || raffleId == 0)
                    && (a.ClientId == clientId || clientId == 0)
                    && a.IdentifyType == (int)IdentifyBachTypeEnum.Sellers)
                    .AsEnumerable()
                    .Select(t => this.ListaIdentificacionPremiosVendedor(t)).ToList();
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Lotes de Billetes");

            return new { result = true, identifyBachs, raffles, clients };
        }

        internal object GetIdentifyList(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();

            var raffles = new List<object>();
            var clients = new List<object>();
            var identifyBachs = new List<object>();
            if (raffleId == 0 && clientId == 0)
            {
                raffles = context.Raffles.Where(s => s.Statu == (int)RaffleStatusEnum.Generated).Select(r => new
                {
                    r.Id,
                    r.RaffleSequence,
                    r.Name,
                    Prices = r.Prospect.Prospect_Price.Select(p => new
                    {
                        p.PriceId,
                        p.FactionPrice
                    })
                }).ToList<object>();


                clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.PriceId
                }).ToList<object>();
            }
            else
            {
                var awards = context.RaffleAwards.Where(r => r.RaffleId == raffleId).ToList();
                identifyBachs = context.IdentifyBaches.Where(a =>
                    (a.RaffleId == raffleId || raffleId == 0)
                    && (a.ClientId == clientId || clientId == 0))
                    .AsEnumerable()
                    .Select(t => this.IdentifyBachMainToObject(t, awards)).ToList();
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Lotes de Billetes");

            return new { result = true, identifyBachs, raffles, clients };
        }

        internal object GetIdentifyDetilsData(int identifyId)
        {
            var context = new TicketsEntities();
            var identifyBach = context.IdentifyBaches.Where(a => a.Id == identifyId).FirstOrDefault();
            if (identifyBach == null)
            {
                return new
                {
                    result = false,
                    message = "No se encontro el lote."
                };
            }
            var pricePerFraction = identifyBach.Raffle.Prospect.Prospect_Price.Where(p => p.PriceId == identifyBach.Client.PriceId).Select(p => p.FactionPrice).FirstOrDefault();

            if (identifyBach.IdentifyType == (int)IdentifyBachTypeEnum.Gamers)
            {
                return new
                {
                    result = true,
                    identifyBach = this.IdentifyBachToObject(identifyBach),
                    pricePerFraction
                };
            }
            else
            {
                return new
                {
                    result = true,
                    identifyBach = this.IdentifyBachSellerToObject(identifyBach),
                    pricePerFraction
                };
            }
        }

        internal object GetIdentifySellerDetilsData(int identifyId)
        {
            var context = new TicketsEntities();
            var identifyBach = context.IdentifyBaches.Where(a => a.Id == identifyId).FirstOrDefault();
            if (identifyBach == null)
            {
                return new
                {
                    result = false,
                    message = "No se encontro el lote."
                };
            }
            var pricePerFraction = identifyBach.Raffle.Prospect.Prospect_Price.Where(p => p.PriceId == identifyBach.Client.PriceId).Select(p => p.FactionPrice).FirstOrDefault();
            return new
            {
                result = true,
                identifyBach = this.IdentifyBachSellerToObject(identifyBach),
                pricePerFraction
            };
        }

        internal object GetIdentifyData(int identifyId)
        {
            var context = new TicketsEntities();
            /*var config = context.SystemConfigs.FirstOrDefault();
            int xpiredDays = 90;
            if (config != null)
            {
                var xpired = context.Catalogs.FirstOrDefault(c => c.Id == config.RaffleXpiredTime);
                if (xpired != null)
                {
                    xpiredDays = int.Parse(xpired.Description);
                }
            }*/
            var date = DateTime.Now;
            var raffles = context.Raffles.Where(s =>
                s.Statu == (int)RaffleStatusEnum.Generated
                && date <= s.DueRaffleDate
                ).Select(r => new
                {
                    r.Id,
                    r.RaffleSequence,
                    r.Name,
                    production = r.Prospect.Production,
                    Prices = r.Prospect.Prospect_Price.Select(p => new
                    {
                        p.PriceId,
                        p.FactionPrice
                    }).ToList()
                }).OrderByDescending(o => o.Id).ToList();

            var clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
            {
                r.Id,
                r.Name,
                r.PriceId
            }).ToList();

            var winners = context.Winners.Select(s => new
            {
                s.Id,
                Document = s.DocumentNumber,
                Name = s.WinnerName,
                s.Phone,
            }).ToList();

            var documentTypes = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.DocumentType && w.Statu == true).Select(s => new
            {
                s.Id,
                Name = s.NameDetail
            }).ToList();

            var genders = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.Gender && w.Statu == true).Select(s => new
            {
                s.Id,
                Name = s.NameDetail
            }).ToList();

            var identifyBach = context.IdentifyBaches.Where(a => a.Id == identifyId).AsEnumerable()
                .Select(t => this.IdentifyBachToObject(t)).ToList().FirstOrDefault();

            return new { result = true, identifyBach, raffles, clients, winners, documentTypes, genders };
        }

        internal object GetIdentifySellerData(int identifyId)
        {
            var context = new TicketsEntities();
            /*var config = context.SystemConfigs.FirstOrDefault();
            int xpiredDays = 90;
            if (config != null)
            {
                var xpired = context.Catalogs.FirstOrDefault(c => c.Id == config.RaffleXpiredTime);
                if (xpired != null)
                {
                    xpiredDays = int.Parse(xpired.Description);
                }
            }*/
            var date = DateTime.Now;
            var raffles = context.Raffles.Where(s =>
                s.Statu == (int)RaffleStatusEnum.Generated
                && date <= s.DueRaffleDate
                ).Select(r => new
                {
                    r.Id,
                    r.RaffleSequence,
                    r.Name,
                    production = r.Prospect.Production,
                    Prices = r.Prospect.Prospect_Price.Select(p => new
                    {
                        p.PriceId,
                        p.FactionPrice
                    }).ToList()
                }).OrderByDescending(o => o.Id).ToList();

            var clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
            {
                r.Id,
                r.Name,
                r.PriceId
            }).ToList();

            var winners = context.Winners.Select(s => new
            {
                s.Id,
                Document = s.DocumentNumber,
                Name = s.WinnerName,
                s.Phone,
            }).ToList();

            var documentTypes = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.DocumentType && w.Statu == true).Select(s => new
            {
                s.Id,
                Name = s.NameDetail
            }).ToList();

            var genders = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.Gender && w.Statu == true).Select(s => new
            {
                s.Id,
                Name = s.NameDetail
            }).ToList();

            var identifyBach = context.IdentifyBaches.Where(a => a.Id == identifyId).AsEnumerable()
                .Select(t => this.IdentifyBachSellerToObject(t)).ToList().FirstOrDefault();

            return new { result = true, identifyBach, raffles, clients, winners, documentTypes, genders };
        }

        internal object CertificationAwardData(int iNumberId, int number, int fractionFrom, int fractionTo, int raffleAwardId, int fractions)
        {
            object certificateObject;

            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var certification = new AwardCertification()
                        {
                            IdentifyNumberId = iNumberId,
                            Number = number,
                            FractionFrom = fractionFrom,
                            FractionTo = fractionTo,
                            Fractions = fractions,
                            RaffleAwardId = raffleAwardId,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now
                        };
                        context.AwardCertification.Add(certification);
                        var identifyNumber = context.IdentifyNumbers.FirstOrDefault(w => w.Id == iNumberId);
                        identifyNumber.Status = (int)AwardCertificationStatuEnum.Certified;
                        context.SaveChanges();
                        tx.Commit();
                        certificateObject = new
                        {
                            certification.Id,
                            IdentifyNumberId = iNumberId,
                            Number = number,
                            FractionFrom = fractionFrom,
                            FractionTo = fractionTo,
                            Fractions = fractions,
                            RaffleAwardId = raffleAwardId,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now
                        };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new { result = false, message = e.Message };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Certificación de Premios mayores", certificateObject);
            return new { result = true, certificateObject, message = "Certificación de Premio Realizada" };

        }

        internal object CertificationAwardData(int number, int raffleAwardId, int fractions)
        {
            object certificateObject;

            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var certification = new CertificationNumber()
                        {
                            Number = number,
                            RaffleAwardId = raffleAwardId,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now,
                            Fractions = fractions
                        };
                        context.CertificationNumbers.Add(certification);
                        context.SaveChanges();
                        tx.Commit();
                        certificateObject = new
                        {
                            certification.Id,
                            Number = number,
                            RaffleAwardId = raffleAwardId,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now
                        };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new { result = false, message = e.Message };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Certificación de Premio devueltos", certificateObject);
            return new { result = true, certificateObject, message = "Certificación de Premio Realizada" };

        }

        internal object GetClientNumbers(int clientId, int raffleId)
        {
            var context = new TicketsEntities();
            var allocationModel = new TicketAllocationNumberModel();
            var numbers = context.TicketAllocationNumbers.AsEnumerable()
                .Where(t => t.TicketAllocation.RaffleId == raffleId && t.TicketAllocation.ClientId == clientId)
                .Select(t => allocationModel.ToObject(t)).ToList();
            return numbers;
        }

        internal object IdentifyAward(IdentifyBach identifyBach)
        {
            IdentifyBach newIdentifyBach;
            Winner newWinner;
            object IdentifyObject;
            Raffle raffle;
            Client client;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (identifyBach.Id <= 0)
                        {
                            if (identifyBach.WinnerId == 0 || identifyBach.WinnerId == null)
                            {
                                if (context.Winners.Any(a => a.DocumentNumber == identifyBach.Cedula))
                                {
                                    newWinner = context.Winners.FirstOrDefault(w => w.DocumentNumber == identifyBach.Cedula);
                                }
                                else
                                {
                                    newWinner = new Winner
                                    {
                                        DocumentNumber = identifyBach.Cedula,
                                        WinnerName = identifyBach.Nombre,
                                        Phone = identifyBach.Telefono
                                    };
                                }
                                context.Winners.Add(newWinner);
                                context.SaveChanges();
                            }
                            else
                            {
                                newWinner = context.Winners.FirstOrDefault(f => f.Id == identifyBach.WinnerId);
                            }

                            newIdentifyBach = new IdentifyBach
                            {
                                RaffleId = identifyBach.RaffleId,
                                ClientId = identifyBach.ClientId,
                                Type = identifyBach.Type,
                                Statu = (int)BachIdentifyStatuEnum.Inproces,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                Nombre = identifyBach.Nombre,
                                Cedula = identifyBach.Cedula,
                                Telefono = identifyBach.Telefono,
                                Notas = identifyBach.Notas,
                                IdentifyType = (int)IdentifyBachTypeEnum.Gamers,
                                WinnerId = newWinner.Id
                            };

                            context.IdentifyBaches.Add(newIdentifyBach);
                        }
                        else
                        {
                            newIdentifyBach = context.IdentifyBaches.FirstOrDefault(u => u.Id == identifyBach.Id);
                            newIdentifyBach.RaffleId = identifyBach.RaffleId;
                            newIdentifyBach.ClientId = identifyBach.ClientId;
                            newIdentifyBach.Type = identifyBach.Type;
                            newIdentifyBach.Nombre = identifyBach.Nombre;
                            newIdentifyBach.Cedula = identifyBach.Cedula;
                            newIdentifyBach.Telefono = identifyBach.Telefono;
                            newIdentifyBach.Notas = identifyBach.Notas;
                        }
                        context.SaveChanges();
                        newIdentifyBach.IdentifyNumbers = new List<IdentifyNumber>();

                        var identifyNumbers = new List<IdentifyNumber>();
                        foreach (var number in identifyBach.IdentifyNumbers)
                        {
                            if (identifyNumbers.Any(i => i.NumberId == number.NumberId && i.FractionFrom == number.FractionFrom && i.FractionTo == number.FractionTo) == false)
                            {
                                var nedit = context.IdentifyNumbers.FirstOrDefault(p => p.Id == number.Id);
                                if (nedit == null)
                                {
                                    var Currentnumber = context.TicketAllocationNumbers.FirstOrDefault(n => n.TicketAllocation.RaffleId == newIdentifyBach.RaffleId
                                    //&& n.TicketAllocation.ClientId == newIdentifyBach.ClientId
                                    && n.Number == number.NumberId);
                                    if (Currentnumber != null)
                                    {
                                        var identifyNumber = new IdentifyNumber()
                                        {
                                            IdentifyBachId = newIdentifyBach.Id,
                                            NumberId = Currentnumber.Id,
                                            FractionFrom = number.FractionFrom,
                                            FractionTo = number.FractionTo,
                                            Status = (int)AwardCertificationStatuEnum.Identified,
                                            IdentifyBachNumberType = (int)IdentifyBachNumberTypeEnum.Gamer
                                        };
                                        identifyNumbers.Add(identifyNumber);
                                    }
                                }
                            }
                        }
                        var distintNumbers = identifyNumbers.Distinct().ToList();
                        context.IdentifyNumbers.AddRange(distintNumbers);
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        raffle = context.Raffles.FirstOrDefault(r => r.Id == newIdentifyBach.RaffleId);
                        client = context.Clients.FirstOrDefault(c => c.Id == newIdentifyBach.ClientId);
                        IdentifyObject = IdentifyBachToObject(newIdentifyBach);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        return new
                        {
                            result = false,
                            message = ex.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, newIdentifyBach.Id > 0 ? LogActionsEnum.Update : LogActionsEnum.Insert, "Identificación de premios", IdentifyObject);

            return new
            {
                result = true,
                bachId = newIdentifyBach.Id,
                RaffleDesc = raffle.Name,
                RaffleId = raffle.Id,
                RaffleDate = raffle.DateSolteo.ToUnixTime(),
                ClientDesc = client.Name,
                message = "Lote de numeros guardado correctamente."
            };
        }

        internal object IdentifyAwardLight(AuxIdentifyBach identifyBach)
        {
            IdentifyBach newIdentifyBach;
            Winner newWinner;
            object IdentifyObject;
            Raffle raffle;
            Client client;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (identifyBach.WinnerId == 0 || identifyBach.WinnerId == null)
                        {
                            if (context.Winners.Any(a => a.DocumentNumber == identifyBach.DocumentNumber))
                            {
                                newWinner = context.Winners.FirstOrDefault(w => w.DocumentNumber == identifyBach.DocumentNumber);
                            }
                            else
                            {
                                newWinner = new Winner
                                {
                                    DocumentType = identifyBach.DocumentType,
                                    DocumentNumber = identifyBach.DocumentNumber.ToUpper(),
                                    WinnerName = identifyBach.WinnerName.ToUpper(),
                                    Phone = identifyBach.WinnerPhone,
                                    GenderId = identifyBach.GenderId,
                                    CreateUser = WebSecurity.CurrentUserId,
                                    CreateDate = DateTime.Now
                                };
                                context.Winners.Add(newWinner);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            newWinner = context.Winners.FirstOrDefault(f => f.Id == identifyBach.WinnerId);
                        }

                        if (newWinner == null)
                        {
                            newWinner = context.Winners.FirstOrDefault(f => f.Id == (int)DefaultEnum.DefaultWinner);
                        }

                        if (identifyBach.Id <= 0)
                        {
                            newIdentifyBach = new IdentifyBach
                            {
                                RaffleId = identifyBach.RaffleId,
                                ClientId = identifyBach.ClientId,
                                Type = identifyBach.Type,
                                Statu = (int)BachIdentifyStatuEnum.Inproces,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                /*Nombre = newWinner.WinnerName,
                                Cedula = newWinner.DocumentNumber,
                                Telefono = newWinner.Phone,*/
                                Notas = identifyBach.Notes,
                                IdentifyType = (int)IdentifyBachTypeEnum.Gamers,
                                WinnerId = newWinner.Id
                            };

                            context.IdentifyBaches.Add(newIdentifyBach);
                        }
                        else
                        {
                            newIdentifyBach = context.IdentifyBaches.FirstOrDefault(u => u.Id == identifyBach.Id);
                            newIdentifyBach.RaffleId = identifyBach.RaffleId;
                            newIdentifyBach.ClientId = identifyBach.ClientId;
                            newIdentifyBach.Type = identifyBach.Type;
                            /*newIdentifyBach.Nombre = identifyBach.WinnerName;
                            newIdentifyBach.Cedula = identifyBach.DocumentNumber;
                            newIdentifyBach.Telefono = identifyBach.WinnerPhone;*/
                            newIdentifyBach.Notas = identifyBach.Notes;
                            newIdentifyBach.WinnerId = newWinner.Id;
                        }
                        context.SaveChanges();
                        newIdentifyBach.IdentifyNumbers = new List<IdentifyNumber>();

                        var identifyNumbers = new List<IdentifyNumber>();
                        foreach (var number in identifyBach.IdentifyNumbers)
                        {
                            if (identifyNumbers.Any(i => i.NumberId == number.NumberId && i.FractionFrom == number.FractionFrom && i.FractionTo == number.FractionTo) == false)
                            {
                                var nedit = context.IdentifyNumbers.FirstOrDefault(p => p.Id == number.Id);
                                if (nedit == null)
                                {
                                    var Currentnumber = context.TicketAllocationNumbers.FirstOrDefault(n => n.TicketAllocation.RaffleId == newIdentifyBach.RaffleId
                                    //&& n.TicketAllocation.ClientId == newIdentifyBach.ClientId
                                    && n.Number == number.NumberId);
                                    if (Currentnumber != null)
                                    {
                                        var identifyNumber = new IdentifyNumber()
                                        {
                                            IdentifyBachId = newIdentifyBach.Id,
                                            NumberId = Currentnumber.Id,
                                            FractionFrom = number.FractionFrom,
                                            FractionTo = number.FractionTo,
                                            Status = (int)AwardCertificationStatuEnum.Identified,
                                            IdentifyBachNumberType = (int)IdentifyBachNumberTypeEnum.Gamer
                                        };
                                        identifyNumbers.Add(identifyNumber);
                                    }
                                }
                            }
                        }
                        var distintNumbers = identifyNumbers.Distinct().ToList();
                        context.IdentifyNumbers.AddRange(distintNumbers);
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        raffle = context.Raffles.FirstOrDefault(r => r.Id == newIdentifyBach.RaffleId);
                        client = context.Clients.FirstOrDefault(c => c.Id == newIdentifyBach.ClientId);
                        IdentifyObject = IdentifyBachToObject(newIdentifyBach);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        return new
                        {
                            result = false,
                            message = ex.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, newIdentifyBach.Id > 0 ? LogActionsEnum.Update : LogActionsEnum.Insert, "Identificación de premios", IdentifyObject);

            return new
            {
                result = true,
                bachId = newIdentifyBach.Id,
                RaffleDesc = raffle.Name,
                RaffleId = raffle.Id,
                RaffleDate = raffle.DateSolteo.ToUnixTime(),
                ClientDesc = client.Name,
                message = "Lote de numeros guardado correctamente."
            };
        }

        internal object IdentifySellerAward(IdentifyBach identifyBach)
        {
            IdentifyBach newIdentifyBach;
            Winner newWinner;
            object IdentifyObject;
            Raffle raffle;
            Client client;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (identifyBach.Id <= 0)
                        {
                            if (identifyBach.WinnerId == 0 || identifyBach.WinnerId == null)
                            {
                                if (context.Winners.Any(a => a.DocumentNumber == identifyBach.Cedula))
                                {
                                    newWinner = context.Winners.FirstOrDefault(w => w.DocumentNumber == identifyBach.Cedula);
                                }
                                else
                                {
                                    newWinner = new Winner
                                    {
                                        DocumentNumber = identifyBach.Cedula,
                                        WinnerName = identifyBach.Nombre,
                                        Phone = identifyBach.Telefono
                                    };
                                }
                                context.Winners.Add(newWinner);
                                context.SaveChanges();
                            }
                            else
                            {
                                newWinner = context.Winners.FirstOrDefault(f => f.Id == identifyBach.WinnerId);
                            }

                            newIdentifyBach = new IdentifyBach
                            {
                                RaffleId = identifyBach.RaffleId,
                                ClientId = identifyBach.ClientId,
                                Type = identifyBach.Type,
                                Statu = (int)BachIdentifyStatuEnum.Inproces,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                Nombre = identifyBach.Nombre,
                                Cedula = identifyBach.Cedula,
                                Telefono = identifyBach.Telefono,
                                Notas = identifyBach.Notas,
                                IdentifyType = (int)IdentifyBachTypeEnum.Sellers
                            };

                            context.IdentifyBaches.Add(newIdentifyBach);
                        }
                        else
                        {
                            newIdentifyBach = context.IdentifyBaches.FirstOrDefault(u => u.Id == identifyBach.Id);
                            newIdentifyBach.RaffleId = identifyBach.RaffleId;
                            newIdentifyBach.ClientId = identifyBach.ClientId;
                            newIdentifyBach.Type = identifyBach.Type;
                            newIdentifyBach.Nombre = identifyBach.Nombre;
                            newIdentifyBach.Cedula = identifyBach.Cedula;
                            newIdentifyBach.Telefono = identifyBach.Telefono;
                            newIdentifyBach.Notas = identifyBach.Notas;
                        }
                        context.SaveChanges();
                        newIdentifyBach.IdentifyNumbers = new List<IdentifyNumber>();

                        var identifyNumbers = new List<IdentifyNumber>();
                        foreach (var number in identifyBach.IdentifyNumbers)
                        {
                            if (identifyNumbers.Any(i => i.NumberId == number.NumberId && i.FractionFrom == number.FractionFrom && i.FractionTo == number.FractionTo) == false)
                            {
                                var nedit = context.IdentifyNumbers.FirstOrDefault(p => p.Id == number.Id);
                                if (nedit == null)
                                {
                                    var Currentnumber = context.TicketAllocationNumbers.FirstOrDefault(n => n.TicketAllocation.RaffleId == newIdentifyBach.RaffleId
                                    //&& n.TicketAllocation.ClientId == newIdentifyBach.ClientId
                                    && n.Number == number.NumberId);
                                    if (Currentnumber != null)
                                    {
                                        var identifyNumber = new IdentifyNumber()
                                        {
                                            IdentifyBachId = newIdentifyBach.Id,
                                            NumberId = Currentnumber.Id,
                                            FractionFrom = number.FractionFrom,
                                            FractionTo = number.FractionTo,
                                            Status = (int)AwardCertificationStatuEnum.Identified,
                                            IdentifyBachNumberType = (int)IdentifyBachNumberTypeEnum.Seller
                                        };
                                        identifyNumbers.Add(identifyNumber);
                                    }
                                }
                            }
                        }
                        var distintNumbers = identifyNumbers.Distinct().ToList();
                        context.IdentifyNumbers.AddRange(distintNumbers);
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        raffle = context.Raffles.FirstOrDefault(r => r.Id == newIdentifyBach.RaffleId);
                        client = context.Clients.FirstOrDefault(c => c.Id == newIdentifyBach.ClientId);
                        IdentifyObject = IdentifyBachSellerToObject(newIdentifyBach);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        return new
                        {
                            result = false,
                            message = ex.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, newIdentifyBach.Id > 0 ? LogActionsEnum.Update : LogActionsEnum.Insert, "Identificación de premios", IdentifyObject);

            return new
            {
                result = true,
                bachId = newIdentifyBach.Id,
                RaffleDesc = raffle.Name,
                RaffleId = raffle.Id,
                RaffleDate = raffle.DateSolteo.ToUnixTime(),
                ClientDesc = client.Name,
                message = "Lote de numeros guardado correctamente."
            };
        }

        internal object IdentifySellerAwardLight(AuxIdentifyBach identifyBach)
        {
            IdentifyBach newIdentifyBach;
            Winner newWinner;
            object IdentifyObject;
            Raffle raffle;
            Client client;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (identifyBach.WinnerId == 0 || identifyBach.WinnerId == null)
                        {
                            if (context.Winners.Any(a => a.DocumentNumber == identifyBach.DocumentNumber))
                            {
                                newWinner = context.Winners.FirstOrDefault(w => w.DocumentNumber == identifyBach.DocumentNumber);
                            }
                            else
                            {
                                newWinner = new Winner
                                {
                                    DocumentType = identifyBach.DocumentType,
                                    DocumentNumber = identifyBach.DocumentNumber.ToUpper(),
                                    WinnerName = identifyBach.WinnerName.ToUpper(),
                                    Phone = identifyBach.WinnerPhone,
                                    GenderId = identifyBach.GenderId,
                                    CreateUser = WebSecurity.CurrentUserId,
                                    CreateDate = DateTime.Now
                                };
                                context.Winners.Add(newWinner);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            newWinner = context.Winners.FirstOrDefault(f => f.Id == identifyBach.WinnerId);
                        }

                        if (newWinner == null)
                        {
                            newWinner = context.Winners.FirstOrDefault(f => f.Id == (int)DefaultEnum.DefaultWinner);
                        }

                        if (identifyBach.Id <= 0)
                        {
                            newIdentifyBach = new IdentifyBach
                            {
                                RaffleId = identifyBach.RaffleId,
                                ClientId = identifyBach.ClientId,
                                Type = identifyBach.Type,
                                Statu = (int)BachIdentifyStatuEnum.Inproces,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                /*Nombre = newWinner.WinnerName,
                                Cedula = newWinner.DocumentNumber,
                                Telefono = newWinner.Phone,*/
                                Notas = identifyBach.Notes,
                                IdentifyType = (int)IdentifyBachTypeEnum.Gamers,
                                WinnerId = newWinner.Id
                            };

                            context.IdentifyBaches.Add(newIdentifyBach);
                        }
                        else
                        {
                            newIdentifyBach = context.IdentifyBaches.FirstOrDefault(u => u.Id == identifyBach.Id);
                            newIdentifyBach.RaffleId = identifyBach.RaffleId;
                            newIdentifyBach.ClientId = identifyBach.ClientId;
                            newIdentifyBach.Type = identifyBach.Type;
                            /*newIdentifyBach.Nombre = identifyBach.WinnerName;
                            newIdentifyBach.Cedula = identifyBach.DocumentNumber;
                            newIdentifyBach.Telefono = identifyBach.WinnerPhone;*/
                            newIdentifyBach.Notas = identifyBach.Notes;
                            newIdentifyBach.WinnerId = newWinner.Id;
                        }
                        context.SaveChanges();
                        newIdentifyBach.IdentifyNumbers = new List<IdentifyNumber>();

                        var identifyNumbers = new List<IdentifyNumber>();
                        foreach (var number in identifyBach.IdentifyNumbers)
                        {
                            if (identifyNumbers.Any(i => i.NumberId == number.NumberId && i.FractionFrom == number.FractionFrom && i.FractionTo == number.FractionTo) == false)
                            {
                                var nedit = context.IdentifyNumbers.FirstOrDefault(p => p.Id == number.Id);
                                if (nedit == null)
                                {
                                    var Currentnumber = context.TicketAllocationNumbers.FirstOrDefault(n => n.TicketAllocation.RaffleId == newIdentifyBach.RaffleId
                                    //&& n.TicketAllocation.ClientId == newIdentifyBach.ClientId
                                    && n.Number == number.NumberId);
                                    if (Currentnumber != null)
                                    {
                                        var identifyNumber = new IdentifyNumber()
                                        {
                                            IdentifyBachId = newIdentifyBach.Id,
                                            NumberId = Currentnumber.Id,
                                            FractionFrom = number.FractionFrom,
                                            FractionTo = number.FractionTo,
                                            Status = (int)AwardCertificationStatuEnum.Identified,
                                            IdentifyBachNumberType = (int)IdentifyBachNumberTypeEnum.Seller
                                        };
                                        identifyNumbers.Add(identifyNumber);
                                    }
                                }
                            }
                        }
                        var distintNumbers = identifyNumbers.Distinct().ToList();
                        context.IdentifyNumbers.AddRange(distintNumbers);
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        raffle = context.Raffles.FirstOrDefault(r => r.Id == newIdentifyBach.RaffleId);
                        client = context.Clients.FirstOrDefault(c => c.Id == newIdentifyBach.ClientId);
                        IdentifyObject = IdentifyBachSellerToObject(newIdentifyBach);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();

                        return new
                        {
                            result = false,
                            message = ex.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, newIdentifyBach.Id > 0 ? LogActionsEnum.Update : LogActionsEnum.Insert, "Identificación de premios", IdentifyObject);

            return new
            {
                result = true,
                bachId = newIdentifyBach.Id,
                RaffleDesc = raffle.Name,
                RaffleId = raffle.Id,
                RaffleDate = raffle.DateSolteo.ToUnixTime(),
                ClientDesc = client.Name,
                message = "Lote de numeros guardado correctamente."
            };
        }

        internal object ValidateNumberAward(AwardTicketModel awardTicket, int clientId)
        {
            var context = new TicketsEntities();
            List<string> messageList = new List<string>();
            var message = "";
            var raffleAwards = context.RaffleAwards.Where(r => r.RaffleId == awardTicket.RaffleId).ToList();
            var clientGroup = 0;
            if (clientId != 0)
            {
                clientGroup = context.Clients.Where(c => c.Id == clientId).FirstOrDefault().GroupId;
            }

            var allocationNumbers = context.TicketAllocationNumbers.Where(t => t.Number == awardTicket.NumberId &&
                                                                          t.TicketAllocation.RaffleId == awardTicket.RaffleId &&
                                                                          t.TicketType == (int)TicketsTypeEnum.AvailableTicket).ToList();

            if (allocationNumbers.Any() == false)
            {
                message = "El billete " + awardTicket.NumberId + " no se ha asignado.";
            }
            else
            {
                var SellerAward = raffleAwards.Where(r => r.RaffleId == awardTicket.RaffleId && r.ControlNumber == awardTicket.NumberId).ToList();

                if (SellerAward.Any())
                {
                    if (SellerAward.Any(a => a.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived))
                    {
                        string noAllocateFractions = "";
                        for (var fraction = awardTicket.FractionFrom; fraction <= awardTicket.FractionTo; fraction++)
                        {
                            if (allocationNumbers.Where(a => fraction >= a.FractionFrom && fraction <= a.FractionTo).Any() == false)
                            {
                                noAllocateFractions += fraction + (fraction != awardTicket.FractionTo ? ", " : "");
                            }
                        }
                        if (noAllocateFractions != "")
                        {
                            messageList.Add("Las fracciones ( " + noAllocateFractions + " ) del billete " + awardTicket.NumberId + " no fueron asignada.");

                            return new
                            {
                                result = false,
                                messages = messageList
                            };
                        }

                        for (int i = 0; i < allocationNumbers.Count(); i++)
                        {
                            var allocationNumber = allocationNumbers[i];

                            if (allocationNumber.Statu == (int)TicketStatusEnum.Alloated)
                            {
                                if (awardTicket.FractionFrom >= allocationNumber.FractionFrom && awardTicket.FractionTo <= allocationNumber.FractionTo)
                                {
                                    messageList.Add("La fracción " + allocationNumber.FractionFrom + " hasta " + allocationNumber.FractionTo + " del billete " + allocationNumber.Number + " fue asignada pero no impresa.");
                                }
                                continue;
                            }
                            else if (allocationNumber.Statu == (int)TicketStatusEnum.Printed)
                            {
                                if (awardTicket.FractionFrom >= allocationNumber.FractionFrom && awardTicket.FractionTo <= allocationNumber.FractionTo)
                                {
                                    messageList.Add("La fracción " + allocationNumber.FractionFrom + " hasta " + allocationNumber.FractionTo + " del billete " + allocationNumber.Number + " fue impresa pero no facturada.");
                                }
                                continue;
                            }
                            var returneds = context.TicketReturns.Where(r => r.RaffleId == allocationNumber.TicketAllocation.RaffleId && r.TicketAllocationNumber.Number == allocationNumber.Number).ToList();
                            string returnedError = "";
                            for (var fraction = awardTicket.FractionFrom; fraction <= awardTicket.FractionTo; fraction++)
                            {
                                if (returneds.Where(r => fraction >= r.FractionFrom && fraction <= r.FractionTo).Any())
                                {
                                    returnedError += fraction + (fraction == awardTicket.FractionTo ? "" : ", ");
                                }
                            }
                            if (returnedError != "")
                            {
                                messageList.Add("La fracciones ( " + returnedError + " ) del billete " + allocationNumber.Number + " fue devuelta.");
                                continue;
                            }

                            var identifyNumbers = context.IdentifyNumbers.Where(r =>
                                r.IdentifyBach.RaffleId == allocationNumber.TicketAllocation.RaffleId &&
                                r.TicketAllocationNumber.Number == allocationNumber.Number &&
                                r.IdentifyBachNumberType == (int)IdentifyBachNumberTypeEnum.Gamer
                                /*&& r.IdentifyBach.Type == awardTicket.Type*/).ToList();

                            string identifyError = "";
                            for (var fraction = awardTicket.FractionFrom; fraction <= awardTicket.FractionTo; fraction++)
                            {
                                if (identifyNumbers.Where(r => fraction >= r.FractionFrom && fraction <= r.FractionTo).Any())
                                {
                                    identifyError += fraction + (fraction == awardTicket.FractionTo ? "" : ", ");
                                }
                            }
                            if (identifyError != "")
                            {
                                var isPayed = Utils.IdentifyBachIsPayedMinor(identifyNumbers.FirstOrDefault().IdentifyBach, raffleAwards) || Utils.IdentifyBachIsPayedMayor(identifyNumbers.FirstOrDefault().IdentifyBach, raffleAwards);
                                if (isPayed == true)
                                {
                                    messageList.Add("La fracciones ( " + identifyError + " ) del billete " + allocationNumber.Number + " fue pagada.");
                                }
                                else
                                {
                                    messageList.Add("La fracciones ( " + identifyError + " ) del billete " + allocationNumber.Number + " fue identificada.");
                                }
                                continue;
                            }
                        }

                        if (message == "")
                        {
                            if (raffleAwards.Where(r =>
                                r.ControlNumber == awardTicket.NumberId
                                && r.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived
                                ).Any() == false)
                            {
                                messageList.Add("Las fracciones " + awardTicket.FractionFrom + " hasta " + awardTicket.FractionTo + " del billete " + awardTicket.NumberId + " no tienen premios menores.");
                            }
                            else
                            {
                                if (clientId != 0)
                                {
                                    if (raffleAwards.Where(r =>
                                    r.ControlNumber == awardTicket.NumberId
                                    && (r.Award.TypesAwardId == 1 || r.Award.TypesAwardId == 6)
                                    && r.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived
                                    && clientGroup == 36
                                    ).Any() == true)
                                    {
                                        messageList.Add("Los clientes mayoristas no pueden pagar premios mayores." + awardTicket.NumberId);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "El billete " + awardTicket.NumberId + " pertenece a los premios al vendedor.";
                    }
                }
                else
                {
                    message = "El billete " + awardTicket.NumberId + " no contiene premios.";
                }
            }

            if (message != "")
            {
                messageList.Add(message);
            }

            return new
            {
                result = messageList.Count == 0,
                messages = messageList
            };
        }

        internal object ValidateSellerNumberAward(AwardTicketModel awardTicket, int clientId)
        {
            var context = new TicketsEntities();
            List<string> messageList = new List<string>();
            var message = "";
            var raffleAwards = context.RaffleAwards.Where(r => r.RaffleId == awardTicket.RaffleId).ToList();
            var clientGroup = 0;
            if (clientId != 0)
            {
                clientGroup = context.Clients.Where(c => c.Id == clientId).FirstOrDefault().GroupId;
            }
            var allocationNumbers = context.TicketAllocationNumbers.Where(t => t.Number == awardTicket.NumberId &&
                                                                          t.TicketAllocation.RaffleId == awardTicket.RaffleId &&
                                                                          t.TicketType == (int)TicketsTypeEnum.AvailableTicket).ToList();

            if (allocationNumbers.Any() == false)
            {
                message = "El billete " + awardTicket.NumberId + " no se ha asignado.";
            }
            else
            {
                var SellerAward = raffleAwards.Where(r => r.RaffleId == awardTicket.RaffleId && r.ControlNumber == awardTicket.NumberId).ToList();

                if (SellerAward.Any())
                {
                    if (SellerAward.Any(a => a.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived))
                    {
                        string noAllocateFractions = "";
                        for (var fraction = awardTicket.FractionFrom; fraction <= awardTicket.FractionTo; fraction++)
                        {
                            if (allocationNumbers.Where(a => fraction >= a.FractionFrom && fraction <= a.FractionTo).Any() == false)
                            {
                                noAllocateFractions += fraction + (fraction != awardTicket.FractionTo ? ", " : "");
                            }
                        }
                        if (noAllocateFractions != "")
                        {
                            messageList.Add("Las fracciones ( " + noAllocateFractions + " ) del billete " + awardTicket.NumberId + " no fueron asignada.");

                            return new
                            {
                                result = false,
                                messages = messageList
                            };
                        }

                        for (int i = 0; i < allocationNumbers.Count(); i++)
                        {
                            var allocationNumber = allocationNumbers[i];

                            if (allocationNumber.Statu == (int)TicketStatusEnum.Alloated)
                            {
                                if (awardTicket.FractionFrom >= allocationNumber.FractionFrom && awardTicket.FractionTo <= allocationNumber.FractionTo)
                                {
                                    messageList.Add("La fracción " + allocationNumber.FractionFrom + " hasta " + allocationNumber.FractionTo + " del billete " + allocationNumber.Number + " fue asignada pero no impresa.");
                                }
                                continue;
                            }
                            else if (allocationNumber.Statu == (int)TicketStatusEnum.Printed)
                            {
                                if (awardTicket.FractionFrom >= allocationNumber.FractionFrom && awardTicket.FractionTo <= allocationNumber.FractionTo)
                                {
                                    messageList.Add("La fracción " + allocationNumber.FractionFrom + " hasta " + allocationNumber.FractionTo + " del billete " + allocationNumber.Number + " fue impresa pero no facturada.");
                                }
                                continue;
                            }
                            var returneds = context.TicketReturns.Where(r => r.RaffleId == allocationNumber.TicketAllocation.RaffleId && r.TicketAllocationNumber.Number == allocationNumber.Number).ToList();
                            string returnedError = "";
                            for (var fraction = awardTicket.FractionFrom; fraction <= awardTicket.FractionTo; fraction++)
                            {
                                if (returneds.Where(r => fraction >= r.FractionFrom && fraction <= r.FractionTo).Any())
                                {
                                    returnedError += fraction + (fraction == awardTicket.FractionTo ? "" : ", ");
                                }
                            }
                            if (returnedError != "")
                            {
                                messageList.Add("La fracciones ( " + returnedError + " ) del billete " + allocationNumber.Number + " fue devuelta.");
                                continue;
                            }

                            var identifyNumbers = context.IdentifyNumbers.Where(r =>
                                r.IdentifyBach.RaffleId == allocationNumber.TicketAllocation.RaffleId &&
                                r.TicketAllocationNumber.Number == allocationNumber.Number &&
                                r.IdentifyBachNumberType == (int)IdentifyBachNumberTypeEnum.Seller
                                /*&& r.IdentifyBach.Type == awardTicket.Type*/).ToList();

                            string identifyError = "";
                            for (var fraction = awardTicket.FractionFrom; fraction <= awardTicket.FractionTo; fraction++)
                            {
                                if (identifyNumbers.Where(r => awardTicket.FractionFrom >= r.FractionFrom && awardTicket.FractionTo <= r.FractionTo).Any())
                                {
                                    identifyError += fraction + (fraction == awardTicket.FractionTo ? "" : ", ");
                                }
                            }
                            if (identifyError != "")
                            {
                                var isPayed = Utils.IdentifyBachSellerIsPayedMinor(identifyNumbers.FirstOrDefault().IdentifyBach, raffleAwards) || Utils.IdentifyBachSellerIsPayedMayor(identifyNumbers.FirstOrDefault().IdentifyBach, raffleAwards);
                                if (isPayed == true)
                                {
                                    messageList.Add("La fracciones ( " + identifyError + " ) del billete " + allocationNumber.Number + " fue pagada.");
                                }
                                else
                                {
                                    messageList.Add("La fracciones ( " + identifyError + " ) del billete " + allocationNumber.Number + " fue identificada.");
                                }
                                continue;
                            }
                        }

                        if (message == "")
                        {
                            if (raffleAwards.Where(r =>
                                r.ControlNumber == awardTicket.NumberId
                                && r.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived
                                ).Any() == false)
                            {
                                messageList.Add("Las fracciones " + awardTicket.FractionFrom + " hasta " + awardTicket.FractionTo + " del billete " + awardTicket.NumberId + " no tienen premios menores.");
                            }
                            else
                            {
                                if (clientId != 0)
                                {
                                    if (raffleAwards.Where(r =>
                                    r.ControlNumber == awardTicket.NumberId
                                    && (r.Award.TypesAwardId == 1 || r.Award.TypesAwardId == 6)
                                    && r.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived
                                    && clientGroup == 36
                                    ).Any() == true)
                                    {
                                        messageList.Add("Los clientes mayoristas no pueden pagar premios mayores." + awardTicket.NumberId);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "El billete " + awardTicket.NumberId + " no pertenece a los premios al vendedor.";
                    }
                }
                else
                {
                    message = "El billete " + awardTicket.NumberId + " no contiene premios.";
                }
            }

            if (message != "")
            {
                messageList.Add(message);
            }

            return new
            {
                result = messageList.Count == 0,
                messages = messageList
            };
        }

        internal object IdentifyNumberDelete(IdentifyNumber model)
        {
            var context = new TicketsEntities();
            var identifyNumber = context.IdentifyNumbers.FirstOrDefault(m => m.Id == model.Id);
            if (identifyNumber == null)
            {
                return new { result = false, message = "No se encontro la identificación de numero" };
            }
            var identifyObject = IdentifyNumberToObejct(identifyNumber);
            context.IdentifyNumbers.Remove(identifyNumber);
            context.SaveChanges();
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Identificación de numero", identifyObject);
            return new { result = true, message = "Identificación de numero borrada correcamente" };
        }

        internal object IdentifyBachDelete(IdentifyBach model)
        {
            object identifyBachObject;
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var identifyBach = context.IdentifyBaches.FirstOrDefault(m => m.Id == model.Id);
                        var raffleAwards = context.RaffleAwards.Where(r => r.RaffleId == identifyBach.RaffleId).ToList();
                        identifyBachObject = IdentifyBachMainToObject(identifyBach, raffleAwards);
                        if (identifyBach == null)
                        {
                            return new { result = false, message = "No se encontro el lote de identificación de numero" };
                        }
                        context.IdentifyNumbers.RemoveRange(identifyBach.IdentifyNumbers);
                        context.SaveChanges();

                        context.IdentifyBaches.Remove(identifyBach);
                        context.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new { result = false, message = e.Message };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Lote de Identificacion de Numero", identifyBachObject);

            return new { result = true, message = "Lote de numero borrado correctamente" };
        }

        #region Public Method
        public object ListaIdentificacionPremios(IdentifyBach identifyBach)
        {
            return new
            {
                identifyBach.Id,
                identifyBach.ClientId,
                ClientDesc = identifyBach.Client.Name,
                identifyBach.RaffleId,
                identifyBach.Statu,
                identifyBach.Winner.WinnerName,
                hasPayment = (identifyBach.IdentifyBachPayments.Count > 0 || identifyBach.NoteCredits.Count > 0),
                RaffleDesc = identifyBach.Raffle.Name
            };
        }

        public object ListaIdentificacionPremiosVendedor(IdentifyBach identifyBach)
        {
            return new
            {
                identifyBach.Id,
                identifyBach.ClientId,
                ClientDesc = identifyBach.Client.Name,
                identifyBach.RaffleId,
                identifyBach.Statu,
                identifyBach.Winner.WinnerName,
                hasPayment = (identifyBach.IdentifyBachPayments.Count > 0 || identifyBach.NoteCredits.Count > 0),
                RaffleDesc = identifyBach.Raffle.Name
            };
        }

        public object IdentifyBachMainToObject(IdentifyBach identifyBach, List<RaffleAward> awards)
        {
            return new
            {
                identifyBach.Id,
                identifyBach.ClientId,
                ClientDesc = identifyBach.Client.Name,
                identifyBach.Statu,
                identifyBach.RaffleId,
                identifyBach.Nombre,
                RaffleDesc = identifyBach.Raffle.Name,
                hasPayment = (identifyBach.IdentifyBachPayments.Count > 0 || identifyBach.NoteCredits.Count > 0),
                isPayed = Utils.IdentifyBachIsPayedMinor(identifyBach, awards)
            };
        }

        public object IdentifyBachToObject(IdentifyBach identifyBach)
        {
            var context = new TicketsEntities();
            var creditNote = context.NoteCredits.Where(c => c.IdentifyBaches.Where(s => s.Id == identifyBach.Id).Any()).ToList();
            var awards = context.RaffleAwards.Where(a => a.RaffleId == identifyBach.RaffleId).ToList();
            return new
            {
                identifyBach.Id,
                identifyBach.ClientId,
                percent = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista || identifyBach.Client.GroupId == (int)ClientGroupEnum.DistribuidorElectronico ? 2 : 0,
                ClientDesc = context.Clients.FirstOrDefault(c => c.Id == identifyBach.ClientId).Name,
                identifyBach.Winner.DocumentNumber,
                identifyBach.Winner.WinnerName,
                identifyBach.Winner.Phone,
                identifyBach.Notas,
                identifyBach.WinnerId,
                identifyBach.RaffleId,
                raffleSequence = context.Raffles.FirstOrDefault(c => c.Id == identifyBach.RaffleId).RaffleSequence,
                RaffleDesc = context.Raffles.FirstOrDefault(c => c.Id == identifyBach.RaffleId).Name,
                RaffleDate = context.Raffles.FirstOrDefault(c => c.Id == identifyBach.RaffleId).DateSolteo.ToUnixTime(),
                CreateDate = identifyBach.CreateDate.ToUnixTime(),
                identifyBach.CreateUser,
                identifyBach.Statu,
                hasPayment = (identifyBach.IdentifyBachPayments.Count > 0 || identifyBach.NoteCredits.Count > 0),
                isPayed = identifyBach.Type == (int)IdentifyBachTypeEnum.Menor ? Utils.IdentifyBachIsPayedMinor(identifyBach, awards) : Utils.IdentifyBachIsPayedMayor(identifyBach, awards),
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == identifyBach.Statu).NameDetail,
                productionLength = identifyBach.Raffle.Prospect.Production,
                NoteCredits = creditNote.Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    ClientDesc = c.ClientId + " - " + c.Client.Name,
                    c.TotalCash,
                    Note = c.Concepts
                }),
                IdentifyBachPayments = context.IdentifyBachPayments.Where(ib => ib.IdentifyBachId == identifyBach.Id).Select(p => new
                {
                    p.Id,
                    p.ClientId,
                    ClientDesc = p.ClientId + " - " + p.Client.Name,
                    p.Value,
                    p.Note
                }),
                IdentifyNumbers = context.IdentifyNumbers.AsEnumerable().Where(n => n.IdentifyBachId == identifyBach.Id)
                .Select(n => IdentifyNumberToObejct(n)).ToList()
            };
        }

        public object IdentifyBachSellerToObject(IdentifyBach identifyBach)
        {
            var context = new TicketsEntities();
            var creditNote = context.NoteCredits.Where(c => c.IdentifyBaches.Where(s => s.Id == identifyBach.Id).Any()).ToList();
            var awards = context.RaffleAwards.Where(a => a.RaffleId == identifyBach.RaffleId).ToList();
            return new
            {
                identifyBach.Id,
                identifyBach.ClientId,
                percent = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista || identifyBach.Client.GroupId == (int)ClientGroupEnum.DistribuidorElectronico ? 2 : 0,
                ClientDesc = context.Clients.FirstOrDefault(c => c.Id == identifyBach.ClientId).Name,
                identifyBach.Winner.DocumentNumber,
                identifyBach.Winner.WinnerName,
                identifyBach.Winner.Phone,
                identifyBach.Notas,
                identifyBach.WinnerId,
                identifyBach.RaffleId,
                raffleSequence = context.Raffles.FirstOrDefault(c => c.Id == identifyBach.RaffleId).RaffleSequence,
                RaffleDesc = context.Raffles.FirstOrDefault(c => c.Id == identifyBach.RaffleId).Name,
                RaffleDate = context.Raffles.FirstOrDefault(c => c.Id == identifyBach.RaffleId).DateSolteo.ToUnixTime(),
                CreateDate = identifyBach.CreateDate.ToUnixTime(),
                identifyBach.CreateUser,
                identifyBach.Statu,
                hasPayment = (identifyBach.IdentifyBachPayments.Count > 0 || identifyBach.NoteCredits.Count > 0),
                isPayed = identifyBach.Type == (int)IdentifyBachTypeEnum.Menor ? Utils.IdentifyBachSellerIsPayedMinor(identifyBach, awards) : Utils.IdentifyBachSellerIsPayedMayor(identifyBach, awards),
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == identifyBach.Statu).NameDetail,
                productionLength = identifyBach.Raffle.Prospect.Production,
                NoteCredits = creditNote.Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    ClientDesc = c.ClientId + " - " + c.Client.Name,
                    c.TotalCash,
                    Note = c.Concepts
                }),
                IdentifyBachPayments = context.IdentifyBachPayments.Where(ib => ib.IdentifyBachId == identifyBach.Id).Select(p => new
                {
                    p.Id,
                    p.ClientId,
                    ClientDesc = p.ClientId + " - " + p.Client.Name,
                    p.Value,
                    p.Note
                }),
                IdentifyNumbers = context.IdentifyNumbers.AsEnumerable().Where(n => n.IdentifyBachId == identifyBach.Id)
                .Select(n => IdentifyNumberSellerToObejct(n)).ToList()
            };
        }

        private object IdentifyNumberToObejct(IdentifyNumber n)
        {
            var context = new TicketsEntities();
            var identifyObject = new
            {
                n.Id,
                n.NumberId,
                NumberDesc = context.TicketAllocationNumbers.FirstOrDefault(tn => tn.Id == n.NumberId).Number,
                n.IdentifyBachId,
                n.FractionFrom,
                n.FractionTo,
                n.IdentifyBachNumberType,
                Fractions = (n.FractionTo - n.FractionFrom) + 1,
                n.Status,
                AwardName = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo)
                || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => s.Award.Name).FirstOrDefault(),

                Value = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId && ra.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo)
                || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)).FirstOrDefault(),

                Total = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId && ra.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo) || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => (s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)) * ((n.FractionTo - n.FractionFrom) + 1)).FirstOrDefault(),

                RaffleAwards = context.RaffleAwards.Where(ra =>
                    ra.RaffleId == n.IdentifyBach.RaffleId
                    && ra.ControlNumber == n.TicketAllocationNumber.Number
                    && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo) || ra.Award.ByFraction == (int)ByFractionEnum.N)
                    && ra.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived).Select(
                ra => new
                {
                    AwardName = ra.Award.Name,
                    AwardValue = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value : ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber),
                    FractionFrom = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionFrom,
                    FractionTo = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionTo,
                    ra.Id,
                    LawDiscount = ra.Award.ByFraction == (int)ByFractionEnum.S || ra.Award.TypesAwardId == (int)AwardTypeEnum.Mayors ? context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor : 0,
                    Total = ra.Award.ByFraction == (int)ByFractionEnum.S || ra.Award.TypesAwardId == (int)AwardTypeEnum.Mayors ? (ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value * (context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor / 100) : (ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber) * (n.FractionTo - n.FractionFrom + 1)) * (context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor / 100)) : (ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber)),
                }).ToList()
            };
            return identifyObject;
        }

        private object IdentifyNumberSellerToObejct(IdentifyNumber n)
        {
            var context = new TicketsEntities();
            var identifyObject = new
            {
                n.Id,
                n.NumberId,
                NumberDesc = context.TicketAllocationNumbers.FirstOrDefault(tn => tn.Id == n.NumberId).Number,
                n.IdentifyBachId,
                n.FractionFrom,
                n.FractionTo,
                n.IdentifyBachNumberType,
                Fractions = (n.FractionTo - n.FractionFrom) + 1,
                n.Status,

                AwardName = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo)
                || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => s.Award.Name).FirstOrDefault(),

                Value = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId && ra.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo)
                || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)).FirstOrDefault(),

                Total = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId && ra.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo) || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => (s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)) * ((n.FractionTo - n.FractionFrom) + 1)).FirstOrDefault(),

                RaffleAwards = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo) || ra.Award.ByFraction == (int)ByFractionEnum.N)
                && ra.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived).Select(
                ra => new
                {
                    AwardName = ra.Award.Name,
                    AwardValue = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value : ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber),
                    FractionFrom = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionFrom,
                    FractionTo = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionTo,
                    ra.Id,
                    LawDiscount = ra.Award.ByFraction == (int)ByFractionEnum.S || ra.Award.TypesAwardId == (int)AwardTypeEnum.Mayors ? context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor : 0,
                    Total = ra.Award.ByFraction == (int)ByFractionEnum.S || ra.Award.TypesAwardId == (int)AwardTypeEnum.Mayors ? (ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value * (context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor / 100) : (ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber) * (n.FractionTo - n.FractionFrom + 1)) * (context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor / 100)) : (ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber)),
                }).ToList()
            };

            return identifyObject;
        }
        #endregion
    }
}
