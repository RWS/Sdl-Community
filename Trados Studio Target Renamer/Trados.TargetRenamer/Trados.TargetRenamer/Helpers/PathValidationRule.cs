using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
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
            catch (Exception)
			{
                return new ValidationResult(false, PluginResources.EmptyPath);
            }

			try
			{
				Path.GetDirectoryName(path);
			}
            catch (Exception)
			{
                return new ValidationResult(false, PluginResources.InvalidPath);
            }

	        var folderPathRegEx =
		        new Regex(@"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w ]*))+\\?");

			var match = folderPathRegEx.Match(path);

	        if (!match.Success || match.Length < path.Length)
		        return new ValidationResult(false, PluginResources.InvalidPathChars);

	        return ValidationResult.ValidResult;
        }
    }
}