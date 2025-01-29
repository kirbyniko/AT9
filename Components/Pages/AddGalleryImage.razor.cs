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
using AT9;
using AT9.Models.AbstractTheatre;

namespace AT9.Components.Pages
{
    public partial class AddGalleryImage
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
        public AT9.AbstractTheatreService AbstractTheatreService { get; set; }

        public List<UploadedFile> UploadedFiles = new List<UploadedFile>();
        protected async override Task OnInitializedAsync()
        {
            galleryImage = new AT9.Models.AbstractTheatre.GalleryImage();
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.GalleryImage galleryImage;

        [Inject]
        protected SecurityService Security { get; set; }

        public string Validate() 
        { 
        
           if(galleryImage.Name == null || galleryImage.Name == "")
            {
                return "Name is required";
            }
            if (galleryImage.Description == null || galleryImage.Description == "")
            {
                return "Description is required";
            }
            return "";
        }

        protected async Task FormSubmit()
        {
            if (Validate() != "")
            {
                DialogService.Alert(Validate(), "Error");
                return;
            }
            else
            {
                try
                {
                    foreach (UploadedFile file in UploadedFiles)
                    {
                        AbstractImage image = new AbstractImage();
                        image.Image1 = file.StreamData;

                        await AbstractTheatreService.CreateImage(image);

                        int imageid = image.ImageId;
                        int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);

                        galleryImage.ImageId = imageid;
                        galleryImage.ProfileID = profileid;


                    }
                    if (UploadedFiles.Count < 1)
                    {
                        DialogService.Alert("You must upload a file. This is the gallery section where you show off your artwork!", "Missing file");
                        return;
                    }

                    await AbstractTheatreService.CreateGalleryImage(galleryImage);
                    DialogService.Close(galleryImage);
                }
                catch (Exception ex)
                {
                    errorVisible = true;
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
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}