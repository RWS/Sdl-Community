using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMProvider
{
    public class Language
    {
        public string ThreeLetterCodeFull { get; private set; }
        public string Region { get; private set; }

        public Language(string threeLetterCodeFull)
        {
            if(threeLetterCodeFull.Length != 3 && threeLetterCodeFull.Length != 6)
                throw new ArgumentException("A language code is not in the right format.");
            this.ThreeLetterCodeFull = threeLetterCodeFull;
            if (threeLetterCodeFull.Length > 3) this.Region = threeLetterCodeFull.Substring(4, 2);
            else this.Region = "";
        }

    }
}
