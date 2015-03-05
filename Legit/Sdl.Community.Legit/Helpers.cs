using System;
using System.Runtime.InteropServices;

namespace Sdl.Community.Legit
{
    public class Helpers
    {
        public static T CreateComObject<T>(object comObject)
        {
            ReleaseComObject(comObject);
            // comment1
            return Activator.CreateInstance<T>();
        }

        public static void ReleaseComObject(object comObject)
        {
            if (comObject != null)
            {
                Marshal.ReleaseComObject(comObject); //conflict 2
                //comment
            }
        }
    }
}