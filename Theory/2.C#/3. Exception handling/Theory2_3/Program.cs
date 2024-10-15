using System.Transactions;

int number1;
int number2;
int result;

try
{
    Console.WriteLine("Enter first number: ");
    number1 = int.Parse(Console.ReadLine());
    Console.WriteLine("Enter second number: ");
    number2 = int.Parse(Console.ReadLine());
    result = number1/number2;
}
catch(DivideByZeroException)
{
    Console.WriteLine("You can't divide by zero");
    Environment.Exit(221);
}
catch(FormatException)
{
    Console.WriteLine("Input value must be integer!");
    Environment.Exit(223);
}
finally
{
    Console.WriteLine($"All good!");
}