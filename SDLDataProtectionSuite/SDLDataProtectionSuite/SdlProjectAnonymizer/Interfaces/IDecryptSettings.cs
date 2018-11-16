namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Interfaces
{
	internal interface IDecryptSettings
	{
		string EncryptionKey { get; set; }
		bool IgnoreEncrypted { get; set; }
	}
}