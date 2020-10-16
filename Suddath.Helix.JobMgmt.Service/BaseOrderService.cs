using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using System.Linq;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services
{
    public abstract class BaseOrderService : BaseService
    {
        private JobDbContext _dbContext;

        public BaseOrderService(JobDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(dbContext, mapper, httpContextAccessor)
        {
            _dbContext = dbContext;
        }

        protected async Task<GetJobCostMetricsResponse> GetOceanFreightMetrics(int superServiceOrderId, int? accountEntityId = null)
        {
            var response = new GetJobCostMetricsResponse();

            var serviceOrder = _dbContext.ServiceOrder.AsNoTracking()
                .Include(s => s.SuperServiceOrder)
                .FirstOrDefault(so => so.ServiceId == ServiceId.OCEAN_OCEAN_FREIGHT && so.SuperServiceOrderId == superServiceOrderId);

            switch (serviceOrder.SuperServiceOrder.SuperServiceModeId.GetValueOrDefault())
            {
                case SuperServiceModeId.OCEAN_FCL:
                    var OFContainerStuff = _dbContext.ServiceOrderOceanFreightContainer.AsNoTracking()
                        .Include(c => c.ServiceOrderOceanFreightLooseItem)
                        .Include(c => c.ServiceOrderOceanFreightLiftVan)
                        .Where(c => c.ServiceOrderId == serviceOrder.Id);

                    response.GrossWeightLb = OFContainerStuff.SelectMany(x => x.ServiceOrderOceanFreightLooseItem.Select(li => li.GrossWeightLb)).Sum() +
                        OFContainerStuff.SelectMany(x => x.ServiceOrderOceanFreightLiftVan.Select(lv => lv.GrossWeightLb)).Sum();

                    response.NetWeightLb = OFContainerStuff.SelectMany(x => x.ServiceOrderOceanFreightLooseItem.Select(li => li.NetWeightLb)).Sum() +
                        OFContainerStuff.SelectMany(x => x.ServiceOrderOceanFreightLiftVan.Select(lv => lv.NetWeightLb)).Sum();

                    response.GrossVolumeCUFT = OFContainerStuff.SelectMany(x => x.ServiceOrderOceanFreightLooseItem.Select(li => li.VolumeCUFT)).Sum() +
                        OFContainerStuff.SelectMany(x => x.ServiceOrderOceanFreightLiftVan.Select(lv => lv.VolumeCUFT)).Sum();
                    break;

                case SuperServiceModeId.OCEAN_LCL:
                    var OFLcls = _dbContext.ServiceOrderOceanFreightLCL.AsNoTracking()
                        .Where(so => so.ServiceOrderId == serviceOrder.Id);

                    response.GrossWeightLb = OFLcls.Select(x => x.GrossWeightLb).Sum();
                    response.NetWeightLb = OFLcls.Select(x => x.NetWeightLb).Sum();
                    response.GrossVolumeCUFT = OFLcls.Select(x => x.VolumeCUFT).Sum();
                    break;

                case SuperServiceModeId.OCEAN_RO_RO:
                    var OFRoRos = _dbContext.ServiceOrderOceanFreightVehicle.AsNoTracking()
                        .Where(roro => roro.ServiceOrderId == serviceOrder.Id && roro.ServiceOrderOceanFreightContainerId == null);

                    response.GrossWeightLb = OFRoRos.Select(x => x.WeightLb).Sum();
                    response.NetWeightLb = OFRoRos.Select(x => x.WeightLb).Sum();
                    response.GrossVolumeCUFT = OFRoRos.Select(x => x.VolumeCUFT).Sum();
                    break;
            }

            var surveyEntity = await _dbContext.SuperServiceOrderSurveyResult.FindAsync(superServiceOrderId);
            response.SurveyNetWeightLb = surveyEntity?.NetWeightLb;
            response.SurveyGrossWeightLb = surveyEntity?.GrossWeightLb;
            if (accountEntityId.HasValue)
            {
                response.OverweightPercentage = GetOverweightPercentage(accountEntityId.Value);
            }
            //get authorized gross & net weight
            await GetAuthorizedWeights(response, serviceOrder);

            return response;
        }

        protected async Task<GetJobCostMetricsResponse> GetAirFreightMetrics(int superServiceOrderId, int? accountEntityId = null)
        {
            var response = new GetJobCostMetricsResponse();

            var serviceOrder = _dbContext.ServiceOrder.Include(s => s.SuperServiceOrder).AsNoTracking().FirstOrDefault(so => so.ServiceId == ServiceId.AIR_AIR_FREIGHT && so.SuperServiceOrderId == superServiceOrderId);

            var AFItems = _dbContext.ServiceOrderAirFreightItem.AsNoTracking()
                .Where(soafi => soafi.ServiceOrderId == serviceOrder.Id);

            response.GrossWeightLb = AFItems.Select(x => x.GrossWeightLb).Sum();
            response.NetWeightLb = AFItems.Select(x => x.NetWeightLb).Sum();
            response.GrossVolumeCUFT = AFItems.Select(x => x.VolumeCUFT).Sum();

            var surveyEntity = await _dbContext.SuperServiceOrderSurveyResult.FindAsync(superServiceOrderId);
            response.SurveyNetWeightLb = surveyEntity?.NetWeightLb;
            response.SurveyGrossWeightLb = surveyEntity?.GrossWeightLb;
            if (accountEntityId.HasValue)
            {
                response.OverweightPercentage = GetOverweightPercentage(accountEntityId.Value);
            }
            //get authorized gross & net weight
            await GetAuthorizedWeights(response, serviceOrder);

            return response;
        }

        protected async Task<GetJobCostMetricsResponse> GetRoadFreightMetrics(int superServiceOrderId, int? accountEntityId = null)
        {
            var response = new GetJobCostMetricsResponse();

            var serviceOrder = _dbContext.ServiceOrder.Include(s => s.SuperServiceOrder).FirstOrDefault(so => so.ServiceId == ServiceId.ROAD_FREIGHT && so.SuperServiceOrderId == superServiceOrderId);

            var RFItems = _dbContext.ServiceOrderRoadFreightLTL
                .Where(sorfi => sorfi.ServiceOrderId == serviceOrder.Id);

            response.GrossWeightLb = RFItems.Select(x => x.GrossWeightLb).Sum();
            response.NetWeightLb = RFItems.Select(x => x.NetWeightLb).Sum();
            response.GrossVolumeCUFT = RFItems.Select(x => x.VolumeCUFT).Sum();

            var surveyEntity = await _dbContext.SuperServiceOrderSurveyResult.FindAsync(superServiceOrderId);
            response.SurveyNetWeightLb = surveyEntity?.NetWeightLb;
            response.SurveyGrossWeightLb = surveyEntity?.GrossWeightLb;
            if (accountEntityId.HasValue)
            {
                response.OverweightPercentage = GetOverweightPercentage(accountEntityId.Value);
            }
            //get authorized gross & net weight
            await GetAuthorizedWeights(response, serviceOrder);

            return response;
        }

        protected async Task<GetJobCostMetricsResponse> GetStorageOAMetrics(int superServiceOrderId, int? accountEntityId = null)
        {
            var response = new GetJobCostMetricsResponse();

            var serviceOrder = _dbContext.ServiceOrder.Include(s => s.SuperServiceOrder).FirstOrDefault(so => so.ServiceId == ServiceId.STORAGE_ORIGIN_AGENT &&
                                                                              so.SuperServiceOrderId == superServiceOrderId);

            var OaItems = _dbContext.ServiceOrderMoveInfo
                .Where(somi => somi.ServiceOrderId == serviceOrder.Id);

            response.NetWeightLb = OaItems.Select(x => x.NetWeightLb).Sum();

            var surveyEntity = await _dbContext.SuperServiceOrderSurveyResult.FindAsync(superServiceOrderId);
            response.SurveyNetWeightLb = surveyEntity?.NetWeightLb;
            response.SurveyGrossWeightLb = surveyEntity?.GrossWeightLb;
            if (accountEntityId.HasValue)
            {
                response.OverweightPercentage = GetOverweightPercentage(accountEntityId.Value);
            }
            //get authorized gross & net weight
            await GetAuthorizedWeights(response, serviceOrder);

            return response;
        }

        protected async Task GetAuthorizedWeights(GetJobCostMetricsResponse response, ServiceOrder serviceOrder)
        {
            var authorizations = await _dbContext.JobSuperServiceAuthorization.AsNoTracking()
                                     .Where(a => a.JobId == serviceOrder.JobId &&
                                                 a.MeasurementType.Contains("LBS")).ToListAsync();

            var ssType = authorizations.Where(a => a.SuperServiceId == serviceOrder.SuperServiceOrder.SuperServiceId);

            response.AuthorizedNetWeight = ssType.Where(a => a.MeasurementType.ToUpper().Equals(MeasurementType.NET_LBS)).Select(a => a.Amount).Sum();
            response.AuthorizedGrossWeight = ssType.Where(a => a.MeasurementType.ToUpper().Equals(MeasurementType.GROSS_LBS)).Select(a => a.Amount).Sum();
        }

        protected decimal GetOverweightPercentage(int accountEntityId)
        {
            return accountEntityId == 214 ? (decimal)1.05 : (decimal)1.10;
        }
    }
}