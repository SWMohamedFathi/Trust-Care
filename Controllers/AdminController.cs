using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustCare.Models;

namespace TrustCare.Controllers
{
    public class AdminController : Controller
    {

        private readonly ModelContext _context;
        public AdminController(ModelContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {

            ViewBag.UsersCount = _context.Users.Count();
            ViewBag.SubCount = _context.Subscriptions.Count();
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.Phone = HttpContext.Session.GetInt32("Phone");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Password = HttpContext.Session.GetString("Password");
            ViewBag.Dataofbirth = HttpContext.Session.GetString("Dataofbirth");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");




            return View();
        }
       



    }
}
