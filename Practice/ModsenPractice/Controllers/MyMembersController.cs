using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsenPractice.Data; 
using ModsenPractice.Entity; 
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ModsenPractice.Helpers;

namespace ModsenPractice.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MyMembersController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ModsenPracticeContext _context;

        public MyMembersController(IWebHostEnvironment hostEnvironment, ModsenPracticeContext context)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        [HttpGet("{eventId}/members")]
        public async Task<ActionResult<IEnumerable<Member>>> GetEventMembers(int eventId)
        {
            var existingEvent = await _context.Events.Include(e => e.EventMembers).FirstOrDefaultAsync(e => e.Id == eventId);

            if (existingEvent == null)
                return NotFound("Событие не найдено.");

            return Ok(existingEvent.EventMembers);
        }

        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<Member>> GetMemberById(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);

            if (member == null)
                return NotFound("Участник не найден.");

            return Ok(member);
        }

        [HttpPost("{eventId}/register")]
        public async Task<IActionResult> RegisterMember(int eventId, [FromBody] int memberId)
        {
            var existingEvent = await _context.Events.Include(e => e.EventMembers).FirstOrDefaultAsync(e => e.Id == eventId);
            var existingMember = await _context.Members.FindAsync(memberId);

            if (existingEvent == null || existingMember == null)
                return NotFound("Событие или участник не найден.");

            if (existingEvent.EventMembers.Any(m => m.Id == memberId))
                return BadRequest("Участник уже зарегистрирован на это событие.");

            existingEvent.EventMembers.Add(existingMember);
            await _context.SaveChangesAsync();

            return Ok("Участник успешно зарегистрирован на событие.");
        }

        [HttpDelete("unregister")]
        public async Task<IActionResult> UnregisterFromEvent(int eventId, int memberId)
        {
            // Находим событие
            var eventEntity = await _context.Events
                .Include(e => e.EventMembers)
                .FirstOrDefaultAsync(e => e.Id == eventId);
            
            if (eventEntity == null)
            {
                return NotFound($"Event with ID {eventId} not found.");
            }

            var member = eventEntity.EventMembers.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
            {
                return NotFound($"Member with ID {memberId} is not registered for this event.");
            }

            eventEntity.EventMembers.Remove(member);
            await _context.SaveChangesAsync();

            return Ok($"Member with ID {memberId} has been unregistered from event with ID {eventId}.");
        }

    }

}
