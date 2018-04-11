using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CarRentalProject.Models;
using CarRentalProject.Data;
using CarRentalProject.Services;
using System.Net.Mail;
using System.Net;

namespace CarRentalProject.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          ApplicationDbContext context
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                
            };

            return View(model);
        }
        public async Task<IActionResult> SendOrder()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            MailAddress addressFrom = new MailAddress("avanesovbest@gmail.com", "Аванесов Артем");

            MailAddress addressTo = new MailAddress(user.Email);

            MailMessage m = new MailMessage(addressFrom, addressTo);

            m.Subject = "Информацию о заказах";

            var rents = _context.Rents.Where(x => x.UserID == user.Id);

            foreach (var rent in rents)

            {
                var auto = _context.Auto.First(x => x.ID == rent.CarID);
                m.Body = string.Format("<div><br>Информация о заказе: <br>" +
                                   "Марка машины: {0} <br>" +
                                   "Модель: {1}<br>" +
                                    "Год выпуска: {2}<br>" +
                                    "Цвет: {3}<br>" +
                                    "Дата начала аренды: {4}<br>" +
                                    "Дата окончания аренды: {5}<br>" +

                                   "</div>", auto.Brand, auto.Model, auto.Year, auto.Color, rent.StartDate, rent.EndDate);

                m.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                smtp.Credentials = new NetworkCredential("avanesovbest@gmail.com", "awanesow1996");
                smtp.EnableSsl = true;
                smtp.Send(m);

                

            }
            StatusMessage = "All rents are sended";
            
            return RedirectToAction("Index","Home");
        }





        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("CarRentalProject"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}
