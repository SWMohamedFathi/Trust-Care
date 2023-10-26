using System;
using System.Net;
//using System.Net.Mail;
using System.Threading.Tasks;
using Humanizer;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using MimeKit;
using TrustCare.Models;
using TrustCare.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;

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

            var Beneficiar = _context.Beneficiaries.ToList();
            var Subscrip = _context.Subscriptions.ToList();
            var user = _context.Users.ToList();



            var model = from b in Beneficiar where b.BeneficiaryId == BeneficiaryId
                        join s in Subscrip on b.SubscriptionId equals s.SubscriptionId
                        join u in user on s.UserId equals u.UserId
                        select new { Beneficiar = b, Subscrip = s, User = u };

            var selectedUserEmail = model.FirstOrDefault()?.User?.Email;
            var selectedRelation = model.FirstOrDefault()?.Beneficiar.Relationship;



            if (Beneficiary != null)
            {
                Beneficiary.ApprovalStatus = "Accepted";
                await _context.SaveChangesAsync();
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("dev.mohamedfathi@gmail.com"));
                email.To.Add(MailboxAddress.Parse(selectedUserEmail));
                email.Subject = "Beneficiary has been added";
                //email.Body = new TextPart(TextFormat.Plain) { Text = "Example Plain Text Message Body" };

                ViewBag.CurrentTime = DateTime.Now;
                var pdf = GeneratePDF(ViewBag.FirstName, ViewBag.CurrentTime, selectedRelation);

                var multipart = new Multipart("mixed");

                var text = new TextPart(TextFormat.Plain)
                {
                    Text = "Thank you for your subscription. Please find the attached in email.",
                };

                multipart.Add(text);

                var attachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(System.IO.File.OpenRead(pdf)), // Attach the PDF here
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(pdf), // Set the file name
                };

                multipart.Add(attachment);

                email.Body = multipart;

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("devmohamedfathi@gmail.com", "hzyj mzsn sstr sakn");
                smtp.Send(email);
                smtp.Disconnect(true);
                //_context.Add(beneficiary);
                //await _context.SaveChangesAsync();

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
                _context.Beneficiaries.Remove(Beneficiary);  

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
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
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

        public string GeneratePDF(string SubName, DateTime SubDate ,string relation)
        {

            // Generate a unique file name for the PDF
            string pdfName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");


            // Create a new document with A4 size and margins
            Document document = new Document(PageSize.A4, 200, 200, 200, 200);

            try
            {
                // Create a PDF writer and open the document for writing
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfName, FileMode.Create));
                document.Open();


                PdfContentByte cb = writer.DirectContent;
                cb.SetLineWidth(2); // Set the border line width
                cb.Rectangle(36, 36, PageSize.A4.Width - 72, PageSize.A4.Height - 72); // Adjust coordinates as needed
                cb.Stroke();
                // Add content to the PDF
                document.Add(new Paragraph("TrustCare"));
                document.Add(Chunk.NEWLINE);
                document.Add(Chunk.NEWLINE);
                document.Add(Chunk.NEWLINE);
                document.Add(new Paragraph("Your request to add your beneficiary has been accepted"));
                document.Add(Chunk.NEWLINE);
                document.Add(new Paragraph("Relation of Subscribed: " + relation));
                document.Add(new Paragraph("Date added: " + SubDate.ToString("yyyy-MM-dd")));
                //document.Add(new Paragraph("SubscriptionAmount: $" + Subamount.ToString("0.00")));

                // Close the document and writer
                document.Close();
                writer.Close();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during PDF generation
                Console.WriteLine("Error: " + ex.Message);
                return null; // Return null to indicate an error
            }

            return pdfName; // Return the file path to the generated PDF
        }


        public IActionResult Create(decimal? SubscriptionId)
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
            ViewBag.SubscriptionId = HttpContext.Session.GetInt32("SubscriptionId");


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


            ViewBag.SubscriptionId = HttpContext.Session.GetInt32("SubscriptionId");
        

            //if (subscription == null)
            //{
            //    ViewBag.NotSub = "You Must be subscribed";
            //    return RedirectToAction("Create", "Banks");
            //}
            HttpContext.Session.SetInt32("SubscriptionId", (int)subscription.SubscriptionId);


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


                beneficiary.SubscriptionId =  HttpContext.Session.GetInt32("SubscriptionId");


                beneficiary.ApprovalStatus = "Pending";
                _context.Beneficiaries.Add(beneficiary);
                _context.SaveChanges();





            }


            return View(beneficiary);

        }


     

        // GET: Beneficiaries/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
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
