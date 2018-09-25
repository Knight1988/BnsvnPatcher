using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace BNSDat
{
    public partial class XmlPatch
    {
        public static XmlPatch ReadProfile(string path)
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

        public static void Extract(string inputFile, bool is64Bit, Action<double, double> progress = null)
        {
            // Delete extract folder
            var extractFolder = inputFile + ".files";
            if (Directory.Exists(extractFolder)) Directory.Delete(extractFolder, true);

            // Extract file
            var bnsDat = new BNSDat();
            bnsDat.Extract(inputFile, is64Bit, progress);
        }

        public static void Compress(string inputFolder, bool is64Bit, Action<double, double> progress = null)
        {
            var bnsDat = new BNSDat();
            bnsDat.Compress(inputFolder, is64Bit, progress);
        }
    }
}