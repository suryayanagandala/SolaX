using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{

    [Table("Products")]
    public class Products
    {
        [Key]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [Required,MaxLength(250)]
        [Column("ProductName")]
        public required string ProductName {  get; set; }


        public int CategoryId { get; set; }

        [Required,MaxLength(200)]
        [Column("Brand")]
        public required string Brand {  get; set; }

        [Required,MaxLength(100)]
        [Column("Type")]
        public required string Type {  get; set; }

        [Required]
        [Column("CapacityValue")]
        public int CapacityValue {  get; set; }

        [Required]
        [Column("Capacity")]
        public required string Capacity { get; set; }

        [Required]
        [Column("Price")]
        public decimal Price {  get; set; }

        [Required]
        [Column("Warranty_Years")]
        public decimal Warranty_Years {  get; set; }

        [Required]
        [Column("Stock")]
        public int Stock {  get; set; }

        [Required]
        [Column("Description")]
        public required string Description { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

      
        [Column("CreatedAt")]
        public DateTimeOffset CreatedAt {  get; set; }

       
        [Column("UpdatedAt")]
        public DateTimeOffset? UpdatedAt { get;set; }

        public double? AverageRating { get; set; }
        public int? RatingCount { get; set; }

        public string? DeletedReason { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }


        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;


        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public ICollection<CartItem> CartItem {  get; set; }= new List<CartItem>();

        public ICollection<OrderItems> OrderItem { get; set; }= new List<OrderItems>();

        public ICollection<ProductReview> ProductReviews { get; set; }=new List<ProductReview>();
    }
}
