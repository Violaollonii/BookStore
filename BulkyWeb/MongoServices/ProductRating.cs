using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BulkyBookWeb.Models
{
    public class ProductRating
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Mongo do e krijojë këtë automatikisht

        public string ProductId { get; set; }  // ID e produktit nga SQL si string
        public string UserId { get; set; }     // GUID nga tabela AspNetUsers
        public int RatingValue { get; set; }   // Yjet (1–5)
    }
}
