using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.TMOptimizer
{
    public interface IWizardPageControl
    {

        bool Next();

        bool Previous();

        void Help();

        void Finish();

        void Cancel();
    }
}
