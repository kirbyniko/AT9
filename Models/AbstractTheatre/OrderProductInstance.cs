using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("OrderProductInstances", Schema = "dbo")]
    public partial class OrderProductInstance
    {
        [Key]
        [Column("OrderProductID")]
        [Required]
        public int OrderProductId { get; set; }

        [Column("OrderID")]
        public int? OrderId { get; set; }

        [Column("ProductVariationID")]
        public int? ProductVariationId { get; set; }
    }
}