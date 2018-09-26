using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using BNSDat;
using NUnit.Framework;

namespace bnsvn_dat.Tests
{
    [TestFixture]
    public class Tests
    {
        private void ExtractXmlSample()
        {
            var baseFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "bnsdat");
            var input = Path.Combine(baseFolder, "xmlFiles.zip");

            using (var archive = ZipFile.OpenRead(input))
            {
                foreach (var entry in archive.Entries)
                {
                    var entryFullname = Path.Combine(baseFolder, entry.FullName);
                    var entryPath = Path.GetDirectoryName(entryFullname);
                    if (!Directory.Exists(entryPath))
                    {
                        Debug.Assert(entryPath != null, nameof(entryPath) + " != null");
                        Directory.CreateDirectory(entryPath);
                    }

                    var entryFn = Path.GetFileName(entryFullname);
                    if (!string.IsNullOrEmpty(entryFn))
                        entry.ExtractToFile(entryFullname, true);
                }
            }
        }

        [Test]
        public void ProfileReadTest()
        {
            // 
            var profilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Profiles", "DK profile.xml");

            // Act
            var patch = XmlPatch.ReadProfile(profilePath);
            
            // Assert
            Assert.AreEqual("config[bit].dat.files\\\\system.config2.xml", patch.Patches[0].FileName);
            Assert.AreEqual("use-context-simple-mode-player-character-level", patch.Patches[0].FindAndReplace[0].Find.Option.Name);
            Assert.AreEqual("use-context-simple-mode-player-character-level", patch.Patches[0].FindAndReplace[0].Replace.Option.Name);
            Assert.AreEqual("[0-9.]+", patch.Patches[0].FindAndReplace[0].Find.Option.Value);
            Assert.AreEqual("1", patch.Patches[0].FindAndReplace[0].Replace.Option.Value);
        }

        [Test]
        public void ReplaceValues()
        {
            // Arrange
            ExtractXmlSample();
            var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "bnsdat");
            var profilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Profiles", "DK profile.xml");
            var patch = XmlPatch.ReadProfile(profilePath);

            // Act
            XmlPatch.ReplaceValues(patch, basePath);
        }

        [TestCase("config.dat", false, "system.config2.xml")]
        [TestCase("config64.dat", true, "system.config2.xml")]
        [TestCase("xml.dat", false, "client.config2.xml")]
        [TestCase("xml64.dat", true, "client.config2.xml")]
        public void ExtractDatTest(string fileName, bool is64Bit, string fileToCheck)
        {
            // Arrange
            var pathOriginal = Path.Combine(TestContext.CurrentContext.TestDirectory, "bnsdat", fileName);

            // Act
            var bnsDat = new BNSDat.BNSDat();
            bnsDat.Extract(pathOriginal, is64Bit);

            // Assert
            var extractedDir = pathOriginal + ".files";
            var fileCheck = Path.Combine(extractedDir, fileToCheck);
            FileAssert.Exists(fileCheck);
        }

        [TestCase("config.dat.files", false)]
        [TestCase("config64.dat.files", true)]
        [TestCase("xml.dat.files", false)]
        [TestCase("xml64.dat.files", true)]
        public void CompressDatTest(string folderName, bool is64Bit)
        {
            // Arrange
            ExtractXmlSample();
            var pathOriginal = Path.Combine(TestContext.CurrentContext.TestDirectory, "bnsdat", folderName);
            
            // Act
            var bnsDat = new BNSDat.BNSDat();
            bnsDat.Compress(pathOriginal, true);
        }
    }
}