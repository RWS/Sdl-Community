namespace Sdl.Community.IATETerminologyProvider.Model
{
    public interface IPathInfo
    {
        string ApplicationFullPath { get; }
        string DbaseCacheFullPath { get; }
        string RWSAppStoreFullPath { get; }
        string SettingsFilePath { get; }
        string TemporaryStorageFullPath { get; }
    }
}