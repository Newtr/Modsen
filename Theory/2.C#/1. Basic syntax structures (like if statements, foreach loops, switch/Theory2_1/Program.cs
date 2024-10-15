string? userInput;
int number = 0;

Console.WriteLine("Please input a number:");
userInput = Console.ReadLine();

try
{
  number = int.Parse(userInput);
}
catch
{
    Console.WriteLine("Your input value is not number!");
    Environment.Exit(1);
}
finally
{
    if(userInput.Contains('-'))
    {
        Console.WriteLine("Your number is negative");
    }
    if(userInput == "0")
    {
        Console.WriteLine("Your number is zero");
    }
    if(!userInput.Contains('-') && userInput != "0")
    {
        Console.WriteLine("Your number is positive");
    }

    Console.WriteLine($"Let's start to count from 1 to {number}:");

    foreach (var item in Enumerable.Range(1, number))
    {
        Console.WriteLine(item);
    }
}
