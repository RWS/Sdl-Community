using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace LanguageWeaverProvider.Helpers
{
    public static class WebView2ThemeHelper
    {
        private const string BrowserScript = @"document.oncontextmenu = (e) => { return false; };

                                                document.onreadystatechange = () => {
                                                    if (document.readyState != 'interactive') 
                                                        { return; }

                                                    for (const footerElement of document.getElementsByTagName('footer')) 
                                                    {
                                                        footerElement.style.visibility = 'collapse';
                                                        footerElement.style.padding = '0px';
                                                        footerElement.style.position = 'fixed';
                                                    }
                                                };";

        public static async Task InitializeThemeAsync(WebView2 webView, string subFolder)
        {
            bool isDark = WindowThemeHelper.IsDarkTheme();

            if (webView.CoreWebView2 is null)
            {
                var userDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, subFolder);
                Directory.CreateDirectory(userDataFolder);

                var options = new CoreWebView2EnvironmentOptions
                {
                    AllowSingleSignOnUsingOSPrimaryAccount = true
                };

                if (isDark)
                {
                    options.AdditionalBrowserArguments = "--enable-features=WebContentsForceDark";
                }

                var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);

                webView.CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = userDataFolder
                };

                await webView.EnsureCoreWebView2Async(environment);
                await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(BrowserScript);
            }

            webView.CoreWebView2.Profile.PreferredColorScheme = isDark
                ? CoreWebView2PreferredColorScheme.Dark
                : CoreWebView2PreferredColorScheme.Light;

            webView.DefaultBackgroundColor = isDark ? Color.FromArgb(255, 43, 43, 43) : Color.White;
        }
    }
}