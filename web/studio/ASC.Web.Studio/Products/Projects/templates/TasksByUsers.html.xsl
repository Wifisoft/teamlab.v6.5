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

  <xsl:key name="g1" match="r" use="@c8"/>
  <xsl:key name="g2" match="r" use="concat(@c8, '|', @c0)"/>
  <xsl:key name="g3" match="r" use="concat(@c8, '|', @c0, '|', @c2)"/>

  <xsl:template match="*">

    <div id="reportBody">
      <table class="pm-tablebase" cellspacing="0" cellpadding="6">
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
          </tr>
        </thead>

        <tbody>

          <xsl:for-each select="//r[generate-id() = generate-id(key('g1', @c8))]">
            <xsl:sort select="@c12" />
            <xsl:variable name="user" select="@c8"/>
            <tr>
              <td colspan='4' class='tintMedium pm-report-userName'>
                <div style='width:920px;overflow:hidden;'>
                  <a href="{$p8}{@c13}" class='smallLinkHeader'>
                    <xsl:value-of select="@c12"/>
                  </a>
                </div>
              </td>
            </tr>

            <xsl:for-each select="key('g1', $user) [generate-id() = generate-id(key('g2', concat($user, '|', @c0)))]">
              <xsl:sort select="@c1"/>
              <xsl:variable name="proj" select="@c0"/>
              <tr>
                <td colspan='4' class='borderBase pm-report-userName'>
                  <div style='padding-top:19px;width:920px;overflow:hidden;'>
                    <a href='{$p8}{$p9}/products/projects/tasks.aspx?prjID={@c0}&amp;action=2' class='bigLinkHeader'>
                      <xsl:value-of select="@c1"/>
                    </a>
                  </div>
                </td>
              </tr>

              <xsl:for-each  select="key('g2', concat($user, '|', $proj)) [generate-id() = generate-id(key('g3', concat($user, '|', @c0, '|', @c2)))]">
                <xsl:variable name="milestone" select="@c2"/>
                <tr>
                  <td class='borderBase'></td>
                  <td class='borderBase headerBaseMedium' colspan='2'>
                    <div style='width:750px;overflow:hidden;'>
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
                    <div style='width:80px;overflow:hidden;white-space:nowrap;'>
                      <b class='{@c15}'>
                        <xsl:value-of select="@c4"/>
                      </b>
                    </div>
                  </td>
                </tr>

                <xsl:for-each select="../r[@c8 = $user and @c0 = $proj and @c2 = $milestone]">
                  <tr>
                    <td class="borderBase" />
                    <td class="borderBase" />
                    <td class="borderBase">
                      <div style='width:690px;overflow:hidden;'>
                        <xsl:if test="@c16 = '2'">
                          <a href="{$p8}{$p9}/products/projects/tasks.aspx?prjID={@c0}&amp;ID={@c6}" class="report-closedTask">
                            <xsl:value-of select="@c7"/>
                          </a>
                        </xsl:if>
                        <xsl:if test="@c16 != '2'">
                          <a href="{$p8}{$p9}/products/projects/tasks.aspx?prjID={@c0}&amp;ID={@c6}">
                            <xsl:value-of select="@c7"/>
                          </a>
                        </xsl:if>
                      </div>
                    </td>
                    <td class="borderBase">
                      <div style='width:80px;overflow:hidden;white-space:nowrap;' class='{@c17}'>
                        <xsl:value-of select="@c10"/>
                      </div>
                    </td>
                  </tr>
                </xsl:for-each>

              </xsl:for-each>

              <tr>
                <td colspan='4'>
                  <br/>
                </td>
              </tr>
            </xsl:for-each>

          </xsl:for-each>

        </tbody>
      </table>
    </div>
  </xsl:template>

</xsl:stylesheet>