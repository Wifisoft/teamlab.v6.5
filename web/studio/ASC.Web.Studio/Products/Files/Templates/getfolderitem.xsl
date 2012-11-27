<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />
  <register type="ASC.Web.Files.Resources.FilesCommonResource,ASC.Web.Files" alias="fres" />

  <xsl:template match="folder">
    <li name="addRow">
      <xsl:attribute name="class">
        clearFix fileRow folderRow newFolder
        <xsl:if test="provider_name">
          thirdPartyEntry
        </xsl:if>
        <xsl:if test="error">
          errorEntry
        </xsl:if>
      </xsl:attribute>
      <xsl:attribute name="data-id">folder_<xsl:value-of select="id" /></xsl:attribute>
      <xsl:if test="spare_data">
        <xsl:attribute name="spare_data"><xsl:value-of select="spare_data" /></xsl:attribute>
      </xsl:if>
      <div class="checkbox">
        <input type="checkbox" >
          <xsl:attribute name="title"><resource name="fres.TitleSelectFile" /></xsl:attribute>
        </input>
      </div>
      <div class="thumb-folder">
        <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
        <xsl:if test="provider_name">
          <div>
            <xsl:attribute name="class"><xsl:value-of select="provider_name" /></xsl:attribute>
          </div>
        </xsl:if>
      </div>
      <div class="entry-info">
        <div class="entryTitle">
          <div class="name">
            <div class="fade_title"></div>
            <a class="name" onmousedown="return false;">
              <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
              <xsl:attribute name="href">#<xsl:value-of select="id" /></xsl:attribute>
              <xsl:value-of select="title" />
            </a>
          </div>
          <xsl:if test="isnew = 'true'">
            <div class="is_new">
              <xsl:attribute name="title"><resource name="fres.RemoveIsNew" /></xsl:attribute>
              <resource name="fres.IsNew" />
            </div>
          </xsl:if>
        </div>
        <div class="entryInfo">

          <xsl:choose>
            <xsl:when test="error">
              <xsl:value-of select="error" />
            </xsl:when>
            <xsl:otherwise>
              <span class="create_by">
                <xsl:attribute name="title"><xsl:value-of select="create_by" /></xsl:attribute>
                <xsl:value-of select="create_by" />
              </span>
            </xsl:otherwise>
          </xsl:choose>          
          
          <span class="entryInfoHidden">
            
            <xsl:choose>
              <xsl:when test="error">
                
              </xsl:when>
              <xsl:otherwise>
                <span> | </span>
                <resource name="fres.TitleCreated" />&#160;<span>
                  <xsl:value-of select="create_on" />
                </span>
                <xsl:if test="not(provider_name)">
                  <span> | </span>
                  <resource name="fres.TitleFiles" />&#160;<span class="countFiles">
                    <xsl:value-of select="total_files" />
                  </span>
                  <span> | </span>
                  <resource name="fres.TitleSubfolders" />&#160;<span class="countFolders">
                    <xsl:value-of select="total_sub_folder" />
                  </span>
                </xsl:if>
                <input type="hidden" name="create_by">
                  <xsl:attribute name="value"><xsl:value-of select="create_by" /></xsl:attribute>
                </input>
                <input type="hidden" name="modified_by">
                  <xsl:attribute name="value"><xsl:value-of select="modified_by" /></xsl:attribute>
                </input>
              </xsl:otherwise>
            </xsl:choose>

            <input type="hidden" name="entry_data">
              <xsl:attribute name="value">
                folder = {
                  entryType: "folder",
                  access: <xsl:value-of select="access" />,
                  create_by_id: "<xsl:value-of select="create_by_id" />",
                  create_on: "<xsl:value-of select="create_on" />",
                  id: "<xsl:value-of select="id" />",
                  modified_on: "<xsl:value-of select="modified_on" />",
                  shared: <xsl:value-of select="shared" />,
                  title: "<xsl:value-of select="title" />",
                  isnew: <xsl:value-of select="isnew" />,
                  shareable: <xsl:value-of select="shareable" />,
                  provider_name: "<xsl:value-of select="provider_name" />",
                  provider_username: "<xsl:value-of select="provider_username" />",
                  provider_id: "<xsl:value-of select="provider_id" />",
                  error: "<xsl:value-of select="error" />"
                  <!--create_by: "<xsl:value-of select="create_by" />",    encode-->
                  <!--modified_by: "<xsl:value-of select="modified_by" />",    encode-->
                  <!--total_files: <xsl:value-of select="total_files" />,    dynamic-->
                  <!--total_sub_folder: <xsl:value-of select="total_sub_folder" />,    dynamic-->
                }
              </xsl:attribute>
            </input>
          </span>
        </div>
      </div>
      <div class="rowActions">
        <xsl:attribute name="title"><resource name="fres.TitleShowFolderActions" /></xsl:attribute>
      </div>
      <div>
        <xsl:attribute name="class">share_action
          <xsl:if test="shared = 'true'">
            shareBlue
          </xsl:if>
        </xsl:attribute>
        <xsl:attribute name="title"><resource name="fres.TitleShareFile" /></xsl:attribute>
      </div>
    </li>
  </xsl:template>

  <xsl:template match="file">
    <li style="display:none;" name="addRow">
      <xsl:attribute name="class">
        clearFix fileRow newFile
        <xsl:if test="contains(file_status, 'IsEditing')">
          onEdit
        </xsl:if>
        <xsl:if test="provider_name">
          thirdPartyEntry
        </xsl:if>
      </xsl:attribute>
      <xsl:attribute name="data-id">file_<xsl:value-of select="id" /></xsl:attribute>
      <xsl:if test="spare_data">
        <xsl:attribute name="spare_data"><xsl:value-of select="spare_data" /></xsl:attribute>
      </xsl:if>
      <div class="checkbox">
        <input type="checkbox" >
          <xsl:attribute name="title"><resource name="fres.TitleSelectFile" /></xsl:attribute>
        </input>
      </div>
      <div class="thumb-file">
        <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
      </div>
      <div class="entry-info">
        <div class="entryTitle">
          <div class="name">
            <div class="fade_title"></div>
            <a class="name" onmousedown="return false;">
              <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
              <xsl:value-of select="title" />
            </a>
          </div>
          <xsl:if test="version > 1">
            <div class="version">
              <xsl:attribute name="title"><resource name="fres.ShowVersions" />(<xsl:value-of select="version" />)</xsl:attribute>
              <resource name="fres.Version" />
              <xsl:value-of select="version" />
            </div>
          </xsl:if>
          <xsl:if test="contains(file_status, 'IsNew')">
            <div class="is_new">
              <xsl:attribute name="title"><resource name="fres.RemoveIsNew" /></xsl:attribute>
              <resource name="fres.IsNew" />
            </div>
          </xsl:if>
        </div>
        <div class="entryInfo">
          <span class="create_by">
            <xsl:attribute name="title"><xsl:value-of select="modified_by" /></xsl:attribute>
            <xsl:value-of select="modified_by" />
          </span>
          <span class="entryInfoHidden">
            <span> | </span>
            <span class="titleCreated">
              <xsl:choose>
                <xsl:when test="version > 1">
                  <resource name="fres.TitleModified" />
                </xsl:when>
                <xsl:otherwise>
                  <resource name="fres.TitleUploaded" />
                </xsl:otherwise>
              </xsl:choose>
            </span>&#160;<span class="modified_date">
              <xsl:choose>
                <xsl:when test="version > 1">
                  <xsl:value-of select="modified_on" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="create_on" />
                </xsl:otherwise>
              </xsl:choose>
            </span>
            <span> | </span>
            <span><xsl:value-of select="content_length" /></span>
            <input type="hidden" name="modified_by">
              <xsl:attribute name="value"><xsl:value-of select="modified_by" /></xsl:attribute>
            </input>
            
            <input type="hidden" name="entry_data">
              <xsl:attribute name="value">
                file = {
                  entryType: "file",
                  access: <xsl:value-of select="access" />,
                  create_by_id: "<xsl:value-of select="create_by_id" />",
                  create_on: "<xsl:value-of select="create_on" />",
                  id: "<xsl:value-of select="id" />",
                  modified_on: "<xsl:value-of select="modified_on" />",
                  shared: <xsl:value-of select="shared" />,
                  title: "<xsl:value-of select="title" />",
                  content_length: "<xsl:value-of select="content_length" />",
                  file_status: "<xsl:value-of select="file_status" />",
                  version: <xsl:value-of select="version" />,
                  provider_name: "<xsl:value-of select="provider_name" />"
                  <!--create_by: "<xsl:value-of select="create_by" />",    encode-->
                  <!--modified_by: "<xsl:value-of select="modified_by" />",    encode-->
                }
              </xsl:attribute>
            </input>
          </span>
        </div>
      </div>
      <div class="rowActions">
        <xsl:attribute name="title"><resource name="fres.TitleShowActions" /></xsl:attribute>
      </div>
      <div>
        <xsl:attribute name="class">share_action
          <xsl:if test="shared = 'true'">
            shareBlue
          </xsl:if>
        </xsl:attribute>
        <xsl:attribute name="title"><resource name="fres.TitleShareFile" /></xsl:attribute>
      </div>
      <div class="fileEdit pencil">
        <xsl:attribute name="title"><resource name="fres.ButtonEdit" /></xsl:attribute>
      </div>
      <div class="fileEditing pencil"></div>
    </li>
  </xsl:template>

</xsl:stylesheet>