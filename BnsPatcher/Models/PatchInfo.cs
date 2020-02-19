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
        public double ProgressMaximum { get; set; }
        public bool IsPatching { get; }
        // Config
        public bool ShowDps { get; set; } = true;
        public bool FastExtract { get; set; } = true;
        public bool DisableAutoBias { get; set; } = true;
        public bool NoChatBan { get; set; } = true;
        public Exception Exception { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
