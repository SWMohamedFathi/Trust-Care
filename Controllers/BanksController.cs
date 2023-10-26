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
    public class BanksController : Controller
    {
        private readonly MailingService _mailingService;
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;


        public BanksController(ModelContext context, MailingService mailingService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mailingService = mailingService;
            this.webHostEnvironment = webHostEnvironment;


        }

        public string GeneratePDF(string SubName, DateTime SubDate, decimal Subamount)
        {

            // Generate a unique file name for the PDF
            string pdfFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");

            // Create a new document with A4 size and margins
            Document document = new Document(PageSize.A4, 200, 200, 200, 200);

            try
            {
                // Create a PDF writer and open the document for writing
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfFileName, FileMode.Create));
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
                document.Add(new Paragraph("SubscriptionName: " + SubName));
                document.Add(new Paragraph("SubscriptionDate: " + SubDate.ToString("yyyy-MM-dd")));
                document.Add(new Paragraph("SubscriptionAmount: $" + Subamount.ToString("0.00")));

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

            return pdfFileName; // Return the file path to the generated PDF
        }


     
        // GET: Banks
        public async Task<IActionResult> Index()
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
            
            return _context.Banks != null ? 
                          View(await _context.Banks.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Banks'  is null.");
        }

        // GET: Banks/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Banks == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks
                .FirstOrDefaultAsync(m => m.BankId == id);
            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        // GET: Banks/Create
        public IActionResult Create()
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
            return View();


        }

        // POST: Banks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BankId,Owner,CardNumber,Cvv,Balance")] Bank bank)
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


            var userId = HttpContext.Session.GetInt32("UserId");

           

            var userInBank = _context.Banks.FirstOrDefault(b => b.CardNumber == b.CardNumber && b.Cvv == b.Cvv);
          

            var subscribed = _context.Subscriptions.FirstOrDefault(s => s.UserId == userId);

            if (subscribed != null)
            {

                ViewBag.notsubscribed = "notsubscribed";

                return View("PaymentFailure");
            }
            if (userInBank != null && userInBank.Balance >= 40)
            {


                userInBank.Balance -= 40;


                var subscription = new Subscription
                {
                    UserId = userId, 
                    SubscriptionDate = DateTime.Now,
                    SubscriptionAmount = 40,
                    PaymentStatus = "Paid",
                    PaymentMethod = "Visa"
                };
                _context.Subscriptions.Add(subscription);
                _context.SaveChanges();

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("dev.mohamedfathi@gmail.com"));
                email.To.Add(MailboxAddress.Parse(HttpContext.Session.GetString("Email")));
                email.Subject = "Welcome to Trustcare";
                //email.Body = new TextPart(TextFormat.Plain) { Text = "Example Plain Text Message Body" };

                ViewBag.CurrentTime = DateTime.Now;
                var pdf = GeneratePDF(ViewBag.FirstName, ViewBag.CurrentTime, 40);

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
                

       
                return View("PaymentSuccess","Subscriptions");
            }
            else
            {
                
                return View("PaymentFalier");
            }

 
        }
     
        // GET: Banks/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Banks == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }
            return View(bank);
        }

        // POST: Banks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("BankId,Owner,CardNumber,Cvv,Balance")] Bank bank)
        {
            if (id != bank.BankId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankExists(bank.BankId))
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
            return View(bank);
        }

        // GET: Banks/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Banks == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks
                .FirstOrDefaultAsync(m => m.BankId == id);
            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Banks == null)
            {
                return Problem("Entity set 'ModelContext.Banks'  is null.");
            }
            var bank = await _context.Banks.FindAsync(id);
            if (bank != null)
            {
                _context.Banks.Remove(bank);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankExists(decimal id)
        {
          return (_context.Banks?.Any(e => e.BankId == id)).GetValueOrDefault();
        }



    }
}
