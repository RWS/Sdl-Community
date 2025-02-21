using Multilingual.Excel.FileType.Constants;

namespace Multilingual.Excel.FileType.Services.Entities
{
	public class EntityMarkerConversionService
	{

		public string ForwardEntityMarkersConversion(string inputText)
		{
			return inputText
				.Replace(EntityConstants.BeginEntityRef, EntityConstants.BeginSdlEntityRefEscape)
				.Replace(EntityConstants.EndEntityRef, EntityConstants.EndSdlEntityRefEscape);
		}

		public string BackwardEntityMarkersConversion(string inputText)
		{
			return inputText
				.Replace(EntityConstants.BeginSdlEntityRefEscape, EntityConstants.BeginEntityRef)
				.Replace(EntityConstants.EndSdlEntityRefEscape, EntityConstants.EndEntityRef)
				.Replace(EntityConstants.AmpersandEntityRefEscape, EntityConstants.AmpersandEntityRef)
				.Replace(EntityConstants.LessThanEntityRefEscape, EntityConstants.LessThanEntityRef) 
				.Replace(EntityConstants.GreaterThanEntityRefEscape, EntityConstants.GreaterThanEntityRef)
				.Replace(EntityConstants.SoftReturnEntityRefEscape, EntityConstants.SoftReturnEntityRef)
				.Replace(EntityConstants.EmptyTargetRefEscape, string.Empty);
		}
	}
}
