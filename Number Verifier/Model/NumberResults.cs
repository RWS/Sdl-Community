using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Model
{
	public class NumberResults : INumberResults
    {
        public NumberResults(NumberModel numberModel)
        {
            SourceText = numberModel.SourceText;
			TargetText = numberModel.TargetText;
			InitialSourceNumbers = numberModel.InitialSourceNumbers;
			InitialTargetNumbers = numberModel.InitialTargetNumbers;
			IsHindiVerification = numberModel.IsHindiVerification;
			SourceNumbers = numberModel.SourceNumbers;
			TargetNumbers = numberModel.TargetNumbers;
			Settings = numberModel.Settings;
		}

		public INumberVerifierSettings Settings { get; set; }

        public List<string> SourceNumbers { get; set; }

        public List<string> TargetNumbers { get; set; }

		public List<string> InitialSourceNumbers { get; set; }

        public List<string> InitialTargetNumbers { get; set; }

        public string SourceText { get; set; }

        public string TargetText { get; set; }

		public bool IsHindiVerification { get; set; }
	}
}