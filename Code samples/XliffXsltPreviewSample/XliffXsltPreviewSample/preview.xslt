<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:x="urn:oasis:names:tc:xliff:document:1.2" exclude-result-prefixes="x">
  <xsl:output method="html" encoding="utf-8"/>
  <xsl:template match="/">
    <html>
      <body>
        <h1>XSLT preview</h1>
        <table>
          <xsl:for-each select="//x:trans-unit">
            <tr>
              <td class="src"><xsl:value-of select="x:source"/></td>
              <td class="tgt"><xsl:value-of select="x:target"/></td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
