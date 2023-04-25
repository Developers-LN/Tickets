using System.Linq;
using Tickets.Models.Enums;

namespace Tickets.Models
{
    public class SystemConfigModel
    {
        #region System Config

        internal object GetSystemConfigData()
        {
            var context = new TicketsEntities();
            var config = context.SystemConfigs.FirstOrDefault();
            if (config == null)
            {
                config = new SystemConfig()
                {
                    Id = 0,
                    LoteryAdmin = "",
                    MaxReturnTickets = 0,
                    TicketDesign = 0,
                    Cargo = ""
                };
            }
            var ticketDesings = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.TicketsDesing).Select(c => new
            {
                id = c.Id,
                name = c.NameDetail
            });

            var xpiredTimes = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.RaffleXpiredTime).Select(c => new
            {
                id = c.Id,
                name = c.NameDetail
            });

            return new
            {
                config,
                ticketDesings,
                xpiredTimes,
            };
        }

        internal object SystemConfig(SystemConfig systemConfig)
        {
            using (var context = new TicketsEntities())
            {
                using (var tm = context.Database.BeginTransaction())
                {
                    if (systemConfig.Id == 0)
                    {
                        context.SystemConfigs.Add(systemConfig);
                    }
                    else
                    {
                        var config = context.SystemConfigs.FirstOrDefault(s => s.Id == systemConfig.Id);
                        config.MaxReturnTickets = systemConfig.MaxReturnTickets;
                        config.LoteryAdmin = systemConfig.LoteryAdmin;
                        config.TicketDesign = systemConfig.TicketDesign;
                        config.RaffleXpiredTime = systemConfig.RaffleXpiredTime;
                        config.LawDiscountPercentMayor = systemConfig.LawDiscountPercentMayor;
                        config.Cargo = systemConfig.Cargo;
                        config.Comercial = systemConfig.Comercial;
                        config.ComercialCargo = systemConfig.ComercialCargo;
                        config.ControlPremio = systemConfig.ControlPremio;
                        config.ControlPremioCargo = systemConfig.ControlPremioCargo;
                        config.CajaGeneral = systemConfig.CajaGeneral;
                        config.CajaGeneralCargo = systemConfig.CajaGeneralCargo;
                        config.Inspectoria = systemConfig.Inspectoria;
                        config.InspectoriaCargo = systemConfig.InspectoriaCargo;
                        config.CreditosCobros = systemConfig.CreditosCobros;
                        config.CreditosCobrosCargo = systemConfig.CreditosCobrosCargo;
                        config.ProduccionSorteo = systemConfig.ProduccionSorteo;
                        config.ProduccionSorteoCargo = systemConfig.ProduccionSorteoCargo;
                        config.Facturacion = systemConfig.Facturacion;
                        config.FacturacionCargo = systemConfig.FacturacionCargo;
                    }
                    context.SaveChanges();
                    tm.Commit();
                }
            }
            return new
            {
                result = true,
                message = "configuración guardada correctamente."
            };
        }
        #endregion
    }
}