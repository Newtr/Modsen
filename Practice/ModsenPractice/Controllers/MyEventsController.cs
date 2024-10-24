using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsenPractice.Data; 
using ModsenPractice.Entity; 
using Microsoft.AspNetCore.Hosting;
using System.IO;

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

        // GET: api/MyEvents
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

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] MyEvent newEvent, IFormFile EventImage)
        {
            // Проверяем, есть ли файл изображения
            if (EventImage != null && EventImage.Length > 0)
            {
                // Сохраняем изображение и получаем путь к файлу
                string imagePath = SaveImage(EventImage);
                newEvent.ImagePath = imagePath; // Присваиваем путь к картинке в объект события
            }

            // Добавляем событие в базу данных
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            DeleteUnusedImages();

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MyEvent>> UpdateEventById(int id, [FromForm] MyEvent updatedEvent, IFormFile EventImage)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest();
            }

            // Проверяем, был ли загружен новый файл
            if (EventImage != null)
            {
                string imagePath = SaveImage(EventImage);

                // Обновляем свойство ImagePath с новым путем к изображению
                updatedEvent.ImagePath = imagePath;
            }

            _context.Entry(updatedEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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
                DeleteUnusedImages();
            }

            // Возвращаем статус 204 (No Content), если обновление прошло успешно
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

        // Метод для сохранения изображения
        private string SaveImage(IFormFile EventImage)
        {
            // Путь к директории, где хранятся изображения
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");

            // Проверяем, существует ли директория, и создаем её, если нужно
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Получаем список всех файлов в директории
            var existingFiles = Directory.GetFiles(uploadsFolder, "Imo*.jpg");

            // Определяем следующий номер файла
            int nextFileNumber = existingFiles.Length + 1;

            // Генерируем имя файла: Imo1.jpg, Imo2.jpg и т.д.
            var uniqueFileName = $"Imo{nextFileNumber}.jpg";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                EventImage.CopyTo(fileStream);
            }

            return Path.Combine("images", uniqueFileName);
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        // Метод для удаления неиспользуемых изображений
        private void DeleteUnusedImages()
        {
            // Путь к папке с изображениями
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");

            // Получаем все файлы в папке
            var allFiles = Directory.GetFiles(uploadsFolder);

            // Собираем пути всех изображений, которые используются в событиях
            var usedImages = _context.Events.Select(e => e.ImagePath).ToList();

            // Проверяем каждый файл, если он не используется, удаляем его
            foreach (var filePath in allFiles)
            {
                var fileName = Path.GetFileName(filePath);
                if (!usedImages.Contains("images\\" + fileName))
                {
                    // Удаляем неиспользуемое изображение
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}
