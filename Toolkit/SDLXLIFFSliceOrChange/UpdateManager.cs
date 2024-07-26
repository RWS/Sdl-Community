﻿using System;
using System.Reflection;
using System.Xml;
using NLog;
using Sdl.Core.Globalization;

namespace SDLXLIFFSliceOrChange
{
	public class UpdateManager
    {
        private SDLXLIFFSliceOrChange _sdlxliffSliceOrChange;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public UpdateManager(SDLXLIFFSliceOrChange sdlxliffSliceOrChange)
        {
            _sdlxliffSliceOrChange = sdlxliffSliceOrChange;
        }

        public bool DoUpdateElementBasedOnSystemInformation(object segment)
        {
            var doUpdateElement = false;
            var originValue = string.Empty;
            if (((XmlElement)segment).HasAttribute("origin"))
            {
                originValue = ((XmlElement)segment).Attributes["origin"].Value;
            }

            switch (originValue)
            {
                case "tm": //translation memory
                    doUpdateElement = _sdlxliffSliceOrChange.ckSystemTranslationMemory.Checked;
                    break;
                case "mt": //automated translation
                    doUpdateElement = _sdlxliffSliceOrChange.ckSystemMachineTranslation.Checked;
                    break;
                case "auto-propagated": //auto propagated
                    doUpdateElement = _sdlxliffSliceOrChange.ckAutoPropagated.Checked;
                    break;
                case "": //it does not have origin attribute or it's out of our purpose
                default:
                    break;
            }

            if (!doUpdateElement)
            {
                var prevSegs = ((XmlElement) segment).GetElementsByTagName("sdl:prev-origin");
                if (prevSegs.Count == 0) return doUpdateElement;
                var prevSeg = (XmlElement) prevSegs[0];

                if (prevSeg.HasAttribute("origin"))
                {
                    originValue = prevSeg.Attributes["origin"].Value;
                }

                switch (originValue)
                {
                    case "tm": //translation memory
                        doUpdateElement = _sdlxliffSliceOrChange.ckSystemTranslationMemory.Checked;
                        break;
                    case "mt": //automated translation
                        doUpdateElement = _sdlxliffSliceOrChange.ckSystemMachineTranslation.Checked;
                        break;
                    case "auto-propagated": //auto propagated
                        doUpdateElement = _sdlxliffSliceOrChange.ckAutoPropagated.Checked;
                        break;
                    case "": //it does not have origin attribute or it's out of our purpose
                    default:
                        break;
                }
            }
            return doUpdateElement;
        }

