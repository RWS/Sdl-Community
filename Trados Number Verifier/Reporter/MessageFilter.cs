using System.Collections.Generic;

namespace Sdl.Community.NumberVerifier.Reporter
{
	public class MessageFilter : IMessageFilter
	{
		public MessageFilter(Dictionary<string, bool> resultTable) => ResultTable = resultTable;

		private Dictionary<string, bool> ResultTable { get; }

		public bool IsAllowed(string message) => !ResultTable.ContainsKey(message) || ResultTable[message];
	}
}