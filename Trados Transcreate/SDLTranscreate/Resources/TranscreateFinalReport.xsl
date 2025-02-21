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
										width: 10%;
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

										.InfoDataText {
										color: #262726;
										font-family: Segoe UI;
										font-size: 13px;
										line-height: 20px;
										text-align: left;
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
														Project:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/project/@name"/>
												</td>
										</tr>
										<tr>
												<td class="InfoItem">
														Back-Project:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/backProject/@name"/>
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
														Source:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/source/@name"/>
												</td>
										</tr>
										<tr>
												<td class="InfoItem">
														Target:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/target/@name"/>
												</td>
										</tr>
										<tr>
												<td class="InfoItem">
														File:
												</td>
												<td class="InfoData">
														<xsl:value-of select="//taskInfo/@file"/>
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

								<xsl:apply-templates select="//translations"/>

						</body>
				</html>
		</xsl:template>

		<xsl:template match="translations">
				<xsl:apply-templates select="version"/>
		</xsl:template>

		<xsl:template match="version">

				<h2 class="first">
						<xsl:value-of select="@type"/>
				</h2>

				<table class="ReportTable" border="0" cellspacing="0" cellpadding="2"  width="100%">
						<tr>
								<th class="TypeHead" width="10%">
										Text Function
								</th>
								<th class="TypeHead" width="25%">
										Source Copy
								</th>
								<th class="TypeHead" width="25%">
										Translation
								</th>
								<th class="TypeHead" width="25%">
										Back-Translation
								</th>
								<th class="TypeHead" width="20%">
										Comments
								</th>
						</tr>
						<xsl:for-each select="segments/segment">
								<tr>
										<td class="InfoDataText">
												<xsl:value-of select="@textFunction"/>
										</td>
										<td class="InfoDataText">
												<xsl:value-of select="source"/>
										</td>
										<td class="InfoDataText">
												<xsl:value-of select="target"/>
										</td>
										<td class="InfoDataText">
												<xsl:value-of select="back"/>
										</td>
										<td class="InfoDataText">
												<xsl:for-each select="comments/comment">
													<p style="margin: 0px; padding: 3px;">
														<xsl:value-of select="."/>
													</p>
												</xsl:for-each>
										</td>
								</tr>
						</xsl:for-each>
				</table>
		</xsl:template>


</xsl:stylesheet>
