using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.Projects;

namespace Sdl.Community.InvoiceAndQuotes.OpenXML.Word
{
    class WordHelperBase
    {
        // Updates a SpreadsheetDocument.
        public void UpdatePackage(string templateFile, string filePath, List<ProjectFile> projectFiles, Customer customer, User user)
        {
            File.Copy(templateFile, filePath, true);
            Thread.Sleep(100);
            using (WordprocessingDocument package = WordprocessingDocument.Open(filePath, true, new OpenSettings() { AutoSave = true }))
            {
                UpdateParts(package, projectFiles, customer, user);
            }
        }

        private void UpdateParts(WordprocessingDocument document, List<ProjectFile> projectFiles, Customer customer,
                                 User user)
        {
            try
            {
                OpenXmlElement root = document.MainDocumentPart.Document.Body;
                UpdateElementsForRoot(projectFiles, customer, user, root);
            }
            catch {}

            try
            {
                foreach (FooterPart footerPart in document.MainDocumentPart.FooterParts)
                {
                    OpenXmlElement root = footerPart.Footer;
                    UpdateElementsForRoot(customer, user, root);
                }
            }
            catch { }

            try
            {
                foreach (HeaderPart headerPart in document.MainDocumentPart.HeaderParts)
                {
                    OpenXmlElement root = headerPart.Header;
                    UpdateElementsForRoot(customer, user, root);
                }
            }
            catch { }

            #region comments part
            //try
            //{
            //    if (document.MainDocumentPart.WordprocessingCommentsPart == null) return;
            //    OpenXmlElement root = document.MainDocumentPart.WordprocessingCommentsPart.Comments;
            //    UpdateElementsForRoot(projectFiles, customer, user, root);
            //}
            //catch { }
            #endregion
        }

        private void UpdateElementsForRoot(Customer customer, User user, OpenXmlElement root)
        {
            List<OpenXmlElement> elements = InitializeRoot(root, false);
            UpdateElements(root, elements, customer, user);
        }

        private List<OpenXmlElement> InitializeRoot(OpenXmlElement root, bool removeOldOnes = true)
        {
            var initialElements = root.ChildElements;
            if (removeOldOnes)
            {
                if (initialElements == null || initialElements.Count <= 0) return new List<OpenXmlElement>();
                List<OpenXmlElement> elements =
                    initialElements.Select(initialElement => (OpenXmlElement) initialElement.Clone()).ToList();
                root.RemoveAllChildren();
                return elements;
            }
            return initialElements.ToList();
        }

        private void UpdateElementsForRoot(IEnumerable<ProjectFile> projectFiles, Customer customer, User user, OpenXmlElement root)
        {
            List<OpenXmlElement> elements = InitializeRoot(root);
            UpdateElements(root, elements, projectFiles, customer, user);
        }

        private void UpdateElements(OpenXmlElement root, List<OpenXmlElement> elements, IEnumerable<ProjectFile> projectFiles, Customer customer, User user)
        {
            foreach (var projectFile in projectFiles)
            {
                UpdateSingleFile(root, elements, new List<TokensProvider>() {customer, user, projectFile});

                #region empty lines
                var emptyLineParagraph = new Paragraph() { RsidParagraphAddition = "0077013B", RsidRunAdditionDefault = "00684C81", ParagraphId = "5E06053C", TextId = "77777777" };
                var emptyLineRun = new Run();
                var emptyLineText = new Text() { Text = String.Empty };
                emptyLineRun.Append(emptyLineText);
                emptyLineParagraph.Append(emptyLineRun);
                root.Append(emptyLineParagraph); 
                root.Append((OpenXmlElement)emptyLineParagraph.Clone());
                #endregion
            }
        }

        private void UpdateSingleFile(OpenXmlElement root, List<OpenXmlElement> elements, List<TokensProvider> objectsWithTokens, bool removeOldOnes = true)
        {
            var newElements = !removeOldOnes?elements.ToList() : elements.Select(element => (OpenXmlElement) element.Clone()).ToList();
            var childElements = new List<OpenXmlElement>();

            var paragraphs = newElements.OfType<Paragraph>();
            foreach (var paragraph in paragraphs)
                childElements.AddRange(paragraph.ChildElements);

            var tables = newElements.OfType<Table>();
            foreach (var table in tables)
                childElements.AddRange(table.ChildElements);

            UpdateChildElements(childElements, objectsWithTokens);
            if (removeOldOnes)
                root.Append(newElements);
        }

        private void UpdateElements(OpenXmlElement root, List<OpenXmlElement> elements, Customer customer, User user)
        {
            UpdateSingleFile(root, elements, new List<TokensProvider>() { customer, user }, false);
        }

        private void UpdateChildElements(List<OpenXmlElement> childElements, List<TokensProvider> objectsWithTokens)
        {
            if (childElements.All(element => element.ChildElements == null || element.ChildElements.Count == 0))
                return;
            foreach (var element in childElements)
            {
                if (element.ChildElements == null || element.ChildElements.Count == 0)
                    continue;
                IEnumerable<Text> textElements = element.ChildElements.OfType<Text>();
                if (textElements.Any())
                    UpdateTextElements(textElements, objectsWithTokens);

                UpdateChildElements(element.ChildElements.ToList(), objectsWithTokens);
            }
        }

        private void UpdateTextElements(IEnumerable<Text> textElements, List<TokensProvider> objectsWithTokens)
        {
            foreach (var textElement in textElements)
            {
                foreach (var objectsWithToken in objectsWithTokens)
                {
                    if (objectsWithToken == null) continue;
                    IEnumerable<Token> tokens = objectsWithToken.GetAllTokensFromString(textElement.Text);
                    textElement.Text = objectsWithToken.ReplaceTokensInString(textElement.Text, tokens);
                }
            }
        }
    }
}
