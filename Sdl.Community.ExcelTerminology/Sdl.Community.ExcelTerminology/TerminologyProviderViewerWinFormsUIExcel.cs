using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Ui;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    [TerminologyProviderViewerWinFormsUI]
    public class TerminologyProviderViewerWinFormsUIExcel : ITerminologyProviderViewerWinFormsUI
    {

        public Control Control
        {
            get
            {
                var control = new TermsList
                {
                    Text = @"TerminologyProviderViewerWinFormsUIExcel",
                    BackColor = Color.White
                };

                JumpToTermAction += control.JumpToTerm;
                AddAndEditAction += control.AddAndEdit;
                return control;
            }
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return true;
        }

        public void JumpToTerm(IEntry entry)
        {
            JumpToTermAction?.Invoke(entry);
        }

        public void AddTerm(string source, string target)
        {
          
        }

        public void EditTerm(IEntry term)
        {
            
        }

        public void AddAndEditTerm(IEntry term, string source, string target)
        {
            var dataGrid = new ExcelDataGrid
            {
                Term = target,
                Approved = null
            };
            AddAndEditAction?.Invoke(term, dataGrid);
        }

        public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
        {

        }

        public void Release()
        {
            
        }


        public bool Initialized => true;
        public IEntry SelectedTerm { get; set; }
        public event EventHandler TermChanged;
        public event EventHandler<EntryEventArgs> SelectedTermChanged;
        public event Action<IEntry> JumpToTermAction;
        public event Action<IEntry, ExcelDataGrid> AddAndEditAction;
    }
}
