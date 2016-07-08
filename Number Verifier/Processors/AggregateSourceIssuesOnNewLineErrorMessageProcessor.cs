using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class AggregateSourceIssuesOnNewLineErrorMessageProcessor : IErrorMessageProcessor
    {
        public string GenerateMessage(INumberResults numberResult)
        {
            var result = new StringBuilder();
                foreach (var sourceNumber in numberResult.SourceNumbers)
                {
                    result.AppendLine(sourceNumber);
                }
            return result.ToString();
        }
    }
}
