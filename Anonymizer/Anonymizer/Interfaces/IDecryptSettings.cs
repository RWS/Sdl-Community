namespace Sdl.Community.projectAnonymizer.Interfaces
{
	interface IDecryptSettings
	{
		string EncryptionKey { get; set; }
		bool IgnoreEncrypted { get; set; }
	}
}
