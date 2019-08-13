using System;
using System.Windows.Forms;

namespace Sdl.Community.ExcelTerminology.Ui
{
	public delegate void TabPressed(object source, TabPressedEventArgs e);

    public class CustomTabTextBox: TextBox
    {
        public event TabPressed OnTabPressed;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                    if (OnTabPressed != null)
                    {
                        var eArgs = new TabPressedEventArgs {Handled = false};
                        OnTabPressed(this, eArgs);
                        if(eArgs.Handled) return true;
                        return base.ProcessCmdKey(ref msg, keyData);

                    }
                    return base.ProcessCmdKey(ref msg, keyData);
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }

    public class TabPressedEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}