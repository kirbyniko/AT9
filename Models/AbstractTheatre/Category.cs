using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("Categories", Schema = "dbo")]
    public partial class Category
    {
        [Key]
        [Column("Category_ID")]
        [Required]
        public int CategoryId { get; set; }

        [Column("Category_Name")]
        public string CategoryName { get; set; }
    }
}