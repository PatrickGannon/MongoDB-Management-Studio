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
    public class MainViewModelTest
    {
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
            viewModel.RunQueryCommand.Execute(null);

            //Then
            Assert.AreEqual(2, viewModel.Items.Count);
            Assert.AreEqual(5, viewModel.Headers.Count);
        }

        [Test]
        public void ShouldShowCollections()
        {
            //Given
            var collections = new List<string>() {"collection1", "collection2"};
            IMongoQuery query = MockRepository.GenerateStub<IMongoQuery>();
            query.Stub(q => q.GetCollections("localhost", "test", "27017")).Return(collections);
            IMongoQueryFactory queryFactory = MockRepository.GenerateStub<IMongoQueryFactory>();
            queryFactory.Stub(factory => factory.BuildQuery()).Return(query);

            MainViewModel viewModel = new MainViewModel()
            {
                Server = "localhost",
                Database = "test",
                Port = "27017",
                Query = "folks:this.middle_initial == 'Q'",
                MongoQueryFactory = queryFactory
            };

            //When
            viewModel.ShowCollectionsCommand.Execute(null);

            //Then
            Assert.AreEqual(2, viewModel.Collections.Count);
        }

        [Test]
        public void ShouldShowErrorWhenShowingCollectionsWithNoDatabaseSpecified()
        {
            //Given
            IMongoQueryFactory queryFactory = MockRepository.GenerateStub<IMongoQueryFactory>();
            IUserMessageService messageService = MockRepository.GenerateMock<IUserMessageService>();

            MainViewModel viewModel = new MainViewModel()
            {
                Server = "localhost",
                Database = "",
                Port = "27017",
                Query = "folks:this.middle_initial == 'Q'",
                MongoQueryFactory = queryFactory,
                UserMessageService = messageService
            };

            //When
            viewModel.ShowCollectionsCommand.Execute(null);

            //Then
            messageService.AssertWasCalled(service => service.ShowMessage("You must specify a non-empty database name"));
        }
    }
}
