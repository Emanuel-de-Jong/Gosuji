using Gosuji.Client.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole, string, IdentityUserClaim<string>, UserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options)
    {
        public DbSet<SettingConfig> SettingConfigs { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(e => e.CurrentSubscription)
                .WithMany()
                .HasForeignKey(e => e.CurrentSubscriptionId);

            builder.Entity<SettingConfig>();
            builder.Entity<UserSubscription>();
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
        }
    }
}
