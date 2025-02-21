using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.XliffToLegacyConverter
{
    public partial class FormSettings : Form
    {
        public bool IsSaved;
        public FormSettings()
        {
            InitializeComponent();
        
        }

        private void LoadListViewRows()
        {
            var label = new Label();
            var checkBox = new CheckBox();

            var lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Match Type - Perfect Match:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Navy
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 0);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 0);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 0);



            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Match Type - Context Match:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Navy
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 1);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 1);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 1);


            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Match Type - Exact Match:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Navy
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 2);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 2);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 2);



            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Match Type - Fuzzy Match:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Navy
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 3);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 3);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 3);




            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Match Type - No Match:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Navy
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 4);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 4);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 4);



            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Locked Segment:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.DarkViolet
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 5);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 5);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 5);




            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Status - Not Translated:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 6);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 6);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 6);


            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Status - Draft:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 7);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 7);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 7);



            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Status - Translated:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 8);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 8);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 8);


            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = "Status - Translation Rejected:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 9);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 9);

            checkBox = new CheckBox();
            checkBox.Checked = false;
            checkBox.CheckAlign = ContentAlignment.MiddleCenter;
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 9);


            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Status - Translation Approved:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 10);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 10);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 10);


            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Status - Sign-off Rejected:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 11);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 11);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 11);


            lvi = listViewImportExport.Items.Add(string.Empty);
            label = new Label
            {
                Text = @"Status - Signed Off:",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Brown
            };
            listViewImportExport.AddEmbeddedControl(label, 0, 12);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 1, 12);

            checkBox = new CheckBox
            {
                Checked = false,
                CheckAlign = ContentAlignment.MiddleCenter
            };
            listViewImportExport.AddEmbeddedControl(checkBox, 2, 12);


         
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            IsSaved = true;
            Close();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            treeView_main.ExpandAll();
            treeView_main.Select();
            treeView_main.SelectedNode = treeView_main.Nodes[0];

        
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void treeView_main_AfterSelect(object sender, TreeViewEventArgs e)
        {

            switch (e.Node.Name)
            {
                case "Node_general":
                    {
                        panel_general.BringToFront();
                    } break;
                case "Node_filterSettings":
                    {
                        panel_FilterSettings.BringToFront();
                    } break;
                case "Node_segmentStatusAssignment":
                    {
                        panel_SegmentStatusAssignment.BringToFront();
                    } break;
                case "Node_errorHandling":
                    {
                        panel_ErrorHandling.BringToFront();
                    } break;
            
            }
        }

      
    
    }
}
