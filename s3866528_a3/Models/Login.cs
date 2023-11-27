using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace s3866528_a3.Models
{
    [DynamoDBTable("login")]
    public class Login
    {
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [DynamoDBHashKey]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        [DynamoDBProperty]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [DynamoDBProperty]
        public string Name { get; set; }
        [DynamoDBProperty]
        public string ProfileImageUrl { get; set; }
        [DynamoDBProperty("Preferences")]
        public List<string> Preferences { get; set; }

        [DynamoDBIgnore]
        [Display(Name = "Profile Image")] 
        public IFormFile ProfileImage { get; set; }
    }
}
