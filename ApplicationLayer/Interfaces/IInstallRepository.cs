using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IInstallRepository
    {
        Task<List<InstallationCreateDto>> GetInstallationStatus(string userId);

        Task<string> UploadInstallationReports(InstallationReportDto dto);

        Task MarkasProgress(string id);

       
    }
}
