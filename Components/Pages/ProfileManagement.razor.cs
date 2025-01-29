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
using static AT9.Components.Pages.AddProductVariation;
using System.Drawing;
using Microsoft.AspNetCore.Components;

namespace AT9.Components.Pages
{
    public partial class ProfileManagement
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
        protected AbstractTheatreService ATService { get; set; }

        public bool hasProfilePicture { get; set; } = false;

        public AbstractImage ProfilePic { get; set; } = new AbstractImage();

        public AbstractProfile Profile = new AbstractProfile();

        public List<UploadedFile> UploadedFiles { get; set; } = new();

        public List<ProductVariation> CustomPackages { get; set; } = new();

        public GalleryVideo GalleryVideo { get; set; } = new GalleryVideo();

        public List<ImageView> Views { get; set; } = new List<ImageView>();

        public List<VideoView> VideoViews { get; set; } = new List<VideoView>();

        public List<ReportableView> ReportViews { get; set; } = new List<ReportableView>();

        public IEnumerable<ReportableOrder> Sales { get; set; } = new List<ReportableOrder>();

        public IEnumerable<ReportableOrder> Purchases { get; set; } = new List<ReportableOrder>();

        public IEnumerable<ReportableView> ReportViews1 { get; set; }

        public bool isEditingBio { get; set; } = false;

        public string EditingBioText;

        public bool isEditingVideo { get; set; } = false;

