using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Toeicking2021.Controllers
{
    public class DynamicLink : Controller
    {
        public IActionResult Index(string email)
        {
            if (email=="test")
            {
                ViewBag.hint = email;
                return View();
            }
            else
            {
                ViewBag.hint = "failed";
                return View();
            }
            
        }
    }
}
