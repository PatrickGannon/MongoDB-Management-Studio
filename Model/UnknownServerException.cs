using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDBManagementStudio.Model
{
    public class UnknownServerException : ApplicationException
    {
        public UnknownServerException(string message, Exception inner) : base(message, inner) {}
        public UnknownServerException(string message) : base(message) { }
    }
}
