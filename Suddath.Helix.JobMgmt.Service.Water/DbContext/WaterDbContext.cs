using Microsoft.EntityFrameworkCore;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    public class WaterDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public virtual DbSet<Name> Names { get; set; }
        public virtual DbQuery<MoveTracking> MoveTrackings { get; set; }
        public virtual DbSet<Move> Moves { get; set; }
        public virtual DbSet<MoveItem> MoveItems { get; set; }
        public virtual DbSet<MoveAgent> MoveAgents { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Notes> Notes { get; set; }
        public virtual DbSet<PaymentSent> PaymentSent { get; set; }
        public virtual DbSet<PaymentReceived> PaymentReceived { get; set; }
        public virtual DbSet<InsuranceClaims> InsuranceClaims { get; set; }

        public virtual DbSet<MoveInformation> MoveInformation { get; set; }

        public DbQuery<Move_Managers> Move_Managers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle(@"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=watersandbox.sudco.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL9i)));User Id=comapp_user;Password=comapp7457;Validate Connection=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("BINGO");

            modelBuilder.Entity<MoveAgent>()
                .HasKey(ma => new { ma.Id, ma.VendorNameId, ma.JobCategory });

            modelBuilder.Entity<Move>()
                .HasOne(x => x.Account)
                .WithOne()
                .HasForeignKey<Move>(m => m.AccountId);

            modelBuilder.Entity<Move>()
                .HasMany(x => x.Names)
                .WithOne(x => x.Move)
                .HasForeignKey(n => n.ShipperMoveId);

            modelBuilder.Entity<Move>()
                .HasOne(x => x.Profile)
                .WithOne()
                .HasForeignKey<Move>(m => m.AccountId);

            modelBuilder.Entity<PaymentSent>()
                .HasKey(ps => new { ps.MOVES_ID, ps.NAMES_ID });

            modelBuilder.Entity<PaymentReceived>()
                .HasKey(ps => new { ps.MOVES_ID, ps.NAMES_ID });
        }
    }
}