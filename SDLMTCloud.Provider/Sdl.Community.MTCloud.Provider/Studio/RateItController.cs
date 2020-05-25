using System;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = System.Windows.Forms.Application;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[ViewPart(
		Id = "SDL MT Cloud RateIt",
		Name = "SDL MT Cloud RateIt",
		Description = "SDL MT Cloud RateIt",
		Icon = "")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public  class RateItController : AbstractViewPartController
	{		
		private Lazy<View.RateItControl> _control;

		protected override void Initialize()
		{
			_control = new Lazy<View.RateItControl>(()=>new View.RateItControl());						
		}

		public IRatingService RateIt => _control?.Value.RatingService;

		protected override Control GetContentControl()
		{
			//var parent = _control.Value.Controls.Owner.Parent;
			//var form = parent as Form;
			//var parent = cont.Get
			//foreach (Window window in Application.OpenForms)
			//{
			//	Console.WriteLine(window.Title);
			//}
			return _control.Value;

		}
	}
}
