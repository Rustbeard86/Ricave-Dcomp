using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ricave.Core
{
    public static class AssemblyLoadUtility
    {
        public static bool TryLoadAssembly(string path, out Assembly assembly)
        {
            AssemblyLoadUtility.CheckAddAssemblyResolveHandler();
            bool flag;
            try
            {
                byte[] array = File.ReadAllBytes(path);
                string text = FilePathUtility.GetFilePathWithoutExtension(path) + ".pdb";
                if (File.Exists(text))
                {
                    byte[] array2 = File.ReadAllBytes(text);
                    assembly = AppDomain.CurrentDomain.Load(array, array2);
                }
                else
                {
                    assembly = AppDomain.CurrentDomain.Load(array);
                }
                if (assembly == null)
                {
                    Log.Error("Failed to load assembly at " + path + ".", false);
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while loading assembly at " + path + ".", ex);
                assembly = null;
                flag = false;
            }
            return flag;
        }

        public static bool VerifyAssembly(Assembly assembly)
        {
            bool flag;
            try
            {
                assembly.GetTypes();
                flag = true;
            }
            catch (ReflectionTypeLoadException ex)
            {
                string text;
                if (ex.LoaderExceptions != null)
                {
                    text = " Errors:\n" + string.Join("\n\n", ex.LoaderExceptions.Select<Exception, string>((Exception x) => x.ToString()));
                }
                else
                {
                    text = "";
                }
                Log.Error("Loaded assembly " + ((assembly != null) ? assembly.ToString() : null) + " is unusable." + text, false);
                flag = false;
            }
            return flag;
        }

        private static void CheckAddAssemblyResolveHandler()
        {
            if (AssemblyLoadUtility.assemblyResolveHandlerAdded)
            {
                return;
            }
            AssemblyLoadUtility.assemblyResolveHandlerAdded = true;
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) => Assembly.GetExecutingAssembly();
        }

        private static bool assemblyResolveHandlerAdded;
    }
}