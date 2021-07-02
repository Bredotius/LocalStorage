using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage
{
    class DocDialogViewModel : INotifyPropertyChanged
    {
        public Doc Doc;

        public List<Doc> DocList = new List<Doc>();

        public DialogParams Params = new DialogParams();
        public DocDialogViewModel(Doc document)
        {
            // получаем объект Doc
            Doc = document;
        }
        public string DialogTitle
        {
            get { return Params.Title; }
            set
            {
                Params.Title = value;
                OnPropertyChanged("DialogTitle");
            }
        }
        public string DialogAcceptButton
        {
            get { return Params.AcceptButton; }
            set
            {
                Params.AcceptButton = value;
                OnPropertyChanged("DialogButton");
            }
        }
        public string DialogChoseButton
        {
            get { return Params.ChoseButton; }
            set
            {
                Params.ChoseButton = value;
                OnPropertyChanged("DialogChoseButton");
            }
        }
        public string DocTitleVisability
        {
            get { return Params.DocTitleVisability; }
            set
            {
                Params.DocTitleVisability = value;
                OnPropertyChanged("DocTitleVisability");
            }
        }
        public string DocDescVisability
        {
            get { return Params.DocDescVisability; }
            set
            {
                Params.DocDescVisability = value;
                OnPropertyChanged("DocDescVisability");
            }
        }
        public string DocTitle
        {
            get { return Doc.Title; }
            set
            {
                Doc.Title = value;
                OnPropertyChanged("Title");
            }
        }
        public string DocDescription
        {
            get { return Doc.Description; }
            set
            {
                Doc.Description = value;
                OnPropertyChanged("Description");
            }
        }

        RelayCommand choseFileCommand;
        // подтверждение добавления файла
        // выбор файла
        public RelayCommand ChoseFileCommand
        {
            get
            {
                return choseFileCommand ??
                  (choseFileCommand = new RelayCommand((o) =>
                  {
                      // инициализация диалогового окна для выбора файла
                      OpenFileDialog openFileDialog = new OpenFileDialog();
                      // настройки диалогового окна
                      openFileDialog.InitialDirectory = "c:\\";
                      openFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
                      openFileDialog.RestoreDirectory = true;
                      openFileDialog.Multiselect = true;

                      if (openFileDialog.ShowDialog() == true)
                      {
                          if (openFileDialog.FileNames.Length > 0)
                          {
                              foreach (String fileName in openFileDialog.FileNames)
                              {
                                  string path = fileName;
                                  // позиция "\" перед именем файла.
                                  var pos = path.LastIndexOf("\\");
                                  // получаем значение именем файла.
                                  var filename = path.Substring(pos + 1);
                                  pos = filename.LastIndexOf(".");
                                  DialogChoseButton = filename;
                                  Doc someDoc = new Doc();
                                  // передаем значение именем файла.
                                  someDoc.Title = filename.Substring(0, pos);
                                  // приобразуем файл в массив данных и передаем новому элементу
                                  someDoc.Bytes = File.ReadAllBytes(path);
                                  // добавляем новый объект в список
                                  DocList.Add(someDoc);
                              }
                          }
                      }
                  }));
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
