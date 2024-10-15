List<string> listik = new List<string> {"Apple", "Watermelon", "Strawberry"};
List<int> listik2 = new List<int> {8, 16, 32};

Storage<string> myStorage = new Storage<string>();
Storage<int> myStorage2 = new Storage<int>();

foreach (var item in listik)
{
    myStorage.AddElement(item);
}

myStorage.ShowAllElements();

myStorage.DeleteElement("Watermelon");

myStorage.ShowAllElements();

foreach (var item in listik2)
{
    myStorage2.AddElement(item);
}

myStorage2.ShowAllElements();

myStorage2.DeleteElement(16);

myStorage2.ShowAllElements();

public class Storage<T>
{
    private List<T> storageList = new List<T>();

    public void AddElement(T newElement)
    {
        storageList.Add(newElement);
    }

    public void DeleteElement(T element)
    {
        storageList.Remove(element);
    }

    public void ShowAllElements()
    {
        Console.WriteLine("Here's your's all elements: ");
        foreach (var item in storageList)
        {
            Console.WriteLine(item);
        }
    }
}