<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format" xmlns:XmlReporting="urn:XmlReporting">
	<xsl:template match="/task">
		<html>
			<head>
				<link rel="stylesheet" href="C:/Things/Code/SDL/Trados/Bin/Mixed Platforms/Debug/ReportResources/css/reports.css" />
				<style>
					<!-- Add styles for the QA Providers tree view -->
					.qa-providers-container { margin: 20px 0; }
					.qa-providers-container ul { list-style: none; margin: 0; padding: 0; }
					.qa-providers-container li { margin: 4px 0; }
					.qa-providers-container .toggle {
					cursor: pointer;
					font-weight: bold;
					display: inline-block;
					width: 12px;
					text-align: center;
					user-select: none;
					}
					.qa-providers-container .invisible { visibility: hidden; }
					.qa-providers-container .children { display: none; margin-left: 20px; margin-top: 4px; }
					.qa-providers-container li.expanded > .children { display: block; }
					.qa-providers-container .value { color: red; }
					.qa-providers-container .enabled {color: green; }
				</style>
				<script>
					<![CDATA[
					
					function OpenFileAndRunVerification(guid)
					{
						if (window.external)
						{
							try
							{
							    window.chrome.webview.postMessage(guid);
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
					
					function OpenFileAndGoToSegment(guid, paragraph, segment)
					{
						if (window.external)
						{
							try
							{
							    window.chrome.webview.postMessage(guid + "/" + paragraph + "/" + segment);
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
					
					function toggleCategory(toggle) {
						console.log("toggle");
						var li = toggle.parentElement;
						
						if (li.classList.contains('expanded')) {
							li.classList.remove('expanded');
							toggle.textContent = '+';
						} else {
							li.classList.add('expanded');
							toggle.textContent = '–';
						}
						
						
						
					}
					
				window.addEventListener('DOMContentLoaded', function() {
					// Set logo image
					var others = document.querySelectorAll('.other-img');
					others.forEach(function(img) {
						img.src = getLogoUrl(img.getAttribute('data-img'));
					});
					var images = document.querySelectorAll('.flag-img');
					images.forEach(function(img) {
						var lcid = img.getAttribute('data-lcid');
						// Construct the flag URL as needed
						img.src = getFlagUrl(lcid);
					});
				});

				function getFlagUrl(lcid) {
					// Adjust the path as needed for your deployment
					return 'C:/Program Files (x86)/Trados/Trados Studio/Studio18Beta/ReportResources/images/Flags/' + lcid + '.bmp';
				}
				
				function getLogoUrl(imgName) {
				// Adjust the path as needed for your deployment
					return 'C:/Program Files (x86)/Trados/Trados Studio/Studio18Beta/ReportResources/images/' + imgName;
				}

           ]]>
				</script>
			</head>
			<body>

				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td width="100%">
							<h1>
								Verify Files Report
							</h1>
						</td>
						<td valign="top">
							<img class="other-img" data-img="TradosStudio_Logo.svg" src="" alt="Trados Studio Logo" />
						</td>
					</tr>
				</table>

				<h2 class="first">
					Summary
				</h2>

				<table class="InfoTable" border="0" cellspacing="0" cellpadding="2" width="100%">
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
								VerificationReport_Customer
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
								VerificationReport_DueDate
							</td>
							<td class="InfoData">
								<xsl:value-of select="//taskInfo/project/@dueDate"/>
							</td>
						</tr>
					</xsl:if>

					<tr>
						<td class="InfoItem">
							Files:
						</td>
						<td class="InfoData">
							<xsl:value-of select="count(//task/file)"/>
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

				<!-- Add QA Providers section if it exists -->
				<xsl:if test="VerificationSettings">
					<h2>
						Active QA Providers
					</h2>
					<div class="qa-providers-container">
						<xsl:apply-templates select="VerificationSettings" mode="qa-tree"/>
					</div>
				</xsl:if>

				<h2>
					Statistics
				</h2>

				<table border="0" cellspacing="10" cellpadding="0">
					<tr>
						<td valign="top">
							<xsl:call-template name="overview" />
						</td>
					</tr>
				</table>

				<h2>
					File Details
				</h2>

				<table border="0" cellspacing="10" cellpadding="0" width="100%" >
					<tr>
						<td colspan="2">
							<xsl:apply-templates select="file" />
						</td>
					</tr>
				</table>
			</body>
		</html>
	</xsl:template>



	<!-- Templates for QA Providers tree view -->
	<xsl:template match="*[*]" mode="qa-tree">
		<ul>
			<li>
				<span class="toggle" onclick="toggleCategory(this)">+</span>
				<xsl:text> </xsl:text>
				<xsl:value-of select="@Name"/>

				<ul class="children">
					<xsl:apply-templates select="*" mode="qa-tree"/>
				</ul>
			</li>
		</ul>
	</xsl:template>

	<xsl:template match="*[not(*)]" mode="qa-tree">
		<li>
			<span class="toggle invisible">&#160;</span>
			<xsl:text> </xsl:text>
			<xsl:value-of select="@Name"/>
			<xsl:choose>
				<xsl:when test=". = 'True'">
					: <span class="enabled">
						<xsl:value-of select="."/>
					</span>
				</xsl:when>

				<xsl:otherwise>
					: <span class="value">
						<xsl:value-of select="."/>
					</span>
				</xsl:otherwise>
			</xsl:choose>
		</li>
	</xsl:template>

	<xsl:template match="plugin">
		<tr>
			<td class="InfoItem">
				<xsl:choose>
					<xsl:when test="position() = 1">
						VerificationReport_ActiveVerifiers
					</xsl:when>
					<xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="InfoData">
				<xsl:value-of select="@name" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template name="overview">
		<xsl:variable name="nErrors" select="count(file/Messages/Message[(ErrorLevel='Error' or ErrorLevel='Unspecified') and Ignored='false'])" />
		<xsl:variable name="nWarnings" select="count(file/Messages/Message[ErrorLevel='Warning' and Ignored='false'])" />
		<xsl:variable name="nInformation" select="count(file/Messages/Message[ErrorLevel='Note' and Ignored='false'])" />
		<xsl:variable name="nIgnored" select="count(file/Messages/Message[Ignored='true'])" />

		<table class="VerificationStatisticsTable" border="0" cellspacing="0" cellpadding="2" width="200px">
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nErrors > 0">font-weight:bold;color:red;</xsl:if>
				</xsl:attribute>
				<td>
					Errors:
				</td>
				<td>
					<xsl:value-of select="$nErrors"/>
				</td>
			</tr>
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nWarnings > 0">font-weight:bold;</xsl:if>
				</xsl:attribute>
				<td>
					Warnings:
				</td>
				<td>
					<xsl:value-of select="$nWarnings"/>
				</td>
			</tr>
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nInformation > 0">font-weight:bold;</xsl:if>
				</xsl:attribute>
				<td>
					Information:
				</td>
				<td>
					<xsl:value-of select="$nInformation"/>
				</td>
			</tr>
			<tr	style="font-weight:bold;background-color:#aed0f0;">
				<td>
					Total:
				</td>
				<td>
					<xsl:value-of select="$nErrors + $nWarnings + $nInformation"/>
				</td>
			</tr>
			<tr>
				<td>
					<br/>
				</td>
				<td>
					<br/>
				</td>
			</tr>
			<tr>
				<xsl:attribute name="style">
					<xsl:if test="$nIgnored > 0">font-weight:bold;color:red</xsl:if>
				</xsl:attribute>
				<td>
					Ignored:
				</td>
				<td>
					<xsl:value-of select="$nIgnored"/>
				</td>
			</tr>
		</table>
	</xsl:template>


	<xsl:template match="file">

		<table class="VerificationReportTable" border="0" cellspacing="0" cellpadding="2">

			<tr>
				<th>
					<!--Condition the header colspan for the two different versions-->
					<xsl:choose>
						<xsl:when test="//taskInfo/@version">
							<xsl:attribute name="colspan">
								9
							</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="colspan">
								6
							</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:attribute name="style">
						<xsl:if test="count(Messages/Message[ErrorLevel='Error' or ErrorLevel='Unspecified'])">color:red;</xsl:if>
					</xsl:attribute>
					<img class="flag-img" data-lcid="{@lcid}" src="" alt="Flag" />
					&#160;
					<a>
						<xsl:attribute name="href">
							javascript:OpenFileAndRunVerification('<xsl:value-of select="@guid"/>')
						</xsl:attribute>
						<xsl:value-of select="@name"/>
					</a>
					<a>
						<xsl:attribute name="id">
							<xsl:value-of select="@guid"/>
						</xsl:attribute>
						<xsl:attribute name="style">display:none</xsl:attribute>
						<xsl:attribute name="href">
							<xsl:value-of select="@path"/>
						</xsl:attribute>
						<xsl:value-of select="@name"/>
					</a>
				</th>
			</tr>

			<xsl:choose>
				<xsl:when test="(Messages/@includeIgnored='False' and count(Messages/Message[Ignored='false'])=0) or (Messages/@includeIgnored='True' and count(Messages/Message)=0)">
					<tr>
						<td valign="top">&#160;</td>
						<td valign="top"></td>
						<td valign="top" colspan="3" width="100%">
							VerificationReport_NoErrorsReported
						</td>
						<td valign="top">&#160;</td>
					</tr>
				</xsl:when>

				<xsl:otherwise>
					<tr style="color:gray">
						<td valign="top">&#160;</td>
						<td align="center" valign="top" style="font-weight:bold;color:gray">
							&#160;Segment&#160;
						</td>
						<td valign="top">&#160;</td>
						<td valign="top" style="font-weight:bold;color:gray">
							Type
						</td>
						<td valign="top" style="font-weight:bold;color:gray">
							Message
						</td>

						<td valign="top" style="font-weight:bold;color:gray">
							More Specific
						</td>
						<xsl:if test="//taskInfo/@version">
							<td id="newSourceSegment" valign="top" style="font-weight:bold;color:gray">
								Source
							</td>
							<td id="newTargetSegment" valign="top" style="font-weight:bold;color:gray">
								Target
							</td>
						</xsl:if>

						<td valign="top" style="font-weight:bold;color:gray">
							Verifier
						</td>
					</tr>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</table>

		<!-- Add a gap between each file -->
		&#160;&#160;&#160;


	</xsl:template>

	<xsl:template match="Message">

		<xsl:variable name="IncludeIgnored" select="../@includeIgnored"/>

		<xsl:if test="Ignored='false' or $IncludeIgnored ='True'">

			<xsl:variable name="GrayStyle">
				<xsl:if test="Ignored='true'">
					color:gray;
				</xsl:if>
			</xsl:variable>

			<tr>
				<td valign="top">&#160;</td>

				<td valign="top" align="left" style="{$GrayStyle}">
					<a>
						<xsl:attribute name="href">
							javascript:OpenFileAndGoToSegment('<xsl:value-of select="../../@guid"/>', '<xsl:value-of select="ParagraphUnitId"/>', '<xsl:value-of select="SegmentId"/>')
						</xsl:attribute>
						<xsl:value-of select="SegmentId"/>
					</a>
				</td>

				<xsl:apply-templates select="ErrorLevel"/>

				<!-- condition also the view of the columns-->
				<td>
					<xsl:choose>
						<xsl:when test="//taskInfo/@version">
							<xsl:attribute name="width">
								40%
							</xsl:attribute>
							<xsl:attribute name ="valign">
								top
							</xsl:attribute>
							<xsl:attribute name ="align">
								left
							</xsl:attribute>
							<xsl:attribute name ="style">
								{$GrayStyle}
							</xsl:attribute>
						</xsl:when>

						<xsl:otherwise>
							<xsl:attribute name="width">
								100%
							</xsl:attribute>
							<xsl:attribute name ="valign">
								top
							</xsl:attribute>
							<xsl:attribute name ="align">
								left
							</xsl:attribute>
							<xsl:attribute name ="style">
								{$GrayStyle}
							</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:value-of select="Text"/>
				</td>
				<td>More Specific</td>

				<xsl:if test="//taskInfo/@version">
					<td valign="top" align="left" width ="30%" style="white-space:normal; {$GrayStyle}">
						<xsl:value-of select="Source"/>
					</td>
					<td valign="top" align ="left" width ="30%" style="white-space:normal; {$GrayStyle}">
						<xsl:value-of select = "Target" />
					</td>
				</xsl:if>

				<td valign="top" style="white-space:nowrap; {$GrayStyle}">
					<xsl:value-of select="Origin"/>
				</td>
			</tr>
		</xsl:if>

	</xsl:template>

	<xsl:template match="ErrorLevel">
		<td valign="top">
			<xsl:choose>
				<xsl:when test="text()='Error'">
					<img class="other-img" data-img="error.gif" src="" alt="Error" />
				</xsl:when>
				<xsl:when test="text()='Warning'">
					<img class="other-img" data-img="warning.gif" src="" alt="Warning" />
				</xsl:when>
				<xsl:when test="text()='Note'">&#160;</xsl:when>
				<xsl:when test="text()='Unspecified'">
					<img class="other-img" data-img="error.gif" src="" alt="Error" />
				</xsl:when>
			</xsl:choose>
		</td>

		<xsl:variable name="GrayStyle">
			<xsl:if test="../Ignored='true'">
				color:gray;
			</xsl:if>
		</xsl:variable>

		<td valign="top" style="white-space: nowrap;padding-right:10px; {$GrayStyle}">
			<xsl:choose>
				<xsl:when test="text()='Error'">
					Error
				</xsl:when>
				<xsl:when test="text()='Warning'">
					Warning
				</xsl:when>
				<xsl:when test="text()='Note'">
					Information
				</xsl:when>
				<xsl:when test="text()='Unspecified'">
					UnknownError
				</xsl:when>
			</xsl:choose>
		</td>
	</xsl:template>
</xsl:stylesheet>