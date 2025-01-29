using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("Products", Schema = "dbo")]
    public partial class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Product_ID")]
        public int ProductId { get; set; }

        [Column("Product_Name")]
        public string ProductName { get; set; }

        [Column("Profile_ID")]
        public int ProfileId { get; set; }
    }
}