﻿using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests.SourceSettings
{
	public class SourceSettingsAndPreventLocalization
    {
        public static Mock<INumberVerifierSettings> AllTypesOfSpacesChecked()
        {
            var numberVerifierSettings = ThousandsSeparatorsSpaceNoBreakThinSpace();
            numberVerifierSettings.Setup(t => t.SourceThousandsNobreakThinSpace).Returns(true);

            return numberVerifierSettings;
        }

        public static Mock<INumberVerifierSettings> SpaceCommaPeriod()
        {
            var numberVerifierSettings = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();
            numberVerifierSettings.Setup(t => t.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(t => t.SourceThousandsComma).Returns(true);
            numberVerifierSettings.Setup(t => t.SourceThousandsPeriod).Returns(true);

            return numberVerifierSettings;
        }

        public static Mock<INumberVerifierSettings> ThousandsSeparatorsSpaceAndNoBreak()
        {
            var numberVerifierSettings = Utilities.NumberVerifierLocalizationsSettings.PreventLocalization();
            numberVerifierSettings.Setup(t => t.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(t => t.SourceThousandsNobreakSpace).Returns(true);

            return numberVerifierSettings;
        }

        public static Mock<INumberVerifierSettings> ThousandsSeparatorsSpaceNoBreakThinSpace()
        {
            var numberVerifierSettings = ThousandsSeparatorsSpaceAndNoBreak();
            numberVerifierSettings.Setup(t => t.SourceThousandsThinSpace).Returns(true);

            return numberVerifierSettings;
        }
    }
}