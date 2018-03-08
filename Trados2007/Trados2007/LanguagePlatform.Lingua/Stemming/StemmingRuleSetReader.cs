using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
    public class StemmingRuleSetReader : IDisposable
    {
        private System.IO.TextReader _Reader;
        private bool _OwnStream;

        public StemmingRuleSetReader(System.IO.TextReader reader)
        {
            _Reader = reader;
            _OwnStream = false;
        }

        public StemmingRuleSetReader(System.IO.Stream s)
        {
            _Reader = new System.IO.StreamReader(s);
            // NOTE do we really own the stream here? Should be safe.
            _OwnStream = true;
        }

        public StemmingRuleSetReader(string path)
        {
            _Reader = new System.IO.StreamReader(path, System.Text.Encoding.UTF8, true);
            _OwnStream = false;
        }

        public StemmingRuleSet Read(System.Globalization.CultureInfo culture)
        {
            string line;
            StemmingRuleSet result = new StemmingRuleSet(culture);
            StemmingRuleParser parser = new StemmingRuleParser(result);

            while ((line = _Reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("#") || line.Length == 0)
                    continue;

                parser.Add(line);
            }

            Close();

            return result;
        }

        private void Close()
        {
            if (_OwnStream && _Reader != null)
            {
                _Reader.Close();
                _Reader.Dispose();
            }
            _Reader = null;
            _OwnStream = false;
        }

        public void Dispose()
        {
            Close();
        }

        public static StemmingRuleSet Read(string path, System.Globalization.CultureInfo culture)
        {
            using (StemmingRuleSetReader rdr = new StemmingRuleSetReader(path))
            {
                return rdr.Read(culture);
            }
        }
    }
}
