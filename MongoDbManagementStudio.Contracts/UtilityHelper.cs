using System;
using System.Configuration;

namespace MongoDbManagementStudio.Contracts
{
    public static class UtilityHelper
    {
        private static QueryDriverTypes _queryType;

        /// <summary>
        /// Get default QueryDriverTypes from app settings 
        /// </summary>
        /// <returns></returns>
        public static QueryDriverTypes GetDefaultQueryDriverType()
        {
            if (_queryType == QueryDriverTypes.Unspecified)
            {
                var driverType = ConfigurationManager.AppSettings["QueryDriverType"];
                if (!String.IsNullOrEmpty(driverType))
                {
                    try
                    {
                        _queryType = (QueryDriverTypes)Enum.Parse(typeof(QueryDriverTypes), driverType);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Unrecognized QueryDriverType; default to CSharp.");
                    }
                }
                if (_queryType == QueryDriverTypes.Unspecified)
                    _queryType = QueryDriverTypes.CSharp;
            }
            return _queryType;
        }
    }
}