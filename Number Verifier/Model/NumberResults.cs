using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Model
{
    public class NumberResults : INumberResults
    {
        private INumberVerifierSettings _settings;
        private List<string> _sourceNumbers;
        private List<string> _targetNumbers;
        private string _targetText;

        public NumberResults(INumberVerifierSettings settings, 
            List<string> sourceNumbers,
            List<string> targetNumbers)
        {
            _settings = settings;
            _sourceNumbers = sourceNumbers;
            _targetNumbers = targetNumbers;
        }

        public NumberResults(INumberVerifierSettings settings, List<string> sourceNumbers, List<string> targetNumbers,string sourceText,string targetText) 
            :this(settings, sourceNumbers,targetNumbers)
        {
            SourceText = sourceText;
            _targetText = targetText;
        }

        public INumberVerifierSettings Settings
        {
            get
            {
                return _settings;
            }

            set
            {
                _settings = value;
            }
        }

        public List<string> SourceNumbers
        {
            get
            {
                return _sourceNumbers;
            }

            set
            {
                _sourceNumbers = value;
            }
        }

        public List<string> TargetNumbers
        {
            get
            {
                return _targetNumbers;
            }

            set
            {
                _targetNumbers = value;
            }
        }

        public string SourceText { get; set; }

        public string TargetText { get; set; }
    }
}
