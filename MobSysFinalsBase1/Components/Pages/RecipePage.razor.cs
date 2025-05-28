using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class RecipePage : ComponentBase
    {
        public enum ViewMode { Grid, List }
        public ViewMode CurrentView { get; set; } = ViewMode.Grid;

        [Inject] public MealService MealService { get; set; }

        public List<MealRecipe> Recipes { get; set; } = new();
        public bool IsLoading { get; set; } = true;
        public MealRecipe SelectedRecipe { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            Recipes = await MealService.GetRandomRecipesAsync(6);
            IsLoading = false;
        }

        public void Show(MealRecipe recipe) => SelectedRecipe = recipe;

        public void SetView(ViewMode mode) => CurrentView = mode;
    }
}