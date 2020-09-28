namespace Sdl.Community.MTCloud.Provider.Model
{
	public class FeedbackSendingStatus
	{
		private Status _status;
		public string Message { get; private set; }

		public Status Status
		{
			get => _status;
			set
			{
				_status = value;
				switch (value)
				{
					default:
						Message = "";
						return;

					case Status.Sent:
						Message = "Feedback sent";
						return;

					case Status.NotSent:
						Message = "Feedback was not sent";
						return;
				}
			}
		}
	}
}