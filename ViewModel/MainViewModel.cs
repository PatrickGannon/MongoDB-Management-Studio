using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;
using MongoDBManagementStudio.Model;

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
        public RelayCommand RunQueryCommand { get; private set; }
        public RelayCommand ShowCollectionsCommand { get; private set; }

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
            RunQueryCommand = new RelayCommand(() => { RunQuery(); });
            ShowCollectionsCommand = new RelayCommand(() => ShowCollections() );
            
            //TODO: Remove this hack: default query factory
            MongoQueryFactory = new MongoDbCSharpQueryFactory();
            Collections = new ObservableCollection<string>();
        }

        public ObservableCollection<ItemViewModel> Items
        {
            get; private set;
        }

        public IMongoQueryFactory MongoQueryFactory
        {
            set
            {
                _mongoQueryFactory = value;
            }
        }

        public string Server { get; set; }
        public string Database { get; set; }
        public string Query { get; set; }
        public string Port { get; set; }
        public ObservableCollection<FieldViewModel> Headers { get; private set; }
        private IMongoQueryFactory _mongoQueryFactory = null;
        public ObservableCollection<string> Collections { get; private set; }

        private void RunQuery()
        {
            IMongoQuery mongoQuery = _mongoQueryFactory.BuildQuery();
            IEnumerable documents = null;

            try
            {
                documents = mongoQuery.RunQuery(Server, Database, Port, Query);

                Headers.Clear();
                Items.Clear();

                foreach (IDictionary document in documents)
                {
                    ItemViewModel model = new ItemViewModel(Headers);

                    foreach (string key in document.Keys)
                    {
                        string k = key;
                        if (Headers.SingleOrDefault(field => field.FieldName == k) == null)
                            Headers.Add(new FieldViewModel() {FieldName = key});

                        model.Data[key] = document[key].ToString();
                    }

                    Items.Add(model);
                }

                ListViewExtension.RefreshUI(Headers);
            }
            catch (QueryValidationException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while executing the query: " + ex);
                return;
            }
            finally
            {
                mongoQuery.Dispose();
            }
        }

        private void ShowCollections()
        {
            IMongoQuery query = _mongoQueryFactory.BuildQuery();
            IList<string> collections = query.GetCollections(Server, Database, Port);

            Collections.Clear();

            foreach (string collection in collections)
                Collections.Add(collection);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}