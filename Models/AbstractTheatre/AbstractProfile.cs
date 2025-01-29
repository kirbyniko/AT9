using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("Profiles", Schema = "dbo")]
    public partial class AbstractProfile
    {
        [Key]
        [Column("Profile_ID")]
        [Required]
        public int ProfileId { get; set; }

        [Column("AspNetUserID")]
        public string AspNetUserId { get; set; }

        [Column("ProfilePictureID")]
        public int? ProfilePictureId { get; set; }

        public string Username { get; set; }

        public string Bio { get; set; }
    }
}