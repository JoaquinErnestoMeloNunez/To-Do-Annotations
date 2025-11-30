using Microsoft.AspNetCore.Mvc;
using To_Do_Annonations.Domain.Entities;

namespace To_Do_Annotations.Presentation.Controllers
{
    public class AccessController : Controller
    {
        private readonly To_Do_Annotations.Persistence.Database.AppContext _context;

        public AccessController(To_Do_Annotations.Persistence.Database.AppContext context)
        {
            _context = context;
        }

        // GET: /Access/Login (Muestra el formulario)
        public IActionResult Login()
        {
            // Si ya está logueado, lo mandamos a sus tareas
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Tasks");
            }
            return View();
        }

        // POST: /Access/Login (Recibe los datos del form)
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Buscamos usuario en BD (Sin APIs, consulta directa)
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Guardamos el ID en la "memoria" del servidor (Session)
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);

                return RedirectToAction("Index", "Tasks");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        // GET: /Access/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Access/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Guardar nuevo usuario
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: /Access/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Borra la sesión
            return RedirectToAction("Login");
        }
    }
}