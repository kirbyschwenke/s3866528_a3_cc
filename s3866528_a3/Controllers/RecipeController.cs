using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using s3866528_a3.Models;
using s3866528_a3.Services;
using System.Security.Claims;

namespace s3866528_a3.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DashboardController> _logger;
        private readonly RecipesApi _recipesApi;
        private readonly IRecipeService _recipeService;

        public RecipeController(IAmazonDynamoDB dynamoDbClient, ILogger<DashboardController> logger, RecipesApi recipesApi, IRecipeService recipeService)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
            _recipesApi = recipesApi;
            _recipeService = recipeService;
        }

        public IActionResult Index()
        {
            var model = new List<Recipe>();

            return View(model);
        }

        // Action to get healthy recipes
        public async Task<IActionResult> GetHealthyRecipes()
        {
            try
            {
                // Fetch healthy recipes from the Tasty API
                var recipes = await _recipesApi.GetRecipesList(0, 10, "healthy");

                // Return the recipes as a view or any other desired format
                return View("Index", recipes);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error view or message
                return View("Error");
            }
        }

        // Action to search recipes based on keywords
        public async Task<IActionResult> SearchRecipes(string keywords)
        {
            try
            {
                // Fetch recipes from the Tasty API based on user-entered keywords
                var recipes = await _recipesApi.GetRecipesByKeywords(keywords);

                // Return the recipes as a view or any other desired format
                return View("Index", recipes);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error view or message
                return View("Error");
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var recipe = await _recipesApi.GetRecipeById(id);

                if (recipe != null)
                {
                    return View("Details", recipe);
                }
                else
                {
                    // Log a message indicating that the recipe was not found
                    Console.WriteLine($"Recipe with ID {id} not found.");
                    return View("RecipeNotFound");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception, and return an error view or message
                Console.WriteLine($"An error occurred while fetching recipe by ID: {ex.Message}");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveRecipe(int recipeId)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");

                // Print the user's email to the console for debugging
                Console.WriteLine($"User Email in Session: {userEmail}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    // Handle the case where the user email is not available
                    return Json(new { success = false, message = "User not logged in." });
                }

                var savedRecipeIds = _recipeService.GetSavedRecipeIds(userEmail);

                if (savedRecipeIds.Contains(recipeId))
                {
                    // Recipe is already saved, handle this scenario if needed
                    return Json(new { success = false, message = "Recipe already saved." });
                }

                var table = Table.LoadTable(_dynamoDbClient, "recipes-saved");

                // Create a new item in the "recipes-saved" table
                var savedRecipe = new Document();
                savedRecipe["Email"] = userEmail;
                savedRecipe["RecipeId"] = recipeId; 

                await table.PutItemAsync(savedRecipe);

                return Json(new { success = true, message = "Recipe saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during saving the recipe.");
                // You may want to log the error or return a specific error message
                return Json(new { success = false, message = "An error occurred during saving the recipe." });
            }
        }



    }
}
