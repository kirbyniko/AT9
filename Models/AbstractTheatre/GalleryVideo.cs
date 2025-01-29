using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("GalleryVideos", Schema = "dbo")]
    public partial class GalleryVideo : GalleryItem
    {
       
        public string? VideoLink { get; set; }

        [Column("isProfileVideo")]
        public bool? IsProfileVideo { get; set; }

 
    }
}