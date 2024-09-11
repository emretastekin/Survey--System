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
        private readonly string _uploadsFolderPath;

        public SurveyController(ApplicationContext context)
        {
            _context = context;
            _uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(_uploadsFolderPath))
            {
                Directory.CreateDirectory(_uploadsFolderPath);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string surname, string email, string phoneNumber, string answer1, IFormFile? imageFile, int rating, bool yesNoAnswer)
        {
            if (ModelState.IsValid)
            {
                var surveyAnswer = new Survey
                {
                    Name = name,
                    Surname = surname,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Answer1 = answer1,
                    Rating= rating,
                    YesNoAnswer= yesNoAnswer
                };

                // Dosya yükleme işlemi
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(_uploadsFolderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    surveyAnswer.ImagePath = $"/uploads/{fileName}";
                }

                // Anketi veri tabanına ekle
                _context.Surveys.Add(surveyAnswer);
                await _context.SaveChangesAsync();

                // E-posta gönderimi
                await SendEmailAsync(surveyAnswer);

                return RedirectToAction("Success");
            }

            return View();
        }

        private async Task SendEmailAsync(Survey survey)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("emretastekin2369@gmail.com", "rxqg ilhc dqne xxym"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("emretastekin2369@gmail.com",$"{survey.Email}"),
                Subject = "Yeni Anket Yanıtı",
                Body = $"İsim: {survey.Name}\nSoyisim: {survey.Surname}\nTelefon: {survey.PhoneNumber}\nEmail: {survey.Email}\n" +
                       $"Soru 1: Bu bir sorudur.: {survey.Answer1}\n" +
                       $"Puanlama: {survey.Rating}\n" +
                       $"Evet/Hayır Cevabı: {(survey.YesNoAnswer == true ? "Evet" : "Hayır")}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add("emretastekin2369@gmail.com");

            mailMessage.ReplyToList.Add(new MailAddress(survey.Email));


            if (!string.IsNullOrEmpty(survey.ImagePath))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", survey.ImagePath.TrimStart('/'));
                var attachment = new Attachment(imagePath);
                mailMessage.Attachments.Add(attachment);
            }

            await smtpClient.SendMailAsync(mailMessage);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
