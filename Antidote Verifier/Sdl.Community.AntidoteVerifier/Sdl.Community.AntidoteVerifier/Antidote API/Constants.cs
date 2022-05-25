using System.Runtime.InteropServices;

namespace Sdl.Community.AntidoteVerifier.Antidote_API
{
    class Constants
    {
        public const string AntidoteApp = "AntQ";
        public const string RegistryInstallLocation = "SOFTWARE\\Druide informatique inc.\\Antidote";
        public const string RegistryInstallLocationValue = "DossierAntidote";
        public const string AntidoteExecutable = "Antido32.exe";
        public const string ApiAntidote = "-activex";
        public const string ProgIDAntidoteApiOle = "Antidote.ApiOle";
    }

    [ComVisible(false)]
    sealed public class ConstantsUtils
    {
        public const string Corrector = "Correcteur";
        public const string LastSelectedGuide = "GuideDernierChoisi";
        public const string LastSelectedDictionary = "DictionnaireDernierChoisi";

    }
}
