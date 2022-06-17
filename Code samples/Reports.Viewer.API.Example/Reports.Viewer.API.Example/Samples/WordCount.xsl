<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:user="http://mycompany.com/mynamespace" xmlns:fo="http://www.w3.org/1999/XSL/Format" xmlns:XmlReporting="urn:XmlReporting">
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
		
		function getDirectory(filename)
		{
			var index = filename.lastIndexOf("\\");
			
			if (index == -1)
			{
				return "";
			}
			else
			{
				return filename.substr(0, index + 1);
			}
		}
		
		function getCharsPerWord(words, chars)
		{
			return (chars / words).toFixed(2);
		}	
		]]>
	</msxsl:script>
	<xsl:key name="directory" match="file" use="user:getDirectory(string(@name))"/>
	<xsl:template match="/">
		<html>
			<head>
				<xsl:value-of select="XmlReporting:GetDefaultCssLinkTag()" disable-output-escaping="yes"/>
				<script language="JavaScript">
					<![CDATA[
					
				function OpenFile(guid)
				{
					if (window.external)
					{
						try
						{
							window.external.OpenFile(guid); 
						}
						catch (e)
						{
							document.getElementById(guid).click();
						}
					}
					else
					{
						document.getElementById(guid).click();
					}
				}

				function Switch(fileguid, showhide)
				{
					if (showhide == "hide")
					{
						document.getElementById(fileguid + "_plus").style.display="";
						document.getElementById(fileguid + "_minus").style.display="none";							
						document.getElementById(fileguid + "_files").style.display="none";
					}
					else
					{
						document.getElementById(fileguid + "_plus").style.display="none";
						document.getElementById(fileguid + "_minus").style.display="";							
						document.getElementById(fileguid + "_files").style.display="";
					}
				}
						
					]]>
				</script>
			</head>
			<body>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<table width="100%" border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td width="100%">
								<h1>
									<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Title')"/>
								</h1>
							</td>
							<td valign="top">
								<img>
									<xsl:attribute name="src">
										<xsl:value-of select="XmlReporting:GetImagesUrl()"/>/TradosStudio_Logo.svg
									</xsl:attribute>
								</img>
							</td>
						</tr>
					</table>
					<h2 class="first">
						<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Summary')"/>
					</h2>
					<table class="InfoList" width="100%" border="0" cellpadding="0" cellspacing="2">
						<tr>
							<td class="InfoItem">
								<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Task')" />
							</td>
							<td class="InfoData">
								<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Name')" />
							</td>
						</tr>

						<tr>
							<td class="InfoItem">
								<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Project')"/>
							</td>
							<td class="InfoData">
								<xsl:value-of select="//taskInfo/project/@name"/>
							</td>
						</tr>
						<xsl:if test="//customer/@name">
							<tr>
								<td class="InfoItem">
									<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Customer')"/>
								</td>
								<td class="InfoData">
									<xsl:value-of select="//taskInfo/customer/@name"/>
									<xsl:if test="//customer/@email">
										&#32;(<xsl:value-of select="//taskInfo/customer/@email"/>)
									</xsl:if>
								</td>
							</tr>
						</xsl:if>
						<xsl:if test="//project/@dueDate">
							<tr>
								<td class="InfoItem">
									<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_DueDate')" />
								</td>
								<td class="InfoData">
									<xsl:value-of select="//taskInfo/project/@dueDate"/>
								</td>
							</tr>
						</xsl:if>
						<tr>
							<td class="InfoItem">
								<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Files')" />
							</td>
							<td class="InfoData">
								<xsl:value-of select="count(//language/file)"/>
							</td>
						</tr>
						<tr>
							<td class="InfoItem">
								<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_CreatedAt')" />
							</td>
							<td class="InfoData">
								<xsl:value-of select="//taskInfo/@runAt"/>
							</td>
						</tr>
						<xsl:if test="//taskInfo/@runTime">
							<tr>
								<td class="InfoItem">
									<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_RunTime')" />
								</td>
								<td class="InfoData">
									<xsl:value-of select="//taskInfo/@runTime"/>
								</td>
							</tr>
						</xsl:if>

					</table>
					
					<xsl:for-each select="//language">
						<xsl:call-template name="ProcessLanguage"/>
					</xsl:for-each>

					<h2>
						<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Totals')" />
					</h2>
					<table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
						<xsl:call-template name="WordCountHeading">
							<xsl:with-param name="batch" select="1"/>
						</xsl:call-template>
						<tbody>
							<xsl:call-template name="WriteWordCount">
								<xsl:with-param name="total" select="//batchTotal/total" />
								<xsl:with-param name="batch" select="1"/>
							</xsl:call-template>
						</tbody>
					</table>
				</table>
			</body>
		</html>
	</xsl:template>

	<xsl:template name="ProcessLanguage">
		<h2>
			<xsl:value-of select="@name"/>
		</h2>
		<table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
			<xsl:call-template name="WordCountHeading"/>
			<tbody>
				<xsl:for-each select="file">
					<xsl:call-template name="WriteWordCount">
						<xsl:with-param name="total" select="total" />
					</xsl:call-template>

					<xsl:if test="mergedFiles">
						<tbody style='display: none;'>
							<xsl:attribute name="id">
								<xsl:value-of select="@guid"/>
								<xsl:text>_files</xsl:text>
							</xsl:attribute>

							<xsl:for-each select="mergedFiles/mergedFile">
								<xsl:call-template name="WriteWordCount">
									<xsl:with-param name="total" select="total" />
									<xsl:with-param name="mergeFile" select="1"/>
								</xsl:call-template>
							</xsl:for-each>
						</tbody>
					</xsl:if>

				</xsl:for-each>
				<tr>
					<td class="File" align="left" width="100%">
						<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Total')" />
					</td>
					<td class="Total">
						<xsl:value-of select="sum(file/total/@segments)"/>
					</td>
					<td class="Total">
						<xsl:value-of select="sum(file/total/@words)"/>
					</td>
					<td class="Total">
						<xsl:value-of select="sum(file/total/@characters)"/>
					</td>
					<td class="Total">
						<xsl:value-of select="sum(file/total/@placeables)"/>
					</td>
					<xsl:if test="file/total/@tags">
						<td class="Total">
							<xsl:value-of select="sum(file/total/@tags)"/>
						</td>
					</xsl:if>
				</tr>
			</tbody>
		</table>
	</xsl:template>

	<xsl:template name="WordCountHeading">
		<xsl:param name="batch" select="0"/>
		<thead>
			<tr>
				<th class="TypeHead" width="100%">
					<xsl:choose>
						<xsl:when test="boolean($batch)">
							<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Total')" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_File')" />
						</xsl:otherwise>
					</xsl:choose>
				</th>
				<th class="Unit">
					<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Segments')" />
				</th>
				<xsl:if test="//total/@words">
					<th class="Unit">
						<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Words')" />
					</th>
				</xsl:if>
				<th class="Unit">
					<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Characters')" />
				</th>
				<th class="Unit">
					<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Placeables')" />
				</th>
				<xsl:if test="//total/@tags">
					<th class="Unit">
						<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Tags')" />
					</th>
				</xsl:if>
			</tr>
		</thead>
	</xsl:template>

	<xsl:template name="WriteWordCount">
		<xsl:param name="total"/>
		<xsl:param name="batch" select="0"/>
		<xsl:param name ="mergeFile" select="0"/>

		<xsl:choose>

			<xsl:when test="boolean($batch)">

				<tr>
					<th class="File" align="left" valign="top">
						<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_Files')" />&#32;
						<xsl:value-of select="count(//language/file)"/>
					</th>
					<xsl:call-template name="WriteCounts">
						<xsl:with-param name="total" select="$total" />
					</xsl:call-template>

				</tr>

			</xsl:when>

			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="mergedFiles">
						<tbody>
							<xsl:attribute name="id">
								<xsl:value-of select="@guid"/>
								<xsl:text>_plus</xsl:text>
							</xsl:attribute>
							<tr>
								<th class="File" align="left" valign="top">
									<img>
										<xsl:attribute name="onclick">
											Switch('<xsl:value-of select="@guid"/>','show');
										</xsl:attribute>
										<xsl:attribute name="src">
											<xsl:value-of select="XmlReporting:GetImagesUrl()"/>/plus.gif
										</xsl:attribute>
									</img>
									<xsl:text>&#32;</xsl:text>
									<a>
										<xsl:attribute name="href">
											javascript:OpenFile('<xsl:value-of select="@guid"/>')
										</xsl:attribute>
										<xsl:value-of select="$total/parent::node()/@name"/>
									</a>
									<xsl:text>&#32;(</xsl:text>
									<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_MergedFiles')" />
									<xsl:value-of select="count(mergedFiles/mergedFile)"/>
									<xsl:text>)</xsl:text>
								</th>
								<xsl:call-template name="WriteCounts">
									<xsl:with-param name="total" select="$total" />
								</xsl:call-template>
							</tr>
						</tbody>

						<tbody style='display: none;'>
							<xsl:attribute name="id">
								<xsl:value-of select="@guid"/>
								<xsl:text>_minus</xsl:text>
							</xsl:attribute>
							<tr>
								<th class="File" align="left" valign="top">
									<img>
										<xsl:attribute name="onclick">
											Switch('<xsl:value-of select="@guid"/>','hide');
										</xsl:attribute>
										<xsl:attribute name="src">
											<xsl:value-of select="XmlReporting:GetImagesUrl()"/>/minus.gif
										</xsl:attribute>
									</img>
									<xsl:text>&#32;</xsl:text>
									<a>
										<xsl:attribute name="href">
											javascript:OpenFile('<xsl:value-of select="@guid"/>')
										</xsl:attribute>
										<xsl:value-of select="$total/parent::node()/@name"/>
									</a>
									<xsl:text>&#32;(</xsl:text>
									<xsl:value-of select="XmlReporting:GetResourceString('WordCountReport_MergedFiles')" />
									<xsl:value-of select="count(mergedFiles/mergedFile)"/>
									<xsl:text>)</xsl:text>
								</th>
								<xsl:call-template name="WriteCounts">
									<xsl:with-param name="total" select="$total" />
								</xsl:call-template>
							</tr>
						</tbody>
					</xsl:when>

					<xsl:otherwise>
						<tr>
							<th>
								<xsl:choose>
									<xsl:when test="boolean($mergeFile)">
										<xsl:attribute name="class">MergedFile</xsl:attribute>
									</xsl:when>
									<xsl:otherwise>
										<xsl:attribute name="class">File</xsl:attribute>
									</xsl:otherwise>
								</xsl:choose>

								<xsl:value-of select="@name"/>
							</th>
							<xsl:call-template name="WriteCounts">
								<xsl:with-param name="total" select="$total" />
							</xsl:call-template>
						</tr>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="WriteCounts">
		<xsl:param name="total"/>
		<td>
			<xsl:value-of select="$total/@segments"/>
		</td>
		<td>
			<xsl:value-of select="$total/@words"/>
		</td>
		<td>
			<xsl:value-of select="$total/@characters"/>
		</td>
		<td>
			<xsl:value-of select="$total/@placeables"/>
		</td>
		<xsl:if test="$total/@tags">
			<td>
				<xsl:value-of select="$total/@tags"/>
			</td>
		</xsl:if>
	</xsl:template>



</xsl:stylesheet>
