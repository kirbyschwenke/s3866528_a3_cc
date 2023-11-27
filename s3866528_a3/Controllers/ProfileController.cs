using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Mvc;
using s3866528_a3.Models;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon;
using Amazon.DynamoDBv2.DataModel;

public class ProfileController : Controller
{
    private readonly IAmazonDynamoDB _dynamoDbClient;
    private readonly ILogger<ProfileController> _logger;
    private readonly ImageUploader _imageUploader;

    public ProfileController(IAmazonDynamoDB dynamoDbClient, ILogger<ProfileController> logger, ImageUploader imageUploader)
    {
        _dynamoDbClient = dynamoDbClient;
        _logger = logger;
        _imageUploader = imageUploader;
    }

    public async Task<IActionResult> Index()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Home");
        }

        var user = await GetUserByEmail(userEmail);

        return View(user);
    }

    private async Task<Login> GetUserByEmail(string userEmail)
    {
        try
        {
            var context = new DynamoDBContext(_dynamoDbClient);
            var user = await context.LoadAsync<Login>(userEmail);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching user: {ex.Message}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(Login user)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var existingUser = await GetUserByEmail(userEmail);

        try
        {
            if (existingUser == null)
            {
                return RedirectToAction("Index", "Profile");
            }

            // Check if a new profile image is uploaded
            if (user.ProfileImage != null && user.ProfileImage.Length > 0)
            {
                // Read the file into a byte array
                using (var ms = new MemoryStream())
                {
                    user.ProfileImage.CopyTo(ms);
                    var imageBytes = ms.ToArray();

                    // Use the ImageUploader to upload the image to S3
                    string s3ObjectKey = $"{user.Email}.profile-image.jpg";
                    _imageUploader.UploadImage(imageBytes, s3ObjectKey, "a3profileimage");

                    // Update the user profile with the new image URL
                    existingUser.ProfileImageUrl = s3ObjectKey;
                }
            }

            // Update other user properties
            existingUser.Name = user.Name;
            existingUser.Preferences = user.Preferences;

            // Update the user profile in the database
            await UpdateUser(existingUser);

            return RedirectToAction("Index", "Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating user profile: {ex.Message}");
            return View("Error");
        }
    }

    private async Task UpdateUser(Login user)
    {
        try
        {
            var context = new DynamoDBContext(_dynamoDbClient);
            await context.SaveAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while updating user: {ex.Message}");
            throw;
        }
    }
}