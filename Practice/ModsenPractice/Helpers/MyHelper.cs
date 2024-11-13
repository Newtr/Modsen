using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
            var uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName;
            string filePath;

            do
            {
                int randomNumber = new Random().Next(1, 1_000_000);

                uniqueFileName = $"Imo{randomNumber}.jpg";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

            } while (File.Exists(filePath));

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
            var uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");

            var allFiles = Directory.GetFiles(uploadsFolder);

            var usedImages = dbContext.EventImages.Select(img => img.ImagePath).ToList();

            foreach (var filePath in allFiles)
            {
                var fileName = Path.GetFileName(filePath);
                if (!usedImages.Contains("images\\" + fileName))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        public static void SendEmail(string toEmail, string subject, string body)
        {
            string fromEmail = "newt3rr@gmail.com";      
            string password = "yync qxpf yuzl ltrd";     

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

    }
}