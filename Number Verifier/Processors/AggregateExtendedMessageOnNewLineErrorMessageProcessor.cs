using System.Text;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors
{
	public class AggregateExtendedMessageOnNewLineErrorMessageProcessor : IErrorMessageProcessor
	{
		public string GenerateMessage(INumberResults numberResult, string errorMessage)
		{
			char[] charsToRemove = { ' ', ',' };
			var result = new StringBuilder();

			if (numberResult.SourceNumbers.Count > 0)
			{
				result.AppendFormat("Source issue(s): ");
				foreach (var source in numberResult.SourceNumbers)
				{
					result.AppendFormat($"{source}, ");
				}
				// remove the last 2 chars from the last word attached to string, which represents the ',' and empty space
				result = result.Remove(result.Length - 2, 2);
				result.AppendFormat("; ");
			}
			if (numberResult.TargetNumbers.Count > 0)
			{
				result.AppendFormat("Target issue(s): ");
				foreach (var target in numberResult.TargetNumbers)
				{
					result.AppendFormat($"{target}, ");
				}
			}
			return result.ToString().TrimEnd(charsToRemove);
		}
	}
}