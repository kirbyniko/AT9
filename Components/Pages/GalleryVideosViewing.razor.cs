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
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml;
using System.Drawing;
using AT9.Models;

namespace AT9.Components.Pages
{
    public partial class GalleryVideosViewing
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

        [Parameter]
        public int ID { get; set; }

        public GalleryVideo GalleryVideo { get; set; } = new GalleryVideo();

        string Username { get; set; } = "gloob";

        public string Imgdata { get; set; }

        public int Views { get; set; }

        public AbstractProfile Profile { get; set; } = new AbstractProfile();
        protected override async void OnInitialized()
        {
   
            if (ID != 0 && ID != null)
            {

                GalleryVideo = await AbstractTheatreService.GetGalleryVideoByGalleryVideoId(ID);

                VideoView videoview = new VideoView();

                videoview.ViewTime = DateTime.UtcNow;
                videoview.GalleryVideoId = GalleryVideo.ID;
                await AbstractTheatreService.CreateVideoView(videoview);
                var imageview = await AbstractTheatreService.GetVideoViews();
                List<VideoView> imgviews = imageview.Where(x => x.GalleryVideoId == GalleryVideo.ID).ToList();
                Views = imgviews.Count();
                Profile = await AbstractTheatreService.GetProfileByProfileId(GalleryVideo.ProfileID??0);
                Username = Profile.Username;  
            }
            else
            {
                NavigationManager.NavigateTo($"");

            }

        }

        public void NavigateToProfile(int ID)
        {
            NavigationManager.NavigateTo($"/profile-viewing/{ID}");
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