using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsenPractice.Controllers;
using ModsenPractice.Data; // Импортируй пространство имён с контекстом
using ModsenPractice.Entity; // Импортируй пространство имён с сущностями
using ModsenPractice.Patterns.Repository;
using ModsenPractice.Patterns.Repository.Interfaces;
using ModsenPractice.Patterns.UnitOfWork;
using Moq;
using Xunit;

public class UserRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddUserSuccessfully()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ModsenPracticeContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Используем InMemoryDatabase
            .Options;

        // Создаём новый контекст с InMemoryDatabase
        using var context = new ModsenPracticeContext(options);
        
        var userRepo = new UserRepository(context);
        var newUser = new User { Username = "testuser", PasswordHash = "password123" };

        // Act
        await userRepo.AddAsync(newUser);
        await context.SaveChangesAsync();

        // Assert
        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(userInDb);
        Assert.Equal(newUser.Username, userInDb.Username);
    }
    
    [Fact]
    public async Task LoginUser_ShouldLoginSuccessfully()
    {
        // Настройка in-memory базы данных
        var options = new DbContextOptionsBuilder<ModsenPracticeContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Заполнение базы данных тестовыми данными
        using (var context = new ModsenPracticeContext(options))
        {
            context.Users.Add(new User { Username = "testuser", PasswordHash = "password123", Email = "testemail@example.com" });
            await context.SaveChangesAsync();
        }

        // Настройка моков для Unit of Work и UserRepository
        using (var context = new ModsenPracticeContext(options))
        {
            var userRepository = new UserRepository(context);
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            // Настраиваем `UserRepository` в UoW
            mockUnitOfWork.Setup(u => u.UserRepository).Returns(userRepository);

            // Создаем контроллер с UoW
            var userController = new UserController(mockUnitOfWork.Object);

            // Выполнение метода логина
            var result = await userController.Login(new ModsenPractice.DTO.UserLoginDto { Email = "testuser", Password = "password123" });

            // Проверка результата
            Assert.NotNull(result);
            mockUnitOfWork.Verify(u => u.UserRepository, Times.Once);  // Проверяем, что обращение к UserRepository было
        }
    }

    [Fact]
    public async Task CheckInformation_ShouldReturnMessageForLoggedUser()
    {
        // Настройка in-memory базы данных
        var options = new DbContextOptionsBuilder<ModsenPracticeContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Добавление пользователя в базу
        using (var context = new ModsenPracticeContext(options))
        {
            context.Users.Add(new User { Username = "testuser", PasswordHash = "password123" });
            await context.SaveChangesAsync();
        }

        // Настройка UnitOfWork для использования in-memory базы данных
        using (var context = new ModsenPracticeContext(options))
        {
            var userRepository = new UserRepository(context);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(userRepository);

            var controller = new UserController(mockUnitOfWork.Object);

            // Установка ClaimsPrincipal для имитации авторизованного пользователя
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "testuser")
                    }, "mock"))
                }
            };

            // Выполнение теста
            var result = controller.CheckInformation() as OkObjectResult;

            // Проверка результата
            Assert.NotNull(result);
            Assert.Equal("Hello testuser, this is a super cool page. You can see it because you have been logged in.", result.Value);
        }
    }



}
