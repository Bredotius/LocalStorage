using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage
{
    class Search : INotifyPropertyChanged
    {
        private string query;
        private DateTime firstDate;
        private DateTime lastDate;
        private bool inTitle;
        private bool inDescription;

        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                OnPropertyChanged("Query");
            }
        }
        public DateTime FirstDate
        {
            get { return firstDate; }
            set
            {
                firstDate = value;
                OnPropertyChanged("FirstDate");
            }
        }
        public DateTime LastDate
        {
            get { return lastDate; }
            set
            {
                lastDate = value;
                OnPropertyChanged("LastDate");
            }
        }
        public bool InTitle
        {
            get { return inTitle; }
            set
            {
                inTitle = value;
                OnPropertyChanged("InTitle");
            }
        }
        public bool InDescription
        {
            get { return inDescription; }
            set
            {
                inDescription = value;
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
