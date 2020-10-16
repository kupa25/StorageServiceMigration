﻿using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Infrastructure;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Services;
using Suddath.Helix.JobMgmt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class JobsDbAccess
    {
        public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;;database=Jobs;trusted_connection=yes;";
        //public static string connectionString = @"data source=daue2helix3sql01.database.windows.net;initial catalog=Helix3.Jobs;User ID=helix3_app;Password=CHEjSEK7qMHdt7!; Connect Timeout=120;MultipleActiveResultSets=True;";

        private static List<BillableItemType> _billableItemTypes;

        public static void ChangeDateCreated(int jobId, DateTime date, string regNumber)
        {
            Console.WriteLine($"Updating Jobs Created Date to {date}");
            Trace.WriteLine($"{regNumber}, Updating Jobs Created Date to {date}");
            using (var context = new JobDbContext(connectionString))
            {
                var createdJob = context.Job.Find(jobId);

                createdJob.DateCreated = date;
                context.SaveChanges();
            }
        }

        internal static async Task<List<ServiceOrder>> GetServiceOrderForJobs(int jobId, string regNumber)
        {
            Console.WriteLine($"Retrieve all the ServiceOrders Created");
            Trace.WriteLine($"{regNumber}, Retrieve all the ServiceOrders Created");

            List<ServiceOrder> result;
            using (var context = new JobDbContext(connectionString))
            {
                result = await context.ServiceOrder.AsNoTracking()
                                                 .Include(so => so.SuperServiceOrder)
                                                 .Where(so => so.SuperServiceOrder.JobId == jobId).ToListAsync();
            }

            return result;
        }

        internal static async Task<Transferee> GetJobsTransfereeId(int jobId)
        {
            Transferee transfereeEntity;

            using (var context = new JobDbContext(connectionString))
            {
                transfereeEntity = context.Job.AsNoTracking().Include(j => j.Transferee)
                                              .Single(j => j.Id == jobId)
                                              .Transferee;
            }

            return transfereeEntity;
        }

        internal static void ChangeDisplayName(int ssoId, string regNumber)
        {
            Console.WriteLine($"Changing Storage DisplayName to {regNumber}");
            Trace.WriteLine($"{regNumber}, Changing Storage DisplayName to {regNumber}");

            using (var context = new JobDbContext(connectionString))
            {
                var createdJob = context.SuperServiceOrder.Find(ssoId);

                createdJob.DisplayId = regNumber;
                context.SaveChanges();
            }
        }

        internal static async Task<List<BillableItemType>> RetrieveBillableItemTypes(string regNumber)
        {
            Console.WriteLine($"Retrieve JC BillableItemTypes");
            Trace.WriteLine($"{regNumber}, Retrieve JC BillableItemTypes");

            if (_billableItemTypes == null || _billableItemTypes.Count == 0)
            {
                using (var context = new JobDbContext(connectionString))
                {
                    _billableItemTypes = await context.BillableItemType.AsNoTracking().Where(bi => bi.IsActive.Value).ToListAsync();
                }
            }

            return _billableItemTypes;
        }

        internal static async Task CreateVendorInvoiceRecord(int id, string regNumber, string cHECK, string iNVOICE_NUMBER, DateTime? dATE_PAID)
        {
            Console.WriteLine($"Adding Vendor Invoice Record for {regNumber}");
            Trace.WriteLine($"{regNumber}, Adding Vendor Invoice Record for ");

            using (var context = new JobDbContext(connectionString))
            {
                //var vendorInvoice = new VendorInvoice
                //{
                //}

                //pitem.PayableItemStatusIdentifier = status;
                //context.SaveChanges();
            }
        }

        internal static async Task MarkAllAsVoid(int superServiceOrderId, string regNumber)
        {
            Console.WriteLine($"Marking JC records as void");
            Trace.WriteLine($"{regNumber}, Marking JC void ");

            try
            {
                using (var context = new JobDbContext(connectionString))
                {
                    var bitem = context.BillableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId).ToList();
                    var pitem = context.PayableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId).ToList();

                    bitem.ForEach(i => i.BillableItemStatusIdentifier = BillableItemStatusIdentifier.VOID);
                    pitem.ForEach(i =>
                    {
                        i.PayableItemStatusIdentifier = PayableItemStatusIdentifier.VOID;
                        i.VoidedBy = GetCurrentUserEmail();
                        i.VoidedDateTime = DateTime.UtcNow;
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while marking items void");
                Trace.WriteLine($"{regNumber}, Error while marking items void");
            }
        }

        internal static async Task MarkAsPosted(int superServiceOrderId, DateTime accrualFinancialPeriodDateTime, bool isOriginalAccrual, string regNumber)
        {
            Console.WriteLine($"Marking JC as posted");
            Trace.WriteLine($"{regNumber}, Marking JC as posted ");

            try
            {
                using (var _dbContext = new JobDbContext(connectionString))
                {
                    var dateStamp = DateTime.UtcNow;
                    string currentUser = GetCurrentUserEmail();

                    //Get list of billables that are pending accrual to validate the list we received
                    var itemsToMark = await _dbContext.BillableItem.Where(bi => bi.SuperServiceOrderId == superServiceOrderId && bi.BillableItemStatusIdentifier == BillableItemStatusIdentifier.ACCRUAL_PENDING).ToListAsync();

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
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static string GetCurrentUserEmail()
        {
            return "MigrationScript @test.com";
        }

        internal static async Task LockJC(int jobId, string regNumber, int superServiceOrderId)
        {
            Console.WriteLine($"Locking JC");
            Trace.WriteLine($"{regNumber}, Locking JC ");

            try
            {
                using (var _dbContext = new JobDbContext(connectionString))
                {
                    //BILLABLE ITEM
                    var dateStamp = DateTime.UtcNow;
                    string currentUser = "MigrationScript@test.com";

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

                    superServiceOrder.ModifiedBy = "MigrationScript@test.com";
                    superServiceOrder.DateModified = dateStamp;

                    await _dbContext.SaveChangesAsync();

                    //PAYBLE ITEM
                    var pitems = await _dbContext.PayableItem.Where(pi => pi.SuperServiceOrderId == superServiceOrderId && pi.PayableItemStatusIdentifier == PayableItemStatusIdentifier.QUEUED).ToListAsync();

                    pitems.ForEach(item =>
                    {
                        item.PayableItemStatusIdentifier = PayableItemStatusIdentifier.ACCRUAL_PENDING;
                        item.DateModified = dateStamp;
                        item.ModifiedBy = currentUser;
                    });

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While trying to lock");
                Trace.WriteLine($"{regNumber}, Error while trying to lock");
                Trace.WriteLine($"{regNumber}, {ex.Message}");
            }
        }
    }
}