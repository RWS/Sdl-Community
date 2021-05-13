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
				if (((string)value).Length > 0)
				{
					text = value.ToString();
				}
			}
			catch (Exception e)
			{
				return new ValidationResult(false, "Selection is empty.");
			}

			return text.IndexOfAny(Path.GetInvalidPathChars()) >= 0
				? new ValidationResult(false, "Please do not use invalid characters.")
				: ValidationResult.ValidResult;
		}
	}
}
