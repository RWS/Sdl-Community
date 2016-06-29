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
        public static Mock<INumberVerifierSettings> AllowLocalization()
        {
            var iNumberSettingsMock = new Mock<INumberVerifierSettings>(MockBehavior.Loose);
            iNumberSettingsMock.Setup(x => x.AllowLocalizations).Returns(true);

            return iNumberSettingsMock;
            
        }

        public static Mock<INumberVerifierSettings> RequireLocalization()
        {
            var iNumberSettingsMock = new Mock<INumberVerifierSettings>(MockBehavior.Loose);
            iNumberSettingsMock.Setup(x => x.RequireLocalizations).Returns(true);

            return iNumberSettingsMock;

        }

        public static Mock<INumberVerifierSettings> PreventLocalization()
        {
            var iNumberSettingsMock = new Mock<INumberVerifierSettings>(MockBehavior.Loose);
            iNumberSettingsMock.Setup(x => x.PreventLocalizations).Returns(true);

            return iNumberSettingsMock;

        }
    }
}
