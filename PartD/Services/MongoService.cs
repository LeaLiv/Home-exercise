
using MongoDB.Driver;

namespace PartD.Services;

public class MongoService
{ 
    static MongoClient client = new MongoClient("mongodb+srv://leahl11730:Lea0195!@cluster0.qxme0.mongodb.net/");
       static IMongoDatabase database = client.GetDatabase("Groceries");
    public static async Task<List<T>> getCollection<T>(string CollectionName)
    {
        
        IMongoCollection<T> Collection = database.GetCollection<T>(CollectionName);
        return await Collection.Find(_ => true).ToListAsync();
    }

    public static async Task InsertOne<T>(T item ,string collectionName){
        IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);
        await Collection.InsertOneAsync(item);
    }
}