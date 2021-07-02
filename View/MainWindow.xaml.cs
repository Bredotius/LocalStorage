using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LocalStorage
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }

        private void documentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = (MainViewModel)DataContext;
            viewmodel.SelectedDocuments = documentsList.SelectedItems.Cast<Doc>().ToList();
            viewmodel.SelectedDocsCount = documentsList.SelectedItems.Count;

            if (documentsList.SelectedItems.Count == 1)
            {
                Open.Visibility = Visibility.Visible;
                Edit.Visibility = Visibility.Visible;
                Delete.Visibility = Visibility.Visible;
                Copy.Visibility = Visibility.Visible;
                CheckAll.IsChecked = false;
            }
            if (documentsList.SelectedItems.Count > 1)
            {
                Open.Visibility = Visibility.Collapsed;
                Edit.Visibility = Visibility.Collapsed;
            }
            if (documentsList.SelectedItems.Count == 0)
            {
                Open.Visibility = Visibility.Collapsed;
                Edit.Visibility = Visibility.Collapsed;
                Delete.Visibility = Visibility.Collapsed;
                Copy.Visibility = Visibility.Collapsed;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            documentsList.SelectAll();
            Delete.Visibility = Visibility.Visible;
            Copy.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            documentsList.UnselectAll();
        }
    }
}