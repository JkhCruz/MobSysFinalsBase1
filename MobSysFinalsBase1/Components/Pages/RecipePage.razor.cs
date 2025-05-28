using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class RecipePage : ComponentBase
    {
        public enum ViewMode { Grid, List }
        public ViewMode CurrentView { get; set; } = ViewMode.Grid;

        /// <summary>
        /// Injects NavigationManager from MauiProgram.cs
        /// </summary>
        [Inject]
        public NavigationManager Nav { get; set; }

        /// <summary>
        /// Injects HttpClient from MauiProgram.cs
        /// </summary>
        [Inject]
        public HttpClient Http { get; set; }

        public class Recipe
        {
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }
        }

        public List<Recipe> Recipes { get; set; } = new();
        public bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadRecipes();
        }

        public async Task LoadRecipes()
        {
            IsLoading = true;
            Recipes = new List<Recipe>();
            var uniqueTitles = new HashSet<string>();
            int targetCount = 5;
            int attempt = 0;

            var randomRecipeTasks = new List<Task<Recipe>>();
            while (randomRecipeTasks.Count < targetCount && attempt < 10)
            {
                randomRecipeTasks.Add(GetRandomRecipeAsync());
                attempt++;
            }

            var fetched = await Task.WhenAll(randomRecipeTasks);
            foreach (var recipe in fetched)
            {
                if (recipe != null && uniqueTitles.Add(recipe.Title))
                {
                    Recipes.Add(recipe);
                }
                if (Recipes.Count >= targetCount) break;
            }
            IsLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task<Recipe> GetRandomRecipeAsync()
        {
            try
            {
                var response = await Http.GetFromJsonAsync<TheMealDbRandomResponse>("https://www.themealdb.com/api/json/v1/1/random.php");
                var meal = response?.Meals?[0];
                if (meal == null) return null;
                return new Recipe
                {
                    Title = meal.strMeal,
                    ImageUrl = meal.strMealThumb,
                    Author = meal.strArea ?? "Unknown",
                    Description = !string.IsNullOrWhiteSpace(meal.strInstructions) && meal.strInstructions.Length > 80
                        ? meal.strInstructions.Substring(0, 77) + "..."
                        : meal.strInstructions ?? ""
                };
            }
            catch
            {
                return null;
            }
        }

        public void SetView(ViewMode mode) => CurrentView = mode;

        public void ViewRecipe(Recipe recipe)
        {
            if (!string.IsNullOrEmpty(recipe?.Title))
                Nav.NavigateTo($"/recipe/{System.Uri.EscapeDataString(recipe.Title)}");
        }

        // DTO for TheMealDB
        public class TheMealDbRandomResponse
        {
            [JsonPropertyName("meals")]
            public List<TheMealDbMeal> Meals { get; set; }
        }
        public class TheMealDbMeal
        {
            public string strMeal { get; set; }
            public string strMealThumb { get; set; }
            public string strArea { get; set; }
            public string strInstructions { get; set; }
        }
    }
}