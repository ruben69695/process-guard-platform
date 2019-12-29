using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Process_Guard_Admin.Models;

namespace Process_Guard_Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Process Guard Admin ASP.NET Core Web Application";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Rubén Arrebola de Haro";
            ViewData["Twitter"] = "https://twitter.com/rarrebolaedcm15";
            ViewData["GitHub"] = "https://github.com/ruben69695";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
