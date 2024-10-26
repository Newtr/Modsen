namespace ModsenPractice.Entity
{
    public class EventImage
    {
    public int Id { get; set; }
    public int EventId { get; set; }
    public string ImagePath { get; set; }

    //свойство для связи с MyEvent
    public MyEvent MyEvent { get; set; }
    }

}