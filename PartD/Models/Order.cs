
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace partD.Models{
    public class Order{
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public string SupplierId { get; set; } // ID of the supplier
        public string GrocerId { get; set; } // ID of the grocer
        public string status { get; set; } // Status of the order (e.g., "Pending", "Completed")
        public Object[] Products { get; set; }  
    }
}