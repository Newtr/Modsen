
using ModsenPractice.Helpers;

namespace ModsenPractice.Controllers
{
    public class EmailService
{
    public void SendEventUpdatedEmail(string userEmail, int eventId)
    {
        string subject = "Event Updated";
        string body = $"Event with id {eventId} has been successfully updated.";
        MyHelpers.SendEmail(userEmail, subject, body);
    }
}
}