using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using OnlineMarket.Domain.Models;

namespace OnlineMarket.DAL
{
    public sealed class MarketDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public DbSet<Audit> Audits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductsOrders { get; set; }
        public DbSet<Promocode> Promocodes { get; set; }

        public MarketDbContext(DbContextOptions<MarketDbContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("shop");

            modelBuilder
                .UseSerialColumns()
                .HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Audit>(entity =>
            {
                entity.ToTable("audit");

                entity.HasKey(e => e.Id).HasName("audit_pk");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id").IsRequired();
                entity.Property(e => e.TableName).HasColumnName("table_name").IsRequired();
                entity.Property(e => e.ColumnName).HasColumnName("column_name").IsRequired();
                entity.Property(e => e.OldValue).HasColumnName("old_value");
                entity.Property(e => e.NewValue).HasColumnName("new_value");
                entity.Property(e => e.Action).HasColumnName("action").HasMaxLength(10).IsRequired();
                entity.Property(e => e.Tmstamp).HasColumnName("tmstamp").IsRequired();
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();

                entity.HasOne(e => e.User).WithMany(e => e.Logs).HasForeignKey(e => e.UserId).HasConstraintName("audit_user_fk");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasKey(e => e.Id).HasName("user_pk");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired();
                entity.Property(e => e.SecondName).HasColumnName("second_name").IsRequired();
                entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired();
                entity.Property(e => e.Phone).HasColumnName("phone").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.Login).HasColumnName("login").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Password).HasColumnName("password").IsRequired();

                entity.HasAlternateKey(e => e.Login).HasName("user_login_uk");
                entity.HasAlternateKey(e => e.Email).HasName("user_email_uk");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product",
                    t =>
                    {
                        t.HasCheckConstraint("product_price_chk", "current_price > 0");
                    });

