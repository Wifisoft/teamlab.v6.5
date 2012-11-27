// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="AssemblyInfo.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("ASC.Licensing")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("ASC.Licensing")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("4834574f-9dbc-43e9-8cbc-765ef191996f")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if (SERVER)
[assembly: InternalsVisibleTo("ASC.Licensing.Server, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e70ba46982e260"+
"6d60a3a1f0054bc9726219faedfa9776cf06416d26a1a5c29a3537a60d50fa806164452519e1a0"+
"7ebadbdef58514b5b459bead560c6404e62a87f0498e5dbddb7601d9291b77639637694b776f59"+
"fe48baf21eabd904c26800b2d36c5996557d2954eb93207ac2df67d1cb8a6220917718aa7a1454"+
"ef2cbde4")]
#endif

#if (TESTS)
[assembly: InternalsVisibleTo("ASC.Licesnsing.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e70ba46982e260" +
"6d60a3a1f0054bc9726219faedfa9776cf06416d26a1a5c29a3537a60d50fa806164452519e1a0" +
"7ebadbdef58514b5b459bead560c6404e62a87f0498e5dbddb7601d9291b77639637694b776f59" +
"fe48baf21eabd904c26800b2d36c5996557d2954eb93207ac2df67d1cb8a6220917718aa7a1454" +
"ef2cbde4")]
#endif