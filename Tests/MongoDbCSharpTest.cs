using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDBManagementStudio.Model;
using NUnit.Framework;

namespace MongoDBManagementStudio.Tests
{
    [TestFixture, Ignore("MongoDB server must be running locally for these tests to pass")]
    public class MongoDbCSharpTest
    {
        [Test]
        public void ShouldReturnCollectionsFromDatabase()
        {
            //Given
            Mongo db = new Mongo(string.Format("Server={0}:{1}", "localhost", "27017"));
            db.Connect();
            IMongoCollection collection = db["test"]["folks"];
            Document doc1 = new Document() { { "first_name", "Bill" }, { "middle_initial", "Q" }, { "last_name", "Jackson" }, { "address", "744 Nottingham St." } };
            Document doc2 = new Document() { { "first_name", "Ralph" }, { "middle_initial", "M" }, { "last_name", "Buckingham" }, { "state", "CA" } };
            Document doc3 = new Document() { { "first_name", "Ronald" }, { "middle_initial", "Q" }, { "last_name", "Weasly" }, { "city", "Santa Rosa" } };
            collection.Insert(doc1);
            collection.Insert(doc2);
            collection.Insert(doc3);
            var queryProvider = new MongoDbCSharpQuery();

            try
            {
                //When
                IList<string> collections = queryProvider.GetCollections("localhost", "test", "27017");

                //Then
                Assert.IsNotNull(collections.Where(c => c == "folks").SingleOrDefault());
            }
            finally
            {
                //Clean Up
                collection.Delete(doc1);
                collection.Delete(doc2);
                collection.Delete(doc3);
                queryProvider.Dispose();
            }
        }

        [Test]
        public void ShouldReturnAllDataFromCollection()
        {
            //Given
            Mongo db = new Mongo(string.Format("Server={0}:{1}", "localhost", "27017"));
            db.Connect();
            IMongoCollection collection = db["test"]["folks"];
            Document doc1 = new Document() { { "first_name", "Bill" }, { "middle_initial", "Q" }, { "last_name", "Jackson" }, { "address", "744 Nottingham St." } };
            Document doc2 = new Document() { { "first_name", "Ralph" }, { "middle_initial", "M" }, { "last_name", "Buckingham" }, { "state", "CA" } };
            Document doc3 = new Document() { { "first_name", "Ronald" }, { "middle_initial", "Q" }, { "last_name", "Weasly" }, { "city", "Santa Rosa" } };
            collection.Insert(doc1);
            collection.Insert(doc2);
            collection.Insert(doc3);
            var queryProvider = new MongoDbCSharpQuery();

            try
            {
                //When
                IEnumerable data = queryProvider.RunQuery("localhost", "test", "27017", "folks");

                int numberOfRows = 0;
                foreach (var row in data) numberOfRows++;

                //Then
                Assert.AreEqual(3, numberOfRows);
            }
            finally
            {
                //Clean Up
                collection.Delete(doc1);
                collection.Delete(doc2);
                collection.Delete(doc3);
                queryProvider.Dispose();
            }
        }

        [Test]
        public void ShouldReturnLimitedDataFromCollection()
        {
            //Given
            Mongo db = new Mongo(string.Format("Server={0}:{1}", "localhost", "27017"));
            db.Connect();
            IMongoCollection collection = db["test"]["folks"];
            Document doc1 = new Document() { { "first_name", "Bill" }, { "middle_initial", "Q" }, { "last_name", "Jackson" }, { "address", "744 Nottingham St." } };
            Document doc2 = new Document() { { "first_name", "Ralph" }, { "middle_initial", "M" }, { "last_name", "Buckingham" }, { "state", "CA" } };
            Document doc3 = new Document() { { "first_name", "Ronald" }, { "middle_initial", "Q" }, { "last_name", "Weasly" }, { "city", "Santa Rosa" } };
            collection.Insert(doc1);
            collection.Insert(doc2);
            collection.Insert(doc3);
            var queryProvider = new MongoDbCSharpQuery();

            try
            {
                //When
                IEnumerable data = queryProvider.RunQuery("localhost", "test", "27017", "folks:this.middle_initial == 'Q' limit 1");

                int numberOfRows = 0;
                IDictionary doc = null;
                foreach (IDictionary row in data)
                {
                    doc = row;
                    numberOfRows++;
                }

                //Then
                Assert.AreEqual(1, numberOfRows);
                Assert.AreEqual("Jackson", doc["last_name"]);
            }
            finally
            {
                //Clean Up
                collection.Delete(doc1);
                collection.Delete(doc2);
                collection.Delete(doc3);
                queryProvider.Dispose();
            }
        }
    }
}
