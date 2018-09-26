using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BnsPatcher.Annotations;

namespace BnsPatcher.Models
{
    class PatchInfo : INotifyPropertyChanged 
    {
        private string _datFolder;
        private string _profilePath;
        private double _progress;
        private string _progressText;
        private double _progressMaximum;
        private bool _isPatching;

        public string DatFolder
        {
            get { return _datFolder; }
            set
            {
                if (value == _datFolder) return;
                _datFolder = value;
                OnPropertyChanged();
            }
        }

        public string ProfilePath
        {
            get { return _profilePath; }
            set
            {
                if (value == _profilePath) return;
                _profilePath = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get { return _progress; }
            set
            {
                if (Math.Abs(value - _progress) < double.Epsilon) return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                if (value == _progressText) return;
                _progressText = value;
                OnPropertyChanged();
            }
        }

        public double ProgressMaximum
        {
            get { return _progressMaximum; }
            set
            {
                if (Math.Abs(value - _progressMaximum) < double.Epsilon) return;
                _progressMaximum = value;
                OnPropertyChanged();
            }
        }

        public bool IsPatching
        {
            get { return _isPatching; }
            set
            {
                if (value == _isPatching) return;
                _isPatching = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotPatching));
            }
        }

        public bool IsNotPatching => !_isPatching;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
