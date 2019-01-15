using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IATETerminologyProvider.Service
{
	public class SerializerService
	{
		public void Save<T>(T obj, string path)
		{
			var serializer = new XmlSerializer(typeof(T));

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
				var serializer = new XmlSerializer(typeof(T));
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
				var serializer = new XmlSerializer(typeof(T));

				object content;
				using (var w = new StreamReader(path, Encoding.UTF8))
				{
					content = serializer.Deserialize(w);
				}

				return (T)content;
			}

			return default(T);
		}

		public T Deserialize<T>(string content) where T : new()
		{
			var reader = new StringReader(content);
			var serializer = new XmlSerializer(typeof(T));
			var obj = serializer.Deserialize(XmlReader.Create(reader));
			var result = (T)obj;

			return result;
		}

		public T Deserialize<T>(string content, XmlSerializer serializer) where T : new()
		{
			var reader = new StringReader(content);
			var obj = serializer.Deserialize(XmlReader.Create(reader));
			var result = (T)obj;

			return result;
		}
	}
}
