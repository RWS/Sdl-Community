using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ApplyTMTemplate.UI;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
    public class ApplyTMTemplateAction : AbstractTellMeAction
    {
	    public ApplyTMTemplateAction()
	    {
		    Name = "Start Apply TM Template";
	    }

	    public override void Execute()
	    {
			var mainWindowViewModel = new MainWindowViewModel(new TemplateLoader(), new TMLoader());

		    var mainWindow = new MainWindow
		    {
			    DataContext = mainWindowViewModel
		    };

		    mainWindow.Show();
		}

	    public override bool IsAvailable => true;

	    public override string Category => "Apply TM Template results";

	    public override Icon Icon => PluginResources.ATTA;
	}
}
