using Sdl.Community.TargetWordCount.Models;
using System.ComponentModel;

namespace Sdl.Community.TargetWordCount.Helpers
{
    public static class DefaultInvoiceRateService
    {
        public static BindingList<InvoiceItem> CreateDefaultInvoiceRates()
        {
            return new BindingList<InvoiceItem>()
            {
                new InvoiceItem(RateType.Locked, string.Empty),
                new InvoiceItem(RateType.PerfectMatch, string.Empty),
                new InvoiceItem(RateType.ContextMatch, string.Empty),
                new InvoiceItem(RateType.Repetitions, string.Empty),
                new InvoiceItem(RateType.CrossFileRepetitions, string.Empty),
                new InvoiceItem(RateType.OneHundred, string.Empty),
                new InvoiceItem(RateType.NinetyFive, string.Empty),
                new InvoiceItem(RateType.EightyFive, string.Empty),
                new InvoiceItem(RateType.SeventyFive, string.Empty),
                new InvoiceItem(RateType.Fifty, string.Empty),
                new InvoiceItem(RateType.New, string.Empty),
                new InvoiceItem(RateType.Total, string.Empty)
            };
        }

    }
}
