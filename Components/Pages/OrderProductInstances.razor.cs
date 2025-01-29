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
    public partial class OrderProductInstances
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

        protected IEnumerable<AT9.Models.AbstractTheatre.OrderProductInstance> orderProductInstances;

        protected RadzenDataGrid<AT9.Models.AbstractTheatre.OrderProductInstance> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            orderProductInstances = await AbstractTheatreService.GetOrderProductInstances();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddOrderProductInstance>("Add OrderProductInstance", null);
            await grid0.Reload();
        }

        protected async Task EditRow(AT9.Models.AbstractTheatre.OrderProductInstance args)
        {
            await DialogService.OpenAsync<EditOrderProductInstance>("Edit OrderProductInstance", new Dictionary<string, object> { {"OrderProductId", args.OrderProductId} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AT9.Models.AbstractTheatre.OrderProductInstance orderProductInstance)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AbstractTheatreService.DeleteOrderProductInstance(orderProductInstance.OrderProductId);

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
                    Detail = $"Unable to delete OrderProductInstance"
                });
            }
        }
    }
}