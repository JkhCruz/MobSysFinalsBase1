using MobSysFinalsBase1.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Shared
{
    /// <summary>
    /// Centralized Class for handling local SQLite Database things for the App
    /// Loaded in MauiProgram as Singleton (one instance only within the App)
    /// </summary>
    public class DatabaseContext
    {
        SQLiteAsyncConnection database;
        public static DatabaseContext Instance { set; get; }
        public DatabaseContext()
        {
            //init from constructor
            DatabaseContext.Instance = this;
        }

        /// <summary>
        /// Initialize Database Availability
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            //Create tables
            await database.CreateTableAsync<User>();
            await database.CreateTableAsync<Recipe>();
        }

        // User methods
        public async Task<List<User>> Users()
        {
            await Init();
            return await database.Table<User>().ToListAsync();
        }

        public async Task<int> SaveUser(User incoming)
        {
            await Init();
            if (incoming.ID != 0)
                return await database.UpdateAsync(incoming);//update existing
            else
                return await database.InsertAsync(incoming);//insert new
        }

        public async Task<int> DeleteUser(User incoming)
        {
            await Init();
            return await database.DeleteAsync(incoming);
        }

        // Recipe methods
        public async Task<List<Recipe>> Recipes()
        {
            await Init();
            return await database.Table<Recipe>().ToListAsync();
        }

        public async Task<int> SaveRecipe(Recipe incoming)
        {
            await Init();
            if (incoming.Id != 0)
                return await database.UpdateAsync(incoming);
            else
                return await database.InsertAsync(incoming);
        }

        public async Task<int> DeleteRecipe(int id)
        {
            await Init();
            // For consistency, always delete by Id
            var recipe = await database.Table<Recipe>().Where(r => r.Id == id).FirstOrDefaultAsync();
            if (recipe != null)
                return await database.DeleteAsync(recipe);
            return 0;
        }

        public async Task<Recipe> GetRecipeById(int id)
        {
            await Init();
            return await database.Table<Recipe>().Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateRecipe(Recipe incoming)
        {
            await Init();
            // Update by Id
            return await database.UpdateAsync(incoming);
        }
    }
}