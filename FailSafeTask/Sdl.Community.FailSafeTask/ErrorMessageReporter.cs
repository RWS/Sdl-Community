using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;

namespace Sdl.Community.FailSafeTask
{
    public class ErrorMessageReporter : AbstractBilingualContentHandler
    {
        private readonly List<Tuple<string, string>> messages = new List<Tuple<string, string>>();
        private IBilingualContentMessageReporter reporter = null;

        public override void Initialize(IDocumentProperties documentInfo)
        {
            reporter = MessageReporter;
        }

        public void ReportErrors()
        {
            foreach (var msg in messages)
            {
                reporter.ReportMessage(null, msg.Item1, Sdl.FileTypeSupport.Framework.NativeApi.ErrorLevel.Error, msg.Item2, msg.Item1);
            }
        }

        public void StoreMessage(string name, string msg)
        {
            messages.Add(new Tuple<string, string>(name, msg));
        }
    }
}