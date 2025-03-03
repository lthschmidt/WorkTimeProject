using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ApplicationContext _context;

    public TaskController(ApplicationContext context)
    {
        _context = context;
    }

    // Получить задачи (все или по ID проекта)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Task>>> GetTasks([FromQuery] int? projectId)
    {
        var query = _context.Tasks.AsQueryable();
        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }
        var tasks = await query.ToListAsync();
        return Ok(tasks);
    }


    // Получить задачи по ID проекта
    [HttpGet("{id}")]
    public async Task<ActionResult<Task>> GetTask(int id)
    {
        var Task = await _context.Tasks.FindAsync(id);
        if (Task == null)
            return NotFound();
        return Task;
    }

    // Добавить проект
    [HttpPost]
    public async Task<ActionResult<Task>> CreateTask(Task Task)
    {
        var project = await _context.Projects.FindAsync(Task.ProjectId);
        if (project == null)
            return NotFound("Проект не найден");

        Task.Project = project;  // Привязываем существующий проект
        _context.Tasks.Add(Task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = Task.Id }, Task);
    }

    // Редактирование задачи
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] Task updatedTask)
    {
        if (id != updatedTask.Id)
        {
            return BadRequest("ID задачи в URL и в теле запроса не совпадают.");
        }

        var existingTask = await _context.Tasks.FindAsync(id);
        if (existingTask == null)
        {
            return NotFound("Задача не найдена.");
        }

        // Обновляем свойства
        existingTask.Name = updatedTask.Name;
        existingTask.ProjectId = updatedTask.ProjectId;
        existingTask.IsActive = updatedTask.IsActive;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingTask);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Ошибка при обновлении задачи: {ex.Message}");
        }
    }

    // Удалить задачу
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound("Проект не найден");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
