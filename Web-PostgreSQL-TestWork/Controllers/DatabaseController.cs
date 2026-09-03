using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Web_PostgreSQL_TestWork.Models;

namespace Web_PostgreSQL_TestWork.Controllers
{
    public class DatabaseController : Controller
    {
        // Храним подключение к БД
        private readonly NpgsqlConnection _connection;

        // Конструктор - вызывается когда создается контроллер (аналог __init__ в Django)
        // Получаем готовое подключение и логгер извне (как dependency injection в Django)
        public DatabaseController(NpgsqlConnection connection)
        {
            _connection = connection; // Сохраняем подключение для использования в методах
        }

        //Страница с таблицой из базы данных
        public async Task<IActionResult> Index()
        {
            var bd_object = new List<BD_Objects>();

            try
            {
                await _connection.OpenAsync();
                using var command = new NpgsqlCommand("SELECT INN, Description FROM BD_Objects", _connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    bd_object.Add(new BD_Objects
                    {
                        INN = reader.GetString(0),
                        Description = reader.GetString(1),
                    });
                }

                return View(bd_object);
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        //Страница ввода данных в базу данных
        public IActionResult AddObject()
        {
            return View();
        }

        //Страница проверки данных введенных пользователем для баззы данных
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(BD_Objects newObjects)
        {
            if (ModelState.IsValid) {
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _connection.OpenAsync();

                        // INSERT запрос с RETURNING Id
                        using var command = new NpgsqlCommand(
                            "INSERT INTO BD_Objects (INN, Description) VALUES (@inn, @description) RETURNING Id",
                            _connection
                        );

                        // Защита от SQL-инъекций
                        command.Parameters.AddWithValue("@inn", newObjects.INN);
                        command.Parameters.AddWithValue("@description", newObjects.Description);

                        // Выполняем и получаем ID созданной записи
                        var newId = (int)await command.ExecuteScalarAsync();

                        Console.WriteLine($"✅ Добавлена запись с ID: {newId}");

                        // Перенаправляем на страницу со списком
                        return RedirectToAction("Index");
                    }
                    catch (NpgsqlException ex)
                    {
                        ModelState.AddModelError("", $"Ошибка базы данных: {ex.Message}");
                    }
                    finally
                    {
                        await _connection.CloseAsync();
                    }
                }

                return Redirect("/Database/");
                }
            return View("AddObject", newObjects);
        }
    }
}
