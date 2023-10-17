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
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CurrentTime = DateTime.Now;

            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        

          public IActionResult Testimonial()
        {
            return View();
        }

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