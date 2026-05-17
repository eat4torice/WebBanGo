using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanGo.Models.Entities
{
    [Table("Categories")]
    public class Category
    {
       [Key]
       [Column("id")]
       public int Id { get; set; }

        // ten danh muc san pham
       [Required]
       [MaxLength(100)]
       [Column("name")]
       public string Name { get; set; } = string.Empty;

       // mo ta danh muc san pham
       [Column("description")]
       public string? Description { get; set; }
       // ID của danh mục cha (NULL nếu là danh mục gốc)
       [Column("parent_id")]
       public int? ParentId { get; set; }

        // ==========================================
        // NAVIGATION PROPERTIES (Quan hệ)
        // ==========================================

        /// <summary>
        /// Danh mục cha (self-reference)
        /// </summary>
        [ForeignKey("ParentId")]
        public Category? ParentCategory { get; set; }

        /// <summary>
        /// Các danh mục con (self-reference)
        /// </summary>
        [InverseProperty("ParentCategory")]
        public ICollection<Category>? SubCategories { get; set; }

        /// <summary>
        /// Danh sách sản phẩm thuộc danh mục này
        /// Quan hệ 1-N: 1 Category → Nhiều Product
        /// </summary>
        public ICollection<Product>? Products { get; set; }
    }
}