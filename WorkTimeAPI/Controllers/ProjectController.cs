using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ProjectController(ApplicationContext context)
    {
        _context = context;
    }

    // Получить все проекты
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        return await _context.Projects.ToListAsync();
    }

    // Получить проект по ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound("Проект не найден");
        return project;
    }

    // Добавить проект
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    // Редактирование проекта (PUT)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] Project updatedProject)
    {
        if (id != updatedProject.Id)
        {
            return BadRequest("ID проекта в URL и в теле запроса не совпадают.");
        }

        var existingProject = await _context.Projects.FindAsync(id);
        if (existingProject == null)
        {
            return NotFound("Проект не найден.");
        }

        // Обновляем свойства
        existingProject.Name = updatedProject.Name;
        existingProject.Code = updatedProject.Code;
        existingProject.IsActive = updatedProject.IsActive;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingProject);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Ошибка при обновлении проекта: {ex.Message}");
        }
    }

    // Удалить проект
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound("Проект не найден");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
