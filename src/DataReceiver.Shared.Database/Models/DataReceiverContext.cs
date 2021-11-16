using System;
using MessagePublisher.Shared.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DataReceiver.Shared.Database
{
    public partial class DataReceiverContext : DbContext
    {
        public DataReceiverContext()
        {
        }

        public DataReceiverContext(DbContextOptions<DataReceiverContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Combination> Combinations { get; set; }
        public virtual DbSet<Pool> Pools { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Combination>(entity =>
            {
                entity.ToTable("combination");

                entity.HasIndex(e => new { e.pool_id, e.combination_id }, "combination_with_pool_id");

                entity.Property(e => e.id);

                entity.Property(e => e.combination_id);

                entity.Property(e => e.create_time)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.investment_number);

                entity.Property(e => e.liability)
                    .HasColumnType("decimal(12, 2)");

                entity.Property(e => e.odds)
                    .HasColumnType("decimal(6, 2)");

                entity.Property(e => e.odds_number);

                entity.Property(e => e.pool_id);

                entity.Property(e => e.sales)
                    .HasColumnType("decimal(12, 2)");
            });

            modelBuilder.Entity<Pool>(entity =>
            {
                entity.ToTable("pool");

                entity.Property(e => e.id)
                    .ValueGeneratedNever();

                entity.Property(e => e.create_time)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.game_id);

                entity.Property(e => e.instance_name)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                var config = ConfigurationExtractor.Instance.Config;
                string connectionString = config["DataReceiverContext"];
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
