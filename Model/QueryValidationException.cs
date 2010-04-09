using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDBManagementStudio.Model
{
    public class QueryValidationException : ApplicationException
    {
        public QueryValidationException(string message) : base(message) {}
    }
}