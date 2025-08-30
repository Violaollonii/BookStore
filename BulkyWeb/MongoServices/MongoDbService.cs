using MongoDB.Driver;
using Microsoft.Extensions.Options;
using BulkyBookWeb.Models;

namespace BulkyBookWeb.MongoServices
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ProductComment> _comments;
        private readonly IMongoCollection<ProductRating> _ratings;
        private readonly IMongoCollection<EmployeeMessage> _employeeMessages;
        private readonly IMongoCollection<CommentReaction> _commentReactions; // ✅ Shtuar

        public MongoDbService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            _comments = _database.GetCollection<ProductComment>(settings.Value.CommentCollectionName);
            _ratings = _database.GetCollection<ProductRating>(settings.Value.RatingCollectionName);
            _employeeMessages = _database.GetCollection<EmployeeMessage>(settings.Value.EmployeeMessageCollectionName);
            _commentReactions = _database.GetCollection<CommentReaction>("CommentReactions"); // ✅ Inicializim
        }

        public async Task<List<ProductComment>> GetAllAsync()
        {
            return await _comments.Find(_ => true).ToListAsync();
        }

        public async Task<List<ProductComment>> GetByProductIdAsync(int productId)
        {
            return await _comments.Find(c => c.ProductId == productId).ToListAsync();
        }

        public async Task AddAsync(ProductComment comment)
        {
            await _comments.InsertOneAsync(comment);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _comments.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UpdateAsync(string id, ProductComment updatedComment)
        {
            var result = await _comments.ReplaceOneAsync(c => c.Id == id, updatedComment);
            return result.ModifiedCount > 0;
        }

        public async Task AddRatingAsync(ProductRating rating)
        {
            await _ratings.InsertOneAsync(rating);
        }

        public async Task<List<ProductRating>> GetRatingsByProductIdAsync(string productId)
        {
            return await _ratings.Find(r => r.ProductId == productId).ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(string productId)
        {
            var ratings = await _ratings.Find(r => r.ProductId == productId).ToListAsync();
            return ratings.Count > 0 ? ratings.Average(r => r.RatingValue) : 0;
        }

        // EmployeeMessage Methods
        public async Task AddEmployeeMessageAsync(EmployeeMessage message)
        {
            await _employeeMessages.InsertOneAsync(message);
        }

        public async Task<List<EmployeeMessage>> GetAllEmployeeMessagesAsync()
        {
            return await _employeeMessages.Find(_ => true).ToListAsync();
        }

        public async Task<bool> DeleteEmployeeMessageAsync(string id)
        {
            var collection = _database.GetCollection<EmployeeMessage>("EmployeeMessage");
            var result = await collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteCommentIfAuthorAsync(string commentId, string userName)
        {
            var comment = await _comments.Find(c => c.Id == commentId).FirstOrDefaultAsync();

            if (comment == null || comment.UserName != userName)
                return false;

            var result = await _comments.DeleteOneAsync(c => c.Id == commentId);
            return result.DeletedCount > 0;
        }

        // Like/Dislike Methods
        // Like/Dislike Methods
        public async Task AddOrUpdateReactionAsync(CommentReactionDto dto)
        {
            var filter = Builders<CommentReaction>.Filter.Where(r => r.CommentId == dto.CommentId && r.UserId == dto.UserId);
            var existing = await _commentReactions.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                if (existing.ReactionType == dto.ReactionType)
                {
                    await _commentReactions.DeleteOneAsync(filter);
                }
                else
                {
                    var update = Builders<CommentReaction>.Update.Set(r => r.ReactionType, dto.ReactionType);
                    await _commentReactions.UpdateOneAsync(filter, update);
                }
            }
            else
            {
                await _commentReactions.InsertOneAsync(new CommentReaction
                {
                    CommentId = dto.CommentId,
                    UserId = dto.UserId,
                    ReactionType = dto.ReactionType
                });
            }
        }

        public async Task<(int Likes, int Dislikes)> GetReactionCountsAsync(string commentId)
        {
            var likeFilter = Builders<CommentReaction>.Filter.Where(r => r.CommentId == commentId && r.ReactionType == "like");
            var dislikeFilter = Builders<CommentReaction>.Filter.Where(r => r.CommentId == commentId && r.ReactionType == "dislike");

            var likes = (int)await _commentReactions.CountDocumentsAsync(likeFilter);
            var dislikes = (int)await _commentReactions.CountDocumentsAsync(dislikeFilter);

            return (likes, dislikes);
        }

        public async Task<string?> GetUserReactionAsync(string commentId, string userId)
        {
            var filter = Builders<CommentReaction>.Filter.Where(r => r.CommentId == commentId && r.UserId == userId);
            var reaction = await _commentReactions.Find(filter).FirstOrDefaultAsync();
            return reaction?.ReactionType;
        }

    }
}
