using System;

namespace Trados.Transcreate.Model
{
	public class WizardException : Exception
	{
		public string Description { get; set; }

		public WizardException()
		{
		}

		public WizardException(string message, string description)
			: base(message)
		{
			Description = description;
		}

		public WizardException(string message, string description, Exception inner)
			: base(message, inner)
		{
			Description = description;
		}
	}
}
