using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Binder;
using System;
using System.Runtime.CompilerServices;

var serviceCollection = new ServiceCollection();    //Создаем объект с коллекциями. Там просто уже содержаться сервисы которые считаются обзепринятыми
ConfigureService(serviceCollection);    // тут уже мы в эту коллекцию добавляем наш созданный сервис Ilogger, указываем класс его реализации и добавляем транзитивный класс application. То есть по факту просто регистриуем новый сервис

var serviceProvider = serviceCollection.BuildServiceProvider(); // Напрямую к коллекции сервисов мы не образаемся. Для этого нам нужен посредний(провайдер). Тут и создается посредник и через него мы уже можем образаться к нашей коллекции

var app = serviceProvider.GetService<Application>();    // Вот тут мы уже образаемся к классу Application. Но напрямую мы к немсу обратиться не можем поэтому мы через провайдер обращаемся к ними, а так как мы при регистрации указали что будем создават экземпляр класса
app.Run();                                              // то тут мы и создаем экземпляр класса. По факту просто класс создали

static void ConfigureService(IServiceCollection services)   // Метод для регистрации новых сервисов
{
    services.AddSingleton<ILogger, Logger>();   // Добавление сервиса в коллекцию уже имеющихся сервисов. Первый параметр это сам наш сервис Ilogger, второй параметр указывает на класс который реализует наш сервис
    services.AddTransient<Application>();       // То есть по факту мы будем обращаться к сервису через его реализацию в Logger. Singletone говорит нам о том что у нас будет реализован сервис только в классе Logger
}                                              // Если бы не SingleTone то мы могли бы создать сервис Ilogger и добавить несколько классов его реализации типа Logger1, Logger2 и каждый из них по разному бы реализовывал бы Log() исходного сервиса.
                                            //Для того чтобы все было централизовано и используем Singleton
                                            //Далле добавляем наш класс Application в коллекцию как Транзитный, то есть при обращении к этому классу мы будем создавать новый экземпляр этого класса. Чтобы не работать напрямую с родителем мы работаем через его детей 😈😈

public interface ILogger    // По факту это наш функционал который мы хотим видеть, ну то есть интерфейс.
{                           // Здесь только методы и общая логика
    void Log(string message);
}

public class Logger : ILogger   // Тут уже будет непосредственно реализован наш интерфейс
{
    public void Log(string message)
    {
        Console.WriteLine($"[Я реализация Ilogger меня зовут Logger]: {message}");
    }
}

public class Application    // Любой объект, который будет использовать  наш сервис Ilogger
{
    private readonly ILogger logger;    // Создается наш экземпляр интерфейса логгер но для того чтобы использовать поля с readonly их нужно реализовать в конструкторе

    public string example = "Hello there";

    public Application(ILogger logger)
    {
        this.logger = logger;
    }

    public void Run()   // Вызов функции
    {
        logger.Log("Run");  // МЫ как бы обращаемся к сервису ILogger но он нам предоставляет не себя а своб реализацию в Logger, Поэтому этот код будет выполнятьяс корректно
    }
}
