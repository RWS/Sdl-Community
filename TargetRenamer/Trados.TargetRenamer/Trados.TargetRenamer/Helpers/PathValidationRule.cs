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
				if (((string) value).Length > 0)
				{
					path = value.ToString();
				}
			}
			catch (Exception e)
			{
				return new ValidationResult(false, "Path is empty.");
			}

			try
			{
				Path.GetDirectoryName(path);
			}
			catch (Exception e)
			{
				return new ValidationResult(false, "Path is not valid.");
			}

			if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
				return new ValidationResult(false, "Path contains invalid characters.");

			return ValidationResult.ValidResult;
		}
	}
}
