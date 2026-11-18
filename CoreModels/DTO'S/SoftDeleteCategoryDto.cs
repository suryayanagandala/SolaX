using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class SoftDeleteCategoryDto
    {
        public required int CategoryId {  get; set; }    

        public required string Reason {  get; set; }
    }
}
