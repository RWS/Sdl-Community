using System;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
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