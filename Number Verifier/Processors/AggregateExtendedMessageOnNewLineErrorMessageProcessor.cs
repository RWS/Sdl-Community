using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class AggregateExtendedMessageOnNewLineErrorMessageProcessor:IErrorMessageProcessor
    {
        public string GenerateMessage(INumberResults numberResult)
        {
            var result = new StringBuilder();

            result.AppendLine("Source:");
            foreach (var source in numberResult.SourceNumbers)
            {
                result.AppendLine(source);
            }

            result.AppendLine("Target:");
            foreach (var target in numberResult.TargetNumbers)
            {
                result.AppendLine(target);
            }
            

            return result.ToString();
        }
    }
}
