using System.IO;

namespace CheckerLang
{
    public class ModuleLoader
    {
        public static string LoadModule(string moduleidentifier, SourcePos pos) {
            try {
                var assembly = typeof(Environment).Assembly;
                var strm = assembly.GetManifestResourceStream("checkerlang.module-" + moduleidentifier.ToLower());
                if (strm != null)
                {
                    return new StreamReader(strm).ReadToEnd();
                }

                if (File.Exists("./" + new FileInfo(moduleidentifier).Name))
                {
                    return File.ReadAllText("./" + new FileInfo(moduleidentifier).Name);
                }
                    
                throw new ControlErrorException("Module " + new FileInfo(moduleidentifier).Name + " not found", pos);
            } catch (IOException) {
                throw new ControlErrorException("Module " + new FileInfo(moduleidentifier).Name + " not found", pos);
            }
        }
    }
}