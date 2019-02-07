using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Sdl.Community.CleanUpTasks.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class HtmlParserTests : IClassFixture<TestUtilities>
    {
        [Fact]
        public void ConstructorThrowsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HtmlHelper(null, null));
        }

        [Theory]
        [InlineData("<hello>Some text <1 and other text</hello>")]
        [InlineData("1 < 0.5")]
        public void ParseHtmlLessThanSymbol(string html)
        {
            StringBuilder builder = new StringBuilder();
            HtmlEntitizer entitizer = new HtmlEntitizer();
            string input = entitizer.Entitize(html);
            HtmlTagTable tagTable = new HtmlTagTable(input);

            Build(builder, input, tagTable);

            output.WriteLine(html);
            var processed = entitizer.DeEntitize(builder.ToString());
            output.WriteLine(processed);

            Assert.Equal(html, processed);
        }

        [Fact]
        public void ParseBrokenHtmlEndTagOnly()
        {
            var html = "</body>";

            StringBuilder builder = new StringBuilder();
            HtmlTagTable tagTable = new HtmlTagTable(html);

            Build(builder, html, tagTable);

            Assert.Equal(html, builder.ToString());
        }

        [Fact]
        public void ParseBrokenHtmlNoStartTag()
        {
            var html = "Some text with broken html</p>";

            StringBuilder builder = new StringBuilder();
            HtmlTagTable tagTable = new HtmlTagTable(html);

            Build(builder, html, tagTable);

            Assert.Equal(html, builder.ToString());
        }

        [Theory]
        [InlineData("html.html")]
        [InlineData("htmlform.html")]
        [InlineData("htmlcomments.html")]
        public void ParsedHtmlSameAsOriginal(string file)
        {
            var html = GetHtml(file);

            StringBuilder builder = new StringBuilder();
            HtmlTagTable tagTable = new HtmlTagTable(html);

            Build(builder, html, tagTable);

            output.WriteLine(html);
            output.WriteLine(builder.ToString());

            Assert.Equal(html, builder.ToString());
        }

        #region Fixture

        public HtmlParserTests(TestUtilities utility, ITestOutputHelper output)
        {
            Utility = utility;
            this.output = output;
        }

        public TestUtilities Utility { get; }

        private readonly ITestOutputHelper output;

        private string GetHtml(string fileName)
        {
            var path = Path.Combine(Utility.SaveFolder, fileName);

            return File.ReadAllText(path);
        }

        #endregion Fixture

        #region HTML

        private static void Build(StringBuilder builder, string html, HtmlTagTable tagTable)
        {
            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            var parser = new HtmlHelper(html, tagTable);

            if (parser.ParseErrors.Count() > 0)
            {
                // Fall back on regex parsing
                foreach (var chunk in Regex.Split(html, "(<.+?>)"))
                {
                    if (Regex.IsMatch(chunk, "<.+?>"))
                    {
                        builder.Append(chunk);
                    }
                    else
                    {
                        builder.Append(chunk);
                    }
                }

                return;
            }

            foreach (var node in parser.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    if (!tagTable.Table[node.Name].HasEndTag)
                    {
                        builder.Append(parser.GetRawStartTag(node));
                    }
                    else if (tagTable.Table[node.Name].IsEndGhostTag)
                    {
                        builder.Append(parser.GetRawEndTag(node));
                    }
                    else
                    {
                        builder.Append(parser.GetRawStartTag(node));
                        Build(builder, node.InnerHtml, tagTable);

                        if (node.Closed)
                        {
                            builder.Append(parser.GetRawEndTag(node));
                        }

                        node.RemoveAllChildren();
                    }
                }
                else if (node.NodeType == HtmlNodeType.Text)
                {
                    builder.Append(node.InnerText);
                }
                else
                {
                    builder.Append(node.InnerHtml);
                }
            }
        }

        #endregion HTML
    }
}