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
    public class MyEventsController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ModsenPracticeContext _context;

        public MyEventsController(ModsenPracticeContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]public async Task<ActionResult<IEnumerable<MyEvent>>> GetAllEvents()
        {
            var allEvents = await _context.Events
            .Include(e => e.EventMembers)
            .ThenInclude(em => em.MemberEvents)
            .ToListAsync();

            return Ok(allEvents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MyEvent>> GetEventById(int id)
        {
            var _myEvent = await _context.Events.FindAsync(id);

            if (_myEvent != null)
            {
                return Ok(_myEvent);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<MyEvent>> GetEventByName(string name)
        {
            var _myEvent = await _context.Events.FirstOrDefaultAsync(_evnt => _evnt.Name == name);

            if (_myEvent != null)
            {
                return Ok(_myEvent);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<MyEvent>>> GetEventByCriteria(DateTime? date = null, string? location = null, string? category = null)
        {
            var query = _context.Events.AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(e => e.DateOfEvent.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.EventLocation.Contains(location));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.EventCategory.Contains(category));
            }

            var filteredEvents = await query.ToListAsync();

            return Ok(filteredEvents);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] MyEvent newEvent, List<IFormFile> eventImages)
        {
            if (eventImages != null && eventImages.Count > 0)
            {
                newEvent.EventImages = new List<EventImage>();

                foreach (var image in eventImages)
                {
                    string imagePath = MyHelpers.SaveImage(image, _hostEnvironment);
                    var eventImage = new EventImage { ImagePath = imagePath };
                    newEvent.EventImages.Add(eventImage);
                }
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            MyHelpers.DeleteUnusedImages(_hostEnvironment, _context);

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
        }

        [HttpPost("{id}/add-images")]
        public async Task<IActionResult> AddImagesToEvent(int id, List<IFormFile> eventImages)
        {
            var existingEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == id);

            if (existingEvent == null)
            {
                return NotFound("Событие не найдено.");
            }

            if (eventImages == null || eventImages.Count == 0)
            {
                return BadRequest("Не предоставлены изображения.");
            }

            foreach (var image in eventImages)
            {
                string imagePath = MyHelpers.SaveImage(image, _hostEnvironment);

                var eventImage = new EventImage
                {
                    ImagePath = imagePath,
                    EventId = existingEvent.Id
                };

                existingEvent.EventImages.Add(eventImage);
            }

            await _context.SaveChangesAsync();

            return Ok("Изображения успешно добавлены.");
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<MyEvent>> UpdateEventById(int id, [FromForm] MyEvent updatedEvent, List<IFormFile> eventImages, [FromForm] string userEmail)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest();
            }

            var existingEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.Name = updatedEvent.Name;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.DateOfEvent = updatedEvent.DateOfEvent;
            existingEvent.EventLocation = updatedEvent.EventLocation;
            existingEvent.EventCategory = updatedEvent.EventCategory;
            existingEvent.MaxMember = updatedEvent.MaxMember;

            _context.EventImages.RemoveRange(existingEvent.EventImages);

            existingEvent.EventImages = new List<EventImage>();
            foreach (var image in eventImages)
            {
                string imagePath = MyHelpers.SaveImage(image, _hostEnvironment);
                var eventImage = new EventImage { ImagePath = imagePath, EventId = existingEvent.Id };
                existingEvent.EventImages.Add(eventImage);
            }

            try
            {
                await _context.SaveChangesAsync();

                string subject = "Event Updated";
                string body = $"Event with id {id} has been successfully updated.";
                
                MyHelpers.SendEmail(userEmail, subject, body);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MyHelpers.EventExists(id, _context))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
            }
            finally
            {
                MyHelpers.DeleteUnusedImages(_hostEnvironment, _context);
            }

            return Ok($"Event with id {id} was updated");
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var myEvent = await _context.Events
                .Include(e => e.EventImages)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (myEvent == null)
            {
                return NotFound();
            }

            var imagesFolder = _hostEnvironment.WebRootPath;

            foreach (var eventImage in myEvent.EventImages)
            {
                var imagePath = Path.Combine(imagesFolder, eventImage.ImagePath);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.EventImages.RemoveRange(myEvent.EventImages);

            _context.Events.Remove(myEvent);
            
            await _context.SaveChangesAsync();

            return Ok($"Event with id {id} and all associated images were deleted");
        }

    }
}
