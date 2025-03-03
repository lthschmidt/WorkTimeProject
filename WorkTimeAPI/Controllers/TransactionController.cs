using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ApplicationContext _context;

    public TransactionController(ApplicationContext context)
    {
        _context = context;
    }

    // Создать новую проводку
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
    {
        Console.WriteLine($"Поиск задачи с id={transaction.TaskId}");
        var task = await _context.Tasks.FindAsync(transaction.TaskId);
        if (task == null)
            return BadRequest("Задача не найдена");

        if (!task.IsActive)
            return BadRequest("Нельзя создать проводку для неактивной задачи");

        var totalWorkTimeToday = await _context
            .Transactions.Where(t => t.UserId == transaction.UserId && t.Date == transaction.Date)
            .SumAsync(t => t.Hours);

        if (totalWorkTimeToday + transaction.Hours > 24)
            return BadRequest("Превышено суточное ограничение в 24 часа");
            
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(
            nameof(GetTransactionById),
            new { id = transaction.Id },
            transaction
        );
    }

    // Получить проводку по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
            return NotFound("Проводка не найдена");
        return Ok(transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions(
    [FromQuery] int? userId,
    [FromQuery] int? taskId,
    [FromQuery] DateTime? date,
    [FromQuery] string? sortBy = "date",
    [FromQuery] bool ascending = false)
    {
        Console.WriteLine($"userId={userId}, date={date}");
        var query = _context.Transactions.AsQueryable();

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);

        if (taskId.HasValue)
            query = query.Where(t => t.TaskId == taskId.Value);

        if (date.HasValue)
            query = query.Where(t => t.Date.Date == date.Value.Date);

        // Сортировка
        query = sortBy?.ToLower() switch
        {
            "hours" => ascending ? query.OrderBy(t => t.Hours) : query.OrderByDescending(t => t.Hours),
            _ => ascending ? query.OrderBy(t => t.Date.Date) : query.OrderByDescending(t => t.Date.Date)
        };

        var transactions = await query.ToListAsync();
        return Ok(transactions);
    }


    // Удалить проводку
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null)
            return NotFound("Проводка не найдена");

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
