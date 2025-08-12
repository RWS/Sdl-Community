using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Multilingual.XML.FileType.Extensions
{
    public static class XMLDocumentExtensions
    {
        public static void AddAllNamespaces(this XmlDocument doc, XmlNamespaceManager nsmgr)
        {
            var root = doc.DocumentElement;
            if (root == null) return;

            foreach (XmlAttribute attr in root.Attributes)
            {
                if (attr.Prefix == "xmlns")
                {
                    nsmgr.AddNamespace(attr.LocalName, attr.Value);
                }
                else if (attr.Name == "xmlns")
                {
                    nsmgr.AddNamespace(string.Empty, attr.Value); // default namespace
                }
            }
        }

        public static XmlNode SafeSelectSingleNode(this XmlNode node, string path, XmlNamespaceManager nsmgr)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            if (path.TrimStart().StartsWith("["))
                path = "(.)" + path;  // parentheses make it valid for .NET XPath

            return node.SelectSingleNode(path, nsmgr);
        }
    }
}
