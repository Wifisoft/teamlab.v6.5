using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Web.Studio.Core
{
    class TcpIpFilterActions
    {
        public static readonly IAction TcpIpFilterAction = new ASC.Common.Security.Authorizing.Action(new Guid("0000ffff-ae36-4d2e-818d-726cb650aeb7"), "Tcp/Ip filter", false, false);
    }


    [DebuggerDisplay("IP = {SecurityId}")]
    class TcpIpFilterSecurityObject : ISecurityObject
    {
        private readonly byte[] ipbytes;


        public TcpIpFilterSecurityObject(IPAddress ip)
        {
            if (ip == null) throw new ArgumentNullException("ip");

            ipbytes = ip.GetAddressBytes();
            if (ipbytes.Length == 4)
            {
                ipbytes = Enumerable.Repeat((byte)0, 12).Concat(ipbytes).ToArray(); // to IPv6
            }

            var s = new StringBuilder(40);
            for (var i = 0; i < ipbytes.Length; i++)
            {
                s.AppendFormat("{0:x2}", ipbytes[i]);
                if ((i + 1) % 2 == 0) s.Append(":");
            }
            s.Length -= 1;
            SecurityId = s.ToString();
        }


        public Type ObjectType
        {
            get { return GetType(); }
        }

        public object SecurityId
        {
            get;
            private set;
        }


        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return false; }
        }


        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            var bytes = ((TcpIpFilterSecurityObject)objectId).ipbytes.ToArray(); // copy
            for (var i = bytes.Length - 1; 0 <= i; i--)
            {
                if (bytes[i] != 0)
                {
                    bytes[i] = 0;
                    break;
                }
                if (i == 0)
                {
                    return null;
                }
            }
            return new TcpIpFilterSecurityObject(new IPAddress(bytes));
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            throw new NotImplementedException();
        }


        public static IPAddress ParseObjectId(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentNullException("objectId");
            }

            var ipstring = objectId;
            if (objectId.Contains('|'))
            {
                ipstring = objectId.Split('|')[1];
            }
            ipstring = ipstring.Replace(":", string.Empty).Trim();
            if (ipstring.Length != 32)
            {
                throw new FormatException("objectId");
            }

            var ipbytes = new byte[16];
            for (var i = 0; i < ipbytes.Length; i++)
            {
                ipbytes[i] = byte.Parse(string.Concat(ipstring[2 * i], ipstring[2 * i + 1]), NumberStyles.HexNumber);
            }

            // ipv4 or ipv6
            return new IPAddress(ipbytes.Take(12).All(b => b == 0) ? ipbytes.Skip(12).ToArray() : ipbytes);
        }
    }
}
