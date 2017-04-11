using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.TermInjector
{
    public partial class ValidationErrorForm : Form
    {
        public ValidationErrorForm(List<KeyValuePair<string,string>> errors)
        {
            InitializeComponent();
            object[] columns = new object[2];
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            foreach (var error in errors)
            {
                columns[0] = error.Key;
                columns[1] = error.Value;
                rows.Add(new DataGridViewRow());
                rows[rows.Count - 1].CreateCells(dataGridView1,columns);
                
            }
            dataGridView1.Rows.AddRange(rows.ToArray());
            
            this.Update();
            this.ShowDialog();
        }

        private void ValidationErrorForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}