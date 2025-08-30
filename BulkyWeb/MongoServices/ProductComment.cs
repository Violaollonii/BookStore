using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BulkyBookWeb.MongoServices
{
    public class ProductComment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        // HEQIM këtë rresht:
        // [System.Text.Json.Serialization.JsonIgnore]
        public string Id { get; set; }

        public int ProductId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
