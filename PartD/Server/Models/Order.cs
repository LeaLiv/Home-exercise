
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
        public string SupplierId { get; set; } // ID of the supplier
        [BsonElement("GrocerId")]
        public string GrocerId { get; set; } // ID of the grocer
        [BsonElement("status")]
        public string status { get; set; } // Status of the order (e.g., "Pending", "Completed")
        [BsonElement("Products")]
        public Product[] Products { get; set; }
    }
}