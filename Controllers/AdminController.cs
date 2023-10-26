using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using MimeKit;
using System.Globalization;
using TrustCare.Models;
using Newtonsoft.Json;

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
            ViewBag.BenefiCount = _context.Beneficiaries.Count(b => b.ApprovalStatus == "Pending");
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.Phone = HttpContext.Session.GetInt32("Phone");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Password = HttpContext.Session.GetString("Password");
            ViewBag.Dataofbirth = HttpContext.Session.GetString("Dataofbirth");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.RoleId = HttpContext.Session.GetInt32("RoleId");





            return View();
        }
        public IActionResult Report()
        {
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.benefit = _context.Subscriptions.Sum(x => x.SubscriptionAmount);
            ViewBag.RegisteredUsers = _context.Users.Count();
            ViewBag.SubscribersNumber = _context.Subscriptions.Count(user => user.PaymentStatus.ToLower() == "Paid".ToLower());

            var model = _context.Subscriptions
                .Include(c => c.Beneficiaries)
                .Include(c => c.User)
                .ToList();

        return View(model);


        }

        [HttpPost]
        public IActionResult Report(int? month, int? year)
        {
            int currentMonth = DateTime.Now.Month;
            var model = _context.Subscriptions
                .Include(c => c.Beneficiaries)
                .Include(c => c.User)
                .ToList();

            if (month == 0 && year != null)
            {
                var subscriptionsFilteredByYear = model.Where(x => x.SubscriptionDate.Value.Year == year);
                ViewBag.benefit = subscriptionsFilteredByYear.Sum(x => x.SubscriptionAmount);
                ViewBag.RegisteredUsers = subscriptionsFilteredByYear.Count();
                ViewBag.SubscribersNumber = subscriptionsFilteredByYear.Count(user => user.PaymentStatus.ToLower() == "Paid".ToLower());
                return View(subscriptionsFilteredByYear);
            }
            else if (month != null && year != null)
            {
                var subscriptionsFilteredByMonthAndYear = model.Where(x => x.SubscriptionDate.Value.Year == year && x.SubscriptionDate.Value.Month == month);
                ViewBag.benefit = subscriptionsFilteredByMonthAndYear.Sum(x => x.SubscriptionAmount);
                ViewBag.RegisteredUsers = subscriptionsFilteredByMonthAndYear.Count();
                ViewBag.SubscribersNumber = subscriptionsFilteredByMonthAndYear.Count(user => user.PaymentStatus.ToLower() == "Paid".ToLower());
                return View(subscriptionsFilteredByMonthAndYear);
            }

            return View(model);





        }
    }
}