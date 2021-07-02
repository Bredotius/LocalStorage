using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage
{
    class SearchDialogViewModel : INotifyPropertyChanged
    {
        public Search Search;
        public SearchDialogViewModel(Search s)
        {
            // получаем объект Search
            Search = s;
        }
        public string Query
        {
            get { return Search.Query; }
            set
            {
                Search.Query = value;
                OnPropertyChanged("Query");
            }
        }
        public DateTime FirstDate
        {
            get { return Search.FirstDate; }
            set
            {
                Search.FirstDate = value;
                OnPropertyChanged("FirstDate");
            }
        }
        public DateTime LastDate
        {
            get { return Search.LastDate; }
            set
            {
                Search.LastDate = value;
                OnPropertyChanged("LastDate");
            }
        }
        public bool InTitle
        {
            get { return Search.InTitle; }
            set
            {
                Search.InTitle = value;
                OnPropertyChanged("InTitle");
            }
        }
        public bool InDescription
        {
            get { return Search.InDescription; }
            set
            {
                Search.InDescription = value;
                OnPropertyChanged("InDescription");
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
