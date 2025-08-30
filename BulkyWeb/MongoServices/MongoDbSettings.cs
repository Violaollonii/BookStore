namespace BulkyBookWeb.MongoServices
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CommentCollectionName { get; set; }
        public string RatingCollectionName { get; set; }

        public string EmployeeMessageCollectionName { get; set; }
        public string ReactionCollectionName { get; set; }

    }

}
