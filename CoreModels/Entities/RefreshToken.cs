using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("RefreshTokens")]
    public class RefreshTokens
    {
        [Key]
        public int TokenId {  get; set; }

        [Required]
        [StringLength(6)]
        public string UserId { get; set; } = null!;

        [Required]
        public string RefreshToken {  get; set; }= null!;


        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }



    }
}
