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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MyEvent>>> GetAllEvents()
        {
            var allEvents = await _context.Events.ToListAsync();
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
            // Проверяем, существует ли событие с данным ID
            var existingEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == id);

            if (existingEvent == null)
            {
                return NotFound("Событие не найдено.");
            }

            // Проверяем, были ли переданы изображения
            if (eventImages == null || eventImages.Count == 0)
            {
                return BadRequest("Не предоставлены изображения.");
            }

            // Добавляем новые изображения к событию
            foreach (var image in eventImages)
            {
                // Сохраняем изображение и получаем путь
                string imagePath = MyHelpers.SaveImage(image, _hostEnvironment);

                // Создаем запись изображения для текущего события
                var eventImage = new EventImage
                {
                    ImagePath = imagePath,
                    EventId = existingEvent.Id
                };

                existingEvent.EventImages.Add(eventImage);
            }

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            return Ok("Изображения успешно добавлены.");
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<MyEvent>> UpdateEventById(int id, [FromForm] MyEvent updatedEvent, List<IFormFile> eventImages)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest();
            }

            // Загружаем текущее событие
            var existingEvent = await _context.Events.Include(e => e.EventImages).FirstOrDefaultAsync(e => e.Id == id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            // Обновляем основные поля события
            existingEvent.Name = updatedEvent.Name;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.DateOfEvent = updatedEvent.DateOfEvent;
            existingEvent.EventLocation = updatedEvent.EventLocation;
            existingEvent.EventCategory = updatedEvent.EventCategory;
            existingEvent.MaxMember = updatedEvent.MaxMember;

            // Удаляем старые изображения
            _context.EventImages.RemoveRange(existingEvent.EventImages);

            // Добавляем новые изображения
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
            finally
            {
                MyHelpers.DeleteUnusedImages(_hostEnvironment, _context);
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var myEvent = await _context.Events.FindAsync(id);

            if (myEvent == null)
            {
                return NotFound();
            }

            _context.Events.Remove(myEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
