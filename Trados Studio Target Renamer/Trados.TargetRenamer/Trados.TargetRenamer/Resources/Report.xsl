<?xml version="1.0" encoding="iso-8859-1"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
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
					text-align: left;
					padding: 5px;
					}

					.ReportTable td.HeaderText {
					text-align: left;
					background-color: #F8FAFA;
					font-size: 11px;
					line-height: 20px;
					padding: 5px;
					color: #0A1E2C;
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

					.Type {
					border-left: #e5ecf0 1px solid;
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
				</style>
			</head>
			<body>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td width="100%">
							<h1>
								<xsl:value-of select="//@name" />&#160;Report
							</h1>
						</td>
						<td valign="top">
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
							<xsl:value-of select="//@name" />
						</td>
					</tr>

					<tr>
						<td class="InfoItem">
							Project:
						</td>
						<td class="InfoData">
							<xsl:value-of select="//taskInfo/project/@name" />
						</td>
					</tr>

					<xsl:if test="//customer/@name">
						<tr>
							<td class="InfoItem">
								Customer:
							</td>
							<td class="InfoData">
								<xsl:value-of select="//taskInfo/customer/@name" />
								<xsl:if test="//customer/@email and //customer/@email != ''">
									&#32;(<xsl:value-of select="//taskInfo/customer/@email" />)
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
								<xsl:value-of select="//taskInfo/project/@dueDate" />
							</td>
						</tr>
					</xsl:if>
					<tr>
						<td class="InfoItem">
							Language:
						</td>
						<td class="InfoData">
							<xsl:value-of select="//taskInfo/language/@name" />
						</td>
					</tr>
					<tr>
						<td class="InfoItem">
							Files:
						</td>
						<td class="InfoData">
							<xsl:value-of select="count(task/files/file)" />
						</td>
					</tr>
					<tr>
						<td class="InfoItem">
							Location:
						</td>
						<td class="InfoData">
							<a>
								<xsl:attribute name="href">
									<xsl:value-of select="//settings/@path" />
								</xsl:attribute>
								<xsl:value-of select="//settings/@path" />
							</a>
						</td>
					</tr>

					<tr>
						<td class="InfoItem">
							Created At:
						</td>
						<td class="InfoData">
							<xsl:value-of select="//taskInfo/@runAt" />
						</td>
					</tr>
				</table>

				<h2 class="first">
					Settings
				</h2>

				<table class="InfoList" width="100%" border="0" cellpadding="0" cellspacing="2">
					<xsl:choose>
						<xsl:when test="//taskInfo/settings/@useRegExpr = 'True'">
							<tr>
								<td class="InfoItem">
									Action:
								</td>
								<td class="InfoData">
									Used regular expression.
								</td>
							</tr>
							<tr>
								<td class="InfoItem">
									Searched for:
								</td>
								<td class="InfoData">
									<xsl:value-of select="//taskInfo/settings/@regExprSearchFor" />
								</td>
							</tr>
							<tr>
								<td class="InfoItem">
									Replaced with:
								</td>
								<td class="InfoData">
									<xsl:value-of select="//taskInfo/settings/@regExprReplaceWith" />
								</td>
							</tr>
						</xsl:when>
						<xsl:otherwise>
							<xsl:choose>
								<xsl:when test="//taskInfo/settings/@suffix = 'True'">
									<tr>
										<td class="InfoItem">
											Action:
										</td>
										<td class="InfoData">
											Appended as Suffix.
										</td>
									</tr>
								</xsl:when>
								<xsl:otherwise>
									<tr>
										<td class="InfoItem">
											Action:
										</td>
										<td class="InfoData">
											Appended as Prefix.
										</td>
									</tr>
								</xsl:otherwise>
							</xsl:choose>
							<tr>
								<td class="InfoItem">
									Delimiter:
								</td>
								<td class="InfoData">
									<xsl:value-of select="//taskInfo/settings/@delimiter" />
								</td>
							</tr>
							<xsl:choose>
								<xsl:when test="//taskInfo/settings/@targetLanguage = 'True'">
									<tr>
										<td class="InfoItem">
											Append Target Language:
										</td>
										<td class="InfoData">
											<xsl:value-of select="//taskInfo/settings/@targetLanguage" />
										</td>
									</tr>
									<tr>
										<td class="InfoItem">
											Use short locales:
										</td>
										<td class="InfoData">
											<xsl:value-of select="//taskInfo/settings/@shortLocales" />
										</td>
									</tr>
								</xsl:when>
								<xsl:otherwise>
									<tr>
										<td class="InfoItem">
											Append Custom String:
										</td>
										<td class="InfoData">
											<xsl:value-of select="//taskInfo/settings/@useCustomString" />
										</td>
									</tr>
									<tr>
										<td class="InfoItem">
											Custom String:
										</td>
										<td class="InfoData">
											<xsl:value-of select="//taskInfo/settings/@customString" />
										</td>
									</tr>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</table>

				<h2 class="firstWithEmphasis">
					Renamed Files
				</h2>

				<table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<th class="TypeHead" width="50%">
							Original Filename
						</th>
						<th class="TypeHead" width="50%">
							Target Filename
						</th>
					</tr>

					<xsl:for-each select="//files/file/@location[not(.=preceding::file/@location)]">
						<xsl:sort />
						<xsl:variable name ="Location" select="." />
						<tr>
							<td class="HeaderText">
								Location:
								<xsl:value-of select="." />
							</td>
							<td class="HeaderText">
								<a>
									<xsl:attribute name="href">
										<xsl:value-of select="../@newLocation" />
									</xsl:attribute>
									<xsl:value-of select="../@newLocation" />
								</a>
							</td>
						</tr>
						<xsl:for-each select="//files/file[@location = current()]">
							<tr>
								<td class="TextIndented">
									<xsl:value-of select="@originalName" />
								</td>
								<td class="TextIndented">
									<xsl:value-of select="@newName" />
								</td>
							</tr>
						</xsl:for-each>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>