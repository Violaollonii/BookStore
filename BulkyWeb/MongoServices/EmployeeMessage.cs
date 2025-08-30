using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BulkyBookWeb.MongoServices
{
    public class EmployeeMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // 👈 E LËNË e hapur për frontend, nuk është JsonIgnore

        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; }
        

    }
}
