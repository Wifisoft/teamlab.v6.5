
if (typeof window.ASC === 'undefined') {
  window.ASC = {};
}
if (typeof window.ASC.Controls === 'undefined') {
  window.ASC.Controls = {};
}
if (typeof window.ASC.Controls.XSLTManager === 'undefined') {
  window.ASC.Controls.XSLTManager = (function () {
    var loadXML = (function () {
      if (window.ActiveXObject) {
        return function (file) {
          if (typeof file !== 'string' || file.length === 0) {
            return undefined;
          }
          var xhttp = new ActiveXObject('Microsoft.XMLHTTP');
          xhttp.open('GET', file, false);
          xhttp.send('');
          return xhttp.responseXML;
        };
      }
      return function (file) {
        if (typeof file !== 'string' || file.length === 0) {
          return undefined;
        }
        var xhttp = new XMLHttpRequest();
        xhttp.open('GET', file, false);
        xhttp.send('');
        return xhttp.responseXML;
      };
    })();

    var createXML = (function () {
      if (window.ActiveXObject) {
        return function (data) {
          if (typeof data !== 'string' || data.length === 0) {
            return undefined;
          }
          var xmlDoc = new ActiveXObject('Microsoft.XMLDOM');
          xmlDoc.async = 'false';
          xmlDoc.loadXML(data);
          if (xmlDoc.parseError.errorCode != 0) {
            throw 'Can\'t create xml document';
          }
          return xmlDoc;
        };
      }
      return function (data) {
        if (typeof data !== 'string' || data.length === 0) {
          return undefined;
        }
        var xmlDoc = new DOMParser();
        try {
          xmlDoc = xmlDoc.parseFromString(data, 'text/xml');
        } catch (err) {
          throw 'Can\'t create xml document : ' + err;
        }
        return xmlDoc;
      };
    })();

    var translateFromFile = function (xml, xsl) {
      if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
        return '';
      }
      if (typeof xml === 'string') {
        xml = loadXML(xml);
      }
      if (typeof xsl === 'string') {
        xsl = loadXML(xsl);
      }
      return xmlTranslate(xml, xsl);
    }

    var translateFromString = function (xml, xsl) {
      if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
        return '';
      }
      if (typeof xml === 'string') {
        xml = createXML(xml);
      }
      if (typeof xsl === 'string') {
        xsl = createXML(xsl);
      }
      
      return xmlTranslate(xml, xsl);
    }

    var xmlTranslate = (function () {
      if (window.ActiveXObject) {        
        return function (xml, xsl) {
          var
            xmlstr = '';
          if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
            return xmlstr;
          }
          try { xsl.resolveExternals = true; } catch (err) { }
          try {          
            xmlstr = xml.transformNode(xsl);            
          } catch (err) {
            throw 'Can\'t translate xml : ' + err;
          }
          return xmlstr;
        };
      }
      return function (xml, xsl) {
        var
          xmlstr = '';
        if (typeof xml === 'undefined' || typeof xsl === 'undefined') {
          return xmlstr;
        }
        var
          xmlDocument = null,
          xsltProcessor = null,
          xmlSerializer = null;
        try {
          xsltProcessor = new XSLTProcessor();
          xsltProcessor.importStylesheet(xsl);
          xmlDocument = xsltProcessor.transformToFragment(xml, document);
        } catch (err) {
          throw 'Can\'t translate xml : ' + err;
        }
        try {
          xmlSerializer = new XMLSerializer();
          xmlstr = xmlSerializer.serializeToString(xmlDocument);
        } catch (err) {
          throw 'Can\'t serialized xml : ' + err;
        }
        return xmlstr;
      };
    })();

    return {
      createXML : createXML,
      loadXML   : loadXML,

      translate           : xmlTranslate,
      translateFromFile   : translateFromFile,
      translateFromString : translateFromString
    };
  })();
}
