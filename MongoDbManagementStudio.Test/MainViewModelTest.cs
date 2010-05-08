using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDbManagementStudio.Contracts;
using MongoDBManagementStudio.ViewModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace MongoDbManagementStudio.Test
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

            MainViewModel viewModel = new MainViewModel(queryFactory)
                                          {
                                              Server = "localhost", Database = "test", Port = "27017",
                                              Query = "folks:this.middle_initial == 'Q'",
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

            MainViewModel viewModel = new MainViewModel(queryFactory)
                                          {
                                              Server = "localhost",
                                              Database = "test",
                                              Port = "27017",
                                              Query = "folks:this.middle_initial == 'Q'",
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

            MainViewModel viewModel = new MainViewModel(queryFactory)
                                          {
                                              Server = "localhost",
                                              Database = "",
                                              Port = "27017",
                                              Query = "folks:this.middle_initial == 'Q'",
                                              UserMessageService = messageService
                                          };

            //When
            viewModel.ShowCollectionsCommand.Execute(null);

            //Then
            messageService.AssertWasCalled(service => service.ShowMessage("You must specify a non-empty database name"));
        }

        [Test]
        public void ShouldCopyDataToClipboard()
        {
            //Given
            DictionaryBase doc1 = new Document() { { "first_name", "Bill" }, { "middle_initial", "Q" }, { "last_name", "Jackson" }, { "address", "744 Nottingham St." } };
            DictionaryBase doc3 = new Document() { { "first_name", "Ronald" }, { "middle_initial", "Q" }, { "last_name", "Weasly" }, { "city", "Santa Rosa" } };
            IList<DictionaryBase> documents = new List<DictionaryBase>() { doc1, doc3 };
            IMongoQuery query = MockRepository.GenerateStub<IMongoQuery>();
            query.Stub(q => q.RunQuery("localhost", "test", "27017", "folks:this.middle_initial == 'Q'")).Return(documents);
            IMongoQueryFactory queryFactory = MockRepository.GenerateStub<IMongoQueryFactory>();
            queryFactory.Stub(factory => factory.BuildQuery()).Return(query);
            IClipboardService clipboardService = MockRepository.GenerateMock<IClipboardService>();
            IUserMessageService messageService = MockRepository.GenerateMock<IUserMessageService>();

            MainViewModel viewModel = new MainViewModel(queryFactory)
                                          {
                                              Server = "localhost",
                                              Database = "test",
                                              Port = "27017",
                                              Query = "folks:this.middle_initial == 'Q'",
                                              ClipboardService = clipboardService,
                                              UserMessageService = messageService
                                          };

            //When
            viewModel.RunQueryCommand.Execute(null);
            viewModel.CopyToClipboardCommand.Execute(null);

            //Then
            clipboardService.AssertWasCalled(clipboard => clipboard.SetText(
                                                              "last_name\tfirst_name\tmiddle_initial\taddress\tcity\t\r\nJackson\tBill\tQ\t744 Nottingham St.\t\t\r\nWeasly\tRonald\tQ\t\tSanta Rosa\t\r\n"));
            messageService.AssertWasCalled(service => service.ShowMessage("Results copied to clipboard"));
        }
    }
}