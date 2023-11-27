using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

public class CreateRecipesTable
{
    public async Task CreateTable(DynamoDBSetup dynamoDBSetup)
    {
        AmazonDynamoDBClient client = dynamoDBSetup.GetDynamoDBClient();

        string tableName = "recipes-saved";

        if (!await TableExists(client, tableName))
        {
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Email",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "RecipeId",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Email",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "RecipeId",
                        KeyType = "RANGE"
                    }
                },
                BillingMode = BillingMode.PAY_PER_REQUEST,
            };

            await client.CreateTableAsync(request);

            Console.WriteLine("Recipes-saved table created.");
        }
    }

    private async Task<bool> TableExists(AmazonDynamoDBClient client, string tableName)
    {
        try
        {
            var describeTableRequest = new DescribeTableRequest
            {
                TableName = tableName
            };

            var describeTableResponse = await client.DescribeTableAsync(describeTableRequest);

            return true;
        }
        catch (ResourceNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
}
