using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            get => _datFolder;
            set
            {
                if (value == _datFolder) return;
                _datFolder = value;
                OnPropertyChanged();
            }
        }

        public string ProfilePath
        {
            get => _profilePath;
            set
            {
                if (value == _profilePath) return;
                _profilePath = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                if (value == _progress) return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get => _progressText;
            set
            {
                if (value == _progressText) return;
                _progressText = value;
                OnPropertyChanged();
            }
        }

        public double ProgressMaximum
        {
            get => _progressMaximum;
            set
            {
                if (value == _progressMaximum) return;
                _progressMaximum = value;
                OnPropertyChanged();
            }
        }

        public bool IsPatching
        {
            get => _isPatching;
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
