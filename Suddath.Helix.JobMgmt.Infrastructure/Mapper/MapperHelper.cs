using AutoMapper;
using Suddath.Helix.Common.Extensions;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrderStorage;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public static class DtoTranslations
    {
        static DtoTranslations()
        {
            Mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<JobsMapperProfile>();
                cfg.AddProfile<EventMapperProfile>();
                cfg.AddProfile<TaskOrderMapperProfile>();
            }).CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static TransfereeDto ToDto(this Transferee transferee)
        {
            return Mapper.Map<TransfereeDto>(transferee);
        }

        public static TransfereeFlatDto ToTransfereeResponse<T>(this Job job)
        {
            return Mapper.Map<TransfereeFlatDto>(job);
        }

        public static GetTaskOrderMemberInfoResponse ToTaskOrderMemberInfoResponse<T>(this Job job)
        {
            return Mapper.Map<GetTaskOrderMemberInfoResponse>(job);
        }

        public static T ToJobDto<T>(this Job job)
        {
            return Mapper.Map<T>(job);
        }

        public static T FromJob<T>(this Job job)
        {
            return Mapper.Map<T>(job);
        }

        public static T ToTaskOrderDto<T>(this TaskOrder taskOrder)
        {
            return Mapper.Map<T>(taskOrder);
        }

        public static T ToTransfereeIntegrationEvent<T>(this Job2Dto job2Dto)
        {
            return Mapper.Map<T>(job2Dto);
        }

        public static T FromJobSurveyInfo<T>(this JobSurveyInfo job)
        {
            return Mapper.Map<T>(job);
        }

        public static S ToIntegrationEvent<T, S>(this T entity)
        {
            return Mapper.Map<S>(entity);
        }

        public static JobDto ToJob(Job job)
        {
            return Mapper.Map<JobDto>(job);
        }

        public static CreateBillableItemResponse ToCreateBillableItemResponse(BillableItem entity)
        {
            var response = new CreateBillableItemResponse();

            switch (entity.BillToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    response.BillToType = EntityType.ACCOUNT_ENTITY;
                    response.BillToId = entity.BillToAccountEntityId;
                    response.BillToName = entity.BillToAccountEntity?.Name;
                    break;

                case EntityType.VENDOR:
                    response.BillToType = EntityType.VENDOR;
                    response.BillToId = entity.BillToVendorId;
                    response.BillToName = entity.BillToVendor?.Name;
                    break;

                case EntityType.TRANSFEREE:
                    response.BillToType = EntityType.TRANSFEREE;
                    response.BillToId = entity.BillToTransfereeId;
                    response.BillToName = string.Concat(entity.BillToTransferee?.FirstName, " ", entity.BillToTransferee?.LastName);
                    break;
            }

            return response;
        }

        public static CreateVendorInvoiceResponse ToCreateVendorInvoiceResponse(VendorInvoice entity)
        {
            int vendorId = 0;

            var response = new CreateVendorInvoiceResponse()
            {
                SuperServiceName = entity.SuperServiceOrder.SuperService?.SuperServiceName,
                DisplayId = entity.SuperServiceOrder.DisplayId
            };

            //TODO: FIX ME BY FIXING THE LINQ QUERY>GetJobsForSearchIndexAsync
            switch (entity.BillFromType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    vendorId = entity.BillFromAccountEntityId.GetValueOrDefault();

                    response.BillFromType = EntityType.ACCOUNT_ENTITY;
                    response.BillFromId = entity.BillFromAccountEntityId;
                    response.BillFromName = entity.BillFromAccountEntity?.Name;
                    response.VendorAccountingId = entity.BillFromAccountEntity?.AccountingId;
                    response.VendorShortAddress = entity.BillFromAccountEntity.ShortAddress;

                    break;

                case EntityType.VENDOR:
                    vendorId = entity.BillFromVendorId.GetValueOrDefault();

                    response.BillFromType = EntityType.VENDOR;
                    response.BillFromId = entity.BillFromVendorId;
                    response.BillFromName = entity.BillFromVendor?.Name;
                    response.VendorAccountingId = entity.BillFromVendor?.Accounting_SI_Code;
                    response.VendorShortAddress = entity.BillFromVendor?.ShortAddress;

                    break;

                case EntityType.TRANSFEREE:
                    vendorId = entity.BillFromTransfereeId.GetValueOrDefault();
                    response.BillFromType = EntityType.TRANSFEREE;
                    response.BillFromId = entity.BillFromTransfereeId;
                    response.BillFromName = string.Concat(entity.BillFromTransferee?.FirstName, " ", entity.BillFromTransferee?.LastName);
                    response.VendorAccountingId = entity.BillFromTransferee?.AccountingId.ToString();
                    response.VendorShortAddress = ToShortAddress(entity.SuperServiceOrder.Job.OriginAddress);
                    break;
            }

            response.ServiceOrderId = entity.SuperServiceOrder.Job.ServiceOrder
                        .FirstOrDefault(s => s.SuperServiceOrderId == entity.SuperServiceOrderId && (
                                             s.ServiceId == ServiceId.AIR_JC ||
                                             s.ServiceId == ServiceId.OCEAN_JC ||
                                             s.ServiceId == ServiceId.ROAD_JC ||
                                             s.ServiceId == ServiceId.STORAGE_JC)).Id;

            return response;
        }

        public static CreateInvoiceResponse ToCreateInvoiceResponse(Invoice entity)
        {
            int vendorId = 0;

            var response = new CreateInvoiceResponse()
            {
                SuperServiceName = entity.SuperServiceOrder.SuperService.SuperServiceName,
                DisplayId = entity.SuperServiceOrder.DisplayId
            };

            //TODO: FIX ME BY FIXING THE LINQ QUERY>GetJobsForSearchIndexAsync
            switch (entity.BillToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    vendorId = entity.BillToAccountEntityId.GetValueOrDefault();
                    response.BillToType = EntityType.ACCOUNT_ENTITY;
                    response.BillToId = entity.BillToAccountEntityId;
                    response.BillToName = entity.BillToAccountEntity?.Name;
                    response.VendorAccountingId = entity.BillToAccountEntity?.AccountingId;
                    response.VendorShortAddress = entity.BillToAccountEntity.ShortAddress;

                    break;

                case EntityType.VENDOR:
                    vendorId = entity.BillToVendorId.GetValueOrDefault();
                    response.BillToType = EntityType.VENDOR;
                    response.BillToId = entity.BillToVendorId;
                    response.BillToName = entity.BillToVendor?.Name;
                    response.VendorAccountingId = entity.BillToVendor?.Accounting_SI_Code;
                    response.VendorShortAddress = entity.BillToVendor?.ShortAddress;

                    break;

                case EntityType.TRANSFEREE:
                    vendorId = entity.BillToTransfereeId.GetValueOrDefault();
                    response.BillToType = EntityType.TRANSFEREE;
                    response.BillToId = entity.BillToTransfereeId;
                    response.BillToName = string.Concat(entity.BillToTransferee?.FirstName, " ", entity.BillToTransferee?.LastName);
                    response.VendorAccountingId = entity.BillToTransferee?.AccountingId.ToString();
                    response.VendorShortAddress = ToShortAddress(entity.SuperServiceOrder.Job?.OriginAddress);

                    break;
            }

            response.ServiceOrderId = entity.SuperServiceOrder.Job.ServiceOrder
                        .FirstOrDefault(s => s.SuperServiceOrderId == entity.SuperServiceOrderId && (
                                             s.ServiceId == ServiceId.AIR_JC ||
                                             s.ServiceId == ServiceId.OCEAN_JC ||
                                             s.ServiceId == ServiceId.ROAD_JC ||
                                             s.ServiceId == ServiceId.STORAGE_JC)).Id;

            return response;
        }

        public static GetBillableItemResponse ToGetBillableItemResponse(BillableItem entity)
        {
            var response = new GetBillableItemResponse();

            switch (entity.BillToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    response.BillToType = EntityType.ACCOUNT_ENTITY;
                    response.BillToId = entity.BillToAccountEntityId;
                    response.BillToName = entity.BillToAccountEntity?.Name;
                    response.BillToLabel = $"{EntityType.ACCOUNT_ENTITY}-{entity.BillToAccountEntityId}";
                    break;

                case EntityType.VENDOR:
                    response.BillToType = EntityType.VENDOR;
                    response.BillToId = entity.BillToVendorId;
                    response.BillToName = entity.BillToVendor?.Name;
                    response.BillToLabel = $"{EntityType.VENDOR}-{entity.BillToVendorId}";
                    break;

                case EntityType.TRANSFEREE:
                    response.BillToType = EntityType.TRANSFEREE;
                    response.BillToId = entity.BillToTransfereeId;
                    response.BillToName = $"{entity.BillToTransferee?.FirstName} {entity.BillToTransferee?.LastName}";
                    response.BillToLabel = $"{EntityType.TRANSFEREE}-{entity.BillToTransfereeId}";
                    break;
            }

            return response;
        }

        internal static string GetContactName(Job src, string moveConsultant)
        {
            string result = null;

            if (src.JobContact != null && src.JobContact.Count > 0 && !string.IsNullOrEmpty(moveConsultant))
            {
                result = src.JobContact.FirstOrDefault(c => c.ContactType.ToUpper() == ConsultantType.MoveConsultant).FullName;
            }

            return result;
        }

        internal static string ToJobStatus(string status)
        {
            string statusPk = string.Empty;

            //TODO: Really bad way to do this.. but getting around a problem.
            switch (status.ToLower())
            {
                case "active-(booked)":
                    statusPk = JobStatusIdentifier.ACTIVE;
                    break;

                case "survey & bid":
                    statusPk = JobStatusIdentifier.SURVEY_BID;
                    break;

                case "On-Hold":
                    statusPk = JobStatusIdentifier.ON_HOLD;
                    break;

                default:
                    statusPk = status.ToUpper();
                    break;
            }

            return statusPk;
        }

        public static BillableItem ToBillableItem(GetBillableItemResponse dto)
        {
            var response = new BillableItem();

            switch (dto.BillToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    response.BillToType = EntityType.ACCOUNT_ENTITY;
                    response.BillToAccountEntityId = dto?.BillToId;
                    break;

                case EntityType.VENDOR:
                    response.BillToType = EntityType.VENDOR;
                    response.BillToVendorId = dto?.BillToId;
                    break;

                case EntityType.TRANSFEREE:
                    response.BillToType = EntityType.TRANSFEREE;
                    response.BillToTransfereeId = dto?.BillToId;
                    break;
            }

            return response;
        }

        public static ServiceOrderStorageRevenue ToBillToStorageRevenue(dynamic entity)
        {
            //TODO: Make this generic so that we don't have duplicate code from above.

            ServiceOrderStorageRevenue response = new ServiceOrderStorageRevenue();
            if (!string.IsNullOrEmpty(entity.BillToType))
            {
                switch (entity.BillToType.ToUpper())
                {
                    case EntityType.ACCOUNT_ENTITY:
                        response.BillToType = EntityType.ACCOUNT_ENTITY;
                        response.BillToAccountEntityId = entity?.BillToId;
                        break;

                    case EntityType.VENDOR:
                        response.BillToType = EntityType.VENDOR;
                        response.BillToVendorId = entity?.BillToId;
                        break;

                    case EntityType.TRANSFEREE:
                        response.BillToType = EntityType.TRANSFEREE;
                        response.BillToTransfereeId = entity?.BillToId;
                        break;
                }
            }

            return response;
        }

        public static GetStorageRevenueResponse ToBillToStorageResponse(ServiceOrderStorageRevenue entity)
        {
            //TODO: Make this generic so that we don't have duplicate code from above.

            GetStorageRevenueResponse response = new GetStorageRevenueResponse();

            if (!string.IsNullOrEmpty(entity.BillToType))
            {
                switch (entity.BillToType.ToUpper())
                {
                    case EntityType.ACCOUNT_ENTITY:
                        response.BillToType = EntityType.ACCOUNT_ENTITY;
                        response.BillToId = entity.BillToAccountEntityId;
                        response.BillToName = entity.BillToAccountEntity?.Name;
                        response.BillToLabel = $"{EntityType.ACCOUNT_ENTITY}-{entity.BillToAccountEntityId}";
                        break;

                    case EntityType.VENDOR:
                        response.BillToType = EntityType.VENDOR;
                        response.BillToId = entity.BillToVendorId;
                        response.BillToName = entity.BillToVendor?.Name;
                        response.BillToLabel = $"{EntityType.VENDOR}-{entity.BillToVendorId}";
                        break;

                    case EntityType.TRANSFEREE:
                        response.BillToType = EntityType.TRANSFEREE;
                        response.BillToId = entity.BillToTransfereeId;
                        response.BillToName = $"{entity.BillToTransferee?.FirstName} {entity.BillToTransferee?.LastName}";
                        response.BillToLabel = $"{EntityType.TRANSFEREE}-{entity.BillToTransfereeId}";
                        break;
                }
            }

            return response;
        }

        internal static object ToDateTime(DateTime? date, TimeSpan? time)
        {
            if (!time.HasValue)
            {
                return null;
            }

            var result = date.HasValue ? date.GetValueOrDefault() : new DateTime();
            result = result + time.GetValueOrDefault();

            return result;
        }

        public static JobInfoDto ToJobInfo(Job job)
        {
            var originAddress = Mapper.Map<AddressDto>(job.OriginAddress);
            var destinationAddress = Mapper.Map<AddressDto>(job.DestinationAddress);

            return new JobInfoDto()
            {
                OriginAddressLabel = job.OriginAddress?.Display,
                OriginAddressAdditionalInfo = job.OriginAddress?.AdditionalAddressInfo,
                DestinationAddressLabel = job.DestinationAddress?.Display,
                DestinationAddressAdditionalInfo = job.DestinationAddress?.AdditionalAddressInfo,
                Addresses = new List<AddressDto>
                {
                    originAddress,
                    destinationAddress
                }
            };
        }

        internal static object ToTimeSpan(DateTime? datetime)
        {
            if (!datetime.HasValue)
            {
                return null;
            }

            return datetime.GetValueOrDefault().TimeOfDay;
        }

        public static TransfereeDto ToTransferee(Transferee transferee)
        {
            var originPhones = new List<PhoneDto>();
            var destinationPhones = new List<PhoneDto>();

            foreach (var item in transferee.TransfereePhone)
            {
                var phoneDto = Mapper.Map<PhoneDto>(item.Phone);

                if (item.Type?.ToLowerClean() == "origin")
                {
                    phoneDto.LocationType = "Origin";
                    originPhones.Add(phoneDto);
                }
                else if (item.Type?.ToLowerClean() == "destination")
                {
                    phoneDto.LocationType = "Destination";
                    destinationPhones.Add(phoneDto);
                }
            }

            var transfereeDto = transferee.ToDto();
            transfereeDto.Emails = Mapper.Map<List<EmailDto>>(transferee.Email);
            transfereeDto.OriginPhones = originPhones;
            transfereeDto.DestinationPhones = destinationPhones;

            return transfereeDto;
        }

        public static TaskOrderResponse ToTaskOrder(TaskOrder taskOrder)
        {
            return Mapper.Map<TaskOrderResponse>(taskOrder);
        }

        public static string ToShortAddress(Address address)
        {
            if (address == null)
            {
                return string.Empty;
            }

            return ToShortAddress(address.City, address.State, address.Country);
        }

        public static string ToShortAddress(HfAddressDto address)
        {
            if (address == null)
            {
                return string.Empty;
            }

            return ToShortAddress(address.City, address.StateOrProvince, address.CountryCode);
        }

        public static string ToShortAddress(string city, string state, string country)
        {
            if (!string.IsNullOrEmpty(country))
            {
                if (country.IsUSA())
                {
                    return string.Concat(string.IsNullOrEmpty(city) ? string.Empty : string.Concat(city, ", "), state);
                }
                else
                {
                    return string.Concat(string.IsNullOrEmpty(city) ? string.Empty : string.Concat(city, ", "), country);
                }
            }
            else
            {
                return String.Empty;
            }
        }

        public static string ToShortAddress(DutyStationAddressDto address)
        {
            if (!string.IsNullOrEmpty(address.CountryCode))
            {
                if (address.CountryCode.IsUSA())
                {
                    return string.Concat(string.IsNullOrEmpty(address.City) ? string.Empty : string.Concat(address.City, ", "), address.StateOrProvince);
                }
                else
                {
                    return string.Concat(string.IsNullOrEmpty(address.City) ? string.Empty : string.Concat(address.City, ", "), address.CountryCode);
                }
            }
            else
            {
                return String.Empty;
            }
        }

        #region Job Contacts

        public static JobContactDto ToModel(this JobContact entity)
        {
            return Mapper.Map<JobContactDto>(entity);
        }

        public static JobContact ToNewEntity(this CreateJobContactDto model)
        {
            return Mapper.Map<JobContact>(model);
        }

        public static JobContact ToEntity(this JobContactDto model)
        {
            return Mapper.Map<JobContact>(model);
        }

        #endregion Job Contacts

        #region ForSearchIndex

        public static string GetMasterAwbNumber(ICollection<ServiceOrder> serviceOrders)
        {
            var serviceOrder = serviceOrders?.Where(so => so.ServiceOrderAirFreight != null).FirstOrDefault();
            return serviceOrder?.ServiceOrderAirFreight?.MasterAwbNumber;
        }

        public static string GetMasterBolNumber(ICollection<ServiceOrder> serviceOrders)
        {
            var serviceOrder = serviceOrders?.Where(so => so.ServiceOrderOceanFreight != null).FirstOrDefault();
            return serviceOrder?.ServiceOrderOceanFreight?.MasterBOLNumber;
        }

        public static ICollection<string> GetContainers(ICollection<ServiceOrder> serviceOrders)
        {
            var containers = new List<string>();

            foreach (var item in serviceOrders?.Where(so => so.ServiceOrderOceanFreightContainer != null))
            {
                foreach (var c in item.ServiceOrderOceanFreightContainer?.Where(f => !string.IsNullOrEmpty(f.ContainerNumber)))
                {
                    containers.Add(c.ContainerNumber);
                }
            }
            return containers;
        }

        public static ICollection<JobSearchInvoiceDocument> GetSearchInvoices(ICollection<SuperServiceOrder> ssos)
        {
            var invs = new List<JobSearchInvoiceDocument>();

            foreach (var item in ssos?.Where(so => so.VendorInvoice != null && so.VendorInvoice.Count > 0))
            {
                foreach (var v in item.VendorInvoice?.Where(f => !string.IsNullOrEmpty(f.VendorInvoiceNumber)))
                {
                    var invEntity = ToCreateVendorInvoiceResponse(v);
                    invs.Add(new JobSearchInvoiceDocument()
                    {
                        SuperServiceOrderId = v.SuperServiceOrderId,
                        SuperServiceName = invEntity.SuperServiceName,
                        DisplayId = invEntity.DisplayId,
                        ItemId = invEntity.ServiceOrderId,
                        InvoiceNumber = v.VendorInvoiceNumber,
                        VendorName = invEntity.BillFromName,
                        VendorAccountingId = invEntity.VendorAccountingId,
                        VendorShortAddress = invEntity.VendorShortAddress
                    });
                }
            }

            foreach (var item in ssos?.Where(so => so.Invoice != null))
            {
                foreach (var v in item.Invoice?.Where(f => !string.IsNullOrEmpty(f.InvoiceNumber)))
                {
                    var invEntity = ToCreateInvoiceResponse(v);
                    invs.Add(new JobSearchInvoiceDocument()
                    {
                        SuperServiceOrderId = v.SuperServiceOrderId,
                        SuperServiceName = invEntity.SuperServiceName,
                        DisplayId = invEntity.DisplayId,
                        ItemId = invEntity.ServiceOrderId,
                        InvoiceNumber = v.InvoiceNumber,
                        VendorName = invEntity.BillToName,
                        VendorAccountingId = invEntity.VendorAccountingId,
                        VendorShortAddress = invEntity.VendorShortAddress
                    });
                }
            }

            return invs;
        }

        #endregion ForSearchIndex

        #region payables

        public static GetPayableItemResponse ToGetPayableItemResponse(PayableItem entity)
        {
            var response = new GetPayableItemResponse();

            switch (entity.BillFromType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    response.BillFromType = EntityType.ACCOUNT_ENTITY;
                    response.BillFromId = entity.BillFromAccountEntityId;
                    response.BillFromName = entity.BillFromAccountEntity?.Name;
                    response.BillFromLabel = string.Concat(EntityType.ACCOUNT_ENTITY, "-", entity.BillFromAccountEntityId);
                    break;

                case EntityType.VENDOR:
                    response.BillFromType = EntityType.VENDOR;
                    response.BillFromId = entity.BillFromVendorId;
                    response.BillFromName = entity.BillFromVendor?.Name;
                    response.BillFromLabel = string.Concat(EntityType.VENDOR, "-", entity.BillFromVendorId);

                    break;

                case EntityType.TRANSFEREE:
                    response.BillFromType = EntityType.TRANSFEREE;
                    response.BillFromId = entity.BillFromTransfereeId;
                    response.BillFromName = string.Concat(entity.BillFromTransferee?.FirstName, " ", entity.BillFromTransferee?.LastName);
                    response.BillFromLabel = string.Concat(EntityType.TRANSFEREE, "-", entity.BillFromTransfereeId);

                    break;
            }

            return response;
        }

        public static PayableItem ToPayableItem(GetPayableItemResponse dto)
        {
            var response = new PayableItem();

            switch (dto.BillFromType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    response.BillFromType = EntityType.ACCOUNT_ENTITY;
                    response.BillFromAccountEntityId = dto?.BillFromId;
                    break;

                case EntityType.VENDOR:
                    response.BillFromType = EntityType.VENDOR;
                    response.BillFromVendorId = dto?.BillFromId;
                    break;

                case EntityType.TRANSFEREE:
                    response.BillFromType = EntityType.TRANSFEREE;
                    response.BillFromTransfereeId = dto?.BillFromId;
                    break;
            }

            return response;
        }

        public static JobSuperServiceAuthorization FilterAuthorization(int superServiceId, string measurementType, ICollection<JobSuperServiceAuthorization> authorizationsList)
        {
            return authorizationsList.SingleOrDefault(a => a.SuperServiceId == superServiceId && a.MeasurementType.ToUpper().Equals(measurementType));
        }

        #endregion payables
    }
}