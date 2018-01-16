using System;
using System.Collections.Generic;

namespace Sdl.Community.Utilities.TMTool.Task
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="progress">percent of processed</param>
	/// <param name="operationType">operation currently performing</param>
	public delegate void OnProgressDelegate(double progress, string operationMsg);
	/// <summary>
	/// 
	/// </summary>
	/// <param name="log">message to write to log</param>
	public delegate void OnAddLogDelegate(string logMsg);

	public interface ITask
	{
		/// <summary>
		/// task unique identifier
		/// </summary>
		Guid TaskID { get; }
		/// <summary>
		/// task name
		/// </summary>
		string TaskName { get; }
		/// <summary>
		/// file types that can be processed by current task
		/// key - filter, value - description
		/// e.g. key - "*.sdltm", value - "SDL Translation Memory files"
		/// </summary>
		Dictionary<string, string> SupportedFileTypes { get; }
		/// <summary>
		/// settings user control object
		/// </summary>
		IControl Control { get; }

		/// <summary>
		/// performs task
		/// </summary>
		/// <param name="fileName">file full path to perform task on</param>
		void Execute(string fileName);

		event OnProgressDelegate OnProgress;
		event OnAddLogDelegate OnLogAdded;
	}
}
