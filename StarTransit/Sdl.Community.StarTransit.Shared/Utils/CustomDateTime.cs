using System;

namespace Sdl.Community.StarTransit.Shared.Utils
{
	public static class CustomDateTime
	{
		public static string CreateCustomDate(DateTime dateTime)
		{
			var date = CustomExchangeDate(dateTime);
			var time = CustomExchangeTime(dateTime);
			var customDateTime = string.Concat(date, "T", time, "Z");
			return customDateTime;
		}

		public static string CustomExchangeDate(DateTime dateTime)
		{
			var date = dateTime.Year + dateTime.Month.ToString() + dateTime.Day;
			return date;
		}

		public static string CustomExchangeTime(DateTime dateTime)
		{
			var time = dateTime.Hour.ToString() + dateTime.Minute + dateTime.Second;
			return time;
		}
	}
}