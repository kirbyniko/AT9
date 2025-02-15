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
    public partial class EditProduct
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
            product = await AbstractTheatreService.GetProductByProductId(ProductId);
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.Product product;

        [Inject]
        protected SecurityService Security { get; set; }

        public string Validate()
        {

            if (product.ProductName == null || product.ProductName == "")
            {
                return "Name is required";
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
                    await AbstractTheatreService.UpdateProduct(ProductId, product);
                    DialogService.Close(product);
                }
                catch (Exception ex)
                {
                    errorVisible = true;
                }
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}