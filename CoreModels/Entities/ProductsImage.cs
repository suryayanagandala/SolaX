using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("ProductImage")]
    public class ProductImage
    {
        [Key]
        [Column("ImageId")]
        public int ImageId {  get; set; }

        [Required]
        [Column("ProductId")]
        public int ProductId {  get; set; }

        [Required]
        [Column("ImageUrl")]
        public required string ImageUrl { get; set; }

        [Column("CreatedAt")]
        public DateTimeOffset CreatedAt {  get; set; }

        [Column("UpdatedAt")]
        public DateTimeOffset? UpdatedAt { get;set; }
            
        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [ForeignKey("ProductId")]
        public Products Product { get; set; } = null!;

    }
}
