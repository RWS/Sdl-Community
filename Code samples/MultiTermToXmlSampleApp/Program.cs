using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.Terminology.TerminologyProvider.Core.Termbase;

namespace MultiTermToXmlSampleApp
{
    internal class Program
    {
        private const int PageSize = 200;

        private static IStudioTermbase GetTermbase(string termbasePath)
        {
            var termbaseUri = new Uri("ttb." + new Uri(termbasePath).AbsoluteUri);
            var provider = TerminologyProviderManager.Instance.GetTerminologyProvider(termbaseUri);
            if (provider == null || !provider.Initialize())
            {
                return null;
            }

            return provider as IStudioTermbase;
        }

        private static IEnumerable<Entry> GetAllEntries(IStudioTermbase termbase)
        {
            var lastSeenId = 0;
            while (true)
            {
                var page = termbase.GetEntries(lastSeenId, PageSize);
                if (page == null || page.Count == 0)
                {
                    yield break;
                }

                foreach (var entry in page)
                {
                    yield return entry;
                }

                lastSeenId = page[page.Count - 1].Id;
                if (page.Count < PageSize)
                {
                    yield break;
                }
            }
        }

        private static IEnumerable<XElement> ToXml(IEnumerable<EntryField> fields)
        {
            return fields?.Select(field => new XElement("field",
                       new XAttribute("name", field.Name ?? string.Empty),
                       new XAttribute("value", field.Value ?? string.Empty),
                       ToXml(field.Fields)))
                   ?? Enumerable.Empty<XElement>();
        }

        private static XElement ToXml(Entry entry)
        {
            return new XElement("entry",
                new XAttribute("id", entry.Id),
                ToXml(entry.Fields),
                entry.Languages?.Select(language => new XElement("language",
                    new XAttribute("name", language.Name ?? string.Empty),
                    new XAttribute("locale", language.Locale?.Name ?? string.Empty),
                    ToXml(language.Fields),
                    language.Terms?.Select(term => new XElement("term",
                        new XAttribute("value", term.Value ?? string.Empty),
                        ToXml(term.Fields))))));
        }

        private static void ExportTermbaseToXml(string termbasePath, string xmlPath)
        {
            var termbase = GetTermbase(termbasePath);
            if (termbase == null)
            {
                Console.WriteLine($"Could not open termbase {termbasePath}");
                return;
            }

            var document = new XDocument(new XElement("termbase",
                new XAttribute("path", termbasePath),
                GetAllEntries(termbase).Select(ToXml)));
            document.Save(xmlPath);
        }

        static void Main(string[] args)
        {
            var termbasePath = "C:\\john\\OneDrive - SDL\\Documents\\Trados\\MultiTerm\\Termbases\\de pl multi.ttb";
            ExportTermbaseToXml(termbasePath, Path.ChangeExtension(termbasePath, ".xml"));
        }
    }
}