using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IATETerminologyProvider.Ui
{
	public partial class SearchResultsControl : UserControl
	{
		public SearchResultsControl()
		{
			InitializeComponent();
		}

		public WebBrowser Browser => searchResultsWebBrowser;
	}
}
