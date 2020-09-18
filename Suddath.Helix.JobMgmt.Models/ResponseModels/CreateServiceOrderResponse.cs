namespace Suddath.Helix.JobMgmt.Models.ResponseModels
{
    public class CreateServiceOrderResponse
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAbbreviation { get; set; }
    }
}