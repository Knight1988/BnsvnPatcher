using System;
using System.IO;
using System.Reflection;
using BnsPatcher;
using NUnit.Framework;

namespace bnsvn_dat.Tests
{
    [TestFixture]
    public class Tests
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [Test]
        public void ReadDirectory()
        {
            var datEditor = new DatEditor();
            datEditor.LoadFolder(AssemblyDirectory + "\\data");
        }

        [Test]
        public void LoadDataFiles()
        {
            var datEditor = new DatEditor();
            datEditor.LoadDataFile("F:\\Games\\Garena\\32834\\service\\contents\\Local\\GARENA\\data\\xml64.dat");
        }

        [Test]
        public void ExtractFile()
        {
            var datEditor = new DatEditor();
            var xmlText = datEditor.ExtractFile("F:\\Games\\Garena\\32834\\service\\contents\\Local\\GARENA\\data\\xml64.dat", "client.config2.xml");
        }
    }
}