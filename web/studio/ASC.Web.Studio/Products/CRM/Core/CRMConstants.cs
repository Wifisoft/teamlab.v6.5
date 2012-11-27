using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace ASC.CRM.Core
{
    public static class CRMConstants
    {
        public static readonly String StorageModule = "crm";
        public static readonly String DatabaseId = "crm";
        public static readonly String FileKeyFormat = "{0}/{1}/{2}/{3}"; // ProjectID/FileID/FileVersion/FileTitle

    }
}


