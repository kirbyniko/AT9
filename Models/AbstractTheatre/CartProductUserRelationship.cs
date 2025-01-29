using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("CartProduct_User_Relationships", Schema = "dbo")]
    public partial class CartProductUserRelationship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Cart_User_Relationship_ID")]
        public int CartUserRelationshipId { get; set; }

        [Column("ProductVariationID")]
        public int ProductVariationId { get; set; }

        [Column("ProfileID")]
        public int? ProfileId { get; set; }
    }
}