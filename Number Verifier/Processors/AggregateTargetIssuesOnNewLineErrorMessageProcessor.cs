using System.Text;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors
{
	public class AggregateTargetIssuesOnNewLineErrorMessageProcessor : IErrorMessageProcessor
    {
        public string GenerateMessage(INumberResults numberResult, string errorMessage)
        {
            var result = new StringBuilder();
            foreach (var sourceNumber in numberResult.TargetNumbers)
            {
                result.AppendLine(sourceNumber);
            }
            return result.ToString();
        }
    }
}