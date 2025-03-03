using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080") // Укажи порт, на котором работает Vue
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// // Инициализация базы данных тестовыми данными
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
//     //db.Database.Migrate(); // Применяем миграции

//     if (!db.Users.Any()) // Проверяем, есть ли уже пользователи
//     {
//         db.Users.AddRange(new User { Name = "Tom", Email = "tom1999@inbox.com", Password = "1111" }, new User { Name = "Alice", Email = "alice1999@inbox.com", Password = "5555" });
//         db.SaveChanges();
//         Console.WriteLine("Тестовые пользователи добавлены в БД");
//     }
// }



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Включаем маршрутизацию для API
    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers(); // <-- ВАЖНО!
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.Run();
