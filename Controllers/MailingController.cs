using Microsoft.AspNetCore.Mvc;
using TrustCare.Dtos;
using TrustCare.Services;

namespace TrustCare.Controllers
{ 
        [Route("api/[controller]")]
        [ApiController]
        public class MailingController : ControllerBase
        {
            private readonly MailingService _mailingService;

            public MailingController(MailingService mailingService)
            {
                _mailingService = mailingService;
            }

            [HttpPost("send")]
            public async Task<IActionResult> SendMail([FromForm] MailRequestDto dto)
            {
                await _mailingService.SendEmailAsync(dto.ToEmail, dto.Subject, dto.Body, dto.Attachments);
                return Ok();
            }

            [HttpPost("welcome")]
            public async Task<IActionResult> SendWelcomeEmail([FromBody] WelcomeRequestDto dto)
            {
                var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\EmailTemplate.html";
                var str = new StreamReader(filePath);

                var mailText = str.ReadToEnd();
                str.Close();

                mailText = mailText.Replace("[username]", dto.UserName).Replace("[email]", dto.Email);

                await _mailingService.SendEmailAsync(dto.Email, "Welcome to our channel", mailText);
                return Ok();
            }
        }
    }
