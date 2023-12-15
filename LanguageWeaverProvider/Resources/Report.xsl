<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:XmlReporting="urn:XmlReporting">
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
                    width: 30%;
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
                    width: 100%
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
                    margin: 0 0 30 0;
                    }

                    h1.first {
                    font-family: Segoe UI;
                    font-weight: normal;
                    font-size: 22px;
                    color: #636463;
                    padding: 20px 20px 20px 0px;
                    margin: 0px 20pc 2px 10;
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
                <h1>Segment quality estimations</h1>
                <h2 class="first">Summary</h2>
                <table class="InfoList">
                    <xsl:for-each select="//Summary/*">
                        <tr>
                            <td class="InfoItem"><xsl:value-of select="local-name()" /></td>
                            <td class="InfoData"><xsl:value-of select="." /></td>
                        </tr>
                    </xsl:for-each>
                </table>

                <xsl:for-each select="//Data/File">
                    <h2>
                        <xsl:value-of select="@Name" />
                    </h2>
                    <table class="ReportTable">
                        <tr>
                            <th class="TypeHead">Estimation</th>
                            <th class="TypeHead">Number of segments</th>
                            <th class="TypeHead">Number of words</th>
                        </tr>
                        <xsl:for-each select="QeValues">
                            <tr>
                                <td class="InfoData">
                                    <xsl:value-of select="." />
                                </td>
                                <td class="InfoData">
                                    <xsl:value-of select="@SegmentsTotal" />
                                </td>
                                <td class="InfoData">
                                    <xsl:value-of select="@WordsTotal" />
                                </td>
                            </tr>
                        </xsl:for-each>
                    </table>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>
