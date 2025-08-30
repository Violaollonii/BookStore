namespace BulkyBookWeb.MongoServices
{
    public class CommentReactionDto
    {

        public string CommentId { get; set; } = null!;

        public string UserId { get; set; }
        public string ReactionType { get; set; }
    }
}
