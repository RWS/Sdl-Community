using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TQA
{
    [Action( "TqaRibbonGroupRun", Name = "Run TQA", Icon = "tqa3" )]
    [ActionLayout( typeof( TqaRibbonGroup ), 10, DisplayType.Large )]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
	public class StartButton : AbstractViewControllerAction<ProjectsController>
    {
        private MainWindow _mw;
        protected override void Execute()
        {
            if( _mw != null )
            {
                if( !_mw.Visible )
                {
                    _mw = new MainWindow( Controller );
                    _mw.Show();
                }
            }
            else
            {
                _mw = new MainWindow( Controller );
                _mw.Show();
            }

        }
    }
}
