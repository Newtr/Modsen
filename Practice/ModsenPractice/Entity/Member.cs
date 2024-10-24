namespace ModsenPractice.Entity
{
    
public class Member
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public DateTime DateOfBirth { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Email { get; set; }

    public List<MyEvent> MemberEvents { get; set; }

    public Member()
    {
        MemberEvents = new List<MyEvent>();
    }
}

}