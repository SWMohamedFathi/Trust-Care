using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrustCare.Models;

namespace TrustCare.Controllers
{
    public class BeneficiariesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BeneficiariesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: Beneficiaries
        public async Task<IActionResult> Index()
        {
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
            var modelContext = _context.Beneficiaries.Include(b => b.Subscription);
            return View(await modelContext.ToListAsync());
        }




        public async Task<IActionResult> AcceptBeneficiary(decimal BeneficiaryId)
        {
            var Beneficiary = await _context.Beneficiaries.FindAsync(BeneficiaryId);

            if (Beneficiary != null)
            {
                Beneficiary.ApprovalStatus = "Accepted";
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {

                return RedirectToAction("Make sure you have a Request");
            }
        }

        public async Task<IActionResult> RejectBeneficiary(decimal BeneficiaryId)
        {
            var Beneficiary = await _context.Beneficiaries.FindAsync(BeneficiaryId);

            if (Beneficiary != null)
            {
                Beneficiary.ApprovalStatus = "Rejected";
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {

                return RedirectToAction("Make sure you have a Request");
            }
        }

        // GET: Beneficiaries/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Beneficiaries == null)
            {
                return NotFound();
            }

            var beneficiary = await _context.Beneficiaries
                .Include(b => b.Subscription)
                .FirstOrDefaultAsync(m => m.BeneficiaryId == id);
            if (beneficiary == null)
            {
                return NotFound();
            }

            return View(beneficiary);
        }

        // GET: Beneficiaries/Create
        public IActionResult Create( decimal? SubscriptionId)
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


            //var subscription = _context.Subscriptions.Where(x => x.SubscriptionId == SubscriptionId).FirstOrDefault();

            //if (subscription != null)
            //{
            //    HttpContext.Session.SetInt32("SubscriptionId", (int)subscription.SubscriptionId);
            //    var beneficiary = new Beneficiary
            //    {
            //        SubscriptionId = subscription.SubscriptionId
            //    };

            //    return View(beneficiary);
            //}

          
            return View(); 
                                                   //ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                                                   //ViewBag.LastName = HttpContext.Session.GetString("LastName");
                                                   //ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");


            //var beneficiary = new Beneficiary
            //{

            //    Subscription = new Subscription
            //    {
            //        UserId = ViewBag.UserId = HttpContext.Session.GetInt32("UserId"),

            //    }


            //};

            //return View(beneficiary);
            ////ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId");
            //return View();
        }

        // POST: Beneficiaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeneficiaryId,SubscriptionId,Relationship,ProofDocument,ApprovalStatus,Proof_Document")] Beneficiary beneficiary)
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
            ViewBag.CurrentTime = DateTime.Now;

            var userid = HttpContext.Session.GetInt32("UserId");
            var subscription = _context.Subscriptions.FirstOrDefault(sub => sub.UserId == userid);


            if(subscription == null)
            {
                TempData["NOTSubed"] = "You Must be subscribed first";
                return RedirectToAction("Create", "Subscriptions");
            }


            if (ModelState.IsValid)
            {
                if (beneficiary.Proof_Document != null)
                {
                    string wwwRootPath = webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + beneficiary.Proof_Document.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await beneficiary.Proof_Document.CopyToAsync(fileStream);
                    }

                    beneficiary.ProofDocument = fileName;
                }


                beneficiary.SubscriptionId = ViewBag.SubscriptionId = HttpContext.Session.GetInt32("SubscriptionId");


                beneficiary.ApprovalStatus = "Pending";

                _context.Add(beneficiary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(beneficiary);
            //if (ModelState.IsValid)
            //{
            //     if (beneficiary.Proof_Document != null)
            //    {
            //        string wwwRootPath = webHostEnvironment.WebRootPath;

            //        string fileName = Guid.NewGuid().ToString() + beneficiary.Proof_Document.FileName;

            //        string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

            //        using (var fileStream = new FileStream(path, FileMode.Create))
            //        {
            //            await beneficiary.Proof_Document.CopyToAsync(fileStream);
            //        }

            //        beneficiary.ProofDocument = fileName;
            //    }

            //    // Set UserId based on the session
            //    beneficiary.SubscriptionId = HttpContext.Session.GetInt32("SubscriptionId");

            //    // Set an appropriate initial status for ApprovalStatus
            //    beneficiary.ApprovalStatus = "Pending";



            //    _context.Add(beneficiary);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            //return View(beneficiary);
            //if (ModelState.IsValid)
            //{
            //    if (beneficiary.Proof_Document != null)
            //    {
            //        string wwwRootPath = webHostEnvironment.WebRootPath;

            //        string fileName = Guid.NewGuid().ToString() + beneficiary.Proof_Document.FileName;

            //        string path = Path.Combine(wwwRootPath + "/Images/" + fileName);

            //        using (var fileStream = new FileStream(path, FileMode.Create))
            //        {
            //            await beneficiary.Proof_Document.CopyToAsync(fileStream);
            //        }

            //        beneficiary.ProofDocument = fileName;
            //    }
            //    _context.Add(beneficiary);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", beneficiary.SubscriptionId);
            //return View(beneficiary);
        }

        // GET: Beneficiaries/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Beneficiaries == null)
            {
                return NotFound();
            }

            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary == null)
            {
                return NotFound();
            }
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", beneficiary.SubscriptionId);
            return View(beneficiary);
        }

        // POST: Beneficiaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("BeneficiaryId,SubscriptionId,Relationship,ProofDocument,ApprovalStatus")] Beneficiary beneficiary)
        {
            if (id != beneficiary.BeneficiaryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beneficiary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficiaryExists(beneficiary.BeneficiaryId))
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
            ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "SubscriptionId", "SubscriptionId", beneficiary.SubscriptionId);
            return View(beneficiary);
        }

        // GET: Beneficiaries/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Beneficiaries == null)
            {
                return NotFound();
            }

            var beneficiary = await _context.Beneficiaries
                .Include(b => b.Subscription)
                .FirstOrDefaultAsync(m => m.BeneficiaryId == id);
            if (beneficiary == null)
            {
                return NotFound();
            }

            return View(beneficiary);
        }

        // POST: Beneficiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Beneficiaries == null)
            {
                return Problem("Entity set 'ModelContext.Beneficiaries'  is null.");
            }
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary != null)
            {
                _context.Beneficiaries.Remove(beneficiary);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeneficiaryExists(decimal id)
        {
          return (_context.Beneficiaries?.Any(e => e.BeneficiaryId == id)).GetValueOrDefault();
        }
    }
}
