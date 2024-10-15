List<Student> myStudents = new List<Student>();
myStudents.Add(new Student("Ivan",16));
myStudents.Add(new Student("Kate",18));
myStudents.Add(new Student("Petr",20));
myStudents.Add(new Student("Andrey",22));

foreach (var item in myStudents)
{
    Console.WriteLine(item.Name);
}

int arrLength = myStudents.ToArray().Length;

for (int i = 0; i < arrLength; i++)
{
    if (myStudents[i].Name == "Petr")
    {
        myStudents.RemoveAt(i);
        break;
    }
}

foreach (var item in myStudents)
{
    Console.WriteLine(item.Name);
}

public class Student
{
    public string Name {get;set;}
    public int Age {get; set;}

    public Student(string name, int age)
    {
        Name = name;
        Age = age;
    }

}

// public class Listik<T>
// {
//     List<Student> myList = new List<Student>();

//     public void AddStudent(Student obj)
//     {
//         myList.Add(obj);
//     }

//     public void DeleteStudent(Student obj)
//     {
//         myList.Remove(obj);
//     }

//     public void ShowAllStudents()
//     {
//         Console.WriteLine("All students:");
//         foreach (Student item in myList)
//         {
//             Console.WriteLine(item)
//         }
//     }
    
// }