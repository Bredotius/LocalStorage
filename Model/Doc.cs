using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LocalStorage
{
    public class Doc : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _description;
        private byte[] _bytes;
        private string _date;
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value) return;
                _id = value;
                OnPropertyChanged();
            }
        }
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
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged();
            }
        }
        public byte[] Bytes
        {
            get { return _bytes; }
            set
            {
                if (_bytes == value) return;
                _bytes = value;
                OnPropertyChanged();
            }
        }
        public string Date
        {
            get { return _date; }
            set
            {
                if (_date == value) return;
                _date = value;
                OnPropertyChanged();
            }
        }
        /*public Doc()
        {
            Title = null;
            Description = null;
            Bytes = null;
            Date = null;
        }
        public Doc(string title, byte[] bytes)
        {
            Title = title;
            Bytes = bytes;
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

