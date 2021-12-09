using System.Collections.Generic;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests.Utilities
{
	public static class NumberVerifierLocalizationsSettings
    {
        public static Mock<INumberVerifierSettings> AllowLocalization()
        {
            var iNumberSettingsMock = Settings();
            iNumberSettingsMock.Setup(x => x.AllowLocalizations).Returns(true);

            return iNumberSettingsMock;
        }

        public static string GetAlphanumericsCustomSeparator(INumberVerifierSettings numberVerifierSettings)
        {
            return numberVerifierSettings.CustomsSeparatorsAlphanumerics ?
                numberVerifierSettings.AlphanumericsCustomSeparator
                : string.Empty;
        }

        public static string GetHindi(INumberVerifierSettings numberVerifierSettings)
        {
            return numberVerifierSettings.HindiNumberVerification ?
                numberVerifierSettings.HindiNumber
                : string.Empty;
        }

        public static IEnumerable<string> GetSourceDecimalSeparators(INumberVerifierSettings numberVerifierSettings)
        {
            yield return numberVerifierSettings.SourceDecimalComma ? @"\u002C" : string.Empty;
            yield return numberVerifierSettings.SourceDecimalPeriod ? @"\u002E" : string.Empty;
            yield return numberVerifierSettings.SourceDecimalCustom ? numberVerifierSettings.SourceDecimalCustomSeparator
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
            yield return numberVerifierSettings.SourceThousandsCustom
                ? numberVerifierSettings.SourceThousandsCustomSeparator
                : string.Empty;
        }

        public static IEnumerable<string> GetTargetDecimalSeparators(INumberVerifierSettings numberVerifierSettings)
        {
            yield return numberVerifierSettings.TargetDecimalComma ? @"\u002C" : string.Empty;
            yield return numberVerifierSettings.TargetDecimalPeriod ? @"\u002E" : string.Empty;
            yield return numberVerifierSettings.TargetDecimalCustom ?
                numberVerifierSettings.TargetDecimalCustomSeparator
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
            yield return numberVerifierSettings.TargetThousandsCustom
                ? numberVerifierSettings.TargetThousandsCustomSeparator
                : string.Empty;
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
            mock.Setup(r => r.GetAlphanumericCustomSeparator())
              .Returns(GetAlphanumericsCustomSeparator(mock.Object));
            mock.Setup(r => r.GetHindiNumber())
              .Returns(GetHindi(mock.Object));
        }

        public static Mock<INumberVerifierSettings> PreventLocalization()
        {
            var iNumberSettingsMock = Settings();
            iNumberSettingsMock.Setup(x => x.PreventLocalizations).Returns(true);

            return iNumberSettingsMock;
        }

        public static Mock<INumberVerifierSettings> RequireLocalization()
        {
            var iNumberSettingsMock = Settings();

            InitSeparators(iNumberSettingsMock);

            iNumberSettingsMock.Setup(x => x.RequireLocalizations).Returns(true);

            return iNumberSettingsMock;
        }

        private static Mock<INumberVerifierSettings> Settings()
        {
            var iNumberSettingsMock = new Mock<INumberVerifierSettings>(MockBehavior.Loose);
            iNumberSettingsMock.Setup(r => r.CheckInOrder).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportAddedNumbers).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportModifiedAlphanumerics).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportModifiedNumbers).Returns(true);
            iNumberSettingsMock.Setup(r => r.ReportRemovedNumbers).Returns(true);
            iNumberSettingsMock.Setup(r => r.CustomsSeparatorsAlphanumerics).Returns(true);

            return iNumberSettingsMock;
        }
    }
}