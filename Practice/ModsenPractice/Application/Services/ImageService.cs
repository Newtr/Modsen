using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ModsenPractice.Data;
using ModsenPractice.Entity;
using ModsenPractice.Helpers;
using System.IO;

namespace ModsenPractice.Controllers
{
    public class ImageService
{
    public List<EventImage> SaveImages(List<IFormFile> images, IWebHostEnvironment hostEnvironment)
    {
        var savedImages = new List<EventImage>();
        foreach (var image in images)
        {
            string imagePath = MyHelpers.SaveImage(image, hostEnvironment);
            savedImages.Add(new EventImage { ImagePath = imagePath });
        }
        return savedImages;
    }

    public void DeleteUnusedImages(IWebHostEnvironment hostEnvironment, ModsenPracticeContext context)
    {
        MyHelpers.DeleteUnusedImages(hostEnvironment, context);
    }

    public void DeleteImages(IEnumerable<EventImage> eventImages, IWebHostEnvironment hostEnvironment)
    {
        foreach (var eventImage in eventImages)
        {
            var imagePath = Path.Combine(hostEnvironment.WebRootPath, eventImage.ImagePath);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }
}
}