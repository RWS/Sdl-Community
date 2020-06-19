using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Common
{
	public class Enumerators
	{
		public enum Action
		{
			None = 0,
			Export = 1,
			Import = 2
		}

		public enum Status
		{
			None = 0,
			Ready = 1,
			Success = 2,
			Error = 3,
			Warning
		}

		public enum XLIFFSupport
		{
			none = 0,
			xliff12sdl = 1,
			xliff12polyglot =2
			// TODO spport for this format will come later on in the development cycle
			//xliff20sdl = 2
		}

		public static List<ConfirmationStatus> GetConfirmationStatuses()
		{
			var confirmationStatuses = new List<ConfirmationStatus>
			{
				new ConfirmationStatus
				{
					Id = string.Empty,
					Name = "Don't Change"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.Unspecified.ToString(),
					Name = "Unspecified"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.Draft.ToString(),
					Name = "Draft"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.Translated.ToString(),
					Name = "Translated"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.RejectedTranslation.ToString(),
					Name = "Translation Rejected"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.ApprovedTranslation.ToString(),
					Name = "Translation Approved"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.RejectedSignOff.ToString(),
					Name = "SignOff Rejected"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.ApprovedSignOff.ToString(),
					Name = "SignOff Approved"
				}
			};

			return confirmationStatuses;
		}


	}
}
