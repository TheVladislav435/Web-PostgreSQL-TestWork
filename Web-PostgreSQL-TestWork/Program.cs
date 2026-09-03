using Microsoft.EntityFrameworkCore;
using Npgsql;
using Web_PostgreSQL_TestWork.Data;

// Создание странителя и считывание данных из appsettings.json
var builder = WebApplication.CreateBuilder(args);

// Добавляем контекст базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<NpgsqlConnection>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new NpgsqlConnection(connectionString);
});

// Добавляем контролера строителю
builder.Services.AddControllersWithViews();

// Создаем веб приложение из полученных данных
var app = builder.Build();

// Проверка не запущенно ли приложение в режиме разработки
if (!app.Environment.IsDevelopment())
{
    // Показуем страницу ошибки без подробностей(тот же 404 что и в django только по другому)
    app.UseExceptionHandler("/Home/Error");
    // Заставляет запомнить браузер что он должен открыться в https а не в http
    app.UseHsts();
}
// Встраиваем в приложение правило перенапровления на https
app.UseHttpsRedirection();
// Встраиваем машрутезатор согласно url запросов
app.UseRouting();

// Встраиваем возможность пользователем авторизации и аутентификации
app.UseAuthorization();

// Подключаем статические файлы
app.MapStaticAssets();

// Подключаем маршруты контролеров
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
