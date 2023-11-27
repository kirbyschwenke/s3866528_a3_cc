using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.DynamoDBv2;

public class ImageUploader : IDisposable
{
    private readonly DynamoDBSetup _dynamoDbClient;
    private readonly AmazonS3Client _s3Client;

    public ImageUploader(DynamoDBSetup dynamoDbClient)
    {
        _dynamoDbClient = dynamoDbClient;
        _s3Client = new AmazonS3Client(_dynamoDbClient.AccessKey, _dynamoDbClient.SecretKey, _dynamoDbClient.Region);
    }

    public bool UploadImage(byte[] imageBytes, string objectKey, string bucketName)
    {
        try
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                // Optionally, you can handle the case of no image here.
                Console.WriteLine("No image to upload.");
                return false;
            }

            using (TransferUtility transferUtility = new TransferUtility(_s3Client))
            {
                transferUtility.Upload(new MemoryStream(imageBytes), bucketName, objectKey);
            }

            return true;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"Error uploading file to S3: {e.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}