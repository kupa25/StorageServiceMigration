using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Models
{
    public class MoveServiceDto : ServiceDto
    {
        public MoveServiceDto()
        {
            Contacts = new List<ContactDto>();
        }
        public string Id { get; set; }

        public string MobilityType { get; set; }

        public string ModeOfTransport { get; set; }
        public string OrderNumber { get; set; }

        public ICollection<ContactDto> Contacts { get; set; }

        public string Status { get; set; }

        public int? GrossWeight { get; set; }

        public int? NetWeight { get; set; }

        public string Account { get; set; }

        public MoveServiceDetailDto Origin { get; set; }
        public MoveServiceDetailDto Destination { get; set; }
        public bool IsSIMove
        {
            get
            {
                return Id.StartsWith("SI", StringComparison.OrdinalIgnoreCase);
            }
        }
        public bool IsMSMove
        {
            get
            {
                return Id.StartsWith("MS", StringComparison.OrdinalIgnoreCase);
            }
        }

        public VanLineDto VanLine { get; set; }

    }
}
