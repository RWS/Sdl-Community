//using Sdl.Desktop.IntegrationApi;
//using Sdl.Desktop.IntegrationApi.Extensions;

//namespace Sdl.Community.TQA
//{
//	[Action( "TqaRibbonGroupHelp", Name = "Read me", Icon = "Help" )]
//	[ActionLayout( typeof( TqaRibbonGroup ), 11, DisplayType.Large )]
//	public class HelpButton : AbstractAction
//	{
//		private HelpForm _hf;
//		protected override void Execute()
//		{
//			if( _hf != null )
//			{
//				if( !_hf.Visible )
//				{
//					_hf = new HelpForm();
//					_hf.Show();
//				}
//			}
//			else
//			{
//				_hf = new HelpForm();
//				_hf.Show();
//			}
//		}
//	}
//}