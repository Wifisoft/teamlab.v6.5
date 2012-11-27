using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.IO;
using System;

namespace TMResourceData
{
    public class AssemblyWork
    {
        static readonly List<Assembly> ListAssembly = new List<Assembly>();


        public static void PatchResourceManager(Type resourceManagerType)
        {
            if (resourceManagerType.GetProperty("ResourceManager", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public) != null)
            {
                try
                {
                    var resManager = resourceManagerType.InvokeMember("ResourceManager", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, resourceManagerType, new object[] { });
                    var fileName = resourceManagerType.Name + ".resx";

                    var databaseResourceManager = new DBResourceManager(fileName, resManager as ResourceManager);
                    resourceManagerType.InvokeMember("resourceMan", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField, null, resourceManagerType, new object[] { databaseResourceManager });
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is FileNotFoundException && ((FileNotFoundException)e.InnerException).FileName == "App_GlobalResources")
                    {
                        // ignore
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static void UploadResourceData(Assembly[] assemblies)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (!ListAssembly.Contains<Assembly>(callingAssembly))
            {
                ListAssembly.Add(callingAssembly);
                RemoveResManager(callingAssembly);
            }

            foreach (var assembly in assemblies.Except(ListAssembly).Where(assembly => ListAssembly.IndexOf(assembly) < 0 && (assembly.GetName().Name.IndexOf("ASC") >= 0 || assembly.GetName().Name.IndexOf("App_GlobalResources") >= 0)))
            {
                if (assembly.GetName().Name == "ASC.Common" || assembly.GetName().Name == "ASC.Core.Common")
                {
                    continue;
                }
                ListAssembly.Add(assembly);
                RemoveResManager(assembly);
            }
        }

        static void RemoveResManager(Assembly assembly)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    PatchResourceManager(type);
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // ignore
            }
        }
    }
}
