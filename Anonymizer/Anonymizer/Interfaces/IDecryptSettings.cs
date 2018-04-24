using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.Anonymizer.Interfaces
{
	interface IDecryptSettings
	{
		string EncryptionKey { get; set; }
	}
}
