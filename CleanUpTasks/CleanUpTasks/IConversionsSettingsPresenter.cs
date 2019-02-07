using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;

namespace Sdl.Community.CleanUpTasks
{
	[ContractClass(typeof(IConversionsSettingsPresenterContract))]
    public interface IConversionsSettingsPresenter
    {
        void AddFile();
        void RemoveFile();
        void GenerateFile(IConversionFileView view);
        void EditFile(IConversionFileView view);
        void DownClick();
        void UpClick();
        void SaveSettings();
        void Initialize();
    }
}