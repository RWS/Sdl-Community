using System.Diagnostics;
using System.Globalization;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;

namespace TMXTests
{
	public class TestParser
	{
		[Fact]
		public void Test()
		{
			var parser = new TmxParser("..\\..\\..\\..\\SampleTestFiles\\#4.tmx", quickImport:false);
			Assert.Equal(false, parser.HasError);
			var source = new CultureInfo(parser.Header.SourceLanguage);
			var target = new CultureInfo(parser.Header.TargetLanguage);
			var tus = parser.TryReadNextTUs();
			
			Assert.Equal("INTRODUCTION", tus[0].Texts[0].FormattedText);
			Assert.Equal("INTRODUCCIÓN", tus[0].Texts[1].FormattedText);

			Assert.Equal("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.",
				tus[1].Texts[0].FormattedText);
			Assert.Equal("Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.", 
				tus[1].Texts[1].FormattedText);
		}

		[Fact]
		public void TestStringCharAt()
		{
			var root = "..\\..\\..\\..";
			var file = $"{root}\\SampleTestFiles\\#4.tmx";
			var text = File.ReadAllText(file);
			var lineIdx = 0;
			var colIdx = 0;
			var text2 = "";
			foreach (var ch in text)
			{
				if (ch == '\r')
				{
					++lineIdx;
					colIdx = 0;
					text2 += ch;
				}
				else if (ch == '\n')
					text2 += ch;
				else
					text2 += Util.StringCharAt(text, lineIdx, colIdx++).ch;
			}
			Assert.True(text == text2);
		}

		[Fact]
		public void TestRemoveXmlNodeContainingChar()
		{
			var s = @"<?xml version=""1.0"" encoding=""utf-8""?>
<aa>This document contains both the<inner></inner> bla</aa>
<bb>bla <inner>Interserve Construction Health and</inner> bla</bb>
<cc>bla <inner>that Interserve Construction and all subcontractors</inner> bla</cc>
<dd>bla <inner>achieve our vision of being the trusted Partner to all those </inner> bla</dd>
<ee>bla <inner>shareholders, customers, employees,</inner> bla</ee>
<ff>bla <inner> suppliers, members of the community</inner> bla</ff>
<gg>bla <inner> in which we are working, or any</inner> bla</gg>
<hh>bla <inner>Subcontractors are required</inner> bla</hh>
<ii>bla <inner> to assist and co-operate with</inner> bla</ii>
<jj>bla <inner> Interserve Construction with health</inner> bla</jj>
<kk>bla <inner>, safety and environmental related</inner> bla</kk>
<ll>bla <inner> issues, including initiatives</inner> bla</ll>
<mm>bla <inner> that may be</inner> bla</mm>
<nn>bla <inner>operated from time to</inner> bla</nn>
";
			var a = @"<?xml version=""1.0"" encoding=""utf-8""?>
<aa>This document contains both the<inner></inner> bla</aa>
<bb>bla <inner>Interserve Construction Health and</inner> bla</bb>
<cc>bla <inner>that Interserve Construction and all subcontractors</inner> bla</cc>
<dd>bla <inner>achieve our vision of being the trusted Partner to all those </inner> bla</dd>
<ee>bla <inner>shareholders, customers, employees,</inner> bla</ee>
<ff>bla <inner> suppliers, members of the community</inner> bla</ff>
<gg>bla <inner> in which we are working, or any</inner> bla</gg>
<hh>bla <inner>Subcontractors are required</inner> bla</hh>
<ii>bla <inner> to assist and co-operate with</inner> bla</ii>
<jj>bla <inner> Interserve Construction with health</inner> bla</jj>

<ll>bla <inner> issues, including initiatives</inner> bla</ll>
<mm>bla <inner> that may be</inner> bla</mm>
<nn>bla <inner>operated from time to</inner> bla</nn>
";
			var ch = Util.StringCharAt(s, 11, 34);
			Assert.True(ch.ch == 'n');
			Util.TryRemoveXmlNodeContainingChar(ref s, ch.pos, "kk");
			Assert.True(s == a);

			var b = @"<?xml version=""1.0"" encoding=""utf-8""?>
<aa>This document contains both the<inner></inner> bla</aa>
<bb>bla <inner>Interserve Construction Health and</inner> bla</bb>
<cc>bla <inner>that Interserve Construction and all subcontractors</inner> bla</cc>
<dd>bla <inner>achieve our vision of being the trusted Partner to all those </inner> bla</dd>
<ee>bla <inner>shareholders, customers, employees,</inner> bla</ee>
<ff>bla <inner> suppliers, members of the community</inner> bla</ff>

<hh>bla <inner>Subcontractors are required</inner> bla</hh>
<ii>bla <inner> to assist and co-operate with</inner> bla</ii>
<jj>bla <inner> Interserve Construction with health</inner> bla</jj>

<ll>bla <inner> issues, including initiatives</inner> bla</ll>
<mm>bla <inner> that may be</inner> bla</mm>
<nn>bla <inner>operated from time to</inner> bla</nn>
";
			ch = Util.StringCharAt(s, 7, 33);
			Assert.True(ch.ch == 'o');
			Util.TryRemoveXmlNodeContainingChar(ref s, ch.pos, "gg");
			Assert.True(s == b);



			var c = @"<?xml version=""1.0"" encoding=""utf-8""?>
<aa>This document contains both the<inner></inner> bla</aa>
<bb>bla <inner>Interserve Construction Health and</inner> bla</bb>
<cc>bla <inner>that Interserve Construction and all subcontractors</inner> bla</cc>
<dd>bla <inner>achieve our vision of being the trusted Partner to all those </inner> bla</dd>

<ff>bla <inner> suppliers, members of the community</inner> bla</ff>

<hh>bla <inner>Subcontractors are required</inner> bla</hh>
<ii>bla <inner> to assist and co-operate with</inner> bla</ii>
<jj>bla <inner> Interserve Construction with health</inner> bla</jj>

<ll>bla <inner> issues, including initiatives</inner> bla</ll>
<mm>bla <inner> that may be</inner> bla</mm>
<nn>bla <inner>operated from time to</inner> bla</nn>
";
			ch = Util.StringCharAt(s, 5, 20);
			Assert.True(ch.ch == 'h');
			Util.TryRemoveXmlNodeContainingChar(ref s, ch.pos, "ee");
			Assert.True(s == c);

			var d = @"<?xml version=""1.0"" encoding=""utf-8""?>

<bb>bla <inner>Interserve Construction Health and</inner> bla</bb>
<cc>bla <inner>that Interserve Construction and all subcontractors</inner> bla</cc>
<dd>bla <inner>achieve our vision of being the trusted Partner to all those </inner> bla</dd>

<ff>bla <inner> suppliers, members of the community</inner> bla</ff>

<hh>bla <inner>Subcontractors are required</inner> bla</hh>
<ii>bla <inner> to assist and co-operate with</inner> bla</ii>
<jj>bla <inner> Interserve Construction with health</inner> bla</jj>

<ll>bla <inner> issues, including initiatives</inner> bla</ll>
<mm>bla <inner> that may be</inner> bla</mm>
<nn>bla <inner>operated from time to</inner> bla</nn>
";
			ch = Util.StringCharAt(s, 1, 32);
			Assert.True(ch.ch == 't');
			Util.TryRemoveXmlNodeContainingChar(ref s, ch.pos, "aa");
			Assert.True(s == d);
		}

	}
}