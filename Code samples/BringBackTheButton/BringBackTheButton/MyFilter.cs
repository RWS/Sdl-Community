using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.BringBackTheButton
{
    public class MyFilter: IMessageFilter
    {
        private const int WM_KEYDOWN = 0x100;

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    switch (m.WParam.ToInt32())
                    {
                        case (int)Keys.Escape:
                           
                            break;
                    }
                    break;

            }

            return false; // returning false allows messages to be processed normally
        }
    }
}
