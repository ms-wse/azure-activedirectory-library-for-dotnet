// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Test.ADAL.WinPhone.RealWorldApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IWebAuthenticationContinuable
    {
        private AuthenticationContext ctx  = 
            AuthenticationContext.CreateAsync("https://login.windows.net/aaltests.onmicrosoft.com",
                false).GetResults();
        
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string resource = "https://graph.windows.net/";

            AuthenticationResult result = await ctx.AcquireTokenSilentlyAsync(resource, App.ClientId);
            if (result != null && result.Status == AuthenticationStatus.Succeeded)
            {
                DoGraphApi(result);
            }
            else
            {
                ctx.AcquireTokenAndContinue(resource, App.ClientId, new Uri("https://login.live.com/"), DoGraphApi);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            serviceMgmt.Text = string.Empty;
            string resource = "https://management.core.windows.net/";
            AuthenticationResult result = await ctx.AcquireTokenSilentlyAsync(resource, App.ClientId);
            if (result != null && result.Status == AuthenticationStatus.Succeeded)
            {
                DoServiceManagement(result);
            }
            else
            {
                ctx.AcquireTokenAndContinue(resource, App.ClientId, DoServiceManagement);
            }
        }

        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
           await ctx.ContinueAcquireToken(args);
            
        }


        public async void DoServiceManagement(AuthenticationResult result)
        {

            if (result.Status == AuthenticationStatus.Succeeded)
            {
                serviceMgmt.Text = await CallServiceManagementAPI(result.AccessToken);
            }
            else
            {
                serviceMgmt.Text = string.Format("ERROR {0} - {1}", result.Error, result.ErrorDescription);
            }
        }

        private async Task<string> CallServiceManagementAPI(string accessToken)
        {
            IDictionary<string, string> headers = new Dictionary<string, string>();
            headers["x-ms-version"] = "2013-08-01";
            headers["Authorization"] = "Bearer " + accessToken;
            return await HttpHelper.GetResourceData("https://management.core.windows.net/subscriptions", headers);
        }

        public async void DoGraphApi(AuthenticationResult result)
        {
            MessageDialog md = new MessageDialog(string.Empty);
            if (result.Status == AuthenticationStatus.Succeeded)
            {
                md.Content = await CallGraphApi(result.AccessToken);
            }
            else
            {
                md.Content = string.Format("ERROR {0} - {1}", result.Error, result.ErrorDescription);
            }
            md.ShowAsync();
        }

        private async Task<string> CallGraphApi(string accessToken)
        {
            IDictionary<string, string> headers = new Dictionary<string, string>();
            headers["Authorization"] = "Bearer " + accessToken;
            return await HttpHelper.GetResourceData("https://graph.windows.net/users", headers);
        }
    
    }


}
