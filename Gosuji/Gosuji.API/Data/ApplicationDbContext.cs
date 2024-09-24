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

            builder.Entity<TrainerSettingConfig>()
                .HasIndex(e => e.Hash)
                .IsUnique();

            builder.Entity<PendingUserChange>()
                .HasOne(puc => puc.User)
                .WithOne()
                .HasForeignKey<PendingUserChange>(puc => puc.Id)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                .HasOne<PendingUserChange>()
                .WithOne(puc => puc.User)
                .HasForeignKey<PendingUserChange>(puc => puc.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                .HasMany<RefreshToken>()
                .WithOne()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<User>()
                .HasMany<UserActivity>()
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserMoveCount>()
                .HasOne(umc => umc.User)
                .WithMany()
                .HasForeignKey(umc => umc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                .HasMany<UserMoveCount>()
                .WithOne(umc => umc.User)
                .HasForeignKey(umc => umc.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Feedback>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(fb => fb.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<User>()
                .HasMany<Feedback>()
                .WithOne()
                .HasForeignKey(fb => fb.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Game>()
                .HasOne(g => g.EncodedGameData)
                .WithOne()
                .HasForeignKey<EncodedGameData>(egd => egd.Id)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<EncodedGameData>()
                .HasOne<Game>()
                .WithOne(g => g.EncodedGameData)
                .HasForeignKey<EncodedGameData>(egd => egd.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
                .HasOne(g => g.TrainerSettingConfig)
                .WithMany()
                .HasForeignKey(g => g.TrainerSettingConfigId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<TrainerSettingConfig>()
                .HasMany<Game>()
                .WithOne(g => g.TrainerSettingConfig)
                .HasForeignKey(g => g.TrainerSettingConfigId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Game>()
                .HasOne(g => g.KataGoVersion)
                .WithMany()
                .HasForeignKey(g => g.KataGoVersionId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<KataGoVersion>()
                .HasMany<Game>()
                .WithOne(g => g.KataGoVersion)
                .HasForeignKey(g => g.KataGoVersionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Game>()
                .HasOne(g => g.GameStat)
                .WithMany()
                .HasForeignKey(g => g.GameStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<GameStat>()
                .HasMany<Game>()
                .WithOne(g => g.GameStat)
                .HasForeignKey(g => g.GameStatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
                .HasOne(g => g.OpeningStat)
                .WithMany()
                .HasForeignKey(g => g.OpeningStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<GameStat>()
                .HasMany<Game>()
                .WithOne(g => g.OpeningStat)
                .HasForeignKey(g => g.OpeningStatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
                .HasOne(g => g.MidgameStat)
                .WithMany()
                .HasForeignKey(g => g.MidgameStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<GameStat>()
                .HasMany<Game>()
                .WithOne(g => g.MidgameStat)
                .HasForeignKey(g => g.MidgameStatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Game>()
                .HasOne(g => g.EndgameStat)
                .WithMany()
                .HasForeignKey(g => g.EndgameStatId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<GameStat>()
                .HasMany<Game>()
                .WithOne(g => g.EndgameStat)
                .HasForeignKey(g => g.EndgameStatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Preset>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                .HasMany<Preset>()
                .WithOne()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Preset>()
                .HasOne(p => p.TrainerSettingConfig)
                .WithMany()
                .HasForeignKey(p => p.TrainerSettingConfigId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TrainerSettingConfig>()
                .HasMany<Preset>()
                .WithOne(p => p.TrainerSettingConfig)
                .HasForeignKey(p => p.TrainerSettingConfigId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<SettingConfig>()
                .HasOne(sc => sc.Language)
                .WithMany()
                .HasForeignKey(sc => sc.LanguageId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Language>()
                .HasMany<SettingConfig>()
                .WithOne(sc => sc.Language)
                .HasForeignKey(sc => sc.LanguageId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Subscription>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                .HasMany<Subscription>()
                .WithOne()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<User>()
                .HasOne(u => u.CurrentSubscription)
                .WithMany()
                .HasForeignKey(u => u.CurrentSubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Subscription>()
                .HasOne(s => s.Discount)
                .WithMany()
                .HasForeignKey(s => s.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Discount>()
                .HasMany<Subscription>()
                .WithOne(s => s.Discount)
                .HasForeignKey(s => s.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserState>()
                .HasOne(us => us.LastPreset)
                .WithMany()
                .HasForeignKey(us => us.LastPresetId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Preset>()
                .HasMany<UserState>()
                .WithOne(us => us.LastPreset)
                .HasForeignKey(us => us.LastPresetId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserState>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserState>(us => us.Id)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                .HasOne<UserState>()
                .WithOne()
                .HasForeignKey<UserState>(us => us.Id)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
