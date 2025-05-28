using SQLite;
using System;

namespace MobSysFinalsBase1.Models
{
    public enum MeasurementUnit
    {
        Metric, // grams, liters, celsius, etc.
        US      // ounces, cups, fahrenheit, etc.
    }

    public class Recipe
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100), NotNull]
        public string Name { get; set; }
        public int PrepTimeMinutes { get; set; }
        public MeasurementUnit Unit { get; set; }
        public string IngredientsJson { get; set; }
        public string Steps { get; set; }
        public string ImageBase64 { get; set; }
    }
}