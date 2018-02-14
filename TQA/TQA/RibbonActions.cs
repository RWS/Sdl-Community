using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace TQA
{
    [Action( "TqaRibbonGroupRun", Name = "Run TQA", Icon = "tqa3" )]
    [ActionLayout( typeof( TqaRibbonGroup ), 10, DisplayType.Large )]
    public class StartButton : AbstractViewControllerAction<ProjectsController>
    {
        MainWindow mw;
        protected override void Execute()
        {
            if( mw != null )
            {
                if( !mw.Visible )
                {
                    mw = new MainWindow( Controller );
                    mw.Show();
                }
            }
            else
            {
                mw = new MainWindow( Controller );
                mw.Show();
            }

        }
    }
    [Action( "TqaRibbonGroupHelp", Name = "Read me", Icon = "Help" )]
    [ActionLayout( typeof( TqaRibbonGroup ), 11, DisplayType.Large )]
    public class HelpButton : AbstractAction
    {
        HelpForm hf;
        protected override void Execute()
        {
            if( hf != null )
            {
                if( !hf.Visible )
                {
                    hf = new HelpForm();
                    hf.Show();
                }
            }
            else
            {
                hf = new HelpForm();
                hf.Show();
            }
        }
    }

}
