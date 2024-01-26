using System;

namespace Tickets.Models.AuxModels
{
    public class AuxAward
    {
        public int Id { get; set; }
        public int ProspectId { get; set; }
        public int OrderAward { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public Nullable<int> SourceAward { get; set; }
        public Nullable<int> Terminal { get; set; }
        public decimal Value { get; set; }
        public decimal TotalValue { get; set; }
        public int ByFraction { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public int TypesAwardId { get; set; }
    }
}
