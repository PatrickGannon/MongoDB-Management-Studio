using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;
using MongoDBManagementStudio.Model;
using MongoDBManagementStudio.Properties;

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
        public RelayCommand CopyToClipboardCommand { get; private set; }

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
            CopyToClipboardCommand = new RelayCommand(() => CopyDataToClipboard());
            
            Collections = new ObservableCollection<string>();
        }

        public string Server { get; set; }
        public string Database { get; set; }
        public string Query { get; set; }
        public string Port { get; set; }
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<FieldViewModel> Headers { get; private set; }
        //TODO: Remove these hack: default query factory and user message service
        private IMongoQueryFactory _mongoQueryFactory = new MongoDbCSharpQueryFactory();
        private IUserMessageService _userMessageService = new UserMessageService();
        private IClipboardService _clipboardService = new ClipboardService();
        public ObservableCollection<string> Collections { get; private set; }
        private string _numberOfResults = string.Empty;

        public string NumberOfResults 
        {
            get { return _numberOfResults; }
            private set
            {
                if (_numberOfResults == value)
                    return;

                var oldValue = _numberOfResults;
                _numberOfResults = value;

                RaisePropertyChanged("NumberOfResults", oldValue, value, true);
            }
        }

        public IMongoQueryFactory MongoQueryFactory
        {
            set { _mongoQueryFactory = value; }
        }

        public IUserMessageService UserMessageService
        {
            set { _userMessageService = value; }
        }

        public IClipboardService ClipboardService
        {
            set { _clipboardService = value; }
        }

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

                NumberOfResults = string.Format("{0}: {1} docs", DateTime.Now.ToString("HH:mm:ss"), Items.Count);

                ListViewExtension.RefreshUI(Headers);
            }
            catch (QueryValidationException ex)
            {
                _userMessageService.ShowMessage(ex.Message);
                return;
            }
            catch (UnknownServerException ex)
            {
                _userMessageService.ShowMessage(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                _userMessageService.ShowMessage("An error occurred while executing the query: " + ex);
                return;
            }
            finally
            {
                mongoQuery.Dispose();
            }
        }

        private void CopyDataToClipboard()
        {
            var builder = new StringBuilder();

            if (Items.Count == 0)
            {
                _userMessageService.ShowMessage("No results to copy - run a query first");
                return;
            }

            foreach (FieldViewModel header in Headers)
            {
                builder.Append(header.FieldName + "\t");
            }

            builder.Append(Environment.NewLine);

            foreach (ItemViewModel item in Items)
            {
                List<string> fields = item.DataRow;

                foreach (string field in fields)
                {
                    builder.Append(field + "\t");
                }

                builder.Append(Environment.NewLine);
            }

            _clipboardService.SetText(builder.ToString());
            _userMessageService.ShowMessage("Results copied to clipboard");
        }

        private void ShowCollections()
        {
            if (Database == string.Empty)
            {
                _userMessageService.ShowMessage("You must specify a non-empty database name");
                return;
            }

            IMongoQuery query = _mongoQueryFactory.BuildQuery();

            try
            {
                IList<string> collections = query.GetCollections(Server, Database, Port);

                Collections.Clear();

                foreach (string collection in collections)
                    Collections.Add(collection);
            }
            catch (UnknownServerException ex)
            {
                _userMessageService.ShowMessage(ex.Message);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}