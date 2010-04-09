using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDBManagementStudio.Model
{
    public class MongoDbCSharpQueryFactory : IMongoQueryFactory
    {
        public IMongoQuery BuildQuery()
        {
            return new MongoDbCSharpQuery();
        }
    }
}
