using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using InterpretBank.GlossaryExchangeService.Wrappers.Model;

namespace InterpretBank.GlossaryExchangeService.Wrappers.Interface
{
	public interface IXmlReaderWriterWrapper
	{
		XmlWriter CreateTbx(string path, string glossaryName, string subGlossaryName);
		IEnumerable<XElement> GetTermElements(string path);
	}
}