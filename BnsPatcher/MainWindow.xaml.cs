using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BnsPatcher.Models;
using BNSDat;
using Ookii.Dialogs.Wpf;

namespace BnsPatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PatchInfo _patchInfo = new PatchInfo();
        private double _progress = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _patchInfo;
        }

        private void btnBrowseDatFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == true) _patchInfo.DatFolder = dialog.SelectedPath;
        }

        private void BtnBrowseProfile_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaOpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == true) _patchInfo.ProfilePath = dialog.FileName;
        }

        private void BtnPatch_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => PatchAsync());
        }

        private async Task UpdateProgressAsync(double progress, string progressText)
        {
            await Task.Yield();
            _patchInfo.Progress = progress;
            _patchInfo.ProgressText = progressText;
        }

        private async Task UpdateProgressAsync(double jobDone, double total, string progressText)
        {;
            await UpdateProgressAsync(jobDone / total * 100 + _progress, progressText);
        }

        /// <summary>
        /// Extract, patch and compile dat files
        /// </summary>
        private async void PatchAsync()
        {
            _patchInfo.IsPatching = true;
            try
            {
                // get all dat files
                var files = Directory.GetFiles(_patchInfo.DatFolder, "*.dat");
                _patchInfo.ProgressMaximum = files.Length * 100 * 2 + 100;

                // extract all dat file
                ExtractAll(files);

                // replace values
                await UpdateProgressAsync(_progress+=100, "Patching xml");
                XmlPatch.ReplaceValues(XmlPatch.ReadProfile(_patchInfo.ProfilePath), _patchInfo.DatFolder);

                // compress to dat files
                CompressAll(files);

                await UpdateProgressAsync(_patchInfo.ProgressMaximum, "Done");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error when patching files" + Environment.NewLine + e.ToString());
            }
            finally
            {
                _patchInfo.IsPatching = false;
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(_patchInfo.DatFolder))
            {
                MessageBox.Show("Invalid dat folder");
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(_patchInfo.ProfilePath))
            {
                MessageBox.Show("Invalid profile");
                return false;
            }

            try
            {
                XmlPatch.ReadProfile(_patchInfo.ProfilePath);
            } 
            catch (Exception ex)
            {
                MessageBox.Show("Invalid profile" + Environment.NewLine + ex.ToString());
                return false;
            }


            return true;
        }

        /// <summary>
        /// Compress into dat files
        /// </summary>
        /// <param name="files"></param>
        private void CompressAll(string[] files)
        {
            foreach (var fileName in files)
            {
                var file = new FileInfo(fileName);
                switch (file.Name)
                {
                    case "config.dat":
                    case "xml.dat":
                        if (Directory.GetFiles(file.FullName + ".files", "*.xml").Length < 4) throw new Exception("Invalid extracted files");
                        XmlPatch.Compress(file.FullName + ".files", false, async (jobDone, total) => await UpdateProgressAsync(jobDone, total, $"Compiling {file.Name} {jobDone}/{total}"));
                        break;
                    case "config64.dat":
                    case "xml64.dat":
                        if (Directory.GetFiles(file.FullName + ".files", "*.xml").Length < 4) throw new Exception("Invalid extracted files");
                        XmlPatch.Compress(file.FullName + ".files", true, async (jobDone, total) => await UpdateProgressAsync(jobDone, total, $"Compiling {file.Name} {jobDone}/{total}"));
                        break;
                }

                _progress += 100;
            }
        }

        /// <summary>
        /// Extract dat files
        /// </summary>
        /// <param name="files"></param>
        private void ExtractAll(string[] files)
        {
            foreach (var fileName in files)
            {
                var file = new FileInfo(fileName);
                switch (file.Name)
                {
                    case "config.dat":
                    case "xml.dat":
                        XmlPatch.Extract(file.FullName, false, async (jobDone, total) => await UpdateProgressAsync(jobDone, total, $"Extracting {file.Name} {jobDone}/{total}"));
                        break;
                    case "config64.dat":
                    case "xml64.dat":
                        XmlPatch.Extract(file.FullName, true, async (jobDone, total) => await UpdateProgressAsync(jobDone, total, $"Extracting {file.Name} {jobDone}/{total}"));
                        break;
                }

                _progress += 100;
            }
        }
    }
}
