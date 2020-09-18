namespace Suddath.Helix.JobMgmt.Models
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string ActivityTypeCode { get; set; }
        public int JobId { get; set; }
        public int? SuperServiceOrderId { get; set; }
    }
}
