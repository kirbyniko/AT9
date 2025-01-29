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
    public partial class PaymentMethods
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

        protected IEnumerable<AT9.Models.AbstractTheatre.PaymentMethod> paymentMethods;

        protected RadzenDataGrid<AT9.Models.AbstractTheatre.PaymentMethod> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            paymentMethods = await AbstractTheatreService.GetPaymentMethods();
            int profileid = await AbstractTheatreService.GetProfileID(Security.User.Id);
            paymentMethods = paymentMethods.Where(x => x.ProfileId == profileid);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddPaymentMethod>("Add Payment Method", null);
            await grid0.Reload();
        }

        protected async Task EditRow(AT9.Models.AbstractTheatre.PaymentMethod args)
        {
            await DialogService.OpenAsync<EditPaymentMethod>("Edit Payment Method", new Dictionary<string, object> { {"PaymentMethodId", args.PaymentMethodId} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AT9.Models.AbstractTheatre.PaymentMethod paymentMethod)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AbstractTheatreService.DeletePaymentMethod(paymentMethod.PaymentMethodId);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Payment Method"
                });
            }
        }
    }
}