using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.XLIFF.Manager.Model
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