        public string VideoLink;
        public async Task OnFileChange(UploadChangeEventArgs args, string message)
        {
            try
            {
                UploadedFiles.Clear();

                if (args != null && args.Files != null && args.Files.Any())
                {
                    foreach (var file in args.Files)
                    {
                        try
                        {
                            using (var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024)) 
                            {
                                var memoryStream = new MemoryStream();

                                await stream.CopyToAsync(memoryStream);

                                UploadedFiles.Add(new UploadedFile
                                {
                                    FileName = file.Name,
                                    ContentType = file.ContentType,
                                    Size = file.Size,
                                    StreamData = memoryStream.ToArray() 
                                });
                            }
                        }
                        catch (Exception fileException)
                        {
                            DialogService.Alert("File too big, we have a one MB maximum. File will not be uploaded.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No files were provided.");
                }
            }
            catch (Exception ex)
            {
                DialogService.Alert("Unexpected error.");
            }

            SetProfilePic();
            StateHasChanged();
        }

        protected async System.Threading.Tasks.Task CancelClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {

            isEditingBio = false;
            StateHasChanged();

        }

        protected async System.Threading.Tasks.Task CancelVideoClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {

            isEditingVideo = false;
            StateHasChanged();

        }


        protected override async Task OnInitializedAsync()
        {
            var queryvideos = await ATService.GetGalleryVideos();
            var queryprofiles = await ATService.GetProfiles();
            var queryviews = await ATService.GetImageViews();
            var queryvideoviews = await ATService.GetVideoViews();
            var querygallery = await ATService.GetGalleryImages();
            var querygalleryvideos = await ATService.GetGalleryVideos();
            var queryvariations = await ATService.GetProductVariations();
            var queryproducts = await ATService.GetProducts();
            int profileid = await ATService.GetProfileID(Security.User.Id);
            var queryorders = await ATService.GetOrders();


            List<Product> products = queryproducts.Where(x => x.ProfileId == profileid).ToList();

            List<ProductVariation> productsvariations = queryvariations.ToList();

            List<ProductVariation> myproductvariations = queryvariations.Where(x => x.ProfileID == profileid).ToList();


            List<GalleryImage> images = querygallery.Where(x => x.ProfileID == profileid).ToList();
             
            List<GalleryVideo> videos = querygalleryvideos.Where(x => x.ProfileID == profileid).ToList();

            List<Order> orders = queryorders.Where(x => x.BuyerId == profileid).ToList();

            List<Order> allorders = queryorders.ToList();


            List<ReportableOrder> reportableOrders = new List<ReportableOrder>();

            List<ReportableOrder> sales = new List<ReportableOrder>();

            List<Order> purchases = new List<Order>();


            foreach (var x in orders) 
            {
                ReportableOrder order = new ReportableOrder();
                if(productsvariations.Any(y => y.ProductVariationId == x.ArtworkId) == true)
                {
                    order.Artname = productsvariations.First(y => y.ProductVariationId == x.ArtworkId).Name;

                }
                else {
                    order.Artname = "Product no longer available";
                }
                order.Buyername = queryprofiles.First(y => y.ProfileId == x.BuyerId).Username;
                order.Date = x.Time;
                order.Total = x.Total;
                order.DeliveryAddress = (await ATService.GetDeliveryAddressByDeliveryAddressId(x.DeliveryAddressID)).Addresslineone;
                order.PaymentMethod = (await ATService.GetPaymentMethodByPaymentMethodId(x.PaymentMethodID)).Cardname;
                reportableOrders.Add(order);
            }

            Purchases = reportableOrders;

            foreach(var x in myproductvariations)
            {
                purchases.AddRange(allorders.Where(y => y.ArtworkId == x.ProductVariationId).ToList());
            }

            foreach(var x in purchases) 
            {
                ReportableOrder order = new ReportableOrder();
                order.Artname = productsvariations.First(y => y.ProductVariationId == x.ArtworkId).Name;
                order.Buyername = queryprofiles.First(y => y.ProfileId == x.BuyerId).Username;
                order.Date = x.Time;
                order.Total = x.Total;
                order.DeliveryAddress = (await ATService.GetDeliveryAddressByDeliveryAddressId(x.DeliveryAddressID)).Addresslineone;
                order.PaymentMethod = (await ATService.GetPaymentMethodByPaymentMethodId(x.PaymentMethodID)).Cardname;
                sales.Add(order);

            }

            Sales = sales;


            foreach (var image in images)
            {
                List<ImageView> views = queryviews.Where(x => x.GalleryImageId == image.ID).ToList();
                Views.AddRange(views);
            }
            foreach (var video in videos)
            {
                List<VideoView> views = queryvideoviews.Where(x => x.GalleryVideoId == video.ID).ToList();
                VideoViews.AddRange(views);
            }
            foreach (var x in Views)
            {
                ReportableView view = new ReportableView(x);
                view.ArtworkName = images.First(y => y.ID == x.GalleryImageId).Name;
                view.IsVideo = false;
                ReportViews.Add(view);

            }
            foreach (var x in VideoViews)
            {
                ReportableView view = new ReportableView(x);
                view.ArtworkName = videos.First(y => y.ID == x.GalleryVideoId).Name;

                ReportViews.Add(view);

            }


            List<AbstractProfile> profiles = queryprofiles.Where(x => x.AspNetUserId == Security.User.Id).ToList();
            Profile = profiles.FirstOrDefault();

            List<GalleryVideo> videoList = queryvideos.Where(x => x.ProfileID == Profile.ProfileId).ToList();
            videoList = videoList.Where(x => x.IsProfileVideo == true).ToList();
            if (videoList.Count > 0)
            {
                GalleryVideo = videoList[0];
                VideoLink = GalleryVideo.VideoLink;
            }

            EditingBioText = Profile.Bio;
            ReportViews1 = ReportViews;
            SetProfilePic();

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
        public async void SetProfilePic()
        {
            var profilequeryable = await ATService.GetProfiles();
            var galleryimagequeryable = await ATService.GetGalleryImages();
            var abstractqueryable = await ATService.GetImages();

            List<AbstractProfile> profilelist = profilequeryable.ToList();
            List<GalleryImage> galleryImages = new List<GalleryImage>();
            try
            {
                galleryImages = galleryimagequeryable.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            List<AbstractImage> abstractImages = abstractqueryable.ToList();

            int profileid = await ATService.GetProfileID(Security.User.Id);
            try
            {
                AbstractProfile profile = profilelist.First(x => x.AspNetUserId == Security.User.Id);
                galleryImages = galleryImages.Where(x => x.IsProfilePic == true).ToList();
                GalleryImage galleryImage = galleryImages.First(x => x.ProfileID == profileid);
                ProfilePic = abstractImages.First(x => x.ImageId == galleryImage.ImageId);

                hasProfilePicture = true;
            }
            catch (Exception ex)
            {
                hasProfilePicture = false;
            }
        }

        protected async System.Threading.Tasks.Task SaveClick1(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            int profileid = await ATService.GetProfileID(Security.User.Id);
            foreach (UploadedFile file in UploadedFiles)
            {
                var galleryimagequeryable = await ATService.GetGalleryImages();

                List<GalleryImage> galleryImages = galleryimagequeryable.Where(x => x.ProfileID == profileid).ToList();

                foreach (GalleryImage galleryimage in galleryImages)
                {
                    await ATService.DeleteImage(galleryimage.ImageId);
                    await ATService.DeleteGalleryImage(galleryimage.ID);
                }

                AbstractImage image = new AbstractImage();
                image.Image1 = file.StreamData;

                await ATService.CreateImage(image);

                int imageid = image.ImageId;
                GalleryImage galleryImage = new GalleryImage();
                galleryImage.IsProfilePic = true;
                galleryImage.ImageId = imageid;
                galleryImage.ProfileID = profileid;
                galleryImage.Name = file.FileName;
                galleryImage.Position = file.Position;
                hasProfilePicture = true;
                ProfilePic = image;

                await ATService.CreateGalleryImage(galleryImage);
                StateHasChanged();

            }
        }

        protected async System.Threading.Tasks.Task EditSaveClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            if (isEditingBio == false)
            {
                isEditingBio = true;
                StateHasChanged();

            }
            else
            {
                isEditingBio = false;
                Profile.Bio = EditingBioText;
                await ATService.UpdateProfile(Profile.ProfileId, Profile);

                var queryprofiles = await ATService.GetProfiles();
                List<AbstractProfile> profiles = queryprofiles.Where(x => x.AspNetUserId == Security.User.Id).ToList();
                Profile = profiles.FirstOrDefault();

                StateHasChanged();

            }
        }

        protected async System.Threading.Tasks.Task EditSaveVideoClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            if (isEditingVideo == false)
            {
                isEditingVideo = true;
                StateHasChanged();
            }
            else
            {
                isEditingVideo = false;

                GalleryVideo.VideoLink = VideoLink;

                if (await YouTubeAPI.IsEmbeddableYouTubeVideoAsync(GalleryVideo.VideoLink) == true)
                {
                    var galleryvideos = await ATService.GetGalleryVideos();
                    List<GalleryVideo> videos = galleryvideos.Where(x => x.ProfileID == Profile.ProfileId).ToList();
                    videos = videos.Where(x => x.IsProfileVideo == true).ToList();

                    GalleryVideo.ProfileID = Profile.ProfileId;
                    GalleryVideo.IsProfileVideo = true;

                    if (videos.Count == 0)
                    {
                        await ATService.CreateGalleryVideo(GalleryVideo);

                    }
                    if (videos.Count == 1)
                    {
                        await ATService.UpdateGalleryVideo(GalleryVideo.ID, GalleryVideo);
                    }
                }
                else
                {
                    await DialogService.Alert("Invalid Youtube link, video not saved!", "Error");
                    VideoLink = "";
                }
                StateHasChanged();
            }
        }
    }
}