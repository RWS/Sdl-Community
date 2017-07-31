<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" indent="yes" encoding="utf-8"/>
  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
        <style type="text/css">
          body { font-family: arial, helvetica; font-size: 10pt }
          .header { font-size: 14pt; font-weight: bold; color: navy}
          .noTerms, occurrences, length { font-weight: normal }
          .label { font-weight: bold }
          .infoTag { color: red; }
        </style>
      </head>
      <body bgcolor="white">
        <h1>Task name: Project Terms</h1>
        <hr/>
        <xsl:apply-templates select="/report"/>
        <p/>
      </body>
    </html>
  </xsl:template>
  <!-- ***********************************************************************-->
  <xsl:template match="projectTerms">
    <div>
      <span class="infoTag">
        <xsl:value-of select="infoTag"/>
      </span>
      <p/>
      <h2 class="header">
        <xsl:value-of select="header"/>
      </h2>
      <p/>
      <span class="label">No. of terms: </span>
      <span class="noTerms">
        <xsl:value-of select="noTerms"/>
      </span>
      <p/>
      <span class="label">Occurrences: </span>
      <span class="occurrences">
        <xsl:value-of select="occurrences"/>
      </span>
      <p/>
      <span class="label">Length: </span>
      <span class="length">
        <xsl:value-of select="length"/>
      </span>
      <p/>
    </div>
  </xsl:template>
  <!-- ***********************************************************************-->
</xsl:stylesheet>
