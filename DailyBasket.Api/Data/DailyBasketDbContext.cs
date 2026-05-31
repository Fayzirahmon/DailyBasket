using DailyBasket.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyBasket.Api.Data;

public class DailyBasketDbContext(DbContextOptions<DailyBasketDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(category => category.CategoryName).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Description).HasMaxLength(250);
            entity.HasIndex(category => category.CategoryName).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.ProductName).HasMaxLength(120).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(500);
            entity.Property(product => product.Price).HasColumnType("decimal(10,2)");
            entity.Property(product => product.ImageUrl).HasMaxLength(500);
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(customer => customer.FullName).HasMaxLength(120).IsRequired();
            entity.Property(customer => customer.Email).HasMaxLength(160).IsRequired();
            entity.Property(customer => customer.PhoneNumber).HasMaxLength(30).IsRequired();
            entity.Property(customer => customer.Address).HasMaxLength(300).IsRequired();
            entity.HasIndex(customer => customer.Email).IsUnique();
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(cartItem => new { cartItem.CustomerId, cartItem.ProductId }).IsUnique();
            entity.HasOne(cartItem => cartItem.Customer)
                .WithMany(customer => customer.CartItems)
                .HasForeignKey(cartItem => cartItem.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(cartItem => cartItem.Product)
                .WithMany(product => product.CartItems)
                .HasForeignKey(cartItem => cartItem.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.Property(order => order.DeliveryAddress).HasMaxLength(300).IsRequired();
            entity.Property(order => order.TotalAmount).HasColumnType("decimal(10,2)");
            entity.Property(order => order.Status).HasMaxLength(40).IsRequired();
            entity.HasOne(order => order.Customer)
                .WithMany(customer => customer.Orders)
                .HasForeignKey(order => order.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(orderItem => orderItem.UnitPrice).HasColumnType("decimal(10,2)");
            entity.HasOne(orderItem => orderItem.Order)
                .WithMany(order => order.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(orderItem => orderItem.Product)
                .WithMany(product => product.OrderItems)
                .HasForeignKey(orderItem => orderItem.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Fresh Produce", Description = "Fresh fruits and vegetables for daily meals." },
            new Category { CategoryId = 2, CategoryName = "Dairy and Eggs", Description = "Milk, cheese, yogurt, butter, and eggs." },
            new Category { CategoryId = 3, CategoryName = "Bakery", Description = "Bread, pastries, and baked goods." },
            new Category { CategoryId = 4, CategoryName = "Meat and Seafood", Description = "Fresh chicken, beef, fish, and seafood." },
            new Category { CategoryId = 5, CategoryName = "Frozen Food", Description = "Frozen vegetables, meals, and desserts." },
            new Category { CategoryId = 6, CategoryName = "Pantry Staples", Description = "Rice, pasta, sauces, spices, and canned food." },
            new Category { CategoryId = 7, CategoryName = "Beverages", Description = "Juices, tea, coffee, and bottled drinks." },
            new Category { CategoryId = 8, CategoryName = "Snacks", Description = "Chips, biscuits, nuts, and treats." },
            new Category { CategoryId = 9, CategoryName = "Household", Description = "Cleaning and kitchen supplies." },
            new Category { CategoryId = 10, CategoryName = "Personal Care", Description = "Daily hygiene and personal care items." }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, CategoryId = 1, ProductName = "Banana Bunch", Description = "Sweet ripe bananas.", Price = 5.90m, StockQuantity = 80, ImageUrl = "https://images.unsplash.com/photo-1603833665858-e61d17a86224", IsAvailable = true },
            new Product { ProductId = 2, CategoryId = 1, ProductName = "Broccoli Crown", Description = "Fresh green broccoli.", Price = 4.50m, StockQuantity = 65, ImageUrl = "https://images.unsplash.com/photo-1459411621453-7b03977f4bfc", IsAvailable = true },
            new Product { ProductId = 3, CategoryId = 2, ProductName = "Whole Milk 1L", Description = "Creamy full cream milk.", Price = 7.20m, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b", IsAvailable = true },
            new Product { ProductId = 4, CategoryId = 2, ProductName = "Free Range Eggs", Description = "Pack of 10 eggs.", Price = 9.80m, StockQuantity = 40, ImageUrl = "https://images.unsplash.com/photo-1506976785307-8732e854ad03", IsAvailable = true },
            new Product { ProductId = 5, CategoryId = 3, ProductName = "Wholemeal Bread", Description = "Soft sliced wholemeal loaf.", Price = 6.50m, StockQuantity = 35, ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff", IsAvailable = true },
            new Product { ProductId = 6, CategoryId = 3, ProductName = "Butter Croissant", Description = "Flaky bakery croissant.", Price = 3.90m, StockQuantity = 45, ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a", IsAvailable = true },
            new Product { ProductId = 7, CategoryId = 4, ProductName = "Chicken Breast", Description = "Boneless chicken breast pack.", Price = 16.90m, StockQuantity = 30, ImageUrl = "https://images.unsplash.com/photo-1604503468506-a8da13d82791", IsAvailable = true },
            new Product { ProductId = 8, CategoryId = 4, ProductName = "Salmon Fillet", Description = "Fresh salmon fillet.", Price = 28.00m, StockQuantity = 22, ImageUrl = "https://images.unsplash.com/photo-1599084993091-1cb5c0721cc6", IsAvailable = true },
            new Product { ProductId = 9, CategoryId = 5, ProductName = "Frozen Mixed Vegetables", Description = "Convenient mixed vegetables.", Price = 8.90m, StockQuantity = 60, ImageUrl = "https://images.unsplash.com/photo-1518843875459-f738682238a6", IsAvailable = true },
            new Product { ProductId = 10, CategoryId = 5, ProductName = "Vanilla Ice Cream", Description = "Family tub vanilla ice cream.", Price = 13.90m, StockQuantity = 28, ImageUrl = "https://images.unsplash.com/photo-1563805042-7684c019e1cb", IsAvailable = true },
            new Product { ProductId = 11, CategoryId = 6, ProductName = "Jasmine Rice 5kg", Description = "Fragrant white jasmine rice.", Price = 24.90m, StockQuantity = 32, ImageUrl = "https://images.unsplash.com/photo-1586201375761-83865001e31c", IsAvailable = true },
            new Product { ProductId = 12, CategoryId = 6, ProductName = "Pasta Spaghetti", Description = "Durum wheat spaghetti.", Price = 5.40m, StockQuantity = 75, ImageUrl = "https://images.unsplash.com/photo-1551462147-ff29053bfc14", IsAvailable = true },
            new Product { ProductId = 13, CategoryId = 7, ProductName = "Orange Juice 1L", Description = "Refreshing orange juice.", Price = 8.70m, StockQuantity = 55, ImageUrl = "https://images.unsplash.com/photo-1600271886742-f049cd451bba", IsAvailable = true },
            new Product { ProductId = 14, CategoryId = 7, ProductName = "Ground Coffee", Description = "Medium roast ground coffee.", Price = 18.90m, StockQuantity = 24, ImageUrl = "https://images.unsplash.com/photo-1447933601403-0c6688de566e", IsAvailable = true },
            new Product { ProductId = 15, CategoryId = 8, ProductName = "Potato Chips", Description = "Classic salted chips.", Price = 6.20m, StockQuantity = 70, ImageUrl = "https://images.unsplash.com/photo-1566478989037-eec170784d0b", IsAvailable = true },
            new Product { ProductId = 16, CategoryId = 8, ProductName = "Roasted Almonds", Description = "Crunchy roasted almonds.", Price = 12.90m, StockQuantity = 38, ImageUrl = "https://images.unsplash.com/photo-1508061253366-f7da158b6d46", IsAvailable = true },
            new Product { ProductId = 17, CategoryId = 9, ProductName = "Dishwashing Liquid", Description = "Lemon scented dish soap.", Price = 7.90m, StockQuantity = 42, ImageUrl = "https://images.unsplash.com/photo-1585421514284-efb74c2b69ba", IsAvailable = true },
            new Product { ProductId = 18, CategoryId = 9, ProductName = "Kitchen Towels", Description = "Absorbent paper towels.", Price = 10.50m, StockQuantity = 48, ImageUrl = "https://images.unsplash.com/photo-1583947581924-860bda6a26df", IsAvailable = true },
            new Product { ProductId = 19, CategoryId = 10, ProductName = "Shampoo", Description = "Daily care shampoo.", Price = 14.90m, StockQuantity = 36, ImageUrl = "https://images.unsplash.com/photo-1620916566398-39f1143ab7be", IsAvailable = true },
            new Product { ProductId = 20, CategoryId = 10, ProductName = "Toothpaste", Description = "Fresh mint toothpaste.", Price = 6.90m, StockQuantity = 52, ImageUrl = "https://images.unsplash.com/photo-1607613009820-a29f7bb81c04", IsAvailable = true }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerId = 1, FullName = "Muhammad Ali", Email = "muhammad.ali@example.com", PhoneNumber = "012-3456789", Address = "12 Jalan Meranti, Selangor" },
            new Customer { CustomerId = 2, FullName = "Nur Aisyah", Email = "nur.aisyah@example.com", PhoneNumber = "013-2468101", Address = "22 Jalan Melati, Cyberjaya" },
            new Customer { CustomerId = 3, FullName = "Jason Lim", Email = "jason.lim@example.com", PhoneNumber = "014-9988776", Address = "3 Persiaran Mutiara, Puchong" },
            new Customer { CustomerId = 4, FullName = "Priya Nair", Email = "priya.nair@example.com", PhoneNumber = "016-5544332", Address = "18 Jalan Kenari, Klang" },
            new Customer { CustomerId = 5, FullName = "Tan Mei Ling", Email = "tan.meiling@example.com", PhoneNumber = "017-1234567", Address = "8 Jalan Anggerik, Petaling Jaya" },
            new Customer { CustomerId = 6, FullName = "Daniel Wong", Email = "daniel.wong@example.com", PhoneNumber = "018-7654321", Address = "45 Jalan Harmoni, Subang Jaya" },
            new Customer { CustomerId = 7, FullName = "Siti Aminah", Email = "siti.aminah@example.com", PhoneNumber = "019-3332211", Address = "5 Jalan Mawar, Putrajaya" },
            new Customer { CustomerId = 8, FullName = "Lee Jun Wei", Email = "lee.junwei@example.com", PhoneNumber = "011-2244668", Address = "29 Jalan Flora, Shah Alam" },
            new Customer { CustomerId = 9, FullName = "Farah Hassan", Email = "farah.hassan@example.com", PhoneNumber = "012-1122334", Address = "14 Jalan Dahlia, Kajang" },
            new Customer { CustomerId = 10, FullName = "Marcus Tan", Email = "marcus.tan@example.com", PhoneNumber = "013-8899001", Address = "71 Jalan Sentosa, Cheras" }
        );

        modelBuilder.Entity<CartItem>().HasData(
            new CartItem { CartItemId = 1, CustomerId = 1, ProductId = 1, Quantity = 2 },
            new CartItem { CartItemId = 2, CustomerId = 1, ProductId = 3, Quantity = 1 },
            new CartItem { CartItemId = 3, CustomerId = 2, ProductId = 5, Quantity = 2 },
            new CartItem { CartItemId = 4, CustomerId = 2, ProductId = 13, Quantity = 1 },
            new CartItem { CartItemId = 5, CustomerId = 3, ProductId = 7, Quantity = 1 },
            new CartItem { CartItemId = 6, CustomerId = 3, ProductId = 11, Quantity = 1 },
            new CartItem { CartItemId = 7, CustomerId = 4, ProductId = 15, Quantity = 3 },
            new CartItem { CartItemId = 8, CustomerId = 4, ProductId = 18, Quantity = 1 },
            new CartItem { CartItemId = 9, CustomerId = 5, ProductId = 2, Quantity = 2 },
            new CartItem { CartItemId = 10, CustomerId = 5, ProductId = 20, Quantity = 1 }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order { OrderId = 1, CustomerId = 1, OrderDate = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc), DeliveryAddress = "12 Jalan Meranti, Selangor", TotalAmount = 19.00m, Status = "Delivered" },
            new Order { OrderId = 2, CustomerId = 2, OrderDate = new DateTime(2026, 5, 2, 11, 30, 0, DateTimeKind.Utc), DeliveryAddress = "22 Jalan Melati, Cyberjaya", TotalAmount = 21.70m, Status = "Delivered" },
            new Order { OrderId = 3, CustomerId = 3, OrderDate = new DateTime(2026, 5, 3, 9, 15, 0, DateTimeKind.Utc), DeliveryAddress = "3 Persiaran Mutiara, Puchong", TotalAmount = 41.80m, Status = "Processing" },
            new Order { OrderId = 4, CustomerId = 4, OrderDate = new DateTime(2026, 5, 4, 13, 5, 0, DateTimeKind.Utc), DeliveryAddress = "18 Jalan Kenari, Klang", TotalAmount = 36.90m, Status = "Pending" },
            new Order { OrderId = 5, CustomerId = 5, OrderDate = new DateTime(2026, 5, 5, 15, 40, 0, DateTimeKind.Utc), DeliveryAddress = "8 Jalan Anggerik, Petaling Jaya", TotalAmount = 22.90m, Status = "Delivered" },
            new Order { OrderId = 6, CustomerId = 6, OrderDate = new DateTime(2026, 5, 6, 16, 10, 0, DateTimeKind.Utc), DeliveryAddress = "45 Jalan Harmoni, Subang Jaya", TotalAmount = 33.80m, Status = "Delivered" },
            new Order { OrderId = 7, CustomerId = 7, OrderDate = new DateTime(2026, 5, 7, 12, 25, 0, DateTimeKind.Utc), DeliveryAddress = "5 Jalan Mawar, Putrajaya", TotalAmount = 19.20m, Status = "Processing" },
            new Order { OrderId = 8, CustomerId = 8, OrderDate = new DateTime(2026, 5, 8, 17, 55, 0, DateTimeKind.Utc), DeliveryAddress = "29 Jalan Flora, Shah Alam", TotalAmount = 31.30m, Status = "Pending" },
            new Order { OrderId = 9, CustomerId = 9, OrderDate = new DateTime(2026, 5, 9, 8, 45, 0, DateTimeKind.Utc), DeliveryAddress = "14 Jalan Dahlia, Kajang", TotalAmount = 22.20m, Status = "Delivered" },
            new Order { OrderId = 10, CustomerId = 10, OrderDate = new DateTime(2026, 5, 10, 14, 20, 0, DateTimeKind.Utc), DeliveryAddress = "71 Jalan Sentosa, Cheras", TotalAmount = 23.70m, Status = "Pending" }
        );

        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 5.90m },
            new OrderItem { OrderItemId = 2, OrderId = 1, ProductId = 3, Quantity = 1, UnitPrice = 7.20m },
            new OrderItem { OrderItemId = 3, OrderId = 2, ProductId = 5, Quantity = 2, UnitPrice = 6.50m },
            new OrderItem { OrderItemId = 4, OrderId = 2, ProductId = 13, Quantity = 1, UnitPrice = 8.70m },
            new OrderItem { OrderItemId = 5, OrderId = 3, ProductId = 7, Quantity = 1, UnitPrice = 16.90m },
            new OrderItem { OrderItemId = 6, OrderId = 3, ProductId = 11, Quantity = 1, UnitPrice = 24.90m },
            new OrderItem { OrderItemId = 7, OrderId = 4, ProductId = 8, Quantity = 1, UnitPrice = 28.00m },
            new OrderItem { OrderItemId = 8, OrderId = 4, ProductId = 9, Quantity = 1, UnitPrice = 8.90m },
            new OrderItem { OrderItemId = 9, OrderId = 5, ProductId = 2, Quantity = 2, UnitPrice = 4.50m },
            new OrderItem { OrderItemId = 10, OrderId = 5, ProductId = 10, Quantity = 1, UnitPrice = 13.90m },
            new OrderItem { OrderItemId = 11, OrderId = 6, ProductId = 14, Quantity = 1, UnitPrice = 18.90m },
            new OrderItem { OrderItemId = 12, OrderId = 6, ProductId = 19, Quantity = 1, UnitPrice = 14.90m },
            new OrderItem { OrderItemId = 13, OrderId = 7, ProductId = 12, Quantity = 1, UnitPrice = 5.40m },
            new OrderItem { OrderItemId = 14, OrderId = 7, ProductId = 20, Quantity = 2, UnitPrice = 6.90m },
            new OrderItem { OrderItemId = 15, OrderId = 8, ProductId = 16, Quantity = 1, UnitPrice = 12.90m },
            new OrderItem { OrderItemId = 16, OrderId = 8, ProductId = 17, Quantity = 1, UnitPrice = 7.90m },
            new OrderItem { OrderItemId = 17, OrderId = 8, ProductId = 18, Quantity = 1, UnitPrice = 10.50m },
            new OrderItem { OrderItemId = 18, OrderId = 9, ProductId = 4, Quantity = 1, UnitPrice = 9.80m },
            new OrderItem { OrderItemId = 19, OrderId = 9, ProductId = 15, Quantity = 2, UnitPrice = 6.20m },
            new OrderItem { OrderItemId = 20, OrderId = 10, ProductId = 6, Quantity = 2, UnitPrice = 3.90m },
            new OrderItem { OrderItemId = 21, OrderId = 10, ProductId = 13, Quantity = 1, UnitPrice = 8.70m },
            new OrderItem { OrderItemId = 22, OrderId = 10, ProductId = 3, Quantity = 1, UnitPrice = 7.20m }
        );
    }
}
