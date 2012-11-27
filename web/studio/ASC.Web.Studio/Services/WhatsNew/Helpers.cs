using System;
using System.Collections.Generic;

namespace ASC.Web.Studio.Services.WhatsNew
{
    public static class Helpers
    {
        public static Guid? ParseGuid(string guid)
        {
            try
            {
                return new Guid(guid);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<string> Tokenize(string param)
        {
            return !string.IsNullOrEmpty(param) ? new List<string>(param.Split(',')) : new List<string>();
        }

        public static List<int> ParseInts(string intValue)
        {
            var result = new List<int>();
            if (!string.IsNullOrEmpty(intValue))
            {
                foreach (var val in intValue.Split(','))
                {
                    try
                    {
                        result.Add(int.Parse(val));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
            return result;
        }

        public static int? ParseInt(string intValue)
        {
            try
            {
                return int.Parse(intValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<Guid> ParseGuids(string guids)
        {
            var result = new List<Guid>();
            if (!string.IsNullOrEmpty(guids))
            {
                foreach (var guid in guids.Split(','))
                {
                    try
                    {
                        result.Add(new Guid(guid));
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            return result;
        }
    }
}