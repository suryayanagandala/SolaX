using CoreModels.CustomValidations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Users")]
    public class Users
    {
        [Key,StringLength(6)]
        [Column("UserId")]
        public required string UserId {  get; set; }

        [Required,MaxLength(200)]
        [Unicode(true)]
        [Column("FullName")]
        public required string FullName { get; set; }

        [Required,MaxLength(200)]
        [Unicode(false)]
        [Column("EmailId")]
        public required string EmailId {  get; set; }

        [Required,MinLength(8),MaxLength(12)]
        [Unicode(false)]
        [Column("PasswordHash")]
        [CustomValidation(typeof(CustomValidationForPassword), nameof(CustomValidationForPassword.ValidatePassword))]
        public required string Password { get; set; }
        
        [Required]
        [Column("RoleId")]
        public int RoleId { get; set; }

        [Required]
        [Column("CreatedAt")]
        public DateTimeOffset CreatedDate { get; set; }= DateTimeOffset.Now;


        [Required,MaxLength(350)]
        [Column("Address")]
        public required string Address {  get; set; }

        [Column("UpdatedBy")]
        public string? UpdatedBy {  get; set; }

        [Column("UpdatedAt")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        [Column("CreatedBy")]
        public required string CreatedBy {  get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive {  get; set; }= true;

        [Required,StringLength(10)]
        [Column("PhoneNumber")]
        public required string PhoneNumber {  get; set; }

        [ForeignKey("RoleId")]
        public Roles Role { get; set; } = null!;




        public Technicians? Technician { get; set; }

        public  ICollection<Cart> Cart {  get; set; }=new List<Cart>();

        public ICollection<Orders> Order { get; set; } = new List<Orders>();

        public ICollection<MaintenanceRequests> MaintenanceRequest { get; set;} = new List<MaintenanceRequests>();

        public ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();


        public ICollection<SolarPerformance> solarPerformances { get; set; }=new List<SolarPerformance>();

        public ICollection<ProductReview> ProductReviews { get; set; }= new List<ProductReview>();

        public ICollection<Panel> Panels { get; set; } = new List<Panel>();

    }
}
