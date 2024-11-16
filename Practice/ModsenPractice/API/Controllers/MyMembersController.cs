using Microsoft.AspNetCore.Mvc;
using ModsenPractice.Services;
using ModsenPractice.Entity;

namespace ModsenPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyMembersController : ControllerBase
    {
        private readonly MemberService _memberService;

        public MyMembersController(MemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet("{eventId}/members")]
        public async Task<ActionResult<IEnumerable<Member>>> GetEventMembers(int eventId)
        {
            var members = await _memberService.GetEventMembersAsync(eventId);

            if (members == null)
                return NotFound("Событие не найдено.");

            return Ok(members);
        }

        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<Member>> GetMemberById(int memberId)
        {
            var member = await _memberService.GetMemberByIdAsync(memberId);

            if (member == null)
                return NotFound("Участник не найден.");

            return Ok(member);
        }

        [HttpPost("{eventId}/register")]
        public async Task<IActionResult> RegisterMember(int eventId, [FromBody] int memberId)
        {
            var result = await _memberService.RegisterMemberAsync(eventId, memberId);

            if (result.Contains("не найден"))
                return NotFound(result);
            if (result.Contains("уже зарегистрирован"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("unregister")]
        public async Task<IActionResult> UnregisterFromEvent(int eventId, int memberId)
        {
            var result = await _memberService.UnregisterMemberAsync(eventId, memberId);

            if (result.Contains("not found"))
                return NotFound(result);

            return Ok(result);
        }
    }
}
