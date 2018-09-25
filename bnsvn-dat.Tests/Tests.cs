using System;
using System.Collections.Generic;
using System.IO;
using BNSDat;
using NUnit.Framework;

namespace bnsvn_dat.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void ProfileReadTest()
        {
            // 
            var profilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Profiles", "DK profile.xml");

            // Act
            var patch = XmlPatch.ReadProfile(profilePath);
            
            // Assert
            Assert.AreEqual("xml[bit].dat.files\\\\client.config2.xml", patch.Patches[0].FileName);
            Assert.AreEqual("use-optimal-performance-mode-option", patch.Patches[0].FindAndReplace[0].Find.Option.Name);
            Assert.AreEqual("use-optimal-performance-mode-option", patch.Patches[0].FindAndReplace[0].Replace.Option.Name);
            Assert.AreEqual("false", patch.Patches[0].FindAndReplace[0].Find.Option.Value);
            Assert.AreEqual("true", patch.Patches[0].FindAndReplace[0].Replace.Option.Value);
        }

        [Test]
        public void ReplaceValues()
        {
            var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "bnsdat");
            var profilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Profiles", "DK profile.xml");
            var patch = XmlPatch.ReadProfile(profilePath);

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
            var pathOriginal = Path.Combine(TestContext.CurrentContext.TestDirectory, "bnsdat", folderName);
            
            var bnsDat = new BNSDat.BNSDat();
            bnsDat.Compress(pathOriginal, true);
        }
    }
}