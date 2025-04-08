
using MongoDB.Bson;
using MongoDB.Driver;

namespace PartD.Services;
public class MongoService
{
    static MongoClient client = new MongoClient("mongodb+srv://leahl11730:Lea0195!@cluster0.qxme0.mongodb.net/");
    static IMongoDatabase database = client.GetDatabase("Groceries");

    public static async Task<List<T>> getCollection<T>(string CollectionName)
    {
        try
        {
            IMongoCollection<T> Collection = database.GetCollection<T>(CollectionName);
            return await Collection.Find(_ => true).ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
        return [];

    }

    public static async Task InsertOne<T>(T item, string collectionName)
    {
        IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);
        await Collection.InsertOneAsync(item);
    }

    public static async Task<UpdateResult> UpdateOne<T>(string _id, string value, string fieldName, string collectionName)
    {
        var objectId = new ObjectId(_id);
        IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", objectId); 
        var update = Builders<T>.Update.Set(fieldName, value); 
        var result = await Collection.UpdateOneAsync(filter, update);
        Console.WriteLine($"Updated {result.ModifiedCount} document(s).");
         return result;
    }

    public static async Task<T> GetWithFilter<T>(string collectionName, FilterDefinition<T> filter)
    {
        IMongoCollection<T> Collection = database.GetCollection<T>(collectionName);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }
}