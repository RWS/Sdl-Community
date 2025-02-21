using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ProjectTerms.Controls.Interfaces
{
    public interface IProgressIndicator
    {
        int Maximum { get; set; }
        void Increment(int value);
    }
}
