using System.Net.Http;
using AT9.Models;
using AT9.Models.AbstractTheatre;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AT9.Components.Pages
{
    public partial class Index
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
        protected SecurityService Security { get; set; }

        [Inject]
        protected AbstractTheatreService AbstractTheatreService { get; set; }


        protected async override Task OnInitializedAsync()
        {
            
          
        }

        public void NavigateToShop()
        {
            NavigationManager.NavigateTo($"/shopping-page");
        }

        public void NavigateToGallery()
        {
            NavigationManager.NavigateTo($"/gallery");
        }

        public void NavigateToProfile()
        {
            NavigationManager.NavigateTo($"/profile-management");
        }
      

        /*


        List<ApplicationUser> userlist = usersqueryable.ToList();
        userlist.First(x => x.Email == user.Email);
        */

    }
}