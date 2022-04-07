using System.Windows.Forms;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
    public partial class FilterExpression : UserControl
    {
        public FilterExpression()
        {
            InitializeComponent();
        }

        public void AddItem(string item)
        {
            label_text.Text += (label_text.Text.Trim() != string.Empty ? "; " : string.Empty) + item;
        }

        public void ClearItems()
        {
            label_text.Text = string.Empty;
        }
        
    }
}
