using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace WinFormControlLoading
{
    public partial class UserControl1 : UserControl, IUIControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }
    }
}
