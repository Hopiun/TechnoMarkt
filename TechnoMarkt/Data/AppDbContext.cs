using TechnoMarkt.Models;
using TechnoMarkt.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TechnoMarkt.Data;

public partial class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Brand> Brands { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<City> Cities { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Discount> Discounts { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderLine> OrderLines { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    public DbSet<Review> Reviews { get; set; }

    public DbSet<Store> Stores { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Warehouse> Warehouses { get; set; }

    public DbSet<EventLog> EventLogs { get; set; }

    public DbSet<AppSetting> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brand__5E5A8E271BA017D4");

            entity.HasOne(d => d.Country).WithMany(p => p.Brands).HasConstraintName("FK_Brand_Country");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B47B2D90A1");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasConstraintName("FK_Category_Parent");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.СityId).HasName("PK__City__61D698BE5CCEFA82");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Client__BF21A42480BE8261");

            entity.Property(e => e.WalletBalance).HasDefaultValueSql("((0.00))");

            entity.HasOne<AppUser>().WithOne()
                .HasForeignKey<Client>(e => e.UserId)
                .HasConstraintName("FK_Client_AspNetUsers")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Country__7E8CD055985BE3F3");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF95A6EC9E6");

            entity.HasOne(d => d.Category).WithMany(p => p.Discounts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Discount_Category");

            entity.HasOne(d => d.Item).WithMany(p => p.Discounts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Discount_Item");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA8A5A707D5");

            entity.HasOne(d => d.Store).WithMany(p => p.Employees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Store");

            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasDefaultValue(EmployeeRole.Operator)
                .ValueGeneratedNever();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasDefaultValue(EmployeeStatus.Present)
                .ValueGeneratedNever();

            entity.HasOne<AppUser>().WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .HasConstraintName("FK_Employee_AspNetUsers")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Item__52020FDDA4392901");

            entity.HasOne(d => d.Brand).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_Brand");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_Category");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_Supplier");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__465962290218EFAB");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderTotal).HasDefaultValueSql("((0.00))");
            entity.Property(e => e.Status).HasConversion<string>().HasDefaultValue(OrderStatus.New);

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Client");

            entity.HasOne(d => d.Operator).WithMany(p => p.Orders).HasConstraintName("FK_Order_Operator");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Store");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => e.OrderLineId).HasName("PK__OrderLin__8F2B951FF85B7B4D");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderLines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLine_Item");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderLines).HasConstraintName("FK_OrderLine_Order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EAD3A18DE1");

            entity.Property(e => e.Date).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Method).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>().HasDefaultValue(PaymentStatus.Pending);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments).HasConstraintName("FK_Payment_Order");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments).HasConstraintName("FK_Payment_PaymentMethod");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__8A3EA9EB81A265C1");

            entity.Property(e => e.CardNumber).IsFixedLength();

            entity.HasOne(d => d.Client).WithMany(p => p.PaymentMethods).HasConstraintName("FK_PaymentMethod_Client");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__60883D9045B88A06");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK__Store__A2F2A30C54797B75");

            entity.HasOne(d => d.Admin).WithMany(p => p.Stores).HasConstraintName("FK_Store_Admin");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__6EE594E8D6393AB1");

            entity.HasOne(d => d.Country).WithMany(p => p.Suppliers).HasConstraintName("FK_Supplier_Country");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__734FE6BF63FC749B");

            entity.HasOne(d => d.Item).WithMany(p => p.Warehouses).HasConstraintName("FK_Warehouse_Item");

            entity.HasOne(d => d.Store).WithMany(p => p.Warehouses).HasConstraintName("FK_Warehouse_Store");
        });

        modelBuilder.Entity<EventLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK_EventLog");
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK_AppSettings");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
