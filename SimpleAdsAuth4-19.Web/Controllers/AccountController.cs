using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using SimpleAdsAuth4_19.Web.Models;
using System.Security.Claims;

namespace SimpleAdsAuth4_19.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data source=.\sqlexpress;Initial catalog=SimpleAdsAuth;Integrated Security=true;";
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var repo = new SimpleAdsRepository(_connectionString);
            repo.AddUser(user, password);
            return Redirect("/account/login");
        }
        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new SimpleAdsRepository(_connectionString);
            var user = repo.Login(email, password);

            if(user == null)
            {
                TempData["message"] = "Invalid Login";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>
            {
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role")))
                .Wait();
            return RedirectToAction("newad", "home");
        }
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new SimpleAdsRepository(_connectionString);
            var userEmail = User.Identity.Name;
            var user = repo.GetUserByEmail(userEmail);
            var vm = new MyAccountViewModel
            {
                Ads = repo.GetAdsForUser(user.Id)
            };
            return View(vm);
        }
    }
}
