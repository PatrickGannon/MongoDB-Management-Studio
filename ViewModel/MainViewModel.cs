using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;

namespace MongoDBManagementStudio.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public string Welcome
        {
            get
            {
                return "Welcome to MVVM Light";
            }
        }

        public RelayCommand ButtonCommand
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            Items = new ObservableCollection<ItemViewModel>();
            Query = string.Empty;
            Server = "localhost";
            Port = "27017";
            Database = string.Empty;
            Headers = new ObservableCollection<FieldViewModel>();
                          //{
                          //    new FieldViewModel() {FieldName = "a"}
                          //};

            this.ButtonCommand = new RelayCommand(() =>
                                                      {
                                                          RunQuery();
                                                      }
            );
        }

        public ObservableCollection<ItemViewModel> Items
        {
            get; private set;
        }

        public string Server { get; set; }
        public string Database { get; set; }
        public string Query { get; set; }
        public string Port { get; set; }
        public ObservableCollection<FieldViewModel> Headers { get; private set; }

        private void RunQuery()
        {
            if (Database == string.Empty)
            {
                MessageBox.Show("You must specify a database");
                return;
            }

            if (Query == string.Empty)
            {
                MessageBox.Show("You must specify a query");
                return;
            }

            Mongo db = new Mongo(string.Format("Server={0}:{1}", Server, Port));

            db.Connect();

            //Document query = new Document(); 
            //query["field1"] = 10; 

            ICursor cursor = null;
            try
            {
                string database = Database;

                string[] queryParts = Query.Split(':');

                if (queryParts.Length < 2)
                {
                    MessageBox.Show("Queries must be in the format: {collection}:{where} where {collection} is the name of your collection and {where} is your javascript query condition");
                    return;
                }

                string collection = queryParts[0];
                string query = queryParts[1];
                string where = query;
                cursor = db[database][collection].Find(where);
                //Document d = db[database].SendCommand("db.test.find();");
                Headers.Clear();
                Items.Clear();

                foreach (Document document in cursor.Documents)
                {
                    ItemViewModel model = new ItemViewModel(Headers); // {Name = "my test"};

                    foreach (string key in document.Keys)
                    {
                        //if (!Headers.Contains(key))
                        //    Headers.Add(key);
                        string k = key;
                        if (Headers.SingleOrDefault(field => field.FieldName == k) == null)
                            Headers.Add(new FieldViewModel() {FieldName = key} );

                        model.Data[key] = document[key].ToString();
                    }

                    //Items.Add(new ItemViewModel() { Name = "Query results", Description = document["a"].ToString() });

                    Items.Add(model);
                }
                //MessageBox.Show("Headers: " + headers.Count);
                ListViewExtension.RefreshUI(Headers);
            }
            finally
            {
                if (cursor != null)
                    cursor.Dispose();

                db.Disconnect();
            }

        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}