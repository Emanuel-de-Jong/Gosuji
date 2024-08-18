using Gosuji.Client.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole, string, IdentityUserClaim<string>, UserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options)
    {
        public DbSet<SettingConfig> SettingConfigs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserMoveCount> UserMoveCounts { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<TrainerSettingConfig> TrainerSettingConfigs { get; set; }
        public DbSet<KataGoVersion> KataGoVersions { get; set; }
        public DbSet<Preset> Presets { get; set; }
        public DbSet<GameStat> GameStats { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Changelog> Changelogs { get; set; }
        public DbSet<RateLimitViolation> RateLimitViolations { get; set; }
        public DbSet<UserState> UserStates { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PendingUserChange> PendingUserChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(e => e.CurrentSubscription)
                .WithMany()
                .HasForeignKey(e => e.CurrentSubscriptionId);

            builder.Entity<SettingConfig>();
            builder.Entity<Subscription>();
            builder.Entity<Discount>();

            builder.Entity<UserActivity>(b =>
            {
                b.ToTable("UserActivities");
            });

            builder.Entity<UserMoveCount>();

            builder.Entity<Game>()
                .HasOne(e => e.GameStat)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Game>()
                .HasOne(e => e.OpeningStat)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Game>()
                .HasOne(e => e.MidgameStat)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Game>()
                .HasOne(e => e.EndgameStat)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TrainerSettingConfig>()
                .HasIndex(e => e.Hash)
                .IsUnique();

            builder.Entity<KataGoVersion>();
            builder.Entity<Preset>();
            builder.Entity<GameStat>();
            builder.Entity<Feedback>();
            builder.Entity<Language>();
            builder.Entity<Changelog>();
            builder.Entity<RateLimitViolation>();

            builder.Entity<UserState>()
                .HasOne(e => e.LastPreset)
                .WithMany()
                .HasForeignKey(e => e.LastPresetId);

            builder.Entity<RefreshToken>();

            builder.Entity<PendingUserChange>()
                .HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<PendingUserChange>(e => e.Id);
        }

        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            IDbModel dbModel = entity as IDbModel;
            dbModel.ModifyDate = DateTimeOffset.UtcNow;
            entity = dbModel as TEntity;
            return base.Update(entity);
        }

        //public override int SaveChanges()
        //{
        //    Validate();
        //    return base.SaveChanges();
        //}

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    Validate();
        //    return base.SaveChangesAsync(cancellationToken);
        //}

        private void Validate()
        {
            IEnumerable<object> entries = ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified)
                .Select(e => e.Entity);

            foreach (object? entity in entries)
            {
                ValidationContext validationContext = new(entity);
                Validator.ValidateObject(entity, validationContext, validateAllProperties: true);
            }
        }
    }
}
