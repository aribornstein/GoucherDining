using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace GoucherDiningAppPOC1.Data
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class MealPlanItem
    {
        public MealPlanItem(String name,String meal, Double calories)
        {
            this.Name = name;
            this.Meal = meal;
            this.Calories = calories;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            this.UID = "Item"+localSettings.Values["MaxMealPlanId"].ToString();
            localSettings.Values["MaxMealPlanId"] =(int)localSettings.Values["MaxMealPlanId"] + 1;
        }

        public string UID { get; private set; }
        public string Name { get; private set; }
        public string Meal { get; private set; }
        public Double Calories { get; private set; }
     

        public override string ToString()
        {
            return this.Name;
        }
    }



    

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class MealPlanSource
    {
        private static MealPlanSource _mealPlanDataSource = new MealPlanSource();

        private ObservableCollection<MealPlanItem> _items = new ObservableCollection<MealPlanItem>();
        public ObservableCollection<MealPlanItem> Items
        {
            get { return this._items; }

        }
        public static void  AddMealItem(string name, string meal, double calories)
        {
            _mealPlanDataSource.Items.Add(new MealPlanItem(name, meal, calories));
        }

        public static async void RemoveMealItem(string uid)
        {
            foreach (MealPlanItem e in _mealPlanDataSource.Items)
            {
                if (e.UID.Equals(uid))
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["calorieGoal"] = (double)localSettings.Values["calorieGoal"] + e.Calories;
                    _mealPlanDataSource.Items.Remove(e);
                    break;
                }
            }
            await _mealPlanDataSource.SetDataAsync();
        }
        public static async Task<IEnumerable<MealPlanItem>> GetItemsAsync()
        {
            await _mealPlanDataSource.GetDataAsync();

            return _mealPlanDataSource.Items;   
        }
        public static IEnumerable<MealPlanItem> getCurrentItems()
        {
            return _mealPlanDataSource.Items; 
        }
        public static async void ClearItems()
        {
            foreach (MealPlanItem e in _mealPlanDataSource.Items)
            {
                _mealPlanDataSource.Items.Remove(e);
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["calorieGoal"] = (double)localSettings.Values["calorieGoal"] + e.Calories;
            }
            await _mealPlanDataSource.SetDataAsync();
        }
        public static async void SetItemsAsync()
        {
            await _mealPlanDataSource.SetDataAsync();

        }

        private async Task GetDataAsync()
        {
            if (this._items.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/MealPlanData.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Items"].GetArray();
            foreach (JsonValue itemValue in jsonArray)
            {
                    JsonObject itemObject = itemValue.GetObject();
                    this.Items.Add(new MealPlanItem(itemObject["Name"].GetString(), itemObject["Meal"].GetString(), itemObject["Calories"].GetNumber()));
            }               
        }

        private async Task SetDataAsync()
        {
           // if (this._items.Count != 0)
           //     return;

            Uri dataUri = new Uri("ms-appx:///DataModel/MealPlanData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

            String json ="{\"Items\":"+ JsonConvert.SerializeObject(Items.ToArray())+"}";

            await FileIO.WriteTextAsync(file, json,Windows.Storage.Streams.UnicodeEncoding.Utf8);
           
        }

    }
}