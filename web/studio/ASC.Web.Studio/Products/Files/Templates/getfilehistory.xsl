<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" encoding="utf-8" standalone="yes" indent="yes" omit-xml-declaration="yes" media-type="text/xhtml" />

    <register type="ASC.Web.Files.Resources.FilesCommonResource,ASC.Web.Files" alias="fres" />

    <xsl:template match="fileList">
        <xsl:for-each select="entry">
            <div>
                <xsl:attribute name="data-version">
                    <xsl:value-of select="version" />
                </xsl:attribute>
                <xsl:attribute name="class">
                    clearFix versionRow
                    <xsl:if test="position() mod 2 = 1">even</xsl:if>
                </xsl:attribute>
                <div class="version_num">
                    <xsl:value-of select="version" />.
                </div>
                <div class="version_datetime">
                    <b style="margin-right:8px;">
                        <xsl:value-of select="substring-before(modified_on, ' ')" />
                    </b>
                    <xsl:value-of select="substring-after(modified_on, ' ')" />
                </div>
                <div class="version_author userLink" >
                    <xsl:attribute name="title">
                        <xsl:value-of select="modified_by" />
                    </xsl:attribute>
                    <xsl:value-of select="modified_by" />
                </div>
                <div class="version_size">
                    <xsl:value-of select="content_length" />
                </div>
                <div class="version_operation">
                    <div class="previewVersion">
                        <xsl:attribute name="title">
                            <resource name="fres.OpenFile" />
                        </xsl:attribute>
                    </div>
                    <div class="downloadVersion">
                        <xsl:attribute name="title">
                            <resource name="fres.ButtonDownload" />
                        </xsl:attribute>
                    </div>
                    <a class="version_restore baseLinkAction">
                        <xsl:attribute name="title">
                            <resource name="fres.MakeCurrent" />
                        </xsl:attribute>
                        <resource name="fres.MakeCurrent" />
                    </a>
                </div>
            </div>
        </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>