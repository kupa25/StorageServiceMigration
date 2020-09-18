namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class GetTaskOrderActivityResponse
    {
        public int ActivityId { get; set; }
        public int JobId { get; set; }
        public string ActivityTypeCode { get; set; }
    }
}