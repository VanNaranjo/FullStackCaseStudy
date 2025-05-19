using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaseStudy.DAL.DomainClasses
{
    public class Product
    {
        [Key]
        public string? Id { get; set; }
        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public string? ProductName { get; set; }
        [Required]
        public string? GraphicName { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal CostPrice { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal MSRP { get; set; }
        [Required]
        public int QtyOnHand { get; set; }
        [Required]
        public int QtyOnBackOrder { get; set; }
        [Required]
        [StringLength(50)]
        public string? Description { get; set; }
    }
}   