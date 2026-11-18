using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Panel")]
    public class Panel
    {


        [Key]
        public int PanelId { get; set; }

       
        public string UserId { get; set; } = null!;

        public string PanelName { get; set; } = null!;

        public string Capacity { get; set; } = null!;

        public bool IsActive {  get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }= null!;

        public ICollection<SolarPerformance> Performances { get; set; } = new List<SolarPerformance>();
    }


}
