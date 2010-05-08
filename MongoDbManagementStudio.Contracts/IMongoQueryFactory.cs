namespace MongoDbManagementStudio.Contracts
{
    public interface IMongoQueryFactory
    {
        IMongoQuery BuildQuery();
    }
}