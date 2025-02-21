using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Sdl.Community.InvoiceAndQuotes.OpenXML;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes.Projects
{
    public class ProjectFile: TokensProvider
    {
        private decimal  _linesByKeyStrokes;
        private decimal _linesByCharacters;
        public XPathNavigator XMLNode { get; set; }
        public String AnalyseFile { get; set; }
        public bool IsSummary { get; set; }
        public String FileName { get; set; }

        public decimal LinesByCharacters
        {
            get { return Math.Round(_linesByCharacters, 1); }
            set { _linesByCharacters = Math.Round(value, 1); }
        }

        public decimal ValueByLbC { get; set; }
        public decimal LinesByKeyStrokes
        {
            get { return Math.Round(_linesByKeyStrokes, 1); }
            set { _linesByKeyStrokes = Math.Round(value, 1); }
        }

        public decimal ValueByLbK { get; set; }
        public decimal TotalValueByWords { get; set; }
        public decimal TotalValueByLbC { get; set; }
        public decimal TotalValueByLbK { get; set; }

        public decimal LineCharacters { get; set; }
        public decimal RatePerLine { get; set; }
        public ITemplateRates TemplateRates { get; set; }

        public List<ProjectProperty> ProjectProperties { get; set; }

        public ProjectFile(ITemplateRates _template)
        {
            TemplateRates = _template;
            IsSummary = false;
        }

        public void Prepare()
        {
            if (XMLNode != null && String.IsNullOrEmpty(FileName))
                FileName = XMLNode.SelectSingleNode("@name").Value;
            PreparePropertiesList();
            ReadPropertiesFromXMLNode();
            ComputePropertiesValues();

            GenerateTokens();
        }

        private void PreparePropertiesList()
        {
            ProjectProperties = new List<ProjectProperty>()
                {
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.PerfectMatch, PathInFile = Templates.Templates.PerfectMatchPathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.ContextMatch, PathInFile = Templates.Templates.ContextMatchPathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.Repetitions, PathInFile = Templates.Templates.RepetitionsPathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.Percent100, PathInFile = Templates.Templates.Percent100PathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.Percent95, PathInFile = Templates.Templates.Percent95PathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.Percent85, PathInFile = Templates.Templates.Percent85PathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.Percent75, PathInFile = Templates.Templates.Percent75PathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.Percent50, PathInFile = Templates.Templates.Percent50PathInFile},
                    new ProjectProperty(){StandardType = StandardType.Standard, Type = Templates.Templates.New, PathInFile = Templates.Templates.NewPathInFile},
                    new ProjectProperty(){StandardType = StandardType.Global, Type = Templates.Templates.RepsAnd100Percent, PathInFile = ""},
                    new ProjectProperty(){StandardType = StandardType.Global, Type = Templates.Templates.FuzzyMatches, PathInFile = ""},
                    new ProjectProperty(){StandardType = StandardType.Global, Type = Templates.Templates.NoMatch, PathInFile = ""},
                    new ProjectProperty(){StandardType = StandardType.Both, Type = Templates.Templates.Tags, PathInFile = Templates.Templates.TagsPathInFile}
                };

            ProjectProperties = TemplateRates.FillRatesForProject(ProjectProperties);
            if (TemplateRates.AdditionalRates.Count > 0)
            {
                RateValue ratePerLine =
                    TemplateRates.AdditionalRates.FirstOrDefault(rate => rate.Type == Templates.Templates.RatePerLine);
                if (ratePerLine != null)
                {
                    RatePerLine = ratePerLine.Rate;
                }
                RateValue lineCharacters =
                    TemplateRates.AdditionalRates.FirstOrDefault(rate => rate.Type == Templates.Templates.CharactersPerLine);
                if (lineCharacters != null)
                {
                    LineCharacters = lineCharacters.Rate;
                }
            }
        }

        private void ReadPropertiesFromXMLNode()
        {
            if (XMLNode != null)
            {
                foreach (var projectProperty in ProjectProperties.Where(prop => !String.IsNullOrEmpty(prop.PathInFile)))
                {
                    XPathNavigator node = XMLNode.SelectSingleNode(projectProperty.PathInFile);
                    if (node != null)
                    {
                        if (projectProperty.Type == Templates.Templates.Tags)
                        {
                            projectProperty.Words = Convert.ToDecimal(node.Value);
                        }
                        else
                        {
                            projectProperty.Words = Convert.ToDecimal(node.SelectSingleNode("@words").Value);
                            projectProperty.Characters = Convert.ToDecimal(node.SelectSingleNode("@characters").Value);
                        }
                    }
                }
                foreach (var projectProperty in ProjectProperties.Where(prop => String.IsNullOrEmpty(prop.PathInFile)))
                {
                    ProjectProperty repsProp;
                    ProjectProperty exactProp;
                    ProjectProperty fuzzy95;
                    ProjectProperty fuzzy85;
                    ProjectProperty fuzzy75;
                    ProjectProperty fuzzy50;
                    if (projectProperty.Type == Templates.Templates.RepsAnd100Percent)
                    {
                        repsProp = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Repetitions);
                        exactProp = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent100);
                        projectProperty.Words = repsProp.Words + exactProp.Words;
                        projectProperty.Characters = repsProp.Characters + exactProp.Characters;
                    }
                    else if (projectProperty.Type == Templates.Templates.FuzzyMatches)
                    {
                        fuzzy95 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent95);
                        fuzzy85 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent85);
                        fuzzy75 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent75);
                        fuzzy50 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent50);
                        projectProperty.Words = fuzzy50.Words + fuzzy75.Words + fuzzy85.Words + fuzzy95.Words;
                        projectProperty.Characters = fuzzy50.Characters + fuzzy75.Characters + fuzzy85.Characters +
                                                     fuzzy95.Characters;
                    }
                    else if (projectProperty.Type == Templates.Templates.NoMatch)
                    {
                        exactProp = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent100);
                        fuzzy95 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent95);
                        fuzzy85 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent85);
                        fuzzy75 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent75);
                        fuzzy50 = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent50);
                        XPathNavigator node = XMLNode.SelectSingleNode("analyse/total");
                        if (node != null)
                        {
                            decimal totalWords = Convert.ToDecimal(node.SelectSingleNode("@words").Value);
                            decimal totalCharacters = Convert.ToDecimal(node.SelectSingleNode("@characters").Value);
                            projectProperty.Words = totalWords -
                                                    (exactProp.Words + fuzzy50.Words + fuzzy75.Words + fuzzy85.Words +
                                                     fuzzy95.Words);
                            projectProperty.Characters = totalCharacters -
                                                         (exactProp.Characters + fuzzy50.Characters + fuzzy75.Characters +
                                                          fuzzy85.Characters + fuzzy95.Characters);
                        }
                    }
                }
            }
        }
    
        private void ComputePropertiesValues()
        {
            foreach (var projectProperty in ProjectProperties)
            {
                projectProperty.ValueByWords = projectProperty.Words*projectProperty.Rate;

                if (LineCharacters != 0)
                {
                    decimal rows = projectProperty.Characters/LineCharacters;
                    decimal roundedRows = Math.Round(rows);
                    rows = rows > roundedRows ? roundedRows + 1 : roundedRows;
                    projectProperty.LinesByCharacters = rows;
                    projectProperty.ValueByLbC = rows * RatePerLine * projectProperty.Rate;

                    rows = (projectProperty.Characters + projectProperty.Words)/LineCharacters;
                    roundedRows = Math.Round(rows);
                    rows = rows > roundedRows ? roundedRows + 1 : roundedRows;
                    projectProperty.LinesByKeyStrokes = rows;
                    projectProperty.ValueByLbK = rows * RatePerLine * projectProperty.Rate;
                }
            }

            if (LineCharacters != 0)
            {
                LinesByCharacters = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.LinesByCharacters);
                ValueByLbC = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.ValueByLbC);
                LinesByKeyStrokes = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.LinesByKeyStrokes);
                ValueByLbK = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.ValueByLbK);
            }

            TotalValueByWords = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.ValueByWords);
            TotalValueByLbC = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.ValueByLbC);
            TotalValueByLbK = ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(prop => prop.ValueByLbK);
        }

        public override void GenerateTokens()
        {
            var perfectMatchRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.PerfectMatch);
            var contextMatchRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.ContextMatch);
            var repetitionsRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Repetitions);
            var percent100Rate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Percent100);
            var percent95Rate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Percent95);
            var percent85Rate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Percent85);
            var percent75Rate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Percent75);
            var percent50Rate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Percent50);
            var newRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.New);
            var tagsRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.Tags);
            var repsAnd100PercentRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.RepsAnd100Percent);
            var fuzzyMatcheseRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.FuzzyMatches);
            var noMatchRate = TemplateRates.Rates.FirstOrDefault(rate => rate.Type == Templates.Templates.NoMatch);

            var perfectMatchProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.PerfectMatch);
            var contextMatchProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.ContextMatch);
            var repetitionsProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Repetitions);
            var percent100Property = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent100);
            var percent95Property = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent95);
            var percent85Property = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent85);
            var percent75Property = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent75);
            var percent50Property = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Percent50);
            var newProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.New);
            var tagsProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.Tags);
            var repsAnd100PercentProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.RepsAnd100Percent);
            var fuzzyMatcheseProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.FuzzyMatches);
            var noMatchProperty = ProjectProperties.FirstOrDefault(prop => prop.Type == Templates.Templates.NoMatch);

            Tokens = new List<Token>()
                {
                    new Token(TokenConstants.FILENAME, FileName),
                    new Token(TokenConstants.PERFECTMATCHRATE, perfectMatchRate == null ? 0 : perfectMatchRate.Rate),
                    new Token(TokenConstants.CONTEXTMATCHRATE, contextMatchRate == null ? 0 : contextMatchRate.Rate),
                    new Token(TokenConstants.REPETITIONSRATE, repetitionsRate == null ? 0 : repetitionsRate.Rate),
                    new Token(TokenConstants.ONEHUNDREDRATE, percent100Rate == null ? 0 : percent100Rate.Rate),
                    new Token(TokenConstants.NINETYFIVERATE, percent95Rate == null ? 0 : percent95Rate.Rate),
                    new Token(TokenConstants.EIGHTYFIVERATE, percent85Rate == null ? 0 : percent85Rate.Rate),
                    new Token(TokenConstants.SEVENTYFIVERATE, percent75Rate == null ? 0 : percent75Rate.Rate),
                    new Token(TokenConstants.FIFTYRATE, percent50Rate == null ? 0 : percent50Rate.Rate),
                    new Token(TokenConstants.NEWRATE, newRate == null ? 0 : newRate.Rate),
                    new Token(TokenConstants.TAGSRATE, tagsRate == null ? 0 : tagsRate.Rate),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESRATE, repsAnd100PercentRate == null ? 0 : repsAnd100PercentRate.Rate),
                    new Token(TokenConstants.FUZZYMATCHESRATE, fuzzyMatcheseRate == null ? 0 : fuzzyMatcheseRate.Rate),
                    new Token(TokenConstants.NOMATCHRATE, noMatchRate == null ? 0 : noMatchRate.Rate),

                    new Token(TokenConstants.PERFECTMATCHWORDS, perfectMatchProperty == null ? 0 : perfectMatchProperty.Words),
                    new Token(TokenConstants.CONTEXTMATCHWORDS, contextMatchProperty == null ? 0 : contextMatchProperty.Words),
                    new Token(TokenConstants.REPETITIONSWORDS, repetitionsProperty == null ? 0 : repetitionsProperty.Words),
                    new Token(TokenConstants.ONEHUNDREDWORDS, percent100Property == null ? 0 : percent100Property.Words),
                    new Token(TokenConstants.NINETYFIVEWORDS, percent95Property == null ? 0 : percent95Property.Words),
                    new Token(TokenConstants.EIGHTYFIVEWORDS, percent85Property == null ? 0 : percent85Property.Words),
                    new Token(TokenConstants.SEVENTYFIVEWORDS, percent75Property == null ? 0 : percent75Property.Words),
                    new Token(TokenConstants.FIFTYWORDS, percent50Property == null ? 0 : percent50Property.Words),
                    new Token(TokenConstants.NEWWORDS, newProperty == null ? 0 : newProperty.Words),
                    new Token(TokenConstants.TAGSWORDS, tagsProperty == null ? 0 : tagsProperty.Words),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESWORDS, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.Words),
                    new Token(TokenConstants.FUZZYMATCHESWORDS, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.Words),
                    new Token(TokenConstants.NOMATCHWORDS, noMatchProperty == null ? 0 : noMatchProperty.Words),

                    new Token(TokenConstants.PERFECTMATCHCHARACTERS, perfectMatchProperty == null ? 0 : perfectMatchProperty.Characters),
                    new Token(TokenConstants.CONTEXTMATCHCHARACTERS, contextMatchProperty == null ? 0 : contextMatchProperty.Characters),
                    new Token(TokenConstants.REPETITIONSCHARACTERS, repetitionsProperty == null ? 0 : repetitionsProperty.Characters),
                    new Token(TokenConstants.ONEHUNDREDCHARACTERS, percent100Property == null ? 0 : percent100Property.Characters),
                    new Token(TokenConstants.NINETYFIVECHARACTERS, percent95Property == null ? 0 : percent95Property.Characters),
                    new Token(TokenConstants.EIGHTYFIVECHARACTERS, percent85Property == null ? 0 : percent85Property.Characters),
                    new Token(TokenConstants.SEVENTYFIVECHARACTERS, percent75Property == null ? 0 : percent75Property.Characters),
                    new Token(TokenConstants.FIFTYCHARACTERS, percent50Property == null ? 0 : percent50Property.Characters),
                    new Token(TokenConstants.NEWCHARACTERS, newProperty == null ? 0 : newProperty.Characters),
                    new Token(TokenConstants.TAGSCHARACTERS, tagsProperty == null ? 0 : tagsProperty.Characters),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESCHARACTERS, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.Characters),
                    new Token(TokenConstants.FUZZYMATCHESCHARACTERS, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.Characters),
                    new Token(TokenConstants.NOMATCHCHARACTERS, noMatchProperty == null ? 0 : noMatchProperty.Characters),
 
                    new Token(TokenConstants.PERFECTMATCHVALUEBYWORDS, perfectMatchProperty == null ? 0 : perfectMatchProperty.ValueByWords),
                    new Token(TokenConstants.CONTEXTMATCHVALUEBYWORDS, contextMatchProperty == null ? 0 : contextMatchProperty.ValueByWords),
                    new Token(TokenConstants.REPETITIONSVALUEBYWORDS, repetitionsProperty == null ? 0 : repetitionsProperty.ValueByWords),
                    new Token(TokenConstants.ONEHUNDREDVALUEBYWORDS, percent100Property == null ? 0 : percent100Property.ValueByWords),
                    new Token(TokenConstants.NINETYFIVEVALUEBYWORDS, percent95Property == null ? 0 : percent95Property.ValueByWords),
                    new Token(TokenConstants.EIGHTYFIVEVALUEBYWORDS, percent85Property == null ? 0 : percent85Property.ValueByWords),
                    new Token(TokenConstants.SEVENTYFIVEVALUEBYWORDS, percent75Property == null ? 0 : percent75Property.ValueByWords),
                    new Token(TokenConstants.FIFTYVALUEBYWORDS, percent50Property == null ? 0 : percent50Property.ValueByWords),
                    new Token(TokenConstants.NEWVALUEBYWORDS, newProperty == null ? 0 : newProperty.ValueByWords),
                    new Token(TokenConstants.TAGSVALUEBYWORDS, tagsProperty == null ? 0 : tagsProperty.ValueByWords),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESVALUEBYWORDS, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.ValueByWords),
                    new Token(TokenConstants.FUZZYMATCHESVALUEBYWORDS, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.ValueByWords),
                    new Token(TokenConstants.NOMATCHVALUEBYWORDS, noMatchProperty == null ? 0 : noMatchProperty.ValueByWords),

                    new Token(TokenConstants.TOTALWORDS, ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(property => property.Words)),
                    new Token(TokenConstants.TOTALCHARACTERS, ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(property => property.Characters)),
                    new Token(TokenConstants.TOTALVALUEBYWORDS, ProjectProperties.Where(property => TemplateRates.Rates.Any(rate => rate.Type == property.Type)).Sum(property => property.ValueByWords)),
                    //
                    new Token(TokenConstants.PERFECTMATCHLINESBYCH, perfectMatchProperty == null ? 0 : perfectMatchProperty.LinesByCharacters),
                    new Token(TokenConstants.CONTEXTMATCHLINESBYCH, contextMatchProperty == null ? 0 : contextMatchProperty.LinesByCharacters),
                    new Token(TokenConstants.REPETITIONSLINESBYCH, repetitionsProperty == null ? 0 : repetitionsProperty.LinesByCharacters),
                    new Token(TokenConstants.ONEHUNDREDLINESBYCH, percent100Property == null ? 0 : percent100Property.LinesByCharacters),
                    new Token(TokenConstants.NINETYFIVELINESBYCH, percent95Property == null ? 0 : percent95Property.LinesByCharacters),
                    new Token(TokenConstants.EIGHTYFIVELINESBYCH, percent85Property == null ? 0 : percent85Property.LinesByCharacters),
                    new Token(TokenConstants.SEVENTYFIVELINESBYCH, percent75Property == null ? 0 : percent75Property.LinesByCharacters),
                    new Token(TokenConstants.FIFTYLINESBYCH, percent50Property == null ? 0 : percent50Property.LinesByCharacters),
                    new Token(TokenConstants.NEWLINESBYCH, newProperty == null ? 0 : newProperty.LinesByCharacters),
                    new Token(TokenConstants.TAGSLINESBYCH, tagsProperty == null ? 0 : tagsProperty.LinesByCharacters),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESLINESBYCH, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.LinesByCharacters),
                    new Token(TokenConstants.FUZZYMATCHESLINESBYCH, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.LinesByCharacters),
                    new Token(TokenConstants.NOMATCHLINESBYCH, noMatchProperty == null ? 0 : noMatchProperty.LinesByCharacters),

                    new Token(TokenConstants.PERFECTMATCHVALUEBYLINESBYCH, perfectMatchProperty == null ? 0 : perfectMatchProperty.ValueByLbC),
                    new Token(TokenConstants.CONTEXTMATCHVALUEBYLINESBYCH, contextMatchProperty == null ? 0 : contextMatchProperty.ValueByLbC),
                    new Token(TokenConstants.REPETITIONSVALUEBYLINESBYCH, repetitionsProperty == null ? 0 : repetitionsProperty.ValueByLbC),
                    new Token(TokenConstants.ONEHUNDREDVALUEBYLINESBYCH, percent100Property == null ? 0 : percent100Property.ValueByLbC),
                    new Token(TokenConstants.NINETYFIVEVALUEBYLINESBYCH, percent95Property == null ? 0 : percent95Property.ValueByLbC),
                    new Token(TokenConstants.EIGHTYFIVEVALUEBYLINESBYCH, percent85Property == null ? 0 : percent85Property.ValueByLbC),
                    new Token(TokenConstants.SEVENTYFIVEVALUEBYLINESBYCH, percent75Property == null ? 0 : percent75Property.ValueByLbC),
                    new Token(TokenConstants.FIFTYVALUEBYLINESBYCH, percent50Property == null ? 0 : percent50Property.ValueByLbC),
                    new Token(TokenConstants.NEWVALUEBYLINESBYCH, newProperty == null ? 0 : newProperty.ValueByLbC),
                    new Token(TokenConstants.TAGSVALUEBYLINESBYCH, tagsProperty == null ? 0 : tagsProperty.ValueByLbC),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESVALUEBYLINESBYCH, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.ValueByLbC),
                    new Token(TokenConstants.FUZZYMATCHESVALUEBYLINESBYCH, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.ValueByLbC),
                    new Token(TokenConstants.NOMATCHVALUEBYLINESBYCH, noMatchProperty == null ? 0 : noMatchProperty.ValueByLbC),

                    new Token(TokenConstants.PERFECTMATCHLINESBYKEY, perfectMatchProperty == null ? 0 : perfectMatchProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.CONTEXTMATCHLINESBYKEY, contextMatchProperty == null ? 0 : contextMatchProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.REPETITIONSLINESBYKEY, repetitionsProperty == null ? 0 : repetitionsProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.ONEHUNDREDLINESBYKEY, percent100Property == null ? 0 : percent100Property.LinesByKeyStrokes),
                    new Token(TokenConstants.NINETYFIVELINESBYKEY, percent95Property == null ? 0 : percent95Property.LinesByKeyStrokes),
                    new Token(TokenConstants.EIGHTYFIVELINESBYKEY, percent85Property == null ? 0 : percent85Property.LinesByKeyStrokes),
                    new Token(TokenConstants.SEVENTYFIVELINESBYKEY, percent75Property == null ? 0 : percent75Property.LinesByKeyStrokes),
                    new Token(TokenConstants.FIFTYLINESBYKEY, percent50Property == null ? 0 : percent50Property.LinesByKeyStrokes),
                    new Token(TokenConstants.NEWLINESBYKEY, newProperty == null ? 0 : newProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.TAGSLINESBYKEY, tagsProperty == null ? 0 : tagsProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESLINESBYKEY, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.FUZZYMATCHESLINESBYKEY, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.LinesByKeyStrokes),
                    new Token(TokenConstants.NOMATCHLINESBYKEY, noMatchProperty == null ? 0 : noMatchProperty.LinesByKeyStrokes),

                    new Token(TokenConstants.PERFECTMATCHVALUEBYLINESBYKEYS, perfectMatchProperty == null ? 0 : perfectMatchProperty.ValueByLbK),
                    new Token(TokenConstants.CONTEXTMATCHVALUEBYLINESBYKEYS, contextMatchProperty == null ? 0 : contextMatchProperty.ValueByLbK),
                    new Token(TokenConstants.REPETITIONSVALUEBYLINESBYKEYS, repetitionsProperty == null ? 0 : repetitionsProperty.ValueByLbK),
                    new Token(TokenConstants.ONEHUNDREDVALUEBYLINESBYKEYS, percent100Property == null ? 0 : percent100Property.ValueByLbK),
                    new Token(TokenConstants.NINETYFIVEVALUEBYLINESBYKEYS, percent95Property == null ? 0 : percent95Property.ValueByLbK),
                    new Token(TokenConstants.EIGHTYFIVEVALUEBYLINESBYKEYS, percent85Property == null ? 0 : percent85Property.ValueByLbK),
                    new Token(TokenConstants.SEVENTYFIVEVALUEBYLINESBYKEYS, percent75Property == null ? 0 : percent75Property.ValueByLbK),
                    new Token(TokenConstants.FIFTYVALUEBYLINESBYKEYS, percent50Property == null ? 0 : percent50Property.ValueByLbK),
                    new Token(TokenConstants.NEWVALUEBYLINESBYKEYS, newProperty == null ? 0 : newProperty.ValueByLbK),
                    new Token(TokenConstants.TAGSVALUEBYLINESBYKEYS, tagsProperty == null ? 0 : tagsProperty.ValueByLbK),
                    new Token(TokenConstants.REPSANDONEHUNDREDMATCHESVALUEBYLINESBYKEYS, repsAnd100PercentProperty == null ? 0 : repsAnd100PercentProperty.ValueByLbK),
                    new Token(TokenConstants.FUZZYMATCHESVALUEBYLINESBYKEYS, fuzzyMatcheseProperty == null ? 0 : fuzzyMatcheseProperty.ValueByLbK),
                    new Token(TokenConstants.NOMATCHVALUEBYLINESBYKEYS, noMatchProperty == null ? 0 : noMatchProperty.ValueByLbK),

                    new Token(TokenConstants.TOTALVALUELINESBYCH, TotalValueByLbC),
                    new Token(TokenConstants.TOTALVALUELINESBYKEYS, TotalValueByLbK),

                    new Token(TokenConstants.STANDARDLINECHARACTERS, LineCharacters),
                    new Token(TokenConstants.RATEPERLINE, RatePerLine),
                    new Token(TokenConstants.STANDARDLINESBYCHARACTERS, LinesByCharacters),
                    new Token(TokenConstants.STANDARDLINESBYKEYS, LinesByKeyStrokes),
                    new Token(TokenConstants.VALUESTANDARDLINEBYCHARACTERS, ValueByLbC),
                    new Token(TokenConstants.VALUESTANDARDLINEBYKEYS, ValueByLbK)
                };
        }
    }
}