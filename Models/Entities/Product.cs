using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanGo.Models.Entities
{
    [Table("Products")]
    public class Product
    {
        // id san pham
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // id danh muc san pham
        [Column("category_id")]
        public int? CategoryId { get; set; }

        // sku san pham
        [Required]
        [MaxLength(50)]
        [Column("sku")]
        
        public string Sku { get; set; } = string.Empty;

        // ten san pham
        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        // mo ta san pham
        [Column("description")]
        public string? Description { get; set; }

        // gia san pham
        [Required]
        [Column("price", TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        // don vi tinh san pham
        [Required]
        [MaxLength(20)]
        [Column("unit")]
        public string Unit { get; set; } = string.Empty;

        // loai go
        [Required]
        [MaxLength(100)]
        [Column("wood_type")]
        public string WoodType { get; set; } = string.Empty;

        // kich thuoc san pham
        [MaxLength(100)]
        [Column("dimensions")]
        public string? Dimensions { get; set; }

        // do am cua go
        [MaxLength(50)]
        [Column("moisture_level")]
        public string? MoistureLevel { get; set; } = string.Empty;

        // nguon goc san pham
        [MaxLength(100)]
        [Column("origin")]
        public string? Origin { get; set; } = string.Empty;

        // so luong ton kho
        [Column("stock_quantity" , TypeName = "decimal(10,2)") ]
        public decimal StockQuantity { get; set; }

        // anh
        [MaxLength(255)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        // is active ?
        [Column("is_active" ) ]
        public bool IsActive { get; set; } = true;

        // ngay tao
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ============================================
        // NAVIGATION PROPERTIES (Quan hệ với bảng khác)
        // ============================================

        /// <summary>
        /// Danh mục sản phẩm thuộc về
        /// </summary>
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        /// <summary>
        /// Danh sách chi tiết đơn hàng chứa sản phẩm này
        /// (1 Product có thể xuất hiện trong nhiều OrderItems)
        /// </summary>
        public ICollection<OrderItem>? OrderItems { get; set; }

    }
}