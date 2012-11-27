<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />

  <xsl:template match="folderList">
    <xsl:for-each select="entry">
      <li>
        <xsl:attribute name="class">
          tree_node jstree-closed
          <xsl:if test="access = 'Read'">
            access_read
          </xsl:if>
          <xsl:if test="total_sub_folder = 0 and not(provider_id)">
            jstree-empty
          </xsl:if>
          <xsl:if test="provider_name">
            thirdPartyEntry
          </xsl:if>
        </xsl:attribute>
        <xsl:attribute name="data-id"><xsl:value-of select="id" /></xsl:attribute>
        <span class="jstree-icon expander" > </span>
        <a>
          <xsl:attribute name="href">#<xsl:value-of select="id" /></xsl:attribute>
          <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
          <span>
            <xsl:attribute name="class">
              jstree-icon
              <xsl:if test="provider_name">
                <xsl:value-of select="provider_name" />
              </xsl:if>
            </xsl:attribute>
          </span>
          <xsl:value-of select="title" />
        </a>
      </li>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>