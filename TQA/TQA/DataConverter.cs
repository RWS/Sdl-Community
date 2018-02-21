using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ClosedXML.Excel;
using Sdl.Community.TQA.Model;

namespace Sdl.Community.TQA
{
    public static class DataConverter
    {
	    public static ReportResults ExtractFromXml(string path)
	    {
		    var report = XDocument.Load(path);
		    var languages = report.Descendants("language");
		    var reportResults = new ReportResults
		    {
			    Entries = new List<Entry>(),
			    EvaluationComments = new List<string>()
		    };
		    foreach (var language in languages)
		    {
			    var languageString = language.Attribute("name").Value;
			    var files = language.Descendants("file");

			    foreach (var file in files)
			    {
				    var fileString = file.Attribute("name").Value;
				    var evaluationComment = file.Attribute("evaluationComment").Value;
				    if (!string.IsNullOrEmpty(evaluationComment))
				    {
						reportResults.EvaluationComments.Add(evaluationComment);
					}
					var segments = file.Descendants("segment");

					foreach (var segment in segments)
				    {
					    var segmentId = segment.Attribute("id").Value;
					    var sourceContent = segment.Element("sourceContent");
					    var sourceContentText = new List<Tuple<string, TextType>>();
					    foreach (var group in sourceContent.Descendants("item"))
					    {
						    switch (group.Attribute("type").Value)
						    {
							    case "":
								    sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
								    break;
							    case "FeedbackAdded":
								    if (sourceContent.Elements().Contains(group))
									    sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Added));
								    else
									    sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
								    break;
							    case "FeedbackDeleted":
								    if (sourceContent.Elements().Contains(group))
									    sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Deleted));
								    else
									    sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
								    break;
							    case "FeedbackComment":
								    sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Comment));
								    break;
						    }
					    }

					    var originalTranslation = segment.Attribute("originalTranslation").Value;
					    var revisedTranslations = segment.Element("revisedTranslation").Elements()
						    .Where(e => e.Name == "group" && (e.Attribute("category") != null || e.Attribute("severity") != null));

					    foreach (XElement revisedTranslation in revisedTranslations)
					    {
						    var translation = revisedTranslation.Element("item").Attribute("content").Value;
						    var category = revisedTranslation.Attribute("category").Value;
						    var severity = revisedTranslation.Attribute("severity").Value;
						    var comment = revisedTranslation.Attribute("comment").Value;
						   
						    var revisedTranslationText = new List<Tuple<string, TextType>>();
						    foreach (XElement rTrans in segment.Element("revisedTranslation").Descendants().Where(e => e.Name == "item"))
						    {
							    switch (rTrans.Attribute("type").Value)
							    {
								    case "":
									    revisedTranslationText.Add(
										    new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
									    break;
								    case "FeedbackAdded":
									    if (revisedTranslation.Elements().Contains(rTrans))
										    revisedTranslationText.Add(
											    new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Added));
									    else
										    revisedTranslationText.Add(
											    new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
									    break;
								    case "FeedbackDeleted":
									    if (revisedTranslation.Elements().Contains(rTrans))
										    revisedTranslationText.Add(
											    new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Deleted));
									    else
										    revisedTranslationText.Add(
											    new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
									    break;
								    case "FeedbackComment":
									    revisedTranslationText.Add(
										    new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Comment));
									    break;
							    }
						    }
							   var entry = new Entry(languageString, fileString, segmentId, originalTranslation, revisedTranslationText,
						     sourceContentText, category, severity, comment, translation);
							reportResults.Entries.Add(entry);
						}
					}
			    }
		    }
		    return reportResults;
	    }

	    public static void WriteExcel(string path, ReportResults reportResults)
        {
            using( var fs = new FileStream( path, FileMode.Create ) )
            {
                fs.Write( PluginResources.template, 0, PluginResources.template.Length );
            }
	        var rows = reportResults.Entries;
            var rowsArray = rows.ToArray();
            var rowsCollection = rows.Select( r => r.GetArray() ).ToArray();
            using( var wb = new XLWorkbook( path ) )
            {
                using( var ws = wb.Worksheet( "Evaluation details_input" ) )
                {
                    for( var i = 0; i < rows.Count; i++ )
                    {
                        for(var j = 0; j < rowsCollection[i].Length; j++ )
                        {
                            ws.Row( i + 4 ).Cell( j + 1 ).Value = rowsCollection[i][j];
                        }
                        var cell = ws.Cell( i + 4, 5 );

                        var entry = rowsArray[i].RevisedTranslation;

                        for(var k = 0; k < entry.Count; k++ )
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
                        for(var k = 0; k < entry.Count; k++ )
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

                using(var ws = wb.Worksheet( "Evaluation Report_Initial" ) )
                {
                    ws.Cell( "B9" ).Value = DateTime.Now.ToString( "dd-MMM-yyyy" );
	                ws.Cell("L8").Value = "TQA";
	                for (var i = 0; i < reportResults.EvaluationComments.Count; i++)
	                {
		                ws.Row(i + 42).Cell(1).Value = reportResults.EvaluationComments[i];

	                }
				}
                wb.CalculateMode = XLCalculateMode.Auto;
                wb.Save();
            }
        }
    }
}
