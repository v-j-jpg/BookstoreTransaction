using CommonLibrary.Interface;
using CommonLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Validate([FromForm]Book model)
        {
            try
            {
                IValidation proxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookstoreTransaction/ValidationStatelessService"));

                ViewBag.BookIsValid = "danger";
                ViewBag.AlertMessage = "Unfortunately there was an error during the transaction.";

                if (await proxy.Validation(model))
                {
                    ViewBag.BookIsValid = "success";
                    ViewBag.AlertMessage = "You have succesfully bought a book!";
                }

                return View("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in communication with service " + ex.Message);
            }
        }
    }
}