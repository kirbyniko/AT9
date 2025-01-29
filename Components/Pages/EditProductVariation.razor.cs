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
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Drawing;

namespace AT9.Components.Pages
{
    public partial class EditProductVariation
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

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public int ProductVariationId { get; set; }


        public List<AbstractImage> Images = new List<AbstractImage>();

        protected bool errorVisible;

        protected AT9.Models.AbstractTheatre.ProductVariation productVariation;
        public List<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
        public async Task OnDelete(int ID)
        {
            var querygalleryimages = await AbstractTheatreService.GetGalleryImages();
            var queryimages = await AbstractTheatreService.GetImages();

            List<GalleryImage> galleryimages = querygalleryimages.Where(x => x.ID == ID).ToList();
            foreach(var galleryimage in galleryimages)
            {
                await AbstractTheatreService.DeleteImage(galleryimage.ImageId);
                await AbstractTheatreService.DeleteGalleryImage(galleryimage.ID);
            }

            IQueryable<GalleryImage> galleryImages;
            var x = await AbstractTheatreService.GetGalleryImages();
            galleryImages = x.Where(x => x.ProductVariationId == ProductVariationId);
            GalleryImages = ToObservableCollection(galleryImages);
            SetupImages();

            StateHasChanged();

        }

        protected override async Task OnInitializedAsync()
        {
            productVariation = await AbstractTheatreService.GetProductVariationByProductVariationId(ProductVariationId);

            IQueryable<GalleryImage> galleryImages;
            var x = await AbstractTheatreService.GetGalleryImages();
            galleryImages = x.Where(x => x.ProductVariationId == ProductVariationId);
            GalleryImages = ToObservableCollection(galleryImages);
            SetupImages();

        }
        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }


        IList<GalleryImage> selectedGalleryImages;

        GalleryImage draggedItem;

        public async Task<string> IDtoImage(int id)
        {

            IQueryable<AbstractImage> images;
            var x = await AbstractTheatreService.GetImages();
            images = x.Where(x => x.ImageId == id);
            return BytetoImageBase64(images.FirstOrDefault().Image1);
        }
        void RowRender(RowRenderEventArgs<GalleryImage> args)
        {
            args.Attributes.Add("title", "Drag row to reorder");
            args.Attributes.Add("style", "cursor:grab");
            args.Attributes.Add("draggable", "true");
            args.Attributes.Add("ondragover", "event.preventDefault();event.target.closest('.rz-data-row').classList.add('my-class')");
            args.Attributes.Add("ondragleave", "event.target.closest('.rz-data-row').classList.remove('my-class')");
            args.Attributes.Add("ondragstart", EventCallback.Factory.Create<DragEventArgs>(this, () => draggedItem = args.Data));
            args.Attributes.Add("ondrop", EventCallback.Factory.Create<DragEventArgs>(this, () =>
            {
                var draggedIndex = GalleryImages.IndexOf(draggedItem);
                var droppedIndex = GalleryImages.IndexOf(args.Data);
                GalleryImages.Remove(draggedItem);
                GalleryImages.Insert(draggedIndex <= droppedIndex ? droppedIndex++ : droppedIndex, draggedItem);
                draggedItem.Position = (droppedIndex + 1);
                SetupImages();
                GalleryImages.OrderBy(x => x.Position);

                JSRuntime.InvokeVoidAsync("eval", $"document.querySelector('.my-class').classList.remove('my-class')");
                            StateHasChanged();

            }));
        }

        string ItemSelector(GalleryImage task) => task.Name;
        public void SetupImages()
        {
            var totalImages = GalleryImages.Count();
            var usedPositions = new HashSet<int>();
            foreach (var x in GalleryImages)
            {

                if (x.Position == 0 || x.Position < 1 || x.Position > totalImages)
                {
                    x.Position = GetAvailablePosition(usedPositions, totalImages);
                }
                else
                {
                    if (usedPositions.Contains(x.Position??0))
                    {
                        x.Position = GetAvailablePosition(usedPositions, totalImages);
                    }
                    else
                    {
                        usedPositions.Add(x.Position??0);
                    }
                }
            }
        }
        int GetAvailablePosition(HashSet<int> usedPositions, int totalImages)
        {
            for (int i = 1; i <= totalImages; i++)
            {
                if (!usedPositions.Contains(i))
                {
                    usedPositions.Add(i);
                    return i;
                }
            }
            throw new InvalidOperationException("No available positions found.");
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

            StateHasChanged();
        }

        protected async Task FormSubmit()
        {
            try
            {
                if ((UploadedFiles.Count() + GalleryImages.Count()) > 10)
                {
                    DialogService.Alert("A product can only have 10 images! Please respect the maximum!", "Too many images");
                    return;
                }
                if(productVariation.Price <= 0) 
                { 
                    DialogService.Alert("Price must be greater than 0"); 
                    return;
                }
                if (productVariation.Price > 1000)
                {
                    DialogService.Alert("Price must not be greater than 1,000");
                    return;
                }

                if (Math.Round(productVariation.Price, 2) != productVariation.Price)
                {
                    DialogService.Alert("Price must not contain more than two decimal places");
                    return;
                }
                if (productVariation.Stock >= 10000)
                {
                    DialogService.Alert("Stock must be less than 10,000");
                    return;
                }
                if (productVariation.Stock <= 0)
                {
                    DialogService.Alert("Stock must be greater than 0");
                    return;
                }

                foreach (UploadedFile file in UploadedFiles)
                {
                    AbstractImage image = new AbstractImage();
                    image.Image1 = file.StreamData;

                    await AbstractTheatreService.CreateImage(image);

                    int imageid = image.ImageId;
                    int productvariationid = productVariation.ProductVariationId;

                    GalleryImage galleryImage = new GalleryImage();
                    galleryImage.ImageId = imageid;
                    galleryImage.ProductVariationId = productvariationid;
                    galleryImage.Name = file.FileName;
                    galleryImage.Position = file.Position;


                    await AbstractTheatreService.CreateGalleryImage(galleryImage);


                }

                foreach (GalleryImage galleryImage in GalleryImages) 
                {
                    await AbstractTheatreService.UpdateGalleryImage(galleryImage.ID, galleryImage);
                }

                await AbstractTheatreService.UpdateProductVariation(ProductVariationId, productVariation);
                DialogService.Close(productVariation);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}