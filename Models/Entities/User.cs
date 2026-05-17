using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanGo.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // full name 
        [Required]
        [MaxLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        // email
        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        // mat khau (da hash)
        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        // so dien thoai
        [MaxLength(20)]
        [Column("phone")]
         public string? PhoneNumber { get; set; }

        // dia chi
        [Column("address")]
        public string? Address { get; set; }

        // role cua nguoi dung (admin, customer, etc.)
        [Required]
        [MaxLength(20)]
        [Column("role")]
        public string Role { get; set; } = string.Empty;

        // company name (ten cong ty)
        [MaxLength(150)]
        [Column("company_name")]
        public string? CompanyName { get; set; }

        // ngay tao 
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ==========================================
        // NAVIGATION PROPERTIES (Quan hệ)
        // ==========================================

        /// <summary>
        /// Danh sách đơn hàng của user này
        /// Quan hệ 1-N: 1 User → Nhiều Order
        /// </summary>
        public ICollection<Order>? Orders { get; set; }
    }
}