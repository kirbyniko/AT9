using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("VideoViews", Schema = "dbo")]
    public partial class VideoView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("VideoView_ID")]
        public int VideoViewId { get; set; }

        [Column("GalleryVideo_ID")]
        [Required]
        public int GalleryVideoId { get; set; }

        [Required]
        public DateTime ViewTime { get; set; }
    }
}