<?xml version="1.0"?>
<xsl:stylesheet version="2.0"
  xmlns:xhtml="http://www.w3.org/1999/xhtml"
  xmlns="http://www.w3.org/1999/xhtml"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema"
  exclude-result-prefixes="xhtml xsl xs">

  <xsl:output
    method="html"
    version="1.0"
    encoding="utf-8"
    doctype-public="-//W3C//DTD XHTML 1.1//EN"
    doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"
    indent="yes"/>

  <xsl:param name="p0"/>
  <xsl:param name="p1"/>

  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <meta name="GENERATOR" content="Teamlab Auto Reports"/>
        <style>
          table.sortable
          {
          border-collapse: collapse;
          width: 100%;
          }

          table.sortable thead th
          {
          cursor: pointer;
          cursor: hand;
          color: #82878D;
          font-size: 12px;
          }

          table.sortable tbody td
          {
          text-align: left;
          border-left: none;
          border-right: none;
          vertical-align: middle;
          }
          table.pm-tablebase
          {
          border-collapse: collapse;
          width: 100%;
          }
          table.pm-tablebase thead td
          {
          text-align: left;
          color: #82878D;
          font-size: 12px;
          }
          table.pm-tablebase tbody td
          {
          text-align: left;
          border-left: none;
          border-right: none;
          vertical-align: middle;
          }
          .pm-grayText
          {
          color: #83888d !important;
          }

          .pm-redText
          {
          color: #bf3a3c !important;
          }

          .pm-blueText
          {
          color: #709fb7 !important;
          }

          .pm-greenText
          {
          color: #6bac8b !important;
          }
          dl, dt, dd
          {
          padding: 0px;
          margin: 0px;
          }
          .pm-report-userName
          {
          padding-left: 15px;
          padding-right: 15px;
          }
          .tintMedium
          {
          background-color: #edf6fd;
          }
          .borderBase
          {
          border:solid 1px #d1d1d1;
          }
          a.smallLinkHeader, a.smallLinkHeader:visited, a.smallLinkHeader:hover, a.smallLinkHeader:active
          {
          color: #000000;
          font-family: Arial;
          font-size: 14px;
          font-weight: bold;
          text-decoration: none;
          }
          a.smallLinkHeader:hover
          {
          text-decoration: underline;
          }
          a.smallLinkHeader:active
          {
          text-decoration: none;
          }
          .headerBaseMedium
          {
          color: #3C4046;
          font-size: 14px;
          font-weight:bolder;
          font-family: Arial;
          }
          .noContentBlock
          {
          color: #373737;
          font-size: 14px;
          font-weight: bold;
          margin: 5px 0 11px;
          padding: 100px;
          text-align: center;
          }
        </style>
      </head>
      <body>
        <div style="background-color:#4ca4e3;border-bottom:solid 1px #3b6d92;padding:10px 40px;font-size:28px;color:#fcfdff;">
          Team<span style="color:#d5e8f9;">Lab</span>
        </div>
        <div style="margin:0px 40px;">
          <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
          </xsl:copy>
        </div>
        <div style="margin:30px 0 0 40px;">
            <a style="color:#a3a3a3; font-size: 11px;">
              <xsl:attribute name="href">
                <xsl:value-of select="$p0"/>
              </xsl:attribute>
              <xsl:value-of select="$p1"/>
            </a>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>