using Microsoft.AspNetCore.Mvc;
using BulkyBookWeb.MongoServices;

namespace BulkyBookWeb.Controllers.Api
{
    [Route("api/EmployeeMessage")]

    [ApiController]
    public class EmployeeMessageController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public EmployeeMessageController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _mongoService.GetAllEmployeeMessagesAsync();
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EmployeeMessageDto dto)
        {
            var message = new EmployeeMessage
            {
                EmployeeId = dto.EmployeeId,
                UserName = dto.UserName,
                MessageText = dto.MessageText,
                CreatedAt = DateTime.UtcNow
            };

            await _mongoService.AddEmployeeMessageAsync(message);
            return Ok(message);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mongoService.DeleteEmployeeMessageAsync(id);
            if (!result)
                return NotFound();
            return Ok();



        }
     

    }
}
