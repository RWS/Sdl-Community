using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	[Flags]
	public enum State
	{
		DefaultState = 0x00,
		PatternsEncrypted = 0x01,
		DataEncrypted = 0x02,
		Encrypted = PatternsEncrypted | DataEncrypted,
		Decrypted = 0x04
	}
}