                entity.HasKey(e => e.Id).HasName("product_pk");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.CurrentPrice).HasColumnName("current_price").IsRequired();

                entity.HasAlternateKey(e => e.Name).HasName("product_name_uk");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order",
                    t =>
                    {
                        //TODO: разобраться, как сделать check универсальным (current_timestamp - функция Postgre)
                        t.HasCheckConstraint("order_deliver_by_chk", "deliver_by > current_timestamp");
                    });

                entity.HasKey(e => e.Id).HasName("order_pk");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
                entity.Property(e => e.CreateDate).HasColumnName("create_date").IsRequired();
                entity.Property(e => e.AddressToDeliver).HasColumnName("address_to_deliver").IsRequired();
                entity.Property(e => e.DeliverBy).HasColumnName("deliver_by").IsRequired();
                entity.Property(e => e.Status).HasColumnName("status").IsRequired();
                entity.Property(e => e.PromocodeId).HasColumnName("promocode_id");
                entity.Property(e => e.Comment).HasColumnName("comment").HasMaxLength(100);

                entity.HasOne(e => e.Promocode).WithMany(e => e.Orders).HasForeignKey(e => e.PromocodeId).HasConstraintName("order_promocode_fk");
                entity.HasOne(e => e.User).WithMany(e => e.Orders).HasForeignKey(e => e.UserId).HasConstraintName("order_user_fk").IsRequired();
            });

            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.ToTable("product_order", 
                    t =>
                    {
                        t.HasCheckConstraint("product_order_count_chk", "count > 0");
                        t.HasCheckConstraint("product_order_price_chk", "price > 0");
                    });

                entity.HasKey(e => e.Id).HasName("product_order_pk");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
                entity.Property(e => e.OrderId).HasColumnName("order_id").IsRequired();
                entity.Property(e => e.Count).HasColumnName("count").IsRequired();
                entity.Property(e => e.Price).HasColumnName("price").IsRequired();

                entity.HasOne(e => e.Order).WithMany(e => e.ProductsOrders).HasForeignKey(e => e.OrderId).HasConstraintName("po_order_fk").IsRequired();
                entity.HasOne(e => e.Product).WithMany(e => e.ProductsOrders).HasForeignKey(e => e.ProductId).HasConstraintName("po_product_fk").IsRequired();

                entity.HasAlternateKey(e => new { e.ProductId, e.OrderId }).HasName("product_order_uk");
            });

            modelBuilder.Entity<Promocode>(entity =>
            {
                entity.ToTable("promocode",
                    t =>
                    {
                        t.HasCheckConstraint("promocode_disc_perc_chk", "discount_percent > 0");
                        t.HasCheckConstraint("promocode_disc_sum_chk", "discount_sum > 0");
                        t.HasCheckConstraint("promocode_min_ord_cost_chk", "min_order_cost > 0");
                        t.HasCheckConstraint("promocode_act_dates_chk", "active_to > active_from");
                    });

                entity.HasKey(e => e.Id).HasName("promocode_pk");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.PromoCode).HasColumnName("promocode").IsRequired();
                entity.Property(e => e.DiscountPercent).HasColumnName("discount_percent");
                entity.Property(e => e.DiscountSum).HasColumnName("discount_sum");
                entity.Property(e => e.ActiveFrom).HasColumnName("active_from").IsRequired();
                entity.Property(e => e.ActiveTo).HasColumnName("active_to").IsRequired();
                entity.Property(e => e.MinimumOrderCost).HasColumnName("min_order_cost");

                entity.HasAlternateKey(e => e.PromoCode).HasName("promocode_promo_uk");
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<DateTime>().HaveConversion<Converters.DateTimeConverter>();
            configurationBuilder.Properties<DateTime?>().HaveConversion<Converters.DateTimeNullableConverter>();
        }

        #region логирование
        /// <summary>
        /// DEPRECATED! Use SaveChangesWithLogs(long userId) instead
        /// </summary>
        public new int SaveChanges()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DEPRECATED! Use SaveChangesWithLogs(long userId) instead
        /// </summary>
        public new int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DEPRECATED! Use SaveChangesWithLogsAsync(long userId) instead
        /// </summary>
        public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DEPRECATED! Use SaveChangesWithLogsAsync(long userId) instead
        /// </summary>
        public new Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private List<Audit> GetAuditsForEntity(EntityEntry entry, long userId, DateTime curDateTime, EntityState entityState)
        {
            var audits = new List<Audit>();

            var transactionId = curDateTime.Ticks;
            var tableName = entry.Metadata.GetTableName().ToUpper();
            var props = entry.CurrentValues.Properties;
            long entityId = 0;
            var primaryKeyFields = props.Where(pr => pr.IsPrimaryKey());
            if (primaryKeyFields.Count() == 1 && primaryKeyFields.First().ClrType.FullName.Contains("Int"))
                entityId = long.Parse(entry.OriginalValues[primaryKeyFields.First()].ToString());

            string action =
                entityState == EntityState.Modified ? "UPDT" :
                entityState == EntityState.Added ? "INSR" :
                entityState == EntityState.Deleted ? "DELD" :
                entityState.ToString();

            foreach (var p in props.Where(pr => !pr.IsPrimaryKey()))
            {
                var comparer = p.GetKeyValueComparer();
                var columnName = p.GetColumnBaseName().ToUpper();

                var oldVal = entry.OriginalValues[p];
                var newVal = entry.CurrentValues[p];
                if (!comparer.Equals(oldVal, newVal) || entityState == EntityState.Added)
                {
                    if (!p.ClrType.FullName.Contains("Byte[]"))
                    {
                        audits.Add(new Audit()
                        {
                            Action = action,
                            ColumnName = columnName,
                            TableName = tableName,
                            UserId = userId,
                            Tmstamp = curDateTime.ToUniversalTime(),
                            OldValue = entry.State == EntityState.Added ? null : oldVal == null ? string.Empty : oldVal.ToString(),
                            NewValue = newVal == null ? string.Empty : newVal.ToString(),
                            EntityId = entityId,
                            TransactionId = transactionId
                        });
                    }
                }
            }

            return audits;
        }

        private void SaveDataAndLogs(IEnumerable<EntityEntry> entries, long userId, DateTime curDateTime)
        {
            var addedEntries = entries.Where(e => e.State == EntityState.Added).ToList();

            foreach (var entry in entries.Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted))
                Audits.AddRange(GetAuditsForEntity(entry, userId, curDateTime, entry.State));

            //чтобы у добавленных сущностей появились айдишники
            base.SaveChanges();

            //состояние при сохранении поменялось на Unchanged, нужно указать EntityState.Added явно
            foreach (var entry in addedEntries)
                Audits.AddRange(GetAuditsForEntity(entry, userId, curDateTime, EntityState.Added));

            base.SaveChanges();
        }

        public int SaveChangesWithLogs(long userId)
        {
            List<string> excludeClasses = new List<string>()
            {
                typeof(Audit).Name
                //другие классы, для которых логировать не нужно
            };

            var changeTrackerEntries = ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
                && !excludeClasses.Contains(e.Entity.GetType().FullName.Substring(e.Entity.GetType().FullName.LastIndexOf('.') + 1)))
                .ToList();

            var curDateTime = DateTime.UtcNow;

            SaveDataAndLogs(changeTrackerEntries, userId, curDateTime);

            return changeTrackerEntries.Count;
        }

        private async Task SaveDataAndLogsAsync(IEnumerable<EntityEntry> entries, long userId, DateTime curDateTime)
        {
            var addedEntries = entries.Where(e => e.State == EntityState.Added).ToList();

            foreach (var entry in entries.Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted))
                await Audits.AddRangeAsync(GetAuditsForEntity(entry, userId, curDateTime, entry.State));

            //чтобы у добавленных сущностей появились айдишники
            await base.SaveChangesAsync();

            //состояние при сохранении поменялось на Unchanged, нужно указать EntityState.Added явно
            foreach (var entry in addedEntries)
                await Audits.AddRangeAsync(GetAuditsForEntity(entry, userId, curDateTime, EntityState.Added));

            await base.SaveChangesAsync();
        }

        public async Task<int> SaveChangesWithLogsAsync(long userId)
        {
            List<Type> excludeClasses = new List<Type>()
            {
                typeof(Audit)
            };

            var changeTrackerEntries = ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
                            && !excludeClasses.Select(t => t.Name)
                                .Contains(e.Entity.GetType().FullName.Substring(e.Entity.GetType().FullName.LastIndexOf('.') + 1)))
                .ToList();

            var curDateTime = DateTime.UtcNow;

            await SaveDataAndLogsAsync(changeTrackerEntries, userId, curDateTime);

            return changeTrackerEntries.Count;
        }
        #endregion
    }
}
