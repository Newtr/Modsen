using System.ComponentModel.DataAnnotations;

namespace Theory3_4.Models
 {
    public class Student
    {
        [Key]
        public string Nickname {get;set;}
        public string Name {get;set;}
        public string Surname {get;set;}
    }
 }
