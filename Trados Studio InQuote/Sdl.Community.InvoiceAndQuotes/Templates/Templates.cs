using System.Collections.Generic;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;

namespace Sdl.Community.InvoiceAndQuotes.Templates
{
    internal static class Templates
    {
        public static string PerfectMatch { get { return ResManager.PerfectMatch; } }
        public static string ContextMatch { get { return ResManager.ContextMatch; } }
        public static string Repetitions { get { return ResManager.Repetitions; } }
        public static string Percent100 { get { return ResManager.Percent100; } }
        public static string Percent95 { get { return ResManager.Percent95; } }
        public static string Percent85 { get { return ResManager.Percent85; } }
        public static string Percent75 { get { return ResManager.Percent75; } }
        public static string Percent50 { get { return ResManager.Percent50; } }
        public static string New { get { return ResManager.New; } }
        public static string RepsAnd100Percent { get { return ResManager.RepsAnd100Percent; } }
        public static string FuzzyMatches { get { return ResManager.FuzzyMatches; } }
        public static string NoMatch { get { return ResManager.NoMatch; } }
        public static string Tags { get { return ResManager.Tags; } }

        public static string CharactersPerLine { get { return ResManager.CharactersPerLine; } }
        public static string RatePerLine { get { return ResManager.RatePerLine; } }

        public const string PerfectMatchPathInFile = "analyse/perfect";
        public const string ContextMatchPathInFile = "analyse/inContextExact";
        public const string RepetitionsPathInFile = "analyse/repeated";
        public const string Percent100PathInFile = "analyse/exact";
        public const string Percent95PathInFile = "analyse/fuzzy[@min = 95]";
        public const string Percent85PathInFile = "analyse/fuzzy[@min = 85]";
        public const string Percent75PathInFile = "analyse/fuzzy[@min = 75]";
        public const string Percent50PathInFile = "analyse/fuzzy[@min = 50]";
        public const string NewPathInFile = "analyse/new";
        public const string TagsPathInFile = "analyse/total/@tags";

        private static UIResources ResManager
        {
            get { return new UIResources(Settings.GetSavedCulture()); }
        }

        public static List<RateValue> GetStandardRates()
        {
            return new List<RateValue>()
                {

                    new RateValue() {ResourceToken = "PerfectMatch", Type = PerfectMatch, Rate = 0},
                    new RateValue() {ResourceToken = "ContextMatch", Type = ContextMatch, Rate = 0},
                    new RateValue() {ResourceToken = "Repetitions", Type = Repetitions, Rate = 0},
                    new RateValue() {ResourceToken = "Percent100", Type = Percent100, Rate = 0},
                    new RateValue() {ResourceToken = "Percent95", Type = Percent95, Rate = 0},
                    new RateValue() {ResourceToken = "Percent85", Type = Percent85, Rate = 0},
                    new RateValue() {ResourceToken = "Percent75", Type = Percent75, Rate = 0},
                    new RateValue() {ResourceToken = "Percent50", Type = Percent50, Rate = 0},
                    new RateValue() {ResourceToken = "New", Type = New, Rate = 0},
                    new RateValue() {ResourceToken = "Tags", Type = Tags, Rate = 0}
                };
        }
    }
}