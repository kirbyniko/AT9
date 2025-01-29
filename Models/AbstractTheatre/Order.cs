using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("Orders", Schema = "dbo")]
    public partial class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Order_ID")]
        public int OrderId { get; set; }

        [Column("Artwork_ID")]
        [Required]
        public int ArtworkId { get; set; }

      
        public DateTime Time { get; set; }

        public bool Pending { get; set; }

        [Column("Buyer_ID")]
        public int? BuyerId { get; set; }

        public double Total { get; set; }

        [Column("PaymentMethodID")]
        public int PaymentMethodID { get; set; }

        [Column("DeliveryAddressID")]
        public int DeliveryAddressID { get; set; }
    }
}