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
    public partial class ShoppingCart
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
        public AbstractTheatreService AbstractTheatreService { get; set; }

        public List<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();

          public List<CartProductUserRelationship> Relationships { get; set; } = new List<CartProductUserRelationship>();

        public List<PageProduct> PageProducts { get; set; } = new List<PageProduct>();

        [CascadingParameter]

        public MainLayout Layout { get; set; }




        public double Total { get; set; }
        protected override async Task OnInitializedAsync()
        {
          SetProducts();
        }

        public async void SetProducts()
        {
              var productvariations = await AbstractTheatreService.GetProductVariations();         
            var cartproduct = await AbstractTheatreService.GetCartProductUserRelationships(); 
            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);
           
            cartproduct = cartproduct.Where(x => x.ProfileId == profileid); 

            ProductVariations.Clear();  
            PageProducts.Clear();
            Relationships.Clear();

            Total = 0;

            foreach(var cart in cartproduct)
            {
                int count = (cartproduct.Where(x => x.ProductVariationId == cart.ProductVariationId)).Count();
            
                ProductVariation variation = await AbstractTheatreService.GetProductVariationByProductVariationId(cart.ProductVariationId);

                if (count > variation.Stock)
                {
                    await AbstractTheatreService.DeleteCartProductUserRelationship(cart.CartUserRelationshipId);

                    cartproduct = await AbstractTheatreService.GetCartProductUserRelationships();
                    cartproduct = cartproduct.Where(x => x.ProfileId == profileid);

                }
                else
                {

                    ProductVariations.Add(variation);
                }

                
            } 
            
            Relationships = cartproduct.ToList();

            foreach (var productVariation in ProductVariations)
            {
                PageProducts.Add(new PageProduct(productVariation));
                Total = Total + productVariation.Price;

            }
            Total = (Math.Round(Total, 2));
            foreach (PageProduct pageproduct in PageProducts)
            {
                pageproduct.Imgdata = await FindThumbnail(pageproduct.ProductVariationId);
            }
            Layout.BalanceCart();
            StateHasChanged();

        }

        public async void IncrementProduct(int productvariationid)
        {
            CartProductUserRelationship relationship = Relationships.First(x => x.ProductVariationId == productvariationid);
            ProductVariation product = ProductVariations.First(x => x.ProductVariationId == productvariationid);
            int count = Relationships.Where(x => x.ProductVariationId == x.ProductVariationId).Count();

            if(count < product.Stock)
            {
                CartProductUserRelationship relation = new CartProductUserRelationship();
                relation.ProductVariationId = productvariationid;
                relation.ProfileId = await AbstractTheatreService.GetProfileID(Security.User.Id);
                await AbstractTheatreService.CreateCartProductUserRelationship(relation);
                SetProducts();
            }
            else
            {
                DialogService.Alert("There's not enough stock to support this.");
            }
           
        }

         public async void DecrementProduct(int productvariationid)
        {
            CartProductUserRelationship relationship = Relationships.First(x => x.ProductVariationId == productvariationid);
            ProductVariation product = ProductVariations.First(x => x.ProductVariationId == productvariationid);
            int count = Relationships.Where(x => x.ProductVariationId == x.ProductVariationId).Count();
          
                await AbstractTheatreService.DeleteCartProductUserRelationship(relationship.CartUserRelationshipId);
            SetProducts();
        }

         public async void RemoveProduct(int productvariationid)
        {
            CartProductUserRelationship relationship = Relationships.First(x => x.ProductVariationId == productvariationid);
            foreach(var x in Relationships)
            {
                if(x.ProductVariationId == productvariationid)
                {
                    await AbstractTheatreService.DeleteCartProductUserRelationship(x.CartUserRelationshipId);
                }
            }
            SetProducts();
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
                    return ("images/Default_Product_Picture.png");
                }

                AbstractImage x = await AbstractTheatreService.GetImageByImageId(galleryImage.ImageId);
                return BytetoImageBase64(x.Image1);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ("images/Default_Product_Picture.png");

            }
        }

        public void NavigateToProduct(int ID)
        {
            NavigationManager.NavigateTo($"/product-viewing/{ID}");
        }

        public void NavigateToCheckout()
        {
            NavigationManager.NavigateTo($"/checkout");
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