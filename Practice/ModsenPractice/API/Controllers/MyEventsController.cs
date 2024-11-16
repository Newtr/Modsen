using Microsoft.AspNetCore.Mvc;
using ModsenPractice.Entity;
using Microsoft.AspNetCore.Hosting;

namespace ModsenPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyEventsController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MyEventsController(EventService eventService, IWebHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MyEvent>>> GetAllEvents()
        {
            var allEvents = await _eventService.GetAllEventsAsync();
            return Ok(allEvents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MyEvent>> GetEventById(int id)
        {
            var myEvent = await _eventService.GetEventByIdAsync(id);
            return myEvent != null ? Ok(myEvent) : NotFound();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<MyEvent>> GetEventByName(string name)
        {
            var myEvent = await _eventService.GetEventByNameAsync(name);
            return myEvent != null ? Ok(myEvent) : NotFound();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<MyEvent>>> GetEventByCriteria(DateTime? date = null, string? location = null, string? category = null)
        {
            var filteredEvents = await _eventService.GetEventsByCriteriaAsync(date, location, category);
            return Ok(filteredEvents);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] MyEvent newEvent, List<IFormFile> eventImages)
        {
            var createdEvent = await _eventService.CreateEventAsync(newEvent, eventImages, _hostEnvironment);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPost("{id}/add-images")]
        public async Task<IActionResult> AddImagesToEvent(int id, List<IFormFile> eventImages)
        {
            bool result = await _eventService.AddImagesToEventAsync(id, eventImages, _hostEnvironment);
            return result ? Ok("Изображения успешно добавлены.") : NotFound("Событие не найдено.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MyEvent>> UpdateEventById(int id, [FromForm] MyEvent updatedEvent, List<IFormFile> eventImages, [FromForm] string userEmail)
        {
            bool result = await _eventService.UpdateEventAsync(id, updatedEvent, eventImages, userEmail, _hostEnvironment);
            return result ? Ok($"Event with id {id} was updated") : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            bool result = await _eventService.DeleteEventAsync(id, _hostEnvironment);
            return result ? Ok($"Event with id {id} and all associated images were deleted") : NotFound();
        }
    }
}
