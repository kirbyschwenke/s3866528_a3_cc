using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using s3866528_a3.Models;
using System.Diagnostics;

namespace s3866528_a3.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<HomeController> _logger;
        private readonly RecipesApi _recipesApi;

        public HomeController(IAmazonDynamoDB dynamoDbClient, ILogger<HomeController> logger, RecipesApi recipesApi)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
            _recipesApi = recipesApi;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch healthy recipes from the Tasty API
                var recipes = await _recipesApi.GetRecipesList(0, 3, "healthy");

                // Display the first three recipes on the home page
                return View(recipes);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error view or message
                _logger.LogError(ex, "An error occurred while fetching recipes.");
                return View("Error");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    // Handle the case where email or password is empty.
                    ModelState.AddModelError("Email", "Email or Password can't be empty.");
                    return View("Login");
                }

                var table = Table.LoadTable(_dynamoDbClient, "login");

                var search = table.Query(new QueryOperationConfig
                {
                    Filter = new QueryFilter("Email", QueryOperator.Equal, email)
                });

                var document = await search.GetNextSetAsync();

                if (document.Any())
                {
                    // Retrieve the password from the DynamoDB document.
                    string storedPassword = document[0]["Password"].AsString();

                    // Compare the stored password with the entered password.
                    if (string.Equals(storedPassword, password))
                    {
                        // Store user information in the session.
                        HttpContext.Session.SetString("UserEmail", document[0]["Email"].AsString());
                        HttpContext.Session.SetString("UserName", document[0]["Name"].AsString());

                        return RedirectToAction("Index");
                    }
                }

                ModelState.AddModelError("Email", "Email or password is invalid.");
                if (!ModelState.IsValid)
                {
                    return View("Login");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Login.");
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Login model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var table = Table.LoadTable(_dynamoDbClient, "login");

                    // Check if the email is already registered.
                    var search = table.Query(new QueryOperationConfig
                    {
                        Filter = new QueryFilter("Email", QueryOperator.Equal, model.Email)
                    });

                    var document = await search.GetNextSetAsync();

                    if (document.Any())
                    {
                        ModelState.AddModelError("Email", "Email is already registered.");
                        return View("Register");
                    }

                    // Create a new user in DynamoDB.
                    var user = new Document();
                    user["Email"] = model.Email;
                    user["Name"] = model.Name;
                    user["Password"] = model.Password;

                    await table.PutItemAsync(user);

                    // Redirect to the home page after successful registration.
                    return RedirectToAction("Index");
                }

                // If model validation fails, return the registration form.
                return View("Register");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return RedirectToAction("Index");
            }
        }

        public IActionResult Logout()
        {
            // Logout user.
            HttpContext.Session.Clear();

            return View("Index");
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
    }
}