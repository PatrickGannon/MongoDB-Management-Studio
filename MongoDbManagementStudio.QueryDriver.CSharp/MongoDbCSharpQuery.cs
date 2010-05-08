using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using MongoDB.Driver;
using MongoDbManagementStudio.Contracts;

namespace MongoDbManagementStudio.QueryDriver.CSharp
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("DriverType", QueryDriverTypes.CSharp)]
    [Export(typeof(IMongoQuery))]
    public class MongoDbCSharpQuery : IMongoQuery
    {
        private ICursor _cursor;
        private Mongo _db;

        public IEnumerable RunQuery(string server, string database, string port, string query)
        {
            if (database == string.Empty)
                throw new QueryValidationException("You must specify a non-empty database name");

            if (query == string.Empty)
                throw new QueryValidationException("You must specify a non-empty query");

            _db = new Mongo(string.Format("Server={0}:{1}", server, port));
            IList<DictionaryBase> documents = new List<DictionaryBase>();

            try
            {
                _db.Connect();
            }
            catch (SocketException ex)
            {
                throw new UnknownServerException(string.Format("Unknown server: {0}:{1}", server, port), ex);
            }

            string[] queryParts = query.Split(':');
            string collection = queryParts[0];

            if (queryParts.Length > 1)
            {
                Document spec = new Document();
                string where = queryParts[1];
                const string LIMIT_TEXT = " limit ";
                int limitIndex = where.IndexOf(LIMIT_TEXT);
                int limit = 0;

                if (limitIndex > -1)
                {
                    string limitText;

                    if (int.TryParse(where.Substring(limitIndex + LIMIT_TEXT.Length), out limit))
                        where = where.Substring(0, limitIndex);
                }

                spec.Append("$where", new Code(where));
                _cursor = _db[database][collection].Find(spec, limit, 0);
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

            try
            {
                _db.Connect();
            }
            catch (SocketException ex)
            {
                throw new UnknownServerException(string.Format("Unknown server: {0}:{1}", server, port), ex);
            }

            IList<String> collectionNames = _db[database].GetCollectionNames();
            var filteredCollections = new List<string>();
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