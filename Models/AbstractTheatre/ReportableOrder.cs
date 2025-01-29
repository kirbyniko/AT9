using Microsoft.AspNetCore.Components;

namespace AT9.Models.AbstractTheatre
{
    public class ReportableOrder
    {

        public string Artname { get; set; }

        public DateTime Date { get; set; }

        public string Buyername { get; set; }
        public double Total { get; set; }
        public string PaymentMethod { get; set; }
        public string DeliveryAddress { get; set; }


    }
}
