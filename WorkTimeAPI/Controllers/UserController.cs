using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkTimeAPI.DTOs;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationContext _context;

    public UserController(ApplicationContext context)
    {
        _context = context;
    }

    // Регистрация пользователя
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        _context.Users.Add(user);
        var changes = await _context.SaveChangesAsync();
        Console.WriteLine($"Изменений в БД: {changes}");
        Console.WriteLine($"Добавлен пользователь с ID {user.Id}");
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // Авторизация пользователя
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"email: {request.Email}, password: {request.Password}");
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

        if (user == null)
            return Unauthorized("Неверный email или пароль");

        return Ok(user);
    }

    // Получение информации о пользователе
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Пользователь не найден");
        return Ok(user);
    }

    // Обновление данных пользователя
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        if (id != updatedUser.Id) return BadRequest("ID не совпадают");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Пользователь не найден");

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Password = updatedUser.Password;  // Пароли обычно хранятся хэшированными!

        await _context.SaveChangesAsync();
        return Ok(user);
    }
}
