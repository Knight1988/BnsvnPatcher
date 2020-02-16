using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BnsPatcher.Annotations;

namespace BnsPatcher.Models
{
    class PatchInfo : INotifyPropertyChanged 
    {
        public string DatFolder { get; set; }
        public double Progress { get; set; }
        public string ProgressText { get; set; }
        public double ProgressMaximum { get; }
        public bool IsPatching { get; }
        // Config
        public bool ShowDps { get; }
        public bool FastExtract { get; }
        public bool DisableAutoBias { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
