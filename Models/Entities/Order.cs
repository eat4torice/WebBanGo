using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanGo.Models.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // id nguoi dat hang

        [Column("user_id")]
        public int UserId { get; set; }

        // total amount cua don hang(tong tien cua don hang)
        [Required]
        [Column("total_amount", TypeName = "decimal(15,2)")]
        public decimal TotalAmount { get; set; }

        // trang thai don hang (pending, processing, completed, cancelled)
        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "pending";

        // dia chi giao hang
        [Required]
        [Column("shipping_address")]
        public string ShippingAddress { get; set; } = string.Empty;

        // phuong thuc thanh toan (cod, bank_transfer, debt)
        [Column("payment_method")]
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = "bank_transfer";

        // ghi chu don hang
        [Column("order_notes")]
        public string? OrderNotes { get; set; }

        // ngay tao don hang
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ==========================================
        // NAVIGATION PROPERTIES (Quan hệ)
        // ==========================================

        /// <summary>
        /// Người đặt hàng (khách hàng)
        /// Quan hệ N-1: Nhiều Order → 1 User
        /// </summary>
        [ForeignKey("UserId")]
        public User? User { get; set; }

        /// <summary>
        /// Danh sách chi tiết sản phẩm trong đơn hàng
        /// Quan hệ 1-N: 1 Order → Nhiều OrderItem
        /// </summary>
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}