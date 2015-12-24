using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Windows;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace GoucherDiningAppPOC1.Data
{
    public class Entre
    {

        public Entre(string name,  double calories ,string station)
        {
            this.Name = name;
            this.Calories = calories;
            this.Station = station;
        }
        public string Name { get; private set; }
        public double Calories { get; private set; }
        public string Station { get; private set; }
        public override string ToString()
        {
            return this.Name ;
        }
    }

    public class Menu
    {
        public Menu(string mealType, DateTime date, DateTime open, DateTime close)
        {
            this.MealType = mealType;
            this.Date = date;
            this.Open = open;
            this.Close = close;
            this.Entres = new List<Entre>();
        }
        public string MealType { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime Open { get; private set; }
        public DateTime Close { get; private set; }
        public List<Entre> Entres { get; private set; }
        public override string ToString()
        {
            return this.MealType +" "+this.Date.DayOfWeek;
        }
    
    }

    public class DiningHall
    {
        public DiningHall(String name)
        {
            this.Name = name;
            this.Menus = new List<Menu>();
        }
        public string Name { get; private set; }
        public string Id { get; set; }
        public List<Menu> Menus { get; private set; }        
        public override string ToString()
        {
            return this.Name;
        }
    }

    public class DiningHallSource
    { 
        private static DiningHallSource _diningHallSource = new DiningHallSource();

        private ObservableCollection<DiningHall> _diningHalls = new ObservableCollection<DiningHall>();
        public ObservableCollection<DiningHall> DiningHalls
        {
            get { return this._diningHalls; }
        }

        public static async Task<IEnumerable<DiningHall>> GetDiningHallsAsync()
        {
            await _diningHallSource.GetDiningHallDataAsync();

            return _diningHallSource.DiningHalls;
        }

        public static async Task<DiningHall> GetDiningHallAsync(string name)
        {
            await _diningHallSource.GetDiningHallDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _diningHallSource.DiningHalls.Where((diningHall) => diningHall.Name.Equals(name));
           if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetDiningHallDataAsync()
        {
            if (this._diningHalls.Count != 0)
                return;
            Uri dataUri = new Uri("ms-appx:///DataModel/DiningData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["DiningHalls"].GetArray();

            foreach (JsonValue diningHallValue in jsonArray)
            {
                JsonObject diningHallObject = diningHallValue.GetObject();
                DiningHall diningHall = new DiningHall(diningHallObject["Name"].GetString());
                //build menus
                foreach (JsonValue menuValue in diningHallObject["Menus"].GetArray())
                {
                    JsonObject menuObject = menuValue.GetObject();
                    DateTime open = DateTime.Parse(menuObject["Open"].GetString());
                    DateTime close = DateTime.Parse(menuObject["Close"].GetString());
                    DateTime date = DateTime.Parse(menuObject["Date"].GetString());
                    string mealType =menuObject["MealType"].GetString();
                    Menu menu = new Menu(mealType, date, open,close);
                    //build entres
                    foreach (JsonValue entreValue in menuObject["Entres"].GetArray())
                    {
                        JsonObject entreObject = entreValue.GetObject();
                        Entre entre = new Entre(entreObject["Name"].GetString(),entreObject["Calories"].GetNumber(),entreObject["Station"].GetString());
                        menu.Entres.Add(entre);
                    }
                    diningHall.Menus.Add(menu);                                  
                }
                this.DiningHalls.Add(diningHall);
            }
        }
    }
    
    public  class AzureDiningHallSource
    {
       private IMobileServiceTable<DiningHall> dhTable = App.MobileService.GetTable<DiningHall>();
       private MobileServiceCollection<DiningHall, DiningHall> items;
     //   private IMobileServiceTable<Menu> mTable = App.MobileService.GetTable<Menu>();
      //  private IMobileServiceTable<Entre> eTable = App.MobileService.GetTable<Entre>();

        // Constructor
        public async void PushDHData()
        {

            ObservableCollection<DiningHall> diningHalls = (ObservableCollection<DiningHall>)await DiningHallSource.GetDiningHallsAsync();
            foreach (DiningHall dh in diningHalls) await dhTable.InsertAsync(dh);
            
        }

        public async void GetDHData()
        {
            // This code refreshes the entries in the list view be querying the TodoItems table.
            // The query excludes completed TodoItems
                    try
                    {
                        items = await dhTable.ToCollectionAsync();
                        foreach (DiningHall dh in items)await dhTable.DeleteAsync(dh);
                        var s = ":";
                    }
                    catch (MobileServiceInvalidOperationException e)
                    {
                        var s = ":";
                    }

               

             
        }

    }
}