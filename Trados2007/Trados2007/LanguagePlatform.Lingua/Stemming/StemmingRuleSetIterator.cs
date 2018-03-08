using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
    internal class StemmingRuleSetIterator
    {
        private StemmingRuleSet _Set;
        private int _Position;

        public StemmingRuleSetIterator(StemmingRuleSet set)
        {
            if (set == null)
                throw new ArgumentNullException("set");
            _Set = set;
            _Position = -1;
        }

        public StemmingRule Current
        {
            get { return _Position >= 0 && _Position < _Set.Count ? _Set[_Position] : null; }
        }

        public void First(int priority)
        {
            _Position = -1;
            Next(priority);
        }

        public void Next(int priority)
        {
            if (_Position >= _Set.Count)
                _Position = -1;
            else
            {
                ++_Position;
                if (priority > 0)
                {
                    while (_Position < _Set.Count && _Set[_Position].Priority < priority)
                        ++_Position;
                }
            }
        }

    }
}
