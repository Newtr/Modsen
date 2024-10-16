using System.Linq;

List<Employee> myEmployees = new List<Employee>
{
    new Employee("Maks", 20),
    new Employee("Kirill", 35),
    new Employee("Fedor", 45),
    new Employee("Pavel", 42),
    new Employee("Anton", 17),
    new Employee("Denis", 17),
    new Employee("Artem", 23),
};

var oldGuys = from empl in myEmployees
            where empl.Age > 30
            select empl;

var sortedGuys = from empl in myEmployees
            orderby empl.Name
            select empl;

var uniqueGuys = from empl in myEmployees
            group empl by empl.Age;

var averageAge = myEmployees.Average(empl => empl.Age);

Console.WriteLine("Only oldGuys > 35:");
foreach (var item in oldGuys)
{
    Console.WriteLine(item.Name);
}

Console.WriteLine("Sorted by name:");
foreach (var item in sortedGuys)
{
    Console.WriteLine(item.Name);
}

Console.WriteLine("Group by age:");
foreach (var group in uniqueGuys)
{
    Console.WriteLine($"Group {group.Key} age:");
    foreach (var empl in group)
    {
        Console.WriteLine(empl.Name);
    }
}

Console.WriteLine($"Average age: {(int)averageAge}");


public class Employee
{
    public string Name {get;set;}
    public int Age {get;set;}

    public Employee(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }
}