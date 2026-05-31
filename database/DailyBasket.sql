IF DB_ID('DailyBasketSWE310Db') IS NULL
BEGIN
    CREATE DATABASE DailyBasketSWE310Db;
END
GO

USE DailyBasketSWE310Db;
GO

IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.CartItems', 'U') IS NOT NULL DROP TABLE dbo.CartItems;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
GO

CREATE TABLE dbo.Categories
(
    CategoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    CategoryName NVARCHAR(80) NOT NULL,
    Description NVARCHAR(250) NULL,
    CONSTRAINT UQ_Categories_CategoryName UNIQUE (CategoryName)
);

CREATE TABLE dbo.Products
(
    ProductId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
    CategoryId INT NOT NULL,
    ProductName NVARCHAR(120) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    IsAvailable BIT NOT NULL CONSTRAINT DF_Products_IsAvailable DEFAULT (1),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId),
    CONSTRAINT CK_Products_Price CHECK (Price > 0),
    CONSTRAINT CK_Products_StockQuantity CHECK (StockQuantity >= 0)
);

CREATE TABLE dbo.Customers
(
    CustomerId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(160) NOT NULL,
    PhoneNumber NVARCHAR(30) NOT NULL,
    Address NVARCHAR(300) NOT NULL,
    CONSTRAINT UQ_Customers_Email UNIQUE (Email)
);

CREATE TABLE dbo.CartItems
(
    CartItemId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CartItems PRIMARY KEY,
    CustomerId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_CartItems_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId) ON DELETE CASCADE,
    CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT UQ_CartItems_Customer_Product UNIQUE (CustomerId, ProductId),
    CONSTRAINT CK_CartItems_Quantity CHECK (Quantity > 0)
);

CREATE TABLE dbo.Orders
(
    OrderId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
    CustomerId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT SYSUTCDATETIME(),
    DeliveryAddress NVARCHAR(300) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(40) NOT NULL,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0)
);

CREATE TABLE dbo.OrderItems
(
    OrderItemId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderItems PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice > 0)
);
GO

SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (CategoryId, CategoryName, Description) VALUES
(1, 'Fresh Produce', 'Fresh fruits and vegetables for daily meals.'),
(2, 'Dairy and Eggs', 'Milk, cheese, yogurt, butter, and eggs.'),
(3, 'Bakery', 'Bread, pastries, and baked goods.'),
(4, 'Meat and Seafood', 'Fresh chicken, beef, fish, and seafood.'),
(5, 'Frozen Food', 'Frozen vegetables, meals, and desserts.'),
(6, 'Pantry Staples', 'Rice, pasta, sauces, spices, and canned food.'),
(7, 'Beverages', 'Juices, tea, coffee, and bottled drinks.'),
(8, 'Snacks', 'Chips, biscuits, nuts, and treats.'),
(9, 'Household', 'Cleaning and kitchen supplies.'),
(10, 'Personal Care', 'Daily hygiene and personal care items.');
SET IDENTITY_INSERT dbo.Categories OFF;

