using CommonLibrary.Interface;
using CommonLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List()
        {
            try
            {
                IValidation proxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookstoreTransaction/ValidationStatelessService"));

                var listClients = new List<Customer>();
                List<string> clients = await proxy.GetAllClients();

                clients.ForEach(x => listClients.Add(JsonConvert.DeserializeObject<Customer>(x)!));
                return View(listClients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in communication with service " + ex.Message);
            }

        }
    }
}
