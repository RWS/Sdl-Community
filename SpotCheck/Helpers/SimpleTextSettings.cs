using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sdl.Studio.SpotCheck.Helpers
{
    public class SimpleTextSettings
    {
        private StreamReader _reader;
        private Dictionary<string, string> _data = new Dictionary<string, string>();

        public string SettingsFile { get; private set; }

        #region Init

        public void Open(string[] args)
        {
            _data.Clear();
            if (args == null || args.Length == 0)
                return;

            if (args[0].StartsWith("-f"))
            {
                string file;
                if (args[0] == "-f")
                {
                    file = args[1];
                }
                else
                {
                    file = args[0].Substring(2);
                }
                file = file.Trim(new char[] { '"' });

                if (File.Exists(file))
                {
                    SettingsFile = file;
                    _reader = new StreamReader(file);
                    InitFromFile();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Can't find settings file: " + file);
                }
            }
            else
            {
                InitFromCommandLine(args);
            }
        }

        public void Close()
        {
            if (_reader != null)
                _reader.Close();
        }


        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            StreamWriter sw = new StreamWriter(path, false);
            foreach ( string key in _data.Keys)
            {
                sw.WriteLine(string.Format("{0}={1}", key, _data[key]));
            }
            sw.Close();
        }

        private void InitFromCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                string key_value = args[i];
                string[] parts = key_value.Split(new char[] { '=' });
                if (parts.Length != 2 || parts[0] == "" || parts[1] == "")
                {
                    // handle space in "key= value"
                    if (key_value.EndsWith("="))
                    {
                        parts = new string[2];
                        parts[0] = key_value.Substring(0, key_value.Length - 1);
                        if (i + 1 < args.Length)
                        {
                            ++i;
                            parts[1] = args[i];
                        }
                        else
                        {
                            parts[1] = "";
                        }
                    }

                    // handle space in "key = value" or "key =value"
                    else if (i + 1 < args.Length && args[i + 1].StartsWith("="))
                    {
                        parts = new string[2];
                        parts[0] = key_value;
                        ++i;

                        // handle space in "key =value"
                        if (args[i].Length > 1)
                        {
                            parts[1] = args[i].Substring(1);
                        }
                        // handle space in "key = value"
                        else if (i + 1 < args.Length)
                        {
                            ++i;
                            parts[1] = args[i];
                        }
                        else
                        {
                            parts[1] = "";
                        }
                    }

                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Can't parse parameter " + key_value);
                        continue;
                    }
                }
                _data.Add(parts[0], parts[1].Trim(new char[] { '"' }));
            }
        }

        private void InitFromFile()
        {
            string line = _reader.ReadLine();
            while (line != null)
            {
                if (!line.TrimStart().StartsWith("#"))
                {
                    int pos = line.IndexOf('=');
                    if (pos != -1)
                    {
                        string key = line.Substring(0, pos).Trim();
                        string value = line.Substring(pos + 1, line.Length - pos - 1).Trim();
                        _data[key] = value;
                    }
                }
                line = _reader.ReadLine();
            }
        }

        #endregion

        #region setting values

        public void Clear()
        {
            _data.Clear();
        }

        public void SetValue(string key, string value)
        {
            _data[key] = value;
        }

        public void SetIntValue(string key, int value)
        {
            SetValue(key, value.ToString());
        }

        public void SetBoolValue(string key, bool value)
        {
            SetValue(key, value.ToString());
        }

        public void SetDateTimeValue(string key, DateTime value)
        {
            SetValue(key, value.ToString("dd/MM/yyyy, HH:mm:ss"));
        }

        public void SetListValue(string key, List<string> values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string val in values)
            {
                if (sb.Length != 0)
                    sb.Append(",");
                sb.Append('"');
                sb.Append(val);
                sb.Append('"');
            }

            SetValue(key, sb.ToString());
        }

        public void SetIntListValue(string key, List<int> values)
        {
            SetListValue(key, values.ConvertAll(x => x.ToString()));
        }
        #endregion

        #region getting typed values

        public string GetValue(string key)
        {
            if (!_data.ContainsKey(key))
                return null;
            return _data[key];
        }

        public string GetValue(string key, string defaultValue)
        {
            if (!_data.ContainsKey(key))
                return defaultValue;
            return _data[key];
        }

        public bool GetBoolValue(string key)
        {
            string value = GetValue(key);
            if (value == null)
                return false;

            return bool.Parse(value.ToLower());
        }

        public bool GetBoolValue(string key, bool defaultValue)
        {
            string value = GetValue(key);
            if (value == null)
                return defaultValue;

            return bool.Parse(value.ToLower());
        }

        public int GetIntValue(string key)
        {
            string value = GetValue(key);
            if (value == null)
                return 0;

            return int.Parse(value.ToLower());
        }

        public int GetIntValue(string key, int defaultValue)
        {
            string value = GetValue(key);
            if (value == null)
                return defaultValue;

            return int.Parse(value.ToLower());
        }

        public void GetListValue(string key, List<string> list)
        {
            list.Clear();
            string value = GetValue(key);
            if (String.IsNullOrEmpty(value))
                return;

            StringBuilder sb = new StringBuilder();
            bool quotedSegmentOpen = false;
            bool plainSegmentOpen = false;
            for (int i = 0; i < value.Length; ++i)
            {
                char c = value[i];
                switch (c)
                {
                    case '"':
                        if (quotedSegmentOpen)
                        {
                            list.Add(sb.ToString());
                            sb.Clear();
                            quotedSegmentOpen = false;
                        }
                        else if (plainSegmentOpen)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            quotedSegmentOpen = true;
                        }
                        break;
                    case ',':
                        if (plainSegmentOpen)
                        {
                            list.Add(sb.ToString().Trim());
                            sb.Clear();
                            plainSegmentOpen = false;
                        }
                        else if (quotedSegmentOpen)
                        {
                            sb.Append(c);
                        }
                        break;
                    default:
                        if (!char.IsWhiteSpace(c) && !quotedSegmentOpen && !plainSegmentOpen)
                            plainSegmentOpen = true;

                        if (!(char.IsWhiteSpace(c) && !quotedSegmentOpen))
                            sb.Append(c);
                        break;
                }
            }
            if (plainSegmentOpen)
                list.Add(sb.ToString().Trim());

            list.Remove("");
        }

        private void GetListValue(string key, List<string> list, bool clearList)
        {
            if (clearList)
                list.Clear();
            string val = GetValue(key);
            if (val != null && val.StartsWith("@"))
            {
                GetListValueFromFile(val, list);
            }
            else
            {
                GetListValueFromParameter(val, list);
            }
        }

        public void GetIntListValue(string key, List<int> list)
        {
            List<string> stringlist = new List<string>();
            GetListValue(key, stringlist);

            list.AddRange(stringlist.ConvertAll(i => Int32.Parse(i)));
        }

        public void GetIntListValue(string key, List<int> list, bool clearList)
        {
            List<string> stringlist = new List<string>();
            GetListValue(key, stringlist, clearList);

            list.AddRange(stringlist.ConvertAll(i => Int32.Parse(i)));
        }

        public DateTime GetDateTimeValue(string key)
        {
            if (!_data.ContainsKey(key))
                return DateTime.MinValue;
            return DateTime.ParseExact(_data[key], "dd/MM/yyyy, HH:mm:ss", null);
        }

        public DateTime GetDateTimeValue(string key, DateTime defaultValue)
        {
            if (!_data.ContainsKey(key))
                return defaultValue;
            return DateTime.ParseExact(_data[key], "dd/MM/yyyy, HH:mm:ss", null);
        }

        public void GetListValueFromFile(string value, List<string> list)
        {
            using (StreamReader sr = new StreamReader(value.Substring(1).Trim(new char[] { '"' })))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Trim().Length > 0)
                        list.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }

        public void GetListValueFromParameter(string value, List<string> list)
        {
            if (value == null)
                return;

            string[] items = value.Split(new char[] { ',' });
            foreach (string item in items)
            {
                list.Add(item.Trim());
            }
        }

        #endregion
    }
}