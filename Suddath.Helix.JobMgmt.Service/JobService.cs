using AutoMapper;
using Helix.API.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Suddath.Helix.Common.Extensions;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.Common.Infrastructure.EventBus.Interfaces;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Infrastructure.EventLog;
using Suddath.Helix.JobMgmt.Infrastructure.Mapper;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ServiceOrder;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using Suddath.Helix.JobMgmt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services
{
    public class JobService : BaseService, IJobService
    {
        private readonly JobDbContext _dbContext;
        private readonly IMoveStrategy _moveStrategy;
        private readonly IServiceOrderService _serviceOrderService;
        private readonly IHomeFrontEventService _homeFrontEventService;
        private readonly IMapper _mapper;
        private readonly ILogger<JobService> _logger;
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;

        public JobService(
            IMoveStrategy moveStrategy
            , JobDbContext dbContext
            , IMapper mapper
            , ILogger<JobService> logger
            , IHttpContextAccessor httpContextAccessor
            , IServiceOrderService serviceOrderService
            , IHomeFrontEventService homeFrontEventService
            , IEventBus eventBus
            , IIntegrationEventLogService eventLogService) : base(dbContext, mapper, httpContextAccessor)
        {
            _moveStrategy = moveStrategy ?? throw new ArgumentNullException("moveStrategy");
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _serviceOrderService = serviceOrderService ?? throw new ArgumentNullException("serviceOrderService");
            _eventLogService = eventLogService ?? throw new ArgumentNullException("eventLogService");
            _homeFrontEventService = homeFrontEventService ?? throw new ArgumentNullException("homeFrontEventService");
        }

        public async Task<TransfereeFlatDto> GetTransfereeByJobId(int jobId)
        {
            var query = await _dbContext.Job.AsNoTracking()
                .Include(s => s.Transferee)
                .ThenInclude(s => s.TransfereePhone)
                .ThenInclude(s => s.Phone)
                .Include(s => s.Transferee.Email)
                .Include(s => s.DestinationAddress)
                .Include(s => s.OriginAddress)
                .FirstOrDefaultAsync(x => x.Id == jobId);

            return query.ToTransfereeResponse<TransfereeFlatDto>();
        }

        public async Task<GetTaskOrderMemberInfoResponse> GetTaskOrderTransfereeByJobId(int jobId)
        {
            var query = await _dbContext.Job.AsNoTracking()
                .Include(s => s.TaskOrder.OriginDutyStationAddress)
                .Include(s => s.TaskOrder.DestinationDutyStationAddress)
                .Include(s => s.Transferee)
                .ThenInclude(s => s.TransfereePhone)
                .ThenInclude(s => s.Phone)
                .Include(s => s.Transferee.Email)
                .Include(s => s.DestinationAddress)
                .Include(s => s.OriginAddress)
                .FirstOrDefaultAsync(x => x.Id == jobId);

            return query.ToTaskOrderMemberInfoResponse<GetTaskOrderMemberInfoResponse>();
        }

        public async Task<Job2Dto> GetJobDetailById(int jobId)
        {
            var query = await _dbContext.Job.AsNoTracking()
                            .Include(s => s.Transferee)
                            .ThenInclude(s => s.TransfereePhone)
                            .ThenInclude(s => s.Phone)
                            .Include(s => s.Transferee.Email)
                            .Include(s => s.DestinationAddress)
                            .Include(s => s.OriginAddress)
                            .Include(s => s.Booker)
                            .Include(s => s.AccountEntity)
                            .Include(s => s.JobStatusNavigation)
                            .Include(s => s.BranchNameNavigation)
                            .FirstOrDefaultAsync(x => x.Id == jobId);

            if (query == null)
                return null;

            if (string.Compare(query.BillToType, "Account", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                query.BillToLabel = _dbContext.AccountEntity.FirstOrDefault(a => a.Id == query.BillToId).Name;
            }
            else
            {
                query.BillToLabel = _dbContext.Vendor.FirstOrDefault(a => a.Id == query.BillToId).Name;
            }

            return query.ToJobDto<Job2Dto>();
        }

        public async Task<ICollection<TransfereePortalDto>> GetJobsByAssigneeEmailAsync(string email)
        {
            var jobs = new List<TransfereePortalDto>();

            return jobs;
        }

        public async Task<int> AddJobAsync(Job2Dto job2Dto)
        {
            _logger.LogInformation("Incoming Dto {0}", job2Dto);
            string currentUsername = GetCurrentUserEmail();

            var job = _mapper.Map<Job>(job2Dto.Job);

            if (job != null)
            {
                JobCreatedIntegrationEvent jobCreatedEvent = null;

                await ResilientTransaction.New(_dbContext).ExecuteAsync(async () =>
                {
                    job.TransfereeId = SaveTransferee(job2Dto.Transferee);
                    job.AccountEntity = _dbContext.AccountEntity.FirstOrDefault(a => a.Id == job.AccountEntityId);

                    var dateTimeAdded = DateTime.UtcNow;
                    job.DateCreated = dateTimeAdded;
                    job.DateModified = dateTimeAdded;
                    job.CreatedBy = GetCurrentUserEmail();
                    job.ModifiedBy = GetCurrentUserEmail();

                    if (job2Dto.Job.IsSurveyAndBid)
                    {
                        job.JobStatus = JobStatusIdentifier.SURVEY_BID;
                    }

                    _dbContext.Job.Attach(job);
                    _dbContext.Entry(job).State = EntityState.Added;

                    await _dbContext.SaveChangesAsync();

                    //update the fks in the job table
                    var kvp = await SaveAddresses(job2Dto.JobInfo);
                    job.OriginAddressId = kvp.FirstOrDefault(s => s.Key == AddressType.ORIGIN).Value;
                    job.DestinationAddressId = kvp.FirstOrDefault(s => s.Key == AddressType.DESTINATION).Value;

                    await _dbContext.SaveChangesAsync();

                    jobCreatedEvent = job.FromJob<JobCreatedIntegrationEvent>();
                    jobCreatedEvent.Transferee = job2Dto.ToTransfereeIntegrationEvent<TransfereeCreatedIntegrationEvent>();
                    jobCreatedEvent.UserId = GetJobsMoveConsultantEmail(job.Id);

                    await _eventLogService.SaveEventAsync(jobCreatedEvent);
                });

                // Publish the integration event through the event bus
                _eventBus.Publish(jobCreatedEvent);
                await _eventLogService.MarkEventAsPublishedAsync(jobCreatedEvent.Id);

                _logger.LogInformation("Job added {0}", job.Id);
            }

            return job.Id;
        }

        public async Task UpdateJobAsync(Job2Dto dto)
        {
            var jobEntity = _dbContext.Job.FirstOrDefault(x => x.Id == dto.Job.JobId);

            _mapper.Map(dto.Job, jobEntity);
            jobEntity.DateModified = DateTime.UtcNow;
            jobEntity.ModifiedBy = GetCurrentUserEmail();

            JobCreatedIntegrationEvent jobUpdatedEvent = null;

            //TODO: Verify when we change the BillTo from Vendor to Account.. bill to Type is being changed.
            await ResilientTransaction.New(_dbContext).ExecuteAsync(async () =>
            {
                _dbContext.Job.Update(jobEntity);
                await _dbContext.SaveChangesAsync();

                UpdateTransferee(dto);
                UpdatePhone(dto);
                UpdateTransfereeEmail(dto);
                UpdateAddress(dto);

                jobUpdatedEvent = jobEntity.FromJob<JobCreatedIntegrationEvent>();
                jobUpdatedEvent.UserId = GetCurrentUserEmail();

                await _eventLogService.SaveEventAsync(jobUpdatedEvent);
            });

            // Publish the integration event through the event bus
            _eventBus.Publish(jobUpdatedEvent);
            await _eventLogService.MarkEventAsPublishedAsync(jobUpdatedEvent.Id);

            _logger.LogInformation("Job object updated");
        }

        public async Task<string> ValidateUpdateJobPatchAsync(int jobId, JsonPatchDocument patch)
        {
            _logger.LogInformation($"Validating incoming patch for JobID: {jobId} :: patchJson = {patch}");

            var job = await _dbContext.Job.FindAsync(jobId);
            var validationMessage = string.Empty;
            var jobdto = new JobDto();

            if (job.AccrualStatus == AccrualStatus.POSTED)
            {
                if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(jobdto.BranchName).ToLower())))
                {
                    validationMessage = "Job has accrued a service, Branch Name cannot be updated";
                }

                if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(jobdto.RevenueType).ToLower())))
                {
                    validationMessage = "Job has accrued a service, RevenueType cannot be updated";
                }

                var jobStatusOperation = patch.Operations.FirstOrDefault(x => x.path.ToLower().Contains(nameof(jobdto.Status).ToLower()));

                if (jobStatusOperation != null && ((string)jobStatusOperation.value).ToUpper() == JobStatusIdentifier.CANCELLED)
                {
                    validationMessage = "Job has accrued a service, Job cannot be cancelled";
                }
            }

            return validationMessage;
        }

        public async Task PatchJobAsync(int jobId, JsonPatchDocument patch)
        {
            _logger.LogInformation("Incoming jobId = {0}, patchJson = {1}", jobId, JsonConvert.SerializeObject(patch));

            var tempJobDto = await GetJobDetailById(jobId);

            patch.ApplyTo(tempJobDto);

            _logger.LogInformation("Starting to Update");

            await UpdateJobAsync(tempJobDto);

            _logger.LogInformation("Patch succeeded: jobId = {0}, patchJson = {1}", jobId, JsonConvert.SerializeObject(patch));
        }

        public async Task<PagedResults<GetJobsResponse>> ListJobsAsync(
            int pageNumber,
            int pageSize,
            string filter,
            string sortBy,
            bool descending,
            bool showAll)
        {
            var jobs = await ListJobsUnpagedAsync(filter, showAll, false);
            var mappedResults = _mapper.Map<ICollection<Job>, ICollection<GetJobsResponse>>(jobs).AsQueryable();

            sortBy = sortBy.FirstCharToUpper();

            mappedResults = descending
                ? mappedResults.OrderByDescending(x => GetPropertyValue(x, sortBy))
                : mappedResults.OrderBy(x => GetPropertyValue(x, sortBy));

            var pagedResults = mappedResults.PageQuery(pageSize, pageNumber).ToList();
            var jobIds = pagedResults.Select(pr => pr.JobId);

            var serviceIconQuery = _dbContext.SuperServiceOrder.AsNoTracking()
                                        .Include(sso => sso.SuperService)
                                        .Where(sso => sso.SuperServiceOrderStatusIdentifier != SuperServiceOrderStatusIdentifier.CANCELLED)
                                        .Where(sso => jobIds.Contains(sso.JobId))
                                        .ToList();

            foreach (var item in pagedResults)
            {
                item.SuperServiceIconNames = serviceIconQuery.Where(s => s.JobId == item.JobId).Select(sso => sso.SuperService.SuperServiceIconName).ToList();
            }

            var result = GetPagedList<GetJobsResponse>(mappedResults.Count(), pageNumber, pageSize);
            result.DataList = pagedResults;

            return result;
        }

        public async Task<ICollection<Job2Dto>> ListTaskOrdersAsync(
            string filter,
            string sortBy,
            bool descending,
            bool showAll)
        {
            var jobs = await ListJobsUnpagedAsync(filter, showAll, true);
            return _mapper.Map<List<Job>, List<Job2Dto>>(jobs.ToList());
        }

        private async Task<ICollection<Job>> ListJobsUnpagedAsync(
            string filter,
            bool showAll,
            bool isTaskOrder)
        {
            IQueryable<Job> query = _dbContext.Job.AsNoTracking()
                .Include(m => m.JobContact)
                .Include(m => m.AccountEntity)
                .Include(m => m.Transferee)
                .ThenInclude(t => t.TransfereePhone)
                .ThenInclude(tp => tp.Phone)
                .Include(m => m.OriginAddress)
                .Include(m => m.DestinationAddress)
                .Include(m => m.JobStatusNavigation)
                .Include(m => m.Transferee.Email)
                .Include(m => m.BranchNameNavigation);

            query = AddSearchFilterToQuery(query, filter);

            if (isTaskOrder)
            {
                query = query.Where(j => j.JobSource == JobSourceType.HOME_FRONT);
            }
            else
            {
                query = query.Where(j => j.JobSource != JobSourceType.HOME_FRONT);
            }

            if (!showAll)
            {
                query = query.Where(m => m.JobContact.Any(c => c.ContactType.ToUpper() == ConsultantType.MoveConsultant && c.Email == GetCurrentUserEmail()));
            }

            return query.ToList();
        }

        private IQueryable<Job> AddSearchFilterToQuery(IQueryable<Job> query, string filter)
        {
            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLowerClean();

                query = query.Where(j =>
                    j.Transferee.FirstName.ToLowerClean().Contains(filter)
                    || j.Transferee.LastName.ToLowerClean().Contains(filter)
                    || DtoTranslations.ToShortAddress(j.OriginAddress).ToLowerClean().Contains(filter)
                    || DtoTranslations.ToShortAddress(j.DestinationAddress).ToLowerClean().Contains(filter)
                    || j.JobStatusNavigation.JobStatusDisplayName.ToLowerClean().Contains(filter)
                    || j.Id.ToString().ToLowerClean().Contains(filter));
            }

            return query;
        }

        public async Task<bool> GetJobExistsAsync(int jobId)
        {
            bool exists = (await _dbContext.Job.SingleOrDefaultAsync(m => m.Id == jobId) == null) ? false : true;

            return exists;
        }

        public async Task<ICollection<GetSuperServiceOrderResponse>> GetSuperServiceOrdersAsync(int jobId)
        {
            _logger.LogInformation($"Retrieving Super Service orders for {jobId}");

            var dbSuperServiceOrders = await _dbContext.SuperServiceOrder.AsNoTracking()
                                        .Include(sso => sso.Job)
                                        .Include(sso => sso.SuperService)
                                        .Include(sso => sso.SuperServiceMode)
                                        .Include(sso => sso.SuperServiceOrderStatusIdentifierNavigation)
                                        .Include(sso => sso.ServiceOrder)
                                        .ThenInclude(so => so.Service)
                                        .Where(sso => sso.JobId == jobId)
                                        .ToListAsync();

            var superServiceOrders = _mapper.Map<List<GetSuperServiceOrderResponse>>(dbSuperServiceOrders);

            return superServiceOrders;
        }

        #region patch a job helpers

        private void UpdateTransferee(Job2Dto tempJobDto)
        {
            var transfereeEntity = _dbContext.Transferee.FirstOrDefault(x => x.Id == tempJobDto.Transferee.Id);

            _mapper.Map(tempJobDto.Transferee, transfereeEntity);

            _dbContext.Transferee.Update(transfereeEntity);
            _dbContext.SaveChanges();

            _logger.LogInformation("Transferee object updated");
        }

        private void UpdateTransfereeEmail(Job2Dto tempJobDto)
        {
            var emailEntityList = _dbContext.Job
                .Include(m => m.Transferee.Email)
                .FirstOrDefault(x => tempJobDto.Job.JobId == x.Id).Transferee.Email.ToList();

            tempJobDto.Transferee.Emails.ToList().ForEach(e => e.TransfereeId = tempJobDto.Transferee.Id);

            //TODO: I got the update and Add working on the list by marking the entity list retrieval as no tracking
            // and then later adding the list after automapper directly to the context.  But Delete isn't working
            //_mapper.Map(tempJobDto.Transferee.Emails, emailEntityList);

            var entityToRemove = new List<Email>();
            foreach (var entity in emailEntityList)
            {
                var dto = tempJobDto.Transferee.Emails.FirstOrDefault(e => e.Id == entity.Id);

                if (dto == null)
                {
                    //we need to delete the record because it was deleted from the dto
                    entityToRemove.Add(entity);
                    continue;
                }

                entity.Value = dto.Value;
                entity.Type = dto.EmailType;
            }

            //check if any emails need to be added
            foreach (var emailToBeAdded in tempJobDto.Transferee.Emails.ToList().Where(e => e.Id == 0))
            {
                emailEntityList.Add(new Email
                {
                    TransfereeId = tempJobDto.Transferee.Id.Value,
                    Type = emailToBeAdded.EmailType,
                    Value = emailToBeAdded.Value
                });
            }

            emailEntityList.RemoveAll(e => entityToRemove.Select(er => er.Id).Contains(e.Id));

            _dbContext.Email.UpdateRange(emailEntityList);
            _dbContext.Email.RemoveRange(entityToRemove);
            _dbContext.SaveChanges();

            _logger.LogInformation("Email object updated");
        }

        private void UpdatePhone(Job2Dto tempJobDto)
        {
            //TODO: this made it work. make it better next
            var phoneDtoList = tempJobDto.Transferee.OriginPhones.ToList();
            phoneDtoList.ForEach(p => p.LocationType = "Origin");
            var destPhoneList = tempJobDto.Transferee.DestinationPhones.ToList();
            destPhoneList.ForEach(p => p.LocationType = "Destination");
            phoneDtoList.AddRange(destPhoneList);

            //Get the list of all Phone Entities and Transferee Phone Entities
            var transfereePhones = _dbContext.Job
                .Include(x => x.Transferee.TransfereePhone)
                .ThenInclude(x => x.Phone)
                .FirstOrDefault(x => x.Id == tempJobDto.Job.JobId).Transferee.TransfereePhone.ToList();

            var phoneEntityList = new List<Phone>();
            transfereePhones.ForEach(tp => phoneEntityList.Add(tp.Phone));

            //TODO: Regular mapping from automapper didn't work. It would constantly break when saving.
            //_mapper.Map(tempJobDto.Transferee.OriginPhones, phoneEntityList);

            var entityToRemove = new List<Phone>();
            foreach (var entity in phoneEntityList)
            {
                var phoneDto = phoneDtoList.FirstOrDefault(p => p.Id == entity.Id);

                if (phoneDto == null)
                {
                    //we need to delete the record because it was deleted from the dto
                    entityToRemove.Add(entity);
                    continue;
                }

                entity.Extension = phoneDto.Extension;
                entity.CountryCode = phoneDto.CountryCode;
                entity.Primary = phoneDto.Primary;
                entity.Type = phoneDto.PhoneType;
                entity.NationalNumber = phoneDto.NationalNumber;
                entity.DialCode = phoneDto.DialCode;

                foreach (var transfereePhone in entity.TransfereePhone.Where(tp => tp.TransfereeId == tempJobDto.Transferee.Id))
                {
                    transfereePhone.Type = phoneDto.LocationType;
                }
            }

            //Check to see if anything needs to be added
            foreach (var phoneToBeAdded in phoneDtoList.Where(p => p.Id == 0))
            {
                phoneEntityList.Add(new Phone
                {
                    CountryCode = phoneToBeAdded.CountryCode,
                    Extension = phoneToBeAdded.Extension,
                    Primary = phoneToBeAdded.Primary,
                    Type = phoneToBeAdded.PhoneType,
                    NationalNumber = phoneToBeAdded.NationalNumber,
                    TransfereePhone = new List<TransfereePhone> { new TransfereePhone
                    {
                        TransfereeId = tempJobDto.Transferee.Id.Value,
                        Type = phoneToBeAdded.LocationType
                    } }
                });
            }

            phoneEntityList.RemoveAll(e => entityToRemove.Select(er => er.Id).Contains(e.Id));
            _dbContext.Phone.UpdateRange(phoneEntityList);

            List<TransfereePhone> transfereePhonesToRemove = new List<TransfereePhone>();
            foreach (var phoneEntity in entityToRemove)
            {
                transfereePhonesToRemove.AddRange(phoneEntity.TransfereePhone.ToList());
            }

            _dbContext.TransfereePhone.RemoveRange(transfereePhonesToRemove);
            _dbContext.Phone.RemoveRange(entityToRemove);
            _dbContext.SaveChanges();

            _logger.LogInformation("Phone and TransfereePhone objects updated");
        }

        private void UpdateAddress(Job2Dto tempJobDto)
        {
            var addressEntityList = _dbContext.Address
                .Where(x => tempJobDto.JobInfo.Addresses.Select(a => a.Id).Contains(x.Id)).ToList();

            //TODO: Regular mapping from automapper didn't work. It would constantly break when saving.
            //_mapper.Map(tempJobDto.MoveInfo.Addresses, addressEntityList);

            AddressDto addressDto = null;
            foreach (var entity in addressEntityList)
            {
                addressDto = tempJobDto.JobInfo.Addresses.FirstOrDefault(p => p.Id == entity.Id);

                entity.Address1 = addressDto.Address1;
                entity.Address2 = addressDto.Address2;
                entity.Address3 = addressDto.Address3;
                entity.AdditionalAddressInfo = addressDto.AdditionalAddressInfo;
                entity.City = addressDto.City;
                entity.Country = addressDto.Country;
                entity.Display = addressDto.Display;
                entity.Latitude = addressDto.Latitude;
                entity.Longitude = addressDto.Longitude;
                entity.PostalCode = addressDto.PostalCode;
                entity.State = addressDto.State;
                entity.Type = addressDto.Type;
                entity.AdditionalAddressInfo = (entity.Type == "Origin") ? tempJobDto.JobInfo.OriginAddressAdditionalInfo : (entity.Type == "Destination") ? tempJobDto.JobInfo.DestinationAddressAdditionalInfo : null;
            }

            _dbContext.Address.UpdateRange(addressEntityList);
            _dbContext.SaveChanges();

            _logger.LogInformation("Address updated");
        }

        #endregion patch a job helpers

        #region privates

        private async Task<IEnumerable<KeyValuePair<string, int>>> SaveAddresses(JobInfoDto jobInfo)
        {
            var insertedIds = new List<KeyValuePair<string, int>>();
            //We are strictly expecting only 2 address right now.  Origin and Destination.
            _logger.LogInformation("Saving Transferee Address {0}", jobInfo.Addresses);

            var addresses = _mapper.Map<List<Address>>(jobInfo.Addresses);
            string addressType = string.Empty;

            _dbContext.Address.AddRange(addresses);

            foreach (var address in addresses)
            {
                switch (address.Type)
                {
                    case "Origin":
                        address.AdditionalAddressInfo = jobInfo.OriginAddressAdditionalInfo;
                        address.Display = jobInfo.OriginAddressLabel;
                        break;

                    case "Destination":
                        address.AdditionalAddressInfo = jobInfo.DestinationAddressAdditionalInfo;
                        address.Display = jobInfo.DestinationAddressLabel;
                        break;

                    default:
                        break;
                }
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Address saved and associated to the job");

            return new[] {
                  new KeyValuePair<string,int>("Origin",addresses.FirstOrDefault(s=>s.Type == "Origin").Id),
                  new KeyValuePair<string,int>("Destination",addresses.FirstOrDefault(s=>s.Type == "Destination").Id)
                };
        }

        public async Task<int> SaveAddressAsync(AddressDto address, string type)
        {
            var newAddress = _mapper.Map<Address>(address);
            newAddress.Type = type;

            _dbContext.Address.Add(newAddress);

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(newAddress.Id);
        }

        private int SaveTransferee(TransfereeDto transfereeDto)
        {
            if (transfereeDto == null)
            {
                throw new ArgumentNullException("Transferee is null");
            }

            _logger.LogInformation("Saving Transferee {0}", transfereeDto);

            var transferee = _mapper.Map<Transferee>(transfereeDto);

            List<Phone> originValidPhones = new List<Phone>();
            if (transfereeDto.OriginPhones != null)
            {
                originValidPhones.AddRange(_mapper.Map<List<Phone>>(transfereeDto.OriginPhones
                    .Where(p => ValidatePhoneDto(p)).ToList()));
            }

            List<Phone> destinationValidPhones = new List<Phone>();
            if (transfereeDto.DestinationPhones != null)
            {
                destinationValidPhones.AddRange(_mapper.Map<List<Phone>>(transfereeDto.DestinationPhones
                    .Where(p => ValidatePhoneDto(p)).ToList()));
            }

            //saves the parent AND email objects in one Save, vs how we do with Addresses which is manual af b/c we are using Lookup tables (xref)
            _dbContext.Transferee.Add(transferee);
            _dbContext.SaveChanges();

            _dbContext.Phone.AddRange(originValidPhones);
            _dbContext.Phone.AddRange(destinationValidPhones);
            _dbContext.SaveChanges();

            foreach (var phone in originValidPhones)
            {
                _dbContext.TransfereePhone.Add(
                        new TransfereePhone
                        {
                            TransfereeId = transferee.Id,
                            PhoneId = phone.Id,
                            Type = "Origin"
                        }
                    );
            }
            foreach (var phone in destinationValidPhones)
            {
                _dbContext.TransfereePhone.Add(
                        new TransfereePhone
                        {
                            TransfereeId = transferee.Id,
                            PhoneId = phone.Id,
                            Type = "Destination"
                        }
                    );
            }

            _logger.LogInformation("Origin Phones saved {0}", originValidPhones);
            _logger.LogInformation("Destination Phones saved {0}", destinationValidPhones);

            List<EmailDto> validEmails = transfereeDto.Emails.Where(e => ValidateEmailDto(e)).ToList();

            var emails = _mapper.Map<List<Email>>(validEmails);
            emails.ForEach(e => e.TransfereeId = transferee.Id);

            _dbContext.Email.AddRange(emails);
            _dbContext.SaveChanges();

            _logger.LogInformation("Emails saved {0}", validEmails);
            _logger.LogInformation("Transferee saved {0}", transferee.Id);

            return transferee.Id;
        }

        /// <summary>
        /// Maps the objects sent in to a new integration event of the type passed.
        /// This will create and map the new integration event and then publish it to Service Bus.
        /// </summary>
        /// <returns></returns>
        private async Task MapAndPublishIntegrationEventAsync<T, U>(T entity) where U : IntegrationEvent
        {
            var integrationEvent = _mapper.Map<U>(entity);
            await _eventLogService.SaveEventAsync(integrationEvent);

            _eventBus.Publish(integrationEvent);
            await _eventLogService.MarkEventAsPublishedAsync(integrationEvent.Id);
        }

        private async Task PublishCoordinatorAssignedEventAsync(IEnumerable<JobContactDto> jobContactDtos)
        {
            var moveConsultant = jobContactDtos.FirstOrDefault(jc => jc.ContactType.ToUpper() == ConsultantType.MoveConsultant);
            if (moveConsultant != null)
            {
                var taskOrder = _dbContext.TaskOrder.FirstOrDefault(to => to.JobId == moveConsultant.JobId);
                if (taskOrder != null)
                {
                    await _homeFrontEventService.PublishCoordinatorAssignedEventAsync(moveConsultant.Id);
                }
            }
        }

        #endregion privates

        #region validations

        public bool ValidatePhoneDto(PhoneDto phoneDto)
        {
            if (string.IsNullOrEmpty(phoneDto.NationalNumber))
            {
                return false;
            }

            return true;
        }

        public bool ValidateEmailDto(EmailDto emailDto)
        {
            if (string.IsNullOrEmpty(emailDto.Value))
            {
                return false;
            }

            return Regex.IsMatch(emailDto.Value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        #endregion validations

        public async Task<int> CreateSuperServiceOrderAsync(int jobId, int superServiceId, int? superServiceModeId)
        {
            DateTime dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            var transfereeId = _dbContext.Job.FirstOrDefault(j => j.Id == jobId).TransfereeId;
            var sequenceNumbers = await _dbContext.SuperServiceOrder
                .Include(so => so.Job)
                .Where(so => so.Job.Id == jobId).Select(so => so.SequenceNumber).ToListAsync();
            int nextInSequence = sequenceNumbers.Count == 0 ? 1 : sequenceNumbers.Max() + 1;

            var newSuperServiceOrder = new SuperServiceOrder
            {
                JobId = jobId,
                SuperServiceId = superServiceId,
                TransfereeId = transfereeId,
                SequenceNumber = nextInSequence,
                DisplayId = string.Concat(jobId.ToString(), '-', nextInSequence.ToString()),
                SuperServiceOrderStatusIdentifier = SuperServiceOrderStatusIdentifier.ACTIVE,
                SuperServiceModeId = superServiceModeId,
                DateCreated = dateStamp,
                DateModified = dateStamp,
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
            };

            var dbSuperServiceOrder = _dbContext.SuperServiceOrder.Add(newSuperServiceOrder);
            await _dbContext.SaveChangesAsync();

            return newSuperServiceOrder.Id;
        }

        public async Task<ICollection<CreateServiceOrderResponse>> CreateTemplateServiceOrdersAsync(int superServiceOrderId)
        {
            if (superServiceOrderId <= 0)
            {
                throw new ArgumentException($"Cannot create Service Orders for invalid SuperServiceOrderId: {superServiceOrderId}");
            }

            var superServiceOrder = _dbContext.SuperServiceOrder.FirstOrDefault(sso => sso.Id == superServiceOrderId);

            if (superServiceOrder == null)
            {
                throw new ArgumentException($"Cannot create Service Orders for SuperServiceOrder that does not exist SuperServiceOrderId: {superServiceOrderId}");
            }

            List<int> serviceIdsToCreate = await _dbContext.Service
                                    .Where(s => s.SuperServiceId == superServiceOrder.SuperServiceId)
                                    .Select(s => s.Id)
                                    .ToListAsync();

            var dateStamp = DateTime.UtcNow;

            foreach (var serviceId in serviceIdsToCreate)
            {
                var newServiceOrder = _dbContext.ServiceOrder.Add(new ServiceOrder
                {
                    JobId = superServiceOrder.JobId,
                    ServiceId = serviceId,
                    ServiceOrderStatusIdentifier = ServiceOrderStatusIdentifier.QUEUED,
                    SuperServiceOrderId = superServiceOrder.Id,
                    TransfereeId = superServiceOrder.TransfereeId,
                    DateCreated = dateStamp,
                    DateModified = dateStamp,
                });
            }
            await _dbContext.SaveChangesAsync();

            var dbServiceOrders = await _dbContext.ServiceOrder
                .Include(so => so.Service)
                .Where(x => x.SuperServiceOrderId == superServiceOrderId)
                .OrderBy(x => x.Service.SortOrder).ToListAsync();

            foreach (var serviceOrder in dbServiceOrders)
            {
                await _serviceOrderService.CreateAppropriateServiceOrderSubTable(serviceOrder.Id, serviceOrder.Service.ServiceAbbreviation);

                if (serviceOrder.Service.ServiceAbbreviation == ServiceAbbreviation.INSURANCE_CLAIMS)
                {
                    await _serviceOrderService.SetServiceOrderInsuranceVendorDefaultAsync(serviceOrder.Id);
                }
            }

            return _mapper.Map<ICollection<CreateServiceOrderResponse>>(dbServiceOrders);
        }

        public async Task<CreateSuperServiceOrderResponse> GetSuperServiceOrderByIdAsync(int superServiceOrderId)
        {
            var dbSuperServiceOrder = await _dbContext.SuperServiceOrder
                .Include(sso => sso.SuperService)
                .Include(sso => sso.SuperServiceMode)
                .Where(sso => sso.Id == superServiceOrderId).FirstOrDefaultAsync();

            var superServiceOrder = _mapper.Map<CreateSuperServiceOrderResponse>(dbSuperServiceOrder);

            return superServiceOrder;
        }

        private TransfereePortalDto CreateEmptyLegacyJob()
        {
            return new TransfereePortalDto
            {
                Name = $"Move Job",
                Services = new List<ServiceDto>()
            };
        }

        private int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }

        private List<NewMoves> GetLegacyMovesByEmail(string email, string[] systemArray)
        {
            List<NewMoves> lstNewMoves = _dbContext.NewMoves
                                       .Where(a => a.Email == email.Trim())
                                       .Where(a => systemArray.Contains(a.System))
                                       .AsNoTracking().OrderBy(a => a.System)
                .ThenByDescending(n => n.SystemDate)
                .ThenBy(n => n.GroupCode)
            .ToList();

            return lstNewMoves;
        }

        #region Job Contacts

        public async Task<ICollection<JobContactDto>> CreateJobContactsAsync(int jobId, ICollection<CreateJobContactDto> contacts)
        {
            //This routine upserts contacts based on contacttype; assumption there can only be one contact type per job for now
            if (contacts == null || contacts.Count == 0)
            {
                throw new ArgumentNullException("contacts");
            }

            var job = await _dbContext.Job.Include(i => i.JobContact).FirstOrDefaultAsync(x => x.Id == jobId);

            if (job == null)
            {
                throw new ArgumentException($"Job {jobId} does not exist");
            }

            bool moveConultantUpdated = false;
            foreach (var contact in contacts)
            {
                var existingContactType = job.JobContact.SingleOrDefault(c => c.ContactType == contact.ContactType);
                if (existingContactType == null)
                {
                    job.JobContact.Add(contact.ToNewEntity());
                }
                else //update
                {
                    _dbContext.Entry(existingContactType).CurrentValues.SetValues(contact);
                }

                if (contact.ContactType.ToUpper() == ConsultantType.MoveConsultant)
                {
                    moveConultantUpdated = true;
                }
            }

            await _dbContext.SaveChangesAsync();

            var jobContacts = job.JobContact.Select(e => e.ToModel()).ToList();

            if (moveConultantUpdated)
            {
                await PublishCoordinatorAssignedEventAsync(jobContacts);
            }

            return jobContacts;
        }

        public async Task<ICollection<JobContactDto>> GetJobContactsAsync(int jobId)
        {
            var job = await _dbContext.Job.Include(i => i.JobContact).FirstOrDefaultAsync(x => x.Id == jobId);

            if (job == null)
            {
                throw new ArgumentException($"Job {jobId} does not exist");
            }

            return job.JobContact.Select(e => e.ToModel()).ToList();
        }

        /// <summary>
        /// Patch the Job Contacts for a specific Job
        /// </summary>
        /// <param name="id">JobContactId</param>
        /// <param name="patch">Operations to Patch</param>
        /// <returns></returns>
        public async Task PatchJobContactsAsync(int id, JsonPatchDocument patch)
        {
            if (patch == null)
            {
                throw new ArgumentNullException("patch");
            }

            var entity = await Get<JobContact>(s => s.Id == id);

            if (entity == null)
            {
                throw new ArgumentNullException($"record not found by Id {id}");
            }

            var model = entity.ToModel();
            patch.ApplyTo(model);

            _mapper.Map(model, entity);

            // If the JobContact Entity Updated was the 'Move Consultant" then we should fire off a new integration event.
            if (string.Equals(entity.ContactType, ConsultantType.MoveConsultant, StringComparison.InvariantCultureIgnoreCase))
            {
                await MapAndPublishIntegrationEventAsync<JobContact, MoveContactUpdatedIntegrationEvent>(entity);
                await PublishCoordinatorAssignedEventAsync(new List<JobContactDto> { model });
            }

            await Save(entity);
        }

        public async Task UpdateJobContactsAsync(int jobId, ICollection<JobContactDto> contacts)
        {
            //This routine upserts contacts based on contacttype; assumption there can only be one contact type per job for now
            if (contacts == null || contacts.Count == 0)
            {
                throw new ArgumentNullException("contacts");
            }

            var job = await _dbContext.Job.Include(i => i.JobContact).FirstOrDefaultAsync(x => x.Id == jobId);

            if (job == null)
            {
                throw new ArgumentException($"Job {jobId} does not exist");
            }

            //Key off of contact type instead of ID
            foreach (var contact in contacts)
            {
                var existingContactType = job.JobContact.SingleOrDefault(c => c.ContactType == contact.ContactType);
                if (existingContactType == null)
                {
                    throw new ArgumentException($"JobContact {contact.ContactType} does not exist");
                }
                else //update
                {
                    // If the JobContact Entity Updated was the 'Move Consultant" then we should fire off a new integration event.
                    if (string.Equals(existingContactType.ContactType, ConsultantType.MoveConsultant, StringComparison.InvariantCultureIgnoreCase))
                    {
                        await MapAndPublishIntegrationEventAsync<JobContact, MoveContactUpdatedIntegrationEvent>(existingContactType);
                        await PublishCoordinatorAssignedEventAsync(new List<JobContactDto> { contact });
                    }

                    _dbContext.Entry(existingContactType).CurrentValues.SetValues(contact);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        #endregion Job Contacts

        #region Search Indexer

        public async Task<PagedResults<GetJobsSearchIndexResponse>> GetJobsForSearchIndexAsync(int pageNumber = 1, int pageSize = 50, int lookbackMonths = 9)
        {
            if (lookbackMonths > 0)
            {
                lookbackMonths = -lookbackMonths;
            }
            //retrieves jobs for the last 9 months for indexing search
            var query = _dbContext.Job.AsNoTracking()
                            .Include(j => j.ServiceOrder)
                            .Include(m => m.Transferee)
                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(v => v.VendorInvoice)
                                    .ThenInclude(v => v.BillFromVendor)

                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(v => v.VendorInvoice)
                                    .ThenInclude(v => v.BillFromAccountEntity)

                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(v => v.VendorInvoice)
                                    .ThenInclude(v => v.BillFromTransferee)

                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(v => v.Invoice)
                                    .ThenInclude(v => v.BillToTransferee)

                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(v => v.Invoice)
                                    .ThenInclude(v => v.BillToVendor)

                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(v => v.Invoice)
                                    .ThenInclude(v => v.BillToAccountEntity)

                            .Include(j => j.ServiceOrder)
                                .ThenInclude(j => j.ServiceOrderOceanFreightContainer)
                            .Include(j => j.ServiceOrder)
                                .ThenInclude(j => j.ServiceOrderAirFreight)
                            .Include(j => j.ServiceOrder)
                                .ThenInclude(j => j.ServiceOrderOceanFreight)
                            .Include(j => j.SuperServiceOrder)
                                .ThenInclude(g => g.SuperService)
                            .Where(j => j.DateCreated > DateTime.UtcNow.AddMonths(lookbackMonths));

            var pagedResults = query.PageQuery(pageSize, pageNumber);

            var result = GetPagedList<GetJobsSearchIndexResponse>(query.Count(), pageNumber, pageSize);
            result.DataList = pagedResults.Select(j => j.FromJob<GetJobsSearchIndexResponse>()).ToList();

            return result;
        }

        #endregion Search Indexer
    }
}