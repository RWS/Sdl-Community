﻿namespace Sdl.Community.IATETerminologyProvider.Interface
{
	public interface IMessageBoxService
	{
		MessageDialogResult ShowYesNoMessageBox(string title, string message);
	}
	public enum MessageDialogResult
	{
		Yes,
		No
	}
}
