using Blazored.LocalStorage;
using RecipeApp.Models;

namespace RecipeApp.Services
{
    public class RecipeService
    {
        private readonly ILocalStorageService _localStorage;
        private const string StorageKey = "recipes";

        public RecipeService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var recipes = await _localStorage.GetItemAsync<List<Recipe>>(StorageKey);
            return recipes ?? new List<Recipe>();
        }

        public async Task SaveRecipeAsync(Recipe recipe)
        {
            var recipes = await GetAllRecipesAsync();
            recipes.Add(recipe);
            await _localStorage.SetItemAsync(StorageKey, recipes);
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            var recipes = await GetAllRecipesAsync();
            var index = recipes.FindIndex(r => r.Id == recipe.Id);
            if (index != -1)
            {
                recipes[index] = recipe;
                await _localStorage.SetItemAsync(StorageKey, recipes);
            }
        }

        public async Task DeleteRecipeAsync(Guid id)
        {
            var recipes = await GetAllRecipesAsync();
            recipes.RemoveAll(r => r.Id == id);
            await _localStorage.SetItemAsync(StorageKey, recipes);
        }
    }
}