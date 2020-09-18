using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Suddath.Helix.JobMgmt.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentDetailDto
    {
        public int DocumentId { get; set; }
        public string UserId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentDisplayName { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentStatusDescription { get; set; }
        public string DocumentUploadedDate { get; set; }
        public string FileType
        {
            get
            {
                return DocumentName.Substring(DocumentName.LastIndexOf(".") + 1);
            }
        }

        #region ForUpload
        public string TransfereeEmail { get; set; }
        public string CounselorEmail { get; set; }
        public string OrderNo { get; set; }
        public string TransfloDocumentType { get; set; }
        public bool IsUploadable { get; set; }
        #endregion
    }

    public class DocumentDetaiComparer : IEqualityComparer<DocumentDetailDto>
    {
        public bool Equals(DocumentDetailDto x, DocumentDetailDto y)
        {
            return x.DocumentId == y.DocumentId;
        }

        public int GetHashCode(DocumentDetailDto obj)
        {
            return obj.DocumentId;
        }
    }
}
