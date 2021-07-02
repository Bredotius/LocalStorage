using MaterialDesignThemes.Wpf;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Security.Cryptography;

namespace LocalStorage
{
    class MainViewModel : INotifyPropertyChanged
    {
        ApplicationContext db;
        RelayCommand deleteCommand;
        RelayCommand openCommand;
        RelayCommand copyCommand;
        RelayCommand searchTitleCommand;
        RelayCommand clearCommand;

        private IEnumerable<Doc> selectedDocuments;
        public IEnumerable<Doc> SelectedDocuments
        {
            get { return selectedDocuments; }
            set
            {
                selectedDocuments = value;
                OnPropertyChanged("SelectedDocuments");
            }
        }

        private IEnumerable<Doc> documents;
        public IEnumerable<Doc> Documents
        {
            get { return documents; }
            set
            {
                documents = value;
                OnPropertyChanged("Documents");
            }
        }

        public MainViewModel()
        {
            // устанавливаем соединение с базой данных
            db = new ApplicationContext();
            // загружаем данные из таблицы Docs
            db.Docs.Load();
            // преобразуем данные в список
            // и устанавливаем в качестве контекста данных
            Documents = db.Docs.Local.ToBindingList();
            DocsCount = Documents.Count();
            SetTimeLims();
        }

        DateTime FirstDate;
        DateTime LastDate;
        public void SetTimeLims()
        {
            FirstDate = Convert.ToDateTime(Documents.First().Date);
            LastDate = Convert.ToDateTime(Documents.Last().Date);
        }

        public void UpdTimeLims()
        {
            LastDate = Convert.ToDateTime(Documents.Last().Date);
        }

        // поиск
        public bool IsSearchOn = false;

        public Search extSearch;

        private string searchText;
        public string SearchText 
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        private string _clearSearch;
        public string ClearSearch
        {
            get 
            {
                if (_clearSearch == null)
                    _clearSearch = "Hidden";
                return _clearSearch; 
            }
            set
            {
                _clearSearch = value;
                OnPropertyChanged("ClearSearch");
            }
        }
        // поиск по названию
        void SearchByTitle(string txt)
        {
            // загружаем список документов
            Documents = db.Docs.Local.ToBindingList();
            // поиск только в названию
            Documents = Documents.Where(doc =>
            doc.Title.ToLower().Contains(SearchText.ToLower())).ToList();
        }

