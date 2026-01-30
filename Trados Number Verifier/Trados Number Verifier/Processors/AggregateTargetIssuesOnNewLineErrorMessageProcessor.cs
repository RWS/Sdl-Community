using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
