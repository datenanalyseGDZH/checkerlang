using System;
using System.IO;

namespace CheckerLang
{
    public class ModuleLoader
    {
        public static string LoadModule(string moduleidentifier, SourcePos pos) {
            try 
            {
                var assembly = typeof(Environment).Assembly;
                var strm = assembly.GetManifestResourceStream("checkerlang.module-" + moduleidentifier.ToLower());
                if (strm != null)
                {
                    return new StreamReader(strm).ReadToEnd();
                }

                var reader = GetModuleStream(moduleidentifier);
                if (reader != null) 
                {
                    return reader.ReadToEnd();
                }
                    
                throw new ControlErrorException(new ValueString("ERROR"), "Module " + new FileInfo(moduleidentifier).Name + " not found", pos);
            } 
            catch (IOException) 
            {
                throw new ControlErrorException(new ValueString("ERROR"), "Module " + new FileInfo(moduleidentifier).Name + " not found", pos);
            }
        }

        private static StreamReader GetModuleStream(String moduleidentifier) 
        {
            string userdir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            string moduledir = userdir + "/.ckl/modules";
            string module = moduledir + "/" + new FileInfo(moduleidentifier).Name;
            if (File.Exists(module)) return new StreamReader(module);
            module = "./" + new FileInfo(moduleidentifier).Name;
            if (File.Exists(module)) return new StreamReader(module);
            return null;
        }
    }
}