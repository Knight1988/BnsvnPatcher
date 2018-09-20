using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using bnsvn_dat.Models;

namespace bnsvn_dat.Business
{
    public class XmlPatchBusiness
    {
        public static XmlPatch ReadPatch(string path)
        {
            var serializer = new XmlSerializer(typeof(XmlPatch));
            using (var fileStream = new FileStream(path, FileMode.Open)) 
            {
                var xmlPatch = (XmlPatch) serializer.Deserialize(fileStream );
                return xmlPatch;
            }
        }

        public static void ReplaceValues(XmlPatch xmlPatch, string basePath)
        {
            foreach (var patch in xmlPatch.Patches)
            {
                // Get file path
                var fileName = Path.Combine(basePath, patch.FileName);
                
                // file 32 bir
                var file32 = fileName.Replace("[bit]", string.Empty);
                
                // file 64 bit
                var file64 = fileName.Replace("[bit]", "64");
                
                // replace contents
                ReplaceLines(file32, patch.FindAndReplace);
                ReplaceLines(file64, patch.FindAndReplace);
            }
        }

        private static void ReplaceLines(string fileName, FindAndReplace[] findAndReplaces)
        {
            var lines = File.ReadAllLines(fileName);
            foreach (var findAndReplace in findAndReplaces)
            {
                var find = new Regex($"<option name=\"{findAndReplace.Find.Option.Name}\" value=\"{findAndReplace.Find.Option.Value}\" />");
                var replace =
                    $"<option name=\"{findAndReplace.Find.Option.Name}\" value=\"{findAndReplace.Replace.Option.Value}\" />";
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    lines[i] = find.Replace(line, replace);
                }
            }
            File.WriteAllLines(fileName, lines);
        }
    }
}