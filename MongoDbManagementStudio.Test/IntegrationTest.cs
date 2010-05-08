using MongoDB.Driver;
using MongoDbManagementStudio.Contracts;
using MongoDbManagementStudio.QueryDriver.CSharp;
using MongoDBManagementStudio.ViewModel;
using NUnit.Framework;

namespace MongoDbManagementStudio.Test
{
    [TestFixture]
    public class IntegrationTest
    {
        [Test, Ignore("MongoDB server must be running locally for this test to pass")]
        public void ShouldGetDataFromLocalMongoDbServer()
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

            IMongoQuery query = new MongoDbCSharpQuery();
            IMongoQueryFactory factory = new MongoDbCSharpQueryFactory(query);

            MainViewModel viewModel = new MainViewModel(factory)
                                          {
                                              Server = "localhost",
                                              Database = "test",
                                              Port = "27017",
                                              Query = "folks:this.middle_initial == 'Q'"
                                          };

            try
            {
                //When
                viewModel.RunQueryCommand.Execute(null);

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
    }
}