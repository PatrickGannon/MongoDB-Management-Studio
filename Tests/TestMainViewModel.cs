using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDBManagementStudio.Model;
using MongoDBManagementStudio.ViewModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace MongoDBManagementStudio.Tests
{
    [TestFixture]
    public class TestMainViewModel
    {
        [Test, Ignore("MongoDB server must be running locally for this test to pass")]
        public void ShouldGetDataFromLocalMongoDbServer()
        {
            //Given
            Mongo db = new Mongo(string.Format("Server={0}:{1}", "localhost", "27017"));
            db.Connect();
            IMongoCollection collection = db["test"]["folks"];
            Document doc1 = new Document() {{"first_name", "Bill"}, {"middle_initial", "Q"}, {"last_name", "Jackson"}, {"address", "744 Nottingham St."}};
            Document doc2 = new Document() { { "first_name", "Ralph" }, { "middle_initial", "M" }, { "last_name", "Buckingham" }, {"state", "CA"}};
            Document doc3 = new Document() { { "first_name", "Ronald" }, { "middle_initial", "Q" }, { "last_name", "Weasly" }, {"city", "Santa Rosa"}};
            collection.Insert(doc1);
            collection.Insert(doc2);
            collection.Insert(doc3);

            MainViewModel viewModel = new MainViewModel()
                                          {
                                              Server = "localhost", Database = "test", Port = "27017",
                                              Query = "folks:this.middle_initial == 'Q'"
                                          };

            try
            {
                //When
                viewModel.ButtonCommand.Execute(null);

                //Then
                Assert.AreEqual(2, viewModel.Items.Count);
                Assert.AreEqual(6, viewModel.Headers.Count); //including _id
            }
            finally
            {
                //Clean Up
                collection.Delete(doc1);
                collection.Delete(doc2);
                collection.Delete(doc3);
            }
        }

        [Test]
        public void ShouldPullInDataFromQueryProvider()
        {
            //Given
            DictionaryBase doc1 = new Document() { { "first_name", "Bill" }, { "middle_initial", "Q" }, { "last_name", "Jackson" }, { "address", "744 Nottingham St." } };
            DictionaryBase doc3 = new Document() { { "first_name", "Ronald" }, { "middle_initial", "Q" }, { "last_name", "Weasly" }, { "city", "Santa Rosa" } };
            IList<DictionaryBase> documents = new List<DictionaryBase>() {doc1, doc3};
            IMongoQuery query = MockRepository.GenerateStub<IMongoQuery>();
            query.Stub(q => q.RunQuery("localhost", "test", "27017", "folks:this.middle_initial == 'Q'")).Return(documents);
            IMongoQueryFactory queryFactory = MockRepository.GenerateStub<IMongoQueryFactory>();
            queryFactory.Stub(factory => factory.BuildQuery()).Return(query);

            MainViewModel viewModel = new MainViewModel()
            {
                Server = "localhost", Database = "test", Port = "27017",
                Query = "folks:this.middle_initial == 'Q'",
                MongoQueryFactory = queryFactory
            };

            //When
            viewModel.ButtonCommand.Execute(null);

            //Then
            Assert.AreEqual(2, viewModel.Items.Count);
            Assert.AreEqual(5, viewModel.Headers.Count);
        }
    }
}
