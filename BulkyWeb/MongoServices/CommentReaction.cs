using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BulkyBookWeb.MongoServices
{
    public class CommentReaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CommentId { get; set; } = null!; // ⬅️ Kjo ishte int, tani bëje string
        public string UserId { get; set; }
        public string ReactionType { get; set; }
    }
}
