using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Helix.API.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Suddath.Helix.Common.Extensions;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels;
using Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using Suddath.Helix.JobMgmt.Services.Interfaces;

namespace Suddath.Helix.JobMgmt.Services
{
    public class BillableItemService : BaseOrderService, IBillableItemService
    {
        private readonly JobDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BillableItemService> _logger;
        private readonly ISuperServiceOrderService _superServiceOrderService;
        private readonly IAccountingService _accountingService;

        public BillableItemService(
            ISuperServiceOrderService superServiceOrderService,
            IAccountingService accountingService,
            JobDbContext dbContext,
            IMapper mapper,
            ILogger<BillableItemService> logger,
            IHttpContextAccessor httpContextAccessor) : base(dbContext, mapper, httpContextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _superServiceOrderService = superServiceOrderService ?? throw new ArgumentNullException(nameof(superServiceOrderService));
            _accountingService = accountingService ?? throw new ArgumentNullException(nameof(accountingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region BillableItem CRUD

        public async Task<IEnumerable<GetBillableItemResponse>> GetBillableItemsAsync(int superServiceOrderId)
        {
            var results = await _dbContext.BillableItem
                .Where(bi => bi.SuperServiceOrderId == superServiceOrderId)
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Include(bi => bi.BillableItemType)
                .Include(bi => bi.Invoice)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetBillableItemResponse>>(results);
        }

        public async Task<GetBillableItemResponse> GetBillableItemByIdAsync(int id)
        {
            var result = await _dbContext.BillableItem
                .Where(bi => bi.Id == id)
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Include(bi => bi.BillableItemType)
                .Include(bi => bi.Invoice)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return _mapper.Map<GetBillableItemResponse>(result);
        }

        public async Task<List<GetBillableItemResponse>> GetBillableItemsByIdsAsync(IEnumerable<int> ids)
        {
            var result = await _dbContext.BillableItem
                .Where(bi => ids.Contains(bi.Id))
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Include(bi => bi.BillableItemType)
                .Include(bi => bi.Invoice)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<GetBillableItemResponse>>(result);
        }

        public async Task<CreateBillableItemResponse> CreateBillableItemAsync(int superServiceOrderId)
        {
            var superServiceOrderData = _dbContext.SuperServiceOrder.AsNoTracking()
                                        .Include(sso => sso.Job)
                                        .FirstOrDefault(sso => sso.Id == superServiceOrderId);

            var entity = new BillableItem
            {
                SuperServiceOrderId = superServiceOrderId,
                BillableItemStatusIdentifier = BillableItemStatusIdentifier.QUEUED,
                BillingCurrency = "USD",
                BillingCurrencySymbol = "$"
            };

            string billToType = superServiceOrderData.Job.BillToType;

            switch (billToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    entity.BillToType = EntityType.ACCOUNT_ENTITY;
                    entity.BillToAccountEntityId = superServiceOrderData.Job.BillToId;
                    break;

                case EntityType.VENDOR:
                    entity.BillToType = EntityType.VENDOR;
                    entity.BillToVendorId = superServiceOrderData.Job.BillToId;
                    break;

                case EntityType.TRANSFEREE:
                    entity.BillToType = EntityType.TRANSFEREE;
                    entity.BillToTransfereeId = superServiceOrderData.Job.BillToId;
                    break;
            }
            string username = GetCurrentUserEmail();
            DateTime dateStamp = DateTime.UtcNow;

            entity.DateCreated = dateStamp;
            entity.DateModified = dateStamp;
            entity.CreatedBy = username;
            entity.ModifiedBy = username;

            await Add(entity);
            var dbEntity = _dbContext.BillableItem
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .FirstOrDefault(bi => bi.Id == entity.Id);

            var response = _mapper.Map<CreateBillableItemResponse>(dbEntity);

            return response;
        }

        public async Task UpdateBillableItemAsync(int id, JsonPatchDocument patch)
        {
            _logger.LogInformation("Incoming BillableItemId = {0}, patchJson = {1}", id, JsonConvert.SerializeObject(patch));
            var currentUsername = GetCurrentUserEmail();
            var dateStamp = DateTime.UtcNow;

            var entity = _dbContext.BillableItem
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Include(bi => bi.BillableItemType)
                .FirstOrDefault(bi => bi.Id == id)
                ;

            if (patch.Operations.Any(x => x.path.ToLower().Contains("billtoid")))
            {
                entity.BillToAccountEntityId = null;
                entity.BillToVendorId = null;
                entity.BillToTransfereeId = null;
            }

            var current = _mapper.Map<GetBillableItemResponse>(entity);

            patch.ApplyTo(current);

            _logger.LogInformation("Starting to Update");

            _mapper.Map(current, entity);

            if (patch.Operations.Any(x => x.path.ToLower().StartsWith("/writeoff")))
            {
                entity.WriteOffModifiedBy = currentUsername;
                entity.WriteOffModifiedDateTime = dateStamp;
            }

            if (patch.Operations.Any(x => x.path.ToLower().StartsWith("/accrualexchangerate")))
            {
                entity.AccrualExchangeRateDateTime = dateStamp;
            }

            if (patch.Operations.Any(x => x.path.ToLower().StartsWith("/actualexchangerate")))
            {
                entity.ActualExchangeRateDateTime = dateStamp;
            }

            entity.ModifiedBy = currentUsername;
            entity.DateModified = dateStamp;

            await Save(entity);

            _logger.LogInformation("Patch succeeded: id = {0}, patchJson = {1}", id, JsonConvert.SerializeObject(patch));
        }

        public async Task<string> ValidateUpdateBillableItemAsync(int id, JsonPatchDocument patch)
        {
            var stringBuilder = new StringBuilder();

            var entity = await Get<BillableItem>(x => x.Id == id);
            var current = _mapper.Map<GetBillableItemResponse>(entity);

            //early returns for special cases
            if (
                (entity.BillableItemStatusIdentifier != BillableItemStatusIdentifier.ACTUAL_PENDING &&
                 entity.BillableItemStatusIdentifier != BillableItemStatusIdentifier.ACTUAL_POSTED &&
                 entity.BillableItemStatusIdentifier != BillableItemStatusIdentifier.PARTIAL_PAID) &&

                  (patch.Operations.All(x => x.path.ToLower().StartsWith("/actual")) &&
                    !patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.ActualExchangeRateDateTime).ToLower())))
               )
            {
                return stringBuilder.ToString();
            }
            if (
                (entity.BillableItemStatusIdentifier == BillableItemStatusIdentifier.QUEUED ||
                 entity.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_PENDING ||
                 entity.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_POSTED)
                 &&
                patch.Operations.All(x => x.path.ToLower().StartsWith("/billto"))
               )
            {
                //Patch BillTo when status is in anything but Actual_Pending, Actual_Posted or void
                return stringBuilder.ToString();
            }
            if (patch.Operations.All(x => x.path.ToLower() == "/description"))
            {
                return stringBuilder.ToString();
            }
            if (entity.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACTUAL_POSTED &&
               (
                patch.Operations.All(x => x.path.ToLower().StartsWith("/writeoff"))
               ))
            {
                return stringBuilder.ToString();
            }
            //validations
            if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.ActualExchangeRateDateTime).ToLower())) ||
                (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.AccrualExchangeRateDateTime).ToLower()))))
            {
                stringBuilder.AppendLine("Cannot update ExchangeRate DateTime");
            }
            if (entity.BillableItemStatusIdentifier != BillableItemStatusIdentifier.QUEUED)
            {
                stringBuilder.AppendLine("Cannot update BillableItems not in QUEUED Status");
            }
            if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.SuperServiceOrderId).ToLower())))
            {
                stringBuilder.AppendLine("SuperServiceOrderId cannot be updated");
            }
            if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.InvoiceId).ToLower())))
            {
                stringBuilder.AppendLine("InvoiceId cannot be updated");
            }
            if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.InvoiceNumber).ToLower()))
                || (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.InvoiceDate).ToLower()))))
            {
                stringBuilder.AppendLine("Invoice Number, Invoice Date cannot be updated");
            }
            if (patch.Operations.Any(x =>
            x.path.ToLower().Contains(nameof(current.BillToType).ToLower()))
             && !(patch.Operations.Any(y => y.path.ToLower().Contains(nameof(current.BillToId).ToLower())))
            )
            {
                stringBuilder.AppendLine("BillToType, BillToId must be updated together");
            }
            if (patch.Operations.Any(y => y.path.ToLower().Contains(nameof(current.BillToId).ToLower()))
                 && !(patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.BillToType).ToLower())))
            )
            {
                stringBuilder.AppendLine("BillToType, BillToId must be updated together");
            }
            if (
                (patch.Operations.Any(x => x.path.ToLower().Equals("/billingcurrency")) &&
                  !patch.Operations.Any(x => x.path.ToLower().Equals("/billingcurrencysymbol")))
                 ||
                (!patch.Operations.Any(x => x.path.ToLower().Equals("/billingcurrency")) &&
                  patch.Operations.Any(x => x.path.ToLower().Equals("/billingcurrencysymbol")))
               )
            {
                stringBuilder.AppendLine("Must update currency and symbol together");
            }

            return stringBuilder.ToString();
        }

        public async Task DeleteBillableItemAsync(int id)
        {
            var current = await Get<BillableItem>(bi => bi.Id == id);

            if (current != null)
            {
                await Remove(current);
            }

            return;
        }

        public async Task<string> ValidateDeleteBillableItemAsync(int id)
        {
            var stringBuilder = new StringBuilder();

            var entity = await Get<BillableItem>(x => x.Id == id);

            if (entity.BillableItemStatusIdentifier != BillableItemStatusIdentifier.QUEUED)
            {
                stringBuilder.AppendLine("Cannot delete BillableItems not in QUEUED Status");
            }

            return stringBuilder.ToString();
        }

        public async Task<bool> GetExistsBillableItemByTuple(int jobId, int superServiceOrderId, int id)
        {
            bool exists = (await Get<BillableItem>(bi => bi.Id == id && bi.SuperServiceOrderId == superServiceOrderId
                            && bi.SuperServiceOrder.JobId == jobId) == null) ? false : true;

            return await Task.FromResult(exists);
        }

        #endregion BillableItem CRUD

        public async Task<IEnumerable<GetBillableItemTypeResponse>> GetBillableItemTypesAsync()
        {
            var results = await _dbContext.BillableItemType
                            .Where(bit => bit.IsActive.Value)
                            .AsNoTracking()
                            .ToListAsync();

            return _mapper.Map<IEnumerable<GetBillableItemTypeResponse>>(results);
        }

        public async Task<IEnumerable<GetSuperServiceOrderAvailableBillTosResponse>> GetSuperServiceOrderAvailableBillTosAsync(int superServiceOrderId)
        {
            var mappedResults = new List<GetSuperServiceOrderAvailableBillTosResponse>();

            var serviceOrderVendors = await _dbContext.ServiceOrder
                .Include(so => so.Vendor)
                .Where(so => so.SuperServiceOrderId == superServiceOrderId && so.VendorId != null && so.Vendor.AccountingId != AccountingId.OVERSEAS_AGENT_VENDOR.ToString())
                .Select(so => so.Vendor)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            var jobAccountEntity = _dbContext.SuperServiceOrder
                .Include(sso => sso.Job.AccountEntity)
                .Where(sso => sso.Id == superServiceOrderId && sso.Job.AccountEntity.AccountingId != AccountingId.OVERSEAS_AGENT_ACCOUNT.ToString())
                .Select(sso => sso.Job.AccountEntity)
                .AsNoTracking()
                .ToList();

            var transferee = _dbContext.SuperServiceOrder
                .Where(sso => sso.Id == superServiceOrderId)
                .Include(sso => sso.Job)
                .ThenInclude(j => j.Transferee)
                .Select(sso => sso.Job.Transferee)
                .AsNoTracking()
                .FirstOrDefault();

            var job = _dbContext.SuperServiceOrder
                 .Include(sso => sso.Job)
                 .FirstOrDefault(sso => sso.Id == superServiceOrderId)
                 .Job;

            var jobAccountBillTo = new List<AccountEntity>();
            var jobVendorBillTo = new List<Vendor>();

            switch (job.BillToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    jobAccountBillTo = _dbContext.AccountEntity.Where(ae => ae.Id == job.BillToId && ae.AccountingId != AccountingId.OVERSEAS_AGENT_ACCOUNT.ToString()).ToList();
                    break;

                case EntityType.VENDOR:
                    jobVendorBillTo = _dbContext.Vendor.Where(v => v.Id == job.BillToId && v.AccountingId != AccountingId.OVERSEAS_AGENT_VENDOR.ToString()).ToList();
                    break;
            }

            var alreadyBilledEntities = _dbContext.BillableItem
                .Include(sso => sso.BillToAccountEntity)
                .Include(sso => sso.BillToVendor)
                .Include(sso => sso.BillToTransferee)
                .Where(bi => bi.SuperServiceOrderId == superServiceOrderId)
                .ToList();

            var alreadyBilledAccountEntites = alreadyBilledEntities
                .Where(bi => bi.BillToAccountEntityId != null)
                .Select(bi => bi.BillToAccountEntity)
                .Distinct()
                .ToList();

            var alreadyBilledVendors = alreadyBilledEntities
                .Where(bi => bi.BillToVendorId != null)
                .Select(bi => bi.BillToVendor)
                .Distinct()
                .ToList();

            mappedResults.AddRange(_mapper.Map<List<GetSuperServiceOrderAvailableBillTosResponse>>(
                jobAccountEntity
                .Union(alreadyBilledAccountEntites)
                .Union(jobAccountBillTo)
                .Distinct()
                ));

            mappedResults.AddRange(_mapper.Map<List<GetSuperServiceOrderAvailableBillTosResponse>>(
                serviceOrderVendors
                .Union(alreadyBilledVendors)
                .Union(jobVendorBillTo)
                .Distinct()
                ));

            mappedResults.Add(_mapper.Map<GetSuperServiceOrderAvailableBillTosResponse>(transferee));

            return mappedResults.Distinct().OrderBy(x => x.BillToType).ThenBy(x => x.Name);
        }

        #region Lock/Unlock Accruals

        public async Task<LockAccrualsResponse> LockAccrualsAsync(int superServiceOrderId)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            var items = await _dbContext.BillableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId && bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.QUEUED).ToListAsync();

            items.ForEach(item =>
            {
                item.BillableItemStatusIdentifier = BillableItemStatusIdentifier.ACCRUAL_PENDING;
                item.DateModified = dateStamp;
                item.ModifiedBy = currentUser;
            });

            var superServiceOrder = _dbContext.SuperServiceOrder.Include(sso => sso.Job)
                                                                .FirstOrDefault(x => x.Id == superServiceOrderId);

            superServiceOrder.AccrualPendingDateTime = dateStamp;
            superServiceOrder.AccrualStatus = AccrualStatus.PENDING;

            if (superServiceOrder.Job.AccrualStatus != AccrualStatus.POSTED)
            {
                superServiceOrder.Job.AccrualStatus = AccrualStatus.PENDING;
            }

            superServiceOrder.ModifiedBy = GetCurrentUserEmail();
            superServiceOrder.DateModified = dateStamp;

            await _dbContext.SaveChangesAsync();

            return new LockAccrualsResponse
            {
                RecordsUpdated = items.Count(),
                AccrualPendingDateTime = dateStamp,
            };
        }

        public async Task<UnlockAccrualsResponse> UnlockAccrualsAsync(int superServiceOrderId)
        {
            var items = await _dbContext.BillableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId && bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_PENDING).ToListAsync();

            items.ForEach(item => item.BillableItemStatusIdentifier = BillableItemStatusIdentifier.QUEUED);

            var superServiceOrder = _dbContext.SuperServiceOrder.Include(sso => sso.Job)
                                                                .FirstOrDefault(x => x.Id == superServiceOrderId);

            superServiceOrder.AccrualPendingDateTime = null;
            superServiceOrder.AccrualStatus = null;

            if (superServiceOrder.Job.AccrualStatus == AccrualStatus.PENDING)
            {
                superServiceOrder.Job.AccrualStatus = null;
            }

            await _dbContext.SaveChangesAsync();
            return new UnlockAccrualsResponse
            {
                RecordsUpdated = items.Count(),
            };
        }

        public async Task<string> ValidateUnlockAccrualsAsync(int superServiceOrderId)
        {
            var stringBuilder = new StringBuilder();

            var entity = await Get<SuperServiceOrder>(x => x.Id == superServiceOrderId);

            if (entity.AccrualPostedDateTime != null)
            {
                stringBuilder.AppendLine("Cannot unlock accruals for SuperServiceOrder that has already been posted");
            }

            return stringBuilder.ToString();
        }

        public async Task<string> ValidateLockAccrualsAsync(int superServiceOrderId)
        {
            var stringBuilder = new StringBuilder();
            decimal? netWeight = 0m;
            decimal? grossWeight = 0m;

            var entity = await _dbContext.SuperServiceOrder.Include(sso => sso.Job).FirstOrDefaultAsync(sso => sso.Id == superServiceOrderId);
            var billableItems = await _dbContext.BillableItem.Where(x => x.SuperServiceOrderId == superServiceOrderId).ToListAsync();

            switch (entity.SuperServiceId)
            {
                case SuperServiceId.AIR:
                    var airEntity = await GetAirFreightMetrics(superServiceOrderId);

                    netWeight = airEntity?.NetWeightLb;
                    grossWeight = airEntity?.GrossWeightLb;
                    break;

                case SuperServiceId.OCEAN:
                    var oceanFrieghtMetrics = await GetOceanFreightMetrics(superServiceOrderId);
                    netWeight = oceanFrieghtMetrics.NetWeightLb;
                    grossWeight = oceanFrieghtMetrics.GrossWeightLb;
                    break;

                case SuperServiceId.ROAD:
                    var roadEntity = await GetRoadFreightMetrics(superServiceOrderId);

                    netWeight = roadEntity?.NetWeightLb;
                    grossWeight = roadEntity?.GrossWeightLb;
                    break;

                case SuperServiceId.STORAGE:
                    var storageEntity = await GetStorageOAMetrics(superServiceOrderId);
                    netWeight = storageEntity?.NetWeightLb;
                    grossWeight = storageEntity?.GrossWeightLb;
                    break;
            }

            if (entity.AccrualPendingDateTime != null)
            {
                stringBuilder.AppendLine("Cannot lock accruals for SuperServiceOrder that has already been locked");
            }
            if (entity.AccrualPostedDateTime != null)
            {
                stringBuilder.AppendLine("Cannot lock accruals for SuperServiceOrder that has already been posted");
            }
            if (billableItems.Any(bi => bi.BillableItemTypeId == null))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null BillableItemType");
            }
            if (billableItems.Any(bi => string.IsNullOrWhiteSpace(bi.Description)))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null Description");
            }
            if (billableItems.Any(bi => bi.BillToAccountEntityId == null && bi.BillToVendorId == null && bi.BillToTransfereeId == null))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null BillToId");
            }
            if (billableItems.Any(bi => string.IsNullOrEmpty(bi.BillingCurrency)))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null BillingCurrency");
            }
            if (billableItems.Any(bi => bi.AccrualAmountUSD == null || bi.AccrualAmountUSD == 0))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null or zero amounts (USD)");
            }
            if (billableItems.Any(bi => bi.AccrualAmountBillingCurrency == null || bi.AccrualAmountBillingCurrency == 0))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null or zero amounts (BillingCurrency)");
            }
            if (
                entity.SuperServiceOrderStatusIdentifier != SuperServiceOrderStatusIdentifier.ACTIVE &&
                entity.SuperServiceOrderStatusIdentifier != SuperServiceOrderStatusIdentifier.SIT
               )
            {
                stringBuilder.AppendLine("SuperServiceOrder is not in Active or SIT status");
            }
            if (string.IsNullOrEmpty(entity.Job.AuthorizationPONumber))
            {
                stringBuilder.AppendLine("Associated Job doesn't have a AuthorizationPONumber");
            }
            if (netWeight.GetValueOrDefault() == 0 && grossWeight.GetValueOrDefault() == 0)
            {
                stringBuilder.AppendLine("Items being shipped must have Net or Gross weight");
            }

            return stringBuilder.ToString();
        }

        #endregion Lock/Unlock Accruals

        #region Accruables

        public async Task<IEnumerable<GetPostableResponse>> GetBillableItemAccruablesAsync(int superServiceOrderId, DateTime? actualPackEndDate)
        {
            var dbAccruables = await _dbContext.BillableItem
                .Include(bi => bi.SuperServiceOrder)
                .ThenInclude(sso => sso.Job)
                .Include(bi => bi.BillableItemType)
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Where(bi => bi.SuperServiceOrderId == superServiceOrderId && bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_PENDING)
                .AsNoTracking()
                .ToListAsync();

            var response = MapBillableItemsToAccruables(superServiceOrderId, dbAccruables, actualPackEndDate);

            return response;
        }

        public async Task<GetPostableResponse> GetBillableItemAccruableByIdAsync(int id, DateTime? actualPackEndDate)
        {
            var accruableBillableItem = await _dbContext.BillableItem
                .Include(bi => bi.SuperServiceOrder)
                .ThenInclude(sso => sso.Job)
                .Include(bi => bi.BillableItemType)
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Where(bi => bi.Id == id)
                .AsNoTracking()
                .ToListAsync();

            var response = MapBillableItemsToAccruables(accruableBillableItem.FirstOrDefault().SuperServiceOrderId, accruableBillableItem, actualPackEndDate);

            return response.FirstOrDefault();
        }

        public async Task<string> ValidateBillableItemVoidableByIdAsync(int id)
        {
            var stringBuilder = new StringBuilder();

            var billableItem = await _dbContext.BillableItem
                .Include(bi => bi.SuperServiceOrder)
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Where(bi => bi.Id == id)
                .FirstAsync();

            if (billableItem.BillableItemStatusIdentifier != BillableItemStatusIdentifier.ACCRUAL_POSTED)
            {
                stringBuilder.AppendLine($"Cannot void individual BillableItem in a status of {billableItem.BillableItemStatusIdentifier}");
            }

            return stringBuilder.ToString();
        }

        public async Task<string> ValidateBillableItemAccrualByIdAsync(int id)
        {
            var stringBuilder = new StringBuilder();

            var billableItem = await _dbContext.BillableItem
                .Include(bi => bi.SuperServiceOrder)
                .Where(bi => bi.Id == id)
                .FirstOrDefaultAsync();

            if (billableItem.BillableItemStatusIdentifier != BillableItemStatusIdentifier.QUEUED)
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual that is not in QUEUED status");
            }
            if (billableItem.SuperServiceOrder.AccrualPostedDateTime == null)
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual when SuperServiceOrder has not already been locked and accrued");
            }
            if (billableItem.BillableItemTypeId == null)
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual with null BillableItemType");
            }
            if (string.IsNullOrWhiteSpace(billableItem.Description))
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual with null Description");
            }
            if (billableItem.BillToAccountEntityId == null && billableItem.BillToVendorId == null && billableItem.BillToTransfereeId == null)
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual with null BillToId");
            }
            if (string.IsNullOrEmpty(billableItem.BillingCurrency))
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual with null BillingCurrency");
            }
            if (billableItem.AccrualAmountUSD == null || billableItem.AccrualAmountUSD == 0)
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual with null or zero amounts (USD)");
            }
            if (billableItem.AccrualAmountBillingCurrency == null || billableItem.AccrualAmountBillingCurrency == 0)
            {
                stringBuilder.AppendLine("Cannot post individual BillableItem accrual with null or zero amounts (BillingCurrency)");
            }

            return stringBuilder.ToString();
        }

        public async Task<PagedResults<GetAccruableBatchResponse>> GetAccruableBatches(string sortBy, int pageNumber = 1, int pageSize = 10, string filter = null, bool descending = false)
        {
            var batches = new List<GetAccruableBatchResponse>();

            var query = await _dbContext.SuperServiceOrder
                            .Include(s => s.ServiceOrder)
                            .ThenInclude(s => s.ServiceOrderMoveInfo)
                            .Include(s => s.Job)
                            .ThenInclude(s => s.JobContact)
                            .Include(s => s.BillableItem)
                            .ThenInclude(s => s.BillToAccountEntity)
                            .Include(s => s.PayableItem)
                            .Where(s =>
                                (
                                    s.AccrualPendingDateTime.HasValue
                                    && !s.AccrualPostedDateTime.HasValue
                                )).ToListAsync();

            sortBy = sortBy?.FirstCharToUpper();

            foreach (var item in query)
            {
                Task.WaitAll(
                    Task.Run(async () =>
                    {
                        var getMetrics = await _superServiceOrderService.GetJobCostMetricsAsync(item.Id);

                        RunCalcs(out decimal? totalAccruableRevenue, out decimal? totalAccruableProfit, out decimal? marginPercentage, item);

                        batches.Add(
                            new GetAccruableBatchResponse()
                            {
                                SuperServiceOrderId = item.Id,
                                SuperServiceOrderDisplayId = item.DisplayId,
                                JobId = item.JobId,
                                JobCostServiceOrderId = item.ServiceOrder.FirstOrDefault().Id,
                                AccrualPendingDateTime = item.AccrualPendingDateTime.GetValueOrDefault(),
                                ActualPackEndDate = item.ServiceOrder.FirstOrDefault().ServiceOrderMoveInfo?.ActualPackEndDate.GetValueOrDefault(),
                                BranchName = item.Job?.BranchName,
                                RevenueType = item.Job?.RevenueType,
                                MoveType = item.Job?.MoveType,
                                MoveConsultantName = item.Job?.JobContact?.FirstOrDefault()?.FullName,
                                AccountEntityName = item.Job?.AccountEntity?.Name,
                                NetWeightLb = getMetrics.NetWeightLb,
                                GrossWeightLb = getMetrics.GrossWeightLb,
                                TotalAccruableRevenue = totalAccruableRevenue,
                                TotalAccruableProfit = totalAccruableProfit,
                                MarginPercentage = marginPercentage
                            });
                    })
                );
            }

            var batchesToQueryable = batches.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.Format();

                batchesToQueryable = batchesToQueryable.Where(s => s.BranchName.Format().Contains(filter)
                                    || (EvalProperty(s.RevenueType)).Format().Contains(filter)
                                    || (EvalProperty(s.MoveConsultantName)).Format().Contains(filter)
                                    || (EvalProperty(s.AccountEntityName)).Format().Contains(filter));
            }

            sortBy = sortBy.FirstCharToUpper();

            batchesToQueryable = descending
                ? batchesToQueryable.OrderByDescending(x => GetPropertyValue(x, sortBy))
                : batchesToQueryable.OrderBy(x => GetPropertyValue(x, sortBy));

            var pagedResults = batchesToQueryable.PageQuery(pageSize, pageNumber);

            var result = GetPagedList<GetAccruableBatchResponse>(batchesToQueryable.Count(), pageNumber, pageSize);
            result.DataList = pagedResults.ToList();

            return result;
        }

        #endregion Accruables

        #region Adjustables

        public async Task<GetPostableResponse> GetBillableItemAccrualAdjustableByIdAsync(int id, DateTime? actualPackEndDate)
        {
            var adjustment = await _dbContext.BillableItemAdjustment.FirstOrDefaultAsync(bia => bia.Id == id);

            var adjustableBillableItem = await _dbContext.BillableItem
                .Include(bi => bi.SuperServiceOrder)
                .ThenInclude(sso => sso.Job)
                .Include(bi => bi.BillableItemType)
                .Include(bi => bi.BillToAccountEntity)
                .Include(bi => bi.BillToVendor)
                .Include(bi => bi.BillToTransferee)
                .Where(bi => bi.Id == adjustment.BillableItemId)
                .AsNoTracking()
                .ToListAsync();

            var response = MapBillableItemsToAccruables(adjustableBillableItem.FirstOrDefault().SuperServiceOrderId, adjustableBillableItem, actualPackEndDate);

            var biAdjustableResponse = response.FirstOrDefault();

            string newDocType;
            if (adjustment.NewAmountUSD >= 0)
            {
                newDocType = GPDocType.INVOICE.ToString();
            }
            else
            {
                newDocType = GPDocType.RECEIVABLE_CREDIT_MEMO.ToString();
            }

            biAdjustableResponse.Header.DocType = newDocType;
            biAdjustableResponse.Header.DocNum = adjustment.GPDocNum;
            biAdjustableResponse.Header.DocAmt = Math.Abs(adjustment.NewAmountUSD);

            biAdjustableResponse.LineItems.ForEach(lineItem =>
            {
                lineItem.DocType = newDocType;
                if (lineItem.DistType == GPDistType.RECEIVABLE)
                {
                    lineItem.DistAmt = adjustment.NewAmountUSD;
                    lineItem.DocNum = adjustment.GPDocNum;
                }
                else if (lineItem.DistType == GPDistType.SALES ||
                    lineItem.DistType == GPDistType.CREDIT_MEMO)
                {
                    lineItem.DistAmt = -1 * adjustment.NewAmountUSD;
                    lineItem.DocNum = adjustment.GPDocNum;
                    if (lineItem.DistAmt <= 0)
                    {
                        lineItem.DistType = GPDistType.SALES;
                    }
                    else
                    {
                        lineItem.DistType = GPDistType.CREDIT_MEMO;
                    };
                }
                else
                {
                    throw new Exception("GPDistType not supported");
                }
            });

            return biAdjustableResponse;
        }

        #endregion Adjustables

        #region Mark Accruals Status

        public async Task<PostedAccrualsResponse> MarkAccrualsPostedAsync(int superServiceOrderId, IEnumerable<int> BillableItemIds, DateTime accrualFinancialPeriodDateTime, bool isOriginalAccrual)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of billables that are pending accrual to validate the list we received
            var items = await _dbContext.BillableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId && bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_PENDING).ToListAsync();
            var itemsToMark = items.Where(i => BillableItemIds.Any(b => b == i.Id)).ToList();

            itemsToMark.ForEach(item =>
            {
                item.BillableItemStatusIdentifier = BillableItemStatusIdentifier.ACCRUAL_POSTED;
                item.AccrualPostedBy = currentUser;
                item.AccrualPostedDateTime = dateStamp;
                item.AccrualFinancialPeriodDateTime = accrualFinancialPeriodDateTime;
                item.IsOriginalAccrual = isOriginalAccrual;
                item.OriginalAccrualAmountBillingCurrency = item.AccrualAmountBillingCurrency;
                item.OriginalAccrualAmountUSD = item.AccrualAmountUSD;
                item.OriginalAccrualPostedBy = currentUser;
                item.OriginalAccrualPostedDateTime = dateStamp;
                item.DateModified = dateStamp;
                item.ModifiedBy = currentUser;
            });

            var superServiceOrder = _dbContext.SuperServiceOrder.Include(sso => sso.Job).FirstOrDefault(x => x.Id == superServiceOrderId);
            superServiceOrder.AccrualPostedDateTime = dateStamp;

            superServiceOrder.ModifiedBy = GetCurrentUserEmail();
            superServiceOrder.DateModified = dateStamp;
            superServiceOrder.AccrualStatus = AccrualStatus.POSTED;
            superServiceOrder.Job.ModifiedBy = GetCurrentUserEmail();
            superServiceOrder.Job.DateModified = dateStamp;
            superServiceOrder.Job.AccrualStatus = AccrualStatus.POSTED;

            await _dbContext.SaveChangesAsync();

            return new PostedAccrualsResponse
            {
                BillableRecordsUpdated = itemsToMark.Count(),
                AccrualPostedDateTime = dateStamp,
            };
        }

        public async Task<PostedAccrualsResponse> MarkBillableItemPostedAsync(int billableItemId, DateTime financialPeriodDateTime)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of billables that are pending accrual to validate the list we received
            var itemToMark = _dbContext.BillableItem.FirstOrDefault(bi => bi.Id == billableItemId);

            itemToMark.BillableItemStatusIdentifier = BillableItemStatusIdentifier.ACCRUAL_POSTED;
            itemToMark.AccrualPostedBy = currentUser;
            itemToMark.AccrualPostedDateTime = dateStamp;
            itemToMark.AccrualFinancialPeriodDateTime = financialPeriodDateTime;
            itemToMark.OriginalAccrualAmountBillingCurrency = itemToMark.AccrualAmountBillingCurrency;
            itemToMark.OriginalAccrualAmountUSD = itemToMark.AccrualAmountUSD;
            itemToMark.OriginalAccrualPostedBy = currentUser;
            itemToMark.OriginalAccrualPostedDateTime = dateStamp;
            itemToMark.DateModified = dateStamp;
            itemToMark.ModifiedBy = currentUser;

            await _dbContext.SaveChangesAsync();

            return new PostedAccrualsResponse
            {
                BillableRecordsUpdated = 1,
                AccrualPostedDateTime = dateStamp,
            };
        }

        public async Task<VoidedAccrualsResponse> MarkAccrualsVoidedAsync(int superServiceOrderId, UpdateAccruablesRequest request)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of billables that are eligible for void
            var items = await _dbContext.BillableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId &&
                               bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_POSTED).ToListAsync();

            var itemsToMark = items.Where(i => request.BillableItemIds.Any(b => b == i.Id)).ToList();

            itemsToMark.ForEach(item =>
            {
                item.BillableItemStatusIdentifier = BillableItemStatusIdentifier.VOID;
                item.VoidedDateTime = dateStamp;
                item.VoidedBy = currentUser;
                item.VoidedFinancialPeriodDate = request.EffectiveDateTime;
                item.DateModified = dateStamp;
                item.ModifiedBy = currentUser;
            });

            await _dbContext.SaveChangesAsync();

            return new VoidedAccrualsResponse
            {
                BillableRecordsUpdated = itemsToMark.Count(),
                AccrualVoidedDateTime = dateStamp,
            };
        }

        #endregion Mark Accruals Status

        #region Mark Actuals Status

        public async Task<PostedActualsResponse> MarkActualsAsPostedAsync(int superServiceOrderId, IEnumerable<int> BillableItemIds, DateTime financialPeriodDateTime)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of billables that are pending actual to validate the list we received
            var itemsToMark = await _dbContext.BillableItem
                .Include(bi => bi.Invoice)
                .Where(bi => bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACTUAL_PENDING
                && BillableItemIds.Contains(bi.Id))
                .ToListAsync();

            itemsToMark.ForEach(item =>
            {
                item.BillableItemStatusIdentifier = BillableItemStatusIdentifier.ACTUAL_POSTED;
                item.DateModified = financialPeriodDateTime;
                item.ActualFinancialPeriodDateTime = financialPeriodDateTime;

                item.ActualPostedBy = currentUser;
                item.ActualPostedDateTime = dateStamp;

                item.OriginalActualAmountBillingCurrency = item.ActualAmountBillingCurrency;
                item.OriginalActualAmountUSD = item.ActualAmountUSD;
                item.OriginalActualPostedBy = currentUser;
                item.OriginalActualPostedDateTime = dateStamp;

                item.DateModified = dateStamp;
                item.ModifiedBy = currentUser;
            });

            var invoice = itemsToMark.FirstOrDefault().Invoice;

            invoice.InvoiceStatusIdentifier = InvoiceStatusIdentifier.GENERATED;
            invoice.TotalInvoiceAmount = itemsToMark.Select(bi => bi.ActualAmountUSD).Sum();

            invoice.InvoicePostedDateTime = dateStamp;
            invoice.ModifiedBy = currentUser;
            invoice.DateModified = dateStamp;

            await _dbContext.SaveChangesAsync();

            return new PostedActualsResponse
            {
                BillableRecordsUpdated = itemsToMark.Count(),
                ActualPostedDateTime = dateStamp,
            };
        }

        #endregion Mark Actuals Status

        #region Mark Adjustments Status

        public async Task<PostedAdjustmentsResponse> MarkAdjustmentAsPostedAsync(int id)
        {
            var dateStamp = DateTime.UtcNow;
            var currentUsername = GetCurrentUserEmail();

            var adjustment = _dbContext.BillableItemAdjustment.FirstOrDefault(bia => bia.Id == id);

            if (adjustment.AdjustmentStatus != AdjustmentStatusIdentifier.PENDING)
            {
                throw new Exception($"Cannot mark as posted - BillableItemAdjustment is not in {AdjustmentStatusIdentifier.PENDING} status");
            }

            adjustment.AdjustmentStatus = AdjustmentStatusIdentifier.POSTED;
            adjustment.AdjustmentDateTime = dateStamp;

            var billableItem = _dbContext.BillableItem
                .Include(bi => bi.Invoice)
                .FirstOrDefault(bi => bi.Id == adjustment.BillableItemId);

            switch (adjustment.AdjustmentType)
            {
                case AdjustmentType.ACCRUAL:
                    billableItem.IsAccrualAdjusted = true;
                    billableItem.AccrualAmountUSD = adjustment.NewAmountUSD;
                    billableItem.GPDocNum = adjustment.GPDocNum;

                    billableItem.AccrualAdjustmentBy = currentUsername;
                    billableItem.AccrualAdjustmentDateTime = dateStamp;
                    break;

                case AdjustmentType.ACTUAL:
                    billableItem.IsActualAdjusted = true;
                    billableItem.ActualAmountUSD = adjustment.NewAmountUSD;
                    billableItem.Invoice.InvoiceNumber = adjustment.GPDocNum;

                    billableItem.ActualAdjustmentBy = currentUsername;
                    billableItem.ActualAdjustmentDateTime = dateStamp;
                    break;
            }

            billableItem.ModifiedBy = currentUsername;
            billableItem.DateModified = dateStamp;

            await _dbContext.SaveChangesAsync();

            return new PostedAdjustmentsResponse
            {
                AdjustmentId = adjustment.Id,
                BillableRecordsUpdated = 1,
                AdjustmentPostedDateTime = dateStamp,
            };
        }

        #endregion Mark Adjustments Status

        public async Task<IEnumerable<GetReversableResponse>> GetBillableItemAccrualReversalsAsync(ICollection<int> billableItemIds)
        {
            var query = await _dbContext.BillableItem
                                    .Include(bi => bi.SuperServiceOrder)
                                    .ThenInclude(sso => sso.Job)
                                    .Include(bi => bi.BillableItemType)
                                    .Where(bi => !string.IsNullOrEmpty(bi.GPDocNum) && billableItemIds.Contains(bi.Id))
                                    .AsNoTracking()
                                    .ToListAsync();

            List<GetReversableResponse> reversals = new List<GetReversableResponse>();
            foreach (var i in query)
            {
                GetReversableResponse obj = new GetReversableResponse();
                obj.DocNum = i.GPDocNum;
                obj.BillableItemId = i.Id;
                //obj.AccountingId = GPAccrualEntityId.CUSTOMER_ID_RECEIVABLE_ACCRUAL;
                reversals.Add(obj);
            }

            return reversals;
        }

        #region privates

        private static void RunCalcs(out decimal? totalAccruableRevenue, out decimal? totalAccruableProfit, out decimal? marginPercentage, SuperServiceOrder item)
        {
            totalAccruableRevenue = item.BillableItem.Select(s => s.AccrualAmountUSD).Sum();
            totalAccruableProfit = totalAccruableRevenue - item.PayableItem.Select(s => s.AccrualAmountUSD).Sum();
            marginPercentage = totalAccruableProfit / totalAccruableRevenue;
        }

        private List<GetPostableResponse> MapBillableItemsToAccruables(int superServiceOrderId, List<BillableItem> billableItems, DateTime? actualPackEndDate)
        {
            string superServiceOrderDisplayId = billableItems.Select(x => x.SuperServiceOrder.DisplayId).FirstOrDefault();
            string username = GetCurrentUserEmail();

            var superServiceOrderInfo = _dbContext.SuperServiceOrder
                .Where(sso => sso.Id == superServiceOrderId)
                .Include(sso => sso.Job)
                .Include(sso => sso.Transferee)
                .AsNoTracking()
                .FirstOrDefault();

            string transfereeLastFirstName = String.Concat(superServiceOrderInfo.Transferee.LastName, ", ", superServiceOrderInfo.Transferee.FirstName);
            int sequenceMulitplier = 16384;

            List<GetPostableResponse> response = new List<GetPostableResponse>();
            billableItems.ToList().ForEach(bi =>
            {
                string accountingId = GetAccountingIdByBillableItem(bi);

                if (string.IsNullOrEmpty(accountingId))
                {
                    throw new Exception("AccountingId for BillTo cannot be null");
                }

                int itemBillToId;

                switch (bi.BillToType.ToUpper())
                {
                    case EntityType.ACCOUNT_ENTITY:
                        itemBillToId = bi.BillToAccountEntityId.Value;
                        break;

                    case EntityType.VENDOR:
                        itemBillToId = bi.BillToVendorId.Value;
                        break;

                    case EntityType.TRANSFEREE:
                        itemBillToId = bi.BillToTransfereeId.Value;
                        break;

                    default:
                        throw new Exception("Invalid BillToType");
                }
                string gPDocType;
                int itemGPDistType;

                if (bi.AccrualAmountUSD < 0)
                {
                    gPDocType = GPDocType.RECEIVABLE_CREDIT_MEMO.ToString();
                    itemGPDistType = GPDistType.CREDIT_MEMO;
                }
                else
                {
                    gPDocType = GPDocType.INVOICE.ToString();
                    itemGPDistType = GPDistType.SALES;
                }

                var receivableOrPayable = "Receivable";
                var accrualOrActual = "Accrual";
                var branchName = bi.SuperServiceOrder.Job?.BranchName;
                var revenueType = bi.SuperServiceOrder.Job?.RevenueType;
                var accountCode = bi.BillableItemType.AccountCode;

                string recAcctNr = _accountingService.GetGLCodeAsync(accrualOrActual, receivableOrPayable, branchName, revenueType, GPLedgerAccountCode.RECEIVABLE_ACCOUNT_CODE, GPDistType.RECEIVABLE).Result;

                string salesAcctNr = _accountingService.GetGLCodeAsync(accrualOrActual, receivableOrPayable, branchName, revenueType, accountCode, GPDistType.SALES).Result;
                if (string.IsNullOrEmpty(recAcctNr))
                {
                    _logger.LogError($"No receivable GL account number found for billable item with id {bi.Id}.");
                    throw new Exception("No receivable GL account number found.");
                }
                if (string.IsNullOrEmpty(salesAcctNr))
                {
                    _logger.LogError($"No sales GL account number found for billable item with id {bi.Id}.");
                    throw new Exception("No sales GL account number found.");
                }
                response.Add(
                        new GetPostableResponse
                        {
                            SuperServiceOrderId = bi.SuperServiceOrder.Id,
                            DisplayId = bi.SuperServiceOrder.DisplayId,
                            ActualPackEndDate = actualPackEndDate,
                            EntityType = bi.BillToType,
                            EntityId = itemBillToId,

                            Header = new AccruableARHeader
                            {
                                RecordType = GPRecordType.AR_HEADER,
                                ActualCustomerId = accountingId,
                                CustomerId = GPAccrualEntityId.CUSTOMER_ID_RECEIVABLE_ACCRUAL,
                                BillableItemId = bi.Id,
                                DocType = gPDocType,
                                DocAmt = Math.Abs(bi.AccrualAmountUSD.Value),
                                DocNum = bi.GPDocNum,
                                Desc = String.Concat(bi.SuperServiceOrder.DisplayId, "/", transfereeLastFirstName).Substring(0, Math.Min(String.Concat(bi.SuperServiceOrder.DisplayId, "/", transfereeLastFirstName).Length, 29)),
                                DocDate = DateTime.UtcNow,
                                PostingDate = bi.AccrualFinancialPeriodDateTime ?? DateTime.MinValue,
                                PostingUserName = username,
                                RegNumber = superServiceOrderDisplayId,
                                ShipperName = transfereeLastFirstName,
                                ImportDescription = $"AR Header BillableItem #{bi.Id}",
                                ImportStatus = 0,
                            },
                            LineItems = new List<PostableLineItem>
                            {
                                new PostableLineItem
                                {
                                    BillableItemId = null,
                                    RecordType = GPRecordType.AR_DETAIL,
                                    CustomerId = GPAccrualEntityId.CUSTOMER_ID_RECEIVABLE_ACCRUAL,
                                    DocType = gPDocType,
                                    DistAmt = bi.AccrualAmountUSD.Value,
                                    DistType = GPDistType.RECEIVABLE,
                                    DocNum = bi.GPDocNum,
                                    PostingUserName = username,
                                    MovesId = superServiceOrderDisplayId,
                                    Sequence = sequenceMulitplier,
                                    ItemCode = GPLedgerAccountCode.RECEIVABLE_ACCOUNT_CODE,
                                    ImportDescription = $"AR Detail (Receivable) BillableItem #{bi.Id}-Detail 1 of 2",
                                    ImportStatus = 0,
                                    RegNumber = superServiceOrderDisplayId,
                                    AcctNum = recAcctNr,
                                },
                                new PostableLineItem
                                {
                                    BillableItemId = bi.Id,
                                    RecordType = GPRecordType.AR_DETAIL,
                                    CustomerId = GPAccrualEntityId.CUSTOMER_ID_RECEIVABLE_ACCRUAL,
                                    DocType = gPDocType,
                                    DistAmt = -1*bi.AccrualAmountUSD.Value,
                                    DistType = itemGPDistType,
                                    DocNum = bi.GPDocNum,
                                    PostingUserName = username,
                                    MovesId = superServiceOrderDisplayId,
                                    Sequence = 2*sequenceMulitplier,
                                    ItemCode = bi.BillableItemType.AccountCode,
                                    ImportDescription = $"AR Detail (Sales) BillableItem #{bi.Id}-Detail 2 of 2",
                                    ImportStatus = 0,
                                    RegNumber = superServiceOrderDisplayId,
                                    AcctNum = salesAcctNr,
                                },
                            }
                        }

                );
            });

            return response;
        }

        private string GetAccountingIdByBillableItem(BillableItem billableItem)
        {
            string accountingId;
            string billToType = billableItem.BillToType?.ToUpper();

            if (string.IsNullOrEmpty(billToType))
            {
                throw new Exception("BillableItem is missing BillToType");
            }

            switch (billToType)
            {
                case EntityType.ACCOUNT_ENTITY:

                    accountingId = billableItem.BillToAccountEntity.AccountingId;
                    break;

                case EntityType.VENDOR:

                    accountingId = billableItem.BillToVendor.AccountingId;
                    break;

                case EntityType.TRANSFEREE:

                    accountingId = billableItem.BillToTransferee.AccountingId.ToString();
                    break;

                default:
                    throw new Exception($"Invalid BillToType: {billToType}");
            }

            return accountingId;
        }

        #endregion privates
    }
}