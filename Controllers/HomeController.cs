using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrustCare.Models;

namespace TrustCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
             this._context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
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

            ViewBag.CurrentTime = DateTime.Now;

            //var page = _context.Homepages.ToList();
            //return View(page);
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");

            var homepages = _context.Homepages.ToList();
            var testmonail = _context.Testimonials.Include(x=>x.User).ToList();
            var about = _context.Abouts.ToList();

            var model = Tuple.Create<IEnumerable<Homepage>, IEnumerable<Testimonial>, IEnumerable<About>>(homepages, testmonail, about);
            return View(model);





        }

        public IActionResult AboutUs()
        {
            return View();
        }
        

        //  public IActionResult Testimonial()
        //{
        //    //var testmonial = _context.Testimonials.ToList();
        //    //return View(testmonial);
        //}

        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs([Bind("ContactId,Name,Subject,Address,Phone,Email,Message")] ContactU contactU)
        {
            if (ModelState.IsValid)
            {



                _context.Add(contactU);
                await _context.SaveChangesAsync();


                ViewBag.SuccessMessage = "Well Done..!";

                //return RedirectToAction("ContactUs", "Home");


            }




            return View(contactU);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "home");
        }

     
    }
}