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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Vml;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections.ObjectModel;


namespace AT9.Components.Pages
{
    public partial class ShoppingPage
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
        protected AbstractTheatreService AbstractTheatreService { get; set; }

        public List<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();

        public IQueryable<Product> Products { get; set; }

        public string Search { get; set; }    

        IQueryable<ProductVariation> productVariations;


        public List<PageProduct> PageProducts { get; set; } = new List<PageProduct>();

        public List<PageProduct> AllPageProducts { get; set; } = new List<PageProduct>();


        [Inject]
        protected SecurityService Security { get; set; }


        public int SelectedMinPrice { get; set; } = 0;
        public int SelectedMaxPrice { get; set; } = 1000; 
        private int MinPrice { get; set; } = 0;
        private int MaxPrice { get; set; } = 1000;

        public List<PageProduct> FilteredAndSortedProducts { get; set; } = new List<PageProduct>();

        public int CurrentPage { get; set; } = 1;

        public int _productsPerPage = 5;

   

        public int ProductsPerPage
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


        public void SearchFunction()
        {
            ApplyFiltersAndPagination();
            LoadProducts(CurrentPage);
        }
 
    
        private void OnProductsPerPageChange(ChangeEventArgs e)
        {
            ProductsPerPage = int.Parse(e.Value.ToString());
            CurrentPage = 1; 

            PageProducts = FilteredAndSortedProducts
        .Skip((CurrentPage - 1) * ProductsPerPage)
        .Take(ProductsPerPage)
        .ToList();

            TotalPages = Math.Max(1, (int)Math.Ceiling((double)FilteredAndSortedProducts.Count / ProductsPerPage));
            StateHasChanged();
        }

        public void ApplyFiltersAndPagination()
        {

            if(Search == null || Search == "")
            {
  FilteredAndSortedProducts = AllPageProducts
                .Where(p => p.Price >= SelectedMinPrice && p.Price <= SelectedMaxPrice)
                .OrderBy(p => SortAscending ? p.Price : -p.Price) 
                .ToList();

            }
            else
            {
  FilteredAndSortedProducts = AllPageProducts
                .Where(p => p.Price >= SelectedMinPrice && p.Price <= SelectedMaxPrice)
                .Where(p => p.Name.Contains(Search) || p.Description.Contains(Search)) 
                .OrderBy(p => SortAscending ? p.Price : -p.Price) 
                .ToList();

            }
          
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)FilteredAndSortedProducts.Count / ProductsPerPage));

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

        private void OnMinPriceChange(ChangeEventArgs e)
        {
            SelectedMinPrice = Int32.Parse(e.Value.ToString());
            ApplyFiltersAndPagination();
        }

        private void OnMaxPriceChange(ChangeEventArgs e)
        {
            SelectedMaxPrice = Int32.Parse(e.Value.ToString());
            ApplyFiltersAndPagination();
        }
        public void LoadProducts(int page)
        {
            if (page < 1 || page > TotalPages)
            {
                return; 
            }

            var skip = (page - 1) * ProductsPerPage;
            PageProducts = FilteredAndSortedProducts
                .Skip(skip)
                .Take(ProductsPerPage)
                .ToList();

            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {

            await InitFunction();
        }

        public async Task InitFunction()
        {
            Products = await AbstractTheatreService.GetProducts();
            var productvariations = await AbstractTheatreService.GetProductVariations();

            foreach (var product in Products)
            {
                productVariations = productvariations.Where(x => x.ProductId == product.ProductId);
                List<ProductVariation> productVariations1 = productVariations.ToList();

                try
                {
                    ProductVariations.Add(productVariations1.First());
                }
                catch (Exception ex)
                {

                }
            }

            foreach (var productVariation in ProductVariations)
            {
                AllPageProducts.Add(new PageProduct(productVariation));
            }

            foreach (PageProduct pageproduct in AllPageProducts)
            {
                pageproduct.Imgdata = await FindThumbnail(pageproduct.ProductVariationId);
            }
            ApplyFiltersAndPagination();
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)FilteredAndSortedProducts.Count / ProductsPerPage));

            LoadProducts(CurrentPage);
        }

        public async Task<string> FindThumbnail(int ProductVariationID)
        {
            List<GalleryImage> galleryImages = new List<GalleryImage>();

            try
            {
                var y = await AbstractTheatreService.GetGalleryImages();
                galleryImages = y.OrderBy(x => x.Position).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            GalleryImage galleryImage = new GalleryImage();

            try
            {
                galleryImage = galleryImages.FirstOrDefault(x => x.ProductVariationId == ProductVariationID);
                if (galleryImage == null)
                {
                    return ("\\images\\Default_Product_Picture.png");
                }

                AbstractImage x = await AbstractTheatreService.GetImageByImageId(galleryImage.ImageId);
                return BytetoImageBase64(x.Image1);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ("\\images\\Default_Product_Picture.png");
            }
        }

        public void NavigateToProduct(int ID)
        {
            NavigationManager.NavigateTo($"/product-viewing/{ID}");
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
             if(e.Value.ToString() == "true") 
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