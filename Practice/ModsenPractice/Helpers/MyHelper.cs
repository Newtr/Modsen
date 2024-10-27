using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModsenPractice.Data; 

namespace ModsenPractice.Helpers
{
    public static class MyHelpers
    {
        public static string SaveImage(IFormFile EventImage, IWebHostEnvironment hostEnvironment)
        {
            // Путь к директории, где хранятся изображения
            var uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");

            // Проверяем, существует ли директория, и создаем её, если нужно
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName;
            string filePath;

            do
            {
                // Генерируем случайное число от 1 до 1 миллиона
                int randomNumber = new Random().Next(1, 1_000_000);

                // Формируем имя файла
                uniqueFileName = $"Imo{randomNumber}.jpg";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

            } while (File.Exists(filePath)); // Повторяем, пока не найдем уникальное имя

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                EventImage.CopyTo(fileStream);
            }

            return Path.Combine("images", uniqueFileName);
        }


        public static bool EventExists(int id, ModsenPracticeContext dbContext)
        {
            return dbContext.Events.Any(e => e.Id == id);
        }

        public static void DeleteUnusedImages(IWebHostEnvironment hostEnvironment, ModsenPracticeContext dbContext)
        {
            // Путь к папке с изображениями
            var uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");

            // Получаем все файлы в папке
            var allFiles = Directory.GetFiles(uploadsFolder);

            // Собираем пути всех изображений, которые используются в событиях
            var usedImages = dbContext.EventImages.Select(img => img.ImagePath).ToList();

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