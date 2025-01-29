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
    public partial class GalleryVideos
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

        protected IEnumerable<AT9.Models.AbstractTheatre.GalleryVideo> galleryVideos;

        protected RadzenDataGrid<AT9.Models.AbstractTheatre.GalleryVideo> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);
            galleryVideos = await AbstractTheatreService.GetGalleryVideos();
            galleryVideos = galleryVideos.Where(x => x.ProfileID == profileid);
            galleryVideos = galleryVideos.Where(x => x.ProductVariationId == null);
            galleryVideos = galleryVideos.Where(x => x.IsProfileVideo == null);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddGalleryVideo>("Add Gallery Video", null);
            await grid0.Reload();
        }

        protected async Task EditRow(AT9.Models.AbstractTheatre.GalleryVideo args)
        {
            await DialogService.OpenAsync<EditGalleryVideo>("Edit Gallery Video", new Dictionary<string, object> { {"GalleryVideoId", args.ID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AT9.Models.AbstractTheatre.GalleryVideo galleryVideo)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AbstractTheatreService.DeleteGalleryVideo(galleryVideo.ID);

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
                    Detail = $"Unable to delete Gallery Video"
                });
            }
        }
    }
}