using System.Collections.Generic;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Tests
{
	public class SettingsBuilder : ISettingsBuilder
    {
        private bool AllowLocalizations { get; set; }
        private bool PreventLocalizations { get; set; }

        private bool RequireLocalizations { get; set; }

        private bool SourceDecimalComma { get; set; }

        private string SourceDecimalCustom { get; set; }

        private bool SourceDecimalPeriod { get; set; }

        private bool SourceOmitLeadingZero { get; set; }

        private bool SourceThousandsComma { get; set; }

        private string SourceThousandsCustom { get; set; }

        private bool SourceThousandsPeriod { get; set; }

        private bool TargetDecimalComma { get; set; }

        private string TargetDecimalCustom { get; set; }

        private bool TargetDecimalPeriod { get; set; }

        private bool TargetOmitLeadingZero { get; set; }

        private bool TargetThousandsComma { get; set; }

        private string TargetThousandsCustom { get; set; }

        private bool TargetThousandsPeriod { get; set; }

        private bool CheckInOrder { get; set; }

        public ISettingsBuilder AllowLocalization()
        {
            AllowLocalizations = true;
            RequireLocalizations = false;
            PreventLocalizations = false;
            return this;
        }

        public Mock<INumberVerifierSettings> Build()
        {
            var settingsMock = new Mock<INumberVerifierSettings>();

			if (CheckInOrder) settingsMock.Setup(sm => sm.CheckInOrder).Returns(true);

            if (AllowLocalizations) settingsMock.Setup(sm => sm.AllowLocalizations).Returns(true);
            if (PreventLocalizations) settingsMock.Setup(sm => sm.PreventLocalizations).Returns(true);
            if (RequireLocalizations) settingsMock.Setup(sm => sm.RequireLocalizations).Returns(true);

            if (SourceOmitLeadingZero) settingsMock.Setup(sm => sm.SourceOmitLeadingZero).Returns(true);
            if (TargetOmitLeadingZero) settingsMock.Setup(sm => sm.TargetOmitLeadingZero).Returns(true);

            var sourceThousandSeparators = new List<string>();
            if (SourceThousandsComma) sourceThousandSeparators.Add(",");
            if (SourceThousandsPeriod) sourceThousandSeparators.Add(".");
            if (SourceThousandsCustom is not null) sourceThousandSeparators.Add(SourceThousandsCustom);

            var targetThousandSeparators = new List<string>();
            if (TargetThousandsComma) targetThousandSeparators.Add(",");
            if (TargetThousandsPeriod) targetThousandSeparators.Add(".");
            if (TargetThousandsCustom is not null) targetThousandSeparators.Add(TargetThousandsCustom);

            var sourceDecimalSeparators = new List<string>();
            if (SourceDecimalComma) sourceDecimalSeparators.Add(",");
            if (SourceDecimalPeriod) sourceDecimalSeparators.Add(".");
            if (SourceDecimalCustom is not null) sourceDecimalSeparators.Add(SourceDecimalCustom);

            var targetDecimalSeparators = new List<string>();
            if (TargetDecimalComma) targetDecimalSeparators.Add(",");
            if (TargetDecimalPeriod) targetDecimalSeparators.Add(".");
            if (TargetDecimalCustom is not null) targetDecimalSeparators.Add(TargetDecimalCustom);

            settingsMock.Setup(r => r.ReportAddedNumbers).Returns(true);
            settingsMock.Setup(r => r.ReportModifiedAlphanumerics).Returns(true);
            settingsMock.Setup(r => r.ReportModifiedNumbers).Returns(true);
            settingsMock.Setup(r => r.ReportRemovedNumbers).Returns(true);
            settingsMock.Setup(r => r.CustomsSeparatorsAlphanumerics).Returns(true);

            settingsMock.Setup(sm => sm.GetSourceThousandSeparators())
                .Returns(sourceThousandSeparators);

            settingsMock.Setup(sm => sm.GetTargetThousandSeparators())
                .Returns(targetThousandSeparators);

            settingsMock.Setup(sm => sm.GetSourceDecimalSeparators())
                .Returns(sourceDecimalSeparators);

            settingsMock.Setup(sm => sm.GetTargetDecimalSeparators())
                .Returns(targetDecimalSeparators);

	        ResetFields();

            return settingsMock;
        }

		public ISettingsBuilder ConsiderOrderOfNumbers()
		{
			CheckInOrder = true;
			return this;
		}

		private void ResetFields()
		{
			var properties =
				GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(bool)) property.SetValue(this, false);
				if (property.PropertyType == typeof(string)) property.SetValue(this, null);
			}
		}

        public ISettingsBuilder OmitLeadingZeroInSource()
        {
            SourceOmitLeadingZero = true;
            return this;
        }

        public ISettingsBuilder OmitLeadingZeroInTarget()
        {
            TargetOmitLeadingZero = true;
            return this;
        }

        public ISettingsBuilder PreventLocalization()
        {
            AllowLocalizations = false;
            RequireLocalizations = false;
            PreventLocalizations = true;

            return this;
        }

        public ISettingsBuilder RequireLocalization()
        {
            AllowLocalizations = false;
            RequireLocalizations = true;
            PreventLocalizations = false;
            return this;
        }

        public ISettingsBuilder WithSourceDecimalSeparators(bool comma, bool period, string custom = null)
        {
            SourceDecimalComma = comma;
            SourceDecimalPeriod = period;
            SourceDecimalCustom = custom;

            return this;
        }

        public ISettingsBuilder WithSourceThousandSeparators(bool comma, bool period, string custom = null)
        {
            SourceThousandsComma = comma;
            SourceThousandsPeriod = period;
            SourceThousandsCustom = custom;

            return this;
        }

        public ISettingsBuilder WithTargetDecimalSeparators(bool comma, bool period, string custom = null)
        {
            TargetDecimalComma = comma;
            TargetDecimalPeriod = period;
            TargetDecimalCustom = custom;

            return this;
        }

        public ISettingsBuilder WithTargetThousandSeparators(bool comma, bool period, string custom = null)
        {
            TargetThousandsComma = comma;
            TargetThousandsPeriod = period;
            TargetThousandsCustom = custom;

            return this;
        }
    }
}