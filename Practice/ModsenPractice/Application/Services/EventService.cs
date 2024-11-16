using ModsenPractice.Data;
using ModsenPractice.Entity;
using Microsoft.EntityFrameworkCore;

namespace ModsenPractice.Controllers
{
    public class EventService
{
    private readonly ModsenPracticeContext _context;
    private readonly ImageService _imageService;
    private readonly EmailService _emailService;

    public EventService(ModsenPracticeContext context, ImageService imageService, EmailService emailService)
    {
        _context = context;
        _imageService = imageService;
        _emailService = emailService;
    }

    public async Task<IEnumerable<MyEvent>> GetAllEventsAsync()
    {
        return await _context.Events.Include(e => e.EventMembers).ThenInclude(em => em.MemberEvents).ToListAsync();
    }

    public async Task<MyEvent> GetEventByIdAsync(int id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<MyEvent> GetEventByNameAsync(string name)
    {
        return await _context.Events.FirstOrDefaultAsync(e => e.Name == name);
    }

    public async Task<IEnumerable<MyEvent>> GetEventsByCriteriaAsync(DateTime? date, string? location, string? category)
    {
        var query = _context.Events.AsQueryable();
        
        if (date.HasValue)
            query = query.Where(e => e.DateOfEvent.Date == date.Value.Date);
        if (!string.IsNullOrEmpty(location))
            query = query.Where(e => e.EventLocation.Contains(location));
        if (!string.IsNullOrEmpty(category))
            query = query.Where(e => e.EventCategory.Contains(category));
        
        return await query.ToListAsync();
    }

    public async Task<MyEvent> CreateEventAsync(MyEvent newEvent, List<IFormFile> eventImages, IWebHostEnvironment hostEnvironment)
    {
        newEvent.EventImages = _imageService.SaveImages(eventImages, hostEnvironment);
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        _imageService.DeleteUnusedImages(hostEnvironment, _context);
        return newEvent;
    }

    public async Task<bool> AddImagesToEventAsync(int eventId, List<IFormFile> eventImages, IWebHostEnvironment hostEnvironment)
    {
        var existingEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == eventId);
        if (existingEvent == null) return false;
        
        var images = _imageService.SaveImages(eventImages, hostEnvironment);
        existingEvent.EventImages.AddRange(images);
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateEventAsync(int eventId, MyEvent updatedEvent, List<IFormFile> eventImages, string userEmail, IWebHostEnvironment hostEnvironment)
    {
        var existingEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == eventId);
        if (existingEvent == null) return false;

        existingEvent.Name = updatedEvent.Name;
        existingEvent.Description = updatedEvent.Description;
        existingEvent.DateOfEvent = updatedEvent.DateOfEvent;
        existingEvent.EventLocation = updatedEvent.EventLocation;
        existingEvent.EventCategory = updatedEvent.EventCategory;
        existingEvent.MaxMember = updatedEvent.MaxMember;

        _context.EventImages.RemoveRange(existingEvent.EventImages);
        existingEvent.EventImages = _imageService.SaveImages(eventImages, hostEnvironment);

        await _context.SaveChangesAsync();
        
        _emailService.SendEventUpdatedEmail(userEmail, eventId);
        _imageService.DeleteUnusedImages(hostEnvironment, _context);
        
        return true;
    }

    public async Task<bool> DeleteEventAsync(int eventId, IWebHostEnvironment hostEnvironment)
    {
        var myEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == eventId);
        if (myEvent == null) return false;
        
        _imageService.DeleteImages(myEvent.EventImages, hostEnvironment);
        _context.EventImages.RemoveRange(myEvent.EventImages);
        _context.Events.Remove(myEvent);

        await _context.SaveChangesAsync();
        return true;
    }
}
}