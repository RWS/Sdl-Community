<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns="http://www.w3.org/2000/svg">
  <xsl:output method="html" indent="yes"/>


  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <title>Qualitivity - Activity Documents Overview Report</title>
        <!--Developed by Patrick Hartnett 25/11/2013-->

        <style type="text/css">
          body
          {
          background: #FFFFFF;
          font-family: Verdana, Arial, Helvetica;
          font-size: 10pt;
          }
          a
          {
          color: #666699;
          }
          li
          {
          margin-top: 3;
          }
          h1
          {
          font-size: 18pt;
          }
          h2
          {
          font-size: 14pt;
          }
          h3
          {
          font-size: 12pt;
          }
          table
          {
          font-family: Arial, Verdana, Helvetica;
          border-color: #000000;
          border-collapse: collapse;
          font-size: 9pt;
          }
          th
          {
          color: white;
          background-color: #3366ff;
          }
          td
          {
          border-style: solid;
          border-width: thin;
          border-color: #CED8F6;
          }
          td.segmentId
          {
          color: black;
          background-color: #E9E9E9;
          }
          td.multiplechangesapplied
          {
          color: black;
          background-color: #ffff66;
          }
          td.innerFileName
          {
          color: #003399;
          text-align: center;
          background-color:  #FFF8F0;
          }
          span.text
          {
          color:  Black;
          }
          span.textNew
          {
          color: Blue;
          text-decoration: underline;
          background-color:#FFFF66;
          }
          span.textRemoved
          {
          color:  Red;
          text-decoration: line-through;
          }
          span.tag
          {
          color:  Gray;
          }
          span.tagNew
          {
          background-color: #DDEEFF;
          }
          span.tagRemoved
          {
          background-color:  #FFE8E8;
          }
          span.tagNoWrap
          {
          color: Gray;
          white-space:nowrap
          }
          span.grayNoWrap
          {
          color: Gray;
          white-space:nowrap
          }

          table.bargraph
          {
          margin-top: -1px;
          padding:0;
          font-size: 15px;
          height: 100%;
          width: 100%;
          }

          table.bargraph td.date
          {
          width: 75px;
          text-align:left;
          }

          table.bargraph td
          {
          width: 5px;
          height: 20px;
          font-size: 1px;
          padding-top: 2px;


          }

          table.histogram td
          {
          width: 5px;
          font-size: 1px;
          padding-top: 0px;
          }
          table.histogram td p
          {
          width: 20px;
          height: 3px;
          line-height: 0;
          margin: 0 1px 1px 0;
          padding: 0;
          font-size: 1px;
          }
          .TableCellsNoBorder
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style: none;
          border-color: white;
          }
          .TableCellsBorderTop
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:solid none none none;
          border-color: #C0C0C0
          }
          .TableCellsBorderBottom
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:none none solid none;
          border-color: #C0C0C0
          }
          .TableCellsBorderTopRight
          {
          padding: 2px 5px 2px 5px;
          order-width: thin;
          border-style:solid solid none none;
          border-color: #C0C0C0
          }
          .TableCellsBorderTopRightBottom
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:solid solid solid none;
          border-color: #C0C0C0
          }
          .TableCellsBorderTopRightBottomLeft
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:solid solid solid solid;
          border-color: #C0C0C0
          }
          .TableCellsBorderTopBottomLeft
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:solid none solid solid;
          border-color: #C0C0C0
          }
          .TableCellsBorderRightBottom
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:none solid solid none;
          border-color: #C0C0C0
          }
          .TableCellsBorderTopBottom
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:solid none solid none;
          border-color: #C0C0C0
          }
          .TableCellsBorderRightBottomLeft
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style:none solid none solid;
          border-color: #C0C0C0
          }
          .TableCellsBorderBottomLeft
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style: none none solid solid;
          border-color: #C0C0C0
          }
          .TableCellsBorderRight
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style: none solid none none;
          border-color: #C0C0C0
          }
          .TableCellsBorderLeft
          {
          padding: 2px 5px 2px 5px;
          border-width: thin;
          border-style: none none none solid;
          border-color: #C0C0C0
          }
          .DefaultCellsColorsWhite
          {
          background-color: white;
          text-align: right;
          color: #003300;
          font-weight: normal;
          }
          .DefaultCellsSubHeaderColors
          {
          background-color: #DFEAEA;
          text-align: left;
          color: #003300;
          font-weight: normal;
          }
          .DefaultCellsSubHeaderColorsCenter
          {
          background-color: #DFEAEA;
          text-align: center;
          color: #003300;
          font-weight: normal;
          }
          .DefaultCellsSubHeaderColorsItalic
          {
          background-color: #DFEAEA;
          text-align: left;
          color: #003300;
          font-weight: normal;
          font-style: italic;
          }
          .DefaultCellsSubHeaderColorsRight
          {
          background-color: #DFEAEA;
          text-align: right;
          color: #003300;
          font-weight: normal;
          }
          .DefaultCellsSubHeaderColorsRightItalic
          {
          background-color: #DFEAEA;
          text-align: right;
          color: #003300;
          font-weight: normal;
          font-style: italic;
          }
          .DefaultCellsTitleColumnColors
          {
          background-color: #DFEAEA;
          color: #003300;
          font-weight: bold;
          text-align: left;
          }
          .DefaultCellsTotalsColors
          {
          background-color: #DFEAEA;
          color: #003300;
          font-weight: bold;
          }
          .DefaultCellsTotalsColorsRight
          {
          background-color: #DFEAEA;
          color: #003300;
          font-weight: bold;
          text-align: right;
          }
          .DefaultCellsHeaderColorsCenter
          {
          background-color: #DFEAEA;
          color: #003300;
          font-weight: bold;
          text-align: center;
          }
          .DefaultCellsSectionHeaderColors
          {
          background-color: #F3F3F3;
          color: #003300;
          text-align: left;
          }
          .DefaultCellsSectionHeaderColorsCenter
          {
          background-color: #F3F3F3;
          color: #003300;
          text-align: center;
          }
          .DefaultCellsSectionHeaderColorsItalic
          {
          background-color: #F3F3F3;
          color: #003300;
          text-align: left;
          font-style: italic;
          }
          .DefaultCellsSectionHeaderColorsCenterItalic
          {
          background-color: #F3F3F3;
          color: #003300;
          text-align: center;
          font-style: italic;
          }
          .TableDefaultTopLevelStyle
          {
          cellpadding="0" cellspacing="0" style="color: Black; text-align: left;  background-color: #ECF1FB" border="0" width="100%"
          }
          .TableTopLevelStyle
          {
          border-width: thin;
          border-style: none none none none;
          border-color: #C0C0C0;
          color: Black;
          text-align: left;
          background-color: #ECF1FB";
          }
          .DefaultFileOverviewHeader
          {
          text-align: right; font-size: 12px; padding: 2px 2px 2px 2px;
          }
          .DefaultPadding
          {
          padding: 2px 2px 2px 2px;
          }
          .DefaultPaddingItalic
          {
          padding: 2px 2px 2px 2px;
          font-style: italic;
          }
          .DefaultPaddingRight
          {
          padding: 2px 2px 2px 2px; text-align: Right;
          }
          .DefaultPaddingBold
          {
          padding: 2px 2px 2px 2px;
          font-weight: Bold;
          }
          .DefaultFileSectionHeader
          {
          color: #FFFFFF; text-align: left;  background-color: #006699; font-weight: bold;
          }
          .DefaultPaddingNowrapBold
          {
          padding: 2px 2px 2px 2px;
          font-weight: bold;
          text-align: left;
          white-space: nowrap;
          }
          .DefaultPaddingNowrap
          {
          padding: 2px 2px 2px 2px;
          font-weight: normal;
          text-align: left;
          white-space: nowrap;
          }
          .TextAlignCenter
          {
          text-align: center;
          }
          .TextAlignRight
          {
          text-align:right;
          }
          .TextFontGrayItalicSmaller
          {
          font-style: italic;
          color: gray;
          }
          .ActivityRow
          {
          padding: 1px 1px 1px 1px; background-color: #C0C0C0; font-weight: bold; color: #003300;
          }
          .SummaryHeaderBar
          {
          color: white; text-align: left; background-color: #006699;
          }
        </style>
      </head>
      <body>
        <xsl:apply-templates select="documents"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="documents">
    <xsl:for-each select="document">
      <table class="SummaryHeaderBar" cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
          <td nowrap="nowrap" class="TableCellsBorderTopBottomLeft">
            Document: <xsl:value-of select="@documentName"/>
          </td>
          <td class="TableCellsBorderTopRightBottom TextAlignRight">
            <span class="DefaultPadding">Total Elapsed Time:</span>
            <span class="DefaultPadding">
              <xsl:value-of select="@documentTotalElapsedTime"/>
              <span class="DefaultPaddingItalic"> (hours:</span>
              <span class="DefaultPaddingItalic">
                <xsl:value-of select="@documentTotalElapsedHours"/>)
              </span>
            </span>
            <span class="DefaultPadding"> Document Activities:</span>
            <span class="DefaultPaddingBold">
              <xsl:value-of select="@activities"/>
            </span>
          </td>
        </tr>
      </table>
      <table class="TableTopLevelStyle" border="0" width="100%">
        <tr>

          <xsl:call-template name="TranslationModificationsTableHeaderTop">
          </xsl:call-template>

          <xsl:call-template name="PEMPTableHeaderTop">
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsHeaderTop">
          </xsl:call-template>

        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableHeaderSub">
          </xsl:call-template>

          <xsl:call-template name="PEMPTableHeaderSub">
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsHeaderSub">
          </xsl:call-template>

        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"Perfect Match"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourcePMSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourcePMWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourcePMCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourcePMTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesPMSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesPMWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesPMCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesPMTags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='"100%"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@exactSegments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@exactWords"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@exactCharacters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@exactPercent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@exactWords"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@exactTotal"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"Not Translated"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@notTranslatedOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@notTranslatedUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@notTranslatedUpdatedStatusChangesPercentage"/>
          </xsl:call-template>

        </tr>
        <tr>
          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"Context Match"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceCMSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceCMWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceCMCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceCMTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesCMSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesCMWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesCMCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesCMTags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='"95% - 99%"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@fuzzy99Segments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@fuzzy99Words"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@fuzzy99Characters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@fuzzy99Percent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@fuzzy99Words"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@fuzzy99Total"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>


          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"Draft"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@draftOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@draftUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@draftUpdatedStatusChangesPercentage"/>
          </xsl:call-template>

        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"Repetitions"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceRepsSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceRepsWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceRepsCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceRepsTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesRepsSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesRepsWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesRepsCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesRepsTags"/>
          </xsl:call-template>


          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='"85% - 94%"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@fuzzy94Segments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@fuzzy94Words"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@fuzzy94Characters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@fuzzy94Percent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@fuzzy94Words"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@fuzzy94Total"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>


          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"Translated"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@translatedOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@translatedUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@translatedUpdatedStatusChangesPercentage"/>
          </xsl:call-template>



        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"100%"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceExactSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceExactWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceExactCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceExactTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesExactSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesExactWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesExactCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesExactTags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='"75% - 84%"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@fuzzy84Segments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@fuzzy84Words"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@fuzzy84Characters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@fuzzy84Percent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@fuzzy84Words"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@fuzzy84Total"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>


          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"Rejected"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@translationRejectedOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@translationRejectedUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@translationRejectedUpdatedStatusChangesPercentage"/>
          </xsl:call-template>



        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"95% - 99%"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceFuzzy99Segments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceFuzzy99Words"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceFuzzy99Characters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceFuzzy99Tags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy99Segments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy99Words"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy99Characters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesFuzzy99Tags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='"50% - 74%"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@fuzzy74Segments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@fuzzy74Words"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@fuzzy74Characters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@fuzzy74Percent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@fuzzy74Words"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@fuzzy74Total"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"Approved"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@translationApprovedOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@translationApprovedUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@translationApprovedUpdatedStatusChangesPercentage"/>
          </xsl:call-template>

        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"85% - 94%"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceFuzzy94Segments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceFuzzy94Words"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceFuzzy94Characters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceFuzzy94Tags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy94Segments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy94Words"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy94Characters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesFuzzy94Tags"/>
          </xsl:call-template>


          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='"New"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@newSegments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@newWords"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@newCharacters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@newPercent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@newWords"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@newTotal"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>



          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"S.off Rejected"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@signOffRejectedOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@signOffRejectedUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@signOffRejectedUpdatedStatusChangesPercentage"/>
          </xsl:call-template>


        </tr>
        <tr>
          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"75% - 84%"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceFuzzy84Segments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceFuzzy84Words"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceFuzzy84Characters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceFuzzy84Tags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy84Segments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy84Words"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy84Characters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesFuzzy84Tags"/>
          </xsl:call-template>


          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Segments" select='""'/>
            <xsl:with-param name="Words" select='""'/>
            <xsl:with-param name="Characters" select='""'/>
            <xsl:with-param name="Percent" select='""'/>
            <xsl:with-param name="WordsRate" select='""'/>
            <xsl:with-param name="Total" select='""'/>
            <xsl:with-param name="Currency" select='""'/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='"Signed-off"'/>
            <xsl:with-param name="Original" select="confirmationStatistics/@signedOffOriginal"/>
            <xsl:with-param name="Updated" select="confirmationStatistics/@signedOffUpdated"/>
            <xsl:with-param name="Changes" select="confirmationStatistics/@signedOffUpdatedStatusChangesPercentage"/>
          </xsl:call-template>


        </tr>
        <tr>
          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"50% - 74%"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceFuzzy74Segments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceFuzzy74Words"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceFuzzy74Characters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceFuzzy74Tags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy74Segments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy74Words"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy74Characters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesFuzzy74Tags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Segments" select='""'/>
            <xsl:with-param name="Words" select='""'/>
            <xsl:with-param name="Characters" select='""'/>
            <xsl:with-param name="Percent" select='""'/>
            <xsl:with-param name="WordsRate" select='""'/>
            <xsl:with-param name="Total" select='""'/>
            <xsl:with-param name="Currency" select='""'/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Original" select='""'/>
            <xsl:with-param name="Updated" select='""'/>
            <xsl:with-param name="Changes" select='""'/>
          </xsl:call-template>

        </tr>
        <tr>

          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"New"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceNewSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceNewWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceNewCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceNewTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesNewSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesNewWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesNewCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesNewTags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Segments" select='""'/>
            <xsl:with-param name="Words" select='""'/>
            <xsl:with-param name="Characters" select='""'/>
            <xsl:with-param name="Percent" select='""'/>
            <xsl:with-param name="WordsRate" select='""'/>
            <xsl:with-param name="Total" select='""'/>
            <xsl:with-param name="Currency" select='""'/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Original" select='""'/>
            <xsl:with-param name="Updated" select='""'/>
            <xsl:with-param name="Changes" select='""'/>
          </xsl:call-template>

        </tr>
        <tr>


          <xsl:call-template name="TranslationModificationsTableValues">
            <xsl:with-param name="Title" select='"Automated Translation"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceATSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceATWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceATCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceATTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesATSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesATWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesATCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesATTags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Segments" select='""'/>
            <xsl:with-param name="Words" select='""'/>
            <xsl:with-param name="Characters" select='""'/>
            <xsl:with-param name="Percent" select='""'/>
            <xsl:with-param name="WordsRate" select='""'/>
            <xsl:with-param name="Total" select='""'/>
            <xsl:with-param name="Currency" select='""'/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTableValues">
            <xsl:with-param name="Title" select='""'/>
            <xsl:with-param name="Original" select='""'/>
            <xsl:with-param name="Updated" select='""'/>
            <xsl:with-param name="Changes" select='""'/>
          </xsl:call-template>

        </tr>
        <tr>


          <xsl:call-template name="TranslationModificationsTableTotals">
            <xsl:with-param name="Title" select='"Total"'/>
            <xsl:with-param name="SourceSegments" select="translationModifications/@sourceTotalSegments"/>
            <xsl:with-param name="SourceWords" select="translationModifications/@sourceTotalWords"/>
            <xsl:with-param name="SourceCharacters" select="translationModifications/@sourceTotalCharacters"/>
            <xsl:with-param name="SourceTags" select="translationModifications/@sourceTotalTags"/>
            <xsl:with-param name="ChangedSegments" select="translationModifications/@changesTotalSegments"/>
            <xsl:with-param name="ChangedWords" select="translationModifications/@changesTotalWords"/>
            <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesTotalCharacters"/>
            <xsl:with-param name="ChangedTags" select="translationModifications/@changesTotalTags"/>
          </xsl:call-template>

          <xsl:call-template name="PEMPTableTotals">
            <xsl:with-param name="Title" select='"Total"'/>
            <xsl:with-param name="Segments" select="pempAnalysis/data/@totalSegments"/>
            <xsl:with-param name="Words" select="pempAnalysis/data/@totalWords"/>
            <xsl:with-param name="Characters" select="pempAnalysis/data/@totalCharacters"/>
            <xsl:with-param name="Percent" select="pempAnalysis/data/@totalPercent"/>
            <xsl:with-param name="WordsRate" select="pempAnalysis/rates/@totalWords"/>
            <xsl:with-param name="Total" select="pempAnalysis/rates/@priceTotal"/>
            <xsl:with-param name="Currency" select="pempAnalysis/rates/@currency"/>
            <xsl:with-param name="ShowWordRate" select='"True"'/>
          </xsl:call-template>

          <xsl:call-template name="ConfirmationStatisticsTotals">
            <xsl:with-param name="Title" select='"Total"'/>
            <xsl:with-param name="Changes" select="totals/@statusChangesPercentage"/>
          </xsl:call-template>

        </tr>
      </table>

      <xsl:variable name="documentSourceLanguage" select="@documentSourceLanguage"/>
      <xsl:variable name="documentTargetlanguage" select="@documentTargetlanguage"/>
      <xsl:variable name="documentSourceLanguageFlag" select="@documentSourceLanguageFlag"/>
      <xsl:variable name="documentTargetlanguageFlag" select="@documentTargetlanguageFlag"/>
      <xsl:variable name="documentHasTrackChanges" select="@documentHasTrackChanges"/>

      <xsl:for-each select="activities">
        <table style="margin-top: 5px; margin-bottom: 4px;  color: #003300; text-align: left; " border="1" width="100%">
          <tr class="ActivityRow">
            <td class="TableCellsBorderTopBottomLeft DefaultPaddingNowrapBold">Document Name</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Source</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Target</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Activity Type</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Translation Modifications</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Status Changes</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Quality Metrics</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Comments</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Elapsed Time</td>
            <td class="TableCellsBorderTopBottom DefaultPaddingNowrapBold" >Opened</td>
            <td class="TableCellsBorderTopBottomRight DefaultPaddingNowrapBold" >Closed</td>
          </tr>
          <xsl:for-each select="activity">
            <tr style="background-color: #E3E5E8;">
              <td class="TableCellsBorderTopBottomLeft DefaultPaddingNowrap" >
                <xsl:value-of select="@activityName"/>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <span style="vertical-align: middle">
                  <xsl:text> </xsl:text>
                  <span style="padding: 2px 0px 0px 0px;">
                    <img height="16" >
                      <xsl:attribute name="src">
                        <xsl:value-of select='$documentSourceLanguageFlag' />
                      </xsl:attribute>
                    </img>
                  </span>
                </span>
                <span style="margin:0px 0px 5px 0px">
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="$documentSourceLanguage"/>
                </span>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap">
                <span style="vertical-align: middle">
                  <xsl:text> </xsl:text>
                  <span style="padding: 2px 0px 0px 0px;">
                    <img height="16" >
                      <xsl:attribute name="src">
                        <xsl:value-of select='$documentTargetlanguageFlag' />
                      </xsl:attribute>
                    </img>
                  </span>

                </span>
                <span style="padding:0px 0px 5px 0px">
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="$documentTargetlanguage"/>
                </span>
              </td>
              <td  class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@activityType"/>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@a_documentTotalSegmentContentUpdated"/>
              </td>
              <td  class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@a_documentTotalSegmentStatusUpdated"/>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@a_qualityMetrics_record_count"/>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@a_documentTotalSegmentCommentsUpdatedTotal"/>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@totalElapsedTime"/>
              </td>
              <td class="TableCellsBorderTopBottom DefaultPaddingNowrap" >
                <xsl:value-of select="@started"/>
              </td>
              <td class="TableCellsBorderTopBottomRight DefaultPaddingNowrap">
                <xsl:value-of select="@stopped"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </xsl:for-each>

      <xsl:for-each select="segments">
        <table border="1" cellpadding="3" cellspacing="0" width="100%">
          <tr>
            <th>ID</th>
            <th>Date/Time</th>
            <th>Status</th>
            <th>Match</th>
            <th>Words</th>
            <th>Source</th>
            <th style="white-space: nowrap; ">Target Updated</th>
            <xsl:if test="$documentHasTrackChanges = 'true'">
              <th style="white-space: nowrap; ">Track Changes</th>
            </xsl:if>
            <th style="white-space: nowrap; ">Target Comparison</th>
            <th style="white-space: nowrap; ">PEMp</th>
            <th>Quality Metrics</th>
            <th>Comments</th>
          </tr>
          <xsl:for-each select="segment">
            <tr>
              <td class="segmentId">
                <xsl:value-of select="@segmentId"/>
              </td>
              <xsl:copy-of select="date"/>
              <td>
                <xsl:copy-of select="status"/>
              </td>
              <xsl:copy-of select="translationMatchType"/>
              <td>
                <xsl:value-of select="@words"/>
              </td>
              <td>
                <xsl:copy-of select="source"/>
              </td>
              <td>
                <xsl:copy-of select="target_updated"/>
              </td>
              <xsl:if test="$documentHasTrackChanges = 'true'">
                <td style="vertical-align: text-top;">
                  <xsl:for-each select="targetUpdatedRevisionMarkers">
                    <xsl:apply-templates select="revisionMarker"/>
                  </xsl:for-each>
                </td>
              </xsl:if>
              <td>
                <xsl:copy-of select="comparison"/>
              </td>
              <td>
                <xsl:copy-of select="PEMP"/>
                <br/>
                <span class="grayNoWrap">
                  Edit-Dist.:&#160;<xsl:copy-of select="EditDistance"/>
                </span>
                <br/>
                <span class="grayNoWrap">
                  Max chars:&#160;<xsl:copy-of select="MaxCharacters"/>
                </span>
              </td>
              <td>
                <xsl:for-each select="qualityMetrics">
                  <xsl:apply-templates select="qualityMetric"/>
                </xsl:for-each>
              </td>
              <td>
                <xsl:for-each select="comments">
                  <xsl:apply-templates select="comment"/>
                </xsl:for-each>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </xsl:for-each>
      <br/>
    </xsl:for-each>
  </xsl:template>


  <xsl:template name="TranslationModificationsTableHeaderTop">

    <td width="20%" class="TableCellsBorderTopBottomLeft DefaultCellsSectionHeaderColors">
      Translation Modifications
    </td>
    <td nowrap="nowrap"  colspan="2" class="TableCellsBorderTopBottom DefaultCellsSectionHeaderColorsCenter">
      Total
    </td>
    <td nowrap="nowrap" colspan="4" class="TableCellsBorderTopRightBottom DefaultCellsSectionHeaderColorsCenter">
      Modified
    </td>

    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TranslationModificationsTableHeaderSub">
    <td class="TableCellsBorderTopBottomLeft DefaultCellsSubHeaderColors">

    </td>
    <td class="TableCellsBorderBottom DefaultCellsSubHeaderColorsRight">
      Segments
    </td>
    <td class="TableCellsBorderBottom DefaultCellsSubHeaderColorsRight">
      Words
    </td>
    <!--<td class="TableCellsBorderBottom DefaultCellsSubHeaderColorsRight">
      Chars
    </td>
    <td class="TableCellsBorderBottom DefaultCellsSubHeaderColorsRight">
      Tags/P
    </td>-->
    <td class="TableCellsBorderBottomLeft DefaultCellsSubHeaderColorsRight">
      Segments
    </td>
    <td class="TableCellsBorderBottom DefaultCellsSubHeaderColorsRight">
      %
    </td>
    <td class="TableCellsBorderBottom DefaultCellsSubHeaderColorsRight">
      Words
    </td>
    <td class="TableCellsBorderRightBottom DefaultCellsSubHeaderColorsRight">
      Characters
    </td>
    <!--<td class="TableCellsBorderRightBottom DefaultCellsSubHeaderColorsRight">
      Tags/P
    </td>-->


    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TranslationModificationsTableValues">
    <xsl:param name="Title"/>
    <xsl:param name="SourceSegments"/>
    <xsl:param name="SourceWords"/>
    <xsl:param name="SourceCharacters"/>
    <xsl:param name="SourceTags"/>
    <xsl:param name="ChangedSegments"/>
    <xsl:param name="ChangedWords"/>
    <xsl:param name="ChangedCharacters"/>
    <xsl:param name="ChangedTags"/>

    <td class="TableCellsBorderLeft DefaultCellsTitleColumnColors" nowrap="nowrap" >
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SourceSegments"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SourceWords"/>
    </td>
    <!--<td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SourceCharacters"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SourceTags"/>
    </td>-->
    <td class="TableCellsNoBorder DefaultCellsColorsWhite" nowrap="nowrap" >
      <xsl:value-of select="$ChangedSegments"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite" nowrap="nowrap" >
      <xsl:if test="$ChangedSegments != ''">
        <span style="font-style: italic;color: gray;">
          <xsl:choose>
            <xsl:when test="$SourceSegments > 0 and $ChangedSegments > 0 ">
              <xsl:value-of select="format-number((number($ChangedSegments) div number($SourceSegments)) * 100, '###,###,##0')"/>%
            </xsl:when>
            <xsl:otherwise>
              0%
            </xsl:otherwise>
          </xsl:choose>
        </span>
      </xsl:if>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$ChangedWords"/>
    </td>
    <td class="TableCellsBorderRight DefaultCellsColorsWhite">
      <xsl:value-of select="$ChangedCharacters"/>
    </td>
    <!--<td class="TableCellsBorderRight DefaultCellsColorsWhite">
      <xsl:value-of select="$ChangedTags"/>
    </td>-->

    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TranslationModificationsTableTotals">
    <xsl:param name="Title"/>
    <xsl:param name="SourceSegments"/>
    <xsl:param name="SourceWords"/>
    <xsl:param name="SourceCharacters"/>
    <xsl:param name="SourceTags"/>
    <xsl:param name="ChangedSegments"/>
    <xsl:param name="ChangedWords"/>
    <xsl:param name="ChangedCharacters"/>
    <xsl:param name="ChangedTags"/>

    <td class="TableCellsBorderTopBottomLeft DefaultCellsTitleColumnColors">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SourceSegments"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SourceWords"/>
    </td>
    <!--<td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SourceCharacters"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SourceTags"/>
    </td>-->
    <td class="TableCellsBorderTopBottomLeft DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$ChangedSegments"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:if test="$ChangedSegments != ''">
        <span style="font-style: italic;color: gray;">
          <xsl:choose>
            <xsl:when test="$SourceSegments > 0 and $ChangedSegments > 0 ">
              <xsl:value-of select="format-number((number($ChangedSegments) div number($SourceSegments)) * 100, '###,###,##0')"/>%
            </xsl:when>
            <xsl:otherwise>
              0%
            </xsl:otherwise>
          </xsl:choose>
        </span>
      </xsl:if>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$ChangedWords"/>
    </td>
    <td class="TableCellsBorderTopRightBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$ChangedCharacters"/>
    </td>
    <!--<td class="TableCellsBorderTopRightBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$ChangedTags"/>
    </td>-->
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="PEMPTableHeaderTop">
    <xsl:param name="ShowWordRate"/>

    <td nowrap="nowrap" width="42%" class="TableCellsBorderTopRightBottomLeft DefaultCellsSectionHeaderColors">
      <xsl:attribute name="colspan">
        <xsl:choose>
          <xsl:when test="$ShowWordRate = 'True'">
            7
          </xsl:when>
          <xsl:otherwise>
            6
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      Post-Edit Modifications Analysis
    </td>

    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>

  </xsl:template>

  <xsl:template name="PEMPTableHeaderSub">
    <xsl:param name="ShowWordRate"/>

    <td nowrap="nowrap" width="15%" class="TableCellsBorderTopBottomLeft DefaultCellsSubHeaderColors">
      Analysis Band
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Segments
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Words
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Characters
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Percent
    </td>
    <xsl:if test="$ShowWordRate = 'True'">
      <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
        Rate
      </td>
    </xsl:if>
    <td class="TableCellsBorderTopRightBottom DefaultCellsSubHeaderColorsCenter">
      Total
    </td>

    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="PEMPTableValues">
    <xsl:param name="Title"/>
    <xsl:param name="Segments"/>
    <xsl:param name="Words"/>
    <xsl:param name="Characters"/>
    <xsl:param name="Percent"/>
    <xsl:param name="WordsRate"/>
    <xsl:param name="Total"/>
    <xsl:param name="Currency"/>
    <xsl:param name="ShowWordRate"/>


    <td class="TableCellsBorderLeft DefaultCellsTitleColumnColors" nowrap="nowrap">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Segments"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Words"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Characters"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Percent"/>
      <xsl:if test="$Percent != ''">%</xsl:if>
    </td>
    <xsl:if test="$ShowWordRate = 'True'">
      <td class="TableCellsNoBorder DefaultCellsColorsWhite">
        <xsl:value-of select="$WordsRate"/>
      </td>
    </xsl:if>
    <td class="TableCellsBorderRight DefaultCellsColorsWhite">
      <xsl:value-of select="$Total"/>
      <xsl:if test="$Total != ''">
        <span style="font-style: italic; color: gray">
          (<xsl:value-of select="$Currency"/>)
        </span>
      </xsl:if>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="PEMPTableTotals">
    <xsl:param name="Title"/>
    <xsl:param name="Segments"/>
    <xsl:param name="Words"/>
    <xsl:param name="Characters"/>
    <xsl:param name="Percent"/>
    <xsl:param name="WordsRate"/>
    <xsl:param name="Total"/>
    <xsl:param name="Currency"/>
    <xsl:param name="ShowWordRate"/>

    <td class="TableCellsBorderTopBottomLeft DefaultCellsTitleColumnColors">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Segments"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Words"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Characters"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Percent"/>
    </td>
    <xsl:if test="$ShowWordRate = 'True'">
      <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
        <!--<xsl:value-of select="$WordsRate"/>-->
        <!--Not used-->
      </td>
    </xsl:if>
    <td class="TableCellsBorderTopRightBottom DefaultCellsTotalsColorsRight" nowrap="nowrap">
      <xsl:value-of select="$Total"/>
      <xsl:if test="$Total != ''">
        <span style="font-style: italic; color: gray">
          (<xsl:value-of select="$Currency"/>)
        </span>
      </xsl:if>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="ConfirmationStatisticsHeaderTop">
    <td colspan="4" class="TableCellsBorderTopRightBottomLeft DefaultCellsSectionHeaderColors">
      Comfirmation Statistics (segments)
    </td>
  </xsl:template>

  <xsl:template name="ConfirmationStatisticsHeaderSub">
    <td  nowrap="nowrap" width="10%"  class="TableCellsBorderTopBottomLeft DefaultCellsSubHeaderColors">
      Confirmation Level
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Original
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Updated
    </td>
    <td class="TableCellsBorderTopRightBottom DefaultCellsSubHeaderColorsCenter">
      %
    </td>
  </xsl:template>

  <xsl:template name="ConfirmationStatisticsTableValues">
    <xsl:param name="Title" />
    <xsl:param name="Original" />
    <xsl:param name="Updated" />
    <xsl:param name="Changes" />

    <td class="TableCellsBorderLeft DefaultCellsTitleColumnColors" nowrap="nowrap">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite" nowrap="nowrap">
      <xsl:value-of select="$Original"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite" nowrap="nowrap">
      <xsl:value-of select="$Updated"/>
    </td>
    <td class="TableCellsBorderRight DefaultCellsColorsWhite" nowrap="nowrap">
      <xsl:value-of select="$Changes"/>
      <xsl:if test="$Changes != ''">%</xsl:if>
    </td>
  </xsl:template>

  <xsl:template name="ConfirmationStatisticsTotals">
    <xsl:param name="Title" />
    <xsl:param name="Changes" />

    <td class="TableCellsBorderTopBottomLeft DefaultCellsTitleColumnColors" nowrap="nowrap">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight" nowrap="nowrap">
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight" nowrap="nowrap">
    </td>
    <td class="TableCellsBorderTopRightBottom DefaultCellsTotalsColorsRight" nowrap="nowrap">
      <xsl:value-of select="$Changes"/>
      <xsl:if test="$Changes != ''">%</xsl:if>
    </td>

  </xsl:template>

  <xsl:template match="qualityMetric">
    <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
      <div  style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;">

        <span style="padding: 3px; font-weight: bold;">
          <xsl:value-of select="@name"/>
        </span>


        <span style="padding: 3px 3px 3px 3px; font-weight: bold; color:#084B8A;">
          <span style=" font-weight: bold; color: gray;">
            (
          </span>
          <xsl:value-of select="@severity"/>
        </span>

        <span style=" font-weight: bold; color: black;">
          <xsl:value-of select="@severityWeight"/>
          <span style=" font-weight: bold; color: gray;">
            )
          </span>
        </span>

        <br/>
        <span style="padding: 3px 4px 3px 3px;">
          <xsl:choose>
            <xsl:when test="@status ='Open'">
              <span style="font-weight: bold;color: DarkBlue;">
                <xsl:value-of select="@status"/>
              </span>
            </xsl:when>
            <xsl:when test="@status ='Resolved'">
              <span style="font-weight: bold;color: DarkGreen;">
                <xsl:value-of select="@status"/>
              </span>
            </xsl:when>
            <xsl:otherwise>
              <span style="font-weight: bold;color: DarkRed;">
                <xsl:value-of select="@status"/>
              </span>
            </xsl:otherwise>
          </xsl:choose>



        </span>



        <span style="padding: 3px; font-style: italic;">
          <xsl:value-of select="@modified"/>
        </span>

        <br/>
        <span style="padding: 3px 0px 3px 3px; ">
          By:
        </span>
        <span style="padding: 3px; color: #084B8A; font-weight: bold;">
          <xsl:value-of select="@userName"/>
        </span>
      </div>

      <xsl:if test="content != ''">
        <p style="margin: 0px; padding: 3px;">
          <span style="padding: 0px; font-weight: normal; color:#585858;">
            Content:&#160;
          </span>
          <xsl:value-of select="content"/>
        </p>
      </xsl:if>
      <xsl:if test="comment != ''">
        <p style="margin: 0px; padding: 3px;">
          <span style="padding: 0px; font-weight: normal; color:#585858;">
            Comment:&#160;
          </span>
          <xsl:value-of select="comment"/>
        </p>
      </xsl:if>

    </div>
  </xsl:template>

  <xsl:template match="revisionMarker">
    <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
      <div  style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;">


        <xsl:choose>
          <xsl:when test="@revisionType = 'Insert'">
            <span style="padding: 3px; color: DarkBlue; font-weight: bold;">
              Inserted
            </span>
          </xsl:when>
          <xsl:when test="@revisionType = 'Delete'">
            <span style="padding: 3px; color: DarkRed; font-weight: bold;">
              Deleted
            </span>
          </xsl:when>
          <xsl:otherwise>
            <span style="revisionType: 3px; font-weight: bold;">
              No Change
            </span>
          </xsl:otherwise>
        </xsl:choose>
        <span style="padding: 3px">
          <span style="font-style: italic; color: DarkBlue">
            (<xsl:value-of select="@date"/>)
          </span>
        </span>

        <br/>
        <span style="padding: 3px; color: black;">
          By:&#160;
        </span>
        <span style="padding: 3px; color: DarkSlateGray;">
          <xsl:value-of select="@author"/>
        </span>



      </div>
      <p style="margin: 0px; padding: 3;">
        <xsl:copy-of select="content"/>
      </p>
    </div>
  </xsl:template>

  <xsl:template match="comment">
    <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
      <div  style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;">
        <xsl:choose>
          <xsl:when test="@severity = 'High'">
            <span style="padding: 3px; color: red; font-weight: bold;">
              <xsl:value-of select="@severity"/>
            </span>
          </xsl:when>
          <xsl:otherwise>
            <span style="padding: 3px; font-weight: bold;">
              <xsl:value-of select="@severity"/>
            </span>
          </xsl:otherwise>
        </xsl:choose>

        <span style="padding: 3px; font-style: italic;">
          <xsl:value-of select="@created"/>
        </span>
        <br/>
        <span style="padding: 3px 0px 3px 3px; ">
          By:
        </span>
        <span style="padding: 3px; color: #084B8A; font-weight: bold;">
          <xsl:value-of select="@author"/>
        </span>

      </div>

      <p style="margin: 0px; padding: 3px;">
        <xsl:value-of select="."/>
      </p>
    </div>
  </xsl:template>

</xsl:stylesheet>