SET IDENTITY_INSERT dbo.Products ON;
INSERT INTO dbo.Products (ProductId, CategoryId, ProductName, Description, Price, StockQuantity, ImageUrl, IsAvailable) VALUES
(1, 1, 'Banana Bunch', 'Sweet ripe bananas.', 5.90, 80, 'https://images.unsplash.com/photo-1603833665858-e61d17a86224', 1),
(2, 1, 'Broccoli Crown', 'Fresh green broccoli.', 4.50, 65, 'https://images.unsplash.com/photo-1459411621453-7b03977f4bfc', 1),
(3, 2, 'Whole Milk 1L', 'Creamy full cream milk.', 7.20, 50, 'https://images.unsplash.com/photo-1563636619-e9143da7973b', 1),
(4, 2, 'Free Range Eggs', 'Pack of 10 eggs.', 9.80, 40, 'https://images.unsplash.com/photo-1506976785307-8732e854ad03', 1),
(5, 3, 'Wholemeal Bread', 'Soft sliced wholemeal loaf.', 6.50, 35, 'https://images.unsplash.com/photo-1509440159596-0249088772ff', 1),
(6, 3, 'Butter Croissant', 'Flaky bakery croissant.', 3.90, 45, 'https://images.unsplash.com/photo-1555507036-ab1f4038808a', 1),
(7, 4, 'Chicken Breast', 'Boneless chicken breast pack.', 16.90, 30, 'https://images.unsplash.com/photo-1604503468506-a8da13d82791', 1),
(8, 4, 'Salmon Fillet', 'Fresh salmon fillet.', 28.00, 22, 'https://images.unsplash.com/photo-1599084993091-1cb5c0721cc6', 1),
(9, 5, 'Frozen Mixed Vegetables', 'Convenient mixed vegetables.', 8.90, 60, 'https://images.unsplash.com/photo-1518843875459-f738682238a6', 1),
(10, 5, 'Vanilla Ice Cream', 'Family tub vanilla ice cream.', 13.90, 28, 'https://images.unsplash.com/photo-1563805042-7684c019e1cb', 1),
(11, 6, 'Jasmine Rice 5kg', 'Fragrant white jasmine rice.', 24.90, 32, 'https://images.unsplash.com/photo-1586201375761-83865001e31c', 1),
(12, 6, 'Pasta Spaghetti', 'Durum wheat spaghetti.', 5.40, 75, 'https://images.unsplash.com/photo-1551462147-ff29053bfc14', 1),
(13, 7, 'Orange Juice 1L', 'Refreshing orange juice.', 8.70, 55, 'https://images.unsplash.com/photo-1600271886742-f049cd451bba', 1),
(14, 7, 'Ground Coffee', 'Medium roast ground coffee.', 18.90, 24, 'https://images.unsplash.com/photo-1447933601403-0c6688de566e', 1),
(15, 8, 'Potato Chips', 'Classic salted chips.', 6.20, 70, 'https://images.unsplash.com/photo-1566478989037-eec170784d0b', 1),
(16, 8, 'Roasted Almonds', 'Crunchy roasted almonds.', 12.90, 38, 'https://images.unsplash.com/photo-1508061253366-f7da158b6d46', 1),
(17, 9, 'Dishwashing Liquid', 'Lemon scented dish soap.', 7.90, 42, 'https://images.unsplash.com/photo-1585421514284-efb74c2b69ba', 1),
(18, 9, 'Kitchen Towels', 'Absorbent paper towels.', 10.50, 48, 'https://images.unsplash.com/photo-1583947581924-860bda6a26df', 1),
(19, 10, 'Shampoo', 'Daily care shampoo.', 14.90, 36, 'https://images.unsplash.com/photo-1620916566398-39f1143ab7be', 1),
(20, 10, 'Toothpaste', 'Fresh mint toothpaste.', 6.90, 52, 'https://images.unsplash.com/photo-1607613009820-a29f7bb81c04', 1);
SET IDENTITY_INSERT dbo.Products OFF;

SET IDENTITY_INSERT dbo.Customers ON;
INSERT INTO dbo.Customers (CustomerId, FullName, Email, PhoneNumber, Address) VALUES
(1, 'Muhammad Ali', 'muhammad.ali@example.com', '012-3456789', '12 Jalan Meranti, Selangor'),
(2, 'Nur Aisyah', 'nur.aisyah@example.com', '013-2468101', '22 Jalan Melati, Cyberjaya'),
(3, 'Jason Lim', 'jason.lim@example.com', '014-9988776', '3 Persiaran Mutiara, Puchong'),
(4, 'Priya Nair', 'priya.nair@example.com', '016-5544332', '18 Jalan Kenari, Klang'),
(5, 'Tan Mei Ling', 'tan.meiling@example.com', '017-1234567', '8 Jalan Anggerik, Petaling Jaya'),
(6, 'Daniel Wong', 'daniel.wong@example.com', '018-7654321', '45 Jalan Harmoni, Subang Jaya'),
(7, 'Siti Aminah', 'siti.aminah@example.com', '019-3332211', '5 Jalan Mawar, Putrajaya'),
(8, 'Lee Jun Wei', 'lee.junwei@example.com', '011-2244668', '29 Jalan Flora, Shah Alam'),
(9, 'Farah Hassan', 'farah.hassan@example.com', '012-1122334', '14 Jalan Dahlia, Kajang'),
(10, 'Marcus Tan', 'marcus.tan@example.com', '013-8899001', '71 Jalan Sentosa, Cheras');
SET IDENTITY_INSERT dbo.Customers OFF;

