using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests.Utilities
{
    public class NumberVerifierRequireLocalizationSettings
    {
        public static Mock<INumberVerifierSettings> ThousandsSeparatorsSpaceAndNoBreak()
        {
            var numberVerifierSettings =NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsNobreakSpace).Returns(true);

            return numberVerifierSettings;
        }

        public static Mock<INumberVerifierSettings> ThousandsSeparatorsSpaceNoBreakThinSpace()
        {
            var numberVerifierSettings = ThousandsSeparatorsSpaceAndNoBreak();
            numberVerifierSettings.Setup(t => t.TargetThousandsThinSpace).Returns(true);

            return numberVerifierSettings;
        }

        public static Mock<INumberVerifierSettings> AllTypesOfSpacesChecked()
        {
            var numberVerifierSettings = ThousandsSeparatorsSpaceNoBreakThinSpace();
            numberVerifierSettings.Setup(t => t.TargetThousandsNobreakThinSpace).Returns(true);

            return numberVerifierSettings;

        }

        public static Mock<INumberVerifierSettings> SpaceCommaPeriod()
        {
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsPeriod).Returns(true);

            return numberVerifierSettings;

        }

        public static Mock<INumberVerifierSettings> CommaPeriod()
        {
            var numberVerifierSettings = NumberVerifierLocalizationsSettings.RequireLocalization();
            numberVerifierSettings.Setup(t => t.TargetThousandsComma).Returns(true);
            numberVerifierSettings.Setup(t => t.TargetThousandsPeriod).Returns(true);

            return numberVerifierSettings;

        }
    }
}
