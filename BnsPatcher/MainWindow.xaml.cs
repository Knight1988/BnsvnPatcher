using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using BnsPatcher.Models;
using Ookii.Dialogs.Wpf;

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
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                PatchInfo.DatFolder = dialog.SelectedPath;
            }
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
        {
            ;
            await UpdateProgressAsync(jobDone / total * 100 + _progress, progressText);
        }

        /// <summary>
        /// Extract, patch and compile dat files
        /// </summary>
        private async void PatchAsync()
        {
            if (!Validate()) return;

            await Task.Run(() =>
            {
                try
                {
                    var datEditor = new DatEditor();
                    var xml64Path = Path.Combine(PatchInfo.DatFolder, "xml64.dat");
                    var clientConfig2 = datEditor.ExtractFile(xml64Path, "client.config2.xml");
                    clientConfig2 = ReplaceText(clientConfig2);
                    datEditor.SaveFile(xml64Path, "client.config2.xml", clientConfig2);
                    PatchInfo.Progress = 100;
                    PatchInfo.ProgressMaximum = 100;
                    PatchInfo.ProgressText = "100%";
                }
                catch (Exception e)
                {
                    PatchInfo.Exception = e;
                }
            });

            if (PatchInfo.Exception != null)
            {
                MessageBox.Show(PatchInfo.Exception.ToString());
                PatchInfo.Exception = null;
            }
            else
            {
                MessageBox.Show("Patch Successful");
            }
        }

        private string ReplaceText(string str)
        {
            if (PatchInfo.ShowDps)
            {
                var names = new[]
                {
                    "showtype-public-zone", "showtype-party-4-dungeon-and-cave", "showtype-party-6-dungeon-and-cave",
                    "showtype-field-zone", "showtype-classic-field-zone", "showtype-faction-battle-field-zone",
                    "showtype-jackpot-boss-zone"
                };
                foreach (var name in names)
                {
                    var regex = new Regex($"<option name=\"{name}\" value=\"[0-9.]+\" \\/>");
                    str = regex.Replace(str, $"<option name=\"{name}\" value=\"2\" />");
                }
            }

            if (PatchInfo.NoChatBan)
            {
                var names = new[]
                {
                    "input-papering-check-low-penalty-duration", "input-papering-check-midium-penalty-duration",
                    "input-papering-check-high-penalty-duration"
                };
                foreach (var name in names)
                {
                    var regex = new Regex($"<option name=\"{name}\" value=\"[0-9.]+\" \\/>");
                    str = regex.Replace(str, $"<option name=\"{name}\" value=\"3.000000\" />");
                }
            }

            if (PatchInfo.FastExtract)
            {
                var dic = new Dictionary<string, string>()
                {
                    {"delay-postproc-time", "0.100000" },
                    {"delay-ui-time", "0.100000" },
                    {"train-complete-delay-time", "0.100000" },
                    {"rapid-decompose-duration", "0.100000" },
                    {"decompose-complete-delay", "0.100000" },
                    {"self-restraint-gauge-time", "0.010000" },
                    {"item-transform-progressing-particle-duration", "0.1" },
                };
                foreach (var keyValue in dic)
                {
                    var regex = new Regex($"<option name=\"{keyValue.Key}\" value=\"[0-9.]+\" \\/>");
                    str = regex.Replace(str, $"<option name=\"{keyValue.Key}\" value=\"{keyValue.Value}\" />");
                }
            }

            return str;
        }

        private bool Validate()
        {
            if (PatchInfo.DatFolder == null)
            {
                MessageBox.Show("Chưa chọn thư mục Data");
                return false;
            }

            var xml64 = Path.Combine(PatchInfo.DatFolder, "xml64.dat");
            if (!File.Exists(xml64))
            {
                MessageBox.Show("Không tìm thấy file xml64.dat");
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