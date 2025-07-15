using ContactFormExample.Models;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MimeKit;
using System.IO;


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

            var emailMessage = new MimeMessage()
            {
                Subject = "Contact form message",
            };

            emailMessage.From.Add(new MailboxAddress("Contact Form", mailfrom));
            emailMessage.To.Add(MailboxAddress.Parse(mailto));

            var multipart = new Multipart("mixed");

            var body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Name: {model.Name}\nEmail: {model.Email}\nMessage:\n{model.Text}"
            };
            multipart.Add(body);

            foreach (var attachment in model.Attachments ?? new List<IFormFile>())
            {
                if (attachment.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        var mimeAttachment = new MimePart(attachment.ContentType)
                        {
                            Content = new MimeContent(stream, ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = Path.GetFileName(attachment.FileName)
                        };
                        multipart.Add(mimeAttachment);
                    }
                }
            }

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(mailhost, 465, true);
                smtpClient.Authenticate(mailaccount, mailpw);
                smtpClient.Send(emailMessage);
                smtpClient.Disconnect(true);
            }

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
