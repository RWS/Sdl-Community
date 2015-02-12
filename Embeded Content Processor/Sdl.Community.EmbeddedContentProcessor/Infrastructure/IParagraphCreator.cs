using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.EmbeddedContentProcessor.Infrastructure
{
    public interface IParagraphCreator
    {
      IParagraph GetParagraph();
    }
}
