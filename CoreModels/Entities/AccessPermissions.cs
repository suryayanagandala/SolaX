using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("AccessPermissions")]
    public class AccessPermissions
    {
        [Key]
        public int AccessId { get; set; }

        [Required]
        public int RoleId { get; set; } 

        [Required]
        public string Icon { get; set; } = null!;

        [Required]
        public string AccessName { get; set; } = null!;

        [Required]
        public string RoleName { get; set; } = null!;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = TimeUtility.NowIST();

        [Required]
        public string Url {  get; set; }= null!;

        public bool IsActive { get; set; } = true;


        [ForeignKey(nameof(RoleId))]
        public Roles Roles { get; set; }= null!;
    }
}
