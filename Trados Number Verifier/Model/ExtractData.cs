using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Model
{
    public class ExtractData:IExtractData
    {
        private INumberVerifierSettings _settings;
        private IEnumerable<string> _extractList;


        public ExtractData(INumberVerifierSettings settings,IEnumerable<string>extractList)
        {
            _settings = settings;
            _extractList = extractList;
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

        public IEnumerable<string> ExtractList
        {
            get
            {
                return _extractList;
            }

            set
            {
                _extractList = value;
            }
        }

    }
}
