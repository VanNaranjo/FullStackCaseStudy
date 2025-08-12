using System.ComponentModel.DataAnnotations.Schema;

namespace CaseStudy.DAL.DomainClasses
{
    public class OrderLineItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // needs to be a FK
        public string? ProductId { get; set; } // needs to be a FK
        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyBackOrdered { get; set; }
        [Column(TypeName = "money")]
        public decimal SellingPrice { get; set; }
    }
}