SET IDENTITY_INSERT dbo.CartItems ON;
INSERT INTO dbo.CartItems (CartItemId, CustomerId, ProductId, Quantity) VALUES
(1, 1, 1, 2),
(2, 1, 3, 1),
(3, 2, 5, 2),
(4, 2, 13, 1),
(5, 3, 7, 1),
(6, 3, 11, 1),
(7, 4, 15, 3),
(8, 4, 18, 1),
(9, 5, 2, 2),
(10, 5, 20, 1);
SET IDENTITY_INSERT dbo.CartItems OFF;

SET IDENTITY_INSERT dbo.Orders ON;
INSERT INTO dbo.Orders (OrderId, CustomerId, OrderDate, DeliveryAddress, TotalAmount, Status) VALUES
(1, 1, '2026-05-01T10:00:00', '12 Jalan Meranti, Selangor', 19.00, 'Delivered'),
(2, 2, '2026-05-02T11:30:00', '22 Jalan Melati, Cyberjaya', 21.70, 'Delivered'),
(3, 3, '2026-05-03T09:15:00', '3 Persiaran Mutiara, Puchong', 41.80, 'Processing'),
(4, 4, '2026-05-04T13:05:00', '18 Jalan Kenari, Klang', 36.90, 'Pending'),
(5, 5, '2026-05-05T15:40:00', '8 Jalan Anggerik, Petaling Jaya', 22.90, 'Delivered'),
(6, 6, '2026-05-06T16:10:00', '45 Jalan Harmoni, Subang Jaya', 33.80, 'Delivered'),
(7, 7, '2026-05-07T12:25:00', '5 Jalan Mawar, Putrajaya', 19.20, 'Processing'),
(8, 8, '2026-05-08T17:55:00', '29 Jalan Flora, Shah Alam', 31.30, 'Pending'),
(9, 9, '2026-05-09T08:45:00', '14 Jalan Dahlia, Kajang', 22.20, 'Delivered'),
(10, 10, '2026-05-10T14:20:00', '71 Jalan Sentosa, Cheras', 23.70, 'Pending');
SET IDENTITY_INSERT dbo.Orders OFF;

SET IDENTITY_INSERT dbo.OrderItems ON;
INSERT INTO dbo.OrderItems (OrderItemId, OrderId, ProductId, Quantity, UnitPrice) VALUES
(1, 1, 1, 2, 5.90),
(2, 1, 3, 1, 7.20),
(3, 2, 5, 2, 6.50),
(4, 2, 13, 1, 8.70),
(5, 3, 7, 1, 16.90),
(6, 3, 11, 1, 24.90),
(7, 4, 8, 1, 28.00),
(8, 4, 9, 1, 8.90),
(9, 5, 2, 2, 4.50),
(10, 5, 10, 1, 13.90),
(11, 6, 14, 1, 18.90),
(12, 6, 19, 1, 14.90),
(13, 7, 12, 1, 5.40),
(14, 7, 20, 2, 6.90),
(15, 8, 16, 1, 12.90),
(16, 8, 17, 1, 7.90),
(17, 8, 18, 1, 10.50),
(18, 9, 4, 1, 9.80),
(19, 9, 15, 2, 6.20),
(20, 10, 6, 2, 3.90),
(21, 10, 13, 1, 8.70),
(22, 10, 3, 1, 7.20);
SET IDENTITY_INSERT dbo.OrderItems OFF;
GO
