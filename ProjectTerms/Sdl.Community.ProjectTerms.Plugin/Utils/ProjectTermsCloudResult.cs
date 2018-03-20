using System;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Controls.Interfaces;

namespace Sdl.Community.ProjectTerms.Plugin.Utils
{
    public class ProjectTermsCloudResult
    {
        public IEnumerable<ITerm> WeightedTerms { get; set; }

        public Exception Exception { get; set; }
    }
}
