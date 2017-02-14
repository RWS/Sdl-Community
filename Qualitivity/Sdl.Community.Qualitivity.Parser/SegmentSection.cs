using System;
using System.Xml.Serialization;

namespace Sdl.Community.Parser
{

    [XmlInclude(typeof(Tag))]
    [XmlInclude(typeof(Text))]
    [Serializable]
    public abstract class SegmentSection
    {
    }


}
