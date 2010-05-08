using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MongoDBManagementStudio.ViewModel;

namespace MongoDBManagementStudio
{
    /// <summary>
    /// This application's main window.
    /// </summary>
    /// 
    [Export]
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        [Import]
        public MainViewModel ViewModel
        {
            get { return (MainViewModel) DataContext; }
            set { DataContext = value;}
        }
    }
}