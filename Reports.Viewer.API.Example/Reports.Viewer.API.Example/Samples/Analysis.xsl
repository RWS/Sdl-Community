<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:user="http://mycompany.com/mynamespace" xmlns:fo="http://www.w3.org/1999/XSL/Format"  xmlns:XmlReporting="urn:XmlReporting">
  <msxsl:script language="Javascript"  implements-prefix="user">
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
    
    function calculateLearnings(edits, adaptiveWords, baselineWords)
		{
			if (edits == 0 || Math.max(adaptiveWords, baselineWords) == 0)
			{
				return "0.00%";
			}
      return (100 * edits / Math.max(adaptiveWords, baselineWords)).toFixed(2) + "%";
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
        <table  width="100%" border="0" cellpadding="0" cellspacing="0">
          <tr>
            <td width="100%">
              <h1>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Title')" />
              </h1>
            </td>
            <td valign="top">
              <img>
                <xsl:attribute name="src">C:\Repos\Sdl-Community\Reports.Viewer.API.Example\Reports.Viewer.API.Example\Samples\SynergyPower.jpg</xsl:attribute>
              </img>
            </td>
          </tr>
        </table>

        <h2 class="first">
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Summary')" />
        </h2>

        <table class="InfoList" width="100%" border="0" cellpadding="0" cellspacing="2">
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Task')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Name')" />
            </td>
          </tr>

          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Project')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/project/@name"/>
            </td>
          </tr>

          <xsl:if test="//customer/@name">
            <tr>
              <td class="InfoItem">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Customer')" />
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
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_DueDate')" />
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
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_TMs')" />
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
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Language')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/language/@name"/>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Files')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="count(task/file)"/>
            </td>
          </tr>

          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CreatedAt')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/@runAt"/>
            </td>
          </tr>
          <xsl:if test="//taskInfo/@runTime">
            <tr>
              <td class="InfoItem">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_RunTime')" />
              </td>
              <td class="InfoData">
                <xsl:value-of select="//taskInfo/@runTime"/>
              </td>
            </tr>
          </xsl:if>


        </table>

        <h2 class="first">
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Settings')" />
        </h2>

        <table class="InfoList" width="100%" border="0" cellpadding="0" cellspacing="2">
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_ReportCrossFileRepetitions')" />
            </td>
            <td class="InfoData InfoDataMinColWidth">
              <xsl:choose>
                <xsl:when test="//taskInfo/settings/@reportCrossFileRepetitions='no'">
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_No')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Yes')" />
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_ReportInternalFuzzyMatchLeverage')" />
            </td>
            <td class="InfoData">
              <xsl:choose>
                <xsl:when test="//taskInfo/settings/@reportInternalFuzzyLeverage='yes'">
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Yes')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_No')" />
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_ReportLockedSegmentsSeparately')" />
            </td>
            <td class="InfoData">
              <xsl:choose>
                <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Yes')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_No')" />
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_MinimumMatchScore')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@minimumMatchScore"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_SearchMode')" />
            </td>
            <td colspan="2" class="InfoData">
              <xsl:if test="//taskInfo/settings/@searchMode='bestWins'">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_SearchMode_BestWins')" />
              </xsl:if>
              <xsl:if test="//taskInfo/settings/@searchMode='firstWins'">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_SearchMode_FirstWins')" />
              </xsl:if>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_MissingFormattingPenalty')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@missingFormattingPenalty"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_DifferentFormattingPenalty')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@differentFormattingPenalty"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_MultipleTranslationsPenalty')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@multipleTranslationsPenalty"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_AutoLocalizationPenalty')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@autoLocalizationPenalty"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_TextReplacementPenalty')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@textReplacementPenalty"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_AlignmentPenalty')" />
            </td>
            <td class="InfoData">
              <xsl:value-of select="//taskInfo/settings/@alignmentPenalty"/>
              <xsl:text>%</xsl:text>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <tr>
            <td class="InfoItem">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharacterWidthDifferencePenalty')" />
            </td>
            <td class="InfoData">
              <xsl:choose>
                <xsl:when test="//taskInfo/settings/@characterWidthDifferencePenaltyEnabled='yes'">
                  <xsl:value-of select="//taskInfo/settings/@characterWidthDifferencePenalty"/>
                  <xsl:text>%</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharacterWidthDifferencePenaltyNoMatch')" />                
                </xsl:otherwise>
              </xsl:choose>
            </td>
            <td class="InfoData">
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
          <xsl:if test="//taskInfo/settings/@icuTokenization">
            <tr>
              <td class="InfoItem">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_IcuTokenization')" />
              </td>
              <td class="InfoData">
                <xsl:choose>
                  <xsl:when test="//taskInfo/settings/@icuTokenization='True'">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Yes')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_No')" />
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="InfoData">
                <xsl:text>&#160;</xsl:text>
              </td>
            </tr>
          </xsl:if>

          <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
            <tr>
              <td class="InfoItem">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_EnableFuzzyMatchRepair')" />
              </td>
              <td class="InfoData">
                <xsl:choose>
                  <xsl:when test="//taskInfo/settings/@enableFuzzyMatchRepair='yes'">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Yes')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_No')" />
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="InfoData">
                <xsl:text>&#160;</xsl:text>
              </td>
            </tr>

            <tr>
              <td class="InfoItemWithIndent">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_EnableMtFuzzyMatchRepair')" />
              </td>
              <td class="InfoData">
                <xsl:choose>
                  <xsl:when test="//taskInfo/settings/@enableMtFuzzyMatchRepair='yes'">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Yes')" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_No')" />
                  </xsl:otherwise>
                </xsl:choose>
              </td>
              <td class="InfoData">
                <xsl:text>&#160;</xsl:text>
              </td>
            </tr>
          </xsl:if>


          <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
            <tr>
              <td class="InfoItem">
                <xsl:text>&#160;</xsl:text>
              </td>
              <td class="InfoData">
                <xsl:text>&#160;</xsl:text>
              </td>
              <td class="InfoData">
                <xsl:text>&#160;</xsl:text>
              </td>
            </tr>

            <tr>
              <td class="InfoItem">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentMatchOptions')" />
              </td>
              <td class="InfoData">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWordsWholeTU')" />
              </td>
              <td class="InfoData">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentTUFragment')" />
              </td>
            </tr>
            <tr>
              <td class="InfoItemWithIndent">                
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentMinWords')" />
              </td>
              <td class="InfoData">                
                <xsl:value-of select="//taskInfo/settings/@fullRecallMatchedWords"/>
              </td>
              <td class="InfoData">            
                <xsl:value-of select="//taskInfo/settings/@partialRecallMatchedWords"/>
              </td>
            </tr>
            <tr>
              <td class="InfoItemWithIndent">              
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentMinSignificantWords')" />
              </td>
              <td class="InfoData">  
                <xsl:value-of select="//taskInfo/settings/@fullRecallSignificantWords"/>
              </td>
              <td class="InfoData">      
                <xsl:value-of select="//taskInfo/settings/@partialRecallSignificantWords"/>
              </td>
            </tr>
          </xsl:if>

        </table>

        <h2>
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Totals')" />
        </h2>
        <table class="ReportTable" border="0" cellspacing="0" cellpadding="2" width="100%">
          <tr>
            <th class="TypeHead" width="100%">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Total')" />
            </th>
            <th class="TypeHead">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Type')" />
            </th>
            <th class="Unit">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Segments')" />
            </th>
            <xsl:if test="//total/@words">
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Words')" />
              </th>
            </xsl:if>
            <th class="Unit">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Characters')" />
            </th>
            <th class="Unit">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Percent')" />
            </th>
            <th class="Unit UnitWrap">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Placeables')" />
            </th>
            <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
              <th class="Unit UnitWrap">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentRepair')" />
              </th>
            </xsl:if>
            <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWords')" />
                <br/>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWordsWholeTUInfo')" />
              </th>
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWords')" />
                <br/>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentTUFragmentInfo')" />
              </th>
            </xsl:if>
            <th class="Unit UnitWrap">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Learnings')" />
            </th>
            <xsl:if test="//total/@tags">
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Tags')" />
              </th>
            </xsl:if>

          </tr>
          <xsl:apply-templates select="//batchTotal"/>
        </table>

        <h2>
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FileDetails')" />
        </h2>

        <table class="ReportTable" border="0" cellspacing="0" cellpadding="2"  width="100%">
          <tr>
            <th class="TypeHead" width="100%">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_File')" />
            </th>
            <th class="TypeHead">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Type')" />
            </th>
            <th class="Unit">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Segments')" />
            </th>
            <xsl:if test="//total/@words">
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Words')" />
              </th>
            </xsl:if>
            <th class="Unit">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Characters')" />
            </th>
            <th class="Unit">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Percent')" />
            </th>
            <th class="Unit UnitWrap">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Placeables')" />
            </th>
            <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
              <th class="Unit UnitWrap">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentRepair')" />
              </th>
            </xsl:if>
            <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWords')" />
                <br/>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWordsWholeTUInfo')" />
              </th>
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentWords')" />
                <br/>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_FragmentTUFragmentInfo')" />
              </th>
            </xsl:if>
            <th class="Unit UnitWrap">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Learnings')" />
              </th>
            <xsl:if test="//total/@tags">
              <th class="Unit">
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Tags')" />
              </th>
            </xsl:if>
          </tr>
          <xsl:for-each select="//file[generate-id() = generate-id(key('directory', user:getDirectory(string(@name)))[1])]">
            <xsl:sort select="user:getDirectory(string(@name))"/>
            <xsl:call-template name="folder">
              <xsl:with-param name="currentFolder" select="user:getDirectory(string(@name))" />
            </xsl:call-template>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="folder">
    <xsl:param name="currentFolder"/>
    <xsl:if test="local-name() = 'file'">
      <xsl:apply-templates select="//file[starts-with(@name, $currentFolder) and not(contains(substring-after(@name, $currentFolder), '\'))]">
        <xsl:sort select="@name"/>
      </xsl:apply-templates>
    </xsl:if>
    <xsl:if test="local-name() = 'mergedFile'">
      <xsl:apply-templates select="//mergedFile[starts-with(@name, $currentFolder) and not(contains(substring-after(@name, $currentFolder), '\'))]">
        <xsl:sort select="@name"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>
  <xsl:template name="counts" match="locked | perfect | repeated | crossFileRepeated | exact | inContextExact | new | newBaseline | total | fuzzy">
    <xsl:param name="className"/>
    <xsl:choose>
      <xsl:when test="$className='WithLineAbove'">
        <td class="WithLineAbove">
          <xsl:value-of select="@segments"/>
        </td>
        <xsl:choose>
          <xsl:when test="//total/@words">
            <td class="WithLineAbove">
              <xsl:value-of select="@words" />
            </td>
            <td class="WithLineAbove">
              <xsl:value-of select="@characters" />
            </td>
            <td class="WithLineAbove">
              <xsl:value-of select="user:formatPercentage(number(@words), number(../total/@words))"/>
            </td>
            <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
              <td class="WithLineAbove">
                <xsl:value-of select="@repairWords" />
              </td>
            </xsl:if>
            <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
              <td class="WithLineAbove">
                <xsl:value-of select="@fullRecallWords" />
              </td>
              <td class="WithLineAbove">
                <xsl:value-of select="@partialRecallWords" />
              </td>
            </xsl:if>
          </xsl:when>
          <xsl:otherwise>
            <td class="WithLineAbove">
              <xsl:value-of select="@characters" />
            </td>
            <td class="WithLineAbove">
              <xsl:value-of select="user:formatPercentage(number(@characters), number(../total/@characters))"/>
            </td>
            <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
              <td class="WithLineAbove">
                <xsl:value-of select="@repairWords" />
              </td>
            </xsl:if>
            <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
              <td class="WithLineAbove">
                <xsl:value-of select="@fullRecallWords" />
              </td>
              <td class="WithLineAbove">
                <xsl:value-of select="@partialRecallWords" />
              </td>
            </xsl:if>
          </xsl:otherwise>
        </xsl:choose>
        <td class="WithLineAbove">
          <xsl:value-of select="@placeables"/>
        </td>
        <td class="WithLineAbove">
           <!--<xsl:value-of select="user:calculateLearnings(number(@edits), number(@adaptiveWords), number(@baselineWords))"/>-->
        </td>
        <xsl:if test="//total/@tags">
          <td class="WithLineAbove">
            <xsl:value-of select="@tags"/>
          </td>
        </xsl:if>
      </xsl:when>
      <xsl:otherwise>
        <td>
          <xsl:value-of select="@segments"/>
        </td>
        <xsl:choose>
          <xsl:when test="//total/@words">
            <td>
              <xsl:value-of select="@words" />
            </td>
            <td>
              <xsl:value-of select="@characters" />
            </td>
            <td>
              <xsl:value-of select="user:formatPercentage(number(@words), number(../total/@words))"/>
            </td>
          </xsl:when>
          <xsl:otherwise>
            <td>
              <xsl:value-of select="@characters" />
            </td>
            <td>
              <xsl:value-of select="user:formatPercentage(number(@characters), number(../total/@characters))"/>
            </td>
          </xsl:otherwise>
        </xsl:choose>
        <td>
          <xsl:value-of select="@placeables"/>
        </td>
        <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
          <td>
            <xsl:value-of select="@repairWords" />
          </td>
        </xsl:if>
        <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
          <td>
            <xsl:value-of select="@fullRecallWords" />
          </td>
          <td>
            <xsl:value-of select="@partialRecallWords" />
          </td>
        </xsl:if>
        <td>
         <!--leave TD empty for learnings-->
        </td>
        <xsl:if test="//total/@tags">
          <td>
            <xsl:value-of select="@tags"/>
          </td>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template name="countsLearnings" match="newLearnings">
    <xsl:param name="className"/>
    <xsl:choose>
      <xsl:when test="$className='WithLineAbove'">
        <td class="WithLineAbove">
          <xsl:value-of select="@segments"/>
        </td>
        <xsl:choose>
          <xsl:when test="//total/@words">
            <td class="WithLineAbove">
              <xsl:value-of select="@words" />
            </td>
            <td class="WithLineAbove">
              <xsl:value-of select="@characters" />
            </td>
            <td class="WithLineAbove">
              <xsl:value-of select="user:formatPercentage(number(@words), number(../total/@words))"/>
            </td>
            <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
              <td class="WithLineAbove">
                <xsl:value-of select="@repairWords" />
              </td>
            </xsl:if>
            <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
              <td class="WithLineAbove">
                <xsl:value-of select="@fullRecallWords" />
              </td>
              <td class="WithLineAbove">
                <xsl:value-of select="@partialRecallWords" />
              </td>
            </xsl:if>
          </xsl:when>
          <xsl:otherwise>
            <td class="WithLineAbove">
              <xsl:value-of select="@characters" />
            </td>
            <td class="WithLineAbove">
              <xsl:value-of select="user:formatPercentage(number(@characters), number(../total/@characters))"/>
            </td>
            <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
              <td class="WithLineAbove">
                <xsl:value-of select="@repairWords" />
              </td>
            </xsl:if>
            <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
              <td class="WithLineAbove">
                <xsl:value-of select="@fullRecallWords" />
              </td>
              <td class="WithLineAbove">
                <xsl:value-of select="@partialRecallWords" />
              </td>
            </xsl:if>
          </xsl:otherwise>
        </xsl:choose>
        <td class="WithLineAbove">
          <xsl:value-of select="@placeables"/>
        </td>
        <td class="WithLineAbove">
           <xsl:value-of select="user:calculateLearnings(number(@edits), number(@adaptiveWords), number(@baselineWords))"/>
        </td>
        <xsl:if test="//total/@tags">
          <td class="WithLineAbove">
            <xsl:value-of select="@tags"/>
          </td>
        </xsl:if>
      </xsl:when>
      <xsl:otherwise>
        <td>
          <xsl:value-of select="@segments"/>
        </td>
        <xsl:choose>
          <xsl:when test="//total/@words">
            <td>
              <xsl:value-of select="@words" />
            </td>
            <td>
              <xsl:value-of select="@characters" />
            </td>
            <td>
              <xsl:value-of select="user:formatPercentage(number(@words), number(../total/@words))"/>
            </td>
          </xsl:when>
          <xsl:otherwise>
            <td>
              <xsl:value-of select="@characters" />
            </td>
            <td>
              <xsl:value-of select="user:formatPercentage(number(@characters), number(../total/@characters))"/>
            </td>
          </xsl:otherwise>
        </xsl:choose>
        <td>
          <xsl:value-of select="@placeables"/>
        </td>
        <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
          <td>
            <xsl:value-of select="@repairWords" />
          </td>
        </xsl:if>
        <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
          <td>
            <xsl:value-of select="@fullRecallWords" />
          </td>
          <td>
            <xsl:value-of select="@partialRecallWords" />
          </td>
        </xsl:if>
        <td>
          <xsl:value-of select="user:calculateLearnings(number(@edits), number(@adaptiveWords), number(@baselineWords))"/>
        </td>
        <xsl:if test="//total/@tags">
          <td>
            <xsl:value-of select="@tags"/>
          </td>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="file | batchTotal">
    <xsl:if test="local-name() = 'file'">
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
                  <xsl:value-of select="@name"/>
                </a>
                <xsl:text>&#32;(</xsl:text>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_MergedFiles')" />
                <xsl:value-of select="count(mergedFiles/mergedFile)"/>
                <xsl:text>)</xsl:text>
              </th>
              <xsl:choose>
                <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
                  <th class="Header">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Locked')" />
                  </th>
                  <xsl:apply-templates select="analyse/locked"/>
                </xsl:when>
                <xsl:otherwise>
                  <th class="Header">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
                  </th>
                  <xsl:apply-templates select="analyse/perfect"/>
                </xsl:otherwise>
              </xsl:choose>
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
                  <xsl:value-of select="@name"/>
                </a>
                <xsl:text>&#32;(</xsl:text>
                <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_MergedFiles')" />
                <xsl:value-of select="count(mergedFiles/mergedFile)"/>
                <xsl:text>)</xsl:text>
              </th>
              <xsl:choose>
                <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
                  <th class="Header">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Locked')" />
                  </th>
                  <xsl:apply-templates select="analyse/locked"/>
                </xsl:when>
                <xsl:otherwise>
                  <th class="Header">
                    <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
                  </th>
                  <xsl:apply-templates select="analyse/perfect"/>
                </xsl:otherwise>
              </xsl:choose>
            </tr>
          </tbody>
        </xsl:when>
        <xsl:otherwise>
          <tr>
            <th class="File" align="left" valign="top">
              <xsl:value-of select="@name"/>
            </th>
            <xsl:choose>
              <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
                <th class="Header">
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Locked')" />
                </th>
                <xsl:apply-templates select="analyse/locked"/>
              </xsl:when>
              <xsl:otherwise>
                <th class="Header">
                  <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
                </th>
                <xsl:apply-templates select="analyse/perfect"/>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
    <xsl:if test="local-name() = 'batchTotal'">
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
          <tr>
            <th class="File" align="left" valign="top">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Files')" />&#32;
              <xsl:value-of select="count(parent::node()/file)"/>
            </th>
            <th class="Header">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Locked')" />
            </th>
            <xsl:apply-templates select="analyse/locked"/>
          </tr>
        </xsl:when>
        <xsl:otherwise>
          <tr>
            <th class="File" align="left" valign="top">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Files')" />&#32;
              <xsl:value-of select="count(parent::node()/file)"/>
            </th>
            <th class="Header">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
            </th>
            <xsl:apply-templates select="analyse/perfect"/>
          </tr>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
    <xsl:choose>
      <xsl:when test="analyse/internalFuzzy">
        <xsl:call-template name="internalLayout"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="normalLayout"/>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:if test="mergedFiles">
      <tbody style='display: none;'>
        <xsl:attribute name="id">
          <xsl:value-of select="@guid"/>
          <xsl:text>_files</xsl:text>
        </xsl:attribute>
        <xsl:for-each select="mergedFiles/mergedFile">
          <xsl:sort select="@name"/>
          <xsl:call-template name="mergedFile"></xsl:call-template>
        </xsl:for-each>
      </tbody>
    </xsl:if>
  </xsl:template>
  <xsl:template name="normalLayout">
    <xsl:choose>
      <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
        <tr>
          <th class="CharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/crossFileRepeated) + 8}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
          <th   class="Header">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
          </th>
          <xsl:apply-templates select="analyse/perfect"/>
        </tr>
        <tr>
          <th class="Header">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_InContextExact')" />
          </th>
          <xsl:apply-templates select="analyse/inContextExact"/>
        </tr>
      </xsl:when>
      <xsl:otherwise>
        <tr>
          <th class="CharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/crossFileRepeated) + 7}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
          <th class="Header">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_InContextExact')" />
          </th>
          <xsl:apply-templates select="analyse/inContextExact"/>
        </tr>
      </xsl:otherwise>
    </xsl:choose>
    <tr>
      <th class="Header">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Repetitions')" />
      </th>
      <xsl:apply-templates select="analyse/repeated"/>
    </tr>
    <xsl:if test="analyse/crossFileRepeated">
      <tr>
        <th class="Header">
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CrossFileRepetitions')" />
        </th>
        <xsl:apply-templates select="analyse/crossFileRepeated"/>
      </tr>
    </xsl:if>
    <tr>
      <th class="Header">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Exact')" />
      </th>
      <xsl:apply-templates select="analyse/exact"/>
    </tr>

    <xsl:for-each select="analyse/fuzzy">
      <xsl:sort select="@max" order="descending"/>
      <tr>
        <th class="Header">
          <xsl:value-of select="@min" />
          <xsl:text>% - </xsl:text>
          <xsl:value-of select="@max" />
          <xsl:text>%</xsl:text>
        </th>
        <xsl:call-template name="counts"/>
      </tr>
    </xsl:for-each>

    <tr>
      <th class="Header">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_New')" />
      </th>
      <xsl:apply-templates select="analyse/new"/>
    </tr>
    <tr>
      <th class="Header">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Baseline')" />
      </th>
      <xsl:apply-templates select="analyse/newBaseline"/>
    </tr>
     <tr>
      <th class="Header">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_WithLearnings')" />
      </th>
      <xsl:apply-templates select="analyse/newLearnings"/>
    </tr>
    <xsl:call-template name="totalCounts" />
  </xsl:template>
  <xsl:template name="internalLayout">
    <tr>
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
          <th class="CharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/internalFuzzy) + count(analyse/crossFileRepeated) +9}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
        </xsl:when>
        <xsl:otherwise>
          <th class="CharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/internalFuzzy) + count(analyse/crossFileRepeated) +8}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
          <th class="Header">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
          </th>
          <xsl:apply-templates select="analyse/perfect"/>
        </xsl:when>
        <xsl:otherwise>
          <th class="Header">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_InContextExact')" />
          </th>
          <xsl:apply-templates select="analyse/inContextExact"/>
        </xsl:otherwise>
      </xsl:choose>
    </tr>
    <xsl:if test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
      <tr>
        <th class="Header">
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_InContextExact')" />
        </th>
        <xsl:apply-templates select="analyse/inContextExact"/>
      </tr>
    </xsl:if>
    <th class="Header">
      <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Repetitions')" />
    </th>
    <xsl:apply-templates select="analyse/repeated"/>

    <xsl:if test="analyse/crossFileRepeated">
      <tr>
        <th class="Header">
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CrossFileRepetitions')" />
        </th>
        <xsl:apply-templates select="analyse/crossFileRepeated"/>
      </tr>
    </xsl:if>
    <tr>
      <th class="Header">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Exact')" />
      </th>
      <xsl:apply-templates select="analyse/exact"/>
    </tr>

    <xsl:for-each select="analyse/fuzzy">
      <xsl:sort select="@max" order="descending"/>
      <tr>
        <th class="Header">
          <xsl:value-of select="@min" />
          <xsl:text>% - </xsl:text>
          <xsl:value-of select="@max" />
          <xsl:text>%</xsl:text>
        </th>
        <xsl:call-template name="counts"/>
      </tr>
    </xsl:for-each>
    <tr>
      <th class="HeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Internal')" />
      </th>
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@enableFuzzyMatchRepair">
          <td class="WithLineAbove" colspan="10"></td>
        </xsl:when>
        <xsl:when test="//taskInfo/settings/@fullRecallMatchedWords">
          <td class="WithLineAbove" colspan="9"></td>
        </xsl:when>
        <xsl:otherwise>
          <td class="WithLineAbove" colspan="7"></td>
        </xsl:otherwise>
      </xsl:choose>      
    </tr>
    <xsl:for-each select="analyse/internalFuzzy">
      <xsl:sort select="@max" order="descending"/>
      <tr>
        <th class="HeaderWithIndent">
          <xsl:value-of select="@min" />
          <xsl:text>% - </xsl:text>
          <xsl:value-of select="@max" />
          <xsl:text>%</xsl:text>
        </th>
        <xsl:call-template name="counts"/>
      </tr>
    </xsl:for-each>
    <tr>
      <th class="HeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_New')" />
      </th>
      <xsl:apply-templates select="analyse/new">
        <xsl:with-param name="className">WithLineAbove</xsl:with-param>
      </xsl:apply-templates>
    </tr>
     <tr>
      <th class="HeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Baseline')" />
      </th>
      <xsl:apply-templates select="analyse/newBaseline">
        <xsl:with-param name="className">WithLineAbove</xsl:with-param>
      </xsl:apply-templates>
    </tr>
     <tr>
      <th class="HeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_WithLearnings')" />
      </th>
      <xsl:apply-templates select="analyse/newLearnings">
        <xsl:with-param name="className">WithLineAbove</xsl:with-param>
      </xsl:apply-templates>
    </tr>
    <xsl:call-template name="totalCounts" />
  </xsl:template>
  <xsl:template name="mergedFile">
    <xsl:choose>
      <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
        <tr>
          <th class="MergedFile" align="left" valign="top">
            <xsl:value-of select="@name"/>
          </th>
          <th class="MergedHeader">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Locked')" />
          </th>
          <xsl:apply-templates select="analyse/locked"/>
        </tr>
      </xsl:when>
      <xsl:otherwise>
        <tr>
          <th class="MergedFile" align="left" valign="top">
            <xsl:value-of select="@name"/>
          </th>
          <th class="MergedHeader">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
          </th>
          <xsl:apply-templates select="analyse/perfect"/>
        </tr>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:choose>
      <xsl:when test="analyse/internalFuzzy">
        <xsl:call-template name="internalMergedLayout"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="normalMergedLayout"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="totalCounts">
    <tr>
      <th class="Total">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Total')" />
      </th>
      <td class="Total">
        <xsl:value-of select="analyse/total/@segments"/>
      </td>
      <xsl:if test="analyse/total/@words">
        <td class="Total">
          <xsl:value-of select="analyse/total/@words"/>
        </td>
      </xsl:if>
      <td class="Total">
        <xsl:value-of select="analyse/total/@characters"/>
      </td>
      <td class="Total">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Exact')" />
      </td>
      <td class="Total">
        <xsl:value-of select="analyse/total/@placeables"/>
      </td>
      <xsl:if test="//taskInfo/settings/@enableFuzzyMatchRepair">
        <td class="Total">
          <xsl:value-of select="analyse/total/@repairWords"/>
        </td>
      </xsl:if>
      <xsl:if test="//taskInfo/settings/@fullRecallMatchedWords">
        <td class="Total">
          <xsl:value-of select="analyse/total/@fullRecallWords"/>
        </td>
        <td class="Total">
          <xsl:value-of select="analyse/total/@partialRecallWords"/>
        </td>
      </xsl:if>
       <td class="Total">
        <xsl:value-of select="user:calculateLearnings(number(analyse/total/@edits), number(analyse/total/@adaptiveWords), number(analyse/total/@baselineWords))"/>
      </td>
      <xsl:if test="analyse/total/@tags">
        <td class="Total">
          <xsl:value-of select="analyse/total/@tags"/>
        </td>
      </xsl:if>
    </tr>
  </xsl:template>
  <xsl:template name="normalMergedLayout">
    <tr>
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
          <th class="MergedCharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/crossFileRepeated) + 7}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
          <th class="MergedHeader">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
          </th>
          <xsl:apply-templates select="analyse/perfect"/>
        </xsl:when>
        <xsl:otherwise>
          <th class="MergedCharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/crossFileRepeated) + 7}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
          <th class="MergedHeader">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_InContextExact')" />
          </th>
          <xsl:apply-templates select="analyse/inContextExact"/>
        </xsl:otherwise>
      </xsl:choose>
    </tr>
    <tr>
      <th class="MergedHeader">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Repetitions')" />
      </th>
      <xsl:apply-templates select="analyse/repeated"/>
    </tr>
    <xsl:if test="analyse/crossFileRepeated">
      <tr>
        <th class="MergedHeader">
          <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CrossFileRepetitions')" />
        </th>
        <xsl:apply-templates select="analyse/crossFileRepeated"/>
      </tr>
    </xsl:if>
    <tr>
      <th class="MergedHeader">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Exact')" />
      </th>
      <xsl:apply-templates select="analyse/exact"/>
    </tr>
    <xsl:for-each select="analyse/fuzzy">
      <xsl:sort select="@max" order="descending"/>
      <tr>
        <th class="MergedHeader">
          <xsl:value-of select="@min" />
          <xsl:text>% - </xsl:text>
          <xsl:value-of select="@max" />
          <xsl:text>%</xsl:text>
        </th>
        <xsl:call-template name="counts"/>
      </tr>
    </xsl:for-each>
    <tr>
      <th class="MergedHeader">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_New')" />
      </th>
      <xsl:apply-templates select="analyse/new"/>
    </tr>
    <tr>
      <th class="MergedHeader">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Baseline')" />
      </th>
      <xsl:apply-templates select="analyse/newBaseline"/>
    </tr>
      <tr>
      <th class="MergedHeader">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_WithLearnings')" />
      </th>
      <xsl:apply-templates select="analyse/newLearnings"/>
    </tr>
    <xsl:call-template name="totalCounts" />

  </xsl:template>
  <xsl:template name="internalMergedLayout">
    <tr>
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@reportLockedSegmentsSeparately='yes'">
          <th class="MergedCharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/internalFuzzy) + 8}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
          <th class="MergedHeader">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_PerfectMatch')" />
          </th>
          <xsl:apply-templates select="analyse/perfect"/>
        </xsl:when>
        <xsl:otherwise>
          <th class="MergedCharPerWord" rowspan="{count(analyse/fuzzy) + count(analyse/internalFuzzy) + 8}" align="left" valign="top">
            <xsl:if test="analyse/total/@words">
              <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_CharsPerWord')" />&#32;<xsl:value-of select="user:getCharsPerWord(number(analyse/total/@words), number(analyse/total/@characters))"/>
            </xsl:if>
          </th>
          <th class="MergedHeader">
            <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_InContextExact')" />
          </th>
          <xsl:apply-templates select="analyse/inContextExact"/>
        </xsl:otherwise>
      </xsl:choose>
    </tr>
    <tr>
      <th class="MergedHeader">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Exact')" />
      </th>
      <xsl:apply-templates select="analyse/exact"/>
    </tr>

    <xsl:for-each select="analyse/fuzzy">
      <xsl:sort select="@max" order="descending"/>
      <tr>
        <th class="MergedHeader">
          <xsl:value-of select="@min" />
          <xsl:text>% - </xsl:text>
          <xsl:value-of select="@max" />
          <xsl:text>%</xsl:text>
        </th>
        <xsl:call-template name="counts"/>
      </tr>
    </xsl:for-each>
    <tr>
      <th class="MergedHeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Internal')" />
      </th>      
      <xsl:choose>
        <xsl:when test="//taskInfo/settings/@enableFuzzyMatchRepair">
          <td class="WithLineAbove" colspan="10"></td>
        </xsl:when>
        <xsl:when test="//taskInfo/settings/@fullRecallMatchedWords">
          <td class="WithLineAbove" colspan="9"></td>
        </xsl:when>
        <xsl:otherwise>
          <td class="WithLineAbove" colspan="7"></td>
        </xsl:otherwise>
      </xsl:choose>          
    </tr>

    <th class="MergedHeaderWithIndent">
      <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Repetitions')" />
    </th>
    <xsl:apply-templates select="analyse/repeated"/>

    <xsl:for-each select="analyse/internalFuzzy">
      <xsl:sort select="@max" order="descending"/>
      <tr>
        <th class="MergedHeaderWithIndent">
          <xsl:value-of select="@min" />
          <xsl:text>% - </xsl:text>
          <xsl:value-of select="@max" />
          <xsl:text>%</xsl:text>
        </th>
        <xsl:call-template name="counts"/>
      </tr>
    </xsl:for-each>
    <tr>
      <th class="MergedHeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_New')" />
      </th>
      <xsl:apply-templates select="analyse/new">
        <xsl:with-param name="className">WithLineAbove</xsl:with-param>
      </xsl:apply-templates>
    </tr>
    <tr>
      <th class="MergedHeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_Baseline')" />
      </th>
      <xsl:apply-templates select="analyse/newBaseline">
        <xsl:with-param name="className">WithLineAbove</xsl:with-param>
      </xsl:apply-templates>
    </tr>
    <tr>
      <th class="MergedHeaderWithLineAbove">
        <xsl:value-of select="XmlReporting:GetResourceString('AnalysisReport_WithLearnings')" />
      </th>
      <xsl:apply-templates select="analyse/newLearnings">
        <xsl:with-param name="className">WithLineAbove</xsl:with-param>
      </xsl:apply-templates>
    </tr>
    <xsl:call-template name="totalCounts" />
  </xsl:template>

</xsl:stylesheet>
