using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDBManagementStudio.Model
{
    public interface IMongoQuery : IDisposable
    {
        /// <summary>
        /// Returns a collection of DictionaryBase instances representing the data returned from the query
        /// </summary>
        IEnumerable RunQuery(string server, string database, string port, string query);
        /// <summary>
        /// Returns the collection names present in the supplied database
        /// </summary>
        IList<string> GetCollections(string server, string database, string port);
    }
}