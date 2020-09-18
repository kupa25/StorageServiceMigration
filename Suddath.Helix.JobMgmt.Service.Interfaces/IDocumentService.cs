using System.Collections.Generic;
using System.Threading.Tasks;
using Suddath.Helix.JobMgmt.Models;

namespace Suddath.Helix.JobMgmt.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<List<DocumentDto>> GetAppropriateDocsForUserByEmail(string email);
        Task<List<DocumentDetailDto>> GetFlattenedDocsForUserByEmail(string email);
        Task<DocumentDetailDto> SaveOrUpdateDocUploadHistory(DocumentDetailDto doc);
    }
}
