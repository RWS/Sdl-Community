using System;
using System.Collections.Generic;
using Multilingual.Excel.FileType.Common;
using Multilingual.Excel.FileType.Models;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Providers.StudioComment
{
	public class StudioCommentPropertyProvider
	{
		private readonly List<CommentProperty> _commentProperties;

		public StudioCommentPropertyProvider()
		{
			_commentProperties = new List<CommentProperty>()
			{
				//new CommentProperty
				//{
				//	Name = "Text",
				//	Type = Enumerators.CommentPropertyType.Text,
				//	ValueType = typeof(string),
				//	Description = "The text associated with the comment"
				//},
				new CommentProperty
				{
					Name = "Author",
					Type = Enumerators.CommentPropertyType.Attribute,
					ValueType = typeof(string),
					Description = "Name of person or system that created/updated the comment"
				},
				new CommentProperty
				{
					Name = "Date",
					Type = Enumerators.CommentPropertyType.Attribute,
					ValueType = typeof(DateTime),
					Description = "Time at which the comment was created or last edited"
				},
				new CommentProperty
				{
					Name = "Version",
					Type = Enumerators.CommentPropertyType.Attribute,
					ValueType = typeof(string),
					Description = "The version number to track the comment threads; expected form \"1.0\", \"2.0\" etc."
				},
				new CommentProperty
				{
					Name = "Severity",
					Type = Enumerators.CommentPropertyType.Attribute,
					ValueType = typeof(Severity),
					Description = "Indication of severity of the issue for which the comment has been added"
				}
				//new CommentProperty
				//{
				//	Name = "Annotates",
				//	Type = Enumerators.CommentPropertyType.Attribute,
				//	ValueType = typeof(string),
				//	Description = "Indicates if a note element pertains to the 'source' or the 'target', or 'general'"
				//}
			};
		}

		public List<CommentProperty> DefaultCommentProperties => _commentProperties;
	}
}
