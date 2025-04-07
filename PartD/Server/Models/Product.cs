

using MongoDB.Bson.Serialization.Attributes;

namespace PartD.Models
{
    public class Product
    {

        [BsonElement("productName")]
        public string productName { get; set; }
        [BsonElement("pricePerItem")]
        public decimal pricePerItem { get; set; }
        [BsonElement("minQuantity")]
        public int minQuantity { get; set; }


    }
}