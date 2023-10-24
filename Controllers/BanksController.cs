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


namespace TrustCare.Controllers
{
    public class BanksController : Controller
    {
        private readonly MailingService _mailingService;
        private readonly ModelContext _context;

        public BanksController(ModelContext context, MailingService mailingService)
        {
            _context = context;
            _mailingService = mailingService;

        }
        [HttpPost]
        public IActionResult ProcessSubscriptionPayment([Bind("BankId,Owner,CardNumber,Cvv,Balance")] Bank bank) 
        {
            // Assuming VisaData is a class that represents the user's input from the form

            // Check if the user exists in the Bank table
            var userInBank = _context.Banks.FirstOrDefault(b => b.CardNumber == b.CardNumber && b.Cvv == b.Cvv);
            var userId = HttpContext.Session.GetInt32("UserId");

            //var subscription = _context.Subscriptions.FirstOrDefault(s => s.UserId == userId);

            if (userInBank != null && userInBank.Balance >= 50)
            {
                // Deduct $50 from the user's balance in the Bank table
                userInBank.Balance -= 50;

                // Create a new Subscription record
                var subscription = new Subscription
                {
                    UserId = userId, // Assign the appropriate user ID
                    SubscriptionDate = DateTime.Now,
                    SubscriptionAmount = 50,
                    PaymentStatus = "Paid",
                    PaymentMethod = "Visa" // You can set the payment method accordingly
                };

                _context.Subscriptions.Add(subscription);
                _context.SaveChanges();

                // Redirect or return a success view
                return View("PaymentSuccess");
            }
            else
            {
                // Handle the case where the Visa data is not valid or the user's balance is insufficient
                return View("PaymentFailure");
            }
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

            var userInBank = _context.Banks.Where(b => b.CardNumber == b.CardNumber && b.Cvv == b.Cvv).FirstOrDefault();
            //var userInBank = _context.Banks.FirstOrDefault(b => b.CardNumber == b.CardNumber && b.Cvv == b.Cvv);
            //var userId = HttpContext.Session.GetInt32("UserId");

            //var subscription = _context.Subscriptions.FirstOrDefault(s => s.UserId == userId);

            if (userInBank != null )
            {


                userInBank.Balance -= 40;


                var subscription = new Subscription
                {
                    UserId = userId, // Assign the appropriate user ID
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
                email.Subject = "Test Email Subject2";
                email.Body = new TextPart(TextFormat.Plain) { Text = "Example Plain Text Message Body" };

                //email.Attachments = 

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("devmohamedfathi@gmail.com", "hzyj mzsn sstr sakn");
                smtp.Send(email);
                smtp.Disconnect(true);
                

                //string userEmail = "angularteamwork@gmail.com"; // Replace with the user's email
                //string subject = "Thank you for your subscription!";
                //string body = "Your subscription has been successfully processed. Thank you for choosing our service.";
                //await _mailingService.SendEmailAsync(userEmail, subject, body);

                //using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                //{
                //    smtpClient.Credentials = new NetworkCredential("devmohamedfathi@gmail.com", "hzyj mzsn sstr sakn");
                //    smtpClient.Port = 587;
                //    smtpClient.UseDefaultCredentials = false;
                //    smtpClient.EnableSsl = true;

                //    using (MailMessage message = new MailMessage())
                //    {
                //        message.From = new MailAddress("devmohamedfathi@gmail.com");
                //        message.To.Add(userEmail);
                //        message.Subject = subject;
                //        message.Body = body;
                //        message.IsBodyHtml = false;

                //        // Create a PDF invoice
                //        // You can use a library like iTextSharp or another PDF library to generate the invoice.

                //        // Attach the PDF invoice to the email
                //        // Replace "invoice.pdf" with the actual filename and path of your PDF invoice.
                //        //Attachment attachment = new Attachment("invoice.pdf", MediaTypeNames.Application.Pdf);
                //        //message.Attachments.Add(attachment);
                //        smtpClient.Send(message);

                //    }

                //    await _context.SaveChangesAsync();

                //}
                // Redirect or return a success view
                return View("PaymentSuccess");
            }
            else
            {
                // Handle the case where the Visa data is not valid or the user's balance is insufficient
                return View("PaymentFailure");
            }
            //if (ModelState.IsValid)
            //{
            //    _context.Add(bank);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(bank);
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
