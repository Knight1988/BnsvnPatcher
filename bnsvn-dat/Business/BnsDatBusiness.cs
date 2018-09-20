using System.IO;
using BnSDat;

namespace bnsvn_dat.Business
{
    public class BnsDatBusiness
    {
        public static void Extract(string inputFile, string outputFolder, bool is64Bit)
        {
            // Delete extract folder
            var extractFolder = inputFile + ".files";
            if (Directory.Exists(extractFolder)) Directory.Delete(extractFolder, true);

            // Extract file
            var bnsDat = new BNSDat.BNSDat();
            bnsDat.Extract(inputFile, is64Bit);
        }

        public static void Compress(string inputFolder, string outputFile, bool is64Bit)
        {
            var bnsDat = new BNSDat.BNSDat();
            bnsDat.Compress(inputFolder, is64Bit);
        }
    }
}