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
    public partial class EditPaymentMethod
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
        public int PaymentMethodId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            paymentMethod = await AbstractTheatreService.GetPaymentMethodByPaymentMethodId(PaymentMethodId);
        }
        protected bool errorVisible;
        protected AT9.Models.AbstractTheatre.PaymentMethod paymentMethod;

        [Inject]
        protected SecurityService Security { get; set; }

        public string Validate(PaymentMethod paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod.Cardnumber) || !IsValidCardNumber(paymentMethod.Cardnumber))
            {
                return "Invalid Card Number";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.Cardname))
            {
                return "Cardholder's Name is required";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.Expiration) || !IsValidExpiration(paymentMethod.Expiration))
            {
                return "Invalid Expiration Date";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.Securitycode) || !IsValidSecurityCode(paymentMethod.Securitycode))
            {
                return "Invalid Security Code";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.Fullname))
            {
                return "Full Name is required";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.Zipcode) || !IsValidZipcode(paymentMethod.Zipcode))
            {
                return "Invalid Zipcode";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.Addresslineone))
            {
                return "Address Line 1 is required";
            }

            if (!string.IsNullOrWhiteSpace(paymentMethod.Addresslinetwo) && string.IsNullOrWhiteSpace(paymentMethod.Addresslinetwo))
            {
                return "Address Line 2 should not be empty if provided";
            }

            if (string.IsNullOrWhiteSpace(paymentMethod.City))
            {
                return "City is required";
            }

            return string.Empty; 
        }

        private bool IsValidCardNumber(string cardnumber)
        {
            if (cardnumber.Length < 13 || cardnumber.Length > 19)
                return false;

            int sum = 0;
            bool shouldDouble = false;
            for (int i = cardnumber.Length - 1; i >= 0; i--)
            {
                int digit = cardnumber[i] - '0';
                if (shouldDouble)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                shouldDouble = !shouldDouble;
            }

            return sum % 10 == 0;
        }

        private bool IsValidExpiration(string expiration)
        {
            return Regex.IsMatch(expiration, @"^(0[1-9]|1[0-2])\/?([0-9]{2}|[0-9]{4})$");
        }

        private bool IsValidSecurityCode(string securitycode)
        {
            return Regex.IsMatch(securitycode, @"^\d{3,4}$");
        }

        private bool IsValidZipcode(string zipcode)
        {
            return Regex.IsMatch(zipcode, @"^\d{5}$");
        }

        protected async Task FormSubmit()
        {
            if (Validate(paymentMethod) != "")
            {
                DialogService.Alert(Validate(paymentMethod));
            }
            else
            {
                try
                {
                    await AbstractTheatreService.UpdatePaymentMethod(PaymentMethodId, paymentMethod);
                    DialogService.Close(paymentMethod);
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