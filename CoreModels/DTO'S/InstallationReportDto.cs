using iText.Forms.Form.Element;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO_S
{
    public class InstallationReportDto
    {
        public string InstallationId { get; set; } = null!;
        public string? Notes { get; set; }
        public string? CheckList { get; set; }

        public List<IFormFile> Images { get; set; } = new();
    }

    public class MaintenanceReportDto
    {
        public string RequestId {  get; set; }=string.Empty;

        public string Notes { get; set; } = string.Empty;

        public List<IFormFile> Images { get; set; } = new();

        public string ReplacedParts {  get; set; } = string.Empty;



    }
}
