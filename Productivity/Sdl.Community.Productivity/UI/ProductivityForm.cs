using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services;


namespace Sdl.Community.Productivity.UI
{
    public partial class ProductivityForm : Form
    {
        public ProductivityForm(ProductivityService productivityService, TwitterShareService twitterShare)
        {
            InitializeComponent();
            productivityControl.Initialize(productivityService, twitterShare);
        }

    }
}
