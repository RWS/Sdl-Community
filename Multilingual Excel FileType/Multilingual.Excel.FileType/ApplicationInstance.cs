using System.Windows.Forms;

namespace Multilingual.Excel.FileType
{
    internal class ApplicationInstance
    {
        public static Form GetActiveForm()
        {
            var allForms = Application.OpenForms;
            var activeForm = allForms[allForms.Count - 1];
            foreach (Form form in allForms)
            {
                if (form.GetType().Name == "StudioWindowForm")
                {
                    activeForm = form;
                    break;
                }
            }

            return activeForm;
        }
    }
}