namespace Sdl.Community.projectAnonymizer.Interfaces
{
	internal interface IDecryptSettings
	{
		string EncryptionKey { get; set; }
		bool IgnoreEncrypted { get; set; }
	}
}