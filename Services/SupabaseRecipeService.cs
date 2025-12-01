using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RecipeApp.Models;

namespace RecipeApp.Services
{
    public class SupabaseRecipeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SupabaseRecipeService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            
            var supabaseUrl = configuration["Supabase:Url"]!;
            var supabaseKey = configuration["Supabase:Key"]!;
            
            _baseUrl = $"{supabaseUrl}/rest/v1";
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
            _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
        }

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/recipes?select=*&order=created_at.desc");
                response.EnsureSuccessStatusCode();
                
                var dbRecipes = await response.Content.ReadFromJsonAsync<List<SupabaseRecipe>>();
                return dbRecipes?.Select(r => r.ToRecipe()).ToList() ?? new List<Recipe>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recipes: {ex.Message}");
                return new List<Recipe>();
            }
        }

        public async Task SaveRecipeAsync(Recipe recipe)
        {
            var dbRecipe = SupabaseRecipe.FromRecipe(recipe);
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/recipes", dbRecipe);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            var dbRecipe = SupabaseRecipe.FromRecipe(recipe);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_baseUrl}/recipes?id=eq.{recipe.Id}")
            {
                Content = JsonContent.Create(dbRecipe)
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteRecipeAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/recipes?id=eq.{id}");
            response.EnsureSuccessStatusCode();
        }

        // Supabase用の内部クラス
        private class SupabaseRecipe
        {
            public Guid id { get; set; }
            public string title { get; set; } = string.Empty;
            public string? image_base64 { get; set; }
            public JsonElement ingredients { get; set; }
            public JsonElement steps { get; set; }
            public string? calories { get; set; }
            public string? cooking_time { get; set; }
            public string? servings { get; set; }
            public string? notes { get; set; }
            public DateTime created_at { get; set; }

            public Recipe ToRecipe()
            {
                return new Recipe
                {
                    Id = id,
                    Title = title,
                    ImageBase64 = image_base64,
                    Ingredients = DeserializeList(ingredients),
                    Steps = DeserializeList(steps),
                    Calories = calories,
                    CookingTime = cooking_time,
                    Servings = servings,
                    Notes = notes,
                    CreatedAt = created_at
                };
            }

            private static List<string> DeserializeList(JsonElement element)
            {
                try
                {
                    return JsonSerializer.Deserialize<List<string>>(element.GetRawText()) ?? new List<string>();
                }
                catch
                {
                    return new List<string>();
                }
            }

            public static object FromRecipe(Recipe recipe)
            {
                return new
                {
                    id = recipe.Id,
                    title = recipe.Title,
                    image_base64 = recipe.ImageBase64,
                    ingredients = recipe.Ingredients,
                    steps = recipe.Steps,
                    calories = recipe.Calories,
                    cooking_time = recipe.CookingTime,
                    servings = recipe.Servings,
                    notes = recipe.Notes,
                    created_at = recipe.CreatedAt
                };
            }
        }
    }
}