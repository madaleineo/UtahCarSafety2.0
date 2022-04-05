using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UtahCarSafety.Models;
using UtahCarSafety.Models.ViewModels;

namespace UtahCarSafety.Controllers
{
    public class HomeController : Controller
    {

        private ICrashesRepository repo { get; set; }

        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        //Constructor
        public HomeController(UserManager<IdentityUser> um, SignInManager<IdentityUser> sim, ICrashesRepository temp)
        {
            userManager = um;
            signInManager = sim;
            repo = temp;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(loginModel.Username);

                if (user != null)
                {
                    await signInManager.SignOutAsync();

                    if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
                    {
                        return Redirect(loginModel?.ReturnUrl ?? "/Admin");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid Username or Password");
            return View(loginModel);
        }
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();

            return Redirect(returnUrl);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //public IActionResult Dataset(string CITY, int pageNum = 1)
        public IActionResult Dataset(string CITY)
        {
            int numResults = 75;

            var c = new CrashViewModel
            {
                Crashes = repo.Crashes
                .Include("City")
                .Where(c => c.CITY == CITY || CITY == null)
                .OrderByDescending(c => c.CRASH_DATETIME)
                //.Skip((pageNum - 1) * numResults)
                .Take(numResults),

                //PageInfo = new PageInfo
                //{
                //    TotalNumRecords =
                //        (CITY == null
                //            ? repo.Crashes.Count()
                //            : repo.Crashes.Where(x => x.City.CITY == CITY).Count()),
                //    RecordsPerPage = numResults,
                //    CurrentPage = pageNum,
                //}
            };

            //ViewData["TeamName"] = teamName;

            return View(c);

        }

        public IActionResult KeyStats()
        {
            return View();
        }

        public IActionResult CrashScenarios()
        {
            return View();
        }

        public IActionResult SeverityCalculator()
        {
            return View();
        }

        public IActionResult ViewStories()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddCrash()
        {
            ViewBag.Cities = repo.Cities.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddCrash(Crash c)
        {
            ViewBag.Cities = repo.Cities.ToList();

            if (ModelState.IsValid)
            {
                repo.CreateCrash(c);
                repo.SaveCrash(c);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult EditCrash(int id)
        {
            ViewBag.Cities = repo.Cities.ToList();

            var editCrash = repo.Crashes.Single(x => x.CRASH_ID == id);
            return View("EditCrash", editCrash);
        }

        [HttpPost]
        public IActionResult EditCrash(Crash c)
        {
            ViewBag.Cities = repo.Cities.ToList();

            if (ModelState.IsValid)
            {
                repo.EditCrash(c);
                repo.SaveCrash(c);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult DeleteCrash(int id)
        {

            var deleteCrash = repo.Crashes.Single(x => x.CRASH_ID == id);
            return View(deleteCrash);
        }

        [HttpPost]
        public IActionResult DeleteCrash(Crash c)
        {

            repo.DeleteCrash(c);
            return RedirectToAction("Dataset");
        }


    }
}
