using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using AT9.Data;

namespace AT9.Controllers
{
    public partial class ExportAbstractTheatreController : ExportController
    {
        private readonly AbstractTheatreContext context;
        private readonly AbstractTheatreService service;

        public ExportAbstractTheatreController(AbstractTheatreContext context, AbstractTheatreService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/AbstractTheatre/cartproductuserrelationships/csv")]
        [HttpGet("/export/AbstractTheatre/cartproductuserrelationships/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartProductUserRelationshipsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCartProductUserRelationships(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/cartproductuserrelationships/excel")]
        [HttpGet("/export/AbstractTheatre/cartproductuserrelationships/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCartProductUserRelationshipsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCartProductUserRelationships(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/deliveryaddresses/csv")]
        [HttpGet("/export/AbstractTheatre/deliveryaddresses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDeliveryAddressesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDeliveryAddresses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/deliveryaddresses/excel")]
        [HttpGet("/export/AbstractTheatre/deliveryaddresses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDeliveryAddressesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDeliveryAddresses(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/galleryimages/csv")]
        [HttpGet("/export/AbstractTheatre/galleryimages/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGalleryImagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGalleryImages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/galleryimages/excel")]
        [HttpGet("/export/AbstractTheatre/galleryimages/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGalleryImagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGalleryImages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/galleryvideos/csv")]
        [HttpGet("/export/AbstractTheatre/galleryvideos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGalleryVideosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGalleryVideos(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/galleryvideos/excel")]
        [HttpGet("/export/AbstractTheatre/galleryvideos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGalleryVideosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGalleryVideos(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/images/csv")]
        [HttpGet("/export/AbstractTheatre/images/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetImages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/images/excel")]
        [HttpGet("/export/AbstractTheatre/images/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetImages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/imageviews/csv")]
        [HttpGet("/export/AbstractTheatre/imageviews/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImageViewsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetImageViews(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/imageviews/excel")]
        [HttpGet("/export/AbstractTheatre/imageviews/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImageViewsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetImageViews(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/paymentmethods/csv")]
        [HttpGet("/export/AbstractTheatre/paymentmethods/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPaymentMethodsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPaymentMethods(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/paymentmethods/excel")]
        [HttpGet("/export/AbstractTheatre/paymentmethods/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPaymentMethodsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPaymentMethods(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/videoviews/csv")]
        [HttpGet("/export/AbstractTheatre/videoviews/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportVideoViewsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetVideoViews(), Request.Query, false), fileName);
        }

        [HttpGet("/export/AbstractTheatre/videoviews/excel")]
        [HttpGet("/export/AbstractTheatre/videoviews/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportVideoViewsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetVideoViews(), Request.Query, false), fileName);
        }
    }
}
