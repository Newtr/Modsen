namespace ModsenPractice.Entity
{
    public class EventImage
    {
    public int Id { get; set; }
    public int EventId { get; set; }
    public string ImagePath { get; set; }

    public MyEvent MyEvent { get; set; }
    }

}