using Newtonsoft.Json;
using System.Collections.Generic;

namespace s3866528_a3.Models
{
    public class TastyApiResponse
    {
        public int Count { get; set; }
        public List<Recipe> Results { get; set; }
    }

    public class Recipe
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("num_servings")]
        public int NumServings { get; set; }
        public List<Instruction> Instructions { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        public Nutrition Nutrition { get; set; }
        public string Description { get; set; }

        [JsonProperty("sections")]
        public List<IngredientSection> Ingredients { get; set; }

        // Constructor to initialize the lists
        public Recipe()
        {
            Instructions = new List<Instruction>();
            Ingredients = new List<IngredientSection>();
            Nutrition = new Nutrition();
        }
    }

    public class Instruction
    {
        [JsonProperty("display_text")]
        public string DisplayText { get; set; }
    }

    public class IngredientSection
    {
        [JsonProperty("components")]
        public List<IngredientComponent> Components { get; set; }
    }

    public class IngredientComponent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("raw_text")]
        public string RawText { get; set; }
    }

    public class Nutrition
    {
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Calories { get; set; }
        public int Sugar { get; set; }
        public int Carbohydrates { get; set; }
        public int Fiber { get; set; }
    }
}
