using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using AT9.Models.AbstractTheatre;
using System.Drawing;
using AT9.Components.Layout;

namespace AT9.Components.Pages
{
    public partial class Checkout
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected AbstractTheatreService AbstractTheatreService { get; set; }

        public List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

        public List<DeliveryAddress> deliveryAddresses = new List<DeliveryAddress>();

        public List<PageProduct> PageProducts = new List<PageProduct>();

        public DeliveryAddress DeliveryAddress { get; set; } = new DeliveryAddress();

        public PaymentMethod PaymentMethod { get; set; } = new PaymentMethod();

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        public int ProfileID;

        public double Total;

        protected override async Task OnInitializedAsync()
        {
            ProfileID = await AbstractTheatreService.GetProfileID(Security.User.Id);
            var paymentquery = await AbstractTheatreService.GetPaymentMethods();
            var deliveryquery = await AbstractTheatreService.GetDeliveryAddresses();
            var relationshipsquery = await AbstractTheatreService.GetCartProductUserRelationships();
            List<CartProductUserRelationship> relations = new List<CartProductUserRelationship>();
            List<ProductVariation> productVariations = new List<ProductVariation>();

            paymentMethods = paymentquery.Where(x => x.ProfileId == ProfileID).ToList();
            deliveryAddresses = deliveryquery.Where(x => x.ProfileId == ProfileID).ToList();
            relations = relationshipsquery.Where(x => x.ProfileId == ProfileID).ToList();

            foreach(var x in relations)
            {
                ProductVariation variation = await AbstractTheatreService.GetProductVariationByProductVariationId(x.ProductVariationId);
                productVariations.Add(variation);
            }

             foreach (var productVariation in productVariations)
            {
                PageProducts.Add(new PageProduct(productVariation));
                Total = Total + productVariation.Price;

            }

            foreach (PageProduct pageproduct in PageProducts)
            {
                pageproduct.Imgdata = await FindThumbnail(pageproduct.ProductVariationId);
            }
        }
        public async Task<string> FindThumbnail(int ProductVariationID)
        {
            List<GalleryImage> galleryImages = new List<GalleryImage>();

            try
            {
                var y = await AbstractTheatreService.GetGalleryImages();
                galleryImages = y.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            GalleryImage galleryImage = new GalleryImage();

            try
            {
                galleryImage = galleryImages.FirstOrDefault(x => x.ProductVariationId == ProductVariationID);
                if (galleryImage == null)
                {
                    return ("images/Default_Profile_Pic.png");
                }

                AbstractImage x = await AbstractTheatreService.GetImageByImageId(galleryImage.ImageId);
                return BytetoImageBase64(x.Image1);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ("images/Default_Profile_Pic.png");

            }
        }

        public async void PlaceOrder()
        {
            bool placeorder = true;
            if(deliveryAddresses.Count() < 1)
            {
                placeorder = false;
                DialogService.Alert("Please add a delivery address to checkout!");
                return;
            }
            if(paymentMethods.Count() < 1)
            {
                placeorder = false;
                DialogService.Alert("Please add a payment method to checkout!");
                return;

            }

            bool orderready = true;

            if(PaymentMethod.Cardname == "" || PaymentMethod.Cardname == null) 
            {
                orderready = false;
            }
            if(DeliveryAddress.Fullname == "" || DeliveryAddress.Fullname == null) 
            {
                orderready = false;
            }

            if (orderready == true)
            {

                foreach (var x in PageProducts)
                {
                    Order order = new Order();
                    order.ArtworkId = x.ProductVariationId;
                    order.BuyerId = ProfileID;
                    order.Time = DateTime.UtcNow;
                    order.Total = Total;
                    order.DeliveryAddressID = DeliveryAddress.DeliveryAddressId;
                    order.PaymentMethodID = PaymentMethod.PaymentMethodId;

                    try
                    {
                        await AbstractTheatreService.CreateOrder(order);
                    }
                    catch (Exception ex) { }

                    OrderProductInstance orderProduct = new OrderProductInstance();
                    orderProduct.OrderId = order.OrderId;
                    orderProduct.ProductVariationId = x.ProductVariationId;

                    try {
                        await AbstractTheatreService.CreateOrderProductInstance(orderProduct);

                    }
                    catch (Exception ex)
                    {
                    }

                }

                foreach (var x in PageProducts)
                {
                    ProductVariation productVariation = await AbstractTheatreService.GetProductVariationByProductVariationId(x.ProductVariationId);
                    productVariation.Stock = productVariation.Stock - 1;
                    await AbstractTheatreService.UpdateProductVariation(productVariation.ProductVariationId, productVariation);
                    var userquery = await AbstractTheatreService.GetCartProductUserRelationships();
                    List<CartProductUserRelationship> relationships = userquery.Where(x => x.ProductVariationId == productVariation.ProductVariationId).ToList();
                    foreach (var cartProductUserRelationship in relationships)
                    {
                        await AbstractTheatreService.DeleteCartProductUserRelationship(cartProductUserRelationship.CartUserRelationshipId);
                    }
                }

                Layout.BalanceCart();
                
                if(Total == 0) 
                {
                    await DialogService.Alert("Nothing in your cart, no purchase made!");
                    NavigationManager.NavigateTo("");


                }

                await DialogService.Alert("Successfully created your purchase for $" + Total);
                NavigationManager.NavigateTo("");
            }
            else 
            {
                await DialogService.Alert("Must select a delivery address and a payment method!");
            
            }



        }

        public void NavigateToPaymentMethods()
        {
            NavigationManager.NavigateTo($"/payment-methods");
        }

         public void NavigateToDeliveryAddresses()
        {
            NavigationManager.NavigateTo($"/delivery-addresses");
        }
         public string BytetoImageBase64(byte[] thebytes)
        {
            using (var ms = new MemoryStream(thebytes))
            {
                var image = Image.FromStream(ms); 
                using (var ms2 = new MemoryStream())
                {
                    image.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
                    var byteArray = ms2.ToArray();
                    return $"data:image/png;base64,{Convert.ToBase64String(byteArray)}";
                }
            }
        }

    }
}