Car myCar = new Car("BMW");
myCar.ShowNumber();
myCar.ChangeCarNumber("5252");
myCar.ShowName();
myCar.ChangeCarName("Mashina");
myCar.Drive();

ElectricCar myElectricCar = new ElectricCar("SuperDuperCar", "ElonMusk");
myElectricCar.ShowName();
myElectricCar.Drive();

public abstract class Vehicle
{
    public abstract void Drive();
}

public class Car : Vehicle
{
    private string? carNumber = "1111";
    private string name;

    public Car(string name)
    {
        this.name = name;
    }

    public void ShowNumber()
    {
        Console.WriteLine($"Car number is {carNumber}");
    }

    public void ShowName()
    {
        Console.WriteLine($"Car name is {name}");
    }

    public void ChangeCarNumber(string? newName)
    {
        Console.WriteLine($"Old number: {carNumber}");
        carNumber = newName;
        Console.WriteLine($"Car new number = {carNumber}");
    }

    public void ChangeCarName(string newname)
    {
        Console.WriteLine($"Old name: {name}");
        name = newname;
        Console.WriteLine($"Car new name = {name}");
    }

    public override void Drive()
    {
        Console.WriteLine("Wroom Wroom!");
    }
}

public class ElectricCar : Car
{
    private string creator;

    public ElectricCar(string name, string creator) : base(name)
    {
        this.creator = creator;
    }

    public override void Drive()
    {
        Console.WriteLine("Doom Doom!");
    }
}