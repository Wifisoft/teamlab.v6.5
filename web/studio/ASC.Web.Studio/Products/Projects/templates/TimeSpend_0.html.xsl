<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output method="html" />

  <xsl:param name="p0"/>
  <xsl:param name="p1"/>
  <xsl:param name="p3"/>
  <xsl:param name="p4"/>
  <xsl:param name="p5"/>

  <xsl:decimal-format NaN = "0.00" />
  
  <xsl:key name="users" match="r" use="@c0" />

  <xsl:template match="*">

    <div id="reportBody">
      <table class="sortable" cellspacing="0" cellpadding="10" id="result">
        <thead>
          <tr>
            <td style="text-align:left;">
              <span class="report-tableColumnHeader" unselectable="on">
                <xsl:attribute name="title">
                  <xsl:value-of select="$p3"/>
                </xsl:attribute>
                <xsl:value-of select="$p0"/>
              </span>
              <span id="sorttable_sortfwdind">&#x20;▴</span>
            </td>
            <td width="150px" style="text-align: center;white-space: nowrap;">
              <span class="report-tableColumnHeader" unselectable="on">
                <xsl:attribute name="title">
                  <xsl:value-of select="$p3"/>
                </xsl:attribute>
                <xsl:value-of select="$p1"/>
              </span>
            </td>
          </tr>
        </thead>

        <tbody>
          <xsl:apply-templates select="r[generate-id(.)=generate-id(key('users', @c0))]"/>
        </tbody>
      </table>
    </div>
  </xsl:template>

  <xsl:template match="r">
    <tr class="tintLight">
      <td class='borderBase'>
        <div style='width:800px;overflow:hidden;'>
          <a class="linkAction" href="{$p4}{@c6}">
            <xsl:value-of select="@c5" />
          </a>
        </div>
      </td>
      <td class="borderBase" style="text-align: center;">
        <div style='width:110px;overflow:hidden;'>
          <xsl:value-of select="@c4"/>
        </div>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>