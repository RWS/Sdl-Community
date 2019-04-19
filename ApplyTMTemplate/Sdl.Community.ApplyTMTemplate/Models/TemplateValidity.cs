using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
