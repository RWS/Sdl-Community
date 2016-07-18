using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests.Utilities
{
    public static class NumberVerifierLocalizationsSettings
    {
        private static Mock<INumberVerifierSettings> Settings()
        {
            var iNumberSettingsMock = new Mock<INumberVerifierSettings>(MockBehavior.Loose);
            iNumberSettingsMock.Setup(r => r.ReportAddedNumbers).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportModifiedAlphanumerics).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportModifiedNumbers).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportRemovedNumbers).Returns(true);
            

            return iNumberSettingsMock;
        }

        public static void InitSeparators(Mock<INumberVerifierSettings> mock)
        {
            mock.Setup(r => r.GetSourceDecimalSeparators())
                .Returns(GetSourceDecimalSeparators(mock.Object));
            mock.Setup(r => r.GetSourceThousandSeparators())
                .Returns(GetSourceThousandSeparators(mock.Object));

            mock.Setup(r => r.GetTargetThousandSeparators())
                .Returns(GetTargetThousandSeparators(mock.Object));
            mock.Setup(r => r.GetTargetDecimalSeparators())
               .Returns(GetTargetDecimalSeparators(mock.Object));
        }

        public static IEnumerable<string> GetSourceDecimalSeparators(INumberVerifierSettings numberVerifierSettings)
        {
            yield return numberVerifierSettings.SourceDecimalComma ? @"\u002C" : string.Empty;
            yield return numberVerifierSettings.SourceDecimalPeriod ? @"\u002E" : string.Empty;
            yield return numberVerifierSettings.SourceDecimalCustomSeparator ? numberVerifierSettings.GetSourceDecimalCustomSeparator
                : string.Empty;
        }

        public static IEnumerable<string> GetTargetDecimalSeparators(INumberVerifierSettings numberVerifierSettings)
        {
            yield return numberVerifierSettings.TargetDecimalComma ? @"\u002C" : string.Empty;
            yield return numberVerifierSettings.TargetDecimalPeriod ? @"\u002E" : string.Empty;
            yield return numberVerifierSettings.TargetDecimalCustomSeparator ?
                numberVerifierSettings.GetTargetDecimalCustomSeparator
                : string.Empty;
        }

        public static IEnumerable<string> GetSourceThousandSeparators(INumberVerifierSettings numberVerifierSettings)
        {
            yield return numberVerifierSettings.SourceThousandsSpace ? @"\u0020" : string.Empty;
            yield return numberVerifierSettings.SourceThousandsNobreakSpace ? @"\u00A0" : string.Empty;
            yield return numberVerifierSettings.SourceThousandsThinSpace ? @"\u2009" : string.Empty;
            yield return numberVerifierSettings.SourceThousandsNobreakThinSpace ? @"\u202F" : string.Empty;
            yield return numberVerifierSettings.SourceThousandsComma ? @"\u002C" : string.Empty;
            yield return numberVerifierSettings.SourceThousandsPeriod ? @"\u002E" : string.Empty;
            yield return numberVerifierSettings.SourceThousandsCustomSeparator
                ? numberVerifierSettings.GetSourceThousandsCustomSeparator
                : string.Empty;

        }



        public static IEnumerable<string> GetTargetThousandSeparators(INumberVerifierSettings numberVerifierSettings)
        {
            yield return numberVerifierSettings.TargetThousandsSpace ? @"\u0020" : string.Empty;
            yield return numberVerifierSettings.TargetThousandsNobreakSpace ? @"\u00A0" : string.Empty;
            yield return numberVerifierSettings.TargetThousandsThinSpace ? @"\u2009" : string.Empty;
            yield return numberVerifierSettings.TargetThousandsNobreakThinSpace ? @"\u202F" : string.Empty;
            yield return numberVerifierSettings.TargetThousandsComma ? @"\u002C" : string.Empty;
            yield return numberVerifierSettings.TargetThousandsPeriod ? @"\u002E" : string.Empty;
            yield return numberVerifierSettings.TargetThousandsCustomSeparator
                ? numberVerifierSettings.GetTargetThousandsCustomSeparator
                : string.Empty;
        }

        public static Mock<INumberVerifierSettings> AllowLocalization()
        {
            var iNumberSettingsMock = Settings();
            iNumberSettingsMock.Setup(x => x.AllowLocalizations).Returns(true);

            return iNumberSettingsMock;
            
        }

        public static Mock<INumberVerifierSettings> RequireLocalization()
        {
            var iNumberSettingsMock = Settings();
            iNumberSettingsMock.Setup(x => x.RequireLocalizations).Returns(true);

            return iNumberSettingsMock;

        }

        public static Mock<INumberVerifierSettings> PreventLocalization()
        {
            var iNumberSettingsMock = Settings();
            iNumberSettingsMock.Setup(x => x.PreventLocalizations).Returns(true);

            return iNumberSettingsMock;

        }
    }
}
