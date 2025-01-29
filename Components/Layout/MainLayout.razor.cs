using System.Net.Http;
using AT9.Components.Pages;
using AT9.Models.AbstractTheatre;
using AT9.Services;
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
using static AT9.AbstractTheatreService;

namespace AT9.Components.Layout
{
    public partial class MainLayout
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

        private bool sidebarExpanded = true;

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected AbstractTheatreService AbstractTheatreService { get; set; }


        public int Carttotal { get; set; }
       

        protected override async Task OnInitializedAsync()
        {
            AT9.Models.AbstractTheatre.AbstractProfile profile1 = new AT9.Models.AbstractTheatre.AbstractProfile();
            profile1.AspNetUserId = Security.User.Id;
            var x = await AbstractTheatreService.GetProfiles();
            List<AbstractProfile> profiles1 = x.Where(x => x.AspNetUserId == Security.User.Id).ToList();

            if (profiles1.Count == 0)
            {
                profile1.Username = StripEmail(Security.User.UserName);

                await AbstractTheatreService.CreateProfile(profile1);

            }

            var query = await AbstractTheatreService.GetCartProductUserRelationships();
            var queryprofiles = await AbstractTheatreService.GetProfiles();

            LayoutNotifier.RegisterRefresh(StateHasChanged);

            List<AbstractProfile> profiles = queryprofiles.Where(x => x.AspNetUserId == Security.User.Id).ToList();
            AbstractProfile profile = profiles.First();
            List<CartProductUserRelationship> relationships = query.Where(x => x.ProfileId == profile.ProfileId).ToList();
            Carttotal = relationships.Count();

        }

        public async void BalanceCart()
        {
            var query = await AbstractTheatreService.GetCartProductUserRelationships();
            var queryprofiles = await AbstractTheatreService.GetProfiles();

            List<AbstractProfile> profiles = queryprofiles.Where(x => x.AspNetUserId == Security.User.Id).ToList();
            AbstractProfile profile = profiles.First();
            List<CartProductUserRelationship> relationships = query.Where(x => x.ProfileId == profile.ProfileId).ToList();
            Carttotal = relationships.Count();
            StateHasChanged();
        }

        public static string StripEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            if (atIndex >= 0)
            {
                return email.Substring(0, atIndex);
            }
            return email; 
        }
        public void UpdateCart()
        {
            Carttotal++;
            StateHasChanged();
        }
        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        public void NavigateToCheckout()
        {
            NavigationManager.NavigateTo($"/shopping-cart");
        }

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }
    }
}
