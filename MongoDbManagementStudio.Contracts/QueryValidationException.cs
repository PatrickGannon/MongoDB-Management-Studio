using System;

namespace MongoDbManagementStudio.Contracts
{
    public class QueryValidationException : ApplicationException
    {
        public QueryValidationException(string message) : base(message) {}
    }
}