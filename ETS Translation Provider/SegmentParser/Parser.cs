using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SegmentParser
{
    public static class Parser
    {
        private static Regex startingTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)>");
        private static Regex endingTag = new Regex(@"</(\d+)>");
        private static Regex standaloneTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)/>");
        private static Regex lockedContentTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""(.*)""/>");
        public static Segment[] ParseFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found: " + path);
            }
            string[] text = File.ReadAllLines(path);
            return text.Select(line => ParseLine(line)).ToArray();
        }

        public static Segment ParseLine(string text)
        {
            Segment seg = new Segment();
            // Allow for including any character as long as it's within quotes. If there are no quotes, default to
            // allow any character except >
            string[] tags = Regex.Split(text, @"(<[^>""]*>|<.*?"".*?""\/>)");
            Stack<Tag> startingTags = new Stack<Tag>();
            foreach (string tag in tags)
            {
                if (tag == string.Empty)
                    continue;

                Tag parsedTag = ParseTag(tag);
                if (parsedTag == null)
                {
                    seg.Add(tag);
                    continue;
                }

                if (parsedTag.Type == TagType.Start)
                    startingTags.Push(parsedTag);
                else if (parsedTag.Type == TagType.End && startingTags.Count == 0)
                    throw new System.Exception(string.Format("Line does not have matching starting and ending tags: {0}", text));
                else if (parsedTag.Type == TagType.End)
                {
                    Tag correspondingStartTag = startingTags.Pop();
                    parsedTag.TagID = correspondingStartTag.TagID;
                }

                seg.Add(parsedTag);
            }
            if (startingTags.Count != 0)
                throw new System.Exception(string.Format("Line does not have matching starting and ending tags: {0}", text));

            return seg;
        }

        public static Tag ParseTag(string tag)
        {
            Match m;
            if ((m = startingTag.Match(tag)).Success)
                return new Tag(TagType.Start, m.Groups[3].Value, int.Parse(m.Groups[1].Value), !string.IsNullOrEmpty(m.Groups[2].Value) ? int.Parse(m.Groups[2].Value) : 0, null);
            else if ((m = endingTag.Match(tag)).Success)
                return new Tag(TagType.End, "0", int.Parse(m.Groups[1].Value));
            else if ((m = standaloneTag.Match(tag)).Success)
                return new Tag(TagType.Standalone, m.Groups[3].Value, int.Parse(m.Groups[1].Value), !string.IsNullOrEmpty(m.Groups[2].Value) ? int.Parse(m.Groups[2].Value) : 0, null);
            else if ((m = lockedContentTag.Match(tag)).Success)
                return new Tag(TagType.LockedContent, m.Groups[3].Value, int.Parse(m.Groups[1].Value), !string.IsNullOrEmpty(m.Groups[2].Value) ? int.Parse(m.Groups[2].Value) : 0, m.Groups[4].Value);
            return null;
        }
    }
}
