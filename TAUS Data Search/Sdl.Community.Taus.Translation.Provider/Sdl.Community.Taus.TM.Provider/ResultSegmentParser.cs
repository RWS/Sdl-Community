using System;
using System.Collections.Generic;
using System.Xml;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider
{
    internal class ResultSegmentParser
    {
        internal SearchSegmentResult ReadResult(string xmlFragment)
        {

            var searchSegmentResult = new SearchSegmentResult();


            var textReader = new XmlTextReader(xmlFragment, XmlNodeType.Element, null);
           
            try
            {
                while (textReader.Read())
                {
                    switch (textReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(textReader.Name, "result", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                var readSubtree = textReader.ReadSubtree();
                                searchSegmentResult = ReadResult(readSubtree);
                                readSubtree.Close();
                            }
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            //not required for this implementation
                            break;
                        case XmlNodeType.Comment:
                            //not required for this implementation
                            break;
                        case XmlNodeType.XmlDeclaration:
                            //not required for this implementation
                            break;
                        case XmlNodeType.Document:
                            //not required for this implementation
                            break;
                        case XmlNodeType.DocumentType:
                            //not required for this implementation
                            break;
                        case XmlNodeType.EndElement:
                            //not required for this implementation
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (textReader.ReadState != ReadState.Closed)
                    textReader.Close();
            }
            return searchSegmentResult;
        }

        private static SearchSegmentResult ReadResult(XmlReader xmlReader)
        {
            var searchSegmentResult = new SearchSegmentResult {Segments = new List<Segment.Segment>()};

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "status", StringComparison.OrdinalIgnoreCase) == 0)
                        {                          
                            var readSubtree = xmlReader.ReadSubtree();
                            searchSegmentResult.Status = ReadText(readSubtree);                            
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "auth_key", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            searchSegmentResult.AuthKey = ReadAuthKey(readSubtree);
                            readSubtree.Close();
                        }

                        else if (string.Compare(xmlReader.Name, "reason", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            searchSegmentResult.Reason = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "segment", StringComparison.OrdinalIgnoreCase) == 0)
                        {                           
                            var readSubtree = xmlReader.ReadSubtree();
                            var segment = ReadSegment(readSubtree);
                            searchSegmentResult.Segments.Add(segment);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return searchSegmentResult;

        }

        private static AuthKey ReadAuthKey(XmlReader xmlReader)
        {
            var authKey = new AuthKey();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            authKey.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "manage_url", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            authKey.ManageUrl = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return authKey;

        }

        private static Segment.Segment ReadSegment(XmlReader xmlReader)
        {
            var segment = new Segment.Segment();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "provider", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.Provider = ReadProvider(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "owner", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.Owner = ReadOwner(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "industry", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.Industry = ReadIndustry(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "content_type", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.ContentType = ReadContentType(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "provider", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.Provider = ReadProvider(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "product", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.Product = ReadProduct(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "source_lang", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.SourceLanguage = ReadLanguage(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "target_lang", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.TargetLanguage = ReadLanguage(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "source", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.SourceText = ReadText(readSubtree);
                            segment.SourceSections.Add(new SegmentSection(true, segment.SourceText));
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "target", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.TargetText = ReadText(readSubtree);
                            segment.TargetSections.Add(new SegmentSection(true, segment.TargetText));
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            segment.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return segment;

        }
        
        private static Segment.Provider ReadProvider(XmlReader xmlReader)
        {


            var provider = new Segment.Provider();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            provider.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            provider.Name = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return provider;

        }
        private static Owner ReadOwner(XmlReader xmlReader)
        {


            var owner = new Owner();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            owner.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            owner.Name = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return owner;

        }
        private static Industry ReadIndustry(XmlReader xmlReader)
        {


            var industry = new Industry();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            industry.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            industry.Name = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return industry;

        }
        private static ContentType ReadContentType(XmlReader xmlReader)
        {


            var contentType = new ContentType();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            contentType.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            contentType.Name = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return contentType;

        }
        private static Product ReadProduct(XmlReader xmlReader)
        {
            var product = new Product();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            product.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            product.Name = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return product;

        }
        private static Language ReadLanguage(XmlReader xmlReader)
        {
            var language = new Language();

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(xmlReader.Name, "id", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            language.Id = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        else if (string.Compare(xmlReader.Name, "name", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            var readSubtree = xmlReader.ReadSubtree();
                            language.Name = ReadText(readSubtree);
                            readSubtree.Close();
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Text:
                        //not required for this implementation
                        break;
                    case XmlNodeType.CDATA:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EntityReference:
                        //not required for this implementation
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return language;

        }
   
        private static string ReadText(XmlReader xmlReaderInner)
        {
        
           
            var value = "";

            while (xmlReaderInner.Read())
            {
                switch (xmlReaderInner.NodeType)
                {
                    case XmlNodeType.Element:
                        //not required for this implementation
                        break;
                    case XmlNodeType.Whitespace:
                        value += xmlReaderInner.Value;
                        break;
                    case XmlNodeType.Text:
                        value += xmlReaderInner.Value;
                        break;
                    case XmlNodeType.CDATA:
                        value += xmlReaderInner.Value;                        
                        break;
                    case XmlNodeType.EntityReference:
                        value += xmlReaderInner.Name;
                        break;
                    case XmlNodeType.EndElement:
                        //not required for this implementation
                        break;
                }
            }
            return value;

        }
    }
}
