using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BulkyBookWeb.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace BulkyBookWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IMongoCollection<ProductRating> _ratingsCollection;

        public RatingController(IMongoClient mongoClient)
        {
            // PËRPUTHET me MongoDB që ti po përdor
            var database = mongoClient.GetDatabase("BulkyBookComments");
            _ratingsCollection = database.GetCollection<ProductRating>("ProductRatings");
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitRating([FromBody] ProductRatingDto ratingDto)
        {
            try
            {
                // Print log për testim
                Console.WriteLine("🟢 Hyri në SubmitRating:");
                Console.WriteLine($"ProductId: {ratingDto.ProductId}, UserId: {ratingDto.UserId}, Rating: {ratingDto.RatingValue}");

                if (ratingDto == null || string.IsNullOrEmpty(ratingDto.UserId) || string.IsNullOrEmpty(ratingDto.ProductId))
                    return BadRequest("Të dhënat e vlerësimit janë të pavlefshme.");

                var filter = Builders<ProductRating>.Filter.And(
                    Builders<ProductRating>.Filter.Eq(r => r.ProductId, ratingDto.ProductId),
                    Builders<ProductRating>.Filter.Eq(r => r.UserId, ratingDto.UserId)
                );

                var existingRating = await _ratingsCollection.Find(filter).FirstOrDefaultAsync();

                if (existingRating == null)
                {
                    var newRating = new ProductRating
                    {
                        ProductId = ratingDto.ProductId,
                        UserId = ratingDto.UserId,
                        RatingValue = ratingDto.RatingValue
                    };

                    await _ratingsCollection.InsertOneAsync(newRating);
                }
                else
                {
                    var update = Builders<ProductRating>.Update
                        .Set(r => r.RatingValue, ratingDto.RatingValue);

                    await _ratingsCollection.UpdateOneAsync(filter, update);
                }

                return Ok("Vlerësimi u ruajt me sukses.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Gabim gjatë ruajtjes së rating: " + ex.Message);
                return StatusCode(500, "Gabim gjatë ruajtjes së vlerësimit: " + ex.Message);
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAverageRating(string productId)
        {
            try
            {
                var ratings = await _ratingsCollection.Find(r => r.ProductId == productId).ToListAsync();

                if (!ratings.Any())
                    return Ok(new { averageRating = 0 });

                var average = ratings.Average(r => r.RatingValue);
                return Ok(new { averageRating = average });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Gabim në GET Rating: " + ex.Message);
                return StatusCode(500, "Gabim gjatë leximit të vlerësimeve: " + ex.Message);
            }
        }
    }
}
