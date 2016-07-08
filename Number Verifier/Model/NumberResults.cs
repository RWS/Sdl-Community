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
        public NumberResults(INumberVerifierSettings settings, 
            List<string> sourceNumbers,
            List<string> targetNumbers)
        {
            _settings = settings;
            _sourceNumbers = sourceNumbers;
            _targetNumbers = targetNumbers;
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
    }
}
