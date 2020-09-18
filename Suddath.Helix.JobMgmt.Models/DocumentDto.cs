using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public class DocumentDto
    {
        public DocumentDto()
        {
            Documents = new List<DocumentDetailDto>();
        }
        public string MoveId;
        public List<DocumentDetailDto> Documents { get; set; }
    }
}
