-- Created by GitHub Copilot in SSMS - review carefully before executing

-- ====================================================================
-- 2. XÓA BẢNG CŨ (Nếu tồn tại - Xóa theo thứ tự ngược của khóa ngoại)
-- ====================================================================
DROP TABLE IF EXISTS Order_Items;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Users;
GO

-- ====================================================================
-- 3. CẤU TRÚC CÁC BẢNG (TABLE STRUCTURES)
-- ====================================================================

-- Bảng Người dùng (Quản trị viên, Nhân viên, Khách hàng, Xưởng mộc đối tác)
CREATE TABLE Users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    phone NVARCHAR(20),
    address NVARCHAR(MAX),
    role NVARCHAR(20) NOT NULL DEFAULT 'customer' CHECK (role IN ('customer', 'staff', 'admin')),
    company_name NVARCHAR(150) DEFAULT NULL, -- Dành cho khách hàng doanh nghiệp/xưởng mộc
    created_at DATETIME2 DEFAULT SYSDATETIME()
);
GO

-- Bảng Danh mục Gỗ (Hỗ trợ danh mục đa cấp)
CREATE TABLE Categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    parent_id INT DEFAULT NULL,
    FOREIGN KEY (parent_id) REFERENCES Categories(id) ON DELETE NO ACTION
);
GO

-- Bảng Sản phẩm Gỗ (Tối ưu hóa bộ lọc thuộc tính ngành gỗ)
CREATE TABLE Products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    category_id INT,
    sku NVARCHAR(50) NOT NULL UNIQUE,            -- Mã định danh sản phẩm
    name NVARCHAR(200) NOT NULL,                 -- Tên sản phẩm gỗ
    description NVARCHAR(MAX),                   -- Mô tả chi tiết
    price DECIMAL(15, 2) NOT NULL,              -- Giá bán trên một đơn vị
    unit NVARCHAR(20) NOT NULL CHECK (unit IN ('m3', 'tam', 'thanh', 'kg', 'm2')), -- Đơn vị đặc thù
    wood_type NVARCHAR(100) NOT NULL,            -- Chủng loại (Sồi, Tần bì, Óc chó, Cao su...)
    dimensions NVARCHAR(100),                    -- Kích thước quy cách (VD: 25mm x 150mm x 3000mm)
    moisture_level NVARCHAR(50),                 -- Sấy đạt độ ẩm (VD: 10% (+/-2%))
    origin NVARCHAR(100),                        -- Xuất xứ nguồn gốc
    stock_quantity DECIMAL(10, 2) DEFAULT 0.00, -- Số lượng tồn kho (Dùng DECIMAL vì gỗ xẻ có số lẻ)
    image_url NVARCHAR(255),
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (category_id) REFERENCES Categories(id) ON DELETE NO ACTION
);
GO

-- Bảng Đơn hàng tổng quan
CREATE TABLE Orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    total_amount DECIMAL(15, 2) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending', 'processing', 'shipped', 'delivered', 'cancelled')),
    shipping_address NVARCHAR(MAX) NOT NULL,
    payment_method NVARCHAR(20) DEFAULT 'bank_transfer' CHECK (payment_method IN ('cod', 'bank_transfer', 'debt')), -- Ngành gỗ thường chuyển khoản hoặc công nợ
    order_notes NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT SYSDATETIME(),
    FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE NO ACTION
);
GO

-- Bảng Chi tiết Đơn hàng (Lưu vết giá và số lượng thực tế)
CREATE TABLE Order_Items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT,
    product_id INT,
    quantity DECIMAL(10, 2) NOT NULL,           -- Số lượng mua (Cho phép số thập phân như 1.45 khối)
    price_at_time DECIMAL(15, 2) NOT NULL,      -- Chốt giá tại thời điểm đặt hàng đề phòng thay đổi
    FOREIGN KEY (order_id) REFERENCES Orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES Products(id) ON DELETE NO ACTION
);
GO

-- ====================================================================
-- 4. DỮ LIỆU GIẢ LẬP / DỮ LIỆU MẪU (MOCK DATA)
-- ====================================================================

