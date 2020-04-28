using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.Converters;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class SerializerService
	{
		public void Save<T>(T obj, string path)
		{
			var serializer = new XmlSerializer(typeof(T), new[]{
				typeof(Tag),
				typeof(Text)
			});

			using (var w = new StreamWriter(path, false, Encoding.UTF8))
			{
				serializer.Serialize(w, obj);
			}
		}

		public void SaveJson<T>(T obj, string path)
		{
			var serializer = new JsonSerializer();

			using (var w = new StreamWriter(path, false, Encoding.UTF8))
			{
				serializer.Serialize(w, obj);
			}
		}

		public string Serialize<T>(T obj)
		{
			string result;

			var settings = new XmlWriterSettings
			{
				Indent = false,
				OmitXmlDeclaration = true,
				DoNotEscapeUriAttributes = true
			};

			using (var stream = new MemoryStream())
			{
				var serializer = new XmlSerializer(typeof(T), new[]{
					typeof(Tag),
					typeof(Text)
				});
				serializer.Serialize(XmlWriter.Create(stream, settings), obj);
				stream.Flush();

				stream.Position = 0;
				var reader = new StreamReader(stream);
				result = reader.ReadToEnd();
			}

			return result;
		}

		public string Serialize<T>(T obj, XmlSerializer serializer)
		{
			string result;

			var settings = new XmlWriterSettings
			{
				Indent = false,
				OmitXmlDeclaration = true,
				DoNotEscapeUriAttributes = true
			};

			using (var stream = new MemoryStream())
			{
				serializer.Serialize(XmlWriter.Create(stream, settings), obj);
				stream.Flush();

				stream.Position = 0;
				var reader = new StreamReader(stream);
				result = reader.ReadToEnd();
			}

			return result;
		}

		public T Read<T>(string path) where T : new()
		{
			if (File.Exists(path))
			{
				var serializer = new XmlSerializer(typeof(T), new[]{
					typeof(Tag),
					typeof(Text)
				});

				object content;
				using (var w = new StreamReader(path, Encoding.UTF8))
				{
					content = serializer.Deserialize(w);
				}

				return (T)content;
			}

			return default(T);
		}

		public T ReadJson<T>(string path) where T : new()
		{
			if (File.Exists(path))
			{
				var serializer = new JsonSerializer();
				serializer.Converters.Add(new SegmentElementTypeConverter());
				serializer.Converters.Add(new FieldValueTypeConverter());

				object content;
				using (var w = new StreamReader(path, Encoding.UTF8))
				{
					content = serializer.Deserialize(w, typeof(T));
				}

				return (T)content;
			}

			return default(T);
		}

		public T Deserialize<T>(string content) where T : new()
		{
			var reader = new StringReader(content);
			var serializer = new XmlSerializer(typeof(T), new[]{
				typeof(Tag),
				typeof(Text)
			});
			var obj = serializer.Deserialize(XmlReader.Create(reader));
			var result = (T)obj;

			return result;
		}

		public T Deserialize<T>(string content, XmlSerializer serializer) where T : new()
		{
			content = DiscardIllegalXmlCharacters(content);
			var reader = new StringReader(content);
			var obj = serializer.Deserialize(XmlReader.Create(reader));
			var result = (T)obj;

			return result;
		}

		private string DiscardIllegalXmlCharacters(string content)
		{
			//all characters in range x01-x1f without CR/LF/Tab
			var illegalCharsRegex = new Regex("(?!&#x09;|&#x0a;|&#x0d;)(&#[xX]([0-9a-fA-F]){2};)");
			var illegalMatches = illegalCharsRegex.Matches(content);
			if (illegalMatches.Count > 0)
			{
				foreach (var match in illegalMatches.Cast<Match>().Reverse())
				{
					content = Replace(content, match.Index, match.Length, string.Empty);
				}
			}

			return content;
		}

		private string Replace(string input, int index, int length, string replacement)
		{
			var builder = new StringBuilder();
			builder.Append(input.Substring(0,index));
			builder.Append(replacement);
			builder.Append(input.Substring(index + length));
			return builder.ToString();
		}
	}
}
