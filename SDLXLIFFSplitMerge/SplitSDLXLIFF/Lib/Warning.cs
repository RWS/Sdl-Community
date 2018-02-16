using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    public class Warning : IComparable
    {
        public enum WarningType { NotFound, WrongSplitLocation };

        public string ElementID
        {
            get;
            set;
        }

        public WarningType Type
        {
            get;
            set;
        }

        public Warning(string elementID, WarningType type)
        {
            ElementID = elementID;
            Type = type;
        }

        public string GetMessage()
        {
            switch (Type)
            {
                case WarningType.NotFound:
                    return string.Format(Properties.StringResource.wrnSegNotFound, ElementID);
                case WarningType.WrongSplitLocation:
                    return string.Format(Properties.StringResource.wrnSegWrongSplit, ElementID);
                default:
                    return "";
            }
        }

        public int CompareTo(object obj)
        {
            Warning objCompare = obj as Warning;
            return string.Compare(ElementID, objCompare.ElementID);
        }
    }
}
