using Newtonsoft.Json;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models
{
    public class CatalogModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "idGroup")]
        public int IdGroup { get; set; }

        [JsonProperty(PropertyName = "nameGroup")]
        public string NameGroup { get; set; }

        [JsonProperty(PropertyName = "idDetail")]
        public int IdDetail { get; set; }

        [JsonProperty(PropertyName = "nameDetail")]
        public string NameDetail { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "description2")]
        public string Description2 { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public bool Statu { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        internal object GetCatalogList()
        {
            var context = new TicketsEntities();
            var catalogs = context.Catalogs.AsEnumerable().Where(c => c.Statu == true).Select(u => this.GetCatalogObject(u)).ToList();
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Catalogo");

            return catalogs;
        }

        internal object CatalogCreate(Catalog catalog)
        {
            var context = new TicketsEntities();
            if (catalog.Id <= 0)
            {
                var item = context.Catalogs.Where(c => c.NameGroup == catalog.NameGroup);
                if (item.Any())
                {
                    catalog.IdGroup = item.FirstOrDefault().IdGroup;
                    catalog.IdDetail = item.OrderByDescending(i => i.IdDetail).FirstOrDefault().IdDetail + 1;
                }
                else
                {
                    catalog.IdDetail = 1;
                    var groupList = context.Catalogs.Select(c => c.IdGroup);
                    if (groupList.Any())
                    {
                        catalog.IdGroup = groupList.OrderByDescending(g => g).FirstOrDefault() + 1;
                    }
                    else
                    {
                        catalog.IdGroup = 1;
                    }
                }
                context.Catalogs.Add(catalog);
            }
            else
            {
                var modifyCatalog = context.Catalogs.FirstOrDefault(u => u.Id == catalog.Id);
                modifyCatalog.NameDetail = catalog.NameDetail;
                modifyCatalog.Statu = catalog.Statu;
                modifyCatalog.Description = catalog.Description;
                modifyCatalog.Description2 = catalog.Description2;
            }
            context.SaveChanges();


            Utils.SaveLog(WebSecurity.CurrentUserName, catalog.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Catalogo", this.GetCatalogObject(catalog));
            return true;
        }

        internal object CatalogDelete(int catalogId)
        {
            var context = new TicketsEntities();
            var catalog = context.Catalogs.FirstOrDefault(m => m.Id == catalogId);
            if (catalog != null)
            {
                catalog.Statu = false;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Catalogo", this.GetCatalogObject(catalog));
            }
            return true;
        }

        internal object GetCatalogObject(Catalog catalog)
        {
            var obj = new
            {
                catalog.Id,
                catalog.IdGroup,
                catalog.NameGroup,
                catalog.IdDetail,
                catalog.NameDetail,
                catalog.Description,
                catalog.Description2,
                catalog.Statu,
                StatuDesc = catalog.Statu ? "Activo" : "In activo"
            };
            return obj;
        }

        private CatalogModel CatalogToObject(Catalog catalog)
        {
            CatalogModel catalogModel = new CatalogModel()
            {
                Id = catalog.Id,
                IdGroup = catalog.IdGroup,
                NameGroup = catalog.NameGroup,
                IdDetail = catalog.IdDetail,
                NameDetail = catalog.NameDetail,
                Description = catalog.Description,
                Description2 = catalog.Description2,
                Statu = catalog.Statu,
                StatuDesc = catalog.Statu ? "Activo" : "In activo"
            };
            return catalogModel;
        }

        internal RequestResponseModel GetByGroupIdSelect(int groupId)
        {
            var context = new TicketsEntities();
            var prospectGroups = context.Catalogs.Where(c => c.Statu == true && c.IdGroup == groupId).AsEnumerable()
                .Select(catalog => this.CatalogToObject(catalog)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = prospectGroups
            };
        }

        internal RequestResponseModel GetReprintDesing()
        {
            var context = new TicketsEntities();
            var config = context.SystemConfigs.FirstOrDefault();
            if (config == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "No se encontro ninguna configuración"
                };
            }

            string ticketDesing = context.Catalogs.FirstOrDefault(c => c.Id == config.TicketDesign).Description2;
            return new RequestResponseModel()
            {
                Result = true,
                Object = ticketDesing
            };
        }

        internal RequestResponseModel GetPrintDesing()
        {
            var context = new TicketsEntities();
            var config = context.SystemConfigs.FirstOrDefault();
            if (config == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "No se encontro ninguna configuración"
                };
            }

            string ticketDesing = context.Catalogs.FirstOrDefault(c => c.Id == config.TicketDesign).Description;
            return new RequestResponseModel()
            {
                Result = true,
                Object = ticketDesing
            };
        }
    }
}