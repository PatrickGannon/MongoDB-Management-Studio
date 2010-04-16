using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace MongoDBManagementStudio.Model
{
    public class MongoDbCSharpQuery : IMongoQuery
    {
        private ICursor _cursor = null;
        private Mongo _db = null;

        public IEnumerable RunQuery(string server, string database, string port, string query)
        {
            if (database == string.Empty)
                throw new QueryValidationException("You must specify a non-empty database name");

            if (query == string.Empty)
                throw new QueryValidationException("You must specify a non-empty query");

            _db = new Mongo(string.Format("Server={0}:{1}", server, port));
            IList<DictionaryBase> documents = new List<DictionaryBase>();

            _db.Connect();

            string[] queryParts = query.Split(':');
            string collection = queryParts[0];

            if (queryParts.Length > 1)
            {
                string where = queryParts[1];

                _cursor = _db[database][collection].Find(where);
            }
            else
                _cursor = _db[database][collection].FindAll();

            return _cursor.Documents;
            //Document d = db[database].SendCommand("db.test.find();");
        }

        public void Dispose()
        {
            if (_cursor != null)
                _cursor.Dispose();

            if (_db != null)
                _db.Disconnect();
        }

        public IList<string> GetCollections(string server, string database, string port)
        {
            _db = new Mongo(string.Format("Server={0}:{1}", server, port));
            _db.Connect();

            IList<String> collectionNames = _db[database].GetCollectionNames();

            //Reconsider? Just because you can do something with one line of code doesn't mean you should (remove database prefixes)
            //IList<String> ret = collectionNames.Select(
            //    (c, i) => c.IndexOf(".") > 1 && c.IndexOf(".") != (c.Length - 1) ? c.Substring(c.IndexOf(".") + 1) : c).ToList();

            List<String> filteredCollections = new List<string>();
            var hiddenCollectionCriteria = new string[] {"cubicle", "tmp.", ".$", "system.indexes"};

            foreach (string collectionName in collectionNames)
            {
                if (!hiddenCollectionCriteria.Any(criteria => collectionName.Contains(criteria)))
                {
                    int periodIndex = collectionName.IndexOf(".");
                    string collection = collectionName;

                    if (periodIndex >= 0 && periodIndex != (collectionName.Length - 1))
                        collection = collectionName.Substring(collectionName.IndexOf(".") + 1);

                    filteredCollections.Add(collection);
                }
            }

            return filteredCollections;
        }
    }
}
