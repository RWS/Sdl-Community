using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Trados.TargetRenamer.Helpers
{
	public class PathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var path = string.Empty;
            try
            {
                if (((string)value).Length > 0)
                {
                    path = value.ToString();
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, PluginResources.EmptyPath);
            }

            try
            {
                Path.GetDirectoryName(path);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, PluginResources.InvalidPath);
            }

            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult(false, PluginResources.InvalidPathChars);

            return ValidationResult.ValidResult;
        }
    }
}