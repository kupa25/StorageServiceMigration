﻿using Microsoft.EntityFrameworkCore;
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
            Console.WriteLine("-----------------------------------------------------------------------------------");
            Trace.WriteLine("-----------------------------------------------------------------------------------");
            Trace.WriteLine($"StartTime: {DateTime.Now}");

            Console.WriteLine($"Retrieving Legacy move {regNumber}");
            Trace.WriteLine($"Retrieving Legacy move {regNumber}");
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
                Trace.WriteLine($"No moves retrieved for {regNumber}" + ex);
            }

            return null;
        }

        internal static async Task<List<Notes>> RetrieveNotesForMove(string regNumber)
        {
            Console.WriteLine($"Retrieving Notes for {regNumber}");
            Trace.WriteLine($"Retrieving Notes for {regNumber}");
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
            Trace.WriteLine($"Retrieving Payment_send for {regNumber}");

            try
            {
                using (var context = new WaterDbContext())
                {
                    var paymentSends = await context.PaymentSent.AsNoTracking()
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

        internal static async Task<List<PaymentReceived>> RetrieveJobCostRevenue(string regNumber)
        {
            Console.WriteLine($"Retrieving Payment_received for {regNumber}");
            Trace.WriteLine($"Retrieving Payment_received for {regNumber}");

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
    }
}