using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MongoDbManagementStudio.Contracts;

namespace MongoDbManagementStudio.QueryDriver.CSharp
{
    [ExportMetadata("DriverType", QueryDriverTypes.CSharp)]
    [Export(typeof(IMongoQueryFactory))]
    public class MongoDbCSharpQueryFactory : IMongoQueryFactory
    {
        private IMongoQuery _mongoQuery;

        [ImportMany]
        IEnumerable<Lazy<IMongoQuery, IDriverMetadata>> MongoQueryQueries { get; set; }

        private IMongoQuery MongoQuery
        {
            get
            {
                if (_mongoQuery == null)
                {
                    _mongoQuery = (from q in MongoQueryQueries
                                   where q.Metadata.DriverType == UtilityHelper.GetDefaultQueryDriverType()
                                   select q.Value).First();
                }
                return _mongoQuery;
            }
            set
            {
                _mongoQuery = value;
            }

        }

        public MongoDbCSharpQueryFactory()
        {
        }

        public MongoDbCSharpQueryFactory(IMongoQuery mongoQuery)
        {
            MongoQuery = mongoQuery;
        }

        public IMongoQuery BuildQuery()
        {
            return MongoQuery;
        }
    }
}