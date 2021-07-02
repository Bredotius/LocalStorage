using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage
{
    class DialogParams : INotifyPropertyChanged
    {
        private string _title;
        private string _acceptButton;
        private string _choseButton;
        private string _docTitleVisability;
        private string _docDescVisability;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }
        public string AcceptButton
        {
            get { return _acceptButton; }
            set
            {
                if (_acceptButton == value) return;
                _acceptButton = value;
                OnPropertyChanged();
            }
        }
        public string ChoseButton
        {
            get { return _choseButton; }
            set
            {
                if (_choseButton == value) return;
                _choseButton = value;
                OnPropertyChanged();
            }
        }
        public string DocTitleVisability
        {
            get {
                if (_docTitleVisability == null)
                    _docTitleVisability = "Collapsed";
                return _docTitleVisability; }
            set
            {
                if (_docTitleVisability == value) return;
                _docTitleVisability = value;
                OnPropertyChanged();
            }
        }
        public string DocDescVisability
        {
            get {
                if (_docDescVisability == null)
                    _docDescVisability = "Collapsed";
                return _docDescVisability; }
            set
            {
                if (_docDescVisability == value) return;
                _docDescVisability = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
