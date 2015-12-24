using GoucherDiningTelerikPOC.Common;
using GoucherDiningTelerikPOC.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections;
using System.Globalization;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace GoucherDiningTelerikPOC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MenuPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //this gets the name of the menu for loading purposes
            string diningHall = e.NavigationParameter as string;
            Header.Text = diningHall +" "+ Header.Text;
            //load dining hall information 
            ObservableCollection<DiningHall> diningHalls = (ObservableCollection<DiningHall>)await DiningHallSource.GetDiningHallsAsync();
            //get correct dining hall
            var diningInds = new Dictionary<string, int>() {{"Stimson", 0 },{"Heubeck", 1 },{"P-Stone", 2 },{"Kosher", 3 },{"The Van", 4 },{"Alice's", 5 } };
            //get correct meal index based on current time
            List<Menu> menus = diningHalls[diningInds[diningHall]].Menus;
            int menuIndex = 0;
            while (menuIndex < menus.Count())
            {
                //by date commented out renable later
                //if (DateTime.Now.Date.Equals(menus[menuIndex].Date) && (DateTime.Now.TimeOfDay > menus[menuIndex].Open) && (DateTime.Now.TimeOfDay < menus[menuIndex].Close)) break;
                if ((DateTime.Now.TimeOfDay > menus[menuIndex].Open) && (DateTime.Now.TimeOfDay < menus[menuIndex].Close)) break;
                menuIndex++;
            }
            //set meal 
            var mealInds = new Dictionary<string, int>() { { "Breakfast", 0 }, { "Lunch", 1 }, { "Dinner", 2 }, { "", 3 } };
            if (diningHalls[diningInds[diningHall]].Menus.Count > menuIndex)
            {
                MealSelector.SelectedIndex = mealInds[diningHalls[diningInds[diningHall]].Menus[menuIndex].MealType];
                
                //get entres list
                List<Entre> entres = diningHalls[diningInds[diningHall]].Menus[menuIndex].Entres;
                //build entres grouped by station clean up this code
                IList data = entres.ToGroups(x => x.Name, x => x.Station);
                CollectionViewSource entreStations = new CollectionViewSource();
                entreStations.Source = data;
                entreStations.IsSourceGrouped = true;
                Menu.ItemsSource = entreStations.View;
                Stations.ItemsSource = entreStations.View.CollectionGroups;
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void MultiSelect_Click(object sender, RoutedEventArgs e)
        {
            if (Menu.SelectionMode == ListViewSelectionMode.Single)
            {
                Menu.SelectionMode = ListViewSelectionMode.Multiple;
                addToMealPlan.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                Menu.SelectionMode = ListViewSelectionMode.Single;
                addToMealPlan.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void MealSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this gets the name of the menu for loading purposes
            
           if (Header!=null)
           {
               string diningHall = Header.Text.Replace(" Menu","");
               //load dining hall information 
               ObservableCollection<DiningHall> diningHalls = (ObservableCollection<DiningHall>)await DiningHallSource.GetDiningHallsAsync();
               //get correct dining hall
               var diningInds = new Dictionary<string, int>() { { "Stimson", 0 }, { "Heubeck", 1 }, { "P-Stone", 2 }, { "Kosher", 3 }, { "The Van", 4 }, { "Alice's", 5 } };
               //get correct meal index based on current time
               var mealInds = new Dictionary<string, int>() { { "Breakfast", 0 }, { "Lunch", 1 }, { "Dinner", 2 }, { "", 3 } };
               List<Menu> menus = diningHalls[diningInds[diningHall]].Menus;
               int menuIndex = 0;
               while (menuIndex < menus.Count())
               {
                   //by date disabled renable when data is complete
                  // if (DateTime.Now.Date.Equals(menus[menuIndex].Date) && MealSelector.SelectedIndex== mealInds[menus[menuIndex].MealType]) break;
                   if (MealSelector.SelectedIndex== mealInds[menus[menuIndex].MealType]) break;
                   menuIndex++;
               }
               //set meal 
               if (diningHalls[diningInds[diningHall]].Menus.Count > menuIndex)
               {
                   //get entres list
                   List<Entre> entres = diningHalls[diningInds[diningHall]].Menus[menuIndex].Entres;
                   //build entres grouped by station clean up this code
                   IList data = entres.ToGroups(x => x.Name, x => x.Station);
                   CollectionViewSource entreStations = new CollectionViewSource();
                   entreStations.Source = data;
                   entreStations.IsSourceGrouped = true;
                   Menu.ItemsSource = entreStations.View;
                   Stations.ItemsSource = entreStations.View.CollectionGroups;
               }
           }
        }

        private async void AddToMealPlan_Click(object sender, RoutedEventArgs e)
        {
            if (Menu.SelectionMode == ListViewSelectionMode.Multiple)
            {
                string diningHall = Header.Text.Replace(" Menu", "");
                //MealTypes
                var MealTypes = new List<String> { "Breakfast", "Lunch", "Dinner", "Other" };
                // MealPlanSource 
                await MealPlanSource.GetItemsAsync();

                //add selected 
                foreach (Entre item in Menu.SelectedItems)
                {
                    MealPlanSource.AddMealItem(diningHall+" "+item.Name, MealTypes[MealSelector.SelectedIndex], item.Calories);
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    if (localSettings.Values["calorieGoal"] == null) localSettings.Values["calorieGoal"] = 0.0 - item.Calories;
                    else localSettings.Values["calorieGoal"] = (double)localSettings.Values["calorieGoal"] - item.Calories;
                }
                MealPlanSource.SetItemsAsync();

                Menu.SelectionMode = ListViewSelectionMode.Single;
                addToMealPlan.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
        }
    }
}
