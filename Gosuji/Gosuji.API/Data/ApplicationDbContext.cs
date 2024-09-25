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
        public DbSet<PendingUserChange> PendingUserChanges { get; set; }
        public DbSet<RateLimitViolation> RateLimitViolations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserMoveCount> UserMoveCounts { get; set; }
        public DbSet<Changelog> Changelogs { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<EncodedGameData> EncodedGameDatas { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameStat> GameStats { get; set; }
        public DbSet<KataGoVersion> KataGoVersions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Preset> Presets { get; set; }
        public DbSet<SettingConfig> SettingConfigs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<TrainerSettingConfig> TrainerSettingConfigs { get; set; }
        public DbSet<UserState> UserStates { get; set; }

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PendingUserChange>()
                .HasOne(puc => puc.User)
                .WithOne()
                .HasForeignKey<PendingUserChange>(puc => puc.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<User>()
                .HasOne(u => u.CurrentSubscription)
                .WithMany()
                .HasForeignKey(u => u.CurrentSubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserMoveCount>()
                .HasOne(umc => umc.User)
                .WithMany()
                .HasForeignKey(umc => umc.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<EncodedGameData>()
                .HasOne<Game>()
                .WithOne(g => g.EncodedGameData)
                .HasForeignKey<EncodedGameData>(egd => egd.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Feedback>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(fb => fb.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Game>()
                .HasOne(g => g.TrainerSettingConfig)
                .WithMany()
                .HasForeignKey(g => g.TrainerSettingConfigId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Game>()
                .HasOne(g => g.KataGoVersion)
                .WithMany()
                .HasForeignKey(g => g.KataGoVersionId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Game>()
                .HasOne(g => g.GameStat)
                .WithMany()
                .HasForeignKey(g => g.GameStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Game>()
                .HasOne(g => g.OpeningStat)
                .WithMany()
                .HasForeignKey(g => g.OpeningStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Game>()
                .HasOne(g => g.MidgameStat)
                .WithMany()
                .HasForeignKey(g => g.MidgameStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Game>()
                .HasOne(g => g.EndgameStat)
                .WithMany()
                .HasForeignKey(g => g.EndgameStatId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Preset>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Preset>()
                .HasOne(p => p.TrainerSettingConfig)
                .WithMany()
                .HasForeignKey(p => p.TrainerSettingConfigId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<SettingConfig>()
                .HasOne(sc => sc.Language)
                .WithMany()
                .HasForeignKey(sc => sc.LanguageId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Subscription>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Subscription>()
                .HasOne(s => s.Discount)
                .WithMany()
                .HasForeignKey(s => s.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<TrainerSettingConfig>()
                .HasIndex(e => e.Hash)
                .IsUnique();

            builder.Entity<UserState>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserState>(us => us.Id)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<UserState>()
                .HasOne(us => us.LastPreset)
                .WithMany()
                .HasForeignKey(us => us.LastPresetId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
