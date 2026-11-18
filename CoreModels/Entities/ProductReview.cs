using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    public class ProductReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public required string  UserId { get; set; }

        [Required]
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("ProductId")]
        public Products Product { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public Users Users { get; set; }= null!;

    }
}
