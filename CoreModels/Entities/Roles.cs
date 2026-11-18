using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoreModels.Entities
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        public int RoleId {  get; set; }

        [Required, StringLength(50)]
        [Unicode(false)]
        public required string RoleName { get; set; }

        [Required]
        public bool IsActive { get; set; }=true;

        [Required]
        public DateTimeOffset CreatedAt { get; set; }=DateTimeOffset.Now;

        public virtual ICollection<Users> Users { get; set; } = new List<Users>();

        public virtual ICollection<AccessPermissions> AccessPermissions { get; set; }=new List<AccessPermissions>();

    }
}
