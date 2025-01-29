namespace AT9.Models.AbstractTheatre
{
    
    public class PageProduct() 
    {

        public int ProductVariationId { get; set; }
        public int ProductId { get; set; }
        public int GalleryImageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string Imgdata { get; set; }
        public int ProfileID { get; set; }

        public int Views { get; set; }

        public PageProduct(ProductVariation productVariation) : this()
        {
            ProductVariationId = productVariation.ProductVariationId;
            ProductId = productVariation.ProductId;
            Name = productVariation.Name;
            Description = productVariation.Description;
            Price = productVariation.Price;
            Stock = productVariation.Stock;
        
        }

        public PageProduct(GalleryImage galleryImage, int id) : this()
        {
            GalleryImageId = galleryImage.ID;    
            Name = galleryImage.Name;
            Description = galleryImage.Description;
            ProfileID = id;

        }

    }


}
