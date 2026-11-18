using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; } = null!;

        [Required]
        public required string Slug { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? DeletedReason {  get; set; }

        public DateTimeOffset? DeletedAt {  get; set; }

        public string? ImageUrl { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }



        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public ICollection<Products> Products { get; set; }= new List<Products>();

        //public ICollection<AccessoryCategoryMapping> AccessoryCategories { get; set; }=new List<AccessoryCategoryMapping>();

        public bool IsActive { get; set; } = true;
    }

}
