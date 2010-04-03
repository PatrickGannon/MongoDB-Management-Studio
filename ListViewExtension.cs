using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MongoDBManagementStudio.ViewModel;

namespace MongoDBManagementStudio
{
    public class ListViewExtension
    {
        public static readonly DependencyProperty MatrixSourceProperty =
            DependencyProperty.RegisterAttached("MatrixSource",
                typeof(ObservableCollection<FieldViewModel>), 
                typeof(ListViewExtension),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMatrixSourceChanged)));

        private static GridView _gridView;

        public static ObservableCollection<FieldViewModel> GetMatrixSource(DependencyObject d)
        {
            return (ObservableCollection<FieldViewModel>)d.GetValue(MatrixSourceProperty);
        }

        public static void SetMatrixSource(DependencyObject d, ObservableCollection<FieldViewModel> value)
        {
            d.SetValue(MatrixSourceProperty, value);
        }

        private static void OnMatrixSourceChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ObservableCollection<FieldViewModel> headers = e.NewValue as ObservableCollection<FieldViewModel>;

            ListView listView = d as ListView;
            //listView.ItemsSource = dataMatrix;
            _gridView = listView.View as GridView;

            if (_gridView == null)
                listView.View = _gridView = new GridView();

            RefreshUI(headers);
        }

        public static void RefreshUI(ObservableCollection<FieldViewModel> headers)
        {
            int count = 0;
            _gridView.Columns.Clear();
            foreach (FieldViewModel header in headers)
            {
                _gridView.Columns.Add(
                    new GridViewColumn
                    {
                        Header = header.FieldName,
                        DisplayMemberBinding = new Binding(string.Format("DataRow[{0}]", count))
                    });
                count++;
            }
        }
    }
}
