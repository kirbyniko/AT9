using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AT9.Models.AbstractTheatre
{
    [Table("PaymentMethods", Schema = "dbo")]
    public partial class PaymentMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("PaymentMethodID")]
        public int PaymentMethodId { get; set; }

        [Column("ProfileID")]
        public int? ProfileId { get; set; }

        public string Cardnumber { get; set; }

        public string Cardname { get; set; }

        public string Expiration { get; set; }

        public string Securitycode { get; set; }

        public string Fullname { get; set; }

        public string Zipcode { get; set; }

        public string Addresslineone { get; set; }

        public string Addresslinetwo { get; set; }

        public string City { get; set; }
    }
}