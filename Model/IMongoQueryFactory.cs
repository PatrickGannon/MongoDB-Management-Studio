using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDBManagementStudio.Model
{
    public interface IMongoQueryFactory
    {
        IMongoQuery BuildQuery();
    }
}
