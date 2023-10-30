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




            int currentYear = DateTime.Now.Year;
            var model = _context.Subscriptions
                .Where(x => x.SubscriptionDate.HasValue && x.SubscriptionDate.Value.Year == currentYear)
                .ToList();

            // Create a collection of all months in the current year
            var allMonths = Enumerable.Range(1, 12);

            // Group the data by month and calculate the profit for each month
            var monthlyProfits = allMonths
                .GroupJoin(
                    model,
                    month => month,
                    subscription => subscription.SubscriptionDate.Value.Month,
                    (month, subscriptions) => new
                    {
                        Month = month,
                        TotalProfit = subscriptions.Sum(subscription => subscription.SubscriptionAmount)
                    })
                .OrderBy(result => result.Month)
                .Select(result => result.TotalProfit)
                .ToList();
            var labels = Enumerable.Range(1, 12).Select(month => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month)).ToList();


            //ViewBag.Labels = labels;
            ViewBag.MonthlyProfits = monthlyProfits;
            ViewBag.Labels = labels;
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
            // Retrieve a list of subscriptions from the database, including related data.
            var model = _context.Subscriptions
                .Include(c => c.Beneficiaries)
                .Include(c => c.User)
                .ToList();

            if (month == 0 && year != null)
            {
                // Filter the subscriptions by the specified year.
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

        //public IActionResult Chart()
        //{

        //    //Get the current year and fetch data for the current year


        //}

        //[HttpPost]
        //public IActionResult Chart(int? month, int? year)
        //{

        //    // Get the current year and fetch data for the current year
        //    int currentYear = DateTime.Now.Year;
        //    var model = _context.Subscriptions
        //        .Where(x => x.SubscriptionDate.Value.Year == currentYear)
        //        .Include(c => c.Beneficiaries)
        //        .Include(c => c.User)
        //        .ToList();

        //    // Group the data by month and calculate the profit for each month
        //    var monthlyProfits = model
        //        .GroupBy(x => x.SubscriptionDate.Value.Month)
        //        .Select(g => g.Sum(x => x.SubscriptionAmount))
        //        .ToList();

        //    // Define labels for the months
        //    //var labels = Enumerable.Range(1, 12).Select(month => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month)).ToList();

        //    //ViewBag.Labels = labels;
        //    ViewBag.MonthlyProfits = monthlyProfits;

        //    return View();
        //}

    }
}