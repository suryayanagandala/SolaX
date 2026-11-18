using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{

    [Table("OtpManager")]
    public class OtpManager
    {

        [Key]
        [Column("OtpId")]
        public int Id { get; set; }

        [StringLength(50)]
        [Column("EmailId")]
        public required string Email{ get; set; }

        [StringLength(10)]
        [Column("OtpText")]
        public string Otptext { get; set; } = null!;

        [StringLength(20)]
        [Column("OtpType")]
        public string? Otptype { get; set; }

        [Column("expiration")]
        public DateTimeOffset Expiration { get; set; }

        [Column("createdAt")]
        public DateTimeOffset CreatedDate { get; set; }

        [Column("IsUsed")]
        public bool IsUsed {  get; set; }= false;

        [Column("ResetToken")]
        public Guid token { get; set; }
    }
}
