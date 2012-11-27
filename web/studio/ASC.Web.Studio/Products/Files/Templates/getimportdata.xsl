<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />
  <register type="ASC.Web.Files.Resources.FilesCommonResource, ASC.Web.Files" alias="fres" />

  <xsl:template match="DataToImportList">
    <div style="padding:0 10px;">
      <table class="fl-tablebase">
        <thead>
          <tr>
            <td class="import_head_check">
              <input name="all_checked_document" type="checkbox" />
            </td>
            <td>
              <resource name="fres.DocumentName" />
            </td>
            <td class="import_data_author">
              <resource name="fres.Author" />
            </td>
            <td class="import_head_date">
              <resource name="fres.CreatingDate" />
            </td>
          </tr>
        </thead>
      </table>
    </div>
    <div class="borderBase import_data_table" >
      <table class="fl-tablebase">
        <tbody>
          <xsl:for-each select="entry">
            <tr>
              <xsl:if test="position() mod 2">
                <xsl:attribute name="class">tintMedium</xsl:attribute>
              </xsl:if>
              <td class="borderBase import_data_check">
                <input type="checkbox" name="checked_document">
                  <xsl:attribute name="value"><xsl:value-of select="content_link" /></xsl:attribute>
                </input>
              </td>
              <td class="borderBase">
                <div class="import_title">
                  <xsl:attribute name="title"><xsl:value-of select="title" /></xsl:attribute>
                  <xsl:value-of select="title" />
                </div>
              </td>
              <td class="borderBase import_data_author">
                <div class="import_author">
                  <xsl:attribute name="title"><xsl:value-of select="create_by" /></xsl:attribute>
                  <xsl:value-of select="create_by" />
                </div>
              </td>
              <td class="borderBase import_data_date">
                <xsl:value-of select="create_on" />
              </td>
            </tr>
          </xsl:for-each>
        </tbody>
      </table>
    </div>
  </xsl:template>

</xsl:stylesheet>