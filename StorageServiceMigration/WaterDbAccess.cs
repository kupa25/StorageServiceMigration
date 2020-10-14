using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Suddath.Helix.JobMgmt.Services.Water.Mapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class WaterDbAccess
    {
        public static async Task<Move> RetrieveWaterRecords(string regNumber)
        {
            Trace.WriteLine($"{regNumber},  StartTime: {DateTime.Now}");

            Console.WriteLine($"Retrieving Legacy move {regNumber}");
            Trace.WriteLine($"{regNumber}, Retrieving Legacy move {regNumber}");
            try
            {
                using (var context = new WaterDbContext())
                {
                    var moves = await context.Moves
                   .Include(v => v.Profile)
                   .Include(v => v.Account)
                   .Include(v => v.Names)
                   .Include(v => v.MoveItems)
                   .Include(v => v.MoveAgents)
                       .ThenInclude(v => v.Name)
                   .AsNoTracking()
                   .Where(m => m.RegNumber == regNumber).SingleAsync();

                    return moves;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No moves retrieved for {regNumber}", ex);
                Trace.WriteLine($"{regNumber}, No moves retrieved for {regNumber}");
            }

            return null;
        }

        internal static async Task<List<Notes>> RetrieveNotesForMove(string regNumber)
        {
            Console.WriteLine($"Retrieving Notes for {regNumber}");
            Trace.WriteLine($"{regNumber}, Retrieving Notes for {regNumber}");
            try
            {
                using (var context = new WaterDbContext())
                {
                    var notes = await context.Notes.AsNoTracking()
                   .Where(n => n.TABLE_ID == regNumber && !n.TABLE_NAME.Equals("PROMPTS")).ToListAsync();

                    var result = notes.Where(n => !string.IsNullOrEmpty(n.NOTE)).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }

            return null;
        }

        internal static async Task<List<PaymentSent>> RetrieveJobCostExpense(string regNumber)
        {
            Console.WriteLine($"Retrieving Payment_send for {regNumber}");
            Trace.WriteLine($"{regNumber}, Retrieving Payment_send for {regNumber}");

            try
            {
                using (var context = new WaterDbContext())
                {
                    // open accrual records

                    var openAccrualRecords = await context.PaymentSent.AsNoTracking()
                   .Where(n => n.MOVES_ID == regNumber &&
                          (
                            (n.DATE_BILLED == null && n.ESTIMATED_AMOUNT != null) ||
                            (n.DATE_BILLED == null && n.ADJ_EST_AMOUNT != null)
                          ))
                   .ToListAsync();

                    var regularRecords = await context.PaymentSent.AsNoTracking()
                   .Where(n => n.MOVES_ID == regNumber && n.DATE_BILLED != null)
                   .OrderByDescending(n => n.DATE_BILLED)
                   .Take(4)
                   .ToListAsync();

                    var paymentSends = openAccrualRecords.Union(regularRecords).ToList();

                    return paymentSends;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }

            return null;
        }

        internal static async Task<List<InsuranceClaims>> RetrieveInsuranceClaims(string regNumber)
        {
            Console.WriteLine($"Retrieving Insurance & Claims for {regNumber}");
            Trace.WriteLine($"{regNumber}, Retrieving Insurance & Claims for {regNumber}");
            try
            {
                using (var context = new WaterDbContext())
                {
                    var insurance = await context.InsuranceClaims.AsNoTracking()
                   .Where(n => n.MOVES_ID == regNumber)
                   .ToListAsync();

                    return insurance;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }

            return null;
        }

        internal static async Task<List<PaymentReceived>> RetrieveJobCostRevenue(string regNumber)
        {
            Console.WriteLine($"Retrieving Payment_received for {regNumber}");
            Trace.WriteLine($"{regNumber}, Retrieving Payment_received for {regNumber}");

            try
            {
                using (var context = new WaterDbContext())
                {
                    var paymentSends = await context.PaymentReceived.AsNoTracking()
                   .Where(n => n.MOVES_ID == regNumber)
                   .OrderByDescending(n => n.DATE_BILLED)
                   .Take(4).
                   ToListAsync();

                    return paymentSends;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }

            return null;
        }

        internal static async Task<Name> GetNames(string names_id)
        {
            try
            {
                using (var context = new WaterDbContext())
                {
                    var name = await context.Names.AsNoTracking()
                   .SingleOrDefaultAsync(n => n.Id == names_id);

                    return name;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Trace.WriteLine(ex);
            }

            return null;
        }
    }
}