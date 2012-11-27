// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="HashSecret.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Licensing
{
    internal static class HashSecret
    {
        public static byte[] GetSecret()
        {
            return typeof (Client).Assembly.GetName().GetPublicKey();
        }
    }
}