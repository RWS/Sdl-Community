using System.Collections.Generic;

namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{

    public class TagUnitWarning
    {
        public enum WarningType
        {
            Added = 0,
            Removed,
            Placement
        }


        public string WarningMessage { get; set; }
        public WarningType TypeOfWarning { get; set; }
        public List<TagUnit> TagUnits { get; set; }



        public TagUnitWarning(WarningType typeOfWarning, string warningMessage, List<TagUnit> tagUnits)
        {
            TypeOfWarning = typeOfWarning;
            WarningMessage = warningMessage;
            TagUnits = tagUnits;
        }
    }
}
