﻿using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Suddath.Helix.JobMgmt.Services.Water.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageServiceMigration
{
    public static class WaterDbAccess
    {
        public static async Task<List<Move>> RetrieveWaterRecords()
        {
            Console.WriteLine("Retrieving Legacy moves");
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
                   .Where(m => m.RegNumber == "255950").ToListAsync();

                    return moves;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        internal static async Task<List<CreateJobNoteRequest>> RetrieveNotesForMove(string regNumber)
        {
            Console.WriteLine($"Retrieving Notes for {regNumber}");
            try
            {
                using (var context = new WaterDbContext())
                {
                    var notes = await context.Notes.AsNoTracking()
                   .Where(n => n.TABLE_ID == regNumber && n.CATEGORY == null).ToListAsync();

                    return notes.ToNotesModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}