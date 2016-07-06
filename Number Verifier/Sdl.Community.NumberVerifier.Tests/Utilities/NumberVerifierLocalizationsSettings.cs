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
