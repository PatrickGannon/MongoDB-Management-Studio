using System;

namespace MongoDbManagementStudio.Contracts
{
    public class UnknownServerException : ApplicationException
    {
        public UnknownServerException(string message, Exception inner) : base(message, inner) {}
        public UnknownServerException(string message) : base(message) { }
    }
}