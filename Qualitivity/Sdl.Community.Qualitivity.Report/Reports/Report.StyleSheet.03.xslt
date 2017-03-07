<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns="http://www.w3.org/2000/svg">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">

    <html>
      <head>
        <meta http-equiv="X-UA-Compatible" content="IE=9"/>
        <title>Qualitivity - Project Activity Report</title>


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
            color: #08298A;
            background-color: #D8D8D8;
            font-weight: bold;

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

        <script LANGUAGE="JavaScript">

          function Toggle(node) {
          if (node.nextElementSibling.style.visibility == '')
          {
          //if (node.children.length > 0) {
          if (node.children.item(0).tagName == "IMG") {
          node.children.item(0).src = "data:image/gif;base64,R0lGODlhCQAJAJEAAP7+/oKCggICAgAAACwAAAAACQAJAAACFIyPoAu2spyCyol7W3hxz850CFIAADs=";
          }
          //}
          node.nextElementSibling.style.visibility = 'collapse';

          }
          else {
          //if (node.children.length > 0) {
          if (node.children.item(0).tagName == "IMG") {
          node.children.item(0).src = "data:image/gif;base64,R0lGODlhCQAJAJEAAP7+/oKCggICAgAAACwAAAAACQAJAAACEYyPoAvG614LQFg7ZZbxoR8UADs=";
          }
          //}
          node.nextElementSibling.style.visibility = '';
          }
          }
        </script>

      </head>
      <body>
        <table cellpadding="0" cellspacing="0" style="margin-bottom: 4px;border:none; padding 10px; color: #084B8A; background-color: #F2F2F2; " border="0" width="100%">
          <tr>
            <td style="white-space: nowrap;color: #084B8A;border:none; text-align: center; font-size:20px;font-weight: bold;">
              Project Activity Report
            </td>
          </tr>
        </table>         
        <xsl:apply-templates select="activityRecords"/>
      </body>
    </html>

  </xsl:template>
  
  <xsl:template match="activityRecords">

    <table cellpadding="0" cellspacing="0" style="border:none; margin-bottom: 4px; color: #585858; padding 10px;" border="0" width="100%">
      <tr>
        <td width="100%" style="white-space: nowrap; border:none; text-align: center; font-size:12px;">
          <div>
            <span style="font-size:12px;">
              <xsl:value-of select="userProfile/@name"/>            
            </span>
            <span style="font-size:11px;">
              <xsl:if test="userProfile/@name != userProfile/@user_name">
                <xsl:if test="userProfile/@user_name !=''">
                  <xsl:if test="userProfile/@name !=''">
                    &#160;-&#160;
                  </xsl:if>
                  <xsl:value-of select="userProfile/@user_name"/> 
                </xsl:if>
              </xsl:if>
            </span>
            <span style="font-size:11px;">
              <xsl:if test="userProfile/@street !=''">
                &#160;-&#160;
                <xsl:value-of select="userProfile/@street"/>                              
              </xsl:if>
              <xsl:if test="userProfile/@zip_city_state !=''">
                &#160;-&#160;
                <xsl:value-of select="userProfile/@zip_city_state"/>
              </xsl:if>
              <xsl:if test="userProfile/@country != ''">
                &#160;-&#160;
                <xsl:value-of select="userProfile/@country"/>               
              </xsl:if>                
            </span>
            <span style="font-size:11px; font-style: italic;">
              <xsl:if test="userProfile/@email != ''">
                &#160;-&#160;
                (<xsl:value-of select="userProfile/@email"/>)               
              </xsl:if>
            </span>
            <span style="font-size:11px; font-style: normal;">
              <xsl:if test="userProfile/@vat_code != ''">
                &#160;-&#160;
                VAT: <xsl:value-of select="userProfile/@vat_code"/>
              </xsl:if>
            </span>
          </div>
        </td>
      </tr>
    </table>
    <br/>
    <table cellpadding="0" cellspacing="0" style="border:none; margin-bottom: 4px; padding 10px;" border="0" width="100%">
      <tr>
        <td  style="white-space: nowrap; border:none; text-align: left; font-size:12px;">          
          <div>           
            <fieldset>
              <legend accesskey="I">
                <span style="font-weight:bold;color:#084B8A;">Project Details</span>
              </legend>              
            
             <table cellpadding="0" cellspacing="0" style="border:none; margin-bottom: 4px; padding 10px; vertical-align: top;" border="0" width="100%">
               <tr>
                 <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
               </tr>
              <tr>
                <td width="10%" style="white-space: nowrap; border:none; color: #585858; text-align: right; font-size:12px; padding: 0px 0px 0px 10px;">
                  Name:
                </td>
                <td width="90%" style="white-space: nowrap; border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
                  <xsl:value-of select="project/@name"/>
                </td>
              </tr>
              <tr>
                <td width="10%" style="white-space: nowrap; border:none; color: #585858; text-align: right; font-size:12px; padding: 0px 0px 0px 10px;">
                  Status:
                </td>
                <td width="90%" style="white-space: nowrap; border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
                  <xsl:value-of select="project/@status"/>
                </td>
              </tr>
              <tr>
                <td width="10%" style="white-space: nowrap; border:none; color: #585858; text-align: right; font-size:12px; padding: 0px 0px 0px 10px;">
                  Description:
                </td>
                <td width="90%" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
                  <xsl:value-of select="project/@description"/>
                </td>
              </tr>
               <tr>
                 <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
               </tr>
            </table>
            </fieldset>
          </div>
        </td>
        <td width="30%" style="white-space: nowrap; border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
          <div>
            <fieldset>
              <legend accesskey="I">
                <span style="font-weight:bold;color:#084B8A;">Company Details</span>
              </legend>

              <table cellpadding="0" cellspacing="0" style="border:none; margin-bottom: 4px; padding 10px; vertical-align: top;" border="0" width="100%">
                <tr>
                  <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                </tr>
                <tr>
                  <td width="10%" style="white-space: nowrap; border:none; color: #585858; text-align: right; font-size:12px; padding: 0px 0px 0px 10px;">
                    Name:
                  </td>
                  <td width="90%" style="white-space: nowrap; border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
                    <xsl:value-of select="companyProfile/@name"/>
                  </td>
                </tr>
                <tr>
                  <td width="10%" style="white-space: nowrap; border:none; color: #585858; text-align: right; font-size:12px; padding: 0px 0px 0px 10px;">
                    Address:
                  </td>
                  <td width="90%" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
                    <xsl:value-of select="companyProfile/@full_address"/>
                  </td>
                </tr>
                <tr>
                  <td width="10%" style="white-space: nowrap; border:none; color: #585858; text-align: right; font-size:12px; padding: 0px 0px 0px 10px;">
                    Contact:
                  </td>
                  <td width="90%" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">
                    <xsl:choose>

                      <xsl:when test="companyProfile/@contact_name != ''">
                        <xsl:value-of select="companyProfile/@contact_name"/>
                      </xsl:when>
                      <xsl:otherwise>
                        
                      </xsl:otherwise>
                    </xsl:choose>
                    
                    <xsl:if test="companyProfile/@email != ''">&#160;<span style="font-style: italic;">(<xsl:value-of select="companyProfile/@email"/>)</span>
                    </xsl:if>
                  </td>
                </tr>
                <tr>
                  <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                </tr>
              </table>
            </fieldset>
          </div>
        </td>      
      </tr>
    </table>
    <br/>
    <table cellpadding="0" cellspacing="0" style="border:none; margin-bottom: 4px; padding 10px;" border="0" width="100%">
      <tr>
        <td width="100%" style="border:none; ">
          <fieldset>
            <legend accesskey="I">
              <span style="font-weight:bold;color:#084B8A;">Activity Overview</span>
            </legend>
            <div style="padding: 10px;">
              <xsl:for-each select="activities">
                <xsl:if test="@count > 0">
                  <xsl:for-each select="currency">
                    <br/>
                    <div>
                      <span class="italicGray" style="font-size: 10px;">
                        Currency:&#160;<xsl:value-of select="@currency"/>
                      </span>
                    </div>
                    <table cellpadding="0" cellspacing="0" style="border:none;" border="0" width="100%">
                      <tr >
                        <th >
                          ID
                        </th>
                        <th >
                          Activity
                        </th>
                        <th >
                          Description
                        </th>
                        <th >
                          Status
                        </th>
                        <th >
                          Started
                        </th>
                        <th >
                          Stopped
                        </th>
                        <th >
                          Soruce
                        </th>
                        <th >
                          Target
                        </th>
                        <th >
                          Language
                        </th>
                        <th >
                          Hourly
                        </th>
                        <th >
                          Custom
                        </th>
                        <th >
                          Total
                        </th>
                      </tr>
                      <xsl:for-each select="activity">
                        <tr >
                          <td width="2%" style="white-space: nowrap;padding: 10px 5px 2px 2px" >
                            <xsl:value-of select="@id"/>
                          </td>
                          <td width="40%" style="padding: 10px 5px 2px 5px" >
                            <xsl:value-of select="@name"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                            <xsl:value-of select="@description"/>
                          </td>
                          <td width="5%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                            <xsl:value-of select="@activity_status"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                            <xsl:value-of select="@started"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                            <xsl:value-of select="@stopped"/>
                          </td>
                          <td width="5%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                            <xsl:value-of select="@source_language"/>
                          </td>
                          <td width="5%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                            <xsl:value-of select="@target_language"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right;">
                            <xsl:value-of select="@language_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                            <xsl:value-of select="@currency"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right;">
                            <xsl:value-of select="@hourly_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                            <xsl:value-of select="@currency"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right;">
                            <xsl:value-of select="@custom_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                            <xsl:value-of select="@currency"/>
                          </td>
                          <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right;">
                            <xsl:value-of select="@activity_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                            <xsl:value-of select="@currency"/>
                          </td>
                        </tr>
                      </xsl:for-each>
                      <tr >
                        <td width="2%" style="border:none; padding: 10px 2px 2px 2px" >
                          &#160;
                        </td>
                        <td width="40%" style="border:none; padding: 10px 5px 2px 5px" >
                          &#160;
                        </td>
                        <td width="10%" style="white-space: nowrap;border:none; padding: 10px 5px 2px 5px">
                          &#160;
                        </td>
                        <td width="5%" style="white-space: nowrap; border:none;padding: 10px 5px 2px 5px">
                          &#160;
                        </td>
                        <td width="10%" style="white-space: nowrap; border:none;padding: 10px 5px 2px 5px">
                          &#160;
                        </td>
                        <td width="10%" style="white-space: nowrap; border:none;padding: 10px 5px 2px 5px">
                          &#160;
                        </td>
                        <td width="5%" style="white-space: nowrap;border:none; padding: 10px 5px 2px 5px">
                          &#160;
                        </td>
                        <td width="5%" style="white-space: nowrap;border:none; padding: 10px 5px 2px 5px">
                          &#160;
                        </td>
                        <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right; font-weight: bold">
                          <xsl:value-of select="@language_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                          <xsl:value-of select="@currency"/>
                        </td>
                        <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right; font-weight: bold">
                          <xsl:value-of select="@hourly_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                          <xsl:value-of select="@currency"/>
                        </td>
                        <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right; font-weight: bold">
                          <xsl:value-of select="@custom_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                          <xsl:value-of select="@currency"/>
                        </td>
                        <td width="10%" style="white-space: nowrap; padding: 10px 10px 2px 5px; text-align:right; font-weight: bold">
                          <xsl:value-of select="@currency_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                          <xsl:value-of select="@currency"/>
                        </td>
                      </tr>
                    </table>
                  </xsl:for-each>
                  <br/>
                </xsl:if>
              </xsl:for-each>
            </div>
          </fieldset>
        </td>
      </tr>
    </table>
    <br/>
    <table cellpadding="0" cellspacing="0" style="border:none; margin-bottom: 4px; padding 10px;" border="0" width="100%">
      <tr>
        <td width="100%" style="border:none; ">

          <xsl:for-each select="activities">
            <xsl:if test="@count > 0">
              <xsl:for-each select="currency">
                <xsl:for-each select="activity">
                  <a onClick="Toggle(this)" style="padding: 12px 0px 12px 0px; cursor: pointer;">
                    <img src="data:image/gif;base64,R0lGODlhCQAJAJEAAP7+/oKCggICAgAAACwAAAAACQAJAAACFIyPoAu2spyCyol7W3hxz850CFIAADs=" style="vertical-align: center; padding: 0px 2px 0px 0px;"/>

                    <span style="font-weight:bold;color:#084B8A;">&#160;Activity Details:</span>&#160;
                    <span style="font-weight: bold;color:#084B8A;">
                      <xsl:value-of select="@id"/>
                    </span>&#160;&#160;<span style="color:#084B8A;">
                      <xsl:value-of select="@name"/>
                    </span>
                    
                  </a>
                  <div style="visibility: collapse; ">
                    <fieldset>
                      <table cellpadding="0" cellspacing="0" style="border:none;" border="0" width="100%">
                        <tr>
                          <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right; color:#585858;" >ID</td>
                          <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                            <xsl:value-of select="@id"/>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Name</td>
                          <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                            <xsl:value-of select="@name"/>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Description</td>
                          <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                            <xsl:value-of select="@description"/>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Started</td>
                          <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                            <xsl:value-of select="@started"/>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Stopped</td>
                          <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                            <xsl:value-of select="@stopped"/>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Language</td>
                          <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                            <xsl:value-of select="@source_language"/> -  <xsl:value-of select="@target_language"/>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;">Documents</td>
                          <td width="90%" style="border:none; padding: 10px 10px 10px 10px;  text-align: left;" >
                            <xsl:if test="document_activities/@count > 0 ">
                              <div>

                                <table cellpadding="0" cellspacing="0" style="border:none; text-align:left" border="0" width="100%">
                                  <tr style="color: #084B8A;" >
                                    <td >
                                      Document Name
                                    </td>
                                    <td >
                                      Activity Type
                                    </td>
                                    <td >
                                      Opened
                                    </td>
                                    <td >
                                      Closed
                                    </td>
                                    <td >
                                      Duration
                                    </td>
                                    <td >
                                      Segments Modified
                                    </td>
                                  </tr>
                                  <xsl:for-each select="document_activities/document_activity">
                                    <tr>
                                      <td width="40%" style="white-space: nowrap;padding: 10px 5px 2px 2px" >
                                        <xsl:value-of select="@document_name"/>
                                      </td>
                                      <td width="10%" style="padding: 10px 5px 2px 5px" >
                                        <xsl:value-of select="@document_activity_type"/>
                                      </td>
                                      <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                                        <xsl:value-of select="@started"/>
                                      </td>
                                      <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                                        <xsl:value-of select="@stopped"/>
                                      </td>
                                      <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                                        <xsl:value-of select="@duration"/>
                                      </td>
                                      <td width="10%" style="white-space: nowrap; padding: 10px 5px 2px 5px">
                                        <xsl:value-of select="@records"/>
                                      </td>
                                    </tr>
                                  </xsl:for-each>
                                </table>

                              </div>
                            </xsl:if>
                          </td>
                        </tr>
                        <tr>
                          <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;">Rates</td>
                          <td width="90%" style="border:none; padding: 10px 10px 10px 6px;  text-align: left;" >

                            <xsl:if test="@language_rate_checked = 'true'">
                              <div >
                                <fieldset>
                                  <legend accesskey="I">
                                    <span style="color:#084B8A;">Language Rate</span>
                                  </legend>
                                  <table cellpadding="0" cellspacing="0" style="border:none;" border="0" width="100%">
                                    <tr>
                                      <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Description:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@language_rate_description"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Name:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@language_rate_name"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Total:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@language_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                                        <xsl:value-of select="@currency"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                                    </tr>
                                  </table>
                                </fieldset>
                              </div>
                              <br/>
                            </xsl:if>

                            <xsl:if test="@hourly_rate_checked = 'true'">
                              <div>
                                <fieldset>
                                  <legend accesskey="I">
                                    <span style="color:#084B8A;">Hourly Rate</span>
                                  </legend>
                                  <table cellpadding="0" cellspacing="0" style="border:none;" border="0" width="100%">

                                    <tr>
                                      <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Description:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@hourly_rate_description"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Quantity:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@hourly_rate_quantity"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Rate:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@hourly_rate_rate"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Total:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@hourly_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                                        <xsl:value-of select="@currency"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                                    </tr>
                                  </table>
                                </fieldset>
                              </div>
                              <br/>
                            </xsl:if>

                            <xsl:if test="@custom_rate_checked = 'true'">
                              <div >
                                <fieldset>
                                  <legend accesskey="I">
                                    <span style="color:#084B8A;">Custom Rate</span>
                                  </legend>
                                  <table cellpadding="0" cellspacing="0" style="border:none;" border="0" width="100%">

                                    <tr>
                                      <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Description:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@custom_rate_description"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="5%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: right;color:#585858;" >Total:</td>
                                      <td width="95%" style="border:none;white-space: nowrap;padding: 5px 5px 2px 2px; text-align: left;" >
                                        <xsl:value-of select="@custom_rate_total"/>&#160;<span style="color: darkgray; font-style:italic;"></span>
                                        <xsl:value-of select="@currency"/>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                                    </tr>
                                  </table>
                                </fieldset>
                              </div>
                              <br/>
                            </xsl:if>
                          </td>
                        </tr>
                        <tr>
                          <td colspan="2" style="border:none; text-align: left; font-size:12px; padding: 0px 0px 0px 10px;">&#160;</td>
                        </tr>
                      </table>
                    </fieldset>
                  </div>
                  <br/>
                </xsl:for-each>

              </xsl:for-each>
            </xsl:if>
            <br/>
          </xsl:for-each>

        </td>
      </tr>
    </table>        
  </xsl:template>


</xsl:stylesheet>
