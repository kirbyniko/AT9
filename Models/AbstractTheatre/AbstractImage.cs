using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("Images", Schema = "dbo")]
    public partial class AbstractImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }

        [Column("Image")]
        public byte[] Image1 { get; set; }
    }
}