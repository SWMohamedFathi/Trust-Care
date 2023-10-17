using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using TrustCare.Models;

namespace TrustCare.Controllers
{
    public class AuthentcationController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AuthentcationController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {



            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserId,RoleId,ProfileImage,UserName,Password,Email,FirstName,LastName,Phone,Dateofbirth,ImageFile")] User user)
        {
            if (ModelState.IsValid)
            {

                if (user.ImageFile != null)
                {
                    string wwwRootPath = webHostEnvironment.WebRootPath;

                    string fileName = Guid.NewGuid().ToString() + user.ImageFile.FileName;

                    string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await user.ImageFile.CopyToAsync(fileStream);
                    }

                    user.ProfileImage = fileName;
                }
                var account = _context.Users.Where(x => x.UserName == user.UserName && x.Email == user.Email).FirstOrDefault();
                if (account == null) {

                    user.RoleId = 2;
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ViewBag.Error = "Email is already used, please try another  one.";
                }


            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserId,RoleId,ProfileImage,UserName,Password,Email,FirstName,LastName,Phone,Dateofbirth,ImageFile")] User user)
        {

            var auth = _context.Users.Where(x => x.UserName == user.UserName && x.Password == user.Password).FirstOrDefault();

            if (auth != null)
            {
                var account = _context.Users.Where(x => x.UserId == user.UserId).FirstOrDefault();
                switch (auth.RoleId)
                {
                    case 1:
                        HttpContext.Session.SetInt32("UserId", (int)auth.UserId);
                        HttpContext.Session.SetInt32("RoleId", (int)auth.RoleId);
                        HttpContext.Session.SetString("FirstName", auth.FirstName);
                        HttpContext.Session.SetString("LastName", auth.LastName);
                        HttpContext.Session.SetString("UserName", auth.UserName);
                        HttpContext.Session.SetString("ProfileImage", auth.ProfileImage);
                        HttpContext.Session.SetString("Email", auth.Email);               
                        HttpContext.Session.SetInt32("Phone", (int)auth.Phone);



                        return RedirectToAction("Index", "Admin");
                    case 2:
                        //Var fname = int value 
                        HttpContext.Session.SetInt32("UserId", (int)auth.UserId);
                        HttpContext.Session.SetInt32("RoleId", (int)auth.RoleId);
                        HttpContext.Session.SetString("FirstName", auth.FirstName);
                        HttpContext.Session.SetString("LastName", auth.LastName);
                        HttpContext.Session.SetString("UserName", auth.UserName);
                        HttpContext.Session.SetString("ProfileImage", auth.ProfileImage);
                        HttpContext.Session.SetString("Email", auth.Email);
                        HttpContext.Session.SetInt32("Phone", (int)auth.Phone);
                        return RedirectToAction("Index", "Home");


                }
            }

            else
            {

                ViewBag.Wrong = "Wrong username or password.";

            }

            return View();
        }

        //public IActionResult Profile()
        //{




        //   var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId.HasValue)
        //    {
        //        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

        //        if (user != null)
        //        {
        //            return View(user);
        //        }
        //    }

        //    return View("UserNotFound");
        //}









        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Profile([Bind("UserId,UserName,FirstName,LastName,Password,ImageFile")] User user)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);

        //        if (existingUser != null)
        //        {

        //            existingUser.UserName = user.UserName;
        //            existingUser.FirstName = user.FirstName;
        //            existingUser.LastName = user.LastName;
        //            existingUser.Password = user.Password;
             




        //            if (user.ImageFile != null)
        //            {
        //                string wwwRootPath = webHostEnvironment.WebRootPath;
        //                string fileName = Guid.NewGuid().ToString() + user.ImageFile.FileName;
        //                string path = Path.Combine(wwwRootPath, "Images", fileName);

        //                using (var fileStream = new FileStream(path, FileMode.Create))
        //                {
        //                    await user.ImageFile.CopyToAsync(fileStream);
        //                }

        //                existingUser.ProfileImage = fileName;
        //            }

        //            // Update the user's data
        //            _context.Users.Update(existingUser);
        //            await _context.SaveChangesAsync();

        //            return RedirectToAction("Profile");
        //        }
        //    }

        //    return View("Profile", user);

        //    //if (ModelState.IsValid)
        //    //{
        //    //    // Update the user's profile data in your database
        //    //    _context.Users.Update(user);

        //    //    await _context.SaveChangesAsync();

        //    //    return RedirectToAction("Profile");
        //    //}

        //    //return View("Profile", user);
        //}

    }
    }
