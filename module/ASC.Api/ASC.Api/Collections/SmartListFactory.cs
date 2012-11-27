// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="SmartListFactory.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ASC.Api.Collections
{
    public static class SmartListFactory
    {
         public static SmartList<T> Create<T>(IEnumerable<T> elements)
         {
             if (elements!=null)
             {
                 return new CompiledSmartList<T>(elements);
             }
             return null;
         }

        public static Type GetSmartListType()
        {
            return typeof (CompiledSmartList<>);
        }
    }
}