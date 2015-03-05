using System;

namespace Sdl.Community.Legit
{
    public class ConversionResult
    {
        private readonly string _ttxFilePath;

        public string OriginalFilePath { get; private set; }
        public Exception Exception { get; set; }
        public string SettingsFilePath { get; set; }

        public string TtxFilePath
        {
            get
            {
                return ConversionWasSuccessful ? _ttxFilePath : string.Empty;
            }
        }

        public bool ConversionWasSuccessful
        {
            get
            {
                return Exception == null;
            }
        }

        public ConversionResult(string filePath)
        {
            OriginalFilePath = filePath;
            _ttxFilePath = OriginalFilePath + ".ttx";
        }

        public ConversionResult(string filePath, string settingsFilePath)
            : this(filePath)
        {
            SettingsFilePath = settingsFilePath;
        }
    }
}