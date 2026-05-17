using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanGo.Models.Entities
{
    [Table("Order_Items")]
    public class OrderItem
        {
            [Key]
            [Column("id")]
            public int Id { get; set; }

            [Column("order_id")]
            public int? OrderId { get; set; }

            [Column("product_id")]
            public int? ProductId { get; set; }

            [Required]
            [Column("quantity", TypeName = "decimal(10,2)")]
            public decimal Quantity { get; set; }

            [Required]
            [Column("price_at_time", TypeName = "decimal(15,2)")]
            public decimal PriceAtTime { get; set; }

            [ForeignKey("OrderId")]
            public Order? Order { get; set; }

            [ForeignKey("ProductId")]
            public Product? Product { get; set; }
        }
}