        public bool DoUpdateElementBasedOnScoreInformation(object segment)
        {
	        var doUpdateElement = true;
            var expression = RemoveSpaces(_sdlxliffSliceOrChange.txtMatchValuesExpression.Text);
            if (_sdlxliffSliceOrChange.ckMatchValues.Checked && !string.IsNullOrEmpty(expression))
            {
                decimal percentValue = 0;
                if (((XmlElement)segment).HasAttribute("percent"))
                {
	                var textMatch = string.Empty;
                    if (((XmlElement)segment).HasAttribute("text-match"))
                        textMatch = ((XmlElement)segment).Attributes["text-match"].Value;

                    try
                    {
                        percentValue = Convert.ToDecimal(((XmlElement)segment).Attributes["percent"].Value);
                    }
                    catch (Exception ex)
                    {
						_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
					}

                    doUpdateElement = EvaluateExpression(expression, percentValue);
                    if (percentValue == 100 && (textMatch == "Source" || textMatch == "SourceAndTarget")) doUpdateElement = false;
                }
                else
                {
                    doUpdateElement = false;
                }

                if (percentValue == 0 && expression.IndexOf("=0", StringComparison.Ordinal) != -1)
                    doUpdateElement = EvaluateExpression(expression, percentValue);
            }

            if (_sdlxliffSliceOrChange.ckPerfectMatch.Checked)
            {
                var percent = string.Empty;
                var structMatch = string.Empty;
                var textMatch = string.Empty;
                if (((XmlElement) segment).HasAttribute("percent"))
                    percent = ((XmlElement)segment).Attributes["percent"].Value;
                if (((XmlElement) segment).HasAttribute("struct-match"))
                    structMatch = ((XmlElement)segment).Attributes["struct-match"].Value;
                if (((XmlElement) segment).HasAttribute("text-match"))
                    textMatch = ((XmlElement)segment).Attributes["text-match"].Value;
                //look for perfect match and system
                doUpdateElement = percent == "100" && structMatch == "true" && textMatch == "Source";
            }

            if (_sdlxliffSliceOrChange.ckContextMatch.Checked)
            {
	            var percent = string.Empty;
	            var structMatch = string.Empty;
	            var textMatch = string.Empty;
	            var conf = string.Empty;
                if (((XmlElement)segment).HasAttribute("percent"))
                    percent = ((XmlElement)segment).Attributes["percent"].Value;
                if (((XmlElement)segment).HasAttribute("struct-match"))
                    structMatch = ((XmlElement)segment).Attributes["struct-match"].Value;
                if (((XmlElement)segment).HasAttribute("conf"))
                    conf = ((XmlElement)segment).Attributes["conf"].Value;
                if (((XmlElement)segment).HasAttribute("text-match"))
                    textMatch = ((XmlElement)segment).Attributes["text-match"].Value;
                //look for perfect match and system
                doUpdateElement = percent == "100" && (structMatch == "true" || conf == "Translated") && textMatch == "SourceAndTarget";
            }

            if (!doUpdateElement)
            {
	            var prevSegs = ((XmlElement) segment).GetElementsByTagName("sdl:prev-origin");
                if (prevSegs.Count == 0) return doUpdateElement;
                var prevSeg = (XmlElement) prevSegs[0];

                if (_sdlxliffSliceOrChange.ckMatchValues.Checked && !string.IsNullOrEmpty(_sdlxliffSliceOrChange.txtMatchValuesExpression.Text))
                {
                    decimal percentValue = 0;
                    if (prevSeg.HasAttribute("percent"))
                    {
	                    var textMatch = string.Empty;
                        if (prevSeg.HasAttribute("text-match"))
                            textMatch = prevSeg.Attributes["text-match"].Value;

                        try
                        {
                            percentValue = Convert.ToDecimal(prevSeg.Attributes["percent"].Value);
                        }
                        catch (Exception ex)
                        {
							_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
						}

                        doUpdateElement = EvaluateExpression(_sdlxliffSliceOrChange.txtMatchValuesExpression.Text, percentValue);
                        if (percentValue == 100 && (textMatch == "Source" || textMatch == "SourceAndTarget")) doUpdateElement = false;
                    }
                }

                if (_sdlxliffSliceOrChange.ckPerfectMatch.Checked)
                {
	                var percent = string.Empty;
	                var structMatch = string.Empty;
	                var textMatch = string.Empty;
                    if (prevSeg.HasAttribute("percent"))
                        percent = prevSeg.Attributes["percent"].Value;
                    if (prevSeg.HasAttribute("struct-match"))
                        structMatch = prevSeg.Attributes["struct-match"].Value;
                    if (prevSeg.HasAttribute("text-match"))
                        textMatch = prevSeg.Attributes["text-match"].Value;
                    //look for perfect match and system
                    doUpdateElement = percent == "100" && structMatch == "true" && textMatch == "Source";
                }

                if (_sdlxliffSliceOrChange.ckContextMatch.Checked)
                {
	                var percent = string.Empty;
	                var structMatch = string.Empty;
	                var textMatch = string.Empty;
                    if (prevSeg.HasAttribute("percent"))
                        percent = prevSeg.Attributes["percent"].Value;
                    if (prevSeg.HasAttribute("struct-match"))
                        structMatch = prevSeg.Attributes["struct-match"].Value;
                    if (prevSeg.HasAttribute("text-match"))
                        textMatch = prevSeg.Attributes["text-match"].Value;
                    //look for perfect match and system
                    doUpdateElement = percent == "100" && structMatch == "true" && textMatch == "SourceAndTarget";
                }
            }
            return doUpdateElement;
        }

        private string RemoveSpaces(string text)
        {
            if (text.IndexOf(" ", StringComparison.Ordinal) == -1) return text;
            do
            {
                text = text.Replace(" ", "");
            } while (text.IndexOf(" ", StringComparison.Ordinal) != -1);
            return text;
        }

        private bool EvaluateExpression(string expression, decimal percentValue)
        {
            //Relational operators: = != < > <= >=
            //Logical Operators:  AND OR && ||
            //Brackets: ( )            
            try
            {
                // remove empty spaces 
                while (expression.IndexOf(" ", StringComparison.Ordinal) != -1)
                {
                    expression = expression.Replace(" ", "");
                }
                //add percent value to the expression
                expression = expression.Replace("<", "{0}<").Replace(">", "{0}>").Replace("!", "{0}!").Replace("=", "{0}=");
                expression = expression.Replace("AND", "&&").Replace("OR", "||");
                expression = expression.Replace("!{0}=", "!="); expression = expression.Replace("<{0}=", "<="); expression = expression.Replace(">{0}=", ">=");
                expression = expression.Replace("{0}=", "{0}==");
                expression = string.Format(expression, percentValue);
                var p = new ExpressionEvaluator.CompiledExpression(expression);
                p.Parse();
                p.Compile();
                return (bool) p.Eval();
            }
            catch (Exception ex)
            {
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
				return false;
            }
        }

