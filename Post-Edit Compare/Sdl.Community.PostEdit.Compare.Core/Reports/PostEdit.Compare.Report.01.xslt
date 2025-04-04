<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/2000/svg">
  <xsl:output method="html" indent="yes" encoding="UTF-8"/>

  <xsl:template match="/">

    <html>
      <head>
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <title>PostEdit.Compare Comparison Report</title>


        <style type="text/css">

          .custom-dropdown {
          position: absolute;
          display: none;
          opacity: 0;
          visibility: hidden;
          transition: opacity 0.2s ease-in-out;
          background: white;
          border: 1px solid #ccc;
          border-radius: 6px;
          padding: 4px 0;
          box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
          z-index: 1000;
          min-width: 140px;
          font-family: Arial, sans-serif;
          font-size: 12px;
          }

          .dropdown-content div {
          display: flex;
          align-items: center;
          cursor: pointer;
          padding: 2px 10px;
          white-space: nowrap; /* Prevents text wrapping */
          }

          .dropdown-content div:hover {
          background-color: #f0f0f0;
          }

          .dropdown-content div img {
          width: 14px;
          height: 14px;
          margin-right: 6px;
          opacity: 0.8;
          }

          /* Optional: Slightly highlight the selected item */
          .dropdown-content div:active {
          background-color: #e0e0e0;
          }


          .addComment {
          display: grid;
          grid-template-columns: 1fr auto; /* Two columns: one for the input, one for the dropdown */
          gap: 10px; /* Space between input and dropdown */
          }

          .severity-dropdown {
          width: 100%; /* Ensure it takes up the remaining space */
          }

          .new-status {
          border: 1px solid gray;
          background-color: #ffffcc;
          padding: 2px 5px;
          margin-bottom: 2px;
          font-weight: bold;
          display: inline-block;
          }

          .status-cell {
          position: relative;
          padding-top: 17px;
          }

          .status-dropdown {
          position: absolute;
          top: 2px;
          right: 2px;
          font-size: 10px;
          width: 17px;
          padding: 1px;
          }

          .status-dropdown:focus {
          font-size: 12px;
          width: auto; /* Expand width dynamically */
          }

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
          border: 1px solid #C0C0C0;

          }

          td.segmentId
          {
          color: black;
          background-color: #E9E9E9;
          }

          td.innerFileName
          {
          color: gray;
          text-align: center;
          background-color:  #FFF8F0;
          }

          tr.selectedn
          {
          background-color: #ffff66;
          }

          span.text
          {
          color:  Black;
          }

          span.textNew
          {
          color: Blue;
          text-decoration: underline;
          background-color:#ffff66;
          }

          span.textRemoved
          {
          color:  Red;
          text-decoration: line-through;
          }

          span.tag
          {
          color: Gray;
          border: 2px solid #dadada;
          border-radius: 7px;
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
          span.NoWrapItalic
          {
          font-style: italic;
          color:  Gray;
          white-space:nowrap
          }

          span.tagNew
          {
          color: Gray;
          background-color: #DDEEFF;
          border: 2px solid #dadada;
          border-radius: 7px;
          }

          span.tagRemoved
          {
          color: Gray;
          border: #FFE8E8;
          border: 2px solid #dadada;
          border-radius: 7px;
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
          padding-top: 2px
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
          .DefaultPaddingRight
          {
          padding: 2px 2px 2px 2px; text-align: Right;
          }
          .DefaultPaddingBold
          {
          padding: 2px 2px 2px 2px;font-weight: Bold;
          }
          .DefaultFileSectionHeader
          {
          color: #FFFFFF; text-align: left;  background-color: #006699; font-weight: bold;
          }
          .DefaultNoLeftRightPadding
          {
          padding: 2px 0px 2px 0px;font-weight: normal;
          }
          .TextAlignCenter
          {
          text-align: center;

          }
          .TextAlignRight
          {
          text-align: right;
          }
          .TextFontGrayItalicSmaller
          {
          font-style: italic;
          color: gray;
          }


        </style>

        <!--<link href="SdlXliffCompareReportType01.css"
               rel="stylesheet"
               type="text/css" />-->
        <script type="text/javascript" src="http://www.google.com/jsapi">
          .
        </script>
        <script type="text/javascript">
          <!--google.charts.load('current', {'packages':['corechart']});-->
          google.load('visualization', '1', {packages: ['corechart']});

        </script>

      </head>
      <body>
        <xsl:apply-templates select="files"/>
        <script type="text/javascript">
          <!--google.setOnLoadCallback(drawVisualization);-->
        </script>

      </body>
    </html>

  </xsl:template>

  <xsl:template match="files">
    <xsl:variable name="IncludeHeaderTitle" select="filter/@includeHeaderTitle"/>
    <xsl:variable name="viewFilewWithNoRecords" select="filter/@viewFilewWithNoRecords"/>
    <xsl:variable name="showGoogleCharts" select="filter/@showGoogleCharts"/>
    <xsl:variable name="calculateSummaryAnalysisBasedOnFilteredRows" select="filter/@calculateSummaryAnalysisBasedOnFilteredRows"/>

    <xsl:variable name="showOriginalSourceSegment" select="filter/@showOriginalSourceSegment"/>
    <xsl:variable name="showOriginalTargetSegment" select="filter/@showOriginalTargetSegment"/>
    <xsl:variable name="showOriginalRevisionMarkersTargetSegment" select="filter/@showOriginalRevisionMarkersTargetSegment"/>
    <xsl:variable name="showUpdatedTargetSegment" select="filter/@showUpdatedTargetSegment"/>
    <xsl:variable name="showUpdatedRevisionMarkersTargetSegment" select="filter/@showUpdatedRevisionMarkersTargetSegment"/>
    <xsl:variable name="showTargetComparison" select="filter/@showTargetComparison"/>
    <xsl:variable name="showSegmentComments" select="filter/@showSegmentComments"/>
    <xsl:variable name="showSegmentLocked" select="filter/@showSegmentLocked"/>

    <xsl:variable name="showSegmentStatus" select="filter/@showSegmentStatus"/>
    <xsl:variable name="showSegmentMatch" select="filter/@showSegmentMatch"/>
    <xsl:variable name="showSegmentTerp" select="filter/@showSegmentTerp"/>
    <xsl:variable name="showSegmentPemp" select="filter/@showSegmentPemp"/>

    <a name="filesId_report_header"> </a>

    <table style="background-color: #CEE3F6" border="1" width="100%">
      <tr>
        <td width="70%" class="TableCellsBorderTopBottomLeft DefaultPadding TextAlignCenter">

          <span class="DefaultPadding">
            <xsl:attribute name="style">
              font-size: 20px;
            </xsl:attribute>
            Post-Edit Comparison Report
          </span>

          <br/>
          <span class="DefaultPadding TextFontGrayItalicSmaller">
            <span >
              <xsl:value-of select="@dateCompared"/>
            </span>
            <span>
              <span >
                (Processed:
              </span>
              <span >
                <xsl:value-of select="@count"/>,
              </span>
              <span >
                Compared:
              </span>
              <span>
                <xsl:value-of select="@totalfilesCompared"/>,
              </span>
              <span>
                Errors:
              </span>
              <span>
                <xsl:value-of select="@totalFilesWithErrors"/>)
              </span>
            </span>
          </span>
        </td>

        <td width="100" class="TableCellsBorderTopRightBottom DefaultPadding TextAlignRight">
          <span class="DefaultPadding">
            <span style="color: gray">Generated by: </span>
            <span style="color: #6E6E6E; ">Post-Edit Compare</span>
          </span>
          <br/>
        </td>
      </tr>
    </table>
    <br/>

    <xsl:apply-templates select="filesTotal">
      <xsl:with-param name="showGoogleCharts" select="$showGoogleCharts"/>
    </xsl:apply-templates>

    <a name="filesId_report_files"> </a>
    <xsl:apply-templates select="file">
      <xsl:with-param name="viewFilewWithNoRecords" select="$viewFilewWithNoRecords"/>
      <xsl:with-param name="showGoogleCharts" select="$showGoogleCharts"/>
      <xsl:with-param name="calculateSummaryAnalysisBasedOnFilteredRows" select="$calculateSummaryAnalysisBasedOnFilteredRows"/>

      <xsl:with-param name="showOriginalSourceSegment" select="$showOriginalSourceSegment"/>
      <xsl:with-param name="showOriginalTargetSegment" select="$showOriginalTargetSegment"/>
      <xsl:with-param name="showOriginalRevisionMarkersTargetSegment" select="$showOriginalRevisionMarkersTargetSegment"/>
      <xsl:with-param name="showUpdatedTargetSegment" select="$showUpdatedTargetSegment"/>
      <xsl:with-param name="showUpdatedRevisionMarkersTargetSegment" select="$showUpdatedRevisionMarkersTargetSegment"/>
      <xsl:with-param name="showTargetComparison" select="$showTargetComparison"/>
      <xsl:with-param name="showSegmentComments" select="$showSegmentComments"/>
      <xsl:with-param name="showSegmentLocked" select="$showSegmentLocked"/>

      <xsl:with-param name="showSegmentStatus" select="$showSegmentStatus"/>
      <xsl:with-param name="showSegmentMatch" select="$showSegmentMatch"/>
      <xsl:with-param name="showSegmentTerp" select="$showSegmentTerp"/>
      <xsl:with-param name="showSegmentPemp" select="$showSegmentPemp"/>

    </xsl:apply-templates>

  </xsl:template>

  <xsl:template match="filesTotal">
    <xsl:param name="showGoogleCharts" />
    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="@id"/>
      </xsl:attribute>
      <xsl:text> </xsl:text>
    </a>

    <xsl:call-template name="ReportTotalsArea">
      <xsl:with-param name="showGoogleCharts" select="$showGoogleCharts"/>
    </xsl:call-template>

    <br/>
    <br/>
  </xsl:template>

  <xsl:template match="file">
    <xsl:param name="viewFilewWithNoRecords" />
    <xsl:param name="showGoogleCharts" />
    <xsl:param name="calculateSummaryAnalysisBasedOnFilteredRows" />
    <xsl:param name="showOriginalSourceSegment" />
    <xsl:param name="showOriginalTargetSegment" />
    <xsl:param name="showOriginalRevisionMarkersTargetSegment" />
    <xsl:param name="showUpdatedTargetSegment" />
    <xsl:param name="showUpdatedRevisionMarkersTargetSegment" />
    <xsl:param name="showTargetComparison" />
    <xsl:param name="showSegmentComments" />
    <xsl:param name="showSegmentLocked" />

    <xsl:param name="showSegmentStatus" />
    <xsl:param name="showSegmentMatch" />
    <xsl:param name="showSegmentTerp" />
    <xsl:param name="showSegmentPemp" />

    <a>
      <xsl:attribute name="name">
        <xsl:value-of select="@id"/>
      </xsl:attribute>
      <xsl:text> </xsl:text>
    </a>

    <table class="TableTopLevelStyle" border="0" width="100%" >
      <tr>
        <td class="DefaultFileOverviewHeader">
          <span class ="DefaultPadding">Total Segments:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="totals/@segments"/>
            <span class="DefaultNoLeftRightPadding">, </span>
          </span>
          <span class ="DefaultPadding">Translation Modifications:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="totals/@contentChanges"/>
          </span>
          <span class ="DefaultPaddingBold">
            (<xsl:value-of select="totals/@contentChangesPercentage"/>%)
            <span class="DefaultNoLeftRightPadding">, </span>
          </span>
          <span class ="DefaultPadding">Status Changes:</span>
          <span class="DefaultPaddingBold" >
            <xsl:value-of select="totals/@statusChanges"/>
          </span>
          <span style="padding: 2px 2px 2px 1px">
            (<xsl:value-of select="totals/@statusChangesPercentage"/>%)
            <span class="DefaultNoLeftRightPadding">, </span>
          </span>
          <span class ="DefaultPadding">Comments:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="totals/@comments"/>
          </span>
        </td>
      </tr>
    </table>

    <table class="TableTopLevelStyle" border="1" width="100%">
      <xsl:attribute name="style">
        margin-bottom: 5px;
      </xsl:attribute>
      <tr class="DefaultFileSectionHeader">
        <td class="DefaultPadding">
          Versions
        </td>
        <td class="TableCellsBorderTopBottomLeft DefaultPadding">
          Language
        </td>
        <td width="55%" class="TableCellsBorderTopBottom DefaultPadding">
          File path
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Not Translated
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Draft
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Translated
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Translation Rejected
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Translation Approved
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Sign-off Rejected
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          Signed Off
        </td>
      </tr>

      <tr style="background-color: #F2F2F2">
        <td class="TableCellsBorderTopBottomLeft DefaultPadding">
          Original
        </td>
        <td class="TableCellsBorderTopBottom DefaultPadding">
          <xsl:value-of select="@fileOriginalLanguageId"/>
        </td>
        <td width="55%" class="TableCellsBorderTopBottom DefaultPadding">
          <xsl:value-of select="@filePathOriginal"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@notTranslatedOriginal"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@draftOriginal"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@translatedOriginal"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@translationRejectedOriginal"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@translationApprovedOriginal"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@signOffRejectedOriginal"/>
        </td>
        <td class="TableCellsBorderTopRightBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@signedOffOriginal"/>
        </td>
      </tr>

      <tr style="background-color: #F2F2F2">
        <td class="TableCellsBorderTopBottomLeft DefaultPadding">
          Updated
        </td>
        <td class="TableCellsBorderTopBottom DefaultPadding">
          <xsl:value-of select="@fileTargetLanguageId"/>
        </td>
        <td width="55%" class="TableCellsBorderTopBottom DefaultPadding">
          <xsl:value-of select="@filePathUpdated"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@notTranslatedUpdated"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@draftUpdated"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@translatedUpdated"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@translationRejectedUpdated"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@translationApprovedUpdated"/>
        </td>
        <td class="TableCellsBorderTopBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@signOffRejectedUpdated"/>
        </td>
        <td class="TableCellsBorderTopRightBottom DefaultPaddingRight">
          <xsl:value-of select="confirmationStatistics/@signedOffUpdated"/>
        </td>
      </tr>
    </table>


    <xsl:if test="not(@filePathOriginal = '') and not(@filePathUpdated = '')">
      <xsl:call-template name="ReportFileTotalsArea">
        <xsl:with-param name="showGoogleCharts" select="$showGoogleCharts"/>
        <xsl:with-param name="fileId" select="@id"/>
      </xsl:call-template>
    </xsl:if>

    <table class="TableTopLevelStyle" border="0" width="100%">
      <xsl:attribute name="style">
        margin-top: 3px;margin-bottom: 1px;
      </xsl:attribute>
      <tr>
        <td class="DefaultPadding">
          <span class="DefaultPadding">Filtered Segments:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="filtered/@segments"/>
            <span class="DefaultNoLeftRightPadding">, </span>
          </span>

          <span class="DefaultPadding">Translation Modifications:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="filtered/@contentChanges"/>
            <span class="DefaultNoLeftRightPadding">, </span>
          </span>

          <span class="DefaultPadding">Status Changes:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="filtered/@statusChanges"/>
            <span class="DefaultNoLeftRightPadding">, </span>
          </span>

          <span class="DefaultPadding">Comments:</span>
          <span class="DefaultPaddingBold">
            <xsl:value-of select="filtered/@comments"/>
          </span>
        </td>
      </tr>
    </table>

    <xsl:choose>
      <xsl:when test="@filePathOriginal = ''">
        <!--error-->
        <div style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; font-weight bold; color: #FF0000" >
          Error: Unable to locate the original file
        </div>

      </xsl:when>
      <xsl:when test="@filePathUpdated = ''">
        <!--error-->
        <div style="font-family: Arial, Helvetica, sans-serif; font-size: 14px; font-weight bold; color: #FF0000" >
          Error: Unable to locate the updated file
        </div>
      </xsl:when>
      <xsl:otherwise>
        <xsl:for-each select="innerFiles">
          <xsl:if test="@count > '0' ">
            <table border="1" cellpadding="3"  cellspacing="0" width="100%">
              <tr>
                <th>ID</th>
                <th>TM</th>
                <th>TU</th>
                <xsl:if test="$showSegmentLocked = 'True'">
                  <th>Lock</th>
                </xsl:if>
                <xsl:if test="$showOriginalSourceSegment = 'True'">
                  <th>Source</th>
                  <th>Words</th>
                </xsl:if>
                <xsl:if test="$showSegmentStatus = 'True'">
                  <th>Status</th>
                </xsl:if>
                <xsl:if test="$showSegmentMatch = 'True'">
                  <th>Match</th>
                </xsl:if>
                <xsl:if test="$showOriginalTargetSegment = 'True'">
                  <th>Target (Original)</th>
                </xsl:if>
                <xsl:if test="$showOriginalRevisionMarkersTargetSegment = 'True'">
                  <th>Target (Original) Track Changes</th>
                </xsl:if>
                <xsl:if test="$showUpdatedTargetSegment = 'True'">
                  <th>Target (Updated)</th>
                </xsl:if>
                <xsl:if test="$showUpdatedRevisionMarkersTargetSegment = 'True'">
                  <th>Target (Updated) Track Changes</th>
                </xsl:if>
                <xsl:if test="$showTargetComparison = 'True'">
                  <th>Target (Comparison)</th>
                </xsl:if>
                <xsl:if test="$showSegmentPemp = 'True'">
                  <th>PEMp</th>
                </xsl:if>
                <xsl:if test="$showSegmentTerp = 'True'">
                  <th>TERp</th>
                </xsl:if>
                <xsl:if test="$showSegmentComments = 'True'">
                  <th>Comments</th>
                </xsl:if>
              </tr>
              <xsl:for-each select="innerFile">

                <xsl:if test="@showInnerFileName = 'true'">
                  <tr>

                    <td colspan="12" class="innerFileName">
                      <a>
                        <xsl:attribute name="name">
                          <xsl:value-of select="@id"/>
                        </xsl:attribute>
                        <xsl:text> </xsl:text>
                      </a>
                      <xsl:value-of select="@name"/>
                      <span class="DefaultPadding">
                        (filtered: <xsl:value-of select="@filteredSegmentCount"/>)
                      </span>
                    </td>


                  </tr>
                </xsl:if>
                <xsl:apply-templates select="paragraphs">
                  <xsl:with-param name="showOriginalSourceSegment" select="$showOriginalSourceSegment"/>
                  <xsl:with-param name="showOriginalTargetSegment" select="$showOriginalTargetSegment"/>
                  <xsl:with-param name="showOriginalRevisionMarkersTargetSegment" select="$showOriginalRevisionMarkersTargetSegment"/>
                  <xsl:with-param name="showUpdatedTargetSegment" select="$showUpdatedTargetSegment"/>
                  <xsl:with-param name="showUpdatedRevisionMarkersTargetSegment" select="$showUpdatedRevisionMarkersTargetSegment"/>
                  <xsl:with-param name="showTargetComparison" select="$showTargetComparison"/>
                  <xsl:with-param name="showSegmentComments" select="$showSegmentComments"/>
                  <xsl:with-param name="showSegmentLocked" select="$showSegmentLocked"/>

                  <xsl:with-param name="showSegmentStatus" select="$showSegmentStatus"/>
                  <xsl:with-param name="showSegmentMatch" select="$showSegmentMatch"/>
                  <xsl:with-param name="showSegmentTerp" select="$showSegmentTerp"/>
                  <xsl:with-param name="showSegmentPemp" select="$showSegmentPemp"/>


                </xsl:apply-templates>
              </xsl:for-each>
            </table>
          </xsl:if>
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>

    <br/>
    <br/>
    <br/>

  </xsl:template>

  <xsl:template name="ReportTotalsArea">
    <xsl:param name="showGoogleCharts" />
    <table class="TableTopLevelStyle" border="0" width="100%">
      <tr>

        <xsl:call-template name="TranslationModificationsTableHeaderTop">
        </xsl:call-template>

        <xsl:call-template name="PEMPTableHeaderTop">
          <xsl:with-param name="ShowWordRate" select='"False"'/>

        </xsl:call-template>

        <xsl:call-template name="TERPTableHeaderTop">
        </xsl:call-template>

        <xsl:call-template name="ConfirmationStatisticsHeaderTop">
        </xsl:call-template>

      </tr>
      <tr>
        <xsl:call-template name="TranslationModificationsTableHeaderSub">
        </xsl:call-template>

        <xsl:call-template name="PEMPTableHeaderSub">
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>

        <xsl:call-template name="TERPTableHeaderSub">
        </xsl:call-template>

        <xsl:call-template name="ConfirmationStatisticsHeaderSub">
        </xsl:call-template>


      </tr>
      <tr>

        <xsl:call-template name="TranslationModificationsTableValues">
          <xsl:with-param name="Title" select='"Perfect Match"'/>
          <xsl:with-param name="SourceSegments" select="translationModifications/@sourcePMSegments"/>
          <xsl:with-param name="SourceWords" select="translationModifications/@sourcePMWords"/>
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesPMSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesPMWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesPMCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"0%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp00Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp00SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp00NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp00NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp00Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp00Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp00Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp00Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp00Cap"/>

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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesCMSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesCMWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesCMCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"01% - 05%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp01Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp01SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp01NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp01NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp01Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp01Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp01Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp01Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp01Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesRepsSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesRepsWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesRepsCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"06% - 09%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp06Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp06SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp06NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp06NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp06Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp06Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp06Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp06Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp06Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesExactSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesExactWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesExactCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"10% - 19%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp10Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp10SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp10NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp10NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp10Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp10Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp10Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp10Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp10Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy99Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy99Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy99Characters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"20% - 29%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp20Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp20SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp20NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp20NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp20Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp20Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp20Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp20Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp20Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy94Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy94Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy94Characters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"30% - 39%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp30Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp30SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp30NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp30NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp30Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp30Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp30Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp30Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp30Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy84Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy84Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy84Characters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"40% - 49%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp40Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp40SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp40NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp40NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp40Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp40Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp40Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp40Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp40Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy74Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy74Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy74Characters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"50% +"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp50Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp50SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp50NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp50NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp50Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp50Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp50Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp50Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp50Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesNewSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesNewWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesNewCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='""'/>
          <xsl:with-param name="Segments" select='""'/>
          <xsl:with-param name="SrcWd" select='""'/>
          <xsl:with-param name="NumWd" select='""'/>
          <xsl:with-param name="NumEr" select='""'/>
          <xsl:with-param name="Ins" select='""'/>
          <xsl:with-param name="Del" select='""'/>
          <xsl:with-param name="Sub" select='""'/>
          <xsl:with-param name="Shft" select='""'/>
          <xsl:with-param name="Cap" select='""'/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesATSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesATWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesATCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='""'/>
          <xsl:with-param name="Segments" select='""'/>
          <xsl:with-param name="SrcWd" select='""'/>
          <xsl:with-param name="NumWd" select='""'/>
          <xsl:with-param name="NumEr" select='""'/>
          <xsl:with-param name="Ins" select='""'/>
          <xsl:with-param name="Del" select='""'/>
          <xsl:with-param name="Sub" select='""'/>
          <xsl:with-param name="Shft" select='""'/>
          <xsl:with-param name="Cap" select='""'/>
        </xsl:call-template>

        <xsl:call-template name="ConfirmationStatisticsTableValues">
          <xsl:with-param name="Title" select='""'/>
          <xsl:with-param name="Original" select='""'/>
          <xsl:with-param name="Updated" select='""'/>
          <xsl:with-param name="Changes" select='""'/>
        </xsl:call-template>

      </tr>
      <tr >


        <xsl:call-template name="TranslationModificationsTableTotals">
          <xsl:with-param name="Title" select='"Total"'/>
          <xsl:with-param name="SourceSegments" select="translationModifications/@sourceTotalSegments"/>
          <xsl:with-param name="SourceWords" select="translationModifications/@sourceTotalWords"/>
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesTotalSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesTotalWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesTotalCharacters"/>
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
          <xsl:with-param name="ShowWordRate" select='"False"'/>
        </xsl:call-template>


        <xsl:call-template name="TERPTableTotals">
          <xsl:with-param name="Title" select='"Total"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp00Segments + terpAnalysis/@terp01Segments + terpAnalysis/@terp06Segments + terpAnalysis/@terp10Segments + terpAnalysis/@terp20Segments + terpAnalysis/@terp30Segments + terpAnalysis/@terp40Segments + terpAnalysis/@terp50Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp00SrcWd + terpAnalysis/@terp01SrcWd + terpAnalysis/@terp06SrcWd + terpAnalysis/@terp10SrcWd + terpAnalysis/@terp20SrcWd + terpAnalysis/@terp30SrcWd + terpAnalysis/@terp40SrcWd + terpAnalysis/@terp50SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp00NumWd + terpAnalysis/@terp01NumWd + terpAnalysis/@terp06NumWd + terpAnalysis/@terp10NumWd + terpAnalysis/@terp20NumWd + terpAnalysis/@terp30NumWd + terpAnalysis/@terp40NumWd + terpAnalysis/@terp50NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp00NumEr + terpAnalysis/@terp01NumEr + terpAnalysis/@terp06NumEr + terpAnalysis/@terp10NumEr + terpAnalysis/@terp20NumEr + terpAnalysis/@terp30NumEr + terpAnalysis/@terp40NumEr + terpAnalysis/@terp50NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp00Ins + terpAnalysis/@terp01Ins + terpAnalysis/@terp06Ins + terpAnalysis/@terp10Ins + terpAnalysis/@terp20Ins + terpAnalysis/@terp30Ins + terpAnalysis/@terp40Ins + terpAnalysis/@terp50Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp00Del + terpAnalysis/@terp01Del + terpAnalysis/@terp06Del + terpAnalysis/@terp10Del + terpAnalysis/@terp20Del + terpAnalysis/@terp30Del + terpAnalysis/@terp40Del + terpAnalysis/@terp50Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp00Sub + terpAnalysis/@terp01Sub + terpAnalysis/@terp06Sub + terpAnalysis/@terp10Sub + terpAnalysis/@terp20Sub + terpAnalysis/@terp30Sub + terpAnalysis/@terp40Sub + terpAnalysis/@terp50Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp00Shft + terpAnalysis/@terp01Shft + terpAnalysis/@terp06Shft + terpAnalysis/@terp10Shft + terpAnalysis/@terp20Shft + terpAnalysis/@terp30Shft + terpAnalysis/@terp40Shft + terpAnalysis/@terp50Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp00Cap + terpAnalysis/@terp01Cap + terpAnalysis/@terp06Cap + terpAnalysis/@terp10Cap + terpAnalysis/@terp20Cap + terpAnalysis/@terp30Cap + terpAnalysis/@terp40Cap + terpAnalysis/@terp50Cap"/>
        </xsl:call-template>

        <xsl:call-template name="ConfirmationStatisticsTotals">
          <xsl:with-param name="Title" select='"Total"'/>
          <xsl:with-param name="Changes" select="totals/@statusChangesPercentage"/>
        </xsl:call-template>

      </tr>
      <xsl:if test="$showGoogleCharts = 'True'">
        <tr>

          <td colspan="7" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
            <div id="TranslationModificaitonsColumnChart" ></div>
          </td>

          <td class="TableCellsNoBorder DefaultCellsColorsWhite" border="0">
            &#160;
          </td>

          <td colspan="3" class="TableCellsBorderTopBottomLeft DefaultCellsColorsWhite">
            <div id="PEMpColumnChart" ></div>
          </td>
          <td colspan="3" class="TableCellsBorderTopRightBottom DefaultCellsColorsWhite">
            <div id="PEMpPieChart" ></div>
          </td>

          <td class="TableCellsNoBorder DefaultCellsColorsWhite" border="0">
            &#160;
          </td>

          <td colspan="3" class="TableCellsBorderTopBottomLeft DefaultCellsColorsWhite">
            <div id="TERpColumnChart" ></div>
          </td>
          <td colspan="6" class="TableCellsBorderTopRightBottom DefaultCellsColorsWhite">
            <div id="TERpPieChart" ></div>
          </td>

          <td class="TableCellsNoBorder DefaultCellsColorsWhite" border="0">
            &#160;
          </td>

          <td colspan="4" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
            <div id="ConfirmationStatisticsBarChart" ></div>
          </td>

        </tr>
      </xsl:if>
    </table>
  </xsl:template>

  <xsl:template name="ReportFileTotalsArea">
    <xsl:param name="showGoogleCharts" />
    <xsl:param name="fileId" />
    <table class="TableTopLevelStyle" border="0" width="100%">
      <tr>

        <xsl:call-template name="TranslationModificationsTableHeaderTop">
        </xsl:call-template>

        <xsl:call-template name="PEMPTableHeaderTop">
          <xsl:with-param name="ShowWordRate" select='"True"'/>
        </xsl:call-template>

        <xsl:call-template name="TERPTableHeaderTop">
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

        <xsl:call-template name="TERPTableHeaderSub">
        </xsl:call-template>


        <xsl:call-template name="ConfirmationStatisticsHeaderSub">
        </xsl:call-template>

      </tr>
      <tr>

        <xsl:call-template name="TranslationModificationsTableValues">
          <xsl:with-param name="Title" select='"Perfect Match"'/>
          <xsl:with-param name="SourceSegments" select="translationModifications/@sourcePMSegments"/>
          <xsl:with-param name="SourceWords" select="translationModifications/@sourcePMWords"/>
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesPMSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesPMWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesPMCharacters"/>
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

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"0%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp00Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp00SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp00NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp00NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp00Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp00Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp00Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp00Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp00Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesCMSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesCMWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesCMCharacters"/>
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


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"01% - 05%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp01Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp01SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp01NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp01NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp01Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp01Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp01Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp01Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp01Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesRepsSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesRepsWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesRepsCharacters"/>
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


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"06% - 09%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp06Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp06SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp06NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp06NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp06Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp06Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp06Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp06Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp06Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesExactSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesExactWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesExactCharacters"/>
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

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"10% - 19%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp10Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp10SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp10NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp10NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp10Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp10Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp10Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp10Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp10Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy99Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy99Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy99Characters"/>
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


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"20% - 29%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp20Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp20SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp20NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp20NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp20Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp20Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp20Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp20Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp20Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy94Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy94Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy94Characters"/>
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


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"30% - 39%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp30Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp30SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp30NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp30NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp30Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp30Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp30Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp30Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp30Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy84Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy84Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy84Characters"/>
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


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"40% - 49%"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp40Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp40SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp40NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp40NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp40Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp40Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp40Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp40Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp40Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesFuzzy74Segments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesFuzzy74Words"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesFuzzy74Characters"/>
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

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='"50% +"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp50Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp50SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp50NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp50NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp50Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp50Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp50Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp50Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp50Cap"/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesNewSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesNewWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesNewCharacters"/>
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

        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='""'/>
          <xsl:with-param name="Segments" select='""'/>
          <xsl:with-param name="SrcWd" select='""'/>
          <xsl:with-param name="NumWd" select='""'/>
          <xsl:with-param name="NumEr" select='""'/>
          <xsl:with-param name="Ins" select='""'/>
          <xsl:with-param name="Del" select='""'/>
          <xsl:with-param name="Sub" select='""'/>
          <xsl:with-param name="Shft" select='""'/>
          <xsl:with-param name="Cap" select='""'/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesATSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesATWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesATCharacters"/>
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


        <xsl:call-template name="TERPTableValues">
          <xsl:with-param name="Title" select='""'/>
          <xsl:with-param name="Segments" select='""'/>
          <xsl:with-param name="SrcWd" select='""'/>
          <xsl:with-param name="NumWd" select='""'/>
          <xsl:with-param name="NumEr" select='""'/>
          <xsl:with-param name="Ins" select='""'/>
          <xsl:with-param name="Del" select='""'/>
          <xsl:with-param name="Sub" select='""'/>
          <xsl:with-param name="Shft" select='""'/>
          <xsl:with-param name="Cap" select='""'/>
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
          <xsl:with-param name="ChangedSegments" select="translationModifications/@changesTotalSegments"/>
          <xsl:with-param name="ChangedWords" select="translationModifications/@changesTotalWords"/>
          <xsl:with-param name="ChangedCharacters" select="translationModifications/@changesTotalCharacters"/>
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


        <xsl:call-template name="TERPTableTotals">
          <xsl:with-param name="Title" select='"Total"'/>
          <xsl:with-param name="Segments" select="terpAnalysis/@terp00Segments + terpAnalysis/@terp01Segments + terpAnalysis/@terp06Segments + terpAnalysis/@terp10Segments + terpAnalysis/@terp20Segments + terpAnalysis/@terp30Segments + terpAnalysis/@terp40Segments + terpAnalysis/@terp50Segments"/>
          <xsl:with-param name="SrcWd" select="terpAnalysis/@terp00SrcWd + terpAnalysis/@terp01SrcWd + terpAnalysis/@terp06SrcWd + terpAnalysis/@terp10SrcWd + terpAnalysis/@terp20SrcWd + terpAnalysis/@terp30SrcWd + terpAnalysis/@terp40SrcWd + terpAnalysis/@terp50SrcWd"/>
          <xsl:with-param name="NumWd" select="terpAnalysis/@terp00NumWd + terpAnalysis/@terp01NumWd + terpAnalysis/@terp06NumWd + terpAnalysis/@terp10NumWd + terpAnalysis/@terp20NumWd + terpAnalysis/@terp30NumWd + terpAnalysis/@terp40NumWd + terpAnalysis/@terp50NumWd"/>
          <xsl:with-param name="NumEr" select="terpAnalysis/@terp00NumEr + terpAnalysis/@terp01NumEr + terpAnalysis/@terp06NumEr + terpAnalysis/@terp10NumEr + terpAnalysis/@terp20NumEr + terpAnalysis/@terp30NumEr + terpAnalysis/@terp40NumEr + terpAnalysis/@terp50NumEr"/>
          <xsl:with-param name="Ins" select="terpAnalysis/@terp00Ins + terpAnalysis/@terp01Ins + terpAnalysis/@terp06Ins + terpAnalysis/@terp10Ins + terpAnalysis/@terp20Ins + terpAnalysis/@terp30Ins + terpAnalysis/@terp40Ins + terpAnalysis/@terp50Ins"/>
          <xsl:with-param name="Del" select="terpAnalysis/@terp00Del + terpAnalysis/@terp01Del + terpAnalysis/@terp06Del + terpAnalysis/@terp10Del + terpAnalysis/@terp20Del + terpAnalysis/@terp30Del + terpAnalysis/@terp40Del + terpAnalysis/@terp50Del"/>
          <xsl:with-param name="Sub" select="terpAnalysis/@terp00Sub + terpAnalysis/@terp01Sub + terpAnalysis/@terp06Sub + terpAnalysis/@terp10Sub + terpAnalysis/@terp20Sub + terpAnalysis/@terp30Sub + terpAnalysis/@terp40Sub + terpAnalysis/@terp50Sub"/>
          <xsl:with-param name="Shft" select="terpAnalysis/@terp00Shft + terpAnalysis/@terp01Shft + terpAnalysis/@terp06Shft + terpAnalysis/@terp10Shft + terpAnalysis/@terp20Shft + terpAnalysis/@terp30Shft + terpAnalysis/@terp40Shft + terpAnalysis/@terp50Shft"/>
          <xsl:with-param name="Cap" select="terpAnalysis/@terp00Cap + terpAnalysis/@terp01Cap + terpAnalysis/@terp06Cap + terpAnalysis/@terp10Cap + terpAnalysis/@terp20Cap + terpAnalysis/@terp30Cap + terpAnalysis/@terp40Cap + terpAnalysis/@terp50Cap"/>
        </xsl:call-template>

        <xsl:call-template name="ConfirmationStatisticsTotals">
          <xsl:with-param name="Title" select='"Total"'/>
          <xsl:with-param name="Changes" select="totals/@statusChangesPercentage"/>
        </xsl:call-template>

      </tr>
      <xsl:if test="$showGoogleCharts = 'True'">
        <tr>

          <td colspan="7" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
            <div>
              <xsl:attribute name="id">
                <xsl:text>TranslationModificaitonsColumnChart_</xsl:text>
                <xsl:value-of select="$fileId"/>
              </xsl:attribute>
            </div>
          </td>

          <td class="TableCellsNoBorder DefaultCellsColorsWhite" border="0">
            &#160;
          </td>

          <td colspan="7" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
            <div>
              <xsl:attribute name="id">
                <xsl:text>PEMpLineChart_</xsl:text>
                <xsl:value-of select="$fileId"/>
              </xsl:attribute>
            </div>
          </td>

          <td class="TableCellsNoBorder DefaultCellsColorsWhite" border="0">
            &#160;
          </td>

          <td colspan="10" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
            <div>
              <xsl:attribute name="id">
                <xsl:text>TERpLineChart_</xsl:text>
                <xsl:value-of select="$fileId"/>
              </xsl:attribute>
            </div>
          </td>

          <td class="TableCellsNoBorder DefaultCellsColorsWhite" border="0">
            &#160;
          </td>

          <td colspan="4" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
            <div>
              <xsl:attribute name="id">
                <xsl:text>ConfirmationStatisticsBarChart_</xsl:text>
                <xsl:value-of select="$fileId"/>
              </xsl:attribute>
            </div>
          </td>

        </tr>
      </xsl:if>
    </table>
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


    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TranslationModificationsTableValues">
    <xsl:param name="Title"/>
    <xsl:param name="SourceSegments"/>
    <xsl:param name="SourceWords"/>
    <xsl:param name="ChangedSegments"/>
    <xsl:param name="ChangedWords"/>
    <xsl:param name="ChangedCharacters"/>

    <td class="TableCellsBorderLeft DefaultCellsTitleColumnColors" nowrap="nowrap" >
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SourceSegments"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SourceWords"/>
    </td>
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

    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TranslationModificationsTableTotals">
    <xsl:param name="Title"/>
    <xsl:param name="SourceSegments"/>
    <xsl:param name="SourceWords"/>
    <xsl:param name="ChangedSegments"/>
    <xsl:param name="ChangedWords"/>
    <xsl:param name="ChangedCharacters"/>

    <td class="TableCellsBorderTopBottomLeft DefaultCellsTitleColumnColors">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SourceSegments"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SourceWords"/>
    </td>
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
        <xsl:value-of select="$WordsRate"/>
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

  <xsl:template name="TERPTableHeaderTop">
    <td width="10%" colspan="10" class="TableCellsBorderTopRightBottomLeft DefaultCellsSectionHeaderColors" nowrap="nowrap">
      TERp Analysis
    </td>

    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>

  </xsl:template>

  <xsl:template name="TERPTableHeaderSub">

    <td colspan="1" width="15%" class="TableCellsBorderTopBottomLeft DefaultCellsSubHeaderColors">
      Range
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Segments
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Words
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Ref. Words
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Errors
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Ins
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Del
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Sub
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsSubHeaderColorsRight">
      Shft
    </td>
    <td class="TableCellsBorderTopRightBottom DefaultCellsSubHeaderColorsRight">
      Cap
    </td>


    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TERPTableValues">
    <xsl:param name="Title"/>
    <xsl:param name="Segments" />
    <xsl:param name="SrcWd" />
    <xsl:param name="NumWd" />
    <xsl:param name="NumEr" />
    <xsl:param name="Ins" />
    <xsl:param name="Del" />
    <xsl:param name="Sub" />
    <xsl:param name="Shft" />
    <xsl:param name="Cap" />

    <td class="TableCellsBorderLeft DefaultCellsTitleColumnColors" nowrap="nowrap">
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Segments"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$SrcWd"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$NumWd"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$NumEr"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Ins"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Del"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Sub"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      <xsl:value-of select="$Shft"/>
    </td>
    <td class="TableCellsBorderRight DefaultCellsColorsWhite">
      <xsl:value-of select="$Cap"/>
    </td>
    <td class="TableCellsNoBorder DefaultCellsColorsWhite">
      &#160;
    </td>
  </xsl:template>

  <xsl:template name="TERPTableTotals">
    <xsl:param name="Title"/>
    <xsl:param name="Segments" />
    <xsl:param name="SrcWd" />
    <xsl:param name="NumWd" />
    <xsl:param name="NumEr" />
    <xsl:param name="Ins" />
    <xsl:param name="Del" />
    <xsl:param name="Sub" />
    <xsl:param name="Shft" />
    <xsl:param name="Cap" />

    <td class="TableCellsBorderTopBottomLeft DefaultCellsTitleColumnColors" nowrap="nowrap" >
      <xsl:value-of select="$Title"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Segments"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$SrcWd"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$NumWd"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$NumEr"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Ins"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Del"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight">
      <xsl:value-of select="$Sub"/>
    </td>
    <td class="TableCellsBorderTopBottom DefaultCellsTotalsColorsRight"  >
      <xsl:value-of select="$Shft"/>
    </td>
    <td class="TableCellsBorderTopRightBottom DefaultCellsTotalsColorsRight" nowrap="nowrap">
      <xsl:value-of select="$Cap"/>
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



  <xsl:template match="barGraph">

  </xsl:template>

  <xsl:template match="paragraphs">
    <xsl:param name="showOriginalSourceSegment" />
    <xsl:param name="showOriginalTargetSegment" />
    <xsl:param name="showOriginalRevisionMarkersTargetSegment" />
    <xsl:param name="showUpdatedTargetSegment" />
    <xsl:param name="showUpdatedRevisionMarkersTargetSegment" />
    <xsl:param name="showTargetComparison" />
    <xsl:param name="showSegmentComments" />
    <xsl:param name="showSegmentLocked" />
    <xsl:param name="showSegmentStatus" />
    <xsl:param name="showSegmentMatch" />
    <xsl:param name="showSegmentTerp" />
    <xsl:param name="showSegmentPemp" />

    <xsl:for-each select="paragraph">
      <xsl:for-each select="segments">
        <xsl:apply-templates select="segment">
          <xsl:with-param name="showOriginalSourceSegment" select="$showOriginalSourceSegment"/>
          <xsl:with-param name="showOriginalTargetSegment" select="$showOriginalTargetSegment"/>
          <xsl:with-param name="showOriginalRevisionMarkersTargetSegment" select="$showOriginalRevisionMarkersTargetSegment"/>
          <xsl:with-param name="showUpdatedTargetSegment" select="$showUpdatedTargetSegment"/>
          <xsl:with-param name="showUpdatedRevisionMarkersTargetSegment" select="$showUpdatedRevisionMarkersTargetSegment"/>
          <xsl:with-param name="showTargetComparison" select="$showTargetComparison"/>
          <xsl:with-param name="showSegmentComments" select="$showSegmentComments"/>
          <xsl:with-param name="showSegmentLocked" select="$showSegmentLocked"/>
          <xsl:with-param name="showSegmentStatus" select="$showSegmentStatus"/>
          <xsl:with-param name="showSegmentMatch" select="$showSegmentMatch"/>
          <xsl:with-param name="showSegmentTerp" select="$showSegmentTerp"/>
          <xsl:with-param name="showSegmentPemp" select="$showSegmentPemp"/>
        </xsl:apply-templates>
      </xsl:for-each>
    </xsl:for-each>




  </xsl:template>

  <xsl:template match="segment">
    <xsl:param name="showOriginalSourceSegment" />
    <xsl:param name="showOriginalTargetSegment" />
    <xsl:param name="showOriginalRevisionMarkersTargetSegment" />
    <xsl:param name="showUpdatedTargetSegment" />
    <xsl:param name="showUpdatedRevisionMarkersTargetSegment" />
    <xsl:param name="showTargetComparison" />
    <xsl:param name="showSegmentComments" />
    <xsl:param name="showSegmentLocked" />
    <xsl:param name="showSegmentStatus" />
    <xsl:param name="showSegmentMatch" />
    <xsl:param name="showSegmentTerp" />
    <xsl:param name="showSegmentPemp" />

    <tr data-file-id='{@fileId}' data-project-id='{@projectId}' data-segment-id='{@segmentId}'>

      <td class="segmentId">
        <a href="#" onclick="navigateToSegment('{@segmentId}','{@fileId}','{@projectId}'); return false;">
          <xsl:value-of select="@segmentId"/>
        </a>
      </td>


      <td>
        <xsl:value-of select="@tmName"/>
      </td>
      <td>
        <xsl:apply-templates select="tmTranslationUnit/token"/>
      </td>

      <xsl:if test="$showSegmentLocked = 'True'">
        <xsl:choose>
          <xsl:when test="segmentIsLockedOriginal/text() = 'True'">
            <td style="color: red;background-color: #FFFFDF">
              <xsl:copy-of select="segmentIsLockedOriginal"/>
            </td>
          </xsl:when>
          <xsl:otherwise>
            <td>
              <xsl:copy-of select="segmentIsLockedOriginal"/>
            </td>

          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>


      <xsl:if test="$showOriginalSourceSegment = 'True'">
        <td>
          <xsl:apply-templates select="source/token"/>
        </td>

        <xsl:choose>
          <xsl:when test="statistics/source">
            <td>
              <xsl:value-of select="statistics/source/@words"/>
            </td>
          </xsl:when>
          <xsl:otherwise>
            <td>
              n/a
            </td>
          </xsl:otherwise>
        </xsl:choose>

      </xsl:if>
      <xsl:if test="$showSegmentStatus = 'True'">
        <td class="status-cell">

          <div class="custom-dropdown">
            <div class="dropdown-content">
              <div onclick="updateStatus(this)" data-value="Unspecified">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAMAAAD04JH5AAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAABOUExURf///01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS/////Ly8q2srF1cW+vr601MS4iIh01MS01MS////1+nF4kAAAAYdFJOUwAA/fTctXwx0V0CzS/3Y3+Ahabrh36/3dYJUqEAAAABYktHRACIBR1IAAAAB3RJTUUH6QMfDy8h52yy/QAAAP1JREFUeNrt19kOgjAURVFHUBAojvf/v1QTYyzjA4Z7NO793mYl7ctZLIiIWtmnrdabbZIuh5of8Gy3z7QAs/wgBlhRigFmiRpglRrwELxvlgCsmgwIE6qPpxGBAyCE84jABRCOw6/gA6h7/sHFExBe568dgTMgdATegI7AHdAW+ANaAgGgKVAAegTOgIYgUQAaglIBiAXFQQGIBXmmAMSCvR+gv50aYKkakKgBWzXgpgas1YCVGmCzA/qK94IEEO8FEeC9F1SAWg0IAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH4PMFcAAAD4fgAR/V13kW70v3T5JcsAAAAldEVYdGRhdGU6Y3JlYXRlADIwMjUtMDMtMzFUMTU6NDc6MzMrMDA6MDB08/puAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDI1LTAzLTMxVDE1OjQ3OjMzKzAwOjAwBa5C0gAAACh0RVh0ZGF0ZTp0aW1lc3RhbXAAMjAyNS0wMy0zMVQxNTo0NzozMyswMDowMFK7Yw0AAAAASUVORK5CYII=" alt=""/> Not Translated
              </div>
              <div onclick="updateStatus(this)" data-value="Draft">
                <img src="data:image/png;base64,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" alt=""/> Draft
              </div>
              <div onclick="updateStatus(this)" data-value="Translated">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAMAAABg3Am1AAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAF0UExURf///wAAAMFJPMFJPMFJPMFJPMFJPMFJPMFJPMFJPMFJPMFJPM1sYcZXS92ak/////nt7MRRRMFJPMFJPOSwq8ZXS8FJPA6WYQuUXuCln8ZYTNyZktB0asFJPAyUXlGzi8FJPMFJPMJLPsFJPOCkncFJPBKXYYLIrMjn2zanelOzjQ+SXAyVXpjSupvUvU+yicFJPFy4kwyUXQuUXRCZXRKWYJfQuQyXX124lAyUXQyVXhCUWpbQuV24kw2UXQyVXgyVXV65lQuUXovMswmWXACqVAuVXRCWYPn8+guVXBycac7q4Fa1jwyVXhubZ+Lz6y2jcwmWXA2UXVa1j+n28QyUXr3i0w2UXSqicg6TW3PCoiyicwyUXuTz7R2caACRbQ2UXlm2kcHj1lm2kBycaAuTX+f073bCpQ6UXw2VXgyUXQuUXgyVXg2UXg2UXgiYWw2VXQyVXQuUXg2UXg2UXAyVXgyVXgqTXACMU8FJPA2VXv///xSz42MAAAB5dFJOUwAAILX121wi4f1u99fusYCG9U76o+1ZIXuq7LHP1eDG7Kv9Gqoc+quQ2sYg36KgyR3Ad9wd+6MfwN7dHqO/dNh/v36oQQXb/YFP747Dr/KI4RX5xIZwlNDjMbPikYfvBurDksLxUoax/vqzU7L41Bi+7u2WNvK3VwhzExL0AAAAAWJLR0QAiAUdSAAAAAd0SU1FB+kDHw8uFkbKJrMAAAHISURBVEjHxdXlXwIxGMBxTsXWKdjdBXagIDgRO0Ds7u6W/fXudsXuth28cu/28PvyuYJzOP57ZWRmObPT6HNyE4lEXn56fRpC7gsKi1IW5PuLQUlpqsLlxqAMgHL5qCqMucRelVWgGoua2jpyHvXKlA8qG1AjEepqsgG4R5RoFgPSE9Gi9M5WIVB7WbQR4WyXREDuOzq7urHoIULv2cDjxX0vAH1IEW6jZwKtB/1IEa4BSQT0flA9j6HkT63A0g+PCIHcj45xewvw2fRmoPfjnN4EfBOm3m/uaWDtA5IIkH4S90F+nwzs+tBUiAJ2PcQjmAQCWj/N7xEK6yDgx9sZux6hiApm5/BmfsHoJ3zMXgeLaAnvgssrNr1+SKtr0RgyFr9XwXp8A2zGUulVsIW2gSFEvQp2dvH10QTulXvE6hWwh/YBEQd47vVp94jVK+AwHiUAHB0jr0cKkwAyewJOTtGZnJ9fXCK/R4qoCWT1BFzhwdn1Tez27v7hRNIBjqw9AY/y6Ak9h16UWVjLXq09AW949P7xaQx1Ye0JgF/fP7/UpacFlEzA+rOiBNVz/+4hp+e/UCC7F7yyILMXAOoBSQlQjyAF0lt/y0gtHCRsMqAAAAAldEVYdGRhdGU6Y3JlYXRlADIwMjUtMDMtMzFUMTU6NDY6MjIrMDA6MDDx7Jp6AAAAJXRFWHRkYXRlOm1vZGlmeQAyMDI1LTAzLTMxVDE1OjQ2OjIyKzAwOjAwgLEixgAAACh0RVh0ZGF0ZTp0aW1lc3RhbXAAMjAyNS0wMy0zMVQxNTo0NjoyMiswMDowMNekAxkAAAAASUVORK5CYII=" alt=""/> Translated
              </div>
              <div onclick="updateStatus(this)" data-value="RejectedTranslation">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAMAAABg3Am1AAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAGVUExURf///01MS01MS01MS01MS01MS01MS01MS01MS////01MS01MS01MS01MS01MS01MS01MS1xbW01MS01MS01MS01MS1BPTlVUU2tqaXx7eoGBgHp6eWppaVRTUk1MS01MS01MS3Z1dU9OTb29vf7+/v////z8/Li4uHJxcE5NTE1MS01MS3V1dGRjYs3NzcbGxmFgX46Njfn5+ff393V0c////01MS09OTXx7ek1MS2VlZPv7+01MS9LR0f///01MS3p5eE1MS01MS1hXVk1MS1VUU01MS3Fwb01MS4OCgXp6eU1MS4qJiE1MS4ODgk1MS1pZWE1MS01MS8vLy1NSUU1MS359fE1MS1BQT9nY2E1MS2ppaH18fGhnZk5NTE1MS01MS01MS01MS////01MS+7t7fLY1cpjWMZZTcFJPMFJPICAgE1MS01MS8VWSvHW08NPQsFJPICAgE1MS05NTP///////+3Lx8FKPoCAgE1MS////8dZTv///////9mQiICAgICAgICAgE1MS8FJPICAgP///+Ferr0AAACDdFJOUwAAJE5nb2ZNIRcyl+Xiki0qyP2vI3L89NvLxsvc9vVoA9D9nYCAgZ/T/pMB0OKUmOa6goPRFS78yr3hgTqSAqDMke3xL+pb1XXEzH6/dsRd7gTwleylyUH7j8Xcyd/8tzX7GEp/ho/g0sAfQTD874vl3n5i91x/bOB/JxnTcUkzdNfYBp6aCAAAAAFiS0dEAIgFHUgAAAAHdElNRQfpAx8PMB0FWcDkAAACFUlEQVRIx7XU6VfTQBTG4YRVCioKssvagRaFsom7iKCIuG/IolWUQlNEGzfqWnyT/5tJk7Rj5o7NOR7vp0n7e0ppbqtp/zq6MBWVVdU1tYfCgbpIfQPcOXzkaHnQeAzCHG9qLgNOtDhda1t7R2fXSefY3fM30NvHk/6+gShzZnAoFufXw6fU4DR/fmSUlSYx5gglGOfPdk4wcSan+GM9Z2gwfRY4xwIT5aK7mQbngQsXg4BN8nfVRIJLQMtlJk8ijjh1P67MAFcZNTFglgC914AECYaAOQLMA9fJng3yO0js1Q1ggQasC6iVwU1gUQFuATUyWAJuK0AHUC2DO8BdBWgHqmRwD4gpQBtQKYMIcJ/uo61AhQwePAQekWAAeEytxhPgKQn40tdTYBl4tkL0o/1AhNzWVWAtKvUT689fJOn1fvlKce+0DRroSf7tWpT/xmtNo8Gbwi/L2iZjqa2tlJ+nttMGkWcy+s7bAuCfVGrXsnY98e69ZWVlYdq2qX/gIvnxU6H3xOcve85ZEjmbT46LJf2r9u27Zbnix89fWfecJnpH7Og6v0y7kbVnGF4fAKbtjeleF7Ns8WCQvW1nAqJc74OAUPdm8VFRhOlFEa5XgRD9H0Lsc6peEPlwfUnsl+uDNy5P98XVyEqrYZK9D5yXNQLLZ1K9V7lvQzz7exrs3cpvxHPh//id1+Qx0qWvpXj+v3MAqBuNJulpN/cAAAAldEVYdGRhdGU6Y3JlYXRlADIwMjUtMDMtMzFUMTU6NDg6MjgrMDA6MDBLVfWHAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDI1LTAzLTMxVDE1OjQ4OjI4KzAwOjAwOghNOwAAACh0RVh0ZGF0ZTp0aW1lc3RhbXAAMjAyNS0wMy0zMVQxNTo0ODoyOSswMDowMMtqZ1AAAAAASUVORK5CYII=" alt=""/> Translation Rejected
              </div>
              <div onclick="updateStatus(this)" data-value="ApprovedTranslation">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAMAAABg3Am1AAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAGeUExURf///wAAAE1MS+3CX01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS1VUU2tqaXx7eoGBgHp6eWppaVRTUk1MS01MS01MS01MS09OTXZ1db29vf7+/v////z8/Li4uHJxcE5NTE1MS01MS01MS2RjYs3NzcbGxmFgX01MS/n5+ff393V0c01MS01MS3x7ek1MS2VlZPv7+01MS1BPTtLR0Xp5eE1MS01MS01MS1lYV01MS3Fwb01MS4OCgXp6eU1MS4qJiIODgk1MS1pZWE1MS01MS8vLy01MS359fHV1dFlYV01MS1BQT9nY2PHx8dTT052cm1xbWk1MS2ppaP///////01MS01MS2i8miGebEmuhV24kwyVXU1MSwuUXu3CX01MS01MSwmWXO3CX01MS3Rzc+3CX2loZ1q3kWdmZf///////zioeu3CX01MS+3CXw2UXQyVXu3CX+3CX+3CX+3CX+3CXw2VXe3CX+3CX+3CX01MSw2VXu3CX////2OyovIAAACGdFJOUwAAAAAkTmdvZk0hMpfl4pItKrf9ryNy+PTby8bL3Pb1aAOg/dCdgICBn9P+kwGh4pSY5naCg9Eu+cq94YE6/JLMke0v8FvVdcTMfr/EXe4E8JWlydCiQfuPh5Gw18XceCY1+7rsmr9/f35BMPxBfmLSf9272mdSm2JcunTYu/gmvHy+dNfYxgTBoAAAAAFiS0dEAIgFHUgAAAAHdElNRQfpAx8PMSBEKr20AAACY0lEQVRIx5WUV1vUQBSGSUJfUaQoVUBlBxbbith7wZUuKCAKCpYIyOraFfvql/xsZ5JMMsmcXPBd7c5532cmc5JTUbHzGFRMkcqq6praunrTj7eeJvB6ZlcD/Oze0xgqtMBre5ugpLmlNTBIgVf27RdcW3tHZ1f3AfGzp9c3KME0+w5y5NDh/iwTGRjMDfH/R44KgxD46jFeP36CRcmfFAYt8MVhXu06xdSMnOZrvZTA186cBc6xRLLc6GnVBbHreeDCxaTARvipWkjhEnD5CtOTH0JzIyXwG7rKqOSAa4TQdx3Ik8Ig7zkh3ABukjwb4B2s14VbwCgtsG6gThduA4UU4Q5QqwtjwHiK0AnU6AJv80SK0AFU68IkkEsR2oEqXcgAUzSfbQMqdWF6BrhLCv1AA9XpWWCOFO4B96m3dR5YWCT4B0sPH2XI72EZWMlSWzx+Qn7T5uoa2bunz57HpoblxdtCDIxCco8XjuPYimBZ0jBfepNlZVHjHWc9FCS/sWFsvvJn0cKcvN0lyTvOViBIvui6ReM1N5rWhDOVmxgvjL6JeClIvuTylLgxZq4uy6n3lrF3kg+OFOOFsSlmwfzsjODff/j4KeJjQtEN8tmf3eZ0ZnL4i2nYCq8KIf/VikaOmeAVQeW3v22HfYzzkRDy3y2PsWk+FEL+x09j3QNskvcElXd/GVsBYlN8IES8InBI531B4d3fRnAknj867wllhS+JKw0NnfeEvwk+adiGkbZDwMeNGB9/hpBXrifBx26ppBZsmpd9KCV5aSR5KVjlf+Vkyab4ULAsrRR7BWPCzvIf00WJav2SQ7MAAAAldEVYdGRhdGU6Y3JlYXRlADIwMjUtMDMtMzFUMTU6NDk6MzIrMDA6MDDMTcFpAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDI1LTAzLTMxVDE1OjQ5OjMyKzAwOjAwvRB51QAAACh0RVh0ZGF0ZTp0aW1lc3RhbXAAMjAyNS0wMy0zMVQxNTo0OTozMiswMDowMOoFWAoAAAAASUVORK5CYII=" alt=""/> Translation Approved
              </div>
              <div onclick="updateStatus(this)" data-value="RejectedSignOff">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAMAAABg3Am1AAAAIGNIUk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAC3UExURf///01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS01MS8FJPMFJPE1MS01MS8FJPE1MS01MS01MS01MS01MS01MS3JxcP///+m9ucJNQU1MS01MS2hnZv///8pjWE1MS01MS09OTfLY1cVWSk1MS359fE1MS5SUk01MS4iIh8DAv4CAgLOzs01MS8FJPICAgP///+wgpGoAAAA5dFJOUwAVV3kWn/kt5TcMihP0o1YhBAlEFwLCH8ANqN5Jfn+AnuyeQFbjNf3fgOCXBv2P707Jfbc/v5t/tw62VmwAAAABYktHRACIBR1IAAAAB3RJTUUH6QMfDzIYRwVW6QAAAURJREFUSMfdk9lWwjAQhoeWBpAdhBaqQMUtLriLie//XkaHtJQm6fQcvfG/yun5vmYmmQD8s9Q83/dqZLweMKHCgjqNbzTFLs0G6f8prwzKHoHYS0Dol+0LrLxzT+TilQp+XvB/X6hcUuWmKx9r9YuDVloUax1RBGhroU3CATpd5LsdogA9FHpUHvoo9MnCAIUBWRiiMCQLIxRGZGGMwviYyE+mKEwn+ksYRaFpjZnN9cXNZzsmljIOi2s4OV0slqskm6VktTxbr88vpNTUN58al1eimGvOb6TUFPJSRj/CrXAKytA8CneJSbjnfPOgjVhmmwE8CmOeOOfayPHwbBbEy6GhD+n1zZJ3ZcRFHrgrabsZD9tPWz4qCorfmEraOnhj0/aEsfFYy/mDi7MmsoxGmVAcPndJhvF2GsYHZDecT/Sv8gWL5aSqR4SZAgAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAyNS0wMy0zMVQxNTo1MDoyMyswMDowMJsLPkkAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjUtMDMtMzFUMTU6NTA6MjMrMDA6MDDqVob1AAAAKHRFWHRkYXRlOnRpbWVzdGFtcAAyMDI1LTAzLTMxVDE1OjUwOjI0KzAwOjAweOSZpAAAAABJRU5ErkJggg==" alt=""/> Sign-off Rejected
              </div>
              <div onclick="updateStatus(this)" data-value="ApprovedSignOff">
                <img src="data:image/png;base64,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" alt=""/> Signed Off
              </div>
            </div>
          </div>

          <br/>
          <xsl:apply-templates select="segmentStatus/token"/>

        </td>
      </xsl:if>
      <xsl:if test="$showSegmentMatch = 'True'">
        <td>
          <xsl:choose>
            <xsl:when test="translationMatchType/@backgroundColor">
              <xsl:attribute name="style">
                text-align: center;background-color:<xsl:value-of select="translationMatchType/@backgroundColor"/>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="style">
                text-align: center
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:apply-templates select="translationMatchType/token"/>
        </td>
      </xsl:if>

      <xsl:if test="$showOriginalTargetSegment = 'True'">
        <td>
          <xsl:apply-templates select="targetOriginal/token"/>
        </td>
      </xsl:if>


      <xsl:if test="$showOriginalRevisionMarkersTargetSegment = 'True'">
        <td>
          <xsl:for-each select="targetOriginalRevisionMarkers">
            <xsl:apply-templates select="revisionMarker"/>
          </xsl:for-each>
        </td>
      </xsl:if>


      <xsl:if test="$showUpdatedTargetSegment = 'True'">
        <td>
          <xsl:apply-templates select="targetUpdated/token"/>
        </td>
      </xsl:if>

      <xsl:if test="$showUpdatedRevisionMarkersTargetSegment = 'True'">
        <td>
          <xsl:for-each select="targetUpdatedRevisionMarkers">
            <xsl:apply-templates select="revisionMarker"/>
          </xsl:for-each>
        </td>
      </xsl:if>


      <xsl:if test="$showTargetComparison = 'True'">
        <td>
          <xsl:apply-templates select="targetComparison/token"/>
        </td>
      </xsl:if>



      <xsl:if test="$showSegmentPemp = 'True'">
        <td>

          <xsl:choose>
            <xsl:when test="statistics/target/pemp">
              <xsl:choose>
                <xsl:when test="statistics/target/pemp/@type = 'normal'">
                  <span class="text">
                    <xsl:value-of select="statistics/target/pemp/@pemp"/>%
                  </span>
                </xsl:when>
                <xsl:when test="statistics/target/pemp/@type = 'updated'">
                  <span class="textRemoved">
                    <xsl:value-of select="statistics/target/pemp/@pemp"/>%
                  </span>
                  <br/>
                  <span class="textNew">
                    100%
                  </span>
                </xsl:when>
                <xsl:otherwise>
                  <span class="text">
                    100%
                  </span>
                </xsl:otherwise>
              </xsl:choose>

              <br/>
              <span class="grayNoWrap">
                Edit-Dist.:&#160;<xsl:value-of select="statistics/target/pemp/@editDist"/>
              </span>
              <br/>
              <span class="grayNoWrap">
                Max chars:&#160;<xsl:value-of select="statistics/target/pemp/@maxChars"/>
              </span>
            </xsl:when>
            <xsl:otherwise>
              &#160;
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </xsl:if>

      <xsl:if test="$showSegmentTerp = 'True'">
        <td>
          <xsl:choose>
            <xsl:when test="statistics/target/terp">
              <xsl:choose>
                <xsl:when test="statistics/target/terp/@terp > 100">
                  <span class="textRemoved">
                    <xsl:value-of select="statistics/target/terp/@terp"/>%
                  </span>
                  <span class="text">
                    100%
                  </span>
                </xsl:when>
                <xsl:otherwise>
                  <span class="text">
                    <xsl:value-of select="statistics/target/terp/@terp"/>%
                  </span>
                </xsl:otherwise>
              </xsl:choose>
              <br/>
              <span class="grayNoWrap">
                Nr. Errors:&#160;<xsl:value-of select="statistics/target/terp/@numEr"/>
              </span>
              <br/>
              <span class="grayNoWrap">
                Ref. words:&#160;<xsl:value-of select="statistics/target/terp/@numWd"/>
              </span>
            </xsl:when>
            <xsl:otherwise>
              &#160;
            </xsl:otherwise>
          </xsl:choose>
        </td>
      </xsl:if>


      <xsl:if test="$showSegmentComments = 'True'">
        <td>
          <div class="addComment">
            <input type="text" name="commentInput" placeholder="Add comment"
                   onkeydown="if (event.key === 'Enter') submitComment(this, this.closest('.addComment').querySelector('.severity-dropdown').value, '{@segmentId}', '{@fileId}', '{@projectId}')"/>

            <select class="severity-dropdown" id="severityDropdown">
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>

          </div>
          <div class="comments">
            <xsl:for-each select="comments">
              <xsl:apply-templates select="comment" />
            </xsl:for-each>
          </div>
        </td>
      </xsl:if>
    </tr>
  </xsl:template>

  <xsl:template match="comment">
    <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
      <div style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;">
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
          <xsl:value-of select="@date"/>
        </span>
        <br/>
        <span style="padding: 3px;">
          <xsl:value-of select="@author"/>
        </span>

      </div>

      <p style="margin: 0px; padding: 3;">
        <xsl:value-of select="."/>
      </p>



    </div>
  </xsl:template>

  <xsl:template match="token">
    <xsl:choose>
      <xsl:when test="@type = 'text'">
        <span class="text">
          <xsl:value-of select="."/>
        </span>
      </xsl:when>
      <xsl:when test="@type = 'tag'">
        <span class="tag">
          <xsl:if test="@tooltip">
            <xsl:attribute name="title" >
              <xsl:value-of select="@tooltip"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:value-of select="."/>
        </span>

      </xsl:when>
      <xsl:when test="@type = 'textNew'">
        <span class="textNew">
          <xsl:value-of select="."/>
        </span>
      </xsl:when>
      <xsl:when test="@type = 'textRemoved'">
        <span class="textRemoved">
          <xsl:value-of select="."/>
        </span>
      </xsl:when>
      <xsl:when test="@type = 'tagNew'">
        <span class="tagNew">
          <xsl:if test="@tooltip">
            <xsl:attribute name="title" >
              <xsl:value-of select="@tooltip"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:value-of select="."/>
        </span>
      </xsl:when>
      <xsl:when test="@type = 'tagRemoved'">
        <span class="tagRemoved">
          <xsl:if test="@tooltip">
            <xsl:attribute name="title" >
              <xsl:value-of select="@tooltip"/>
            </xsl:attribute>
          </xsl:if>
          <xsl:value-of select="."/>
        </span>
      </xsl:when>
      <xsl:when test="@type = 'br'">
        <br/>
      </xsl:when>

    </xsl:choose>

  </xsl:template>

  <xsl:template match="revisionMarker">
    <div style="border-style: solid solid dashed solid; border-width: thin; border-color: #C0C0C0 #C0C0C0 #000000 #C0C0C0; margin: 1px 0px 0px 1px; padding: 0; text-align: left;">
      <div style="white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;">
        <xsl:choose>
          <xsl:when test="@Type = 'Insert'">
            <span style="padding: 3px; color: blue; font-weight: bold;">
              Inserted
            </span>
          </xsl:when>
          <xsl:when test="@Type = 'Delete'">
            <span style="padding: 3px; color: red; font-weight: bold;">
              Deleted
            </span>
          </xsl:when>
          <xsl:otherwise>
            <span style="Type: 3px; font-weight: bold;">
              No Change
            </span>
          </xsl:otherwise>
        </xsl:choose>
        <span style="padding: 3px">
          <span style="font-style: italic;">
            <xsl:value-of select="@date"/>
          </span>
        </span>

        <br/>
        <span style="padding: 3px; color: black;">
          <xsl:value-of select="@author"/>
        </span>
      </div>
      <p style="margin: 0px; padding: 3;">
        <xsl:apply-templates select="content/token"/>
      </p>
    </div>
  </xsl:template>


</xsl:stylesheet>
