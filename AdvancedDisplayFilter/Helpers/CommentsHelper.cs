using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
