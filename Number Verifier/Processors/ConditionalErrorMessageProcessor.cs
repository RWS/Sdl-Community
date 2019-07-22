using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors
{
	public class ConditionalErrorMessageProcessor : IErrorMessageProcessor
    {
        public IVerifySpecification Specification;

        public IErrorMessageProcessor TruthProcessor;
        public string GenerateMessage(INumberResults numberResult, string errorMessage)
        {
            if(this.Specification.IsSatisfiedBy(numberResult))
            {
                return TruthProcessor.GenerateMessage(numberResult, errorMessage);
            }
            return string.Empty;
        }
    }
}
