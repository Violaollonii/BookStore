using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ReportController : Controller
    {
        [HttpPost]
        public IActionResult SendTopBooksToAdmin(string topBooks)
        {
            var body = $"Top 3 Librat më fitimprurës:\n\n{topBooks.Replace(",", "\n")}";

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("viola1gmail@gmail.com", "Viola2004."),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("viola1gmail@gmail.com"),
                    Subject = "Top Librat e Muajit nga Punonjësi",
                    Body = body,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add("viola1gmail@gmail.com"); // Zëvendëso me email-in e adminit

                smtpClient.Send(mailMessage);

                TempData["success"] = "Email-i me top librat u dërgua me sukses!";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Dështoi dërgimi i email-it: {ex.Message}";
            }

            return RedirectToAction("AllBooks", "Home");
        }
    }
}
