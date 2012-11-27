<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output method="html" />

  <xsl:param name="p0"/>
  <xsl:param name="p1"/>
  <xsl:param name="p2"/>
  <xsl:param name="p3"/>

  <xsl:key name="projects" match="r" use="@c0" />

  <xsl:template match="*">

    <div id="reportBody">
      <table class="pm-tablebase" cellspacing="0" cellpadding="6" width="100%">
        <thead>
          <tr>
            <td style="width: 50px;padding-left:15px;">
              <xsl:value-of select="$p0"/>
            </td>
            <td>
              <xsl:value-of select="$p1"/>
            </td>
            <td style="text-align:right;width: 80px;padding-right:15px;white-space:nowrap;">
              <xsl:value-of select="$p2"/>
            </td>
          </tr>
        </thead>

        <tbody>
          <xsl:apply-templates select="r[generate-id(.)=generate-id(key('projects', @c0))]"/>
        </tbody>
      </table>
    </div>
  </xsl:template>

  <xsl:template match="r">

    <tr>
      <td colspan='3' class='tintMedium' style='padding-left:15px; padding-right:15px;'>
        <div style='width:920px;overflow:hidden;'>
          <a href='{$p3}/products/projects/projects.aspx?prjID={@c0}' class='smallLinkHeader'>
            <xsl:value-of select="@c1"/>
          </a>
        </div>
      </td>
    </tr>

    <xsl:apply-templates mode="next" select="key('projects', @c0)"/>

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
        <div style='width:760px;overflow:hidden;'>
          <a class='smallLinkHeader' href='{$p3}/products/projects/milestones.aspx?prjID={@c0}&amp;ID={@c2}'>
            <xsl:value-of select="@c3"/>
          </a>
        </div>
      </td>
      <td class='borderBase headerBaseMedium' style='text-align: right;padding-right:15px;'>
        <div class='pm-redText' style='width:80px;overflow:hidden;'>
          <xsl:value-of select="@c4"/>
        </div>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>