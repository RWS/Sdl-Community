<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:template match="/">
    <html>
      <style>
        .All {
        font-family: Segoe UI;
        font-size: 6pt;
        }
        .Table {
        border-collapse: collapse;
        border: 1px solid #8c8c8c;
        background-color: #ddd;
        margin-bottom: 10px;
        }
        .Header {
        color: #4086aa;
        }
        th, td {
        border-collapse: collapse;
        border: 1px solid #8c8c8c;
        background-color: #ddd;
        }
        .Row {
        text-align: center
        }
      </style>
      <body class="All">
        <xsl:for-each select="//Result">
          <table class="Table">
            <tr class="Row">
              <th class="Header">File Name</th>
              <td>
                <xsl:value-of select="@Name" />
              </td>
            </tr>
            <tr class="Row">
              <th class="Header">Result</th>
              <td>
                <xsl:if test="@Failed = 'true'">
                  <p style="color:red">Failed</p>
                  <p style="color:red">
                    <xsl:value-of select="current()" />
                  </p>
                </xsl:if>
                <xsl:if test="@Failed = 'false'">
                  <p style="color:green">Passed</p>
                </xsl:if>
              </td>
            </tr>
          </table>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>