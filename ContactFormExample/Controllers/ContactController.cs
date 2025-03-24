using ContactFormExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Mail;

namespace ContactFormExample.Controllers
{
    public class ContactController : Controller
    {
        private const string mailhost = "";
        private const string mailaccount = "";
        private const string mailfrom = "";
        private const string mailto = "";
        private const string mailpw = "";

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Message"] = "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                string errmsg = "Error: ";
                foreach (var error in allErrors)
                {
                    errmsg += error.ErrorMessage + "\n"; 
                }
                ViewData["Message"] = errmsg;
                return View("Index", model);
            }

            var emailMessage = new MailMessage(mailfrom, mailto)
            {
                Subject = "Contact form message",
                
                Body = $"Name: {model.Name}\nEmail: {model.Email}\nMessage:\n{model.Text}",

                IsBodyHtml = false
            };


            foreach (var attachment in model.Attachments ?? new List<IFormFile>())
            {
                if (attachment.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await attachment.CopyToAsync(stream);
                        emailMessage.Attachments.Add(new Attachment(stream, attachment.FileName));
                    }
                }
            }

            using (var smtpClient = new SmtpClient(mailhost))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(mailaccount, mailpw);
                smtpClient.Port = 587; // or 25, or 465
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(emailMessage);
            }

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
