using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AT9.Models.AbstractTheatre
{
    public class GalleryItem
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Product_Variation_ID")]
        public int? ProductVariationId { get; set; }

        public int? Views { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Column("ProfileID")]
        public int? ProfileID { get; set; }

        public bool? isVideo { get; set; }

        public string? Imgdata { get; set; }




    }
}
