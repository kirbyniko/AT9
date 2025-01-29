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
    public partial class AddGalleryVideo
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

        public AbstractImage Image { get; set; } = new AbstractImage();

        protected override async Task OnInitializedAsync()
        {
            galleryVideo = new AT9.Models.AbstractTheatre.GalleryVideo();
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.GalleryVideo galleryVideo;

        [Inject]
        protected SecurityService Security { get; set; }

        public async Task<string> Validate()
        {

            if (galleryVideo.Name == null || galleryVideo.Name == "")
            {
                return "Name is required";
            }
            if (galleryVideo.Description == null || galleryVideo.Description == "")
            {
                return "Description is required";
            }
            if (galleryVideo.VideoLink == null || galleryVideo.VideoLink == "")
            {
                return "Video Link is required";
            }
            if(await YouTubeAPI.IsEmbeddableYouTubeVideoAsync(galleryVideo.VideoLink) == false)
            {
                return "Invalid YouTube video link";
            }
      


            return "";
        }

        protected async Task FormSubmit()
        {
            if (await Validate() != "")
            {
                DialogService.Alert(await Validate(), "Error");
                return;
            }
            else
            {
                try
                {
                    string videoid = YouTubeAPI.ExtractVideoId(galleryVideo.VideoLink);
                    galleryVideo.VideoLink = YouTubeAPI.EmbedYouTubeVideo(videoid);
                    galleryVideo.ProfileID = await AbstractTheatreService.GetProfileID(Security.User.Id);   
                    await AbstractTheatreService.CreateGalleryVideo(galleryVideo);
                    DialogService.Close(galleryVideo);
                }
                catch (Exception ex)
                {
                    errorVisible = true;
                }
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}