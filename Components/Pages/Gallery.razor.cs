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
using System.Collections.ObjectModel;
using System.Drawing;


namespace AT9.Components.Pages
{
    public partial class Gallery
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
        public List<GalleryItem> AllGalleryItems { get; set; } = new List<GalleryItem>();

        public List<GalleryItem> GalleryItems = new List<GalleryItem>();

        public List<GalleryItem> FilteredAndSortedItems { get; set; } = new List<GalleryItem>();

        [Parameter]
        public int ID { get; set; }


        private int CurrentPage { get; set; } = 1;

        private int _productsPerPage = 5;

        private int ProductsPerPage
        {
            get => _productsPerPage;
            set
            {
                _productsPerPage = value; 
                ApplyFiltersAndPagination();
            }
        }

        private int TotalPages;
        private bool HasNextPage => CurrentPage < TotalPages;
        private bool HasPreviousPage => CurrentPage > 1;

        private bool SortAscending { get; set; } = true;

        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }
        protected override async Task OnInitializedAsync()
        {
            List<GalleryImage> GalleryImages = new List<GalleryImage>();
            List<GalleryVideo> GalleryVideos = new List<GalleryVideo>();

            var galleryImages = await AbstractTheatreService.GetGalleryImages();
            var galleryVideos = await AbstractTheatreService.GetGalleryVideos();
            var imageviews = await AbstractTheatreService.GetImageViews();
            var videoviews = await AbstractTheatreService.GetVideoViews();

            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);

            GalleryImages = galleryImages.ToList();
            GalleryVideos = galleryVideos.ToList();

            foreach (var galleryimage in GalleryImages)
            {
                if (galleryimage.ProductVariationId == null && galleryimage.IsProfilePic == null)
                {
                    GalleryItem item = new();
                    item.ID = galleryimage.ID;
                    item.isVideo = false;
                    item.Description = galleryimage.Description;
                    item.Name = galleryimage.Name;
                    item.ProfileID = profileid;
                    item.Views = imageviews.Where(x => x.GalleryImageId == galleryimage.ID).Count();
                    item.Imgdata = await FindThumbnail(galleryimage.ID);
                    AllGalleryItems.Add(item);
                }
            }

            foreach (var galleryvideo in GalleryVideos)
            {
                if (galleryvideo.ProductVariationId == null && galleryvideo.IsProfileVideo == null)
                {
                    GalleryItem item = new();
                    item.ID = galleryvideo.ID;
                    item.isVideo = true;
                    item.Description = galleryvideo.Description;
                    item.Name = galleryvideo.Name;
                    item.ProfileID = profileid;
                    item.Views = videoviews.Where(x => x.GalleryVideoId == galleryvideo.ID).Count();
                    string videoid = YouTubeAPI.ExtractVideoId(galleryvideo.VideoLink);
                    item.Imgdata = YouTubeAPI.GetThumbnail(videoid);
                    AllGalleryItems.Add(item);
                }
            }

            if(ID != 0 && ID != null ) 
            {
                AllGalleryItems = AllGalleryItems.Where(x => x.ProfileID == ID).ToList();
            }


            foreach (GalleryItem pageproduct in AllGalleryItems)
            {
                if(pageproduct.isVideo == false)
                {
                    List<ImageView> imageviews1 = imageviews.Where(x => x.GalleryImageId == pageproduct.ID).ToList();
                    pageproduct.Views = imageviews1.Count;
                    pageproduct.Imgdata = await FindThumbnail(pageproduct.ID);
                }
              
            }

            ApplyFiltersAndPagination();
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)FilteredAndSortedItems.Count / ProductsPerPage));

            LoadProducts(CurrentPage);
        }


        private void OnProductsPerPageChange(ChangeEventArgs e)
        {
            ProductsPerPage = int.Parse(e.Value.ToString());
            CurrentPage = 1; 

            GalleryItems = FilteredAndSortedItems
        .Skip((CurrentPage - 1) * ProductsPerPage)
        .Take(ProductsPerPage)
        .ToList();

            TotalPages = Math.Max(1, (int)Math.Ceiling((double)FilteredAndSortedItems.Count / ProductsPerPage));
            StateHasChanged();
        }

        private void ApplyFiltersAndPagination()
        {
            FilteredAndSortedItems = AllGalleryItems
                .OrderBy(p => SortAscending ? p.Views : -p.Views) 
                .ToList();

            TotalPages = Math.Max(1, (int)Math.Ceiling((double)FilteredAndSortedItems.Count / ProductsPerPage));

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            LoadProducts(CurrentPage);
        }

        private void ChangePage(int newPage)
        {
            if (newPage > 0 && newPage <= TotalPages)
            {
                CurrentPage = newPage;
                LoadProducts(CurrentPage);
            }
        }

 
        private void LoadProducts(int page)
        {
            if (page < 1 || page > TotalPages)
            {
                return; 
            }

            var skip = (page - 1) * ProductsPerPage;
            GalleryItems = FilteredAndSortedItems
                .Skip(skip)
                .Take(ProductsPerPage)
                .ToList();

            StateHasChanged();
        }

        public async Task<string> FindThumbnail(int GalleryImageID)
        {
            List<GalleryImage> galleryImages = new List<GalleryImage>();

            var y = await AbstractTheatreService.GetGalleryImages();
            galleryImages = y.ToList();

            GalleryImage galleryImage = new GalleryImage();

            try
            {
                galleryImage = galleryImages.First(x => x.ID == GalleryImageID);
                AbstractImage x = await AbstractTheatreService.GetImageByImageId(galleryImage.ImageId);
                return BytetoImageBase64(x.Image1);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return("\\images\\Default_Product_Picture.png");
            }
        }

        public void NavigateToGalleryItem(int ID)
        {
            NavigationManager.NavigateTo($"/gallery-viewing/{ID}");
        }

        public void NavigateToGalleryVideo(int ID)
        {
            NavigationManager.NavigateTo($"/gallery-video-viewing/{ID}");
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

        private void OnSortByChange(ChangeEventArgs e)
        {
            if (e.Value.ToString() == "true")
            {
                SortAscending = true;
            }
            else
            {
                SortAscending = false;
            }
            ApplyFiltersAndPagination();
        }

    }

}