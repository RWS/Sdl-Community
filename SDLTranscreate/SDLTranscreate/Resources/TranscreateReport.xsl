<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:user="http://mycompany.com/mynamespace" xmlns:msxsl="urn:schemas-microsoft-com:xslt">

		<msxsl:script language="Javascript" implements-prefix="user">
				<![CDATA[

		function formatPercentage(number, total)
		{
			if (total == 0)
			{
				return "0.00%";
			}
			
			return (100 * number / total).toFixed(2) + "%";
		}
								    		
		]]>
		</msxsl:script>

		<xsl:template match="/">
				<html>
						<head>
								<style type="text/css">
										body {
										color: #636463;
										font-family: Segoe UI;
										font-size: 13px;
										line-height: 20px;
										}

										.InfoTable {
										border: none;
										border-collapse: collapse;
										font-family: Segoe UI;
										font-size: 10pt;
										}

										.InfoHeader {
										border-bottom: #e5ecf0 1px solid;
										border-right: #e5ecf0 1px solid;
										color: #4086AA;
										background-color: #f8fafa;
										font-family: Segoe UI;
										font-size: 10pt;
										}

										.InfoItem {
										font-weight: bold;
										color: #0A1E2C;
										font-family: Segoe UI;
										font-size: 13px;
										padding-right: 40px;
										white-space: nowrap !important;
										line-height: 20px;
										width: 20%;
										overflow: hidden;
										}

										.InfoItemWithIndent {
										font-weight: bold;
										color: #0A1E2C;
										font-family: Segoe UI;
										font-size: 13px;
										padding-left: 30px;
										padding-right: 40px;
										white-space: nowrap;
										line-height: 20px;
										}

										.InfoData {
										color: #636463;
										font-family: Segoe UI;
										font-size: 13px;
										line-height: 20px;
										}

										.InfoDataMinColWidth {
										width: 10%;
										}

										.ConversionReportTable {
										border: #999999 1px solid;
										border-collapse: collapse;
										color: #4086AA;
										font-family: Segoe UI;
										font-size: 10pt;
										}

										.ConversionReportTable td {
										color: #4086AA;
										text-align: left;
										}

										.ReportTable {
										border: #BFCDD4 1px solid;
										border-collapse: collapse;
										color: #636463;
										font-family: Segoe UI;
										font-size: 13px;
										line-height: 20px;
										}

										.ReportTable th {
										text-align: left;
										background-color: #F8FAFA;
										font-size: 13px;
										line-height: 20px;
										padding: 5px;
										color: #0A1E2C;
										}

										.ReportTable th.Unit {
										text-align: right;
										}

										.ReportTable th.UnitWrap {
										white-space: normal;
										}

										.ReportTable tr {
										text-align: left;
										background-color: #ffffff;
										border-bottom: #E5ECF0 1px solid;
										}

										.ReportTable tr:nth-child(even) {
										background: #F8FAFA;
										}

										.ReportTable tr:nth-child(odd) {
										background: #fff;
										}

										.ReportTable td {
										text-align: right;
										padding: 5px;
										}

										.ReportTable td.File {
										color: #0A1E2C;
										text-align: left;
										}

										.ReportTable tr {
										vertical-align: top;
										}

										.ReportTable td.MergedFile {
										border-top: #e5ecf0 1px solid;
										border-right: #e5ecf0 1px solid;
										color: #0A1E2C;
										text-align: left;
										font-weight: normal;
										vertical-align: top;
										white-space: normal;
										word-break: break-all;
										padding-left: 10%;
										background-color: White;
										}

										.ReportTable td.Text {
										color: #4086AA;
										text-align: left;
										font-size: 8pt;
										}

										.ReportTable td.TextIndented {
										color: #4086AA;
										text-align: left;
										padding-left: 20px;
										font-size: 8pt;
										}

										.TypeHead {
										white-space: nowrap;
										text-align: left;
										background-color: #f8fafa;
										border-bottom: #E5ECF0 1px solid;
										}

										.File {
										border-top: #E5ECF0 1px solid;
										border-right: #E5ECF0 1px solid;
										color: #0A1E2C;
										text-align: left;
										background-color: #f8fafa;
										font-weight: normal;
										word-break: break-all;
										}

										.Number {
										text-align: right;
										}

										.CharPerWord {
										border-bottom: #e5ecf0 1px solid;
										border-right: #e5ecf0 1px solid;
										color: #0A1E2C;
										font: normal italic 10pt;
										white-space: nowrap;
										text-align: left;
										background-color: #f8fafa;
										}

										.Type {
										border-left: #e5ecf0 1px solid;
										}

										.Unit {
										border-bottom: #e5ecf0 1px solid;
										padding-left: 4px;
										color: #4086AA;
										white-space: nowrap;
										text-align: center;
										background-color: #f8fafa;
										border-bottom: #e5ecf0 1px solid;
										}

										.UnitWrap {
										}

										.Header {
										white-space: nowrap;
										text-align: left;
										background-color: #f8fafa;
										}

										.HeaderWithIndent {
										white-space: nowrap;
										text-align: left;
										background-color: #f8fafa;
										padding-left: 0.5cm;
										}

										.HeaderWithLineAbove {
										white-space: nowrap;
										text-align: left;
										background-color: #f8fafa;
										border-top: #e5ecf0 1px solid;
										}

										.WithLineAbove {
										white-space: nowrap;
										border-top: #e5ecf0 1px solid;
										}




										.Total {
										font-weight: bold;
										color: #0A1E2C !important;
										white-space: nowrap;
										text-align: left;
										background-color: #F0F4F6 !important;
										border-top: #bfcdd4 1px solid !important;
										border-bottom: #bfcdd4 1px solid !important;
										}

										.Error {
										color: red;
										}

										.MergedFile {
										border-top: #e5ecf0 1px solid;
										border-right: #e5ecf0 1px solid;
										color: #0A1E2C;
										text-align: left;
										font-weight: normal;
										vertical-align: top;
										white-space: normal;
										word-break: break-all;
										padding-left: 10%;
										background-color: White;
										}

										.MergedCharPerWord {
										border-bottom: #e5ecf0 1px solid;
										border-right: #e5ecf0 1px solid;
										color: #0A1E2C;
										font: normal italic 10pt;
										white-space: nowrap;
										text-align: left;
										padding-left: 10%;
										background-color: White;
										}

										.MergedHeader {
										white-space: nowrap;
										text-align: left;
										background-color: White;
										}

										.MergedHeaderWithIndent {
										white-space: nowrap;
										text-align: left;
										background-color: White;
										padding-left: 0.5cm;
										}

										.MergedHeaderWithLineAbove {
										white-space: nowrap;
										text-align: left;
										background-color: White;
										border-top: #e5ecf0 1px solid;
										}

										.MergedTotal {
										border-top: #e5ecf0 1px solid;
										border-bottom: #e5ecf0 1px solid;
										font-weight: bold;
										white-space: nowrap;
										text-align: left;
										background-color: White;
										}



										/*
										Verification Reports
										*/
										.TitleTable {
										border: #999999 1px solid;
										border-collapse: collapse;
										font-size: 14pt;
										color: #333399;
										background-color: #e2e2e2;
										}


										.TitleTable th {
										text-align: left;
										white-space: nowrap;
										}

										.VerificationReportTable {
										border: #999999 1px solid;
										border-collapse: collapse;
										color: #0000cc;
										font-family: Segoe UI;
										font-size: 10pt;
										}

										.VerificationReportTable th {
										white-space: nowrap;
										text-align: left;
										background-color: #f8fafa;
										border-bottom: #e5ecf0 1px solid;
										border-top: #e5ecf0 1px solid;
										color: #4086AA;
										}

										.VerificationReportTable td {
										border-bottom: #e5ecf0 1px solid;
										border-top: #e5ecf0 1px solid;
										color: #0A1E2C;
										}

										.VerificationReportTable a {
										color: #4086AA;
										}

										.VerificationStatisticsTable {
										border: #999999 1px solid;
										border-collapse: collapse;
										font-family: Segoe UI;
										font-size: 10pt;
										}

										.VerificationStatisticsTable td {
										border-bottom: #e5ecf0 1px solid;
										border-top: #e5ecf0 1px solid;
										border-right: #e5ecf0 1px solid;
										margin-left: 2px;
										}


										h1 {
										font-family: Segoe UI;
										font-weight: normal;
										font-size: 22px;
										color: #636463;
										padding: 0;
										margin: 0px;
										}

										h1.first {
										font-family: Segoe UI;
										font-weight: normal;
										font-size: 22px;
										color: #636463;
										padding: 20px 20px 20px 0px;
										margin: 0px 20pc 2px 0;
										}

										h2.first {
										font-family: Segoe UI;
										line-height: 22px;
										font-size: 16px;
										color: #0A1E2C;
										padding: 20px 0 2px 0;
										margin: 0px 0 2px 0;
										}

										h2.firstWithEmphasis {
										font-family: Segoe UI;
										line-height: 22px;
										font-size: 16px;
										color: #0A1E2C;
										padding: 30px 0 10px 0;
										margin: 0px 0 2px 0;
										}

										h2 {
										font-family: Segoe UI;
										line-height: 22px;
										font-size: 16px;
										color: #0A1E2C;
										padding: 20px 0 2px 0;
										margin: 0px 0 2px 0;
										}

										h3 {
										font-family: Segoe UI;
										font-size: 10px;
										color: #0A1E2C;
										padding: 2px 0 2px 0;
										margin: 0px 0 2px 0;
										}

										.InfoList {
										font-family: Segoe UI;
										font-size: 10pt;
										padding-left: 0px;
										margin-left: 0px;
										padding-top: 0px;
										margin-top: 0px;
										table-layout: auto;
										border-collapse: collapse;
										width: 100%;
										}

										.InfoListItem {
										font-family: Segoe UI;
										font-size: 10pt;
										padding-left: 0px;
										margin-left: 0px;
										}


										.InfoMessage {
										font-family: Segoe UI;
										font-size: 10pt;
										padding-top: 0px;
										padding-left: 0px;
										margin-left: 0px;
										margin-top: 10px;
										margin-bottom: 10px;
										}

								</style>
						</head>
						<body>
								<table  width="100%" border="0" cellpadding="0" cellspacing="0">
										<tr>
												<td width="100%">
														<h1>
																<xsl:value-of select="//@name"/>
														</h1>
												</td>
												<td valign="top">
														<!--<img>
														  <xsl:attribute name="src">
															  <xsl:value-of select="XmlReporting:GetImagesUrl()"/>/StudioPower.jpg
														  </xsl:attribute>
													  </img>-->
												</td>
										</tr>
								</table>

								<h2 class="first">
										Summary
								</h2>

								<table class="InfoList" width="100%" border="0" cellpadding="0" cellspacing="2">
										<tr>
												<td class="InfoItem">
														Task:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/@action"/>
												</td>
										</tr>
										<tr>
												<td class="InfoItem">
														Workflow:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/@workflow"/>
												</td>
										</tr>

										<tr>
												<td class="InfoItem">
														Project:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/project/@name"/>
												</td>
										</tr>

										<xsl:if test="//customer/@name">
												<tr>
														<td class="InfoItem">
																Customer:
														</td>
														<td class="InfoData">
																<xsl:value-of select="//taskInfo/customer/@name"/>
																<xsl:if test="//customer/@email and //customer/@email != ''">
																		&#32;(<xsl:value-of select="//taskInfo/customer/@email"/>)
																</xsl:if>
														</td>
												</tr>
										</xsl:if>

										<xsl:if test="//project/@dueDate">
												<tr>
														<td class="InfoItem">
																Due Date:
														</td>
														<td class="InfoData">
																<xsl:value-of select="//taskInfo/project/@dueDate"/>
														</td>
												</tr>
										</xsl:if>

										<xsl:for-each select="//taskInfo/tm">
												<xsl:choose>
														<xsl:when test="position()=1">
																<tr>
																		<td class="InfoItem">
																				Translation Providers:
																		</td>
																		<td class="InfoData">
																				<xsl:value-of select="@name"/>
																		</td>
																</tr>
														</xsl:when>
														<xsl:otherwise>
																<tr>
																		<td class="InfoItem" />
																		<td class="InfoData">
																				<xsl:value-of select="@name"/>
																		</td>
																</tr>
														</xsl:otherwise>
												</xsl:choose>
										</xsl:for-each>

										<tr>
												<td class="InfoItem">
														Language:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/language/@name"/>
												</td>
										</tr>
										<tr>
												<td class="InfoItem">
														Files:
												</td>
												<td class="InfoData">
														<xsl:value-of select="count(task/file)"/>
												</td>
										</tr>

										<tr>
												<td class="InfoItem">
														Created At:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/@runAt"/>
												</td>
										</tr>
								</table>

								<h2 class="first">
										Settings
								</h2>

								<table class="InfoList" width="100%" border="0" cellpadding="0" cellspacing="2">
										<xsl:choose>
												<xsl:when  test="//@action = 'Export' or //@action = 'ExportBackTranslation'">
														<tr>
																<td class="InfoItem">
																		Xliff Support:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@xliffSupport"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItem">
																		Include Translations:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@includeTranslations"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItem">
																		Copy Source to Target:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@copySourceToTarget"/>
																</td>
														</tr>
												</xsl:when>
												<xsl:when  test="//@action = 'Import' or //@action = 'ImportBackTranslation'">
														<tr>
																<td class="InfoItem">
																		Overwrite existing translations:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@overwriteTranslations"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItem">
																		Origin system:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@originSystem"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItem">
																		Segment Status Assignment
																</td>
																<td class="InfoData">
																		<xsl:text>&#160;</xsl:text>
																</td>
														</tr>

														<tr>
																<td class="InfoItemWithIndent">
																		Translations updated:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@statusTranslationUpdatedId"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItemWithIndent">
																		Translations not updated:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@statusTranslationNotUpdatedId"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItemWithIndent">
																		Segments not imported:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@statusSegmentNotImportedId"/>
																</td>
														</tr>
												</xsl:when>
												<xsl:when  test="//@action = 'CreateBackTranslation'">
												</xsl:when>
												<xsl:when  test="//@action = 'Convert'">
														<tr>
																<td class="InfoItem">
																		Max Alternative Translations:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@maxAlternativeTranslations"/>
																</td>
														</tr>

														<tr>
																<td class="InfoItem">
																		Close Project on Complete:
																</td>
																<td class="InfoData">
																		<xsl:value-of select="//taskInfo/settings/@closeProjectOnComplete"/>
																</td>
														</tr>
												</xsl:when>

										</xsl:choose>

										<tr>
												<td class="InfoItem">
														Exclude Segments Filter:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/settings/@excludeFilterItems"/>
												</td>
										</tr>

								</table>


								<h2 class="firstWithEmphasis">
										<xsl:choose>
												<xsl:when  test="//@action = 'Export' or //@action = 'ExportBackTranslation'">
														Exported
												</xsl:when>
												<xsl:when  test="//@action = 'Import' or //@action = 'ImportBackTranslation'">
														Imported (Translations updated)
												</xsl:when>
												<xsl:otherwise>
														Processed
												</xsl:otherwise>
										</xsl:choose>
								</h2>

								<table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
										<tr>
												<th class="TypeHead" width="100%">
														Total
												</th>
												<th class="TypeHead">
														Confirmation Level
												</th>
												<th class="Unit">
														Segments
												</th>
												<th class="Unit">
														Words
												</th>
												<th class="Unit">
														Characters
												</th>
												<th class="Unit">
														Percent
												</th>
												<th class="Unit">
														Recognized Tokens
												</th>
												<th class="Unit">
														Tags
												</th>
										</tr>
										<xsl:if test="//taskInfo/@action != 'Convert' and //taskInfo/@action != 'CreateBackTranslation'">
												<xsl:apply-templates select="//batchTotal">
														<xsl:with-param name="type" select="'processed'" />
												</xsl:apply-templates>
										</xsl:if>
										<xsl:if test="//taskInfo/@action = 'Convert' or //taskInfo/@action = 'CreateBackTranslation'">
												<xsl:apply-templates select="//batchTotal">
														<xsl:with-param name="type" select="'total'" />
												</xsl:apply-templates>
										</xsl:if>
								</table>

								<h3/>

								<table class="ReportTable" border="0" cellspacing="0" cellpadding="2"  width="100%">
										<tr>
												<th class="TypeHead" width="100%">
														File
												</th>
												<th class="TypeHead">
														Confirmation Level
												</th>
												<th class="Unit">
														Segments
												</th>
												<th class="Unit">
														Words
												</th>
												<th class="Unit">
														Characters
												</th>
												<th class="Unit">
														Percent
												</th>
												<th class="Unit">
														Recognized Tokens
												</th>
												<th class="Unit">
														Tags
												</th>
										</tr>
										<xsl:if test="//taskInfo/@action != 'Convert' and //taskInfo/@action != 'CreateBackTranslation'">
												<xsl:apply-templates select="//file">
														<xsl:with-param name="type" select="'processed'" />
												</xsl:apply-templates>
										</xsl:if>
										<xsl:if test="//taskInfo/@action = 'Convert' or //taskInfo/@action = 'CreateBackTranslation'">
												<xsl:apply-templates select="//file">
														<xsl:with-param name="type" select="'total'" />
												</xsl:apply-templates>
										</xsl:if>
								</table>

								<xsl:if test="//taskInfo/@action != 'Convert' and //taskInfo/@action != 'CreateBackTranslation'">
										<h2 class="firstWithEmphasis">
												<xsl:choose>
														<xsl:when  test="//@action = 'Export' or //@action = 'ExportBackTranslation'">
																Not Exported
														</xsl:when>
														<xsl:when  test="//@action = 'Import' or //@action = 'ImportBackTranslation'">
																Imported (Translations not updated)
														</xsl:when>
														<xsl:otherwise>
																Processed
														</xsl:otherwise>
												</xsl:choose>

										</h2>

										<table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
												<tr>
														<th class="TypeHead" width="100%">
																Total
														</th>
														<th class="TypeHead">
																Confirmation Level
														</th>
														<th class="Unit">
																Segments
														</th>
														<th class="Unit">
																Words
														</th>
														<th class="Unit">
																Characters
														</th>
														<th class="Unit">
																Percent
														</th>
														<th class="Unit">
																Recognized Tokens
														</th>
														<th class="Unit">
																Tags
														</th>
												</tr>
												<xsl:apply-templates select="//batchTotal">
														<xsl:with-param name="type" select="'excluded'" />
												</xsl:apply-templates>
										</table>

										<h3/>

										<table class="ReportTable" border="0" cellspacing="0" cellpadding="2"  width="100%">
												<tr>
														<th class="TypeHead" width="100%">
																File
														</th>
														<th class="TypeHead">
																Confirmation Level
														</th>
														<th class="Unit">
																Segments
														</th>
														<th class="Unit">
																Words
														</th>
														<th class="Unit">
																Characters
														</th>
														<th class="Unit">
																Percent
														</th>
														<th class="Unit">
																Recognized Tokens
														</th>
														<th class="Unit">
																Tags
														</th>
												</tr>
												<xsl:apply-templates select="//file">
														<xsl:with-param name="type" select="'excluded'" />
												</xsl:apply-templates>

										</table>

										<xsl:if test="//taskInfo/@action = 'Import' or //@action = 'ImportBackTranslation'">
												<h2 class="firstWithEmphasis">
														Not Imported
												</h2>

												<table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
														<tr>
																<th class="TypeHead" width="100%">
																		Total
																</th>
																<th class="TypeHead">
																		Confirmation Level
																</th>
																<th class="Unit">
																		Segments
																</th>
																<th class="Unit">
																		Words
																</th>
																<th class="Unit">
																		Characters
																</th>
																<th class="Unit">
																		Percent
																</th>
																<th class="Unit">
																		Recognized Tokens
																</th>
																<th class="Unit">
																		Tags
																</th>
														</tr>
														<xsl:apply-templates select="//batchTotal">
																<xsl:with-param name="type" select="'notProcessed'" />
														</xsl:apply-templates>
												</table>

												<h3/>

												<table class="ReportTable" border="0" cellspacing="0" cellpadding="2"  width="100%">
														<tr>
																<th class="TypeHead" width="100%">
																		File
																</th>
																<th class="TypeHead">
																		Confirmation Level
																</th>
																<th class="Unit">
																		Segments
																</th>
																<th class="Unit">
																		Words
																</th>
																<th class="Unit">
																		Characters
																</th>
																<th class="Unit">
																		Percent
																</th>
																<th class="Unit">
																		Recognized Tokens
																</th>
																<th class="Unit">
																		Tags
																</th>
														</tr>
														<xsl:apply-templates select="//file">
																<xsl:with-param name="type" select="'notProcessed'" />
														</xsl:apply-templates>

												</table>
										</xsl:if>
								</xsl:if>
						</body>
				</html>
		</xsl:template>

		<xsl:template match="file | batchTotal">
				<xsl:param name="type" />
				<xsl:if test="local-name() = 'file'">
						<tr>
								<th class="File" align="left" valign="top">
										<xsl:value-of select="@name"/>
								</th>
								<th class="Header">
										Not Translated
								</th>
								<xsl:if test="$type = 'processed'">
										<xsl:apply-templates select="confirmation/processed/unspecified"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:apply-templates select="confirmation/excluded/unspecified"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:apply-templates select="confirmation/notProcessed/unspecified"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:apply-templates select="confirmation/total/unspecified"/>
								</xsl:if>
						</tr>
				</xsl:if>
				<xsl:if test="local-name() = 'batchTotal'">
						<tr>
								<th class="File" align="left" valign="top">
										Files:&#32;
										<xsl:value-of select="count(parent::node()/file)"/>
								</th>
								<th class="Header">
										Not Translated
								</th>
								<xsl:if test="$type = 'processed'">
										<xsl:apply-templates select="confirmation/processed/unspecified"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:apply-templates select="confirmation/excluded/unspecified"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:apply-templates select="confirmation/notProcessed/unspecified"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:apply-templates select="confirmation/total/unspecified"/>
								</xsl:if>
						</tr>
				</xsl:if>

				<xsl:call-template name="normalLayout">
						<xsl:with-param name="type" select="$type" />
				</xsl:call-template>
		</xsl:template>

		<xsl:template name="normalLayout">
				<xsl:param name="type" />
				<tr>
						<th class="CharPerWord" rowspan="7" align="left" valign="top">
								<xsl:text>&#160;</xsl:text>
						</th>
						<th class="Header">
								Draft
						</th>
						<xsl:if test="$type = 'processed'">
								<xsl:apply-templates select="confirmation/processed/draft"/>
						</xsl:if>
						<xsl:if test="$type = 'excluded'">
								<xsl:apply-templates select="confirmation/excluded/draft"/>
						</xsl:if>
						<xsl:if test="$type = 'notProcessed'">
								<xsl:apply-templates select="confirmation/notProcessed/draft"/>
						</xsl:if>
						<xsl:if test="$type = 'total'">
								<xsl:apply-templates select="confirmation/total/draft"/>
						</xsl:if>
				</tr>

				<tr>
						<th class="Header">
								Translated
						</th>
						<xsl:if test="$type = 'processed'">
								<xsl:apply-templates select="confirmation/processed/translated"/>
						</xsl:if>
						<xsl:if test="$type = 'excluded'">
								<xsl:apply-templates select="confirmation/excluded/translated"/>
						</xsl:if>
						<xsl:if test="$type = 'notProcessed'">
								<xsl:apply-templates select="confirmation/notProcessed/translated"/>
						</xsl:if>
						<xsl:if test="$type = 'total'">
								<xsl:apply-templates select="confirmation/total/translated"/>
						</xsl:if>
				</tr>

				<tr>
						<th class="Header">
								Translation Rejected
						</th>
						<xsl:if test="$type = 'processed'">
								<xsl:apply-templates select="confirmation/processed/rejectedTranslation"/>
						</xsl:if>
						<xsl:if test="$type = 'excluded'">
								<xsl:apply-templates select="confirmation/excluded/rejectedTranslation"/>
						</xsl:if>
						<xsl:if test="$type = 'notProcessed'">
								<xsl:apply-templates select="confirmation/notProcessed/rejectedTranslation"/>
						</xsl:if>
						<xsl:if test="$type = 'total'">
								<xsl:apply-templates select="confirmation/total/rejectedTranslation"/>
						</xsl:if>
				</tr>
				<tr>
						<th class="Header">
								Translation Approved
						</th>
						<xsl:if test="$type = 'processed'">
								<xsl:apply-templates select="confirmation/processed/approvedTranslation"/>
						</xsl:if>
						<xsl:if test="$type = 'excluded'">
								<xsl:apply-templates select="confirmation/excluded/approvedTranslation"/>
						</xsl:if>
						<xsl:if test="$type = 'notProcessed'">
								<xsl:apply-templates select="confirmation/notProcessed/approvedTranslation"/>
						</xsl:if>
						<xsl:if test="$type = 'total'">
								<xsl:apply-templates select="confirmation/total/approvedTranslation"/>
						</xsl:if>
				</tr>
				<tr>
						<th class="Header">
								Sign-off Rejected
						</th>
						<xsl:if test="$type = 'processed'">
								<xsl:apply-templates select="confirmation/processed/rejectedSignOff"/>
						</xsl:if>
						<xsl:if test="$type = 'excluded'">
								<xsl:apply-templates select="confirmation/excluded/rejectedSignOff"/>
						</xsl:if>
						<xsl:if test="$type = 'notProcessed'">
								<xsl:apply-templates select="confirmation/notProcessed/rejectedSignOff"/>
						</xsl:if>
						<xsl:if test="$type = 'total'">
								<xsl:apply-templates select="confirmation/total/rejectedSignOff"/>
						</xsl:if>
				</tr>
				<tr>
						<th class="Header">
								Signed Off
						</th>
						<xsl:if test="$type = 'processed'">
								<xsl:apply-templates select="confirmation/processed/approvedSignOff"/>
						</xsl:if>
						<xsl:if test="$type = 'excluded'">
								<xsl:apply-templates select="confirmation/excluded/approvedSignOff"/>
						</xsl:if>
						<xsl:if test="$type = 'notProcessed'">
								<xsl:apply-templates select="confirmation/notProcessed/approvedSignOff"/>
						</xsl:if>
						<xsl:if test="$type = 'total'">
								<xsl:apply-templates select="confirmation/total/approvedSignOff"/>
						</xsl:if>
				</tr>


				<xsl:call-template name="totalCounts" >
						<xsl:with-param name="type" select="$type" />
				</xsl:call-template>

		</xsl:template>

		<xsl:template name="totalCounts">
				<xsl:param name="type" />
				<tr>
						<th class="Total">
								Total
						</th>
						<td class="Total">
								<xsl:if test="$type = 'processed'">
										<xsl:value-of select="confirmation/processed/total/@segments"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:value-of select="confirmation/excluded/total/@segments"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:value-of select="confirmation/notProcessed/total/@segments"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:value-of select="confirmation/total/total/@segments"/>
								</xsl:if>
						</td>

						<td class="Total">
								<xsl:if test="$type = 'processed'">
										<xsl:value-of select="confirmation/processed/total/@words"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:value-of select="confirmation/excluded/total/@words"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:value-of select="confirmation/notProcessed/total/@words"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:value-of select="confirmation/total/total/@words"/>
								</xsl:if>
						</td>

						<td class="Total">
								<xsl:if test="$type = 'processed'">
										<xsl:value-of select="confirmation/processed/total/@characters"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:value-of select="confirmation/excluded/total/@characters"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:value-of select="confirmation/notProcessed/total/@characters"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:value-of select="confirmation/total/total/@characters"/>
								</xsl:if>
						</td>
						<td class="Total">
								100%
						</td>
						<td class="Total">
								<xsl:if test="$type = 'processed'">
										<xsl:value-of select="confirmation/processed/total/@placeables"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:value-of select="confirmation/excluded/total/@placeables"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:value-of select="confirmation/notProcessed/total/@placeables"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:value-of select="confirmation/total/total/@placeables"/>
								</xsl:if>
						</td>
						<td class="Total">
								<xsl:if test="$type = 'processed'">
										<xsl:value-of select="confirmation/processed/total/@tags"/>
								</xsl:if>
								<xsl:if test="$type = 'excluded'">
										<xsl:value-of select="confirmation/excluded/total/@tags"/>
								</xsl:if>
								<xsl:if test="$type = 'notProcessed'">
										<xsl:value-of select="confirmation/notProcessed/total/@tags"/>
								</xsl:if>
								<xsl:if test="$type = 'total'">
										<xsl:value-of select="confirmation/total/total/@tags"/>
								</xsl:if>
						</td>
				</tr>
		</xsl:template>

		<xsl:template name="counts" match="unspecified | draft | translated | rejectedTranslation | approvedTranslation | rejectedSignOff | approvedSignOff">
				<xsl:param name="className"/>

				<td class="WithLineAbove">
						<xsl:value-of select="@segments"/>
				</td>
				<td class="WithLineAbove">
						<xsl:value-of select="@words" />
				</td>
				<td class="WithLineAbove">
						<xsl:value-of select="@characters" />
				</td>
				<td class="WithLineAbove">
						<xsl:value-of select="user:formatPercentage(number(@words), number(../total/@words))"/>
				</td>
				<td class="WithLineAbove">
						<xsl:value-of select="@placeables"/>
				</td>
				<td class="WithLineAbove">
						<xsl:value-of select="@tags"/>
				</td>
		</xsl:template>
</xsl:stylesheet>
