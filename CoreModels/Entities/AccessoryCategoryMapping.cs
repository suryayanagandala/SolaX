using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("AccessoryCategoryMapping")]
    public class AccessoryCategoryMapping
    {
        [Key]
        public int Id { get; set; }

        public int SourceCategoryId { get; set; }

        public int AccessoryCategoryId { get; set; }

        public bool IsAutoMapped { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(SourceCategoryId))]
        public Category SourceCategory { get; set; } = null!;

        [ForeignKey(nameof(AccessoryCategoryId))]
        public Category AccessoryCategory { get; set; } = null!;
    }
}
