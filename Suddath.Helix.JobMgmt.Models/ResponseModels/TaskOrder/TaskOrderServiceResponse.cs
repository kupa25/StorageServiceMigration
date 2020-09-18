namespace Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder
{
    public class TaskOrderServiceResponse
    {
        public string SuperServiceType { get; set; }

        public string ServiceType { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public decimal? EstimatedWeightLbs { get; set; }
        public decimal? SurveyedWeightLbs { get; set; }
        public decimal? ActualWeightLbs { get; set; }
    }
}