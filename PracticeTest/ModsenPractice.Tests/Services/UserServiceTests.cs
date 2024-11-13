using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsenPractice.Controllers;
using ModsenPractice.Data;
using ModsenPractice.Entity;
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
        var options = new DbContextOptionsBuilder<ModsenPracticeContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using var context = new ModsenPracticeContext(options);
        
        var userRepo = new UserRepository(context);
        var newUser = new User { Username = "testuser", PasswordHash = "password123" };

        await userRepo.AddAsync(newUser);
        await context.SaveChangesAsync();

        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
        Assert.NotNull(userInDb);
        Assert.Equal(newUser.Username, userInDb.Username);
    }
    
    [Fact]
    public async Task LoginUser_ShouldLoginSuccessfully()
    {
        var options = new DbContextOptionsBuilder<ModsenPracticeContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new ModsenPracticeContext(options))
        {
            context.Users.Add(new User { Username = "testuser", PasswordHash = "password123", Email = "testemail@example.com" });
            await context.SaveChangesAsync();
        }

        using (var context = new ModsenPracticeContext(options))
        {
            var userRepository = new UserRepository(context);
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork.Setup(u => u.UserRepository).Returns(userRepository);

            var userController = new UserController(mockUnitOfWork.Object);

            var result = await userController.Login(new ModsenPractice.DTO.UserLoginDto { Email = "testuser", Password = "password123" });

            Assert.NotNull(result);
            mockUnitOfWork.Verify(u => u.UserRepository, Times.Once);
        }
    }

    [Fact]
    public async Task CheckInformation_ShouldReturnMessageForLoggedUser()
    {
        var options = new DbContextOptionsBuilder<ModsenPracticeContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new ModsenPracticeContext(options))
        {
            context.Users.Add(new User { Username = "testuser", PasswordHash = "password123" });
            await context.SaveChangesAsync();
        }

        using (var context = new ModsenPracticeContext(options))
        {
            var userRepository = new UserRepository(context);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(userRepository);

            var controller = new UserController(mockUnitOfWork.Object);

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

            var result = controller.CheckInformation() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Hello testuser, this is a super cool page. You can see it because you have been logged in.", result.Value);
        }
    }



}
