using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Trados.TargetRenamer.Helpers
{
	public class FileNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = string.Empty;
			try
			{
                if (((string) value).Length > 0)
                {
                    text = value.ToString();
                }
            }
            catch (Exception)
			{
                return new ValidationResult(false, PluginResources.SelectionEmpty);
            }

            return text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                ? new ValidationResult(false, PluginResources.InvalidChars)
                : ValidationResult.ValidResult;
        }
    }
}