-- Thêm tài khoản người dùng mẫu
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (id, full_name, email, password_hash, phone, address, role, company_name) VALUES
(1, N'Nguyễn Văn Admin', 'admin@xuonggo.vn', '$2y$10$EIXV...hashed', '0901234567', N'Tòa nhà gỗ, Đường Cộng Hòa, Tân Bình, TP.HCM', 'admin', N'Tổng Kho Gỗ Việt Nam'),
(2, N'Lê Hoàng Nhân Viên', 'sale1@xuonggo.vn', '$2y$10$EIXV...hashed', '0907778889', N'Kho Gỗ số 2, Dĩ An, Bình Dương', 'staff', N'Tổng Kho Gỗ Việt Nam'),
(3, N'Phạm Thành Long', 'longpham@gmail.com', '$2y$10$EIXV...hashed', '0981112223', N'78 Đường Láng, Đống Đa, Hà Nội', 'customer', NULL),
(4, N'Trần Đình Mộc', 'mocdecor@gmail.com', '$2y$10$EIXV...hashed', '0914455667', N'KCN Thạch Thất, Quốc Oai, Hà Nội', 'customer', N'Công Ty TNHH Nội Thất Mộc Decor'),
(5, N'Hoàng Thị Mai', 'maitran_go@yahoo.com', '$2y$10$EIXV...hashed', '0938889991', N'Đường Nguyễn Thị Thập, Quận 7, TP.HCM', 'customer', N'Xưởng Sản Xuất Đồ Gỗ Mỹ Nghệ Mai Hoàng');
SET IDENTITY_INSERT Users OFF;
GO

-- Thêm phân cấp danh mục gỗ mẫu
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (id, name, description, parent_id) VALUES
(1, N'Gỗ Tự Nhiên Xẻ Sấy', N'Gỗ nguyên khối nhập khẩu và nội địa được xẻ và sấy tiêu chuẩn công nghiệp', NULL),
(2, N'Gỗ Công Nghiệp (Ván Ép)', N'Các loại ván sợi, ván dăm phủ bề mặt chuyên dụng cho nội thất hiện đại', NULL),
(3, N'Gỗ Tự Nhiên Tròn', N'Gỗ nguyên cây chưa qua chế biến xẻ sấy', NULL),
-- Danh mục con của Gỗ Tự Nhiên Xẻ Sấy (parent_id = 1)
(4, N'Gỗ Óc Chó (Walnut)', N'Gỗ Óc Chó thượng hạng nhập khẩu Bắc Mỹ', 1),
(5, N'Gỗ Sồi (Oak)', N'Gỗ Sồi trắng và Sồi đỏ nhập khẩu Mỹ / Âu', 1),
(6, N'Gỗ Tần Bì (Ash)', N'Gỗ Tần Bì chịu lực tốt, vân gỗ sáng, dễ bám sơn', 1),
-- Danh mục con của Gỗ Công Nghiệp (parent_id = 2)
(7, N'Ván MDF Chống Ẩm', N'Ván gỗ MDF lõi xanh chống ẩm mốc cho tủ bếp, lavabo', 2),
(8, N'Ván Gỗ Ghép Thanh', N'Gỗ cao su, gỗ tràm ghép thanh phủ keo bóng hoặc phủ veneer', 2);
SET IDENTITY_INSERT Categories OFF;
GO

