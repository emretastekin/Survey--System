using Microsoft.AspNetCore.Mvc;
using SurveyApp.Data;
using SurveyApp.Models;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SurveyApp.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;

        public SurveyController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSurvey(string firstName, string lastName, string email, IFormFile image)
        {
            var surveyResponse = new Survey
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            string imagePath = null;
            if (image != null && image.Length > 0)
            {
                imagePath = Path.Combine("wwwroot/images", Path.GetFileName(image.FileName));
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                surveyResponse.ImagePath = imagePath;
            }

            _context.Surveys.Add(surveyResponse);
            await _context.SaveChangesAsync();

            // E-posta gönderme
            var smtpServer = _configuration["Smtp:Server"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUsername = _configuration["Smtp:Username"];
            var smtpPassword = _configuration["Smtp:Password"];

            using (var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUsername),
                    Subject = "New Survey Response",
                    Body = $@"
                    <html>
                    <body>
                        <h1>New Survey Response</h1>
                        <p><strong>First Name:</strong> {firstName}</p>
                        <p><strong>Last Name:</strong> {lastName}</p>
                        <p><strong>Email:</strong> {email}</p>
                    </body>
                    </html>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                if (!string.IsNullOrEmpty(imagePath))
                {
                    var imageAttachment = new Attachment(imagePath, "image/jpeg");
                    mailMessage.Attachments.Add(imageAttachment);
                }

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Hata yönetimi: E-posta gönderiminde bir sorun oluştu.
                    Console.WriteLine($"E-posta gönderim hatası: {ex.Message}");
                }
            }

            return RedirectToAction("Index");
        }
    }
}
