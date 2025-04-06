
using MongoDB.Driver;

namespace PartD.Services;

public class MongoService
{
    public static async Task<List<T>> getCollection<T>(string CollectionName)
    {
         MongoClient client = new MongoClient("mongodb+srv://leahl11730:Lea0195!@cluster0.qxme0.mongodb.net/");
        var database = client.GetDatabase("Groceries");
        IMongoCollection<T> Collection = database.GetCollection<T>(CollectionName);
        return await Collection.Find(_ => true).ToListAsync();
    }

    public static async
}