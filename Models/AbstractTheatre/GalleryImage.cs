using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("GalleryImage", Schema = "dbo")]
    public partial class GalleryImage : GalleryItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Image_ID")]
        [Required]
        public int ImageId { get; set; }
        public int? Position { get; set; }

        [Column("isProfilePic")]
        public bool? IsProfilePic { get; set; }
    }
}