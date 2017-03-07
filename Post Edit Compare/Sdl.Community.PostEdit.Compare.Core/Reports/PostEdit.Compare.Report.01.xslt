<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/2000/svg">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">

    <html>
      <head>
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <title>PostEdit.Compare Comparison Report</title>


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
        <script type="text/javascript">
          <!--google.setOnLoadCallback(drawVisualization);-->
        </script>

      </head>
      <body>
        <xsl:apply-templates select="files"/>
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

          <td colspan="9" class="TableCellsBorderTopRightBottomLeft DefaultCellsColorsWhite">
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
    <td width="10%" colspan="9" class="TableCellsBorderTopRightBottomLeft DefaultCellsSectionHeaderColors" nowrap="nowrap">
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
    <td class="TableCellsBorderTopRightBottom DefaultCellsSubHeaderColorsRight">
      Shft
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
    <td class="TableCellsBorderRight DefaultCellsColorsWhite">
      <xsl:value-of select="$Shft"/>
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
    <td class="TableCellsBorderTopRightBottom DefaultCellsTotalsColorsRight" nowrap="nowrap" >
      <xsl:value-of select="$Shft"/>
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

    <tr>
      <td class="segmentId">
        <xsl:value-of select="@segmentId"/>
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
        <td>
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
          <xsl:for-each select="comments">
            <xsl:apply-templates select="comment"/>
          </xsl:for-each>
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
