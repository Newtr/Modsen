using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly CarContext context;

    public CarController(CarContext _carContext)
    {
        context = _carContext;
    }


    [HttpGet("GetCars/{CarDate}")]
    public async Task<ActionResult<Car>> GetMyCars(DateTime CarDate)
    {
        var mycars = await context.Cars.ToArrayAsync();
        var carsbydate = mycars.Where(car => car.DateOfCreation >= CarDate);
        
        if(carsbydate.Any())
        {
            return Ok(carsbydate);
        }
        else
        {
            return NotFound("Problem");
        }
    }

}