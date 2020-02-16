using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BnsPatcher.Models;

namespace BnsPatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _progress = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowseDatFolder_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnBrowseProfile_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnPatch_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => PatchAsync());
        }

        private async Task UpdateProgressAsync(double progress, string progressText)
        {
            await Task.Yield();
            PatchInfo.Progress = progress;
            PatchInfo.ProgressText = progressText;
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
        }

        private bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Compress into dat files
        /// </summary>
        /// <param name="files"></param>
        private void CompressAll(string[] files)
        {
        }

        /// <summary>
        /// Extract dat files
        /// </summary>
        /// <param name="files"></param>
        private void ExtractAll(string[] files)
        {
        }
    }
}
