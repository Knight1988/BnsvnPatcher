using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BnsPatcher
{
    public class DatEditor
    {
        private readonly string _aesKey;
        private BNSDat.BNSDat _bnsDat;
        public Dictionary<string, string> MyDictionary { get; } = new Dictionary<string, string>();

        public DatEditor()
        {
            _aesKey = Decryption.DecryptKey("EFBE9CEFBE90D9B5EFBE88EFBEA543EFBEB447EFBEB0274C36285D7FEFBE9BEFBEA4D99EEFBF88EFBE8770EFBEAC10EFBF80");
            _bnsDat = new BNSDat.BNSDat { AES_KEY = _aesKey, XOR_KEY = Decryption.XorKey };
        }

        public void LoadFolder(string path)
        {
            foreach (var file in new DirectoryInfo(path).GetFiles("*.dat"))
            {
                if (file != null)
                {
                    var directoryName = Path.GetDirectoryName(file.FullName);
                    if (!MyDictionary.ContainsKey(file.Name))
                    {
                        MyDictionary.Add(file.Name, directoryName);
                    }
                }
            }
        }

        public void LoadDataFile(string path)
        {
            var fileList = _bnsDat.GetFileList(path, path.Contains("64"));
        }

        public string ExtractFile(string datPath, string filePath)
        {
            var stringList = new List<string>()
            {
                filePath
            };
            var data = _bnsDat.ExtractFile(datPath, stringList, datPath.Contains("64"));
            return Encoding.UTF8.GetString(data[filePath]);
        }

        public void SaveFile(string datPath, string filePath, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            var dictionary = new Dictionary<string, byte[]> {{filePath, bytes}};
            _bnsDat.CompressFiles(datPath, dictionary, datPath.Contains("64"), 1);
        }
    }
}
