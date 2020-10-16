using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models.ResponseModels.BillableItem;
using Suddath.Helix.JobMgmt.Models.ResponseModels.JobCost;
using Suddath.Helix.JobMgmt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suddath.Helix.JobMgmt.Services
{
    public class PayableItemService : BaseOrderService, IPayableItemService
    {
        private readonly JobDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PayableItemService> _logger;
        private readonly IAccountingService _accountingService;

        public PayableItemService(IAccountingService accountingService,
                                JobDbContext dbContext,
                                IMapper mapper,
                                ILogger<PayableItemService> logger,
                                IHttpContextAccessor httpContextAccessor) : base(dbContext, mapper, httpContextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountingService = accountingService ?? throw new ArgumentNullException(nameof(accountingService));
        }

        #region PayableItem CRUD

        public async Task<IEnumerable<GetPayableItemResponse>> GetPayableItemsAsync(int superServiceOrderId)
        {
            var results = await _dbContext.PayableItem
                .Where(pi => pi.SuperServiceOrderId == superServiceOrderId)
                .Include(pi => pi.BillableItemType)
                .Include(pi => pi.BillFromAccountEntity)
                .Include(pi => pi.BillFromTransferee)
                .Include(pi => pi.BillFromVendor)
                .Include(pi => pi.VendorInvoice)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetPayableItemResponse>>(results);
        }

        public async Task<CreatePayableItemResponse> CreatePayableItemAsync(int superServiceOrderId)
        {
            var entity = new PayableItem
            {
                SuperServiceOrderId = superServiceOrderId,
                PayableItemStatusIdentifier = PayableItemStatusIdentifier.QUEUED,
                VendorCurrency = "USD",
                VendorCurrencySymbol = "$"
            };

            string username = GetCurrentUserEmail();
            DateTime dateStamp = DateTime.UtcNow;

            entity.DateCreated = dateStamp;
            entity.DateModified = dateStamp;
            entity.CreatedBy = username;
            entity.ModifiedBy = username;

            await Add(entity);

            var dbEntity = _dbContext.PayableItem
                .FirstOrDefault(pi => pi.Id == entity.Id);

            var response = _mapper.Map<CreatePayableItemResponse>(dbEntity);

            return response;
        }

        public async Task<IEnumerable<GetSuperServiceOrderAvailableBillFromResponse>> GetSuperServiceOrderAvailableBillFromAsync(int superServiceOrderId)
        {
            var mappedResults = new List<GetSuperServiceOrderAvailableBillFromResponse>();

            var serviceOrderVendors = await _dbContext.ServiceOrder
                .Where(so => so.SuperServiceOrderId == superServiceOrderId && so.VendorId != null && so.Vendor.AccountingId != AccountingId.OVERSEAS_AGENT_VENDOR.ToString())
                .Include(so => so.Vendor)
                .Select(so => so.Vendor)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            var accountEntity = await _dbContext.SuperServiceOrder
                .Where(sso => sso.Id == superServiceOrderId && sso.Job.AccountEntity.AccountingId != AccountingId.OVERSEAS_AGENT_ACCOUNT.ToString())
                .Include(sso => sso.Job)
                .ThenInclude(j => j.AccountEntity)
                .Select(sso => sso.Job.AccountEntity)
                .AsNoTracking()
                .ToListAsync();

            var transferee = _dbContext.SuperServiceOrder
                .Where(sso => sso.Id == superServiceOrderId)
                .Include(sso => sso.Job)
                .ThenInclude(j => j.Transferee)
                .Select(sso => sso.Job.Transferee)
                .AsNoTracking()
                .FirstOrDefault();

            var job = _dbContext.SuperServiceOrder
                 .Include(sso => sso.Job)
                 .FirstOrDefault(sso => sso.Id == superServiceOrderId).Job;

            IEnumerable<AccountEntity> accountBillFrom = new List<AccountEntity>();
            IEnumerable<Vendor> vendorBillFrom = new List<Vendor>();

            switch (job.BillToType.ToUpper())
            {
                case EntityType.ACCOUNT_ENTITY:
                    accountBillFrom = _dbContext.AccountEntity.Where(ae => ae.Id == job.BillToId && ae.AccountingId != AccountingId.OVERSEAS_AGENT_ACCOUNT.ToString());
                    break;

                case EntityType.VENDOR:
                    vendorBillFrom = _dbContext.Vendor.Where(v => v.Id == job.BillToId && v.AccountingId != AccountingId.OVERSEAS_AGENT_VENDOR.ToString());
                    break;
            }

            IEnumerable<PayableItem> paidEntities = _dbContext.PayableItem
                .Include(sso => sso.BillFromAccountEntity)
                .Include(sso => sso.BillFromVendor)
                .Include(sso => sso.BillFromTransferee)
                .Where(bi => bi.SuperServiceOrderId == superServiceOrderId);

            IEnumerable<AccountEntity> billedAccountEntites = paidEntities
                .Where(bi => bi.BillFromAccountEntityId != null)
                .Select(bi => bi.BillFromAccountEntity)
                .Distinct();

            IEnumerable<Vendor> billedVendors = paidEntities
                .Where(bi => bi.BillFromVendorId != null)
                .Select(bi => bi.BillFromVendor)
                .Distinct();

            mappedResults.AddRange(_mapper.Map<List<GetSuperServiceOrderAvailableBillFromResponse>>(
                accountEntity
                .Union(billedAccountEntites)
                .Union(accountBillFrom)
                .Distinct()
                ));

            mappedResults.AddRange(_mapper.Map<List<GetSuperServiceOrderAvailableBillFromResponse>>(
                serviceOrderVendors
                .Union(billedVendors)
                .Union(vendorBillFrom)
                .Distinct()
                ));

            mappedResults.Add(_mapper.Map<GetSuperServiceOrderAvailableBillFromResponse>(transferee));

            return mappedResults.Distinct().OrderBy(x => x.BillFromType).ThenBy(x => x.Name);
        }

        public async Task UpdatePayableItemAsync(int id, JsonPatchDocument patch)
        {
            _logger.LogInformation("Incoming PayableItemId = {0}, patchJson = {1}", id, JsonConvert.SerializeObject(patch));
            try
            {
                var dateStamp = DateTime.UtcNow;
                var currentUsername = GetCurrentUserEmail();

                var entity = _dbContext.PayableItem.FirstOrDefault(pi => pi.Id == id);

                var current = _mapper.Map<GetPayableItemResponse>(entity);

                patch.ApplyTo(current);

                _logger.LogInformation("Starting to Update Payable");

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

                _logger.LogInformation("Payable Patch succeeded: id = {0}, patchJson = {1}", id, JsonConvert.SerializeObject(patch));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Payable Patch failed: id = {id}, patchJson = {JsonConvert.SerializeObject(patch)}");
                throw ex;
            }
        }

        public async Task<string> ValidateUpdatePayableItemAsync(int id, JsonPatchDocument patch)
        {
            var stringBuilder = new StringBuilder();

            var entity = await Get<PayableItem>(x => x.Id == id);
            var current = _mapper.Map<GetPayableItemResponse>(entity);

            // hold can be applied in any status except ACTUAL_POSTED and VOID and Partial_Paid
            if ((entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.ACTUAL_POSTED &&
                 entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.VOID &&
                 entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.PARTIAL_PAID) &&
                 patch.Operations.All(x => x.path.ToLower().StartsWith("/hold")))
            {
                return stringBuilder.ToString();
            }
            if (entity.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACTUAL_POSTED &&
               (
                patch.Operations.All(x => x.path.ToLower().StartsWith("/writeoff"))
               ))
            {
                return stringBuilder.ToString();
            }

            //Patching on the right side of the expense grid
            if ((entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.ACTUAL_PENDING &&
                 entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.ACTUAL_POSTED &&
                 entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.PARTIAL_PAID) &&
               (
                   patch.Operations.All(x => x.path.ToLower().StartsWith("/actual")) ||
                   patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.VendorInvoiceNumber).ToLower())) ||
                   patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.VendorInvoiceDate).ToLower())) ||
                   patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.CheckWireNumber).ToLower())) ||
                   patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.PaidDate).ToLower())) &&
                   !patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.ActualExchangeRateDateTime).ToLower()))

               ))
            {
                return stringBuilder.ToString();
            }
            if (patch.Operations.All(x => x.path.ToLower() == "/description"))
            {
                return stringBuilder.ToString();
            }
            if (
                (entity.PayableItemStatusIdentifier == PayableItemStatusIdentifier.QUEUED ||
                entity.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACCRUAL_PENDING ||
                entity.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACCRUAL_POSTED)
                &&
                (
                 patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.BillFromType).ToLower())) &&
                 patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.BillFromId).ToLower()))
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
            if (entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.QUEUED)
            {
                stringBuilder.AppendLine("Cannot update PayableItems not in QUEUED Status");
            }
            if (patch.Operations.Any(x => x.path.ToLower().Contains(nameof(current.SuperServiceOrderId).ToLower())))
            {
                stringBuilder.AppendLine("SuperServiceOrderId cannot be updated");
            }
            if (
                (patch.Operations.Any(x => x.path.ToLower().Equals("/vendorcurrency")) &&
                  !patch.Operations.Any(x => x.path.ToLower().Equals("/vendorcurrencysymbol")))
                 ||
                (!patch.Operations.Any(x => x.path.ToLower().Equals("/vendorcurrency")) &&
                  patch.Operations.Any(x => x.path.ToLower().Equals("/vendorcurrencysymbol")))
               )
            {
                stringBuilder.AppendLine("Must update currency and symbol together");
            }
            if (patch.Operations.Any(x =>
                x.path.ToLower().Contains(nameof(current.BillFromType).ToLower()))
                 && !(patch.Operations.Any(y => y.path.ToLower().Contains(nameof(current.BillFromId).ToLower())))
)
            {
                stringBuilder.AppendLine("Must update BillFromType, BillFromId together");
            }
            if (patch.Operations.Any(x =>
                x.path.ToLower().Contains(nameof(current.BillFromId).ToLower()))
                 && !(patch.Operations.Any(y => y.path.ToLower().Contains(nameof(current.BillFromType).ToLower())))
)
            {
                stringBuilder.AppendLine("Must update BillFromType, BillFromId together");
            }

            return stringBuilder.ToString();
        }

        public async Task DeletePayableItemAsync(int id)
        {
            var current = await Get<PayableItem>(pi => pi.Id == id);

            if (current != null)
            {
                await Remove(current);
            }

            return;
        }

        public async Task<string> ValidateDeletePayableItemAsync(int id)
        {
            var stringBuilder = new StringBuilder();

            var entity = await Get<PayableItem>(x => x.Id == id);

            if (entity.PayableItemStatusIdentifier != PayableItemStatusIdentifier.QUEUED)
            {
                stringBuilder.AppendLine("Cannot delete PayableItems not in QUEUED Status");
            }

            return stringBuilder.ToString();
        }

        public async Task<bool> GetExistsPayableItemByTuple(int jobId, int superServiceOrderId, int id)
        {
            bool exists = (await Get<PayableItem>(pi => pi.Id == id && pi.SuperServiceOrderId == superServiceOrderId
                            && pi.SuperServiceOrder.JobId == jobId) == null) ? false : true;

            return await Task.FromResult(exists);
        }

        #endregion PayableItem CRUD

        public async Task<IEnumerable<GetBillableItemTypeResponse>> GetPayableItemTypesAsync()
        {
            var results = await _dbContext.BillableItemType
                            .Where(bit => bit.IsActive.Value)
                            .AsNoTracking()
                            .ToListAsync();

            return _mapper.Map<IEnumerable<GetBillableItemTypeResponse>>(results);
        }

        #region Lock/Unlock Accruals

        public async Task<LockAccrualsResponse> LockAccrualsAsync(int superServiceOrderId)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            var items = await _dbContext.PayableItem.Where(pi => pi.SuperServiceOrderId == superServiceOrderId && pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.QUEUED).ToListAsync();

            items.ForEach(item =>
            {
                item.PayableItemStatusIdentifier = PayableItemStatusIdentifier.ACCRUAL_PENDING;
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
            var items = await _dbContext.PayableItem.Where(pi => pi.SuperServiceOrderId == superServiceOrderId && pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACCRUAL_PENDING).ToListAsync();

            items.ForEach(item => item.PayableItemStatusIdentifier = PayableItemStatusIdentifier.QUEUED);

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
            var payableItems = await _dbContext.PayableItem.Where(x => x.SuperServiceOrderId == superServiceOrderId).ToListAsync();

            switch (entity.SuperServiceId)
            {
                case SuperServiceId.AIR:
                    var airEntity = await GetAirFreightMetrics(superServiceOrderId);
                    netWeight = airEntity?.NetWeightLb;
                    grossWeight = airEntity?.GrossWeightLb;
                    break;

                case SuperServiceId.OCEAN:
                    var oceanEntity = await GetOceanFreightMetrics(superServiceOrderId);
                    netWeight = oceanEntity?.NetWeightLb;
                    grossWeight = oceanEntity?.GrossWeightLb;
                    break;

                case SuperServiceId.ROAD:
                    var roadEntity = await GetRoadFreightMetrics(superServiceOrderId);

                    netWeight = roadEntity?.NetWeightLb;
                    grossWeight = roadEntity?.GrossWeightLb;
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
            if (payableItems.Any(pi => pi.BillableItemTypeId == null))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null PayableItemType (=BillableItemType)");
            }
            if (payableItems.Any(pi => string.IsNullOrWhiteSpace(pi.Description)))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null Description");
            }
            if (payableItems.Any(pi => string.IsNullOrWhiteSpace(pi.BillFromType)))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null BillFromType");
            }
            if (payableItems.Any(pi => pi.BillFromType == EntityType.VENDOR && pi.BillFromVendorId == null))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null (VENDOR) BillFromId");
            }
            if (payableItems.Any(pi => pi.BillFromType == EntityType.ACCOUNT_ENTITY && pi.BillFromAccountEntityId == null))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null (ACCOUNT) BillFromId");
            }
            if (payableItems.Any(pi => pi.BillFromType == EntityType.TRANSFEREE && pi.BillFromTransfereeId == null))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null (TRANSFEREE) BillFromId");
            }
            if (payableItems.Any(pi => string.IsNullOrEmpty(pi.VendorCurrency)))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null VendorCurrency");
            }
            if (payableItems.Any(pi => pi.AccrualAmountUSD == null || pi.AccrualAmountUSD == 0))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null or zero amounts (USD)");
            }
            if (payableItems.Any(pi => pi.AccrualAmountVendorCurrency == null || pi.AccrualAmountVendorCurrency == 0))
            {
                stringBuilder.AppendLine("Cannot lock accruals with null or zero amounts (VendorCurrency)");
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

            return stringBuilder.ToString();
        }

        #endregion Lock/Unlock Accruals

        #region Accruables

        public async Task<IEnumerable<GetPostableResponse>> GetPayableItemAccruablesAsync(int superServiceOrderId, DateTime? actualPackEndDate)
        {
            string username = GetCurrentUserEmail();

            var dbAccruables = await _dbContext.PayableItem
                .Include(pi => pi.SuperServiceOrder)
                .ThenInclude(sso => sso.Job)
                .Include(pi => pi.BillableItemType)
                .Include(pi => pi.BillFromTransferee)
                .Include(pi => pi.BillFromAccountEntity)
                .Include(pi => pi.BillFromVendor)
                .Where(pi => pi.SuperServiceOrderId == superServiceOrderId && pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACCRUAL_PENDING)
                .AsNoTracking()
                .ToListAsync();

            var response = MapPayableItemsToAccruables(superServiceOrderId, dbAccruables, actualPackEndDate);

            return response;
        }

        public async Task<GetPostableResponse> GetPayableItemAccruableByIdAsync(int id, DateTime? actualPackEndDate)
        {
            var accruablePayableItem = await _dbContext.PayableItem
                .Include(bi => bi.SuperServiceOrder)
                .ThenInclude(sso => sso.Job)
                .Include(bi => bi.BillableItemType)
                .Include(pi => pi.BillFromTransferee)
                .Include(pi => pi.BillFromAccountEntity)
                .Include(pi => pi.BillFromVendor)
                .Where(bi => bi.Id == id)
                .AsNoTracking()
                .ToListAsync();

            var response = MapPayableItemsToAccruables(accruablePayableItem.FirstOrDefault().SuperServiceOrderId, accruablePayableItem, actualPackEndDate);

            return response.FirstOrDefault();
        }

        public async Task<string> ValidatePayableItemVoidableByIdAsync(int id)
        {
            var stringBuilder = new StringBuilder();

            var payableItem = await _dbContext.PayableItem
                .Include(bi => bi.SuperServiceOrder)
                .Where(bi => bi.Id == id)
                .FirstAsync();

            if (payableItem.PayableItemStatusIdentifier != PayableItemStatusIdentifier.ACCRUAL_POSTED)
            {
                stringBuilder.AppendLine($"Cannot void individual PayableItem in a status of {payableItem.PayableItemStatusIdentifier}");
            }

            return stringBuilder.ToString();
        }

        public async Task<string> ValidatePayableItemAccrualByIdAsync(int id)
        {
            var stringBuilder = new StringBuilder();

            var payableItem = await _dbContext.PayableItem
                .Include(bi => bi.SuperServiceOrder)
                .Where(bi => bi.Id == id)
                .FirstOrDefaultAsync();

            if (payableItem.PayableItemStatusIdentifier != PayableItemStatusIdentifier.QUEUED)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual that is not in QUEUED status");
            }
            if (payableItem.SuperServiceOrder.AccrualPostedDateTime == null)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual when SuperServiceOrder has not already been locked and accrued");
            }
            if (payableItem.BillableItemTypeId == null)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null BillableItemType");
            }
            if (string.IsNullOrWhiteSpace(payableItem.Description))
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null Description");
            }
            if (string.IsNullOrWhiteSpace(payableItem.BillFromType))
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null BillFromType");
            }
            if (payableItem.BillFromType == EntityType.VENDOR && payableItem.BillFromVendorId == null)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null (VENDOR) BillFromId");
            }
            if (payableItem.BillFromType == EntityType.ACCOUNT_ENTITY && payableItem.BillFromAccountEntityId == null)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null (ACCOUNT) BillFromId");
            }
            if (payableItem.BillFromType == EntityType.TRANSFEREE && payableItem.BillFromTransfereeId == null)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null (TRANSFEREE) BillFromId");
            }
            if (string.IsNullOrEmpty(payableItem.VendorCurrency))
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null VendorCurrency");
            }
            if (payableItem.AccrualAmountUSD == null || payableItem.AccrualAmountUSD == 0)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null or zero amounts (USD)");
            }
            if (payableItem.AccrualAmountVendorCurrency == null || payableItem.AccrualAmountVendorCurrency == 0)
            {
                stringBuilder.AppendLine("Cannot post individual PayableItem accrual with null or zero amounts (VendorCurrency)");
            }

            return stringBuilder.ToString();
        }

        #endregion Accruables

        #region Mark Accruals Status

        public async Task<PostedAccrualsResponse> MarkAccrualsPostedAsync(int superServiceOrderId, IEnumerable<int> PayableItemIds, DateTime accrualFinancialPeriodDateTime, bool isOriginalAccrual)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of payables that are available for void
            var items = await _dbContext.PayableItem.Where(pi => pi.SuperServiceOrderId == superServiceOrderId && pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACCRUAL_PENDING).ToListAsync();

            var itemsToMark = items.Where(i => PayableItemIds.Any(b => b == i.Id)).ToList();

            itemsToMark.ForEach(item =>
            {
                item.PayableItemStatusIdentifier = PayableItemStatusIdentifier.ACCRUAL_POSTED;
                item.AccrualPostedBy = currentUser;
                item.AccrualPostedDateTime = dateStamp;
                item.AccrualFinancialPeriodDateTime = accrualFinancialPeriodDateTime;
                item.IsOriginalAccrual = isOriginalAccrual;
                item.OriginalAccrualAmountVendorCurrency = item.AccrualAmountVendorCurrency;
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
                PayableRecordsUpdated = itemsToMark.Count(),
                AccrualPostedDateTime = dateStamp,
            };
        }

        public async Task<PostedAccrualsResponse> MarkPayableItemPostedAsync(int payableItemId, DateTime financialPeriodDateTime)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of billables that are pending accrual to validate the list we received
            var itemToMark = _dbContext.PayableItem.FirstOrDefault(bi => bi.Id == payableItemId);

            itemToMark.PayableItemStatusIdentifier = PayableItemStatusIdentifier.ACCRUAL_POSTED;
            itemToMark.AccrualPostedBy = currentUser;
            itemToMark.AccrualPostedDateTime = dateStamp;
            itemToMark.AccrualFinancialPeriodDateTime = financialPeriodDateTime;
            itemToMark.OriginalAccrualAmountVendorCurrency = itemToMark.AccrualAmountVendorCurrency;
            itemToMark.OriginalAccrualAmountUSD = itemToMark.AccrualAmountUSD;
            itemToMark.OriginalAccrualPostedBy = currentUser;
            itemToMark.OriginalAccrualPostedDateTime = dateStamp;
            itemToMark.DateModified = dateStamp;
            itemToMark.ModifiedBy = currentUser;

            await _dbContext.SaveChangesAsync();

            return new PostedAccrualsResponse
            {
                PayableRecordsUpdated = 1,
                AccrualPostedDateTime = dateStamp,
            };
        }

        public async Task<VoidedAccrualsResponse> MarkAccrualsVoidedAsync(int superServiceOrderId, UpdateAccruablesRequest request)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            //Get list of payables that are pending accrual to validate the list we received
            var items = await _dbContext.PayableItem.Where(pi => pi.SuperServiceOrderId == superServiceOrderId &&
                              pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACCRUAL_POSTED).ToListAsync();

            var itemsToMark = items.Where(i => request.PayableItemIds.Any(p => p == i.Id)).ToList();

            itemsToMark.ForEach(item =>
            {
                item.PayableItemStatusIdentifier = PayableItemStatusIdentifier.VOID;
                item.DateModified = dateStamp;
                item.ModifiedBy = currentUser;
                item.VoidedDateTime = dateStamp;
                item.VoidedBy = currentUser;
                item.VoidedFinancialPeriodDate = request.EffectiveDateTime;
            });

            await _dbContext.SaveChangesAsync();

            return new VoidedAccrualsResponse
            {
                PayableRecordsUpdated = itemsToMark.Count(),
                AccrualVoidedDateTime = dateStamp,
            };
        }

        #endregion Mark Accruals Status

        #region Mark Actuals Status

        public async Task<PostedActualsResponse> MarkActualsAsPostedAsync(int superServiceOrderId, IEnumerable<int> PayableItemIds, DateTime accrualFinancialPeriodDateTime)
        {
            var dateStamp = DateTime.UtcNow;
            string currentUser = GetCurrentUserEmail();

            var itemsToMark = await _dbContext.PayableItem
                .Include(pi => pi.VendorInvoice)
                .Where(pi => pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.ACTUAL_PENDING
                && PayableItemIds.Contains(pi.Id)).ToListAsync();

            itemsToMark.ForEach(item =>
            {
                item.PayableItemStatusIdentifier = PayableItemStatusIdentifier.ACTUAL_POSTED;
                item.ActualFinancialPeriodDateTime = accrualFinancialPeriodDateTime;

                item.ActualPostedBy = currentUser;
                item.ActualPostedDateTime = dateStamp;

                item.OriginalActualAmountVendorCurrency = item.ActualAmountVendorCurrency;
                item.OriginalActualAmountUSD = item.ActualAmountUSD;
                item.OriginalActualPostedBy = currentUser;
                item.OriginalActualPostedDateTime = dateStamp;

                item.DateModified = dateStamp;
                item.ModifiedBy = currentUser;
            });

            var vendorInvoice = itemsToMark.FirstOrDefault().VendorInvoice;

            vendorInvoice.VendorInvoiceStatusIdentifier = VendorInvoiceStatusIdentifier.POSTED;
            vendorInvoice.TotalVendorInvoiceAmount = itemsToMark.Select(bi => bi.ActualAmountUSD).Sum();

            vendorInvoice.VendorInvoicePostedDateTime = dateStamp;
            vendorInvoice.ModifiedBy = currentUser;
            vendorInvoice.DateModified = dateStamp;

            await _dbContext.SaveChangesAsync();

            return new PostedActualsResponse
            {
                PayableRecordsUpdated = itemsToMark.Count(),
                ActualPostedDateTime = dateStamp,
            };
        }

        #endregion Mark Actuals Status

        #region reversables

        public async Task<IEnumerable<GetReversableResponse>> GetPayableItemReversalsAsync(ICollection<int> payableItemIds)
        {
            var query = await _dbContext.PayableItem
                                    .Include(pi => pi.SuperServiceOrder)                                    
                                    .ThenInclude(sso => sso.Job)
                                    .Include(pi => pi.BillableItemType)
                                    .Where(pi => !string.IsNullOrEmpty(pi.GPDocNum) && payableItemIds.Contains(pi.Id))
                                    .AsNoTracking()
                                    .ToListAsync();

            List<GetReversableResponse> reversals = new List<GetReversableResponse>();
            foreach (var i in query)
            {
                GetReversableResponse obj = new GetReversableResponse();
                obj.DocNum = i.GPDocNum;
                obj.PayableItemId = i.Id;
                obj.AccountingId = GPAccrualEntityId.VENDOR_ID_PAYABLE_ACCRUAL;
                reversals.Add(obj);
            }

            return reversals;
        }

        #endregion reversables

        #region adjustables

        public async Task<GetPostableResponse> GetPayableItemAdjustableByIdAsync(int id, DateTime? actualPackEndDate)
        {
            var adjustment = await _dbContext.PayableItemAdjustment.FirstOrDefaultAsync(pia => pia.Id == id);

            var adjustablePayableItem = await _dbContext.PayableItem
                .Include(pi => pi.SuperServiceOrder)
                .ThenInclude(sso => sso.Job)
                .Include(pi => pi.BillableItemType)
                .Include(pi => pi.BillFromTransferee)
                .Include(pi => pi.BillFromAccountEntity)
                .Include(pi => pi.BillFromVendor)
                .Where(pi => pi.Id == adjustment.PayableItemId)
                .AsNoTracking()
                .ToListAsync();

            var response = MapPayableItemsToAccruables(adjustablePayableItem.FirstOrDefault().SuperServiceOrderId, adjustablePayableItem, actualPackEndDate);

            string newDocType;
            if (adjustment.NewAmountUSD >= 0)
            {
                newDocType = GPDocType.INVOICE.ToString();
            }
            else
            {
                newDocType = GPDocType.PAYABLE_CREDIT_MEMO.ToString();
            }

            var piAdjustableResponse = response.FirstOrDefault();

            piAdjustableResponse.Header.DocType = newDocType;
            piAdjustableResponse.Header.DocNum = adjustment.GPDocNum;
            piAdjustableResponse.Header.DocAmt = Math.Abs(adjustment.NewAmountUSD);

            piAdjustableResponse.LineItems.ForEach(lineItem =>
            {
                lineItem.DocType = newDocType;
                lineItem.DocNum = adjustment.GPDocNum;

                if (lineItem.DistType == GPDistType.PAYABLE)
                {
                    lineItem.DistAmt = -1 * adjustment.NewAmountUSD;
                }
                else if (lineItem.DistType == GPDistType.PURCHASE)
                {
                    lineItem.DistAmt = adjustment.NewAmountUSD;
                }
                else
                {
                    throw new Exception("GPDistType not supported");
                }
            });

            return piAdjustableResponse;
        }

        #endregion adjustables

        #region Mark Adjustments Status

        public async Task<PostedAdjustmentsResponse> MarkAdjustmentAsPostedAsync(int id)
        {
            var dateStamp = DateTime.UtcNow;
            var currentUsername = GetCurrentUserEmail();

            var adjustment = _dbContext.PayableItemAdjustment.FirstOrDefault(pia => pia.Id == id);

            if (adjustment.AdjustmentStatus != AdjustmentStatusIdentifier.PENDING)
            {
                throw new Exception($"Cannot mark as posted - PayableItemAdjustment is not in {AdjustmentStatusIdentifier.PENDING} status");
            }

            adjustment.AdjustmentStatus = AdjustmentStatusIdentifier.POSTED;
            adjustment.AdjustmentDateTime = dateStamp;

            var payableItem = _dbContext.PayableItem
                .Include(pi => pi.VendorInvoice)
                .FirstOrDefault(pi => pi.Id == adjustment.PayableItemId);

            switch (adjustment.AdjustmentType)
            {
                case AdjustmentType.ACCRUAL:
                    payableItem.IsAccrualAdjusted = true;
                    payableItem.AccrualAmountUSD = adjustment.NewAmountUSD;
                    payableItem.GPDocNum = adjustment.GPDocNum;
                    payableItem.AccrualAdjustmentBy = currentUsername;
                    payableItem.AccrualAdjustmentDateTime = dateStamp;
                    break;

                case AdjustmentType.ACTUAL:
                    payableItem.IsActualAdjusted = true;
                    payableItem.ActualAmountUSD = adjustment.NewAmountUSD;
                    payableItem.VendorInvoice.VendorInvoiceNumber = adjustment.GPDocNum;
                    payableItem.VendorInvoice.GPDocNum = adjustment.GPDocNum;
                    payableItem.ActualAdjustmentBy = currentUsername;
                    payableItem.ActualAdjustmentDateTime = dateStamp;

                    break;
            }

            payableItem.ModifiedBy = currentUsername;
            payableItem.DateModified = dateStamp;

            await _dbContext.SaveChangesAsync();

            return new PostedAdjustmentsResponse
            {
                AdjustmentId = id,
                PayableRecordsUpdated = 1,
                AdjustmentPostedDateTime = dateStamp,
            };
        }

        #endregion Mark Adjustments Status

        #region privates

        private List<GetPostableResponse> MapPayableItemsToAccruables(int superServiceOrderId, List<PayableItem> payableItems, DateTime? actualPackEndDate)
        {
            string superServiceOrderDisplayId = payableItems.Select(x => x.SuperServiceOrder.DisplayId).FirstOrDefault();
            string username = GetCurrentUserEmail();

            var superServiceOrderInfo = _dbContext.SuperServiceOrder
                .Where(sso => sso.Id == superServiceOrderId)
                .Include(sso => sso.Job)
                .ThenInclude(j => j.AccountEntity)
                .Include(sso => sso.Transferee)
                .FirstOrDefault();

            string transfereeLastFirstName = String.Concat(superServiceOrderInfo.Transferee.LastName, ", ", superServiceOrderInfo.Transferee.FirstName);

            List<GetPostableResponse> response = new List<GetPostableResponse>();
            payableItems.ToList().ForEach(pi =>
            {
                var receivableOrPayable = "Payable";
                var accrualOrActual = "Accrual";
                var branchName = pi.SuperServiceOrder.Job?.BranchName;
                var revenueType = pi.SuperServiceOrder.Job?.RevenueType;
                var accountCode = pi.BillableItemType.AccountCode;
                var billFromType = pi.BillFromType.ToUpper();
                int itemBillFromId;

                string accountingId = GetAccountingIdByPayableItem(pi);

                if (string.IsNullOrEmpty(accountingId))
                {
                    throw new Exception("AccountingId for BillFrom cannot be null");
                }

                switch (billFromType)
                {
                    case EntityType.ACCOUNT_ENTITY:
                        itemBillFromId = pi.BillFromAccountEntityId.Value;
                        break;

                    case EntityType.VENDOR:
                        itemBillFromId = pi.BillFromVendorId.Value;
                        break;

                    case EntityType.TRANSFEREE:
                        itemBillFromId = pi.BillFromTransfereeId.Value;
                        break;

                    default:
                        throw new Exception("Invalid BillFromType");
                }

                //the ledger (PAYABLE) GLCode
                string payableGLCode = _accountingService.GetGLCodeAsync(accrualOrActual, receivableOrPayable, branchName, revenueType, GPLedgerAccountCode.PAYABLE_ACCOUNT_CODE, GPDistType.PAYABLE).Result;

                if (string.IsNullOrEmpty(payableGLCode))
                {
                    _logger.LogError($"No payable GL account number found for payable with id {pi.Id}.");
                    throw new Exception("No payable GL account number found.");
                }

                //the item (PURCHASE) GLCode
                string purchaseGLCode = _accountingService.GetGLCodeAsync(accrualOrActual, receivableOrPayable, branchName, revenueType, accountCode, GPDistType.PURCHASE).Result;
                if (string.IsNullOrEmpty(purchaseGLCode))
                {
                    _logger.LogError($"No purchase GL account number found for payable with id {pi.Id}.");
                    throw new Exception("No purchase GL account number found.");
                }

                string gPDocType;

                if (pi.AccrualAmountUSD < 0)
                {
                    gPDocType = GPDocType.PAYABLE_CREDIT_MEMO.ToString();
                }
                else
                {
                    gPDocType = GPDocType.INVOICE.ToString();
                }

                response.Add(
                    new GetPostableResponse
                    {
                        SuperServiceOrderId = pi.SuperServiceOrder.Id,
                        DisplayId = pi.SuperServiceOrder.DisplayId,
                        ActualPackEndDate = actualPackEndDate,
                        EntityType = billFromType,
                        EntityId = itemBillFromId,

                        Header = new AccruableAPHeader
                        {
                            RecordType = GPRecordType.AP_HEADER,
                            ActualVendorId = accountingId,
                            VendorId = GPAccrualEntityId.VENDOR_ID_PAYABLE_ACCRUAL,
                            PayableItemId = pi.Id,
                            DocAmt = Math.Abs(pi.AccrualAmountUSD.Value),
                            DocNum = pi.GPDocNum,
                            DocType = gPDocType,
                            Desc = string.Concat(pi.SuperServiceOrder.DisplayId, "/", transfereeLastFirstName).Substring(0, Math.Min(String.Concat(pi.SuperServiceOrder.DisplayId, "/", transfereeLastFirstName).Length, 29)),
                            DocDate = DateTime.UtcNow,
                            PostingDate = pi.AccrualFinancialPeriodDateTime ?? DateTime.MinValue,
                            PostingUserName = username,
                            RegNumber = superServiceOrderDisplayId,
                            ShipperName = transfereeLastFirstName,
                            ImportDescription = $"AP Header PayableItem #{pi.Id}",
                            ImportStatus = 0,
                        },
                        LineItems = new List<PostableLineItem>
                        {
                            new PostableLineItem
                            {
                                RecordType = GPRecordType.AP_DETAIL,
                                VendorId = GPAccrualEntityId.VENDOR_ID_PAYABLE_ACCRUAL,
                                DistAmt = -1*pi.AccrualAmountUSD.Value,
                                DocType = gPDocType,
                                DistType = GPDistType.PAYABLE,
                                DocNum = pi.GPDocNum,
                                PostingUserName = username,
                                MovesId = superServiceOrderDisplayId,
                                Sequence = GPSequence.MULTIPLIER,
                                ItemCode = GPLedgerAccountCode.PAYABLE_ACCOUNT_CODE,
                                ImportDescription = $"AP Detail (Payable) PayableItem #{pi.Id}-Detail 1 of 2",
                                ImportStatus = 0,
                                ShipperName = transfereeLastFirstName,
                                RegNumber = superServiceOrderDisplayId,
                                AcctNum = payableGLCode,
                            },
                            new PostableLineItem
                            {
                                RecordType = GPRecordType.AP_DETAIL,
                                VendorId = GPAccrualEntityId.VENDOR_ID_PAYABLE_ACCRUAL,
                                DistAmt = pi.AccrualAmountUSD.Value,
                                DocType = gPDocType,
                                DistType = GPDistType.PURCHASE,
                                DocNum = pi.GPDocNum,
                                PostingUserName = username,
                                MovesId = superServiceOrderDisplayId,
                                Sequence = 2*GPSequence.MULTIPLIER,
                                ItemCode = pi.BillableItemType.AccountCode,
                                ImportDescription = $"AP Detail (Purchase) PayableItem #{pi.Id}-Detail 2 of 2",
                                ImportStatus = 0,
                                ShipperName = transfereeLastFirstName,
                                RegNumber = superServiceOrderDisplayId,
                                AcctNum = purchaseGLCode,
                            },
                        }
                    }
                );
            });

            return response;
        }

        private string GetAccountingIdByPayableItem(PayableItem payableItem)
        {
            string accountingId;
            string billFromType = payableItem.BillFromType?.ToUpper();

            if (string.IsNullOrEmpty(billFromType))
            {
                throw new Exception("PayableItem is missing BillFromType");
            }

            switch (billFromType)
            {
                case EntityType.ACCOUNT_ENTITY:

                    accountingId = payableItem.BillFromAccountEntity.AccountingId;
                    break;

                case EntityType.VENDOR:

                    accountingId = payableItem.BillFromVendor.AccountingId;
                    break;

                case EntityType.TRANSFEREE:

                    accountingId = payableItem.BillFromTransferee.AccountingId.ToString();
                    break;

                default:
                    throw new Exception($"Invalid BillFromType: {billFromType}");
            }

            return accountingId;
        }

        #endregion privates
    }
}