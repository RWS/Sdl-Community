using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace TQA
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
    [Action( "TqaRibbonGroupHelp", Name = "Read me", Icon = "Help" )]
    [ActionLayout( typeof( TqaRibbonGroup ), 11, DisplayType.Large )]
    public class HelpButton : AbstractAction
    {
        private HelpForm _hf;
        protected override void Execute()
        {
            if( _hf != null )
            {
                if( !_hf.Visible )
                {
                    _hf = new HelpForm();
                    _hf.Show();
                }
            }
            else
            {
                _hf = new HelpForm();
                _hf.Show();
            }
        }
    }

}
