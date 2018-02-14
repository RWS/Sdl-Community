using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace TQA
{
    public static class DataConverter
    {
        public static IEnumerable<Entry> ExtractFromXml(string path)
        {
            XDocument report = XDocument.Load( path );

            var languages = report.Descendants( "language" );

            foreach( XElement language in languages )
            {
                string languageString = language.Attribute( "name" ).Value;

                var files = language.Descendants( "file" );

                foreach( XElement file in files )
                {
                    string fileString = file.Attribute( "name" ).Value;

                    var segments = file.Descendants( "segment" );

                    foreach( XElement segment in segments )
                    {
                        string segmentID = segment.Attribute( "id" ).Value;

                        var TQAs = segment.Elements( "tqa" );
                        var sourceContent =  segment.Element( "sourceContent" );

                        List<Tuple<string, TextType>> sourceContentText = new List<Tuple<string, TextType>>();
                        foreach( XElement group in sourceContent.Descendants("item") )
                        {
                            switch( group.Attribute( "type" ).Value )
                            {
                                case "":
                                    sourceContentText.Add( new Tuple<string, TextType>( group.Attribute( "content" ).Value, TextType.Regular ) );
                                    break;
                                case "FeedbackAdded":
                                    if( sourceContent.Elements().Contains( group ) )
                                        sourceContentText.Add( new Tuple<string, TextType>( group.Attribute( "content" ).Value, TextType.Added ) );
                                    else
                                        sourceContentText.Add( new Tuple<string, TextType>( group.Attribute( "content" ).Value, TextType.Regular ) );
                                    break;
                                case "FeedbackDeleted":
                                    if( sourceContent.Elements().Contains( group ) )
                                        sourceContentText.Add( new Tuple<string, TextType>( group.Attribute( "content" ).Value, TextType.Deleted ) );
                                    else
                                        sourceContentText.Add( new Tuple<string, TextType>( group.Attribute( "content" ).Value, TextType.Regular ) );
                                    break;
                                case "FeedbackComment":
                                    sourceContentText.Add( new Tuple<string, TextType>( group.Attribute( "content" ).Value, TextType.Comment ) );
                                    break;
                            }
                        }




                        var originalTranslation = segment.Attribute( "originalTranslation" ).Value;

                        var revisedTranslations = segment.Element( "revisedTranslation" ).Elements().Where( e => e.Name == "group" && ( e.Attribute( "category" ) != null || e.Attribute( "severity" ) != null ) );





                        foreach( XElement revisedTranslation in revisedTranslations )
                        {
                            var translation = revisedTranslation.Element( "item" ).Attribute( "content" ).Value;
                            var category = revisedTranslation.Attribute( "category" ).Value;
                            var severity = revisedTranslation.Attribute( "severity" ).Value;
                            var comment = revisedTranslation.Attribute( "comment" ).Value;

                            List<Tuple<string, TextType>> revisedTranslationText = new List<Tuple<string, TextType>>();
                            foreach( XElement rTrans in segment.Element( "revisedTranslation" ).Descendants().Where( e => e.Name == "item" ) )
                            {
                                switch( rTrans.Attribute( "type" ).Value )
                                {
                                    case "":
                                        revisedTranslationText.Add( new Tuple<string, TextType>( rTrans.Attribute( "content" ).Value, TextType.Regular ) );
                                        break;
                                    case "FeedbackAdded":
                                        if( revisedTranslation.Elements().Contains( rTrans ) )
                                            revisedTranslationText.Add( new Tuple<string, TextType>( rTrans.Attribute( "content" ).Value, TextType.Added ) );
                                        else
                                            revisedTranslationText.Add( new Tuple<string, TextType>( rTrans.Attribute( "content" ).Value, TextType.Regular ) );
                                        break;
                                    case "FeedbackDeleted":
                                        if( revisedTranslation.Elements().Contains( rTrans ) )
                                            revisedTranslationText.Add( new Tuple<string, TextType>( rTrans.Attribute( "content" ).Value, TextType.Deleted ) );
                                        else
                                            revisedTranslationText.Add( new Tuple<string, TextType>( rTrans.Attribute( "content" ).Value, TextType.Regular ) );
                                        break;
                                    case "FeedbackComment":
                                        revisedTranslationText.Add( new Tuple<string, TextType>( rTrans.Attribute( "content" ).Value, TextType.Comment ) );
                                        break;
                                }
                            }

                            yield return new Entry( languageString, fileString, segmentID, originalTranslation, revisedTranslationText, sourceContentText, category, severity, comment, translation );
                        }
                    }
                }
            }
        }

        public static void WriteExcel(string path, IEnumerable<Entry> rows)
        {
            using( FileStream fs = new FileStream( path, FileMode.Create ) )
            {
                fs.Write( PluginResources.template, 0, PluginResources.template.Count() );
            }

            var rowsArray = rows.ToArray();
            var rowsCollection = rows.Select( r => r.GetArray() ).ToArray();
            using( XLWorkbook wb = new XLWorkbook( path ) )
            {
                using( IXLWorksheet ws = wb.Worksheet( "Evaluation details_input" ) )
                {
                    for( int i = 0; i < rows.Count(); i++ )
                    {
                        for( int j = 0; j < rowsCollection[i].Count(); j++ )
                        {
                            ws.Row( i + 4 ).Cell( j + 1 ).Value = rowsCollection[i][j];
                        }
                        var cell = ws.Cell( i + 4, 5 );

                        var entry = rowsArray[i].RevisedTranslation;

                        for( int k = 0; k < entry.Count; k++ )
                        {
                            cell.RichText.AddText( entry[k].Item1 );
                            switch( entry[k].Item2 )
                            {
                                case TextType.Added:
                                    cell.RichText.ToArray()[k].SetFontColor( XLColor.GreenPigment );
                                    cell.RichText.ToArray()[k].SetUnderline();
                                    break;
                                case TextType.Deleted:
                                    cell.RichText.ToArray()[k].SetFontColor( XLColor.Red );
                                    cell.RichText.ToArray()[k].SetStrikethrough( true );
                                    break;
                                case TextType.Regular:
                                    continue;
                                case TextType.Comment:
                                    cell.RichText.ToArray()[k].SetFontColor( XLColor.Blue);
                                    cell.RichText.ToArray()[k].SetBold();
                                    break;


                            }
                        }

                        cell = ws.Cell( i + 4, 3 );

                        entry = rowsArray[i].SourceContent;
                        for( int k = 0; k < entry.Count; k++ )
                        {
                            cell.RichText.AddText( entry[k].Item1 );
                            switch( entry[k].Item2 )
                            {
                                case TextType.Added:
                                    cell.RichText.ToArray()[k].SetFontColor( XLColor.GreenPigment );
                                    cell.RichText.ToArray()[k].SetUnderline();
                                    break;
                                case TextType.Deleted:
                                    cell.RichText.ToArray()[k].SetFontColor( XLColor.Red );
                                    cell.RichText.ToArray()[k].SetStrikethrough( true );
                                    break;
                                case TextType.Regular:
                                    continue;
                                case TextType.Comment:
                                    cell.RichText.ToArray()[k].SetFontColor( XLColor.Blue );
                                    cell.RichText.ToArray()[k].SetBold();
                                    break;


                            }
                        }

                    }

                }

                using( IXLWorksheet ws = wb.Worksheet( "Evaluation Report_Initial" ) )
                {
                    ws.Cell( "B8" ).Value = DateTime.Now.ToString( "dd-MMM-yyyy" );
                }

                wb.CalculateMode = XLCalculateMode.Auto;

                wb.Save();
            }
        }



    }
}
