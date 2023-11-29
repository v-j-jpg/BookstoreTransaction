using CommonLibrary.Interface;
using CommonLibrary.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : Controller
    {
        // GET: Books/List
        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List()
        {
            try
            {
                IValidation proxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookstoreTransaction/ValidationStatelessService"));

                var books = new List<Book>();

                List<string> booksJson = await proxy.GetAllBooks();

                booksJson.ForEach(x => books.Add(JsonConvert.DeserializeObject<Book>(x)!));

                return View(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in communication with service " + ex.Message);
            }

        }

        [HttpGet]
        [Route("Payment")]
        public async Task<IActionResult> Payment(long id)
        {
            IValidation proxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookstoreTransaction/ValidationStatelessService"));
            var book = await proxy.GetBook(id);
            var bookCustomerView = JsonConvert.DeserializeObject<BookCustomer>(book);


            //Check if the book is valid
            var isBookValid = await proxy.Validation(JsonConvert.DeserializeObject<Book>(book!));

            //Display info for the customer
            ViewBag.isBookValid = isBookValid ? "success" : "danger";
            ViewBag.AlertMessage = isBookValid ? "Book is available" : "Book is currently not available!";


            return View(bookCustomerView);
        }

        [HttpPost]
        [Route("Buy")]
        public async Task<IActionResult> Buy([FromForm] BookCustomer bookCustomer)
        {
            IValidation proxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/BookstoreTransaction/ValidationStatelessService"));
           
            //Check client (Validate if he is a client of the bank) //If so, return his bank acc (with money)
            var client = await proxy.GetValidClient(JsonConvert.DeserializeObject<Customer>(JsonConvert.SerializeObject(bookCustomer)));


            //PROCESS WITH PAYMENT
            ITransactionCoordinator transactionCoordinator = ServiceProxy.Create<ITransactionCoordinator>(new Uri("fabric:/BookstoreTransaction/TransactionCoordinator"), new ServicePartitionKey(1));
            
            try
            {
                if (!client.Equals(string.Empty) && await transactionCoordinator.Prepare(bookCustomer.BookID, bookCustomer.BankCardNumber, bookCustomer.Count))
                {
                    await transactionCoordinator.Commit();
                    ViewBag.Client = true;
                    return View("Index");
                }
     
            }
            catch (Exception ex)
            {
                //Some service stoped working
                await transactionCoordinator.Rollback();
            }

            //Transaction didn't work (no money or books in stock)
            ViewBag.Client = false;

            return View("Index");
        }

        // GET: BooksController/Details/5
        [HttpGet]
        [Route("Details")]
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: BooksController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BooksController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BooksController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
