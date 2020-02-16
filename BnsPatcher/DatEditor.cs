using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsPatcher
{
    class Decryption
    {
        private static readonly byte[] XorKey = new byte[16]
        {
            164,
            159,
            216,
            179,
            246,
            142,
            57,
            194,
            45,
            224,
            97,
            117,
            92,
            75,
            26,
            7
        };

        private static string DecryptKey(string encKey)
        {
            var encoding = DetectEncoding(encKey);
            var s = EncryptDecrypt(encoding.GetString(FromHex(encKey)));
            return encoding.GetString(Convert.FromBase64String(s));
        }

        public static Encoding DetectEncoding(string content)
        {
            using (var streamWriter = new StreamWriter(".\\dbg.txt"))
            {
                streamWriter.Flush();
                streamWriter.Write(content);
            }
            using (var streamReader = new StreamReader(".\\dbg.txt", true))
            {
                streamReader.ReadToEnd();
                return streamReader.CurrentEncoding;
            }
        }

        private static string EncryptDecrypt(string input)
        {
            var charArray = Encoding.UTF8.GetString(XorKey).ToCharArray();
            var chArray = new char[input.Length];
            for (var index = 0; index < input.Length; ++index)
                chArray[index] = (char)(input[index] ^ (uint)charArray[index % charArray.Length]);
            return new string(chArray);
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            return Enumerable.Range(0, hex.Length / 2).Select<int, byte>(i => Convert.ToByte(hex.Substring(i * 2, 2), 16)).ToArray<byte>();
        }
    }

    class DatEditor
    {
        private string _aesKey;

        public DatEditor()
        {
            _aesKey = Decryption.DecryptKey("EFBE9CEFBE90D9B5EFBE88EFBEA543EFBEB447EFBEB0274C36285D7FEFBE9BEFBEA4D99EEFBF88EFBE8770EFBEAC10EFBF80");
        }
    }
}
