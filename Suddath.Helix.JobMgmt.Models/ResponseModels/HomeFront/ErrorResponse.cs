namespace Suddath.Helix.JobMgmt.Models.ResponseModels.HomeFront
{
    public class ErrorResponse
    {
        public string TaskOrderIdentifier { get; set; }
        public ErrorDetailsResponse ErrorDetails { get; set; }
    }
}