using Sdl.Community.NumberVerifier.DTOs;
using Sdl.Community.NumberVerifier.Extensions;
using Sdl.Community.NumberVerifier.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Sdl.Community.NumberVerifier.GUI
{
    public partial class NumberVerifierProfile : UserControl
    {
        private readonly FilePathDialogService _filePathDialogService;
        private NumberVerifierSettings _settings;

        public NumberVerifierSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }

        public NumberVerifierProfile(
            FilePathDialogService filePathDialogService,
            NumberVerifierSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            _filePathDialogService = filePathDialogService;
            if (ProfileHasChanged()) _settings.ProfilePath = null;
            SetCurrentProfileLabel();
        }


        private void button1_ImportSettings_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(
                "Importing settings will overwrite existing configurations. Do you want to continue?",
                "Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
                ) == DialogResult.No) return;

            var files = _filePathDialogService.GetFilePathInputFromUser(initialDirectory: "", filter: "Settings XML File | *.xml");
            if (files is null || !files.Any()) return;

            try
            {
                var filePath = files.First();
                NumberVerifierSettingsDTO settingsDto = null;

                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(NumberVerifierSettingsDTO));
                    settingsDto = serializer.Deserialize(reader) as NumberVerifierSettingsDTO;
                }

                if (settingsDto == null)
                {
                    return;
                }

                settingsDto.OverwriteNumberVerifierSettings(_settings);
                _settings.ProfilePath = filePath;

                SetCurrentProfileLabel();
                _settings.EndEdit();

                MessageBox.Show("Number Verifier QA profile imported successfully", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failure");
            }
        }

        private void button2_ExportSettings_Click(object sender, System.EventArgs e)
        {
            var fileName = "My Settings";
            // We might need to get the project location as a start for settings
            if (_settings.ProfilePath != null && File.Exists(_settings.ProfilePath))
               fileName = Path.GetFileName(_settings.ProfilePath);
            var saveLocation = GetSaveLocation("Export Settings", "Settings XML File | *.xml", fileName);
            if (string.IsNullOrEmpty(saveLocation)) return;
            if (_settings is null) return;
            var settings = _settings.ToSettingsDTO();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NumberVerifierSettingsDTO));
                using (StreamWriter writer = new StreamWriter(saveLocation))
                {
                    serializer.Serialize(writer, settings);
                }

                MessageBox.Show("Number Verifier QA profile exported successfully", "Success");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failure");
            }
        }

        private string GetSaveLocation(string title, string extension, string fileName)
        {
            var saveLocation = "";
            try
            {
                _filePathDialogService.GetSaveLocationInputFromUser(out saveLocation,
                    title, extension, fileName);
            }
            catch (Exception e)
            {
                // Do nothing
            }

            return saveLocation;
        }

        private bool ProfileHasChanged()
        {
            if (string.IsNullOrEmpty(_settings.ProfilePath)) return false;

            NumberVerifierSettingsDTO settingsDto = null;
            try
            {
                var filePath = _settings.ProfilePath;

                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(NumberVerifierSettingsDTO));
                    settingsDto = serializer.Deserialize(reader) as NumberVerifierSettingsDTO;
                }

                if (settingsDto == null)
                {
                    return true;
                }

                return !SettingsAreEqual(settingsDto, _settings);
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private bool SettingsAreEqual(NumberVerifierSettingsDTO a, NumberVerifierSettings b)
        {
            if (a == null || b == null) return false;

            return a.CheckInOrder == b.CheckInOrder &&
                   a.ExcludeTagText == b.ExcludeTagText &&
                   a.ReportAddedNumbers == b.ReportAddedNumbers &&
                   a.ReportRemovedNumbers == b.ReportRemovedNumbers &&
                   a.ReportModifiedNumbers == b.ReportModifiedNumbers &&
                   a.ReportModifiedAlphanumerics == b.ReportModifiedAlphanumerics &&
                   a.ReportNumberFormatErrors == b.ReportNumberFormatErrors &&
                   a.CustomsSeparatorsAlphanumerics == b.CustomsSeparatorsAlphanumerics &&
                   a.HindiNumberVerification == b.HindiNumberVerification &&
                   a.AddedNumbersErrorType == b.AddedNumbersErrorType &&
                   a.RemovedNumbersErrorType == b.RemovedNumbersErrorType &&
                   a.ModifiedNumbersErrorType == b.ModifiedNumbersErrorType &&
                   a.ModifiedAlphanumericsErrorType == b.ModifiedAlphanumericsErrorType &&
                   a.NumberFormatErrorType == b.NumberFormatErrorType &&
                   a.ReportBriefMessages == b.ReportBriefMessages &&
                   a.ReportExtendedMessages == b.ReportExtendedMessages &&
                   a.AllowLocalizations == b.AllowLocalizations &&
                   a.PreventLocalizations == b.PreventLocalizations &&
                   a.RequireLocalizations == b.RequireLocalizations &&
                   a.SourceThousandsSpace == b.SourceThousandsSpace &&
                   a.SourceThousandsNobreakSpace == b.SourceThousandsNobreakSpace &&
                   a.SourceThousandsThinSpace == b.SourceThousandsThinSpace &&
                   a.SourceThousandsNobreakThinSpace == b.SourceThousandsNobreakThinSpace &&
                   a.SourceThousandsComma == b.SourceThousandsComma &&
                   a.SourceThousandsPeriod == b.SourceThousandsPeriod &&
                   a.SourceNoSeparator == b.SourceNoSeparator &&
                   a.TargetThousandsSpace == b.TargetThousandsSpace &&
                   a.TargetThousandsNobreakSpace == b.TargetThousandsNobreakSpace &&
                   a.TargetThousandsThinSpace == b.TargetThousandsThinSpace &&
                   a.TargetThousandsNobreakThinSpace == b.TargetThousandsNobreakThinSpace &&
                   a.TargetThousandsComma == b.TargetThousandsComma &&
                   a.TargetThousandsPeriod == b.TargetThousandsPeriod &&
                   a.TargetNoSeparator == b.TargetNoSeparator &&
                   a.SourceDecimalComma == b.SourceDecimalComma &&
                   a.SourceDecimalPeriod == b.SourceDecimalPeriod &&
                   a.TargetDecimalComma == b.TargetDecimalComma &&
                   a.TargetDecimalPeriod == b.TargetDecimalPeriod &&
                   a.ExcludeLockedSegments == b.ExcludeLockedSegments &&
                   a.Exclude100Percents == b.Exclude100Percents &&
                   a.ExcludeUntranslatedSegments == b.ExcludeUntranslatedSegments &&
                   a.ExcludeDraftSegments == b.ExcludeDraftSegments &&
                   a.SourceOmitLeadingZero == b.SourceOmitLeadingZero &&
                   a.TargetOmitLeadingZero == b.TargetOmitLeadingZero &&
                   a.SourceThousandsCustom == b.SourceThousandsCustom &&
                   a.TargetThousandsCustom == b.TargetThousandsCustom &&
                   a.SourceDecimalCustom == b.SourceDecimalCustom &&
                   a.TargetDecimalCustom == b.TargetDecimalCustom &&
                   a.SourceThousandsCustomSeparator == b.SourceThousandsCustomSeparator &&
                   a.TargetThousandsCustomSeparator == b.TargetThousandsCustomSeparator &&
                   a.SourceDecimalCustomSeparator == b.SourceDecimalCustomSeparator &&
                   a.TargetDecimalCustomSeparator == b.TargetDecimalCustomSeparator &&
                   a.AlphanumericsCustomSeparator == b.AlphanumericsCustomSeparator &&
                   a.HindiNumber == b.HindiNumber &&
                   ListsAreEqual(a.RegexExclusionList, b.RegexExclusionList) &&
                   ListsAreEqual(a.SourceExcludedRanges, b.SourceExcludedRanges) &&
                   ListsAreEqual(a.TargetExcludedRanges, b.TargetExcludedRanges);
        }

        private bool ListsAreEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null) list1 = new List<T>();
            if (list2 == null) list2 = new List<T>();

            if (list1.Count != list2.Count) return false;

            return list1.SequenceEqual(list2);
        }

        private void SetCurrentProfileLabel()
        {
            if (string.IsNullOrEmpty(_settings.ProfilePath)) return;

            labelProfilePath.Text = _settings.ProfilePath;
            labelProfilePath.Location = new System.Drawing.Point(
                labelCurrentProfile.Location.X + labelCurrentProfile.Width + 3,
                labelCurrentProfile.Location.Y
            );

            labelProfilePath.Visible = true;
            labelCurrentProfile.Visible = true;
        }

    }
}
