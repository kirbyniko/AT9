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
using AT9;

namespace AT9.Components.Pages
{
    public partial class AddCustomPackage
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
        public int ProductId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            productVariation = new AT9.Models.AbstractTheatre.ProductVariation();
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.ProductVariation productVariation;

        protected async Task FormSubmit()
        {
            try
            {
              if(UploadedFiles.Count() > 10)
                {
                    DialogService.Alert("A product can only have 10 images! Please respect the maximum!","Too many images");
                    return;
                }
                if (productVariation.Price <= 0)
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
             


                productVariation.ProfileID = await AbstractTheatreService.GetProfileID(Security.User.Id);
                await AbstractTheatreService.CreateProductVariation(productVariation);

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

              
                StateHasChanged();

                DialogService.Close(productVariation);


            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        private List<UploadedFile> UploadedFiles { get; set; } = new();

        [Inject]
        protected SecurityService Security { get; set; }

        private byte[] GetFileBytes(Stream fileStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
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
                            if (file.Size <= 1024 * 1024)
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
                            else
                            {
                                DialogService.Alert("File too big, we have a one MB maximum. File will not be uploaded.");

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



        public async void ValidatePosition(ChangeEventArgs args, UploadedFile file)
        {
            file.HasError = false;

            if (file.Position < 1 || file.Position > UploadedFiles.Count)
            {
                file.HasError = true;
                file.Position = 1;
                return;
            }


            var positionCount = UploadedFiles.Count(f => f.Position == file.Position);

            while (positionCount > 1)
            {
                if (positionCount > 1)
                {
                    file.HasError = true;
                    file.Position++;

                }
                positionCount = UploadedFiles.Count(f => f.Position == file.Position);
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
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}