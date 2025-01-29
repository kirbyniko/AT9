using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using AT9.Data;
using AT9.Models.AbstractTheatre;

namespace AT9
{
    public partial class AbstractTheatreService
    {
        AbstractTheatreContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly AbstractTheatreContext context;
        private readonly NavigationManager navigationManager;

        public AbstractTheatreService(AbstractTheatreContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportCartProductUserRelationshipsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/cartproductuserrelationships/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/cartproductuserrelationships/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCartProductUserRelationshipsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/cartproductuserrelationships/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/cartproductuserrelationships/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCartProductUserRelationshipsRead(ref IQueryable<AT9.Models.AbstractTheatre.CartProductUserRelationship> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.CartProductUserRelationship>> GetCartProductUserRelationships(Query query = null)
        {
            var items = Context.CartProductUserRelationships.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCartProductUserRelationshipsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCartProductUserRelationshipGet(AT9.Models.AbstractTheatre.CartProductUserRelationship item);
        partial void OnGetCartProductUserRelationshipByCartUserRelationshipId(ref IQueryable<AT9.Models.AbstractTheatre.CartProductUserRelationship> items);


        public async Task<AT9.Models.AbstractTheatre.CartProductUserRelationship> GetCartProductUserRelationshipByCartUserRelationshipId(int cartuserrelationshipid)
        {
            var items = Context.CartProductUserRelationships
                              .AsNoTracking()
                              .Where(i => i.CartUserRelationshipId == cartuserrelationshipid);

 
            OnGetCartProductUserRelationshipByCartUserRelationshipId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCartProductUserRelationshipGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCartProductUserRelationshipCreated(AT9.Models.AbstractTheatre.CartProductUserRelationship item);
        partial void OnAfterCartProductUserRelationshipCreated(AT9.Models.AbstractTheatre.CartProductUserRelationship item);

        public async Task<AT9.Models.AbstractTheatre.CartProductUserRelationship> CreateCartProductUserRelationship(AT9.Models.AbstractTheatre.CartProductUserRelationship cartproductuserrelationship)
        {
            OnCartProductUserRelationshipCreated(cartproductuserrelationship);

            var existingItem = Context.CartProductUserRelationships
                              .Where(i => i.CartUserRelationshipId == cartproductuserrelationship.CartUserRelationshipId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.CartProductUserRelationships.Add(cartproductuserrelationship);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(cartproductuserrelationship).State = EntityState.Detached;
                throw;
            }

            OnAfterCartProductUserRelationshipCreated(cartproductuserrelationship);

            return cartproductuserrelationship;
        }

        public async Task<AT9.Models.AbstractTheatre.CartProductUserRelationship> CancelCartProductUserRelationshipChanges(AT9.Models.AbstractTheatre.CartProductUserRelationship item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCartProductUserRelationshipUpdated(AT9.Models.AbstractTheatre.CartProductUserRelationship item);
        partial void OnAfterCartProductUserRelationshipUpdated(AT9.Models.AbstractTheatre.CartProductUserRelationship item);

        public async Task<AT9.Models.AbstractTheatre.CartProductUserRelationship> UpdateCartProductUserRelationship(int cartuserrelationshipid, AT9.Models.AbstractTheatre.CartProductUserRelationship cartproductuserrelationship)
        {
            OnCartProductUserRelationshipUpdated(cartproductuserrelationship);

            var itemToUpdate = Context.CartProductUserRelationships
                              .Where(i => i.CartUserRelationshipId == cartproductuserrelationship.CartUserRelationshipId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(cartproductuserrelationship);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCartProductUserRelationshipUpdated(cartproductuserrelationship);

            return cartproductuserrelationship;
        }

        partial void OnCartProductUserRelationshipDeleted(AT9.Models.AbstractTheatre.CartProductUserRelationship item);
        partial void OnAfterCartProductUserRelationshipDeleted(AT9.Models.AbstractTheatre.CartProductUserRelationship item);

        public async Task<AT9.Models.AbstractTheatre.CartProductUserRelationship> DeleteCartProductUserRelationship(int cartuserrelationshipid)
        {
            var itemToDelete = Context.CartProductUserRelationships
                              .Where(i => i.CartUserRelationshipId == cartuserrelationshipid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCartProductUserRelationshipDeleted(itemToDelete);


            Context.CartProductUserRelationships.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCartProductUserRelationshipDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/categories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/categories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/categories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/categories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCategoriesRead(ref IQueryable<AT9.Models.AbstractTheatre.Category> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.Category>> GetCategories(Query query = null)
        {
            var items = Context.Categories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCategoryGet(AT9.Models.AbstractTheatre.Category item);
        partial void OnGetCategoryByCategoryId(ref IQueryable<AT9.Models.AbstractTheatre.Category> items);


        public async Task<AT9.Models.AbstractTheatre.Category> GetCategoryByCategoryId(int categoryid)
        {
            var items = Context.Categories
                              .AsNoTracking()
                              .Where(i => i.CategoryId == categoryid);

 
            OnGetCategoryByCategoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCategoryCreated(AT9.Models.AbstractTheatre.Category item);
        partial void OnAfterCategoryCreated(AT9.Models.AbstractTheatre.Category item);

        public async Task<AT9.Models.AbstractTheatre.Category> CreateCategory(AT9.Models.AbstractTheatre.Category category)
        {
            OnCategoryCreated(category);

            var existingItem = Context.Categories
                              .Where(i => i.CategoryId == category.CategoryId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Categories.Add(category);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(category).State = EntityState.Detached;
                throw;
            }

            OnAfterCategoryCreated(category);

            return category;
        }

        public async Task<AT9.Models.AbstractTheatre.Category> CancelCategoryChanges(AT9.Models.AbstractTheatre.Category item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCategoryUpdated(AT9.Models.AbstractTheatre.Category item);
        partial void OnAfterCategoryUpdated(AT9.Models.AbstractTheatre.Category item);

        public async Task<AT9.Models.AbstractTheatre.Category> UpdateCategory(int categoryid, AT9.Models.AbstractTheatre.Category category)
        {
            OnCategoryUpdated(category);

            var itemToUpdate = Context.Categories
                              .Where(i => i.CategoryId == category.CategoryId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(category);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCategoryUpdated(category);

            return category;
        }

        partial void OnCategoryDeleted(AT9.Models.AbstractTheatre.Category item);
        partial void OnAfterCategoryDeleted(AT9.Models.AbstractTheatre.Category item);

        public async Task<AT9.Models.AbstractTheatre.Category> DeleteCategory(int categoryid)
        {
            var itemToDelete = Context.Categories
                              .Where(i => i.CategoryId == categoryid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCategoryDeleted(itemToDelete);


            Context.Categories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCategoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportGalleryImagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/galleryimages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/galleryimages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGalleryImagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/galleryimages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/galleryimages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGalleryImagesRead(ref IQueryable<AT9.Models.AbstractTheatre.GalleryImage> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.GalleryImage>> GetGalleryImages(Query query = null)
        {
            var items = Context.GalleryImages.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnGalleryImagesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGalleryImageGet(AT9.Models.AbstractTheatre.GalleryImage item);
        partial void OnGetGalleryImageByGalleryImageId(ref IQueryable<AT9.Models.AbstractTheatre.GalleryImage> items);


        public async Task<AT9.Models.AbstractTheatre.GalleryImage> GetGalleryImageByGalleryImageId(int galleryimageid)
        {
            var items = Context.GalleryImages
                              .AsNoTracking()
                              .Where(i => i.ID == galleryimageid);

 
            OnGetGalleryImageByGalleryImageId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGalleryImageGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGalleryImageCreated(AT9.Models.AbstractTheatre.GalleryImage item);
        partial void OnAfterGalleryImageCreated(AT9.Models.AbstractTheatre.GalleryImage item);

        public async Task<AT9.Models.AbstractTheatre.GalleryImage> CreateGalleryImage(AT9.Models.AbstractTheatre.GalleryImage galleryimage)
        {
            OnGalleryImageCreated(galleryimage);

            var existingItem = Context.GalleryImages
                              .Where(i => i.ID == galleryimage.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.GalleryImages.Add(galleryimage);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(galleryimage).State = EntityState.Detached;
                throw;
            }

            OnAfterGalleryImageCreated(galleryimage);

            return galleryimage;
        }

        public async Task<AT9.Models.AbstractTheatre.GalleryImage> CancelGalleryImageChanges(AT9.Models.AbstractTheatre.GalleryImage item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGalleryImageUpdated(AT9.Models.AbstractTheatre.GalleryImage item);
        partial void OnAfterGalleryImageUpdated(AT9.Models.AbstractTheatre.GalleryImage item);

        public async Task<AT9.Models.AbstractTheatre.GalleryImage> UpdateGalleryImage(int galleryimageid, AT9.Models.AbstractTheatre.GalleryImage galleryimage)
        {
            OnGalleryImageUpdated(galleryimage);

            var itemToUpdate = Context.GalleryImages
                              .Where(i => i.ID == galleryimage.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(galleryimage);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGalleryImageUpdated(galleryimage);

            return galleryimage;
        }

        partial void OnGalleryImageDeleted(AT9.Models.AbstractTheatre.GalleryImage item);
        partial void OnAfterGalleryImageDeleted(AT9.Models.AbstractTheatre.GalleryImage item);

        public async Task<AT9.Models.AbstractTheatre.GalleryImage> DeleteGalleryImage(int galleryimageid)
        {
            var itemToDelete = Context.GalleryImages
                              .Where(i => i.ID == galleryimageid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnGalleryImageDeleted(itemToDelete);


            Context.GalleryImages.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGalleryImageDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportGalleryVideosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/galleryvideos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/galleryvideos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGalleryVideosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/galleryvideos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/galleryvideos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGalleryVideosRead(ref IQueryable<AT9.Models.AbstractTheatre.GalleryVideo> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.GalleryVideo>> GetGalleryVideos(Query query = null)
        {
            var items = Context.GalleryVideos.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnGalleryVideosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGalleryVideoGet(AT9.Models.AbstractTheatre.GalleryVideo item);
        partial void OnGetGalleryVideoByGalleryVideoId(ref IQueryable<AT9.Models.AbstractTheatre.GalleryVideo> items);


        public async Task<AT9.Models.AbstractTheatre.GalleryVideo> GetGalleryVideoByGalleryVideoId(int galleryvideoid)
        {
            var items = Context.GalleryVideos
                              .AsNoTracking()
                              .Where(i => i.ID == galleryvideoid);

 
            OnGetGalleryVideoByGalleryVideoId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGalleryVideoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGalleryVideoCreated(AT9.Models.AbstractTheatre.GalleryVideo item);
        partial void OnAfterGalleryVideoCreated(AT9.Models.AbstractTheatre.GalleryVideo item);

        public async Task<AT9.Models.AbstractTheatre.GalleryVideo> CreateGalleryVideo(AT9.Models.AbstractTheatre.GalleryVideo galleryvideo)
        {
            OnGalleryVideoCreated(galleryvideo);

            var existingItem = Context.GalleryVideos
                              .Where(i => i.ID == galleryvideo.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.GalleryVideos.Add(galleryvideo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(galleryvideo).State = EntityState.Detached;
                throw;
            }

            OnAfterGalleryVideoCreated(galleryvideo);

            return galleryvideo;
        }

        public async Task<AT9.Models.AbstractTheatre.GalleryVideo> CancelGalleryVideoChanges(AT9.Models.AbstractTheatre.GalleryVideo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGalleryVideoUpdated(AT9.Models.AbstractTheatre.GalleryVideo item);
        partial void OnAfterGalleryVideoUpdated(AT9.Models.AbstractTheatre.GalleryVideo item);

        public async Task<AT9.Models.AbstractTheatre.GalleryVideo> UpdateGalleryVideo(int galleryvideoid, AT9.Models.AbstractTheatre.GalleryVideo galleryvideo)
        {
            OnGalleryVideoUpdated(galleryvideo);

            var itemToUpdate = Context.GalleryVideos
                              .Where(i => i.ID == galleryvideo.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(galleryvideo);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGalleryVideoUpdated(galleryvideo);

            return galleryvideo;
        }

        partial void OnGalleryVideoDeleted(AT9.Models.AbstractTheatre.GalleryVideo item);
        partial void OnAfterGalleryVideoDeleted(AT9.Models.AbstractTheatre.GalleryVideo item);

        public async Task<AT9.Models.AbstractTheatre.GalleryVideo> DeleteGalleryVideo(int galleryvideoid)
        {
            var itemToDelete = Context.GalleryVideos
                              .Where(i => i.ID == galleryvideoid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnGalleryVideoDeleted(itemToDelete);


            Context.GalleryVideos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGalleryVideoDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportImagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportImagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnImagesRead(ref IQueryable<AT9.Models.AbstractTheatre.AbstractImage> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.AbstractImage>> GetImages(Query query = null)
        {
            var items = Context.Images.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnImagesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnImageGet(AT9.Models.AbstractTheatre.AbstractImage item);
        partial void OnGetImageByImageId(ref IQueryable<AT9.Models.AbstractTheatre.AbstractImage> items);


        public async Task<AT9.Models.AbstractTheatre.AbstractImage> GetImageByImageId(int imageid)
        {
            var items = Context.Images
                              .AsNoTracking()
                              .Where(i => i.ImageId == imageid);

 
            OnGetImageByImageId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnImageGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnImageCreated(AT9.Models.AbstractTheatre.AbstractImage item);
        partial void OnAfterImageCreated(AT9.Models.AbstractTheatre.AbstractImage item);

        public async Task<AT9.Models.AbstractTheatre.AbstractImage> CreateImage(AT9.Models.AbstractTheatre.AbstractImage image)
        {
            OnImageCreated(image);

            var existingItem = Context.Images
                              .Where(i => i.ImageId == image.ImageId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Images.Add(image);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(image).State = EntityState.Detached;
                throw;
            }

            OnAfterImageCreated(image);

            return image;
        }

        public async Task<AT9.Models.AbstractTheatre.AbstractImage> CancelImageChanges(AT9.Models.AbstractTheatre.AbstractImage item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnImageUpdated(AT9.Models.AbstractTheatre.AbstractImage item);
        partial void OnAfterImageUpdated(AT9.Models.AbstractTheatre.AbstractImage item);

        public async Task<AT9.Models.AbstractTheatre.AbstractImage> UpdateImage(int imageid, AT9.Models.AbstractTheatre.AbstractImage image)
        {
            OnImageUpdated(image);

            var itemToUpdate = Context.Images
                              .Where(i => i.ImageId == image.ImageId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(image);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterImageUpdated(image);

            return image;
        }

        partial void OnImageDeleted(AT9.Models.AbstractTheatre.AbstractImage item);
        partial void OnAfterImageDeleted(AT9.Models.AbstractTheatre.AbstractImage item);

        public async Task<AT9.Models.AbstractTheatre.AbstractImage> DeleteImage(int imageid)
        {
            var itemToDelete = Context.Images
                              .Where(i => i.ImageId == imageid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnImageDeleted(itemToDelete);


            Context.Images.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterImageDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportImageViewsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/imageviews/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/imageviews/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportImageViewsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/imageviews/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/imageviews/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnImageViewsRead(ref IQueryable<AT9.Models.AbstractTheatre.ImageView> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.ImageView>> GetImageViews(Query query = null)
        {
            var items = Context.ImageViews.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnImageViewsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnImageViewGet(AT9.Models.AbstractTheatre.ImageView item);
        partial void OnGetImageViewByImageViewId(ref IQueryable<AT9.Models.AbstractTheatre.ImageView> items);


        public async Task<AT9.Models.AbstractTheatre.ImageView> GetImageViewByImageViewId(int imageviewid)
        {
            var items = Context.ImageViews
                              .AsNoTracking()
                              .Where(i => i.ImageViewId == imageviewid);

 
            OnGetImageViewByImageViewId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnImageViewGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnImageViewCreated(AT9.Models.AbstractTheatre.ImageView item);
        partial void OnAfterImageViewCreated(AT9.Models.AbstractTheatre.ImageView item);

        public async Task<AT9.Models.AbstractTheatre.ImageView> CreateImageView(AT9.Models.AbstractTheatre.ImageView imageview)
        {
            OnImageViewCreated(imageview);

            var existingItem = Context.ImageViews
                              .Where(i => i.ImageViewId == imageview.ImageViewId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ImageViews.Add(imageview);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(imageview).State = EntityState.Detached;
                throw;
            }

            OnAfterImageViewCreated(imageview);

            return imageview;
        }

        public async Task<AT9.Models.AbstractTheatre.ImageView> CancelImageViewChanges(AT9.Models.AbstractTheatre.ImageView item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnImageViewUpdated(AT9.Models.AbstractTheatre.ImageView item);
        partial void OnAfterImageViewUpdated(AT9.Models.AbstractTheatre.ImageView item);

        public async Task<AT9.Models.AbstractTheatre.ImageView> UpdateImageView(int imageviewid, AT9.Models.AbstractTheatre.ImageView imageview)
        {
            OnImageViewUpdated(imageview);

            var itemToUpdate = Context.ImageViews
                              .Where(i => i.ImageViewId == imageview.ImageViewId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(imageview);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterImageViewUpdated(imageview);

            return imageview;
        }

        partial void OnImageViewDeleted(AT9.Models.AbstractTheatre.ImageView item);
        partial void OnAfterImageViewDeleted(AT9.Models.AbstractTheatre.ImageView item);

        public async Task<AT9.Models.AbstractTheatre.ImageView> DeleteImageView(int imageviewid)
        {
            var itemToDelete = Context.ImageViews
                              .Where(i => i.ImageViewId == imageviewid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnImageViewDeleted(itemToDelete);


            Context.ImageViews.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterImageViewDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrdersRead(ref IQueryable<AT9.Models.AbstractTheatre.Order> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.Order>> GetOrders(Query query = null)
        {
            var items = Context.Orders.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderGet(AT9.Models.AbstractTheatre.Order item);
        partial void OnGetOrderByOrderId(ref IQueryable<AT9.Models.AbstractTheatre.Order> items);


        public async Task<AT9.Models.AbstractTheatre.Order> GetOrderByOrderId(int orderid)
        {
            var items = Context.Orders
                              .AsNoTracking()
                              .Where(i => i.OrderId == orderid);

 
            OnGetOrderByOrderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderCreated(AT9.Models.AbstractTheatre.Order item);
        partial void OnAfterOrderCreated(AT9.Models.AbstractTheatre.Order item);

        public async Task<AT9.Models.AbstractTheatre.Order> CreateOrder(AT9.Models.AbstractTheatre.Order order)
        {
            OnOrderCreated(order);

            var existingItem = Context.Orders
                              .Where(i => i.OrderId == order.OrderId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Orders.Add(order);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(order).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderCreated(order);

            return order;
        }

        public async Task<AT9.Models.AbstractTheatre.Order> CancelOrderChanges(AT9.Models.AbstractTheatre.Order item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderUpdated(AT9.Models.AbstractTheatre.Order item);
        partial void OnAfterOrderUpdated(AT9.Models.AbstractTheatre.Order item);

        public async Task<AT9.Models.AbstractTheatre.Order> UpdateOrder(int orderid, AT9.Models.AbstractTheatre.Order order)
        {
            OnOrderUpdated(order);

            var itemToUpdate = Context.Orders
                              .Where(i => i.OrderId == order.OrderId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(order);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderUpdated(order);

            return order;
        }

        partial void OnOrderDeleted(AT9.Models.AbstractTheatre.Order item);
        partial void OnAfterOrderDeleted(AT9.Models.AbstractTheatre.Order item);

        public async Task<AT9.Models.AbstractTheatre.Order> DeleteOrder(int orderid)
        {
            var itemToDelete = Context.Orders
                              .Where(i => i.OrderId == orderid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderDeleted(itemToDelete);


            Context.Orders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductCategoryInstancesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/productcategoryinstances/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/productcategoryinstances/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductCategoryInstancesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/productcategoryinstances/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/productcategoryinstances/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductCategoryInstancesRead(ref IQueryable<AT9.Models.AbstractTheatre.ProductCategoryInstance> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.ProductCategoryInstance>> GetProductCategoryInstances(Query query = null)
        {
            var items = Context.ProductCategoryInstances.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductCategoryInstancesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductCategoryInstanceGet(AT9.Models.AbstractTheatre.ProductCategoryInstance item);
        partial void OnGetProductCategoryInstanceByProductCategoryInstanceId(ref IQueryable<AT9.Models.AbstractTheatre.ProductCategoryInstance> items);


        public async Task<AT9.Models.AbstractTheatre.ProductCategoryInstance> GetProductCategoryInstanceByProductCategoryInstanceId(string productcategoryinstanceid)
        {
            var items = Context.ProductCategoryInstances
                              .AsNoTracking()
                              .Where(i => i.ProductCategoryInstanceId == productcategoryinstanceid);

 
            OnGetProductCategoryInstanceByProductCategoryInstanceId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductCategoryInstanceGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCategoryInstanceCreated(AT9.Models.AbstractTheatre.ProductCategoryInstance item);
        partial void OnAfterProductCategoryInstanceCreated(AT9.Models.AbstractTheatre.ProductCategoryInstance item);

        public async Task<AT9.Models.AbstractTheatre.ProductCategoryInstance> CreateProductCategoryInstance(AT9.Models.AbstractTheatre.ProductCategoryInstance productcategoryinstance)
        {
            OnProductCategoryInstanceCreated(productcategoryinstance);

            var existingItem = Context.ProductCategoryInstances
                              .Where(i => i.ProductCategoryInstanceId == productcategoryinstance.ProductCategoryInstanceId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ProductCategoryInstances.Add(productcategoryinstance);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(productcategoryinstance).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCategoryInstanceCreated(productcategoryinstance);

            return productcategoryinstance;
        }

        public async Task<AT9.Models.AbstractTheatre.ProductCategoryInstance> CancelProductCategoryInstanceChanges(AT9.Models.AbstractTheatre.ProductCategoryInstance item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductCategoryInstanceUpdated(AT9.Models.AbstractTheatre.ProductCategoryInstance item);
        partial void OnAfterProductCategoryInstanceUpdated(AT9.Models.AbstractTheatre.ProductCategoryInstance item);

        public async Task<AT9.Models.AbstractTheatre.ProductCategoryInstance> UpdateProductCategoryInstance(string productcategoryinstanceid, AT9.Models.AbstractTheatre.ProductCategoryInstance productcategoryinstance)
        {
            OnProductCategoryInstanceUpdated(productcategoryinstance);

            var itemToUpdate = Context.ProductCategoryInstances
                              .Where(i => i.ProductCategoryInstanceId == productcategoryinstance.ProductCategoryInstanceId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(productcategoryinstance);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductCategoryInstanceUpdated(productcategoryinstance);

            return productcategoryinstance;
        }

        partial void OnProductCategoryInstanceDeleted(AT9.Models.AbstractTheatre.ProductCategoryInstance item);
        partial void OnAfterProductCategoryInstanceDeleted(AT9.Models.AbstractTheatre.ProductCategoryInstance item);

        public async Task<AT9.Models.AbstractTheatre.ProductCategoryInstance> DeleteProductCategoryInstance(string productcategoryinstanceid)
        {
            var itemToDelete = Context.ProductCategoryInstances
                              .Where(i => i.ProductCategoryInstanceId == productcategoryinstanceid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductCategoryInstanceDeleted(itemToDelete);


            Context.ProductCategoryInstances.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductCategoryInstanceDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductVariationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/productvariations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/productvariations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductVariationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/productvariations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/productvariations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductVariationsRead(ref IQueryable<AT9.Models.AbstractTheatre.ProductVariation> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.ProductVariation>> GetProductVariations(Query query = null)
        {
            var items = Context.ProductVariations.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductVariationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductVariationGet(AT9.Models.AbstractTheatre.ProductVariation item);
        partial void OnGetProductVariationByProductVariationId(ref IQueryable<AT9.Models.AbstractTheatre.ProductVariation> items);


        public async Task<AT9.Models.AbstractTheatre.ProductVariation> GetProductVariationByProductVariationId(int productvariationid)
        {
            var items = Context.ProductVariations
                              .AsNoTracking()
                              .Where(i => i.ProductVariationId == productvariationid);

 
            OnGetProductVariationByProductVariationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductVariationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductVariationCreated(AT9.Models.AbstractTheatre.ProductVariation item);
        partial void OnAfterProductVariationCreated(AT9.Models.AbstractTheatre.ProductVariation item);

        public async Task<AT9.Models.AbstractTheatre.ProductVariation> CreateProductVariation(AT9.Models.AbstractTheatre.ProductVariation productvariation)
        {
            OnProductVariationCreated(productvariation);

            var existingItem = Context.ProductVariations
                              .Where(i => i.ProductVariationId == productvariation.ProductVariationId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.ProductVariations.Add(productvariation);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(productvariation).State = EntityState.Detached;
                throw;
            }

            OnAfterProductVariationCreated(productvariation);

            return productvariation;
        }

        public async Task<AT9.Models.AbstractTheatre.ProductVariation> CancelProductVariationChanges(AT9.Models.AbstractTheatre.ProductVariation item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductVariationUpdated(AT9.Models.AbstractTheatre.ProductVariation item);
        partial void OnAfterProductVariationUpdated(AT9.Models.AbstractTheatre.ProductVariation item);

        public async Task<AT9.Models.AbstractTheatre.ProductVariation> UpdateProductVariation(int productvariationid, AT9.Models.AbstractTheatre.ProductVariation productvariation)
        {
            OnProductVariationUpdated(productvariation);

            var itemToUpdate = Context.ProductVariations
                              .Where(i => i.ProductVariationId == productvariation.ProductVariationId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(productvariation);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductVariationUpdated(productvariation);

            return productvariation;
        }

        partial void OnProductVariationDeleted(AT9.Models.AbstractTheatre.ProductVariation item);
        partial void OnAfterProductVariationDeleted(AT9.Models.AbstractTheatre.ProductVariation item);

        public async Task<AT9.Models.AbstractTheatre.ProductVariation> DeleteProductVariation(int productvariationid)
        {
            var itemToDelete = Context.ProductVariations
                              .Where(i => i.ProductVariationId == productvariationid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductVariationDeleted(itemToDelete);


            Context.ProductVariations.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductVariationDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductsRead(ref IQueryable<AT9.Models.AbstractTheatre.Product> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.Product>> GetProducts(Query query = null)
        {
            var items = Context.Products.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProductsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductGet(AT9.Models.AbstractTheatre.Product item);
        partial void OnGetProductByProductId(ref IQueryable<AT9.Models.AbstractTheatre.Product> items);


        public async Task<AT9.Models.AbstractTheatre.Product> GetProductByProductId(int productid)
        {
            var items = Context.Products
                              .AsNoTracking()
                              .Where(i => i.ProductId == productid);

 
            OnGetProductByProductId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProductGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCreated(AT9.Models.AbstractTheatre.Product item);
        partial void OnAfterProductCreated(AT9.Models.AbstractTheatre.Product item);

        public async Task<AT9.Models.AbstractTheatre.Product> CreateProduct(AT9.Models.AbstractTheatre.Product product)
        {
            OnProductCreated(product);

            var existingItem = Context.Products
                              .Where(i => i.ProductId == product.ProductId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(product).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCreated(product);

            return product;
        }

        public async Task<AT9.Models.AbstractTheatre.Product> CancelProductChanges(AT9.Models.AbstractTheatre.Product item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductUpdated(AT9.Models.AbstractTheatre.Product item);
        partial void OnAfterProductUpdated(AT9.Models.AbstractTheatre.Product item);

        public async Task<AT9.Models.AbstractTheatre.Product> UpdateProduct(int productid, AT9.Models.AbstractTheatre.Product product)
        {
            OnProductUpdated(product);

            var itemToUpdate = Context.Products
                              .Where(i => i.ProductId == product.ProductId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(product);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductUpdated(product);

            return product;
        }

        partial void OnProductDeleted(AT9.Models.AbstractTheatre.Product item);
        partial void OnAfterProductDeleted(AT9.Models.AbstractTheatre.Product item);

        public async Task<AT9.Models.AbstractTheatre.Product> DeleteProduct(int productid)
        {
            var itemToDelete = Context.Products
                              .Where(i => i.ProductId == productid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductDeleted(itemToDelete);


            Context.Products.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProfilesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/profiles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/profiles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProfilesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/profiles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/profiles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProfilesRead(ref IQueryable<AT9.Models.AbstractTheatre.AbstractProfile> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.AbstractProfile>> GetProfiles(Query query = null)
        {
            var items = Context.Profiles.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnProfilesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProfileGet(AT9.Models.AbstractTheatre.AbstractProfile item);
        partial void OnGetProfileByProfileId(ref IQueryable<AT9.Models.AbstractTheatre.AbstractProfile> items);


        public async Task<AT9.Models.AbstractTheatre.AbstractProfile> GetProfileByProfileId(int profileid)
        {
            var items = Context.Profiles
                              .AsNoTracking()
                              .Where(i => i.ProfileId == profileid);

 
            OnGetProfileByProfileId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnProfileGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProfileCreated(AT9.Models.AbstractTheatre.AbstractProfile item);
        partial void OnAfterProfileCreated(AT9.Models.AbstractTheatre.AbstractProfile item);

        public async Task<AT9.Models.AbstractTheatre.AbstractProfile> CreateProfile(AT9.Models.AbstractTheatre.AbstractProfile profile)
        {
            OnProfileCreated(profile);

            var existingItem = Context.Profiles
                              .Where(i => i.ProfileId == profile.ProfileId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Profiles.Add(profile);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(profile).State = EntityState.Detached;
                throw;
            }

            OnAfterProfileCreated(profile);

            return profile;
        }

        public async Task<AT9.Models.AbstractTheatre.AbstractProfile> CancelProfileChanges(AT9.Models.AbstractTheatre.AbstractProfile item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProfileUpdated(AT9.Models.AbstractTheatre.AbstractProfile item);
        partial void OnAfterProfileUpdated(AT9.Models.AbstractTheatre.AbstractProfile item);

        public async Task<AT9.Models.AbstractTheatre.AbstractProfile> UpdateProfile(int profileid, AT9.Models.AbstractTheatre.AbstractProfile profile)
        {
            OnProfileUpdated(profile);

            var itemToUpdate = Context.Profiles
                              .Where(i => i.ProfileId == profile.ProfileId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(profile);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProfileUpdated(profile);

            return profile;
        }

        partial void OnProfileDeleted(AT9.Models.AbstractTheatre.AbstractProfile item);
        partial void OnAfterProfileDeleted(AT9.Models.AbstractTheatre.AbstractProfile item);

        public async Task<AT9.Models.AbstractTheatre.AbstractProfile> DeleteProfile(int profileid)
        {
            var itemToDelete = Context.Profiles
                              .Where(i => i.ProfileId == profileid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProfileDeleted(itemToDelete);


            Context.Profiles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProfileDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportVideoViewsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/videoviews/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/videoviews/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportVideoViewsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/videoviews/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/videoviews/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnVideoViewsRead(ref IQueryable<AT9.Models.AbstractTheatre.VideoView> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.VideoView>> GetVideoViews(Query query = null)
        {
            var items = Context.VideoViews.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnVideoViewsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnVideoViewGet(AT9.Models.AbstractTheatre.VideoView item);
        partial void OnGetVideoViewByVideoViewId(ref IQueryable<AT9.Models.AbstractTheatre.VideoView> items);


        public async Task<AT9.Models.AbstractTheatre.VideoView> GetVideoViewByVideoViewId(int videoviewid)
        {
            var items = Context.VideoViews
                              .AsNoTracking()
                              .Where(i => i.VideoViewId == videoviewid);

 
            OnGetVideoViewByVideoViewId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnVideoViewGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnVideoViewCreated(AT9.Models.AbstractTheatre.VideoView item);
        partial void OnAfterVideoViewCreated(AT9.Models.AbstractTheatre.VideoView item);

        public async Task<AT9.Models.AbstractTheatre.VideoView> CreateVideoView(AT9.Models.AbstractTheatre.VideoView videoview)
        {
            OnVideoViewCreated(videoview);

            var existingItem = Context.VideoViews
                              .Where(i => i.VideoViewId == videoview.VideoViewId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.VideoViews.Add(videoview);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(videoview).State = EntityState.Detached;
                throw;
            }

            OnAfterVideoViewCreated(videoview);

            return videoview;
        }

        public async Task<AT9.Models.AbstractTheatre.VideoView> CancelVideoViewChanges(AT9.Models.AbstractTheatre.VideoView item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnVideoViewUpdated(AT9.Models.AbstractTheatre.VideoView item);
        partial void OnAfterVideoViewUpdated(AT9.Models.AbstractTheatre.VideoView item);

        public async Task<AT9.Models.AbstractTheatre.VideoView> UpdateVideoView(int videoviewid, AT9.Models.AbstractTheatre.VideoView videoview)
        {
            OnVideoViewUpdated(videoview);

            var itemToUpdate = Context.VideoViews
                              .Where(i => i.VideoViewId == videoview.VideoViewId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(videoview);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterVideoViewUpdated(videoview);

            return videoview;
        }

        partial void OnVideoViewDeleted(AT9.Models.AbstractTheatre.VideoView item);
        partial void OnAfterVideoViewDeleted(AT9.Models.AbstractTheatre.VideoView item);

        public async Task<AT9.Models.AbstractTheatre.VideoView> DeleteVideoView(int videoviewid)
        {
            var itemToDelete = Context.VideoViews
                              .Where(i => i.VideoViewId == videoviewid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnVideoViewDeleted(itemToDelete);


            Context.VideoViews.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterVideoViewDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportDeliveryAddressesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/deliveryaddresses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/deliveryaddresses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDeliveryAddressesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/deliveryaddresses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/deliveryaddresses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDeliveryAddressesRead(ref IQueryable<AT9.Models.AbstractTheatre.DeliveryAddress> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.DeliveryAddress>> GetDeliveryAddresses(Query query = null)
        {
            var items = Context.DeliveryAddresses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDeliveryAddressesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDeliveryAddressGet(AT9.Models.AbstractTheatre.DeliveryAddress item);
        partial void OnGetDeliveryAddressByDeliveryAddressId(ref IQueryable<AT9.Models.AbstractTheatre.DeliveryAddress> items);


        public async Task<AT9.Models.AbstractTheatre.DeliveryAddress> GetDeliveryAddressByDeliveryAddressId(int deliveryaddressid)
        {
            var items = Context.DeliveryAddresses
                              .AsNoTracking()
                              .Where(i => i.DeliveryAddressId == deliveryaddressid);

 
            OnGetDeliveryAddressByDeliveryAddressId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDeliveryAddressGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDeliveryAddressCreated(AT9.Models.AbstractTheatre.DeliveryAddress item);
        partial void OnAfterDeliveryAddressCreated(AT9.Models.AbstractTheatre.DeliveryAddress item);

        public async Task<AT9.Models.AbstractTheatre.DeliveryAddress> CreateDeliveryAddress(AT9.Models.AbstractTheatre.DeliveryAddress deliveryaddress)
        {
            OnDeliveryAddressCreated(deliveryaddress);

            var existingItem = Context.DeliveryAddresses
                              .Where(i => i.DeliveryAddressId == deliveryaddress.DeliveryAddressId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.DeliveryAddresses.Add(deliveryaddress);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(deliveryaddress).State = EntityState.Detached;
                throw;
            }

            OnAfterDeliveryAddressCreated(deliveryaddress);

            return deliveryaddress;
        }

        public async Task<AT9.Models.AbstractTheatre.DeliveryAddress> CancelDeliveryAddressChanges(AT9.Models.AbstractTheatre.DeliveryAddress item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDeliveryAddressUpdated(AT9.Models.AbstractTheatre.DeliveryAddress item);
        partial void OnAfterDeliveryAddressUpdated(AT9.Models.AbstractTheatre.DeliveryAddress item);

        public async Task<AT9.Models.AbstractTheatre.DeliveryAddress> UpdateDeliveryAddress(int deliveryaddressid, AT9.Models.AbstractTheatre.DeliveryAddress deliveryaddress)
        {
            OnDeliveryAddressUpdated(deliveryaddress);

            var itemToUpdate = Context.DeliveryAddresses
                              .Where(i => i.DeliveryAddressId == deliveryaddress.DeliveryAddressId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(deliveryaddress);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDeliveryAddressUpdated(deliveryaddress);

            return deliveryaddress;
        }

        public async Task<int> GetProfileID(string AspNetUserID)
        {
            var queryprofiles = await this.GetProfiles();
            List<AbstractProfile> profiles = queryprofiles.Where(x => x.AspNetUserId == AspNetUserID).ToList();
            AbstractProfile profile = profiles.First();
            return profile.ProfileId;
        }

        partial void OnDeliveryAddressDeleted(AT9.Models.AbstractTheatre.DeliveryAddress item);
        partial void OnAfterDeliveryAddressDeleted(AT9.Models.AbstractTheatre.DeliveryAddress item);

        public async Task<AT9.Models.AbstractTheatre.DeliveryAddress> DeleteDeliveryAddress(int deliveryaddressid)
        {
            var itemToDelete = Context.DeliveryAddresses
                              .Where(i => i.DeliveryAddressId == deliveryaddressid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDeliveryAddressDeleted(itemToDelete);


            Context.DeliveryAddresses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDeliveryAddressDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrderProductInstancesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/orderproductinstances/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/orderproductinstances/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrderProductInstancesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/orderproductinstances/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/orderproductinstances/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrderProductInstancesRead(ref IQueryable<AT9.Models.AbstractTheatre.OrderProductInstance> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.OrderProductInstance>> GetOrderProductInstances(Query query = null)
        {
            var items = Context.OrderProductInstances.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOrderProductInstancesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderProductInstanceGet(AT9.Models.AbstractTheatre.OrderProductInstance item);
        partial void OnGetOrderProductInstanceByOrderProductId(ref IQueryable<AT9.Models.AbstractTheatre.OrderProductInstance> items);


        public async Task<AT9.Models.AbstractTheatre.OrderProductInstance> GetOrderProductInstanceByOrderProductId(int orderproductid)
        {
            var items = Context.OrderProductInstances
                              .AsNoTracking()
                              .Where(i => i.OrderProductId == orderproductid);

 
            OnGetOrderProductInstanceByOrderProductId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrderProductInstanceGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderProductInstanceCreated(AT9.Models.AbstractTheatre.OrderProductInstance item);
        partial void OnAfterOrderProductInstanceCreated(AT9.Models.AbstractTheatre.OrderProductInstance item);

        public async Task<AT9.Models.AbstractTheatre.OrderProductInstance> CreateOrderProductInstance(AT9.Models.AbstractTheatre.OrderProductInstance orderproductinstance)
        {
            OnOrderProductInstanceCreated(orderproductinstance);

            var existingItem = Context.OrderProductInstances
                              .Where(i => i.OrderProductId == orderproductinstance.OrderProductId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.OrderProductInstances.Add(orderproductinstance);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(orderproductinstance).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderProductInstanceCreated(orderproductinstance);

            return orderproductinstance;
        }

        public async Task<AT9.Models.AbstractTheatre.OrderProductInstance> CancelOrderProductInstanceChanges(AT9.Models.AbstractTheatre.OrderProductInstance item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderProductInstanceUpdated(AT9.Models.AbstractTheatre.OrderProductInstance item);
        partial void OnAfterOrderProductInstanceUpdated(AT9.Models.AbstractTheatre.OrderProductInstance item);

        public async Task<AT9.Models.AbstractTheatre.OrderProductInstance> UpdateOrderProductInstance(int orderproductid, AT9.Models.AbstractTheatre.OrderProductInstance orderproductinstance)
        {
            OnOrderProductInstanceUpdated(orderproductinstance);

            var itemToUpdate = Context.OrderProductInstances
                              .Where(i => i.OrderProductId == orderproductinstance.OrderProductId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(orderproductinstance);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderProductInstanceUpdated(orderproductinstance);

            return orderproductinstance;
        }

        partial void OnOrderProductInstanceDeleted(AT9.Models.AbstractTheatre.OrderProductInstance item);
        partial void OnAfterOrderProductInstanceDeleted(AT9.Models.AbstractTheatre.OrderProductInstance item);

        public async Task<AT9.Models.AbstractTheatre.OrderProductInstance> DeleteOrderProductInstance(int orderproductid)
        {
            var itemToDelete = Context.OrderProductInstances
                              .Where(i => i.OrderProductId == orderproductid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderProductInstanceDeleted(itemToDelete);


            Context.OrderProductInstances.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderProductInstanceDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPaymentMethodsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/paymentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/paymentmethods/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPaymentMethodsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/abstracttheatre/paymentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/abstracttheatre/paymentmethods/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPaymentMethodsRead(ref IQueryable<AT9.Models.AbstractTheatre.PaymentMethod> items);

        public async Task<IQueryable<AT9.Models.AbstractTheatre.PaymentMethod>> GetPaymentMethods(Query query = null)
        {
            var items = Context.PaymentMethods.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPaymentMethodsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPaymentMethodGet(AT9.Models.AbstractTheatre.PaymentMethod item);
        partial void OnGetPaymentMethodByPaymentMethodId(ref IQueryable<AT9.Models.AbstractTheatre.PaymentMethod> items);


        public async Task<AT9.Models.AbstractTheatre.PaymentMethod> GetPaymentMethodByPaymentMethodId(int paymentmethodid)
        {
            var items = Context.PaymentMethods
                              .AsNoTracking()
                              .Where(i => i.PaymentMethodId == paymentmethodid);

 
            OnGetPaymentMethodByPaymentMethodId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPaymentMethodGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPaymentMethodCreated(AT9.Models.AbstractTheatre.PaymentMethod item);
        partial void OnAfterPaymentMethodCreated(AT9.Models.AbstractTheatre.PaymentMethod item);

        public async Task<AT9.Models.AbstractTheatre.PaymentMethod> CreatePaymentMethod(AT9.Models.AbstractTheatre.PaymentMethod paymentmethod)
        {
            OnPaymentMethodCreated(paymentmethod);

            var existingItem = Context.PaymentMethods
                              .Where(i => i.PaymentMethodId == paymentmethod.PaymentMethodId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.PaymentMethods.Add(paymentmethod);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(paymentmethod).State = EntityState.Detached;
                throw;
            }

            OnAfterPaymentMethodCreated(paymentmethod);

            return paymentmethod;
        }

        public async Task<AT9.Models.AbstractTheatre.PaymentMethod> CancelPaymentMethodChanges(AT9.Models.AbstractTheatre.PaymentMethod item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPaymentMethodUpdated(AT9.Models.AbstractTheatre.PaymentMethod item);
        partial void OnAfterPaymentMethodUpdated(AT9.Models.AbstractTheatre.PaymentMethod item);

        public async Task<AT9.Models.AbstractTheatre.PaymentMethod> UpdatePaymentMethod(int paymentmethodid, AT9.Models.AbstractTheatre.PaymentMethod paymentmethod)
        {
            OnPaymentMethodUpdated(paymentmethod);

            var itemToUpdate = Context.PaymentMethods
                              .Where(i => i.PaymentMethodId == paymentmethod.PaymentMethodId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(paymentmethod);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPaymentMethodUpdated(paymentmethod);

            return paymentmethod;
        }

        partial void OnPaymentMethodDeleted(AT9.Models.AbstractTheatre.PaymentMethod item);
        partial void OnAfterPaymentMethodDeleted(AT9.Models.AbstractTheatre.PaymentMethod item);

        public async Task<AT9.Models.AbstractTheatre.PaymentMethod> DeletePaymentMethod(int paymentmethodid)
        {
            var itemToDelete = Context.PaymentMethods
                              .Where(i => i.PaymentMethodId == paymentmethodid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPaymentMethodDeleted(itemToDelete);


            Context.PaymentMethods.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPaymentMethodDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}