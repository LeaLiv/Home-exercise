
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PartD.Models
{
    public class Supplier
    {
        [BsonRepresentation(BsonType.ObjectId)]
        // [BsonId]
        public string _id { get; set; }
        [BsonElement("companyName")]
        public string CompanyName { get; set; }
        [BsonElement("phone")]
        public string Phone { get; set; }
        [BsonElement("contactPerson")]
        public string ContactPerson { get; set; }
        [BsonElement("products")]
        public Product[] Products { get; set; } // List of goods supplied by the supplier

    }
}