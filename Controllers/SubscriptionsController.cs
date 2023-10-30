


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using PayPal.Api;
using TrustCare.Models;

using MailKit.Security;

using MimeKit.Text;
using MimeKit;

using MailKit.Net.Smtp;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
namespace TrustCare.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ModelContext _context;
        private IHttpContextAccessor httpContextAccessor;

        IConfiguration _configuration;
        public SubscriptionsController(ModelContext context, IConfiguration iconfiguration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = iconfiguration;
            
            _context = context;
            this.httpContextAccessor = httpContextAccessor;

        }

       


        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");

            var modelContext = _context.Subscriptions.Include(s => s.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create
        //public IActionResult Create()
        //{
        //    ViewBag.FirstName = HttpContext.Session.GetString("FirstName");

        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
        //    return View();
        //}
        public IActionResult Create()
        {
            //ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            //ViewBag.LastName = HttpContext.Session.GetString("LastName");
            //ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");
            //ViewBag.CurrentTime = DateTime.Now;

            //var userNames = _context.Users.Select(u => new SelectListItem
            //{
            //    Value = u.UserId.ToString(),
            //    Text = u.FirstName
            //}).ToList();

            //ViewBag.UserNames = new SelectList(userNames, "Value", "Text");

            //return View();

            // Retrieve user information from the session
            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.LastName = HttpContext.Session.GetString("LastName");
            ViewBag.ProfileImage = HttpContext.Session.GetString("ProfileImage");


            var subscription = new Subscription
            {
              
                User = new User
                {
                    UserId = ViewBag.UserId = HttpContext.Session.GetInt32("UserId"),
                    FirstName = ViewBag.FirstName = HttpContext.Session.GetString("FirstName")

                }
             
            };

            return View(subscription);
        }


        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscriptionId,UserId,SubscriptionDate,SubscriptionAmount,PaymentStatus,PaymentMethod")] Subscription subscription)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(subscription);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", subscription.UserId);
            //return View(subscription);
            if (ModelState.IsValid)
            {

                // Set UserId based on the session
                subscription.UserId = HttpContext.Session.GetInt32("UserId");

             
                subscription.PaymentMethod = "Visa";
                subscription.SubscriptionDate = ViewBag.CurrentTime = DateTime.Now;

                subscription.SubscriptionAmount = 50 ;
            


                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           


            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", subscription.UserId);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("SubscriptionId,UserId,SubscriptionDate,SubscriptionAmount,PaymentStatus,PaymentMethod")] Subscription subscription)
        {
            if (id != subscription.SubscriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.SubscriptionId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", subscription.UserId);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Subscriptions == null)
            {
                return Problem("Entity set 'ModelContext.Subscriptions'  is null.");
            }
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(decimal id)
        {
            return (_context.Subscriptions?.Any(e => e.SubscriptionId == id)).GetValueOrDefault();
        }

    

        //Get
        public IActionResult Search()
        {
            var model = _context.Subscriptions.ToList();
            return View(model);

        }

        //post
        [HttpPost]
        public IActionResult Search(DateTime? StartDate, DateTime? EndDate)
        {

            var model = _context.Subscriptions.ToList();


            if (StartDate == null && EndDate == null)
            {
                return View(model);

            }
            else if (StartDate != null && EndDate == null)
            {

                var result = model.Where(x => x.SubscriptionDate.Value.Date >= StartDate);
                return View(result);


            }

            else if (StartDate == null && EndDate != null)
            {
                var result = model.Where(x => x.SubscriptionDate.Value.Date <= EndDate);
                return View(result);


            }

            else
            {

                var result = model.Where(x => x.SubscriptionDate.Value.Date >= StartDate && x.SubscriptionDate.Value.Date <= EndDate);
            }




            return View();

        }

        //public IActionResult PaymentWithPaypal(string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
        //{


        //    //getting the apiContext  
        //    var ClientID = _configuration.GetValue<string>("PayPal:Key");
        //    var ClientSecret = _configuration.GetValue<string>("PayPal:Secret");
        //    var mode = _configuration.GetValue<string>("PayPal:mode");
        //    APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
        //    // apiContext.AccessToken="Bearer access_token$production$j27yms5fthzx9vzm$c123e8e154c510d70ad20e396dd28287";
        //    try
        //    {
        //        //A resource representing a Payer that funds a payment Payment Method as paypal  
        //        //Payer Id will be returned when payment proceeds or click to pay  
        //        string payerId = PayerID;
        //        if (string.IsNullOrEmpty(payerId))
        //        {
        //            var userId = HttpContext.Session.GetInt32("UserId");

        //            // Create a new subscription 
        //            var subscription = new Subscription
        //            {
        //                UserId = userId,
        //                SubscriptionDate = DateTime.Now,
        //                SubscriptionAmount = 40,
        //                PaymentStatus = "Paid",
        //                PaymentMethod = "Visa"
        //            };
        //            // Add the subscription to the database.
        //            _context.Subscriptions.Add(subscription);
        //            _context.SaveChanges();
        //            //this section will be executed first because PayerID doesn't exist  
        //            //it is returned by the create function call of the payment class  
        //            // Creating a payment  
        //            // baseURL is the url on which paypal sendsback the data.  
        //            string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Home/PaymentWithPayPal?";
        //            //here we are generating guid for storing the paymentID received in session  
        //            //which will be used in the payment execution  
        //            var guidd = Convert.ToString((new Random()).Next(100000));
        //            guid = guidd;
        //            //CreatePayment function gives us the payment approval url  
        //            //on which payer is redirected for paypal account payment  
        //            var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);
        //            //get links returned from paypal in response to Create function call  
        //            var links = createdPayment.links.GetEnumerator();
        //            string paypalRedirectUrl = null;
        //            while (links.MoveNext())
        //            {
        //                Links lnk = links.Current;
        //                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
        //                {
        //                    //saving the payapalredirect URL to which user will be redirected for payment  
        //                    paypalRedirectUrl = lnk.href;
        //                }
        //            }
        //            // saving the paymentID in the key guid  
        //            httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
        //            return Redirect(paypalRedirectUrl);
        //        }
        //        else
        //        {
        //            // This function exectues after receving all parameters for the payment  

        //            var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
        //            var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
        //            //If executed payment failed then we will show payment failure message to user  
        //            if (executedPayment.state.ToLower() != "approved")
        //            {

        //                return View("PaymentFailed");
        //            }
        //            var blogIds = executedPayment.transactions[0].item_list.items[0].sku;


        //            return View("PaymentSuccess");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("PaymentFailed");
        //    }


        //}
        public IActionResult PaymentWithPaypal(string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
        {
            // Get the PayPal API credentials from app settings
            var ClientID = _configuration.GetValue<string>("PayPal:Key");
            var ClientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");

            // Initialize the PayPal API context
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);

            try
            {
                // Check if a PayerID exists, indicating a return from PayPal
                string payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    // User is initiating the payment

                    // Retrieve the user ID (you should have a valid session management mechanism)
                    var userId = HttpContext.Session.GetInt32("UserId");

                    // Create a new subscription
                    var subscription = new Subscription
                    {
                        UserId = userId,
                        SubscriptionDate = DateTime.Now,
                        SubscriptionAmount = 40, // Set the appropriate subscription amount
                        PaymentStatus = "Pending", // Initial status, it should be updated after payment
                        PaymentMethod = "PayPal" // Set the correct payment method
                    };

                    // Add the subscription to the database
                    _context.Subscriptions.Add(subscription);
                    _context.SaveChanges();

                    // Build the PayPal payment request
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Home/PaymentWithPayPal?";
                    guid = Guid.NewGuid().ToString(); // Use a GUID
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);

                    // Extract the PayPal approval URL
                    var approvalUrl = createdPayment.links
                        .FirstOrDefault(link => link.rel.ToLower() == "approval_url")?.href;

                    if (approvalUrl != null)
                    {
                        // Save the payment ID in the session
                        httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);

                        // Redirect the user to PayPal for payment
                        return Redirect(approvalUrl);
                    }
                    else
                    {
                        // Handle the case where no approval URL is available
                        return View("PaymentFailed");
                    }
                }
                else
                {
                    // User is returning from PayPal after payment

                    // Get the payment ID from the session
                    var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");

                    // Execute the PayPal payment
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        // Payment was not approved, handle accordingly
                        return View("PaymentFailed");
                    }

                    // Extract information from the PayPal response, e.g., blogId

                    // Update the payment status in your subscription
                    var subscription = _context.Subscriptions.FirstOrDefault(s => s.UserId == s.UserId && s.PaymentStatus == "Pending");
                    if (subscription != null)
                    {
                        subscription.PaymentStatus = "Paid";
                        // Update any other relevant data based on PayPal response
                        // For example, set subscription.BlogId = blogId;
                        _context.SaveChanges();
                    }

                    return View("PaymentSuccess");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle the error gracefully
                // ...
                return View("PaymentFailed");
            }
        }


        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }



        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
        {
            //create itemlist and add item objects to it  

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Item Detail",
                currency = "USD",
                price = "1",
                quantity = "1",
                sku = "asd"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            //var details = new Details()
            //{
            //    tax = "1",
            //    shipping = "1",
            //    subtotal = "1"
            //};
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = "1.00", // Total must be equal to sum of tax, shipping and subtotal.  
                //details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }



 

    }
}