        //DocsCount = Documents.Count();
        public RelayCommand SearchTitleCommand
        {
            get
            {
                return searchTitleCommand ??
                  (searchTitleCommand = new RelayCommand((o) =>
                  {
                      if (SearchText != null)
                      {
                          SearchByTitle(SearchText);
                          //SearchResults = Documents.Where(doc => doc.Title.ToLower().Contains(SearchText.ToLower()));
                          //SearchResults = db.Docs.Where(doc => doc.Title.ToLower().Contains(SearchText.ToLower())).ToList();
                          ClearSearch = "Visible";
                          IsSearchOn = true;
                      }
                  }));
            }
        }
        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand ??
                  (clearCommand = new RelayCommand((o) =>
                  {
                      Documents = db.Docs.Local.ToBindingList();
                      SearchText = null;
                      extSearch = null;
                      ClearSearch = "Hidden";
                      IsSearchOn = false;
                      DocsCount = Documents.Count();
                  }));
            }
        }
        // расширеный поиск
        void ExtendedSearch(Search search)
        {
            // загружаем список документов
            Documents = db.Docs.Local.ToBindingList();
            // поиск по всем параметрам
            if (search.Query != null && search.InTitle)
            {
                Documents = Documents.Where(doc =>
                {
                    if (search.InDescription && doc.Description != null)
                    {
                        return ((doc.Title.ToLower().Contains(search.Query.ToLower())
                        || doc.Description.ToLower().Contains(search.Query.ToLower()))
                          && (search.FirstDate.CompareTo(Convert.ToDateTime(doc.Date)) <= 0
                          && search.LastDate.CompareTo(Convert.ToDateTime(doc.Date)) >= 0));
                    }
                    else return (doc.Title.ToLower().Contains(search.Query.ToLower())
                          && (search.FirstDate.CompareTo(Convert.ToDateTime(doc.Date)) <= 0
                          && search.LastDate.CompareTo(Convert.ToDateTime(doc.Date)) >= 0));
                });
            }
            // поиск в описании и по дате добавления
            else if (search.Query != null && search.InDescription)
            {
                Documents = Documents.Where(doc =>
                {
                    if (doc.Description != null)
                    {
                        return doc.Description.ToLower().Contains(search.Query.ToLower())
                          && search.FirstDate.CompareTo(Convert.ToDateTime(doc.Date)) <= 0
                          && search.LastDate.CompareTo(Convert.ToDateTime(doc.Date)) >= 0;
                    }
                    else return search.FirstDate.CompareTo(Convert.ToDateTime(doc.Date)) 
                        <= 0 && search.LastDate.CompareTo(Convert.ToDateTime(doc.Date)) >= 0;
                });
            }
            // поиск по дате добавления
            else Documents = Documents.Where(doc => search.FirstDate.CompareTo(Convert.ToDateTime(doc.Date)) 
            <= 0 && search.LastDate.CompareTo(Convert.ToDateTime(doc.Date)) >= 0);

            DocsCount = Documents.Count();
        }

        public ICommand OpenSearchDialogCommand => new RelayCommand(OpenSearchDialog);

        private async void OpenSearchDialog(object o)
        {
            // создание окна формы добавления
            SearchDialog SearchDialog = new SearchDialog();
            SearchDialogViewModel SearchVM = new SearchDialogViewModel(new Search());
            SearchDialog.DataContext = SearchVM;
            SearchDialogViewModel vm = SearchDialog.DataContext as SearchDialogViewModel;
            vm.Search.Query = SearchText;
            vm.Search.FirstDate = FirstDate;
            vm.Search.LastDate = LastDate;
            vm.Search.InTitle = true;
            vm.Search.InDescription = false;
            extSearch = vm.Search;
            //show the dialog
            var result = await DialogHost.Show(SearchDialog, "RootDialog");
            bool res = (bool)result;
            if (res)
            {
                ExtendedSearch(extSearch);
                ClearSearch = "Visible";
                IsSearchOn = true;
            }
        }

        void UpdateSeachList()
        {
            if (extSearch != null)
            {
                ExtendedSearch(extSearch);
            }
            else SearchByTitle(SearchText);
        }

        // добавление
        public ICommand OpenAddDialogCommand => new RelayCommand(AddDialog);


        private async void AddDialog(object o)
        {
            // создание диалогового окна формы добавления
            DocDialog DocDialog = new DocDialog();
            DocDialogViewModel DocVM = new DocDialogViewModel(new Doc());
            // привязка нового объекта к контексту данных диалогового окна
            DocDialog.DataContext = DocVM;
            DocDialogViewModel vm = DocDialog.DataContext as DocDialogViewModel;
            vm.DialogTitle = "Добавление документа";
            vm.DialogAcceptButton = "Добавить";
            vm.DialogChoseButton = "Выбрать документ";
            // отображение окна
            var result = await DialogHost.Show(DocDialog, "RootDialog");
            if (result is bool res && res == true)
            {
                if (vm.DocList.Count > 0)
                {
                    foreach(Doc Doc in vm.DocList)
                    {
                        // название для документа
                        Doc.Title = Doc.Title + ".pdf";
                        // дата добавления
                        Doc.Date = DateTime.Now.ToString();
                        // добавляем новый объект в базу данных
                        db.Docs.Add(Doc);
                    }
                }
                // сохраняем изменения
                db.SaveChanges();
                DocsCount = Documents.Count();
                UpdTimeLims();
            }
        }
        // редактирование
        public ICommand OpenEditDialogCommand => new RelayCommand(EditDialog);

        private async void EditDialog(object o)
        {
            if (SelectedDocuments.Count() == 1)
            {
                // получаем выбранный документ
                Doc SelectedDoc = SelectedDocuments.FirstOrDefault();
                // создание диалогового окна формы добавления
                DocDialog DocDialog = new DocDialog();
                var pos = SelectedDoc.Title.LastIndexOf(".");
                var filename = SelectedDoc.Title.Substring(0, pos);
                // передаем выбранный объект в форму
                DocDialogViewModel DocVM = new DocDialogViewModel(new Doc
                {
                    Id = SelectedDoc.Id,
                    Title = filename,
                    Description = SelectedDoc.Description,
                    Bytes = SelectedDoc.Bytes,
                    Date = SelectedDoc.Date
                });
                // привязка нового объекта к контексту данных диалогового окна
                DocDialog.DataContext = DocVM;
                // отображение окна
                var result = await DialogHost.Show(DocDialog, "RootDialog");
                if (result is bool res && res == true)
                {
                    // находим выбраный документ в базе данных 
                    Doc Doc = db.Docs.Find(DocVM.Doc.Id);
                    if (Doc != null)
                    {
                        // изменяем инфомацию
                        Doc.Description = DocVM.Doc.Description;
                        Doc.Title = DocVM.Doc.Title + ".pdf";
                        Doc.Bytes = DocVM.Doc.Bytes;
                        // сообщаем о изменение документа
                        db.Entry(Doc).State = EntityState.Modified;
                        // сохраняем изменения
                        db.SaveChanges();
                        if (IsSearchOn)
                            UpdateSeachList();
                    }
                }
            }
        }
        // удаление


        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((o) =>
                  {
                      // если ни одного объекта не выделено, выходим
                      if (SelectedDocuments.Count() > 0)
                      {
                          // перебираем выбранные элементы
                          foreach(Doc doc in SelectedDocuments)
                          {
                              // находим элемент в базе данных
                              Doc order = db.Docs.Where(obj => obj.Id == doc.Id).FirstOrDefault();
                              // удаляем выделеный объект
                              db.Docs.Remove(order);
                          }
                          // сохраняем изменения
                          db.SaveChanges();
                          DocsCount = Documents.Count();
                          if (IsSearchOn)
                              UpdateSeachList();
                      }
                  }));
            }
        }
        // чтение
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand((o) =>
                  {
                      if (SelectedDocuments.Count() == 1)
                      {
                          // получаем выбранный элемент
                          Doc SelectedDoc = SelectedDocuments.FirstOrDefault();
                          // записываем массив байтов в файл
                          File.WriteAllBytes(SelectedDoc.Title, SelectedDoc.Bytes);
                          // запускаем файл
                          Process.Start(SelectedDoc.Title);
                      }
                  }));
            }
        }
        // копирование
        public RelayCommand CopyCommand
        {
            get
            {
                return copyCommand ??
                  (copyCommand = new RelayCommand((o) =>
                  {
                      // еслихотть однин объекта выделен
                      if(SelectedDocuments.Count() > 0)
                      {
                          StringCollection paths = new StringCollection();
                          foreach (Doc doc in SelectedDocuments)
                          {
                              // записываем массив байтов в файл
                              File.WriteAllBytes(doc.Title, doc.Bytes);
                              // записываем расположение выделенного файла 
                              paths.Add("G:\\Apps Folder\\LocalStorage\\bin\\Debug\\" + doc.Title);
                          }
                          // помещаем файл в системный буффер обмена 
                          Clipboard.SetFileDropList(paths);
                      }
                  }));
            }
        }

        private int docsCount;
        public int DocsCount
        {
            get { return docsCount; }
            set
            {
                docsCount = value;
                OnPropertyChanged("DocsCount");
            }
        }

        private int selectedDocsCount;
        public int SelectedDocsCount
        {
            get { return selectedDocsCount; }
            set
            {
                selectedDocsCount = value;
                OnPropertyChanged("SelectedDocsCount");
            }
        }

        private async void checkPassword()
        {
            // если пароль не задан
            if (!File.Exists("password"))
            {
                // создание диалогового окна формы добавления
                PassDialog passDialog = new PassDialog();
                PassDialogViewModel DocVM = new PassDialogViewModel("");
                // привязка нового объекта к контексту данных диалогового окна
                passDialog.DataContext = DocVM;
                PassDialogViewModel vm = passDialog.DataContext as PassDialogViewModel;
                // отображение окна
                var result = await DialogHost.Show(passDialog, "RootDialog");
                if (result is bool res && res == true)
                {
                    try
                    {
                        // создаем новый поток для записи в файл
                        FileStream fs = File.Create("password");
                        // инициализируем хэш-функцию
                        SHA256 sha256Hash = SHA256.Create();
                        // генерируем хэш-последовательность
                        string hash = GetHash(sha256Hash, vm.Password);
                        // преобразуем хэш-значение в массив байтов
                        byte[] info = new UTF8Encoding(true).GetBytes(hash);
                        // записываем массив байтов в файл
                        fs.Write(info, 0, info.Length);
                    }
                    catch { }
                }
            }
            // открываем файл содержащий пароль
            using (StreamReader sr = File.OpenText("password"))
            {
                // записываем данный из файла в переменную
                passHash = sr.ReadLine();
            }
        }

        private string adminPanelVisibility;
        public string AdminPanelVisibility
        {
            get { 
                if(adminPanelVisibility == null) { adminPanelVisibility = "Collapsed"; }
                return adminPanelVisibility; 
            }
            set
            {
                adminPanelVisibility = value;
                OnPropertyChanged("AdminPanelVisibility");
            }
        }

        private string passHash;

        public ICommand OpenPasswordDialogCommand => new RelayCommand(PasswordDialog);

        private async void PasswordDialog(object o)
        {
            if (o is bool check && check == true)
            {
                checkPassword();
                // создание диалогового окна формы добавления
                PassDialog passDialog = new PassDialog();
                PassDialogViewModel DocVM = new PassDialogViewModel("");
                // привязка нового объекта к контексту данных диалогового окна
                passDialog.DataContext = DocVM;
                PassDialogViewModel vm = passDialog.DataContext as PassDialogViewModel;
                // отображение окна
                var result = await DialogHost.Show(passDialog, "RootDialog");
                if (result is bool res && res == true)
                {
                    try
                    {
                        // инициализируем хэш-функцию
                        SHA256 sha256Hash = SHA256.Create();
                        // проверка на соответствия введенной строки и пароля
                        if (VerifyHash(sha256Hash, vm.Password, passHash))
                        {
                            AdminPanelVisibility = "Visible";
                        }
                    }
                    catch { }
                }
            }
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // конвертируем введенную строку в массив байтов и вычисляем hash
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            // создаем Stringbuilder для записи байтов в строку
            var sBuilder = new StringBuilder();
            // проходим все байты хэш-значения
            for (int i = 0; i < data.Length; i++)
            {
                // преобразовываем каждый байт в шестнадцатиричное значение
                sBuilder.Append(data[i].ToString("x2"));
            }
            // возвращаем строку
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            // хэшируем введенную строку
            var hashOfInput = GetHash(hashAlgorithm, input);
            // создаем StringComparer для сравнения хэшей
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            // возвращаем результат сравнения
            return comparer.Compare(hashOfInput, hash) == 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
