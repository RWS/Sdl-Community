
namespace Sdl.Community.FragmentAlignmentAutomation.Controls
{
    public class DoubleBufferedListView : System.Windows.Forms.ListView
    {
        public DoubleBufferedListView()
            : base()
        {
            this.DoubleBuffered = true;
        }

        protected sealed override bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }
    }
}
