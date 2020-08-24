using Sdl.Community.MtEnhancedProvider.Model.Interface;

namespace Sdl.Community.MtEnhancedProvider.Model
{
	public class ViewDetails:ModelBase
	{
		public string Name { get; set; }
		public IModelBase ViewModel { get; set; }
	}
}
