using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrustCare.Models;

namespace TrustCare.Controllers
{
    public class TestimonialsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TestimonialsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;

        }





        // GET: Testimonials
        public async Task<IActionResult> Index()
        {
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
            //var modelContext = _context.Testimonials.Include(t => t.User);
            //var testimonialsWithUsers = await modelContext.ToListAsync();

            //return View(testimonialsWithUsers);
            var modelContext = _context.Testimonials.Include(t => t.User);
            return View(await modelContext.ToListAsync());
        }

        public async Task<IActionResult> AcceptTestimonial(decimal testimonialId)
        {
            var testimonial = await _context.Testimonials.FindAsync(testimonialId);

            if (testimonial != null)
            {
                testimonial.ApprovalStatus = "Accepted";
                await _context.SaveChangesAsync();
            }

            
            return RedirectToAction("Index");
        }


    
        public async Task<IActionResult> RejectTestimonial(decimal testimonialId)
        {
            //var testimonial = await _context.Testimonials.FindAsync(testimonialId);

            //if (testimonial != null)
            //{
            //    testimonial.ApprovalStatus = "Rejected";
            //    await _context.SaveChangesAsync();
            //}

            //return RedirectToAction("Index");
            var testimonial = await _context.Testimonials.FindAsync(testimonialId);

            if (testimonial != null)
            {
                testimonial.ApprovalStatus = "Rejected";
                _context.Testimonials.Remove(testimonial); // Remove the testimonial 
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }



        // GET: Testimonials/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TestimonialId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        //// GET: Testimonials/Create
        //public IActionResult Create()
        //{
        //    //if (id == null || _context.Testimonials == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //var testimonial = await _context.Testimonials.FindAsync(id);
        //    //if (testimonial == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
        //    ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
        //    ViewBag.LastName = HttpContext.Session.GetString("LastName");

        //    //////ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
        //    ////return View(testimonial);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
        //    return View();


        //}

        //// POST: Testimonials/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("TestimonialId,UserId,TestimonialText,ApprovalStatus,ImageFile,Imagepath")] Testimonial testimonial)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (testimonial.ImageFile != null)
        //        {
        //            string wwwRootPath = webHostEnvironment.WebRootPath;

        //            string fileName = Guid.NewGuid().ToString() + testimonial.ImageFile.FileName;

        //            string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

        //            using (var fileStream = new FileStream(path, FileMode.Create))
        //            {
        //                await testimonial.ImageFile.CopyToAsync(fileStream);
        //            }

        //            testimonial.Imagepath = fileName;
        //        }

        //        _context.Add(testimonial);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", testimonial.UserId);
        //    return View(testimonial);
        //}
        public IActionResult Create()
        {
            // Retrieve user information from the session
            string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");
            string profileImage = HttpContext.Session.GetString("ProfileImage");
   


            // Create a new Testimonial object with user information
            var testimonial = new Testimonial
            {
                // Populate user information
                User = new User
                {
                    // Assuming UserId is unique for the logged-in user
                    UserId = ViewBag.UserId = HttpContext.Session.GetInt32("UserId"),
                    // Set the user's name
                    FirstName = firstName,
                    LastName = lastName
                },
                // Initialize Imagepath with the profile image
                Imagepath = profileImage
            };

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View(testimonial);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("TestimonialId,TestimonialText,ApprovalStatus,ImageFile,Imagepath")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                if (testimonial.ImageFile != null)
                {
                    string wwwRootPath = webHostEnvironment.WebRootPath;

                    string fileName = Guid.NewGuid().ToString() + testimonial.ImageFile.FileName;

                    string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await testimonial.ImageFile.CopyToAsync(fileStream);
                    }

                    testimonial.Imagepath = fileName;
                }

                // Set UserId based on the session
                testimonial.UserId = HttpContext.Session.GetInt32("UserId");

                // Set an appropriate initial status for ApprovalStatus
                testimonial.ApprovalStatus = "Pending";
                testimonial.Imagepath = HttpContext.Session.GetString("ProfileImage"); ;


                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(testimonial);
        }


        //[HttpPost]
        //public async Task<IActionResult> Create([Bind("TestimonialId,UserId,TestimonialText,ApprovalStatus,ImageFile,Imagepath")] Testimonial testimonial)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Handle image file upload as before

        //        if (testimonial.ImageFile != null)
        //        {
        //            string wwwRootPath = webHostEnvironment.WebRootPath;

        //            string fileName = Guid.NewGuid().ToString() + testimonial.ImageFile.FileName;

        //            string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

        //            using (var fileStream = new FileStream(path, FileMode.Create))
        //            {
        //                await testimonial.ImageFile.CopyToAsync(fileStream);
        //            }

        //            testimonial.Imagepath = fileName;
        //        }

        //        _context.Add(testimonial);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Update UserId, TestimonialText, and ApprovalStatus
        //    testimonial.UserId = HttpContext.Session.GetInt32("UserId");
        //        testimonial.TestimonialText = "Your custom testimonial text, if any";
        //        testimonial.ApprovalStatus = "Pending"; // Set an appropriate initial status

        //        // Rest of your code for saving the testimonial

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", testimonial.UserId);
        //    return View(testimonial);
        //}


        // GET: Testimonials/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", testimonial.UserId);
            return View(testimonial);
        }

        // POST: Testimonials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("TestimonialId,UserId,TestimonialText,ApprovalStatus,ImageFile,Imagepath")] Testimonial testimonial)
        {
            if (id != testimonial.TestimonialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (testimonial.ImageFile != null)
                    {
                        string wwwRootPath = webHostEnvironment.WebRootPath;

                        string fileName = Guid.NewGuid().ToString() + testimonial.ImageFile.FileName;

                        string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await testimonial.ImageFile.CopyToAsync(fileStream);
                        }

                        testimonial.Imagepath = fileName;
                    }
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.TestimonialId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", testimonial.UserId);
            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TestimonialId == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Testimonials == null)
            {
                return Problem("Entity set 'ModelContext.Testimonials'  is null.");
            }
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(decimal id)
        {
          return (_context.Testimonials?.Any(e => e.TestimonialId == id)).GetValueOrDefault();
        }



    }
}
