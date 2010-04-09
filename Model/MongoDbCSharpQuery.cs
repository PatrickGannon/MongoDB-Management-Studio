using System;
using System.Collections;
using System.Collections.Generic;
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

            if (queryParts.Length < 2)
                throw new QueryValidationException("Queries must be in the format: {collection}:{where} where {collection} is the name of your collection and {where} is your javascript query condition");

            string collection = queryParts[0];
            string where = queryParts[1];;
            _cursor = _db[database][collection].Find(where);
            
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
    }
}
