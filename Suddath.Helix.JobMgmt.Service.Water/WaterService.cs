using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Interfaces;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Suddath.Helix.JobMgmt.Services.Water.Mapper;

namespace Suddath.Helix.JobMgmt.Services.Water
{
    public class WaterService : ILegacyMoveService
    {
        private readonly WaterDbContext _dbContext;
        public WaterService(WaterDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
        }

        public MoveIdPrefix IdPrefix => MoveIdPrefix.GMMS_SI;

        public SystemHint[] Hints => new SystemHint[] { SystemHint.US_International_SterlingMoves };

        public async Task<ServiceDto> GetMoveFromIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            id = id.ToUpper().Trim().Replace(IdPrefix.Name,string.Empty);

            var move = await GetMoveByRegNumber(id);

            return move?.ToModel();
        }
        public async Task<ServiceDto> GetMoveFromOrderNumberAsync(string order)
        {
            var move = await GetMoveByRegNumber(order);
            return move?.ToModel();
        }

        #region My Privates
        private async Task<Move> GetMoveByRegNumber(string reg)
        {
            var move = await _dbContext.Moves
                   .Include(v => v.Profile)
                   .Include(v => v.Account)
                   .Include(v => v.Names)
                   .Include(v => v.MoveItems)
                   .Include(v => v.MoveAgents)
                       .ThenInclude(v => v.Name)                
                   .AsNoTracking()
                   .FirstOrDefaultAsync(v => v.RegNumber == reg);

            if (move != null)
            {
                move.MoveTrackings = await _dbContext.MoveTrackings  //This is a view so we can't include in the above query
                  .Where(v => v.Id == move.RegNumber
                  && !v.Description.ToUpper().Equals("MOVE REGISTERED")) //HACK alert: added per requirement for USER STORY 13647
                  .AsNoTracking()
                  .ToListAsync();

                if (!string.IsNullOrEmpty(move.MOVE_MANAGER))
                {
                    var mm = _dbContext.Move_Managers.FirstOrDefault(m => m.USERNAME == move.MOVE_MANAGER);
                    move.MOVE_MANAGER = mm?.REAL_NAME;
                    move.MOVE_MANAGER_EMAIL = mm?.EMAIL;
                    move.MOVE_MANAGER_PHONE = mm?.PHONE;
                }
            }

            return move;
        }
        #endregion
    }
}
