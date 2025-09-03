using Microsoft.AspNetCore.Mvc;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatBotController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("search")]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { Message = "Please type your question." });

            // Normalize user input
            string normalizedQuery = query.Trim().ToLower();


            var result = _context.ChatBotFAQs
                .AsEnumerable() // move to in-memory for string manipulation
                .FirstOrDefault(f => f.Question.Trim().ToLower().Contains(normalizedQuery) ||
                                     f.Answer.Trim().ToLower().Contains(normalizedQuery));

            if (result == null)
                return NotFound(new { Message = "Sorry, I couldn't find an answer for that." });

            return Ok(new { result.Question, result.Answer });
        }

    }
}

