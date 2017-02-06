<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns="http://www.w3.org/2000/svg">
  <xsl:output method="html" indent="yes"/>


  <xsl:template match="/">


    <html>
      <head>

        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <title>PostEdit.Compare Comparison Log</title>
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
          border: 1px solid #888888;
          }
          td.segmentId
          {
          color: black;
          background-color: #E9E9E9;
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
        </style>

        <!--<link href="SdlXliffCompareReportType01.css"
               rel="stylesheet"
               type="text/css" />-->


      </head>
      <body>


        <table border="1" cellpadding="3"  cellspacing="0" width="100%">
          <tr>
            
            
            <!--<td>
              ID
            </td>-->

            <td  colspan = "2" width="10%" style="white-space:nowrap; background-color: #DFEAEA; color: #003300; font-weight: bold">
              Action
            </td>
            
            
            <td width="10%" style="white-space:nowrap; background-color: #DFEAEA; color: #003300; font-weight: bold">
              Date
            </td>
            
            <td width="10%" style="white-space:nowrap; background-color: #DFEAEA; color: #003300; font-weight: bold">
              Name
            </td>


            <td style=" background-color: #DFEAEA; color: #003300; font-weight: bold">
              Details
            </td>

            
            
          </tr>
       
          
            <xsl:apply-templates select="log_entries"/>

          </table>
      </body>
    </html>

  </xsl:template>

  <xsl:template match="log_entry">


    <tr>
      
      <!--<td>
        <xsl:value-of select="@id"/>
      </td>-->
      <td width="2%" style="border-style: none none none none; white-space:nowrap;">
        <img height="30" width="30">
          <xsl:attribute name="src">
            <xsl:choose>              
              <xsl:when test="(@type = 'ComparisonFoldersCompare') or (@type = 'ComparisonColdersLeftChange') or (@type = 'ComparisonFoldersRightChange')">
                Images/FolderCompare.png
              </xsl:when>
              <xsl:when test="@type = 'ComparisonProjectCompare'">
                Images/ProjectCompare.png
              </xsl:when>
              <xsl:when test="(@type = 'ComparisonProjectNew') or (@type = 'ComparisonProjectEdit') or (@type = 'ComparisonProjectSave') or (@type = 'ComparisonProjectRemove')">
                Images/Projects.png
              </xsl:when>
              <xsl:when test="@type = 'FilesCopy' ">
                Images/FilesCopy.png
              </xsl:when>
              <xsl:when test="@type = 'FilesMove' ">
                Images/FilesMove.png
              </xsl:when>
              <xsl:when test="@type = 'FilesDelete' ">
                Images/FilesDelete.png
              </xsl:when>
              <xsl:when test="(@type = 'FiltersAdd') or (@type = 'FilterEdit') or (@type = 'FiltersDelete') ">
                Images/Filters.png
              </xsl:when>
              <xsl:when test="@type = 'ReportCreate'">
                Images/ReportsSave.png
              </xsl:when>
              <xsl:when test="@type = 'ReportSave' ">
                Images/ReportsSave.png
              </xsl:when>
              <xsl:otherwise>
                <!--Images/Compare.png-->
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </img>
      </td>
     
      <td style="border-style: none none none none;">        
        <span style=" white-space:nowrap; font-style: normal; padding: 3px; color: black; font-weight: normal;">
          <xsl:value-of select="@action"/>
        </span>
      </td>

      <td style="border-style: none none none none;">
        <span style=" white-space:nowrap; font-style: italic; padding: 3px; color:  #003399; font-weight: normal;">
          <xsl:value-of select="@date"/>
        </span>
      </td>
      
      <td width="2%" style="border-style: none none none none; white-space:nowrap;">
        <xsl:choose>
          <xsl:when test="@type = 'ComparisonProjectCompare'">
            <xsl:value-of select="@itemName"/>
          </xsl:when>
          <xsl:when test="(@type = 'ReportCreate') or (@type = 'ReportSave') ">
            <span style=" color: black; font-weight: normal;">
              <xsl:value-of select="@itemName"/>
            </span>
          </xsl:when>
          <xsl:when test="(@type = 'FilesCopy') or (@type = 'FilesMove') or (@type = 'FilesDelete') ">
            <xsl:value-of select="@itemName"/>
          </xsl:when>
          <xsl:otherwise>
            /
          </xsl:otherwise>
        </xsl:choose>
      </td>
    
      
      <td style="border-style: none none none none;">

        <xsl:choose>
          <xsl:when test="(@type = 'ComparisonFoldersCompare') or (@type = 'ComparisonFoldersLeftChange') or (@type = 'ComparisonFoldersRightChange') or (@type = 'ComparisonProjectCompare')">
            <span style="font-style: normal; padding: 3px; color: black; font-weight: normal;">
              &#8226;&#160;
              <span style="font-style: italic; padding: 3px; color: gray; font-weight: normal;">
                <xsl:value-of select="@value01"/>
              </span>
            </span>
            <br/>
            <span style="font-style: normal; padding: 3px; color: black; font-weight: normal;">
              &#8226;&#160;
              <span style="font-style: italic; padding: 3px; color: gray; font-weight: normal;">
                <xsl:value-of select="@value02"/>
              </span>
            </span>
          </xsl:when>
          <xsl:when test="(@type = 'ReportCreate') or (@type = 'ReportSave') ">
            <span style="font-style: normal; padding: 3px; color: black; font-weight: normal;">
              &#8226;&#160;
              <span style="font-style: italic; padding: 3px; color: gray; font-weight: normal;">
                <xsl:value-of select="@value01"/>
              </span>
            </span>
          </xsl:when>
          <xsl:when test="@type = 'FilesCopy'">

            <span width="100%">
              <xsl:for-each select="items">               
                  <xsl:apply-templates select="item"/>                
              </xsl:for-each>
            </span>
           
          </xsl:when>
          <xsl:when test="@type = 'FilesMove'">
            <span width="100%">
              <xsl:for-each select="items">
                <xsl:apply-templates select="item"/>
              </xsl:for-each>
            </span>

          </xsl:when>
          <xsl:when test="@type = 'FilesDelete'">
            <span width="100%">
              <xsl:for-each select="items">
                <xsl:apply-templates select="item"/>
              </xsl:for-each>
            </span>

          </xsl:when>
          <xsl:otherwise>
            
            <span style="color: #003399;">
             ...
            </span>

          </xsl:otherwise>
        </xsl:choose>

        
        
        <!--&lt;br/&gt;-->
        <!--<xsl:value-of select="@detail"/>-->

        <!--<xsl:variable name="myVar">
          <xsl:call-template name="string-replace-all">
            <xsl:with-param name="text" select="@detail" />
            <xsl:with-param name="replace" select="'[BR]'" />
            <xsl:with-param name="by" select="'&#xA;'" />
          </xsl:call-template>

         
        </xsl:variable>
        
        <xsl:value-of  select="$myVar"/>
        <br/>-->
        
        
      </td>

    </tr>
  
   

  </xsl:template>

  <xsl:template match="item">
    <span width="100%" style="white-space:nowrap; border-style: none none none none;">
      <span style="font-style: normal; padding: 3px; color: black; font-weight: normal;">
        &#8226;&#160;
      </span>
      <span style="font-style: italic; padding: 3px; color: gray; font-weight: normal;">
        <xsl:value-of select="@from"/>
      </span>
      <xsl:if test="@to !=''">
        <span style="font-style: normal; padding: 3px; color: black; font-weight: normal;">
          &#160;&#160;&#160;&#8594;&#160;
        </span>
        <span style="font-style: italic; padding: 3px; color: gray; font-weight: normal;">
          <xsl:value-of select="@to"/>
        </span>
      </xsl:if>  
      <br/>
    </span>
  </xsl:template>

  <xsl:template name="string-replace-all">
    <xsl:param name="text" />
    <xsl:param name="replace" />
    <xsl:param name="by" />
    <xsl:choose>
      <xsl:when test="contains($text, $replace)">
        <xsl:value-of select="substring-before($text,$replace)" />
        <xsl:value-of select="$by" />
        <xsl:call-template name="string-replace-all">
          <xsl:with-param name="text"
          select="substring-after($text,$replace)" />
          <xsl:with-param name="replace" select="$replace" />
          <xsl:with-param name="by" select="$by" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
 
 
  





</xsl:stylesheet>
