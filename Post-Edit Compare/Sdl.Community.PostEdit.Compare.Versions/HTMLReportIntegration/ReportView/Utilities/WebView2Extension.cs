using Microsoft.Web.WebView2.Wpf;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities
{
    public static class WebView2Extension
    {
        public static async Task<string> RunScript(this WebView2 browser, string script)
        {
            var result = "";
            await browser.Dispatcher.Invoke(async () =>
            {
                result = await browser.ExecuteScriptAsync(script);
            });

            return result;
        }
    }
}