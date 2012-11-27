<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" encoding="utf-8" />

  <xsl:param name="p0"/>
  <xsl:param name="p1"/>
  <xsl:param name="p2"/>

  <xsl:decimal-format NaN = "0.00" />

  <xsl:key name="users" match="r" use="@c0" />

  <xsl:template match="*">


    <xsl:value-of select="$p0"/>
    <xsl:text>,</xsl:text>
    <xsl:value-of select="$p2"/>
    <xsl:text>&#10;</xsl:text>

    <xsl:apply-templates select="r[generate-id(.)=generate-id(key('users', @c0))]"/>

  </xsl:template>

  <xsl:template match="r">

    <xsl:value-of select="@c5" />
    <xsl:text>,</xsl:text>
    <xsl:value-of select="@c4"/>
    <xsl:text>&#10;</xsl:text>
    
  </xsl:template>

</xsl:stylesheet>