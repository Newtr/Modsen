var loaddataTask = LoadData();
var processdataTask = ProcessData();
var savedataTask = SaveData();

await Task.WhenAll(loaddataTask,processdataTask,savedataTask);

Console.WriteLine("All done!");


static async Task LoadData()
{
    Console.WriteLine("Start LoadData()");
    await Task.Delay(3000);
    Console.WriteLine("End LoadData()");
}

static async Task ProcessData()
{
    Console.WriteLine("Start ProcessData()");
    await Task.Delay(2000);
    Console.WriteLine("End ProcessData()");
}

static async Task SaveData()
{
    Console.WriteLine("Start SaveData()");
    await Task.Delay(1000);
    Console.WriteLine("End SaveData()");
}