using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace MongoDBManagementStudio.ViewModel
{
    public class ItemViewModel : ViewModelBase
    {
        public IDictionary<string, string> Data { get; set; }
        private ObservableCollection<FieldViewModel> _headers;

        public ItemViewModel(ObservableCollection<FieldViewModel> headers)
        {
            Data = new Dictionary<string, string>();
            _headers = headers;
        }

        /// <summary>
        /// Data filled out and re-ordered to match header order
        /// </summary>
        public List<string> DataRow
        {
            get
            {
                var dataRow = new List<string>();

                foreach (FieldViewModel field in _headers)
                {
                    string fieldValue = string.Empty;

                    if (Data.ContainsKey(field.FieldName))
                        fieldValue = Data[field.FieldName];

                    dataRow.Add(fieldValue);
                }

                return dataRow;
            }
        }

        public ItemViewModel()
        {
            Data = new Dictionary<string, string>();
            Name = "this is a test";
            Description = "this is a test";
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
