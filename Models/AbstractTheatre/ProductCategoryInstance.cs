using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("Product_Category_Instances", Schema = "dbo")]
    public partial class ProductCategoryInstance
    {
        [Key]
        [Column("Product_Category_Instance_ID")]
        [Required]
        public string ProductCategoryInstanceId { get; set; }

        [Column("Product_ID")]
        public string ProductId { get; set; }

        [Column("Category_ID")]
        public string CategoryId { get; set; }
    }
}