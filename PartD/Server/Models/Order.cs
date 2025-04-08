
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartD.Models;

namespace partD.Models
{
    public class Order
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        [BsonElement("SupplierId")]
        public string SupplierId { get; set; }
        [BsonElement("GrocerId")]
        public string GrocerId { get; set; } 
        [BsonElement("status")]
        public string status { get; set; } 
        [BsonElement("Products")]
        public Product[] Products { get; set; }
    }
}