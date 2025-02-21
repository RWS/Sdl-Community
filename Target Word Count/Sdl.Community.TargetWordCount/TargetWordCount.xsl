<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:template match="/">
    <html>
      <style>
        .Caption {
        background-color: #cae8cb
        }
        .Table {
        border-collapse: collapse,
        border: solid 1px #DDEEEE;
        }
        .Cell {
        text-align: right;
        padding: 8px;
        }
        .Header {
        background-color: #4CAF50;
        color: white;
        padding: 8px;
        }
        .Even {
        background-color: #CCCCCC;
        }

      </style>
      <body>
        <table class="Table">
          <caption class="Caption">
            Total
          </caption>
          <tr>
            <th class="Header">
              Type
            </th>
            <th class="Header">
              Segments
            </th>
            <xsl:if test="//GrandTotal/Total/@TotalCharacters">
              <th class="Header">
                Total Characters
              </th>
            </xsl:if>
            <xsl:if test="//GrandTotal/Total/@CharactersPerLine">
              <th class="Header">
                Characters Per Line
              </th>
            </xsl:if>
            <th class="Header">
              <xsl:value-of select="//GrandTotal/@CountType" />
            </th>
            <th class="Header">
              Rate
            </th>
            <th class="Header">
              Amount
            </th>
          </tr>
          <xsl:if test="//GrandTotal/Locked">
          <tr class="Even">
            <td style="Cell">
              Locked
            </td>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Locked/@Segments" />
            </td>
            <xsl:if test="//GrandTotal/Locked/@TotalCharacters">
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Locked/@TotalCharacters" />
              </td>
            </xsl:if>
            <xsl:if test="//GrandTotal/Locked/@CharactersPerLine">
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Locked/@CharactersPerLine" />
              </td>
            </xsl:if>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Locked/@Count" />
            </td>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Locked/@Rate" />
            </td>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Locked/@Amount" />
            </td>
          </tr>
          </xsl:if>
          <xsl:if test="//GrandTotal/PerfectMatch/@Segments != ''">
            <tr>
              <td style="Cell">
                Perfect Match
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/PerfectMatch/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/PerfectMatch/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/PerfectMatch/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/PerfectMatch/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                Context Match
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/ContextMatch/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/ContextMatch/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/ContextMatch/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/ContextMatch/@Amount" />
              </td>
            </tr>
            <tr>
              <td>
                Repetitions
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Repetitions/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Repetitions/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Repetitions/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Repetitions/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                Cross-file Repetitions
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/CrossFileRepetitions/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/CrossFileRepetitions/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/CrossFileRepetitions/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//CrossFileRepetitions/@Amount" />
              </td>
            </tr>
            <tr>
              <td style="Cell">
                100%
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/OneHundredPercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/OneHundredPercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/OneHundredPercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/OneHundredPercent/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                95% - 99%
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/NinetyFivePercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/NinetyFivePercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/NinetyFivePercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/NinetyFivePercent/@Amount" />
              </td>
            </tr>
            <tr>
              <td style="Cell">
                85% - 94%
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/EightyFivePercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/EightyFivePercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/EightyFivePercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/EightyFivePercent/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                75% - 84%
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/SeventyFivePercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/SeventyFivePercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/SeventyFivePercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/SeventyFivePercent/@Amount" />
              </td>
            </tr>
            <tr>
              <td style="Cell">
                50% - 74%
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/FiftyPercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/FiftyPercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/FiftyPercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/FiftyPercent/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                New
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/New/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/New/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/New/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/New/@Amount" />
              </td>
            </tr>
          </xsl:if>
          <tr>
            <td>
              Total
            </td>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Total/@Segments" />
            </td>
            <xsl:if test="//GrandTotal/Total/@TotalCharacters">
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Total/@TotalCharacters" />
              </td>
            </xsl:if>
            <xsl:if test="//GrandTotal/Total/@CharactersPerLine">
              <td style="Cell">
                <xsl:value-of select="//GrandTotal/Total/@CharactersPerLine" />
              </td>
            </xsl:if>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Total/@Count" />
            </td>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Total/@Rate" />
            </td>
            <td style="Cell">
              <xsl:value-of select="//GrandTotal/Total/@Amount" />
            </td>
          </tr>
        </table>
        <xsl:for-each select="//File">
          <table class="Table">
            <caption class="Caption">
              <xsl:value-of select="@Name" />
            </caption>
            <tr>
              <th class="Header">
                Type
              </th>
              <th class="Header">
                Segments
              </th>
              <xsl:if test="//GrandTotal/Total/@TotalCharacters">
                <th class="Header">
                  Total Characters
                </th>
              </xsl:if>
              <xsl:if test="//GrandTotal/Total/@CharactersPerLine">
                <th class="Header">
                  Characters Per Line
                </th>
              </xsl:if>
              <th class="Header">
                <xsl:value-of select="@CountType" />
              </th>
              <th class="Header">
                Rate
              </th>
              <th class="Header">
                Amount
              </th>
            </tr>
            <xsl:if test="Locked">
              <tr class="Even">
                <td style="Cell">
                  Locked
                </td>
                <td style="Cell">
                  <xsl:value-of select="Locked/@Segments" />
                </td>
                <xsl:if test="Locked/@TotalCharacters">
                  <td style="Cell">
                    <xsl:value-of select="Locked/@TotalCharacters" />
                  </td>
                </xsl:if>
                <xsl:if test="Locked/@CharactersPerLine">
                  <td style="Cell">
                    <xsl:value-of select="Locked/@CharactersPerLine" />
                  </td>
                </xsl:if>
                <td style="Cell">
                  <xsl:value-of select="Locked/@Count" />
                </td>
                <td style="Cell">
                  <xsl:value-of select="Locked/@Rate" />
                </td>
                <td style="Cell">
                  <xsl:value-of select="Locked/@Amount" />
                </td>
              </tr>
            </xsl:if>
            <xsl:if test="//GrandTotal/PerfectMatch/@Segments != ''">
            <tr>
              <td style="Cell">
                Perfect Match
              </td>
              <td style="Cell">
                <xsl:value-of select="PerfectMatch/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="PerfectMatch/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="PerfectMatch/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="PerfectMatch/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                Context Match
              </td>
              <td style="Cell">
                <xsl:value-of select="ContextMatch/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="ContextMatch/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="ContextMatch/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="ContextMatch/@Amount" />
              </td>
            </tr>
            <tr>
              <td>
                Repetitions
              </td>
              <td style="Cell">
                <xsl:value-of select="Repetitions/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="Repetitions/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="Repetitions/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="Repetitions/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                Cross-file Repetitions
              </td>
              <td style="Cell">
                <xsl:value-of select="CrossFileRepetitions/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="CrossFileRepetitions/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="CrossFileRepetitions/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="CrossFileRepetitions/@Amount" />
              </td>
            </tr>
            <tr>
              <td style="Cell">
                100%
              </td>
              <td style="Cell">
                <xsl:value-of select="OneHundredPercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="OneHundredPercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="OneHundredPercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="OneHundredPercent/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                95% - 99%
              </td>
              <td style="Cell">
                <xsl:value-of select="NinetyFivePercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="NinetyFivePercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="NinetyFivePercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="NinetyFivePercent/@Amount" />
              </td>
            </tr>
            <tr>
              <td style="Cell">
                85% - 94%
              </td>
              <td style="Cell">
                <xsl:value-of select="EightyFivePercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="EightyFivePercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="EightyFivePercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="EightyFivePercent/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                75% - 84%
              </td>
              <td style="Cell">
                <xsl:value-of select="SeventyFivePercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="SeventyFivePercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="SeventyFivePercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="SeventyFivePercent/@Amount" />
              </td>
            </tr>
            <tr>
              <td style="Cell">
                50% - 74%
              </td>
              <td style="Cell">
                <xsl:value-of select="FiftyPercent/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="FiftyPercent/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="FiftyPercent/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="FiftyPercent/@Amount" />
              </td>
            </tr>
            <tr class="Even">
              <td style="Cell">
                New
              </td>
              <td style="Cell">
                <xsl:value-of select="New/@Segments" />
              </td>
              <td style="Cell">
                <xsl:value-of select="New/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="New/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="New/@Amount" />
              </td>
            </tr>
            </xsl:if>
            <tr>
              <td>
                Total
              </td>
              <td style="Cell">
                <xsl:value-of select="Total/@Segments" />
              </td>
              <xsl:if test="Total/@TotalCharacters">
                <td style="Cell">
                  <xsl:value-of select="Total/@TotalCharacters" />
                </td>
              </xsl:if>
              <xsl:if test="Total/@CharactersPerLine">
                <td style="Cell">
                  <xsl:value-of select="Total/@CharactersPerLine" />
                </td>
              </xsl:if>
              <td style="Cell">
                <xsl:value-of select="Total/@Count" />
              </td>
              <td style="Cell">
                <xsl:value-of select="Total/@Rate" />
              </td>
              <td style="Cell">
                <xsl:value-of select="Total/@Amount" />
              </td>
            </tr>
          </table>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>