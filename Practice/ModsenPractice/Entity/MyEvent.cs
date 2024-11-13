namespace ModsenPractice.Entity
{

    public class MyEvent
    {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateOfEvent { get; set; }
    public string EventLocation { get; set; }
    public string EventCategory { get; set; }
    public int MaxMember { get; set; }

    public List<EventImage> EventImages { get; set; }

    public ICollection<Member> EventMembers { get; set; } = new List<Member>();

    public MyEvent()
    {
        EventMembers = new List<Member>();
        EventImages = new List<EventImage>();
    }
    }

}