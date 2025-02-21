using System.Xml.Linq;

namespace InterpretBank.GlossaryExchangeService.Wrappers.Interface;

public interface IXmlSerializerWrapper<T>
{
	T Deserialize(string path);
	T Deserialize(XElement element);
	void Serialize(string path, T tbx);
}