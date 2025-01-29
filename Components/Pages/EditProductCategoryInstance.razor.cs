using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace AT9.Components.Pages
{
    public partial class EditProductCategoryInstance
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
        public string ProductCategoryInstanceId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            productCategoryInstance = await AbstractTheatreService.GetProductCategoryInstanceByProductCategoryInstanceId(ProductCategoryInstanceId);
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.ProductCategoryInstance productCategoryInstance;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await AbstractTheatreService.UpdateProductCategoryInstance(ProductCategoryInstanceId, productCategoryInstance);
                DialogService.Close(productCategoryInstance);
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