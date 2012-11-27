<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output method="html" />

  <xsl:param name="p1"/>
  <xsl:param name="p2"/>
  <xsl:param name="p4"/>
  <xsl:param name="p5"/>

  <xsl:decimal-format NaN = "0.00" />
  
  <xsl:key name="users" match="r" use="@c0" />

  <xsl:template match="*">

    <div id="reportBody">
      <table class="pm-tablebase" cellspacing="0" cellpadding="6" width="100%">
        <thead>
          <tr>
            <td style="width: 20px">
            </td>
            <td>
              <xsl:value-of select="$p2"/>
            </td>
            <td width="150px" style="text-align: center;">
              <xsl:value-of select="$p1"/>
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

    <tr>
      <td colspan='3' class='tintMedium' style='padding-left:15px; padding-right:15px;'>
        <div style='width:920px;overflow:hidden;'>
          <a href='{$p4}{@c6}' class='smallLinkHeader'>
            <xsl:value-of select="@c5"/>
          </a>
        </div>
      </td>
    </tr>

    <xsl:apply-templates mode="next" select="key('users', @c0)">
      <xsl:sort select="@c3" data-type="text"/>
    </xsl:apply-templates>

    <tr>
      <td colspan='3'>
        <br/>
      </td>
    </tr>

  </xsl:template>

  <xsl:template match="r" mode="next">
    <tr>
      <td class='borderBase'>
      </td>
      <td class='borderBase'>
        <div style='width:740px;overflow:hidden;'>
          <a href='{$p4}{$p5}/products/projects/tasks.aspx?prjID={@c1}&amp;ID={@c2}'>
            <xsl:value-of select="@c3"/>
          </a>
        </div>
      </td>
      <td class="borderBase" style='text-align: center;'>
        <div style='width:150px;overflow:hidden;'>
          <xsl:value-of select="@c4"/>
        </div>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>