using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("ImageViews", Schema = "dbo")]
    public partial class ImageView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ImageView_ID")]
        public int ImageViewId { get; set; }

        [Column("GalleryImage_ID")]
        [Required]
        public int GalleryImageId { get; set; }

        [Required]
        public DateTime ViewTime { get; set; }
    }
}