using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class CommentsHelper
	{
		public static bool IsCommentTextFoundWithRegex(List<IComment> comments, string regexExpression)
		{
			foreach (var comment in comments)
			{
				var regex = new Regex(regexExpression, RegexOptions.None);
				var match = regex.Match(comment.Text);
				if (match.Success)
				{
					return true;
				}
			}
			return false;
		}
	}
}
