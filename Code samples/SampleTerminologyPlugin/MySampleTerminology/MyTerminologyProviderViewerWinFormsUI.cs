using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MySampleTerminology
{
    [TerminologyProviderViewerWinFormsUI]
    internal class MyTerminologyProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
    {
        private readonly TermProviderControl termControl = new TermProviderControl();
        private MyTerminologyProvider _terminologyProvider;
        public readonly string fileName;

        public bool Initialized => true;

        public Entry SelectedTerm { get; set; }

        public bool CanAddTerm => true;

        public bool IsEditing => true;

        System.Windows.Forms.Control ITerminologyProviderViewerWinFormsUI.Control => termControl;

        public event EventHandler<EntryEventArgs> SelectedTermChanged;
        public event EventHandler TermChanged;

        public void AddAndEditTerm(Entry term, string source, string target)
        {
            MessageBox.Show("Sorry, editing terms is currently not implemented :-(", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void AddTerm(string source, string target)
        {
            string fileName = _terminologyProvider._fileName.Replace("file:///", "");
            StreamReader glossaryFile = new StreamReader(fileName);
            int i = 0;
            while (!glossaryFile.EndOfStream)
            {
                i++;
                glossaryFile.ReadLine();
            }
            glossaryFile.Close();
            int newEntryId = i + 1;
            StreamWriter outFile = new StreamWriter(fileName, true);
            outFile.WriteLine((newEntryId.ToString() + ";" + source + ";" + target + ";"));
            outFile.Close();

            //Show added entry in web browser control of the Termbase Viewer window
            string tmpFile = System.IO.Path.GetTempPath() + "simple_list_entry.htm";
            StreamWriter previewFile = new StreamWriter(tmpFile);
            previewFile.Write("<html><body><b>Entry id:</b> " + newEntryId.ToString() +
                "<br/><b>Source term:</b> " + source +
                "<br/><b>Target term:</b> " + target +
                "</body></html>");
            previewFile.Close();
            termControl.SetNavigation(tmpFile);
        }

        public void CancelTerm()
        {
            throw new NotImplementedException();
        }

        public void EditTerm(Entry term)
        {
            throw new NotImplementedException();
        }

        public void Initialize(ITerminologyProvider terminologyProvider, CultureCode source, CultureCode target)
        {
            _terminologyProvider = (MyTerminologyProvider)terminologyProvider;
        }

        public void JumpToTerm(Entry entry)
        {
            // Load the glossary file
            string fileName = _terminologyProvider._fileName.Replace("file:///", "");
            StreamReader glossaryFile = new StreamReader(fileName);
            string[] chunks;
            string entryContent = String.Empty;
            glossaryFile.ReadLine();
            // Loop through all lines of the file and find the line that has the current entry id
            while (!glossaryFile.EndOfStream)
            {
                string thisLine = glossaryFile.ReadLine();
                chunks = thisLine.Split(';');
                string thisId = chunks[0];
                if (thisId == entry.Id.ToString())
                {
                    entryContent = thisLine;
                    break;
                }
            }

            // Parse the line alongside the semi-colon
            chunks = entryContent.Split(';');

            // Generate a small HTML file to display in the Termbase Viewer control
            string tmpFile = System.IO.Path.GetTempPath() + "simple_list_entry.htm";
            StreamWriter previewFile = new StreamWriter(tmpFile);
            previewFile.Write("<html><body><b>Entry id:</b> " + chunks[0] +
                "<br/><b>Source term:</b> " + chunks[1] +
                "<br/><b>Target term:</b> " + chunks[2] +
                "<br/><b>Definition:</b> " + chunks[3] +
                "</body></html>");
            previewFile.Close();
            termControl.SetNavigation(tmpFile);

            glossaryFile.Close();
        }

        public void Release()
        {
            
        }

        public void SaveTerm()
        {
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return terminologyProviderUri.ToString().StartsWith("file");
        }
    }
}
