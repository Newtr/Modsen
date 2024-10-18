using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Theory3_4.Data;
using Theory3_4.Models;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly StudentContext context;

    public StudentController(StudentContext _context)
    {
        context = _context;
    }
    [HttpGet("ShowStudents")]
    public async Task<ActionResult<IEnumerable<Student>>> GetMyStudents()
    {
        return await context.Students.ToListAsync();
    }


    [HttpPost("Create-Student")]
    public async Task<ActionResult<Student>> PostStudent(Student NEWstudent)
    {
        context.Add(NEWstudent);
        await context.SaveChangesAsync();

        return Ok("Все хорошо. Студент добавлен");
    }

    [HttpPut("Change-Student/{StudentName}")]
    public async Task<ActionResult<Student>> ChangeStudent(string StudentName)
    {
        var myStudent = await context.Students.ToListAsync();

        foreach (var item in myStudent)
        {
            if (item.Name == StudentName)
            {
                item.Name = "Hello this is my new name";
                await context.SaveChangesAsync();
                return Ok("Имя студента было изменено");
            }
        }
        return NotFound();
    }

    [HttpDelete("Delete-Student/{Nickname}")]
    public async Task<ActionResult<Student>> DeleteStudent(string Nickname)
    {
        var student = await context.Students
        .FirstOrDefaultAsync(student => student.Nickname == Nickname);
        try
        {
            context.Remove(student);
            await context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            
            return BadRequest($"An error occurred: {ex.Message}");
        }
        return Ok("Студент успешно удален!");
    }

}