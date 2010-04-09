using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDBManagementStudio.Model
{
    public interface IMongoQuery : IDisposable
    {
        /// <summary>
        /// Returns a collection of DictionaryBase instances
        /// </summary>
        IEnumerable RunQuery(string server, string database, string port, string query);
    }
}