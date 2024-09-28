using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using TestZen.Models;

namespace TestZen.Controllers
{
    public class AccountController : Controller
    {
        private readonly string? _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Register
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new MySqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password ?? throw new ArgumentNullException(nameof(user.password)));
                        var command = new MySqlCommand("INSERT INTO users (fullname, email, phone, address, password) VALUES (@fullname, @email, @phone, @address, @password)", connection);
                        command.Parameters.AddWithValue("@fullname", user.fullname);
                        command.Parameters.AddWithValue("@email", user.email);
                        command.Parameters.AddWithValue("@phone", user.phone);
                        command.Parameters.AddWithValue("@address", user.address);
                        command.Parameters.AddWithValue("@password", passwordHash);
                        await command.ExecuteNonQueryAsync();
                    }
                    
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Login");
                }
                catch (MySqlException ex)
                {
                    ModelState.AddModelError("", "Database error occurred: " + ex.Message);
                    return View(user);
                }
            }
            return View(user);
        }

        // GET: Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            User user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM users WHERE email = @email", connection);
                command.Parameters.AddWithValue("@email", email);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var storedHash = reader["password"].ToString();
                        if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                        {
                            user = new User
                            {
                                fullname = reader["fullname"].ToString(),
                                email = reader["email"].ToString(),
                                phone = reader["phone"].ToString(),
                                address = reader["address"].ToString()
                            };
                        }
                    }
                }
            }

            if (user != null)
            {
                TempData["fullname"] = user.fullname; 
                HttpContext.Session.SetString("fullname", user.fullname);
                return RedirectToAction("Home"); 
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            TempData["fullname"] = "Guest"; 
            return View();
        }
       
       [HttpPost]
        public IActionResult Logout()
        {
            TempData["fullname"] = null; 
            return RedirectToAction("Login"); 
        }

        public IActionResult Home() 
        {
            var fullname = HttpContext.Session.GetString("fullname") ?? "Guest"; 
            ViewData["fullname"] = fullname; 
            return View();
        }
    }
}
