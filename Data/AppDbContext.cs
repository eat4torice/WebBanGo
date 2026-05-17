using Microsoft.EntityFrameworkCore;
using WebBanGo.Models.Entities;

namespace WebBanGo.Data
{
    /// <summary>
    /// AppDbContext - LỚP TRUNG TÂM ĐỂ QUẢN LÝ DATABASE
    /// 
    /// DbContext là gì?
    /// - Là cầu nối giữa C# code và SQL Server database
    /// - Quản lý kết nối database
    /// - Theo dõi thay đổi của Entity (Change Tracking)
    /// - Cung cấp CRUD operations
    /// 
    /// Cách dùng:
    /// using (var context = new AppDbContext())
    /// {
    ///     var products = context.Products.ToList();
    ///     context.Products.Add(newProduct);
    ///     context.SaveChanges();
    /// }
    /// </summary>
    public class AppDbContext : DbContext
    {
        // ==========================================
        // 1. CONSTRUCTOR (Hàm khởi tạo)
        // ==========================================
        
        /// <summary>
        /// Constructor cho ASP.NET Core (Dependency Injection)
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        /// <summary>
        /// Constructor mặc định cho Console App
        /// </summary>
        public AppDbContext()
        {
        }

        // ==========================================
        // 2. DBSET - KHAI BÁO CÁC BẢNG
        // ==========================================
        
        /// <summary>
        /// Bảng Users
        /// Sử dụng: context.Users.ToList() = SELECT * FROM Users
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Bảng Categories (Danh mục phân cấp)
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// Bảng Products (Sản phẩm gỗ)
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Bảng Orders (Đơn hàng)
        /// </summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>
        /// Bảng Order_Items (Chi tiết đơn hàng)
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        // ==========================================
        // 3. ON CONFIGURING - CẤU HÌNH KẾT NỐI
        // ==========================================
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // ⚠️ THAY ĐỔI MẬT KHẨU SA CHO ĐÚNG VỚI SQL SERVER CỦA BẠN
                optionsBuilder.UseSqlServer(
                    @"Server=localhost;Database=webbango;User Id=sa;Password=12345678;TrustServerCertificate=True;Connection Timeout=5",
                    options => options.CommandTimeout(5) // Timeout sau 5 giây
                );
            }
        }

        // ==========================================
        // 4. ON MODEL CREATING - CẤU HÌNH QUAN HỆ
        // ==========================================
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CẤU HÌNH USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("customer");
            });

            // CẤU HÌNH CATEGORIES (Self-reference: cha-con)
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                
                // Quan hệ cha-con
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.SubCategories)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // CẤU HÌNH PRODUCTS
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                
                // Quan hệ N-1: Product → Category
                entity.HasOne(e => e.Category)
                      .WithMany(e => e.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // CẤU HÌNH ORDERS
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("pending");
                entity.Property(e => e.PaymentMethod).HasDefaultValue("bank_transfer");
                
                // Quan hệ N-1: Order → User
                entity.HasOne(e => e.User)
                      .WithMany(e => e.Orders)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // CẤU HÌNH ORDER_ITEMS
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("Order_Items");
                entity.HasKey(e => e.Id);
                
                // Quan hệ N-1: OrderItem → Order (Cascade delete)
                entity.HasOne(e => e.Order)
                      .WithMany(e => e.OrderItems)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Quan hệ N-1: OrderItem → Product
                entity.HasOne(e => e.Product)
                      .WithMany(e => e.OrderItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
