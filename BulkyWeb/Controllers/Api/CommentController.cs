using Microsoft.AspNetCore.Mvc;
using BulkyBookWeb.MongoServices;

namespace BulkyBookWeb.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public CommentController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        // GET për komentet përkatëse
        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int productId)
        {
            var comments = await _mongoService.GetByProductIdAsync(productId);
            return Ok(comments);
        }

        // POST për dërgimin e komentit
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCommentDto commentDto)
        {
            var comment = new ProductComment
            {
                ProductId = commentDto.ProductId,
                UserName = commentDto.UserName,
                Text = commentDto.Text,
                CreatedAt = DateTime.Now
            };

            await _mongoService.AddAsync(comment);
            return Ok(comment);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string userName)
        {
            var success = await _mongoService.DeleteCommentIfAuthorAsync(id, userName);

            if (!success)
                return Unauthorized(new { message = "Nuk mund të fshini këtë koment. Nuk jeni autori!" });

            return Ok(new { message = "Koment u fshi me sukses." });
        }

    }

}
