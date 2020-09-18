using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models.ResponseModels.HomeFront
{
    public class ErrorDetailsResponse
    {
        public string SourceSystemType { get; set; }
        public string OriginalMessagePayload { get; set; }
        public IEnumerable<ErrorItemResponse> ErrorList { get; set; }
    }
}