using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace s3866528_a3.Services
{
    public interface IRecipeService
    {
        List<int> GetSavedRecipeIds(string userEmail);
    }

    public class RecipeService : IRecipeService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DashboardController> _logger;


        public RecipeService(IAmazonDynamoDB dynamoDbClient, ILogger<DashboardController> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
        }

        public List<int> GetSavedRecipeIds(string userEmail)
        {
                try
                {
                    var table = Table.LoadTable(_dynamoDbClient, "recipes-saved");

                    // Query DynamoDB to get the RecipeIds based on the user's email
                    var search = table.Query(new QueryOperationConfig
                    {
                        Filter = new QueryFilter("Email", QueryOperator.Equal, userEmail)
                    });

                    // Extract the RecipeIds from the DynamoDB document
                    var document = search.GetNextSetAsync().Result;
                    var recipeIds = document.Select(item => item["RecipeId"].AsInt()).ToList();

                    return recipeIds;
                }
                catch (Exception ex)
                {
                    // Handle exceptions, log, and return an empty list or throw an exception as needed
                    _logger.LogError($"An error occurred while getting saved recipe IDs: {ex.Message}");
                    return new List<int>();
                }
        }
    }


}
