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
using System.Text.RegularExpressions;

namespace AT9.Components.Pages
{
    public partial class AddDeliveryAddress
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

        protected override async Task OnInitializedAsync()
        {
            deliveryAddress = new AT9.Models.AbstractTheatre.DeliveryAddress();
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.DeliveryAddress deliveryAddress;

        [Inject]
        protected SecurityService Security { get; set; }

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
                    deliveryAddress.ProfileId = await AbstractTheatreService.GetProfileID(Security.User.Id);
                    await AbstractTheatreService.CreateDeliveryAddress(deliveryAddress);
                    DialogService.Close(deliveryAddress);
                }
                catch (Exception ex)
                {
                    errorVisible = true;
                }
            }
        }

        public string Validate()
        {

            if (deliveryAddress.Addresslineone == null || deliveryAddress.Addresslineone == "")
            {
                return "Address line one is required";
            }

            if (deliveryAddress.City == null || deliveryAddress.City == "")
            {
                return "City is required";
            }
            if (deliveryAddress.Fullname == null || deliveryAddress.Fullname == "")
            {
                return "Full name is required";
            }
            if (deliveryAddress.Zipcode == null || !IsValidZipcode(deliveryAddress.Zipcode))
            {
                return "Invalid zipcode";
            }
            return "";
        }

        private bool IsValidZipcode(string zipcode)
        {
            return Regex.IsMatch(zipcode, @"^\d{5}$");
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}