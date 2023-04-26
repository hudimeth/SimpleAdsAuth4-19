using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using SimpleAdsAuth4_19.Web.Models;
using System.Diagnostics;

namespace SimpleAdsAuth4_19.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"data source = .\sqlexpress;initial catalog=SimpleAdsAuth;integrated security=true;";
        public IActionResult Index()
        {
            var repo = new SimpleAdsRepository(_connectionString);
            var vm = new IndexViewModel
            {
                Ads = repo.GetAllAds()
            };

            if (User.Identity.IsAuthenticated)
            {
                var currentUserEmail = User.Identity.Name;
                var currentUserId = repo.GetUserByEmail(currentUserEmail).Id;
                vm.CurrentUserId = currentUserId;
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("account", "login");
            }
            var userEmail = User.Identity.Name;
            var repo = new SimpleAdsRepository(_connectionString);
            var currentUser = repo.GetUserByEmail(userEmail);
            ad.Date = DateTime.Now;
            ad.UserId = currentUser.Id;
            repo.NewAd(ad);
            return RedirectToAction("index", "home");
        }

    }
}