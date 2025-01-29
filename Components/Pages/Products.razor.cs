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

namespace AT9.Components.Pages
{
    public partial class Products
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

        protected IEnumerable<AT9.Models.AbstractTheatre.Product> products;

        protected RadzenDataGrid<AT9.Models.AbstractTheatre.Product> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);
            products = await AbstractTheatreService.GetProducts();
            products = products.Where(x => x.ProfileId == profileid);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddProduct>("Add Product", null);
            await grid0.Reload();
        }

        protected async Task EditRow(AT9.Models.AbstractTheatre.Product args)
        {
            await DialogService.OpenAsync<EditProduct>("Edit Product", new Dictionary<string, object> { {"ProductId", args.ProductId} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AT9.Models.AbstractTheatre.Product product)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var x = await AbstractTheatreService.GetProductVariations();
                    List<ProductVariation> productVariations = x.Where(x => x.ProductId == product.ProductId).ToList();

                    foreach (var variation in productVariations) 
                    {
                        var querygalleryimages = await AbstractTheatreService.GetGalleryImages();
                        var queryimages = await AbstractTheatreService.GetImages();
                        List<GalleryImage> images = querygalleryimages.Where(x => x.ProductVariationId == variation.ProductVariationId).ToList();

                        foreach (GalleryImage image in images)
                        {
                            List<GalleryImage> galleryimages = querygalleryimages.Where(x => x.ID == image.ID).ToList();
                            foreach (var galleryimage in galleryimages)
                            {
                                await AbstractTheatreService.DeleteImage(galleryimage.ImageId);
                                await AbstractTheatreService.DeleteGalleryImage(galleryimage.ID);
                            }


                            StateHasChanged();
                        }

                        await AbstractTheatreService.DeleteProductVariation(variation.ProductVariationId);
                    }

                }

                var deleteResult = await AbstractTheatreService.DeleteProduct(product.ProductId);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            

            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Product"
                });
            }
        }
    }
}