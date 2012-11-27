// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="ByteExtension.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Licensing.Utils;

namespace System
{
    public static class ByteExtension
    {
         public static string Encode(this byte[] data)
         {
             return DataEncoder.ToString(data);
         }
    }
}