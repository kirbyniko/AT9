using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using AT9;
using AT9.Components.Pages;
using AT9.Models.AbstractTheatre;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace AT9.Components.Pages
{
    public partial class ProductVariations
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

        protected IEnumerable<AT9.Models.AbstractTheatre.ProductVariation> productVariations;

        protected RadzenDataGrid<AT9.Models.AbstractTheatre.ProductVariation> grid0;

        [Parameter]
        public int ProductId { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            productVariations = await AbstractTheatreService.GetProductVariations();
            List<ProductVariation> pv = productVariations.ToList();
            productVariations = productVariations.Where(x => x.ProductId == ProductId);
            pv = productVariations.ToList();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            var parameters = new Dictionary<string, object>
                 {
                { "ProductId", ProductId } 
                };
            await DialogService.OpenAsync<AddProductVariation>("Add Product Variation", parameters);
            await grid0.Reload();
        }

        protected async Task EditRow(AT9.Models.AbstractTheatre.ProductVariation args)
        {
            await DialogService.OpenAsync<EditProductVariation>("Edit Product Variation", new Dictionary<string, object> { { "ProductVariationId", args.ProductVariationId } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AT9.Models.AbstractTheatre.ProductVariation productVariation)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {

                    var querygalleryimages = await AbstractTheatreService.GetGalleryImages();
                    var queryimages = await AbstractTheatreService.GetImages();
                    List<GalleryImage> images = querygalleryimages.Where(x => x.ProductVariationId == productVariation.ProductVariationId).ToList();

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

                   

                }




                var deleteResult = await AbstractTheatreService.DeleteProductVariation(productVariation.ProductVariationId);

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
                    Detail = $"Unable to delete Product Variation"
                });
            }
        }
    }
}