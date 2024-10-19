using System.ComponentModel.DataAnnotations;

public class Car 
{
    public DateTime DateOfCreation {get; set;}
    public string Model {get;set;}
    [Key]
    public int CarNumber {get;set;}
}