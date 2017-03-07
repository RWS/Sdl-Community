<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns="http://www.w3.org/2000/svg">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <title>Qualitivity - Quality Assessment Report</title>


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
          background-color:#ffff66;
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

          span.fail
          {
          font-size: 24px;
          color: #B40404;
          font-weight: bold;
          }
          span.pass
          {
          font-size: 24px;
          color: #0B610B;
          font-weight: bold;
          }
          span.bolded
          {
          font-weight: bold;
          }
          span.italicGray
          {
          font-weight: normal;
          font-style: italic;
          color: #424242;
          }
        </style>

      </head>
      <body>
        <table cellpadding="0" cellspacing="0" style="margin-bottom: 4px;  border:none; padding 10px; color: #084B8A;  " border="0" width="100%">
          <tr>
            <td style="white-space: nowrap;color: #084B8A; text-align: center; font-size:20px;font-weight: bold;">
              Quality Assessment Report
            </td>
          </tr>
        </table>
        <xsl:apply-templates select="qualityMetrics"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="qualityMetrics">
    <xsl:variable name="metricGroupName" select="settings/@metricGroupName"/>
    <xsl:variable name="maxSeverityValue" select="settings/@maxSeverityValue"/>
    <xsl:variable name="maxSeverityInValue" select="settings/@maxSeverityInValue"/>
    <xsl:variable name="maxSeverityInType" select="settings/@maxSeverityInType"/>

    <xsl:for-each select="documents">

      <p>
        <span style="color: #084B8A; font-size:16px; font-weight: bold; padding: 1px 1px 1px 1px;">Summary</span>
      </p>

      <table cellpadding="0" cellspacing="0" style="margin-bottom: 4px; padding: 10px; color: #084B8A; background-color: #F2F2F2; " border="0" width="100%">
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Report type: </td>
          <td style="border:none;">
            <xsl:value-of select="@task"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Project: </td>
          <td style="border:none;">
            <xsl:value-of select="@projectName"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Client: </td>
          <td style="border:none;">
            <xsl:value-of select="@clientName"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Documents: </td>
          <td style="border:none;">
            <xsl:value-of select="@documentCount"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Created at: </td>
          <td style="border:none;">
            <xsl:value-of select="@createdAt"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Quality Metric: </td>
          <td style="border:none;">
            <xsl:value-of select="$metricGroupName"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px 10px 2px 2px;font-weight: bold">Assessment threshold: </td>
          <td style="border:none;">
            <xsl:value-of select="$maxSeverityValue"/>&#160;/&#160;<xsl:value-of select="$maxSeverityInValue"/>&#160;<xsl:value-of select="$maxSeverityInType"/>
          </td>
        </tr>
        <tr>
          <td width="5%" style="white-space: nowrap; border:none;color: #003300; padding: 2px 10px 2px 2px;font-weight: bold">Assessment result: </td>
          <td style="border:none;">

            <xsl:if test="@assessmentResultAll = 'PASS'">
              <span style="color: DarkGreen;font-weight: bold"></span>
              <xsl:value-of select="@assessmentResultAll"/>
            </xsl:if>
            <xsl:if test="@assessmentResultAll = 'FAIL'">
              <span style="color: DarkRed;font-weight: bold"></span>
              <xsl:value-of select="@assessmentResultAll"/>
            </xsl:if>
            <span style="padding: 2px 2px 2px 1px; color: #424242;font-style: italic;;font-weight: normal">
              &#160;&#160;<xsl:value-of select="@assessmentResultAllDetails"/>
            </span>
          </td>
        </tr>
      </table>
      <br/>

      <xsl:for-each select="document">
        <xsl:variable name="documentName" select="@documentName"/>
        <xsl:variable name="documentSourceLanguage" select="@documentSourceLanguage"/>
        <xsl:variable name="documentTargetlanguage" select="@documentTargetlanguage"/>
        <xsl:variable name="documentSourceLanguageFlag" select="@documentSourceLanguageFlag"/>
        <xsl:variable name="documentTargetlanguageFlag" select="@documentTargetlanguageFlag"/>
        
        <table cellpadding="0" cellspacing="0" style="color: white; text-align: left;  background-color: #006699"  border="0" width="100%">
          <tr>
            <td colspan="1" rowspan="1" nowrap="nowrap" style="border-style: solid none solid solid; padding: 2px 5px 2px 5px; ">
              Document: <xsl:value-of select="@documentName"/>
            </td>
            <td style="border-style: solid solid solid none; text-align: right; font-size: 12px; padding: 2px 2px 2px 2px;   ">
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
              <span style="padding: 2px 2px 2px 10px"> Activities:</span>
              <span style="padding: 2px 10px 2px 1px; font-weight: Bold">
                <xsl:value-of select="@documentActivities"/>
              </span>
            </td>
          </tr>
        </table>

        <xsl:variable name="documentActivityType" select="@documentActivityType"/>
        <xsl:variable name="severityTotal" select="@severityTotal"/>
        <xsl:variable name="severityStatusOpenTotal" select="@severityStatusOpenTotal"/>
        <xsl:variable name="severityStatusResolvedTotal" select="@severityStatusResolvedTotal"/>
        <xsl:variable name="severityStatusIgnoreTotal" select="@severityStatusIgnoreTotal"/>
        <xsl:variable name="entriesTotal" select="@entriesTotal"/>
        <xsl:variable name="entriesStatusOpenTotal" select="@entriesStatusOpenTotal"/>
        <xsl:variable name="entriesStatusResolvedTotal" select="@entriesStatusResolvedTotal"/>
        <xsl:variable name="entriesStatusIgnoreTotal" select="@entriesStatusIgnoreTotal"/>
        <xsl:variable name="penaltyAllowed" select="@penaltyAllowed"/>
        <xsl:variable name="penaltyApplied" select="@penaltyApplied"/>
        <xsl:variable name="assessmentResult" select="@assessmentResult"/>
        <xsl:variable name="documentTotalSegments" select="@documentTotalSegments"/>
        <xsl:variable name="documentTotalWords" select="@documentTotalWords"/>
        <xsl:variable name="documentTotalChars" select="@documentTotalChars"/>
        <xsl:variable name="documentActivities" select="@documentActivities"/>
        <xsl:variable name="started" select="@started"/>
        <xsl:variable name="stopped" select="@stopped"/>
        <xsl:variable name="documentHasTrackChanges" select="@documentHasTrackChanges"/>

        <table cellpadding="0" cellspacing="0" style="margin-bottom: 4px; color: Black; background-color: #F2F2F2; " border="0" width="100%">
          <xsl:for-each select="types">
            <tr>
              <td nowrap="nowrap"  width="60%" colspan="1" style="text-align: left; padding: 2px 5px 2px 5px; background-color: #F3F3F3; color: #003300; font-weight: normal;font-style: italic">
                Document Summary
              </td>
              <td style="text-align: left; padding: 0px 5px 0px 5px; background-color: white; border:none;" border="0">
                &#160;
              </td>
              <td nowrap="nowrap"  width="40%" colspan="7" style="text-align: left; padding: 2px 5px 2px 5px; background-color: #F3F3F3; color: #003300; font-weight: normal;font-style: italic">
                Quality Metric Assessment Results
              </td>
            </tr>
            <tr>
              <td nowrap="nowrap"  rowspan="25"  width="55%"  style="border-style: solid solid solid solid; border-width: thin; border-color: #C0C0C0;padding: 0px 5px 0px 5px;">
                <table cellpadding="0" cellspacing="0" style="margin-bottom: 4px; color: #084B8A;; background-color: #F2F2F2; " border="0" width="100%">
                  <tr>
                    <td width="15%" style="white-space: nowrap; border:none; color: #003300; padding: 2px; font-weight: bold">Document name: </td>
                    <td style="border:none; font-weight: bold">
                      <xsl:value-of select="$documentName"/>
                    </td>
                  </tr>
                  <tr>
                    <td style="white-space: nowrap; border:none; color: #003300; padding: 2px; font-weight: bold">Activity type: </td>
                    <td style="border:none; font-weight: bold">
                      <xsl:value-of select="$documentActivityType"/>
                    </td>
                  </tr>
                  <tr>
                    <td style="white-space: nowrap; border:none;color: #003300; padding: 2px 10px 2px 2px; font-weight: bold">Activity count: </td>
                    <td style="border:none; font-weight: bold">
                      <xsl:value-of select="$documentActivities"/>
                    </td>
                  </tr>
                  <tr>
                    <td style="white-space: nowrap; border:none;color: #003300; padding: 2px 10px 2px 2px;font-weight: bold">Activity date range: </td>
                    <td style="border:none;">
                      <span style="padding: 2px 5px 2px 1px; font-weight: bold">
                        <xsl:value-of select="$started"/>
                      </span>
                      <span style="padding: 2px 2px 2px 1px; color: #424242; font-weight: bold"> &#160;&gt;&#160; </span>
                      <span style="padding: 2px 5px 2px 1px; font-weight: bold">
                        <xsl:value-of select="$stopped"/>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td style="white-space: nowrap; border:none;color: #003300;padding: 2px; font-weight: bold">Document totals: </td>
                    <td style="border:none;">
                      <span style="padding: 2px 5px 2px 1px; font-weight: bold">
                        <xsl:value-of select="$documentTotalSegments"/>
                      </span>
                      <span style="padding: 2px 2px 2px 1px; color: #424242;font-style: italic;">segments</span>

                      <span style="padding: 2px 2px 2px 5px; font-weight: bold">
                        <xsl:value-of select="$documentTotalWords"/>
                      </span>
                      <span style="padding: 2px 2px 2px 1px;color: #424242;font-style: italic;">words</span>

                      <span style="padding: 2px 2px 2px 5px; font-weight: bold">
                        <xsl:value-of select="$documentTotalChars"/>
                      </span>
                      <span style="padding: 2px 2px 2px 1px;color: #424242;font-style: italic;">characters</span>
                    </td>
                  </tr>
                  <tr>
                    <td style="white-space: nowrap; border:none;color: #003300;padding: 2px; font-weight: bold">Penalty allowed: </td>
                    <td style="border:none;">
                      <span style="font-weight: bold">
                        <xsl:value-of select="$penaltyAllowed"/>
                      </span>
                      <span style="font-style: italic; color: #424242;">
                        <span style="padding: 0px 0px 0px 10px">( </span>
                        <span>
                          <xsl:value-of select="$maxSeverityValue"/>
                        </span>
                        <span style="padding: 0px 0px 0px 0px"> / </span>
                        <span>
                          <xsl:value-of select="$maxSeverityInValue"/>
                        </span>
                        <span style="padding: 0px 0px 0px 0px"> ) * </span>
                        <span>
                          <xsl:value-of select="$documentTotalWords"/>
                        </span>
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td style="white-space: nowrap; border:none;color: #003300; padding: 2px;font-weight: bold">Penalty applied: </td>
                    <td style="border:none;font-weight: bold">
                      <xsl:value-of select="$penaltyApplied"/>
                    </td>
                  </tr>
                  <tr>
                    <td  style="white-space: nowrap;border:none;color: #003300; padding: 2px 10px 2px 2px;font-weight: bold">Assessment result:</td>
                    <td style="border:none; font-weight: bold">
                      <xsl:if test="$assessmentResult = 'PASS'">
                        <span style="color: DarkGreen"></span>
                        <xsl:value-of select="$assessmentResult"/>
                      </xsl:if>
                      <xsl:if test="$assessmentResult = 'FAIL'">
                        <span style="color: DarkRed"></span>
                        <xsl:value-of select="$assessmentResult"/>
                      </xsl:if>
                    </td>
                  </tr>
                </table>
              </td>
              <td style="text-align: left; padding: 0px 5px 0px 5px; background-color: white; border:none;" border="0">
                &#160;
              </td>
              <td width="20%" style="border-style: solid none solid solid;border-width: thin; border-color: #C0C0C0; text-align: left; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold">
                Type
              </td>
              <td nowrap="nowrap"  colspan="1" style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: center; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold">
                Severity
              </td>
              <td nowrap="nowrap"  colspan="1" style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: center; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold">
                Weight
              </td>
              <td nowrap="nowrap" colspan="1" style="border-style:  solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: center; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold">
                Open
              </td>
              <td nowrap="nowrap" colspan="1" style="border-style:  solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: center; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #0B610B; font-weight: bold">
                Resolved
              </td>
              <td nowrap="nowrap" colspan="1" style="border-style:  solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: center; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #B40404; font-weight: bold">
                Ignore
              </td>
              <td nowrap="nowrap" colspan="1" style="border-style:  solid solid solid solid; border-width: thin; border-color: #C0C0C0; text-align: center; padding: 2px 10px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold">
                Total Penalty
              </td>
            </tr>
            <xsl:for-each select="type">
              <tr>
                <td style="text-align: left; padding: 0px 5px 0px 5px; background-color: white; border:none;" border="0">
                  &#160;
                </td>
                <td nowrap="nowrap" style="border-style: none none none solid; border-width: thin; border-color: #C0C0C0; text-align: left; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold; " >
                  <xsl:value-of select="@name"/>
                </td>
                <td style="padding: 2px 5px 2px 5px; text-align: right;background-color: white; color: #003300; font-weight: normal; border:none;">
                  <xsl:value-of select="@severityName"/>
                </td>
                <td style="padding: 2px 5px 2px 5px; text-align: right;background-color: white; color: #003300; font-weight: normal; border:none;">
                  <xsl:value-of select="@severity"/>
                </td>
                <td style="padding: 2px 5px 2px 5px; text-align: right;background-color: white; color: #003300; font-weight: normal; border:none;">
                  <xsl:value-of select="@entriesStatusOpen"/>
                </td>
                <td style="padding: 2px 5px 2px 5px; text-align: right;background-color: white; color: #0B610B; font-weight: normal; border:none;">
                  <xsl:value-of select="@entriesStatusResolved"/>
                </td>
                <td style="padding: 2px 5px 2px 5px; text-align: right;background-color: white; color: #B40404; font-weight: normal; border:none;">
                  <xsl:value-of select="@entriesStatusIgnore"/>
                </td>
                <td style="border-style: none solid none solid; border-width: thin; border-color: #C0C0C0; padding: 2px 10px 2px 5px; text-align: right;background-color: white; color: #003300; font-weight: normal;">
                  <xsl:value-of select="@total"/>
                </td>
              </tr>
            </xsl:for-each>
            <tr>
              <td style="border-style: none none none none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 0px 5px 0px 5px; background-color: white; border:none;" border="0">
                &#160;
              </td>
              <td nowrap="nowrap" style="border-style: solid none solid solid; border-width: thin; border-color: #C0C0C0; text-align: left; padding: 2px 5px 2px 5px; background-color: White; color: #003300; font-weight: bold; " >
                -
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: White; color: #003300; font-weight: bold;">
                &#160;
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: White; color: #003300; font-weight: bold;">
                &#160;
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color:White; color: #003300; font-weight: bold;">
                &#160;
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: White; color: #003300; font-weight: bold; ">
                &#160;
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: White; color: #003300; font-weight: bold; ">
                &#160;
              </td>
              <td style="border-style: solid solid solid solid; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 10px 2px 5px; text-align: right;background-color: White; color: #003300; font-weight: bold;">
                &#160;
              </td>
            </tr>
            <tr>
              <td style="text-align: right; padding: 0px 5px 0px 5px; background-color: white; border:none;" border="0">
                &#160;
              </td>
              <td nowrap="nowrap" style="border-style: solid none solid solid; border-width: thin; border-color: #C0C0C0; text-align: left; padding: 2px 5px 2px 5px; background-color: #DFEAEA; color: #003300; font-weight: bold; " >
                Total
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: #DFEAEA; color: #003300; font-weight: bold;">
                -
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: #DFEAEA; color: #003300; font-weight: bold;">
                -
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: #DFEAEA; color: #003300; font-weight: bold;">
                <xsl:value-of select="$entriesStatusOpenTotal"/>
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: #DFEAEA; color: #0B610B; font-weight: bold; ">
                <xsl:value-of select="$entriesStatusResolvedTotal"/>
              </td>
              <td style="border-style: solid none solid none; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 5px 2px 5px; text-align: right;background-color: #DFEAEA; color: #B40404; font-weight: bold; ">
                <xsl:value-of select="$entriesStatusIgnoreTotal"/>
              </td>
              <td style="border-style: solid solid solid solid; border-width: thin; border-color: #C0C0C0; text-align: right; padding: 2px 10px 2px 5px; text-align: right;background-color: #DFEAEA; color: #003300; font-weight: bold;">
                <xsl:value-of select="$severityTotal"/>
              </td>
            </tr>
          </xsl:for-each>
        </table>

        <xsl:for-each select="segments">
          <xsl:if test="@count != 0">
            <table border="1" cellpadding="3"  cellspacing="0" width="100%"  >
              <tr>
                <th style="white-space: nowrap; ">Seg. ID</th>
                <th style="white-space: nowrap;">QM Type</th>
                <th style="white-space: nowrap;  ">Modified</th>
                <th style="white-space: nowrap">Modified By</th>
                <th style="white-space: nowrap">Severity</th>
                <th style="white-space: nowrap;">Penalty</th>
                <th style="white-space: nowrap; ">Status</th>
                <th style="white-space: nowrap; ">QM Content</th>
                <th style="white-space: nowrap;  ">QM Comment</th>
                <th style="white-space: nowrap;  ">Source Content</th>
                <th style="white-space: nowrap; ">Target (Updated)</th>
                <xsl:if test="$documentHasTrackChanges = 'true'">
                  <th style="white-space: nowrap; ">Track Changes</th>
                </xsl:if>
                <th style="white-space: nowrap; ">Target Comparison</th>
              </tr>

              <xsl:for-each select="segment">

                <tr >
                  <td class="segmentId">
                    <xsl:value-of select="@segmentId"/>
                  </td>
                  <td style="white-space: nowrap">
                    <xsl:value-of select="@qm_type"/>
                  </td>
                  <td>
                    <xsl:value-of select="@qm_modified"/>
                  </td>
                  <td>
                    <xsl:value-of select="@qm_user_name"/>
                  </td>
                  <td>
                    <xsl:value-of select="@qm_severity_name"/>
                  </td>
                  <td>
                    <xsl:value-of select="@qm_severity_value"/>
                  </td>
                  <td>
                    <xsl:choose>
                      <xsl:when test="@qm_status = 'Open'">
                        <span style="color: DarkBlue;">
                          <xsl:value-of select="@qm_status"/>
                        </span>
                      </xsl:when>
                      <xsl:when test="@qm_status = 'Resolved'">
                        <span style="color: DarkGreen;">
                          <xsl:value-of select="@qm_status"/>
                        </span>
                      </xsl:when>
                      <xsl:when test="@qm_status = 'Ignore'">
                        <span style="color: DarkRed;">
                          <xsl:value-of select="@qm_status"/>
                        </span>
                      </xsl:when>
                    </xsl:choose>
                  </td>
                  <td>
                    <xsl:copy-of select="qm_content"/>
                  </td>
                  <td>
                    <xsl:copy-of select="qm_comment"/>
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
                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>
        </xsl:for-each>
        <br/>
        <br/>
      </xsl:for-each>
    </xsl:for-each>
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



</xsl:stylesheet>
