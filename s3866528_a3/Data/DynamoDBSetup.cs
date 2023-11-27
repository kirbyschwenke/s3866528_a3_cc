using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;

public class DynamoDBSetup
{
    public string AccessKey { get; private set; }
    public string SecretKey { get; private set; }
    public RegionEndpoint Region { get; private set; }

    private AmazonDynamoDBClient client;

    public DynamoDBSetup(string accessKey, string secretKey, RegionEndpoint region)
    {
        AccessKey = accessKey;
        SecretKey = secretKey;
        Region = region;

        var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = region
        };

        client = new AmazonDynamoDBClient(awsCredentials, config);
    }

    public AmazonDynamoDBClient GetDynamoDBClient()
    {
        return client;
    }
}
