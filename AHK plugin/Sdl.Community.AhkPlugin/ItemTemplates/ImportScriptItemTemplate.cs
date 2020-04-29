using System.Windows.Input;

namespace Sdl.Community.AhkPlugin.ItemTemplates
{
   public class ImportScriptItemTemplate
    {
	    public string Content { get; set; }
	    public string FilePath { get; set; }
	    public ICommand RemoveFileCommand { get; set; }

		
    }
}
