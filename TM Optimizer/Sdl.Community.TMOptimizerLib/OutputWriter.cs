using System;
using System.IO;

namespace Sdl.Community.TMOptimizerLib
{
	class OutputWriter
    {
        private readonly string _outputFile;
        private StreamWriter _writer;

        public OutputWriter(string outputFile)
        {
            _outputFile = outputFile;
        }

        public void InitializeWorkbenchTmx(string sourceLang)
        {
            _writer = new StreamWriter(_outputFile);
            _writer.Write("<?xml version=\"1.0\" ?>\n" +
                          "<!DOCTYPE tmx SYSTEM \"tmx14.dtd\">\n" +
                          "<tmx version=\"1.4\">\n" +
                          "<header\n" +
                          "creationtool=\"TRADOS Translator's Workbench for Windows\"\n" +
                          "creationtoolversion=\"Edition 8 Build 863\"\n" +
                          "segtype=\"sentence\"\n" +
                          "o-tmf=\"TW4Win 2.0 Format\"\n" +
                          "adminlang=\"EN-US\"\n" +
                          "srclang=\"" + sourceLang + "\"\n" +
                          "datatype=\"rtf\"\n" +
                          "creationdate=\"" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\"\n" +
                          "creationid=\"TMCleaner\"" +
                          ">\n\n</header>\n\n<body>\n\n");
        }

        public void InitializeStudioTmx(string sourceLang)
        {
            _writer = new StreamWriter(_outputFile);
            _writer.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                          "<tmx version=\"1.4\">\n" +
                          "<header\n" +
                          "creationtool=\"SDL Language Platform\"\n" +
                          "creationtoolversion=\"8.0\"\n" +
                          "segtype=\"sentence\"\n" +
                          "o-tmf=\"SDL TM8 Format\"\n" +
                          "adminlang=\"EN-US\"\n" +
                          "srclang=\"" + sourceLang + "\"\n" +
                          "datatype=\"xml\"\n" +
                          "creationdate=\"" + DateTime.Now.ToString("yyyyMMddHHmmss") + "\"\n" +
                          "creationid=\"TMCleaner\"" +
                          ">\n\n</header>\n\n<body>\n\n");
        }

        public void Write(string content)
        {
            _writer.Write(content + "\n\n");
        }
        
        public void Complete()
        {
            _writer.Write("\n</body>\n</tmx>");
            _writer.Flush();
            _writer.Close();
        }
    }
}