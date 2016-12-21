using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDLXLIFFSliceOrChange
{
    public partial class SearchResults : Form
    {
        private DataGridView _searchResultsGrid;

        public DataGridView SearchResultsGrid
        {
            get { return _searchResultsGrid; } 
            set { _searchResultsGrid = value; } 
        }

        private DockStyle _initialDockStyle;
        public SearchResults(DataGridView searchResultsGrid)
        {
            InitializeComponent();
            _searchResultsGrid = searchResultsGrid;
            _initialDockStyle = _searchResultsGrid.Dock;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            _searchResultsGrid.Dock = DockStyle.Fill;
           
            Controls.Add(_searchResultsGrid);
           // _searchResultsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //_searchResultsGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            foreach (DataGridViewRow row in _searchResultsGrid.Rows)
            {
                row.Height = 100;
                row.Resizable= DataGridViewTriState.True;
                
                //   row.DefaultCellStyle.Font = new Font("Tahoma", 7);
            }
        }

        private void SearchResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            _searchResultsGrid.Dock = _initialDockStyle;
        }
    }
}