-- Thêm thông tin sản phẩm gỗ sát thực tế thị trường
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (id, category_id, sku, name, description, price, unit, wood_type, dimensions, moisture_level, origin, stock_quantity, image_url) VALUES
(1, 4, 'WAL-FAS-4/4', N'Gỗ Óc Chó (Walnut) Bắc Mỹ Hạng FAS 4/4', N'Gỗ óc chó nhập khẩu loại tốt nhất (FAS), bản gỗ rộng đẹp, ít mắt chết.', 48500000.00, 'm3', N'Óc Chó (Walnut)', 'Dày 25.4mm x Rộng >100mm', '8% - 12%', N'Bắc Mỹ', 12.45, 'images/walnut.jpg'),
(2, 5, 'WOAK-1C-5/4', N'Gỗ Sồi Trắng (White Oak) Mỹ Hạng 1C', N'Sồi trắng cứng, vân núi, bám đinh vít tốt. Thích hợp gia công tủ bếp.', 22000000.00, 'm3', N'Sồi Trắng (White Oak)', N'Dày 31.8mm x Rộng >120mm', '10% - 14%', N'Mỹ', 35.80, 'images/white-oak.jpg'),
(3, 6, 'ASH-2C-20MM', N'Gỗ Tần Bì (Ash) Âu Quy Cách 20mm', N'Gỗ tần bì kinh tế, vân biên mềm mại, màu vàng nhạt sáng.', 14500000.00, 'm3', N'Tần Bì (Ash)', N'Dày 20mm x Rộng 150mm', '12% (+/-2%)', N'Châu Âu', 58.20, 'images/ash.jpg'),
(4, 7, 'MDF-LX-18MM', N'Ván MDF Lõi Xanh Chống Ẩm HMR 18mm', N'Ván sợi chống ẩm cao cấp, tỉ trọng ép nén cao, không cong vênh.', 410000.00, 'tam', N'MDF Lõi Xanh', '1220mm x 2440mm x 18mm', 'N/A', N'Thái Lan', 850.00, 'images/mdf-18.jpg'),
(5, 7, 'MDF-LX-09MM', N'Ván MDF Lõi Xanh Chống Ẩm HMR 9mm', N'Ván ép lõi xanh chống ẩm, kinh tế, tối ưu chi phí làm hậu tủ.', 245000.00, 'tam', N'MDF Lõi Xanh', '1220mm x 2440mm x 9mm', 'N/A', N'Việt Nam', 1200.00, 'images/mdf-9.jpg'),
(6, 8, 'RUB-AA-18MM', N'Gỗ Cao Su Ghép Thanh Chất Lượng AA 18mm', N'Gỗ cao su tự nhiên ghép thanh ép nhiệt, cả 2 mặt đều đẹp láng mịn.', 520000.00, 'tam', N'Gỗ Cao Su', '1200mm x 2400mm x 18mm', '10%', N'Việt Nam', 320.00, 'images/rubber.jpg');
SET IDENTITY_INSERT Products OFF;
GO

-- Thêm đơn hàng mẫu (Các trạng thái khác nhau từ đang chờ đến đã giao)
SET IDENTITY_INSERT Orders ON;
INSERT INTO Orders (id, user_id, total_amount, status, shipping_address, payment_method, order_notes) VALUES
(1, 4, 77150000.00, 'processing', N'Cụm CN Bình Phú, Thạch Thất, Hà Nội', 'bank_transfer', N'Khách hẹn giao xe thùng 5 tấn vào sáng Thứ Năm. Cần kèm chứng chỉ CO/CQ kiểm định gỗ nhập khẩu.'),
(2, 5, 20800000.00, 'delivered', N'Xưởng mộc Mai Hoàng, Đường Lê Văn Khương, Quận 12, TP.HCM', 'bank_transfer', N'Đã thanh toán đủ qua Techcombank. Gỗ cao su yêu cầu bọc màng PE tránh trầy xước.'),
(3, 3, 2050000.00, 'pending', N'Số 5 Ngõ 12, Phố Cầu Giấy, Hà Nội', 'cod', N'Giao ván MDF lẻ bằng xe ba gác, gọi điện trước khi đi 30 phút.');
SET IDENTITY_INSERT Orders OFF;
GO

-- Thêm chi tiết cho các đơn hàng trên (Bao gồm bán số lượng lẻ)
SET IDENTITY_INSERT Order_Items ON;
INSERT INTO Order_Items (id, order_id, product_id, quantity, price_at_time) VALUES
-- Đơn hàng 1 (Mua 1.5 khối Óc chó và 10 tấm MDF)
(1, 1, 1, 1.50, 48500000.00), -- 1.5 x 48,500,000 = 72,750,000
(2, 1, 4, 10.00, 410000.00),  -- 10 x 410,000 = 4,100,000
-- Đơn hàng 2 (Mua sỉ ván cao su)
(3, 2, 6, 40.00, 520000.00),  -- 40 x 520,000 = 20,800,000
-- Đơn hàng 3 (Mua ván MDF mỏng làm phòng trọ lẻ)
(4, 3, 4, 2.00, 410000.00),   -- 2 x 410,000 = 820,000
(5, 3, 5, 5.00, 245000.00);   -- 5 x 245,000 = 1,225,000
SET IDENTITY_INSERT Order_Items OFF;
GO