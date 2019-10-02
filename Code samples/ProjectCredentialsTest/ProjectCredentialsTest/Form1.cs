using System;
using System.Windows.Forms;

namespace ProjectCredentialsTest
{
	public partial class Form1 : Form
	{
		public string Url { get; set; }
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Url = textBox1.Text;
			
			Close();
		}
	}
}
