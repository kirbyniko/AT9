using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using static AT9.Components.Pages.AddProductVariation;
using AT9.Models.AbstractTheatre;

namespace AT9.Components.Pages
{
    public partial class GalleryImages
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

        protected IEnumerable<AT9.Models.AbstractTheatre.GalleryImage> galleryImages;

        protected RadzenDataGrid<AT9.Models.AbstractTheatre.GalleryImage> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);
            galleryImages = await AbstractTheatreService.GetGalleryImages();
            galleryImages = galleryImages.Where(x => x.ProfileID == profileid);
            galleryImages = galleryImages.Where(x => x.ProductVariationId == null);
            galleryImages = galleryImages.Where(x => x.IsProfilePic == null);


        }

       


        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddGalleryImage>("Add Gallery Image", null);
            await grid0.Reload();
        }

        protected async Task EditRow(AT9.Models.AbstractTheatre.GalleryImage args)
        {
            await DialogService.OpenAsync<EditGalleryImage>("Edit Gallery Image", new Dictionary<string, object> { {"GalleryImageId", args.ID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AT9.Models.AbstractTheatre.GalleryImage galleryImage)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    
                    var deleteResult = await AbstractTheatreService.DeleteGalleryImage(galleryImage.ID);
                    await AbstractTheatreService.DeleteImage(galleryImage.ID);
                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Gallery Image"
                });
            }
        }
    }
}