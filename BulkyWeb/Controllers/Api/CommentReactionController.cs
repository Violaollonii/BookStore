using Microsoft.AspNetCore.Mvc;
using BulkyBookWeb.MongoServices;

namespace BulkyBookWeb.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentReactionController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public CommentReactionController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpPost("react")]
        public async Task<IActionResult> React([FromBody] CommentReactionDto dto)
        {
            await _mongoService.AddOrUpdateReactionAsync(dto);
            return Ok();
        }

        [HttpGet("counts/{commentId}")]
        public async Task<IActionResult> GetCounts(string commentId)
        {
            var (likes, dislikes) = await _mongoService.GetReactionCountsAsync(commentId);
            return Ok(new { likes, dislikes });
        }


        [HttpGet("user-reaction/{commentId}/{userId}")]
        public async Task<IActionResult> GetUserReaction(string commentId, string userId)
        {
            var reaction = await _mongoService.GetUserReactionAsync(commentId, userId);
            return Ok(reaction);
        }


    }
}