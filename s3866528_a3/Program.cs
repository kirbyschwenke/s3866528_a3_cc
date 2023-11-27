using Amazon;
using Amazon.DynamoDBv2;
using s3866528_a3.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");

// Get the API key from configuration
var apiKey = builder.Configuration["RecipesApi:ApiKey"];

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

// Create an instance of DynamoDBSetup to set up the DynamoDB client
var dynamoDBSetup = new DynamoDBSetup("AKIAWWS5HP5KECNBOOP5", "cQTZRE9qxhkFTuzvtF1f9lhlXCOjIGnR55i7CF7w", RegionEndpoint.USEast1);

var dynamoDbClient = dynamoDBSetup.GetDynamoDBClient();
builder.Services.AddSingleton<IAmazonDynamoDB>(dynamoDbClient);
builder.Services.AddSingleton(provider => dynamoDBSetup);
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddSingleton<ImageUploader>(provider => new ImageUploader(provider.GetRequiredService<DynamoDBSetup>()));

var createLoginTable = new CreateLoginTable();
await createLoginTable.CreateTable(dynamoDBSetup);

var createRecipeTable = new CreateRecipesTable();
await createRecipeTable.CreateTable(dynamoDBSetup);

builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<RecipesApi>(client =>
{
    client.BaseAddress = new Uri("https://tasty.p.rapidapi.com/");
});

builder.Services.AddSingleton(provider => new RecipesApi(
    provider.GetRequiredService<IHttpClientFactory>().CreateClient(),
    apiKey
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
