using Newtonsoft.Json;
using s3866528_a3.Models;

public class RecipesApi
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public RecipesApi(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;

        _httpClient.BaseAddress = new Uri("https://tasty.p.rapidapi.com/");
        _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "tasty.p.rapidapi.com");
    }

    public async Task<List<Recipe>> GetRecipesList(int from, int size, string tags)
    {
        try
        {
            var uri = $"recipes/list?from={from}&size={size}&tags={Uri.EscapeDataString(tags)}";
            var response = await _httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Deserialize the entire response
            var tastyApiResponse = JsonConvert.DeserializeObject<TastyApiResponse>(content);

            // Return the list of recipes
            return tastyApiResponse.Results ?? new List<Recipe>();
        }
        catch (Exception ex)
        {
            // Handle exceptions or return an empty list
            Console.WriteLine($"An error occurred while fetching recipes: {ex.Message}");
            return new List<Recipe>();
        }
    }

    public async Task<List<Recipe>> GetRecipesByKeywords(string keywords)
    {
        try
        {
            // Fetch recipes from the Tasty API based on keywords
            var uri = $"recipes/list?from=0&size=10&q={Uri.EscapeDataString(keywords)}";
            var response = await _httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Print the raw JSON response to the console
            Console.WriteLine("Raw JSON response:");
            Console.WriteLine(content);

            // Deserialize the entire response
            var tastyApiResponse = JsonConvert.DeserializeObject<TastyApiResponse>(content);

            // Return the list of recipes
            return tastyApiResponse.Results ?? new List<Recipe>();
        }
        catch (Exception ex)
        {
            // Handle exceptions or return an empty list
            Console.WriteLine($"An error occurred while fetching recipes by keywords: {ex.Message}");
            return new List<Recipe>();
        }
    }

    public async Task<Recipe> GetRecipeById(int id)
    {
        try
        {
            var uri = $"recipes/get-more-info?id={id}";
            var response = await _httpClient.GetAsync(uri);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Print the raw JSON response to the console
            Console.WriteLine("Raw JSON response:");
            Console.WriteLine(content);

            // Deserialize the entire response
            var recipe = JsonConvert.DeserializeObject<Recipe>(content);

            // Return the recipe
            return recipe;
        }
        catch (Exception ex)
        {
            // Handle exceptions or return null
            Console.WriteLine($"An error occurred while fetching recipe by ID: {ex.Message}");
            return null;
        }
    }

}
