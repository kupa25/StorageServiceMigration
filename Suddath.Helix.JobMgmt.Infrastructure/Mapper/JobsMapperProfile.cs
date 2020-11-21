using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Authorization;
using Suddath.Helix.JobMgmt.Models.ResponseModels.Survey;
using System;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class JobsMapperProfile : Profile
    {
        public JobsMapperProfile()
        {
            CreateMap<Job, TransfereeFlatDto>()
               .ForMember(d => d.TransfereeId, opt => opt.MapFrom(src => src.Transferee.Id))
               .ForMember(d => d.AccountingId, opt => opt.MapFrom(src => src.Transferee.AccountingId))
               .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.Transferee.FirstName))
               .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.Transferee.LastName))
               .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Transferee.Email.OrderBy(e => e.Id).FirstOrDefault().Value))
               .ForMember(d => d.Phone, opt => opt.MapFrom(src => src.Transferee.TransfereePhone))
               .ForMember(d => d.Phone, opt => opt.MapFrom(src => src.Transferee.TransfereePhone
                    .OrderByDescending(p => p.PhoneId)
                    .Select(p => new { Phone = string.Concat(p.Phone.DialCode, p.Phone.NationalNumber) })
                    .FirstOrDefault().Phone ?? string.Empty))
               .ForMember(d => d.Address1, opt => opt.MapFrom(src => src.DestinationAddress.Address1))
               .ForMember(d => d.Address2, opt => opt.MapFrom(src => src.DestinationAddress.Address2))
               .ForMember(d => d.Address3, opt => opt.MapFrom(src => src.DestinationAddress.Address3))
               .ForMember(d => d.City, opt => opt.MapFrom(src => src.DestinationAddress.City))
               .ForMember(d => d.State, opt => opt.MapFrom(src => src.DestinationAddress.State))
               .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src.DestinationAddress.PostalCode))
               .ForMember(d => d.AdditionalAddressInfo, opt => opt.MapFrom(src => src.DestinationAddress.AdditionalAddressInfo))
               .ForMember(d => d.Country, opt => opt.MapFrom(src => src.DestinationAddress.Country));

            CreateMap<Document, DocumentDetailDto>()
                .ForMember(d => d.DocumentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.DocumentName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(d => d.DocumentDisplayName, opt => opt.MapFrom(src => src.DisplayName));

            CreateMap<Job, JobDto>()
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.AccountLabel, opt => opt.MapFrom(src => src.AccountEntity.Name))
                .ForPath(d => d.Account.Name, opt => opt.MapFrom(src => src.AccountEntity.Name))
                .ForMember(d => d.BillToLabel, opt => opt.MapFrom(src => src.BillToLabel))
                .ForPath(d => d.BillTo.Id, opt => opt.MapFrom(src => src.BillToId))
                .ForPath(d => d.BillTo.Name, opt => opt.MapFrom(src => src.BillToLabel))
                .ForPath(d => d.Account.Id, opt => opt.MapFrom(src => src.AccountEntityId))
                .ForMember(d => d.BookerLabel, opt => opt.MapFrom(src => src.Booker.Name))
                .ForPath(d => d.Booker.Id, opt => opt.MapFrom(src => src.BookerId))
                .ForPath(d => d.Booker.Name, opt => opt.MapFrom(src => src.Booker.Name))
                .ForMember(d => d.AuthPoNum, opt => opt.MapFrom(src => src.AuthorizationPONumber))
                .ForMember(d => d.Status, opt => opt.MapFrom(src => src.JobStatusNavigation.JobStatusDisplayName))
                .ForMember(d => d.IsSurveyAndBid, opt => opt.MapFrom(src => src.JobStatus.Equals(JobStatusIdentifier.SURVEY_BID)))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.BranchName))
                .ForMember(d => d.BranchDisplayName, opt => opt.MapFrom(src => src.BranchNameNavigation.BranchName))
                .ForMember(d => d.BranchCode, opt => opt.MapFrom(src => src.BranchNameNavigation.BranchCode));

            CreateMap<JobDto, Job>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.JobId))
                .ForPath(d => d.BillToId, opt => opt.MapFrom(src => src.BillTo.Id))
                .ForMember(d => d.BillToType, opt => opt.MapFrom(src => src.BillToType))
                .ForPath(d => d.AccountEntityId, opt => opt.MapFrom(src => src.Account.Id))
                .ForPath(d => d.BookerId, opt => opt.MapFrom(src => src.Booker == null ? null : src.Booker.Id))
                .ForMember(d => d.AuthorizationPONumber, opt => opt.MapFrom(src => src.AuthPoNum))
                .ForMember(d => d.RevenueType, opt => opt.MapFrom(src => src.RevenueType))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.BranchName))
                .ForMember(d => d.MoveType, opt => opt.MapFrom(src => src.MoveType))
                .ForMember(d => d.JobStatus, opt => opt.MapFrom(src => DtoTranslations.ToJobStatus(src.Status)))
                .ForMember(d => d.AccrualStatus, opt => opt.MapFrom(src => src.AccrualStatus))
                .ForMember(d => d.JobSource, opt => opt.MapFrom(src => src.JobSource))
                .ForMember(d => d.AccountCustomerReference, opt => opt.MapFrom(src => src.AccountCustomerReference))
                .ForMember(d => d.ExternalReference, opt => opt.MapFrom(src => src.ExternalReference))
                .ForMember(d => d.ExternalReferenceDescription, opt => opt.MapFrom(src => src.ExternalReferenceDescription))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Transferee, TransfereeDto>().ReverseMap();

            CreateMap<Job, Job2Dto>()
                .ForMember(dest => dest.Job, opt => opt.MapFrom((src, d) => DtoTranslations.ToJob(src)))
                .ForMember(dest => dest.JobInfo, opt => opt.MapFrom((src, d) => DtoTranslations.ToJobInfo(src)))
                .ForMember(dest => dest.Transferee, opt => opt.MapFrom(src => DtoTranslations.ToTransferee(src.Transferee)));

            CreateMap<Job2Dto, Job>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Job.JobId))
                .ForMember(dest => dest.BillToId, opt => opt.MapFrom(src => src.Job.BillTo.Id))
                .ForMember(dest => dest.AccountEntityId, opt => opt.MapFrom(src => src.Job.Account.Id))
                .ForMember(dest => dest.BookerId, opt => opt.MapFrom(src => src.Job.Booker == null ? null : src.Job.Booker.Id))
                .ForMember(dest => dest.AuthorizationPONumber, opt => opt.MapFrom(src => src.Job.AuthPoNum))
                .ForMember(dest => dest.RevenueType, opt => opt.MapFrom(src => src.Job.RevenueType))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Job.BranchName))
                .ForMember(dest => dest.MoveType, opt => opt.MapFrom(src => src.Job.MoveType))
                .ForMember(dest => dest.TransfereeId, opt => opt.MapFrom(src => src.Transferee.Id))
                .ForMember(dest => dest.OriginAddress, opt => opt.MapFrom(src => src.JobInfo.Addresses.FirstOrDefault(s => s.Type == "Origin")))
                .ForMember(dest => dest.DestinationAddress, opt => opt.MapFrom(src => src.JobInfo.Addresses.FirstOrDefault(s => s.Type == "Destination")))
                .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => DtoTranslations.ToJobStatus(src.Job.Status)))
                .ForMember(dest => dest.AccountCustomerReference, opt => opt.MapFrom(src => src.Job.AccountCustomerReference))
                .ForMember(dest => dest.BranchNameNavigation, opt => opt.Ignore());

            CreateMap<Address, AddressDto>()
                .ReverseMap();

            CreateMap<AccountEntity, NameIdDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<Vendor, NameIdDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name)).ReverseMap();

            CreateMap<Vendor, NameIdDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<Email, EmailDto>()
                .ForMember(d => d.EmailType, opt => opt.MapFrom(src => src.Type))
                .ReverseMap();

            CreateMap<Phone, PhoneDto>()
                .ForMember(d => d.Primary, opt => opt.MapFrom(src => src.Primary))
                .ForMember(d => d.Extension, opt => opt.MapFrom(src => src.Extension))
                .ForMember(d => d.PhoneType, opt => opt.MapFrom(src => src.Type))
                .ForPath(d => d.LocationType, opt => opt.MapFrom(src => src.TransfereePhone.FirstOrDefault().Type))
                .ReverseMap();

            CreateMap<Job, GetJobsResponse>()
                .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.AccountName, opt => opt.MapFrom(src => src.AccountEntity.Name))
                .ForMember(d => d.TransfereeFirstName, opt => opt.MapFrom(src => src.Transferee.FirstName))
                .ForMember(d => d.TransfereeLastName, opt => opt.MapFrom(src => src.Transferee.LastName))
                .ForMember(d => d.TransfereePhone, opt => opt.MapFrom(src => src.Transferee.TransfereePhone
                               .FirstOrDefault(tp => tp.Phone.Primary.GetValueOrDefault()).Phone.NationalNumber))
                .ForMember(d => d.OriginAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.OriginAddress)))
                .ForMember(d => d.DestinationAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.DestinationAddress)))
                .ForMember(d => d.TransfereeEmail, opt => opt.MapFrom(src => src.Transferee.Email.OrderByDescending(e => e.Id)
                                    .Select(e => e.Value).FirstOrDefault()))
                .ForMember(d => d.BillToName, opt => opt.MapFrom(
                    src => src.BillToType.Equals("Account", StringComparison.InvariantCultureIgnoreCase) ? src.AccountEntity.Name : src.Booker.Name
                    ))
                .ForMember(d => d.DateModified, opt => opt.MapFrom(src => src.DateModified))
                .ForMember(d => d.MoveStatus, opt => opt.MapFrom(src => src.JobStatusNavigation.JobStatusDisplayName))
                .ForMember(d => d.BranchName, opt => opt.MapFrom(src => src.BranchNameNavigation.BranchName))
                .ForMember(d => d.MoveConsultantName, opt => opt.MapFrom(src => DtoTranslations.GetContactName(src, ConsultantType.MoveConsultant)));

            CreateMap<CreateJobContactDto, JobContact>()
                .ForMember(d => d.PhoneCountryCode, opt => opt.MapFrom(src => src.PhoneCountryCode))
                .ForMember(d => d.PhoneExtension, opt => opt.MapFrom(src => src.PhoneExtension));

            CreateMap<JobContact, JobContactDto>()
                .ForMember(d => d.PhoneCountryCode, opt => opt.MapFrom(src => src.PhoneCountryCode))
                .ForMember(d => d.PhoneExtension, opt => opt.MapFrom(src => src.PhoneExtension))
                .ReverseMap();

            CreateMap<SuperService, GetSuperServicesResponse>();
            CreateMap<SuperServiceOrderStatus, GetSuperServiceOrderStatusResponse>()
                .ForMember(d => d.StatusName, opt => opt.MapFrom(src => src.SuperServiceOrderStatusIdentifier))
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(src => src.SuperServiceOrderStatusDisplayName))
                ;
            CreateMap<JobStatus, GetJobStatusResponse>()
                .ForMember(d => d.StatusName, opt => opt.MapFrom(src => src.JobStatusIdentifier))
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(src => src.JobStatusDisplayName))
                ;

            CreateMap<SuperServiceMode, GetSuperServiceModeResponse>();

            CreateMap<SuperServiceOrder, GetSuperServiceOrderResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => src.SuperService.SuperServiceName))
                .ForMember(d => d.SuperServiceModeName, opt => opt.MapFrom(src => src.SuperServiceMode.ModeName))
                .ForMember(d => d.SuperServiceId, opt => opt.MapFrom(src => src.SuperService.Id))
                .ForMember(d => d.SuperServiceIconName, opt => opt.MapFrom(src => src.SuperService.SuperServiceIconName))
                .ForMember(d => d.Status, opt => opt.MapFrom(src => src.SuperServiceOrderStatusIdentifierNavigation.SuperServiceOrderStatusDisplayName))
                .ForPath(d => d.ServiceOrders, opt => opt.MapFrom(src => src.ServiceOrder))
                .AfterMap((s, resp) =>
                {
                    resp.ServiceOrders = resp.ServiceOrders.OrderBy(x => x.SortOrder);
                });

            CreateMap<SuperServiceOrder, CreateSuperServiceOrderResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => src.SuperService.SuperServiceName))
                .ForMember(d => d.SuperServiceModeName, opt => opt.MapFrom(src => src.SuperServiceMode.ModeName))
                .ForMember(d => d.SuperServiceIconName, opt => opt.MapFrom(src => src.SuperService.SuperServiceIconName))
                .ForMember(d => d.DisplayId,
                    opt => opt.MapFrom(src => src.Job.Id.ToString() + "-" + src.SuperService.SuperServiceName.Substring(0, 1).ToUpper() + src.SequenceNumber))
                .ForMember(d => d.SuperServiceId, opt => opt.MapFrom(src => src.SuperService.Id))
                .ForPath(d => d.ServiceOrders, opt => opt.MapFrom(src => src.ServiceOrder));

            CreateMap<PatchSuperServiceOrderRequest, SuperServiceOrder>()
                .ForMember(d => d.SuperServiceOrderStatusIdentifier, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();

            CreateMap<ServiceOrder, GetServiceOrderResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.Service))
                .ForMember(d => d.ServiceId, opt => opt.MapFrom(src => src.Service.Id))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(d => d.SortOrder, opt => opt.MapFrom(src => src.Service.SortOrder))
                .ForMember(d => d.ServiceAbbreviation, opt => opt.MapFrom(src => src.Service.ServiceAbbreviation));

            CreateMap<ServiceOrder, CreateServiceOrderResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.Service))
                .ForMember(d => d.ServiceId, opt => opt.MapFrom(src => src.Service.Id))
                .ForMember(d => d.ServiceName, opt => opt.MapFrom(src => src.Service.ServiceName))
                .ForMember(d => d.ServiceAbbreviation, opt => opt.MapFrom(src => src.Service.ServiceAbbreviation));

            CreateMap<JobSurveyInfo, GetSurveyInfoResponse>()
                .ForMember(d => d.RequestedPackStartDate, opt => opt.MapFrom(src => src.RequestedPackStartDateTime))
                .ForMember(d => d.RequestedPackEndDate, opt => opt.MapFrom(src => src.RequestedPackEndDateTime))
                .ReverseMap();

            CreateMap<JobSuperServiceAuthorization, GetSuperServiceAuthorizationResponse>()
               .ForMember(d => d.SuperServiceName, opt => opt.MapFrom(src => src.SuperService.SuperServiceName)).ReverseMap();

            CreateMap<Job, GetJobsSearchIndexResponse>()
               .ForMember(d => d.JobId, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.MasterAwbNumber, opt => opt.MapFrom(src => DtoTranslations.GetMasterAwbNumber(src.ServiceOrder)))
               .ForMember(d => d.MasterBolNumber, opt => opt.MapFrom(src => DtoTranslations.GetMasterBolNumber(src.ServiceOrder)))
               .ForMember(d => d.ContainerNumbers, opt => opt.MapFrom(src => DtoTranslations.GetContainers(src.ServiceOrder)))
               .ForMember(d => d.InvoiceDocuments, opt => opt.MapFrom(src => DtoTranslations.GetSearchInvoices(src.SuperServiceOrder)))
               .ForMember(d => d.TransfereeFirstName, opt => opt.MapFrom(src => src.Transferee.FirstName))
               .ForMember(d => d.TransfereeLastName, opt => opt.MapFrom(src => src.Transferee.LastName))
               ;
        }
    }
}