using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("DeliveryAddresses", Schema = "dbo")]
    public partial class DeliveryAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DeliveryAddressID")]
        public int DeliveryAddressId { get; set; }

        [Column("ProfileID")]
        public int? ProfileId { get; set; }

        public string Fullname { get; set; }

        public string Zipcode { get; set; }

        public string Addresslineone { get; set; }

        public string Addresslinetwo { get; set; }

        public string City { get; set; }
    }
}