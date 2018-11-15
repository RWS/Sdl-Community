using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Studio.SpotCheck.Helpers
{
    public class ApplicationSettingsBase
    {
        protected SimpleTextSettings _settings;

        public string SettingsFile { get; private set; }
        public Form MainForm { get; set; }

        public static string DefaultSettingsPath
        {
            get
            {
                string filename = Path.GetFileNameWithoutExtension(new System.Uri(
                    System.Reflection.Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath);
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SDL.Imt" + "\\" + filename + ".settings.txt");
            }
        }

        public virtual void Init(string[] args)
        {
            _settings = new SimpleTextSettings();
            _settings.Open(args);
            _settings.Close();
            SettingsFile = _settings.SettingsFile;
            if (SettingsFile == null)
                SettingsFile = DefaultSettingsPath;
            TextFileToValues();
        }

        public void RestorePosition()
        {
            if (MainForm != null && _settings != null)
            {
                MainForm.Left = _settings.GetIntValue("form-left", MainForm.Left);
                if (MainForm.Left < 0)
                    MainForm.Left = 0;
                MainForm.Top = _settings.GetIntValue("form-top", MainForm.Top);
                if (MainForm.Top < 0)
                    MainForm.Top = 0;
                MainForm.Width = _settings.GetIntValue("form-width", MainForm.Width);
                MainForm.Height = _settings.GetIntValue("form-height", MainForm.Height);
            }
        }

        public void GetPosition(out int left, out int top, out int width, out int height)
        {
            height = _settings.GetIntValue("form-height", 300);
            width = _settings.GetIntValue("form-width", 150);
            left = _settings.GetIntValue("form-left", 50);
            top = _settings.GetIntValue("form-top", 50);

            if (top < 0)
                top = 0;
            if (left < 0)
                left = 0;
        }

        public void SavePosition(int left, int top, int width, int height)
        {
            if (_settings != null)
            {
                _settings.SetIntValue("form-left", left);
                _settings.SetIntValue("form-top", top);
                _settings.SetIntValue("form-width", width);
                _settings.SetIntValue("form-height", height);
            }
        }

        public void SavePosition()
        {
            if (MainForm != null)
            {
                SavePosition(MainForm.Left, MainForm.Top, MainForm.Width, MainForm.Height);
            }
        }

        protected virtual void ValuesToTextFile()
        {
        }

        protected virtual void TextFileToValues()
        {
        }

        public void Load(string path)
        {
            string[] args = new string[] { "-f", path };
            Init(args);
        }

        public void Save(string path)
        {
            Save(path, false);
        }

        public void Save(string path, bool keepPosition)
        {
            if (!keepPosition)
                _settings.Clear();
            ValuesToTextFile();
            if (!keepPosition)
                SavePosition();
            _settings.Save(path);
        }

    }
}
