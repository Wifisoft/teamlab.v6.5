<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output method="html" />

  <xsl:param name="p0"/>
  <xsl:param name="p1"/>
  <xsl:param name="p2"/>
  <xsl:param name="p3"/>
  <xsl:param name="p4"/>
  <xsl:param name="p5"/>
  <xsl:param name="p6"/>
  <xsl:param name="p8"/>
  <xsl:param name="p9"/>

  <xsl:key name="g1" match="r" use="@c0"/>
  <xsl:key name="g2" match="r" use="concat(@c0, '|', @c2)"/>

  <xsl:template match="*">

    <div id="reportBody">
      <table class="pm-tablebase" cellspacing="0" cellpadding="6" width="100%">
        <thead>
          <tr>
            <td style="width: 50px;padding-left:15px">
              <xsl:value-of select="$p0"/>
            </td>
            <td style="width: 50px">
              <xsl:value-of select="$p1"/>
            </td>
            <td>
              <xsl:value-of select="$p2"/>
            </td>
            <td style="width: 80px;white-space:nowrap;">
              <xsl:value-of select="$p6"/>
            </td>
            <td style="width: 150px">
              <xsl:value-of select="$p3"/>
            </td>
          </tr>
        </thead>

        <tbody>
          <xsl:for-each select="//r[generate-id() = generate-id(key('g1', @c0))]" >
            <xsl:variable name="proj" select="@c0"/>
            <tr>
              <td colspan='5' class='tintMedium  pm-report-userName'>
                <div style='width:920px;overflow:hidden;'>
                  <a href='{$p8}{$p9}/products/projects/tasks.aspx?prjID={@c0}&amp;action=2' class='smallLinkHeader'>
                    <xsl:value-of select="@c1"/>
                  </a>
                </div>
              </td>
            </tr>

            <xsl:for-each select="key('g1', $proj) [generate-id() = generate-id(key('g2', concat($proj, '|', @c2)))]">
              <xsl:variable name="milestone" select="@c2"/>
              <tr>
                <td class='borderBase'></td>
                <td class='borderBase headerBaseMedium' colspan='2'>
                  <div style='width:590px;overflow:hidden;'>
                    <xsl:if test="@c2 = '0'">
                      <a class='smallLinkHeader' href='{$p8}{$p9}/products/projects/tasks.aspx?prjID={@c0}&amp;action=2&amp;view=all'>
                        <xsl:value-of select="$p5"/>
                      </a>
                    </xsl:if>
                    <xsl:if test="@c2 != '0'">
                      <a class='smallLinkHeader' href='{$p8}{$p9}/products/projects/milestones.aspx?prjID={@c0}&amp;ID={@c2}'>
                        <xsl:value-of select="@c3"/>
                      </a>
                    </xsl:if>
                  </div>
                </td>
                <td class='borderBase'>
                  <div style='width:80px;overflow:hidden;'>
                    <b class='{@c15}'>
                      <xsl:value-of select="@c4"/>
                    </b>
                  </div>
                </td>
                <td class='borderBase'></td>
              </tr>

              <xsl:for-each select="../r[@c0 = $proj and @c2 = $milestone]">
                <tr>
                  <td class="borderBase">
                  </td>
                  <td class="borderBase">
                  </td>
                  <td class="borderBase">
                    <div style='width:520px;overflow:hidden;'>
                      <a href="{$p8}{$p9}/products/projects/tasks.aspx?prjID={@c0}&amp;ID={@c6}">
                        <xsl:value-of select="@c7"/>
                      </a>
                    </div>
                  </td>
                  <td class="borderBase">
                    <div class="{@c17}" style='width:80px;overflow:hidden;'>
                      <xsl:value-of select="@c10"/>
                    </div>
                  </td>
                  <td class="borderBase">
                    <div style='width:150px;overflow:hidden;'>
                      <xsl:if test="@c13 = ''">
                        <xsl:value-of select="@c12"/>
                      </xsl:if>
                      <xsl:if test="@c13 != ''">
                        <a href="{$p8}{@c13}" class="linkAction">
                          <xsl:value-of select="@c12"/>
                        </a>
                      </xsl:if>
                    </div>
                  </td>
                </tr>
              </xsl:for-each>

            </xsl:for-each>

            <tr>
              <td colspan='5'>
                <br/>
              </td>
            </tr>
          </xsl:for-each>
        </tbody>
      </table>
    </div>
  </xsl:template>

</xsl:stylesheet>