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
        <xsl:for-each select="//File">
          <table class="Table">
            <tr class="Row">
              <th class="Header">File Name</th>
              <td>
                <xsl:value-of select="@Name" />
              </td>
            </tr>
            <tr class="Row">
              <th class="Header">LogFile</th>
              <td>
                <a href="{@FullPath}">
                  View Log File
                </a>
              </td>
            </tr>
            <tr class="Row">
              <th class="Header">Number of Locked Segments</th>
              <td>
                <xsl:value-of select="LockCount" />
              </td>
            </tr>
            <tr class="Row">
              <th class="Header">Number of Removed Tags</th>
              <td>
                <xsl:value-of select="TagRemoveCount" />
              </td>
            </tr>
            <tr class="Row">
              <th class="Header">Number of Text Changes</th>
              <td>
                <xsl:value-of select="ConversionCount" />
              </td>
            </tr>
          </table>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>