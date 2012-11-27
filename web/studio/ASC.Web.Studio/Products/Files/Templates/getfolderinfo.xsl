<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />

  <register type="ASC.Web.Files.Resources.FilesCommonResource,ASC.Web.Files" alias="fres" />

  <xsl:template match="folder|folder_info">
    <xsl:if test="not(provider_name)">
      <resource name="fres.TitleFiles" />&#160;<span id="content_info_count_files"><xsl:value-of select="total_files" /></span>
      <span> | </span>
      <resource name="fres.TitleSubfolders" />&#160;<span id="content_info_count_folders"><xsl:value-of select="total_sub_folder" /></span>
      <input type="hidden" id="access_current_folder">
        <xsl:attribute name="value"><xsl:value-of select="access" /></xsl:attribute>
      </input>
    </xsl:if>
    <input type="hidden">
      <xsl:attribute name="id">folder_shareable</xsl:attribute>
      <xsl:attribute name="value"><xsl:value-of select="shareable" /></xsl:attribute>
    </input>
    <input id="currentFolderTitle" type="hidden">
      <xsl:attribute name="value"><xsl:value-of select="title" /></xsl:attribute>
    </input>
    <input id="currentProviderType" type="hidden">
      <xsl:attribute name="value"><xsl:value-of select="provider_name" /></xsl:attribute>
    </input>
  </xsl:template>

</xsl:stylesheet>