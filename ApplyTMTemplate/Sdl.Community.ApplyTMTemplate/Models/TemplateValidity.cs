using System;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	[Flags]
	public enum TemplateValidity
	{
		IsNotValid = 0,
		IsValid = 1,
		HasResources = 2
	}
}