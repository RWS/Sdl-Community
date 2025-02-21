using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeFeedbackError
	{
		public EdgeFeedbackErrorDetail Error { get; set; }
	}

	public class EdgeFeedbackErrorDetail
	{
		public int Code { get; set; }

		public string Message { get; set; }

		public string Details { get; set; }
	}
}