        public bool DoUpdateElementBasedOnTranslationOriginInformation(object segment)
        {
            var doUpdateElement = false;
            var originValue = string.Empty;
            if (((XmlElement) segment).HasAttribute("origin"))
            {
                originValue = ((XmlElement) segment).Attributes["origin"].Value;
            }

            switch (originValue)
            {
                case "tm": //translation memory
                    doUpdateElement = _sdlxliffSliceOrChange.ckTranslationMemory.Checked;
                    break;
                case "mt": //automated translation
                    doUpdateElement = _sdlxliffSliceOrChange.ckAutomatedTranslation.Checked;
                    break;
                case "interactive": //interactive
                    doUpdateElement = _sdlxliffSliceOrChange.ckInteractive.Checked;
                    break;
                case "auto-propagated": //auto propagated
                    doUpdateElement = _sdlxliffSliceOrChange.ckAutoPropagated.Checked;
                    break;
                case "nmt": //neural machine translation
	                doUpdateElement = _sdlxliffSliceOrChange.ckNeuralMachineTranslation.Checked;
	                break;
                case "": //it does not have origin attribute or it's out of our purpose
                default:
                    break;
            }

            if (!doUpdateElement)
            {
	            var prevSegs = ((XmlElement) segment).GetElementsByTagName("sdl:prev-origin");
                if (prevSegs.Count == 0) return doUpdateElement;
                var prevSeg = (XmlElement) prevSegs[0];

                if (prevSeg.HasAttribute("origin"))
                {
                    originValue = prevSeg.Attributes["origin"].Value;
                }

                switch (originValue)
                {
                    case "tm": //translation memory
                        doUpdateElement = _sdlxliffSliceOrChange.ckTranslationMemory.Checked;
                        break;
                    case "mt": //automated translation
                        doUpdateElement = _sdlxliffSliceOrChange.ckAutomatedTranslation.Checked;
                        break;
                    case "interactive": //interactive
                        doUpdateElement = _sdlxliffSliceOrChange.ckInteractive.Checked;
                        break;
                    case "auto-propagated": //auto propagated
                        doUpdateElement = _sdlxliffSliceOrChange.ckAutoPropagated.Checked;
                        break;
                    case "nmt": //neural machine translation
	                    doUpdateElement = _sdlxliffSliceOrChange.ckNeuralMachineTranslation.Checked;
						break;
                    case "": //it does not have origin attribute or it's out of our purpose
                    default:
                        break;
                }
            }

            return doUpdateElement;
        }

        public bool DoUpdateElementBasedOnLockedInformation(object segment)
        {
	        var doUpdateElement = true;
            if (_sdlxliffSliceOrChange.ckLocked.Checked != _sdlxliffSliceOrChange.ckUnlocked.Checked)
            {
                if (((XmlElement) segment).HasAttribute("locked"))
                {
	                var lockedValue = ((XmlElement) segment).Attributes["locked"].Value;
                    if (lockedValue == "true" && !_sdlxliffSliceOrChange.ckLocked.Checked || lockedValue == "false" && _sdlxliffSliceOrChange.ckLocked.Checked)
                        doUpdateElement = false;
                }
                else
                {
                    if (_sdlxliffSliceOrChange.ckLocked.Checked)
                        doUpdateElement = false;
                }
            }
            return doUpdateElement;
        }

        public bool DoUpdateElementBasedOnTranslatioStatus(object segment)
        {
	        var doUpdateElement = true;
	        var translationStatuses = _sdlxliffSliceOrChange.GetTranslationStatusForSearch();
            if (translationStatuses.Count > 0)
            {
                if (((XmlElement) segment).HasAttribute("conf"))
                {
	                var confStatus = ((XmlElement) segment).Attributes["conf"].Value;
                    if (!translationStatuses.Contains(confStatus))
                        doUpdateElement = false;
                }
                else
                {
                    if (!translationStatuses.Contains(ConfirmationLevel.Unspecified.ToString()))
                        doUpdateElement = false;
                }
            }
            return doUpdateElement;
        }
    }
}