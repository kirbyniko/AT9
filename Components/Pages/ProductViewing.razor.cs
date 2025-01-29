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
using AT9.Services;
using AT9.Components.Layout;

namespace AT9.Components.Pages
{
    public partial class ProductViewing
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
        public AbstractTheatreService AbstractTheatreService { get; set; }

        [Parameter]
        public int ID { get; set; }

        [Parameter]
        public bool isCustom { get; set; }

        private int ProductId;
        public string Imgdata { get; set; }

        public int Amount = 1;

        public ProductVariation productVariation = new ProductVariation();

        public Product Product { get; set; } = new Product();

        public AbstractProfile Profile { get; set; } = new AbstractProfile();

        [Inject]
        protected SecurityService Security { get; set; }

        public List<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();

        public List<AbstractImage> AbstractImages { get; set; } = new List<AbstractImage>();

        RadzenCarousel carousel;

        [CascadingParameter]
        public MainLayout Layout { get; set; }

        protected override async void OnInitialized()
        {
            ProductId = ID;
            var productvariations = await AbstractTheatreService.GetProductVariations();
            var abstractimages = await AbstractTheatreService.GetImages();
            var galleryimages = await AbstractTheatreService.GetGalleryImages();

            Product = await AbstractTheatreService.GetProductByProductId(ProductId);
            if (Product == null)
            {
                productVariation = await AbstractTheatreService.GetProductVariationByProductVariationId(ID);
            }
            else
            {

                ProductVariations = productvariations.Where(x => x.ProductId == ProductId).ToList();
                productVariation = ProductVariations[0];
            }

                List<GalleryImage> galleryImages = galleryimages.Where(x => x.ProductVariationId == productVariation.ProductVariationId).ToList();
                galleryImages = galleryImages.OrderBy(x => x.Position).ToList();
                foreach (var y in galleryImages)
                {
                    AbstractImages.Add(abstractimages.First(x => x.ImageId == y.ImageId));
                }
                int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);

            Profile = await AbstractTheatreService.GetProfileByProfileId(profileid);
            }
        

        public void NavigateToProfile(int ID)
        {
            NavigationManager.NavigateTo($"/profile-viewing/{ID}");
        }
        public async Task<string> FindThumbnail(int ProductVariationID)
        {
            var galleryimages = await AbstractTheatreService.GetGalleryImages();
            var abstractimages = await AbstractTheatreService.GetImages();

            List<AbstractImage> images = new List<AbstractImage>();

            List<GalleryImage> galleryImages = galleryimages.Where(x => x.ProductVariationId == ProductVariationID).ToList();
            foreach (var y in galleryImages)
            {
                images.Add(abstractimages.First(x => x.ImageId == y.ImageId));
            }
            if(images.Count > 0)
            {
                AbstractImage x = await AbstractTheatreService.GetImageByImageId(images.First().ImageId);
                return BytetoImageBase64(x.Image1);
            }
            else
            {
                return "";
            }
           
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

        public async void AddtoCart()
        {
            var query = await AbstractTheatreService.GetCartProductUserRelationships();
            List<CartProductUserRelationship> relationships = query.Where(x => x.ProductVariationId == productVariation.ProductVariationId).ToList();
            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);
            relationships = relationships.Where(x => x.ProfileId == profileid).ToList();
            int count = relationships.Count();
            int x = 0;

            if (productVariation.Stock >= (Amount + count)) 
            {
                
                while (x != Amount)
                {
                    CartProductUserRelationship relationship = new CartProductUserRelationship();

                    relationship.ProductVariationId = productVariation.ProductVariationId;
                    relationship.ProfileId = await AbstractTheatreService.GetProfileID(Security.User.Id);
                    AbstractTheatreService.CreateCartProductUserRelationship(relationship);
                    x++;
                    Layout.UpdateCart();
                }
            }
            else
            {
                if (count > 0) 
                {
                    DialogService.Alert($"There isn't enough stock to support this! You already have {count} of these in your cart!");
                }
                else
                {
                    DialogService.Alert($"There isn't enough stock to support this!");

                }
            }

            StateHasChanged();
         

        }


        protected async System.Threading.Tasks.Task SelectedProductVariation(System.Object args)
        {
             var galleryimages = await AbstractTheatreService.GetGalleryImages();
            var abstractimages = await AbstractTheatreService.GetImages();

            AbstractImages.Clear();

             
            List<GalleryImage> galleryImages = galleryimages.Where(x => x.ProductVariationId == ((ProductVariation)args).ProductVariationId).ToList();
            galleryImages = galleryImages.OrderBy(x => x.Position).ToList();

            foreach (var y in galleryImages)
             {
             AbstractImages.Add(abstractimages.First(x => x.ImageId == y.ImageId));
            }
            StateHasChanged();
         }
     }


}