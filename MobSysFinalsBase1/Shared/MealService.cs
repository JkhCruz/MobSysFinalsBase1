using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MobSysFinalsBase1.Shared
{
    public class MealService
    {
        private readonly HttpClient _http;

        public MealService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MealRecipe>> GetRandomRecipesAsync(int count)
        {
            var recipes = new List<MealRecipe>();
            var titles = new HashSet<string>();
            int attempts = 0;

            var tasks = new List<Task<MealRecipe>>();
            while (tasks.Count < count && attempts < count * 3)
            {
                tasks.Add(GetRandomRecipeAsync());
                attempts++;
            }
            var fetched = await Task.WhenAll(tasks);
            foreach (var recipe in fetched)
            {
                if (recipe != null && titles.Add(recipe.Title))
                    recipes.Add(recipe);
                if (recipes.Count >= count)
                    break;
            }
            return recipes;
        }

        public async Task<MealRecipe> GetRandomRecipeAsync()
        {
            try
            {
                var resp = await _http.GetFromJsonAsync<MealDbResponse>("https://www.themealdb.com/api/json/v1/1/random.php");
                var meal = resp?.Meals?.FirstOrDefault();
                if (meal == null) return null;

                var ingredients = new List<MealIngredient>();
                for (int i = 1; i <= 20; i++)
                {
                    var name = meal.GetType().GetProperty($"strIngredient{i}")?.GetValue(meal)?.ToString();
                    var measure = meal.GetType().GetProperty($"strMeasure{i}")?.GetValue(meal)?.ToString();

                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(measure))
                    {
                        var (us, metric) = ConvertMeasurement(measure.Trim());
                        ingredients.Add(new MealIngredient { Name = name.Trim(), MeasureUS = us, MeasureMetric = metric });
                    }
                }

                var steps = (meal.strInstructions ?? "")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Where(s => s.Length > 0).ToList();

                var prep = new Random(Guid.NewGuid().GetHashCode()).Next(10, 41);

                return new MealRecipe
                {
                    Title = meal.strMeal,
                    ImageUrl = meal.strMealThumb,
                    Author = meal.strArea ?? "Unknown",
                    Description = steps.FirstOrDefault() ?? "",
                    PrepTime = prep,
                    Ingredients = ingredients,
                    Instructions = steps
                };
            }
            catch { return null; }
        }

        private (string us, string metric) ConvertMeasurement(string measure)
        {
            if (string.IsNullOrWhiteSpace(measure)) return ("", "");
            var m = measure.ToLowerInvariant().Trim();

            // Try to extract quantity and unit using regex
            var match = Regex.Match(m, @"^([\d\.,/]+)\s*([a-zA-Z]+)?");
            if (match.Success)
            {
                var qtyRaw = match.Groups[1].Value.Replace(",", ".");
                double qty = 1;
                double.TryParse(qtyRaw.Contains("/") ?
                    (qtyRaw.Contains(".") ? qtyRaw.Substring(0, qtyRaw.IndexOf(".")) : qtyRaw) : qtyRaw, out qty);
                var unit = match.Groups[2].Value;

                // US to Metric
                if (unit.Contains("cup"))
                    return ($"{qty} cup", $"{Math.Round(qty * 240)} ml");
                if (unit.Contains("tbsp") || unit.Contains("tbls") || unit.Contains("tablespoon"))
                    return ($"{qty} tbsp", $"{Math.Round(qty * 15)} ml");
                if (unit.Contains("tsp") || unit.Contains("teaspoon"))
                    return ($"{qty} tsp", $"{Math.Round(qty * 5)} ml");
                if (unit.Contains("oz"))
                    return ($"{qty} oz", $"{Math.Round(qty * 28.35)} g");
                if (unit.Contains("lb"))
                    return ($"{qty} lb", $"{Math.Round(qty * 453.6)} g");

                // Metric to US
                if ((unit == "g" || unit == "gram" || unit == "grams") && !m.Contains("egg"))
                    return ($"{Math.Round(qty / 28.35, 2)} oz", $"{qty} g");
                if (unit == "kg" || unit == "kilogram" || unit == "kilograms")
                    return ($"{Math.Round(qty * 2.20462, 2)} lb", $"{qty} kg");
                if (unit == "ml" || unit == "milliliter" || unit == "milliliters")
                    return ($"{Math.Round(qty / 240, 2)} cup", $"{qty} ml");
                if (unit == "l" || unit == "liter" || unit == "liters")
                    return ($"{Math.Round(qty * 4.22675, 2)} cup", $"{qty} l");
            }

            // For ambiguous or non-numeric measures (e.g., Dash, To Glaze)
            return (measure, measure);
        }
    }

    public class MealRecipe
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int PrepTime { get; set; }
        public List<MealIngredient> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
    }

    public class MealIngredient
    {
        public string Name { get; set; }
        public string MeasureUS { get; set; }
        public string MeasureMetric { get; set; }
    }

    public class MealDbResponse
    {
        public List<Meal> Meals { get; set; }
    }
    public class Meal
    {
        public string strMeal { get; set; }
        public string strMealThumb { get; set; }
        public string strArea { get; set; }
        public string strInstructions { get; set; }
        public string strIngredient1 { get; set; }
        public string strIngredient2 { get; set; }
        public string strIngredient3 { get; set; }
        public string strIngredient4 { get; set; }
        public string strIngredient5 { get; set; }
        public string strIngredient6 { get; set; }
        public string strIngredient7 { get; set; }
        public string strIngredient8 { get; set; }
        public string strIngredient9 { get; set; }
        public string strIngredient10 { get; set; }
        public string strIngredient11 { get; set; }
        public string strIngredient12 { get; set; }
        public string strIngredient13 { get; set; }
        public string strIngredient14 { get; set; }
        public string strIngredient15 { get; set; }
        public string strIngredient16 { get; set; }
        public string strIngredient17 { get; set; }
        public string strIngredient18 { get; set; }
        public string strIngredient19 { get; set; }
        public string strIngredient20 { get; set; }
        public string strMeasure1 { get; set; }
        public string strMeasure2 { get; set; }
        public string strMeasure3 { get; set; }
        public string strMeasure4 { get; set; }
        public string strMeasure5 { get; set; }
        public string strMeasure6 { get; set; }
        public string strMeasure7 { get; set; }
        public string strMeasure8 { get; set; }
        public string strMeasure9 { get; set; }
        public string strMeasure10 { get; set; }
        public string strMeasure11 { get; set; }
        public string strMeasure12 { get; set; }
        public string strMeasure13 { get; set; }
        public string strMeasure14 { get; set; }
        public string strMeasure15 { get; set; }
        public string strMeasure16 { get; set; }
        public string strMeasure17 { get; set; }
        public string strMeasure18 { get; set; }
        public string strMeasure19 { get; set; }
        public string strMeasure20 { get; set; }
    }
}