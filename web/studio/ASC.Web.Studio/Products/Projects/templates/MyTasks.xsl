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
  <xsl:param name="p7"/>
  <xsl:param name="p8"/>
  <xsl:param name="p9"/>

  <xsl:key name="g1" match="r" use="@c8"/>
  <xsl:key name="g2" match="r" use="concat(@c8, '|', @c0)"/>
  <xsl:key name="g3" match="r" use="concat(@c8, '|', @c0, '|', @c2)"/>

  <xsl:template match="*">

    <div id="myTasksBody">


      <xsl:for-each select="//r[generate-id() = generate-id(key('g1', @c8))]">
        <xsl:sort select="@c12" />
        <xsl:variable name="user" select="@c8"/>

        <xsl:if test="$p7 = false">
          <xsl:for-each select="key('g1', $user) [generate-id() = generate-id(key('g2', concat($user, '|', @c0)))]">
            <xsl:sort select="@c1"/>
            <xsl:variable name="proj" select="@c0"/>

            <div>

              <div class="pm_projectName">
                <img align="absmiddle" title="{$p0}" alt="{$p0}" src="{$p5}" />
                <a href="projects.aspx?prjID={@c0}">
                  <xsl:value-of select="@c1"/>
                </a>
              </div>

              <xsl:for-each  select="key('g2', concat($user, '|', $proj)) [generate-id() = generate-id(key('g3', concat($user, '|', @c0, '|', @c2)))]">
                <xsl:variable name="milestone" select="@c2"/>

                <div id="milestoneWithTasksBlock_{@c2}" style="padding-bottom: 0px; padding-left: 30px;" class="pm-milestoneWithTasksBlock">

                  <div style="padding-bottom: 3px; overflow: hidden;" class="clearFix header">
                      <xsl:if test="@c12 != ''">
                        <img align="absmiddle" title="{@c13}" alt="{@c13}" src="{@c12}" />
                      </xsl:if>
                      <xsl:if test="@c2 = '0'">
                        <a class='linkHeader' href='tasks.aspx?prjID={@c0}&amp;action=2&amp;view=all'>
                          <xsl:value-of select="$p4"/>
                        </a>
                      </xsl:if>
                      <xsl:if test="@c2 != '0'">
                        <a class='linkHeader' href='milestones.aspx?prjID={@c0}&amp;ID={@c2}'>
                          <xsl:value-of select="@c3"/>
                        </a>
                      </xsl:if>
                      <xsl:if test="@c14 != ''">
                        <span style="margin-left: 7px;">
                          <span class="{@c14}" style="font-size: 14px;">
                            <xsl:value-of select="@c15"/>
                            <xsl:if test="@c16 != ''">
                              <strong>
                                <xsl:value-of select="@c16"/>
                              </strong>
                            </xsl:if>
                          </span>
                        </span>
                      </xsl:if>
                  </div>

                  <div style="padding-left: 30px;">

                    <table cellspacing="0" cellpadding="5" style="width: 100%;" class="pm-tablebase pm-tasks-block">
                      <thead>
                        <tr>
                          <td class="borderBase">
                          </td>
                          <td class="borderBase">
                            <xsl:value-of select="$p1"/>
                          </td>
                        </tr>
                      </thead>
                      <tbody>

                        <xsl:for-each select="../r[@c8 = $user and @c0 = $proj and @c2 = $milestone]">

                          <tr id="pmtask_{@c6}" onmouseover="ASC.Projects.MyTaskPage.taskItem_mouseOver(this);" onmouseout="ASC.Projects.MyTaskPage.taskItem_mouseOut(this);">
                            <td class="pm-task-checkbox borderBase">
                              <input type="checkbox" title="{$p2}" id="taskStatus_{@c6}" onclick="ASC.Projects.MyTaskPage.changeStatus({@c6});"/>
                              <img style="display: none; margin-left: 5px;" src="{$p6}" id="loaderImg_{@c6}" />
                            </td>
                            <td class="pm-task-title borderBase">
                              <div style="width: 830px; line-height: 18px;">

                                <a href="tasks.aspx?prjID={@c0}&amp;ID={@c6}" style="padding-right: 6px;" title="{@c11}" id='taskTitle_{@c6}'>
                                  <xsl:value-of select="@c7"/>
                                </a>

                                <xsl:if test="@c17 != ''">
                                  <span class="{@c17}" style="white-space: nowrap;">
                                    <xsl:value-of select="@c18"/>
                                  </span>
                                </xsl:if>

                                <xsl:if test="@c20 = 1">
                                  <img id="imgTime_{@c6}" align="absmiddle" class="timeImg" alt="{$p3}" title="{$p3}" src="{@c19}" onclick="ASC.Projects.TimeSpendActionPage.ViewTimeLogPanel({@c0},{@c6})" hastime="{@c20}" />
                                </xsl:if>
                                <xsl:if test="@c20 = 0">
                                  <img id="imgTime_{@c6}" align="absmiddle" class="timeImg" alt="{$p3}" title="{$p3}" src="{@c19}" onclick="ASC.Projects.TimeSpendActionPage.ViewTimeLogPanel({@c0},{@c6})" style="display:none;" hastime="{@c20}" />
                                </xsl:if>

                                <img id="imgTimer_{@c6}" align="absmiddle" class="timeImg" alt="{$p9}" title="{$p9}"  src="{$p8}" onclick="ASC.Projects.TimeSpendActionPage.showTimer('timetracking.aspx?prjID={@c0}&amp;ID={@c6}&amp;action=timer');" style="display:none;"/>

                              </div>
                            </td>
                          </tr>

                        </xsl:for-each>

                      </tbody>
                    </table>

                  </div>

                </div>

              </xsl:for-each>

            </div>

          </xsl:for-each>
        </xsl:if>

        <xsl:if test="$p7">
          <table cellspacing="0" cellpadding="5" style="width: 100%;" class="pm-tablebase pm-tasks-block">
            <tbody>
              <xsl:for-each select="key('g1', $user) [generate-id() = generate-id(key('g2', concat($user, '|', @c0)))]">
                <xsl:sort select="@c1"/>
                <xsl:variable name="proj" select="@c0"/>
                  <xsl:for-each  select="key('g2', concat($user, '|', $proj)) [generate-id() = generate-id(key('g3', concat($user, '|', @c0, '|', @c2)))]">
                    <xsl:variable name="milestone" select="@c2"/>
                      <xsl:for-each select="../r[@c8 = $user and @c0 = $proj and @c2 = $milestone]">
                        <tr id="pmtask_{@c6}" onmouseover="ASC.Projects.MyTaskPage.taskItem_mouseOver(this);" onmouseout="ASC.Projects.MyTaskPage.taskItem_mouseOut(this);">
                          <td class="pm-task-checkbox borderBase">
                            <input type="checkbox" title="{$p2}" id="taskStatus_{@c6}" onclick="ASC.Projects.MyTaskPage.changeStatus({@c6});"/>
                            <img style="display: none; margin-left: 5px;" src="{$p6}" id="loaderImg_{@c6}" />
                          </td>
                          <td class="pm-task-title borderBase">
                            <div style="width: 500px; line-height: 18px;">
                              <a href="tasks.aspx?prjID={@c0}&amp;ID={@c6}" style="padding-right: 6px;" title="{@c11}" id='taskTitle_{@c6}'>
                                <xsl:value-of select="@c7"/>
                              </a>
                              <xsl:if test="@c17 != ''">
                                <span class="{@c17}" style="white-space: nowrap;">
                                  <xsl:value-of select="@c18"/>
                                </span>
                              </xsl:if>
                              <xsl:if test="@c20 = 1">
                                <img id="imgTime_{@c6}" align="absmiddle" class="timeImg" alt="{$p3}" title="{$p3}" src="{@c19}" onclick="ASC.Projects.TimeSpendActionPage.ViewTimeLogPanel({@c0},{@c6})" hastime="{@c20}" />
                              </xsl:if>
                              <xsl:if test="@c20 = 0">
                                <img id="imgTime_{@c6}" align="absmiddle" class="timeImg" alt="{$p3}" title="{$p3}" src="{@c19}" onclick="ASC.Projects.TimeSpendActionPage.ViewTimeLogPanel({@c0},{@c6})" style="display:none;" hastime="{@c20}" />
                              </xsl:if>

                              <img id="imgTimer_{@c6}" align="absmiddle" class="timeImg" alt="{$p9}" title="{$p9}"  src="{$p8}" onclick="ASC.Projects.TimeSpendActionPage.showTimer('timetracking.aspx?prjID={@c0}&amp;ID={@c6}&amp;action=timer');" style="display:none;"/>

                            </div>
                          </td>
                          <td style="text-align: right; padding-top: 9px;" class="pm_taskBlockDescribe borderBase">
                            <div style="width: 400px; overflow-x: hidden;">
                              <xsl:if test="@c1 != ''">
                                <span class="projectIcon_grey">
                                  <xsl:value-of select="@c1"/>  
                                </span>
                              </xsl:if>
                              <xsl:if test="@c3 != ''">
                                <span class="milestoneIcon_grey">
                                  <xsl:value-of select="@c3"/>
                                </span>
                              </xsl:if>
                            </div>
                          </td>
                        </tr>
                      </xsl:for-each>
                  </xsl:for-each>
              </xsl:for-each>
            </tbody>
          </table>
        </xsl:if>

      </xsl:for-each>


    </div>

  </xsl:template>

</xsl:stylesheet>