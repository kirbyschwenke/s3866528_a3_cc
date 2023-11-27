using Microsoft.AspNetCore.Mvc;
using s3866528_a3.Models;
using s3866528_a3.Services;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly RecipesApi _recipesApi;
    private readonly IRecipeService _recipeService;

    public DashboardController(ILogger<DashboardController> logger, RecipesApi recipesApi, IRecipeService recipeService)
    {
        _logger = logger;
        _recipesApi = recipesApi;
        _recipeService = recipeService;
    }

    public async Task<IActionResult> Index()
    {
        // Display the user's saved recipes
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
        {
            // Handle the case where the user email is not available
            return RedirectToAction("Login", "Home"); // Redirect to the login page
        }

        var savedRecipesTasks = _recipeService.GetSavedRecipeIds(userEmail).Select(id => GetRecipeById(id));
        var savedRecipes = await Task.WhenAll(savedRecipesTasks);

        var viewModel = new DashboardViewModel
        {
            SavedRecipes = savedRecipes.ToList()
        };

        return View(viewModel);


    }


    private async Task<Recipe> GetRecipeById(int recipeId)
    {
        try
        {
            // Fetch recipe from the RecipesApi by ID
            var recipe = await _recipesApi.GetRecipeById(recipeId);

            return recipe;
        }
        catch (Exception ex)
        {
            // Handle exceptions, log, and return null or throw an exception as needed
            _logger.LogError($"An error occurred while getting recipe by ID: {ex.Message}");
            return null;
        }
    }

}
