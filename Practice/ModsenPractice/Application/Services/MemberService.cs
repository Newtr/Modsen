
using Microsoft.EntityFrameworkCore;
using ModsenPractice.Data;
using ModsenPractice.Entity;

namespace ModsenPractice.Controllers
{
public class MemberService
    {
        private readonly ModsenPracticeContext _context;

        public MemberService(ModsenPracticeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetEventMembersAsync(int eventId)
        {
            var existingEvent = await _context.Events.Include(e => e.EventMembers)
                                                     .FirstOrDefaultAsync(e => e.Id == eventId);
            if (existingEvent == null)
                return null;

            return existingEvent.EventMembers;
        }

        public async Task<Member> GetMemberByIdAsync(int memberId)
        {
            return await _context.Members.FindAsync(memberId);
        }

        public async Task<string> RegisterMemberAsync(int eventId, int memberId)
        {
            var existingEvent = await _context.Events.Include(e => e.EventMembers)
                                                     .FirstOrDefaultAsync(e => e.Id == eventId);
            var existingMember = await _context.Members.FindAsync(memberId);

            if (existingEvent == null || existingMember == null)
                return "Событие или участник не найден.";

            if (existingEvent.EventMembers.Any(m => m.Id == memberId))
                return "Участник уже зарегистрирован на это событие.";

            existingEvent.EventMembers.Add(existingMember);
            await _context.SaveChangesAsync();

            return "Участник успешно зарегистрирован на событие.";
        }

        public async Task<string> UnregisterMemberAsync(int eventId, int memberId)
        {
            var eventEntity = await _context.Events.Include(e => e.EventMembers)
                                                   .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity == null)
                return $"Event with ID {eventId} not found.";

            var member = eventEntity.EventMembers.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
                return $"Member with ID {memberId} is not registered for this event.";

            eventEntity.EventMembers.Remove(member);
            await _context.SaveChangesAsync();

            return $"Member with ID {memberId} has been unregistered from event with ID {eventId}.";
        }
    }
}