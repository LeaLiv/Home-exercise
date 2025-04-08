using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PartD.Models;

public class Goods
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ItemId { get; set; }
    [BsonElement("Name")]
    public string Name { get; set; } 
    [BsonElement("minCount")]
    public int MinCount { get; set; } 
    [BsonElement("count")]
    public int Count { get; set; }
}