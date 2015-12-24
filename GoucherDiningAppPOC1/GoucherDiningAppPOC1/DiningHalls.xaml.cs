using GoucherDiningAppPOC1.Common;
using GoucherDiningAppPOC1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace GoucherDiningAppPOC1
{
    public class SeriesDataPoint
    {
        public string Category { get; set; }

        public double Value { get; set; }
    }
    public sealed partial class DiningHalls : Page
    {

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private double[] activityLevels = { 1.2, 1.375, 1.55, 1.725, 1.9 };
        public DiningHalls()
        {
            this.InitializeComponent();
            this.loadDiningHallStates();
            this.loadSettings();
            this.buildCharts();
            //activate refresh timer every 30 seconds
            DispatcherTimer refreshDiningHalls = new DispatcherTimer();
            refreshDiningHalls.Interval = TimeSpan.FromSeconds(30);
            refreshDiningHalls.Tick += new EventHandler<object>(upDate);
            refreshDiningHalls.Start();
        }

        
        void upDate(object sender,object e)
        {
            this.loadDiningHallStates();
            this.buildCharts();
        }
        void buildCharts()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            List<SeriesDataPoint> goalSeries = new List<SeriesDataPoint>();
            List<SeriesDataPoint> progressSeries = new List<SeriesDataPoint>();
            for (int i = 0; i < 4; i++)
            { 
               //category Equals Date 
                string date = DateTime.Now.AddDays(i).ToString("MM-dd-yyyy");
                if (localSettings.Values["calorieGoal"] != null)
                {
                    //populate series one based on goal
                    goalSeries.Add(new SeriesDataPoint() { Category = date, Value = (double)localSettings.Values["calorieGoal"] });
                    //populate series two based on progress
                    progressSeries.Add(new SeriesDataPoint() { Category = date, Value = 2049 - (100 * i / 3) });
                }
            }
            this.chart.Series[0].ItemsSource = goalSeries;
            this.chart.Series[1].ItemsSource = progressSeries;
        }
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        private void loadSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //age
            if (localSettings.Values["age"] == null) localSettings.Values["age"] = "";
            age.Text=(string)localSettings.Values["age"];
            //cWeight
            if (localSettings.Values["cWeight"] == null) localSettings.Values["cWeight"] = "";
            cWeight.Text = (string)localSettings.Values["cWeight"];
            //feet
            if (localSettings.Values["feet"] == null) localSettings.Values["feet"] = "";
            feet.Text = (string)localSettings.Values["feet"];
            //inches
            if (localSettings.Values["inches"] == null) localSettings.Values["inches"] = "";
            inches.Text = (string)localSettings.Values["inches"];
            //activityLevel
            if (localSettings.Values["activityLevel"] == null) localSettings.Values["activityLevel"] = 0;
            activityLevel.SelectedIndex=(int)localSettings.Values["activityLevel"];
            //gender
            if (localSettings.Values["gender"] == null) localSettings.Values["gender"] = false;
            gender.IsOn = (Boolean)localSettings.Values["gender"];            
            //gDate
            if (localSettings.Values["gDate"] == null) localSettings.Values["gDate"] = DateTime.Now.Date.ToString();
            gDate.Date = DateTime.Parse((string)localSettings.Values["gDate"]);
            //gWeight
            if (localSettings.Values["gWeight"] == null) localSettings.Values["gWeight"]="";
            gWeight.Text = (string)localSettings.Values["gWeight"];
            //calorieOutput
            if (localSettings.Values["calorieOutput"] == null) localSettings.Values["calorieOutput"] = "Calorie Goal to be calculated.";
            calorieOutput.Text = (string)localSettings.Values["calorieOutput"];
            //calorieGoal 
            if (localSettings.Values["calorieGoal"] == null ) mpGoal.Text = "No Goal Set";
            else  mpGoal.Text = "Goal: " + localSettings.Values["calorieGoal"] + " Calories Left.";
            //daily reset
            if (localSettings.Values["lastClear"] == null) localSettings.Values["lastClear"]=DateTime.Now.ToString();
            else
            {
                if (DateTime.Now > DateTime.Parse((string)localSettings.Values["lastClear"]).AddDays(1))
                {
                    MealPlanSource.ClearItems();
                    localSettings.Values["lastClear"] = DateTime.Now.ToString();
                    
                }
            }

        }
        private async void loadDiningHallStates()
        { 
             //Stimson
            if (await isDiningHallOpen("Stimson")) Stimson.Opacity = 1.0;
            else Stimson.Opacity = 0.30;
            //Heubeck
            if (await isDiningHallOpen("Heubeck")) Heubeck.Opacity = 1.0;
            else Heubeck.Opacity = .30;
            //P-Stone
            if (await isDiningHallOpen("P-Stone")) Pstone.Opacity = 1.0;
            else Pstone.Opacity = .30;
            //Kosher
            if (await isDiningHallOpen("Kosher")) Kosher.Opacity = 1.0;
            else Kosher.Opacity = .30;
            //The Van 
            if (await isDiningHallOpen("The Van")) Van.Opacity = 1.0;
            else Van.Opacity = 0.30;
            //Alice's
            if (await isDiningHallOpen("Alice's")) Alices.Opacity = 1.0;
            else Alices.Opacity = 0.30;
        }
        private async Task<Boolean>  isDiningHallOpen(string diningHall )
        {
            Boolean result = false;
            // change brightness based on open settings
            ObservableCollection<DiningHall> diningHalls = (ObservableCollection<DiningHall>)await DiningHallSource.GetDiningHallsAsync();
            //get correct dining hall
            var diningInds = new Dictionary<string, int>() { { "Stimson", 0 }, { "Heubeck", 1 }, { "P-Stone", 2 }, { "Kosher", 3 }, { "The Van", 4 }, { "Alice's", 5 } };
            //get correct meal index based on current time
            List<Menu> menus = diningHalls[diningInds[diningHall]].Menus;
            int menuIndex = 0;
            while (menuIndex < menus.Count())
            {
                //by date disabled enable when menu pull finalized
                //if (DateTime.Now.Date.Equals(menus[menuIndex].Date) && (DateTime.Now.TimeOfDay > menus[menuIndex].Open) && (DateTime.Now.TimeOfDay < menus[menuIndex].Close)) result = true;
                if ((DateTime.Now.TimeOfDay > menus[menuIndex].Open.TimeOfDay) && (DateTime.Now.TimeOfDay < menus[menuIndex].Close.TimeOfDay)) result = true;
                menuIndex++;
            }            
            var open =menus[1].Close.ToString("hh:mm");
            //compare time to hours for given date if true return true
            return result;

        }

        private void loadMenu(object sender, RoutedEventArgs e)
        { 
           var button = sender as Button;
            if (button != null)
            {
                //go to a menu page
                TextBlock name = (TextBlock)button.Content;
                this.Frame.Navigate(typeof(MenuPage), name.Text.ToString());
            }
        }

        private void CalculateCalorieIntake(object sender, RoutedEventArgs e)
        {
            if (age.Text == "" || !Regex.IsMatch(age.Text, @"^\d+$")) return;
            else if (feet.Text == "" || !Regex.IsMatch(feet.Text, @"^\d+$")) return;// to be fixed
            else if (cWeight.Text == "" || !Regex.IsMatch(cWeight.Text, @"^\d.+$")) return;
            else if (gWeight.Text == "" || !Regex.IsMatch(gWeight.Text, @"^\d.+$")) return;
            else if (gDate.Date == DateTime.Now.Date) return;
            else
            {
                //get inches
                double numInches = 0.0;
                if (!(inches.Text == "") || Regex.IsMatch(inches.Text, @"^\d.+$")) numInches = Convert.ToDouble(inches.Text);
                TimeSpan span = gDate.Date - DateTime.Now;
                if (span.Days > 0)
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["age"] = age.Text;
                    localSettings.Values["feet"] = feet.Text;
                    localSettings.Values["inches"] = inches.Text;
                    localSettings.Values["activityLevel"] = activityLevel.SelectedIndex;
                    localSettings.Values["gender"] = gender.IsOn;
                    localSettings.Values["cWeight"] = cWeight.Text;
                    localSettings.Values["gWeight"] = gWeight.Text;
                    localSettings.Values["gDate"] = gDate.Date.ToString();
                    string newMessage = CalorieIntakeMessage(Convert.ToInt32(age.Text), Convert.ToInt32(feet.Text), numInches, activityLevel.SelectedIndex, gender.IsOn, Convert.ToDouble(cWeight.Text), Convert.ToDouble(gWeight.Text), span.Days);
                    localSettings.Values["calorieOutput"] =calorieOutput.Text = newMessage;
                    mpGoal.Text = "Goal: " + localSettings.Values["calorieGoal"] + " Calories Left.";
                }

            }
        }

        private void CalculateCalorieIntake(object sender, DatePickerValueChangedEventArgs e)
        {
            if (age.Text == "" || !Regex.IsMatch(age.Text, @"^\d+$")) return;
            else if (feet.Text == "" || !Regex.IsMatch(feet.Text, @"^\d+$")) return;// to be fixed
            else if (cWeight.Text == "" || !Regex.IsMatch(cWeight.Text, @"^\d.+$")) return;
            else if (gWeight.Text == "" || !Regex.IsMatch(gWeight.Text, @"^\d.+$")) return;
            else if (gDate.Date == DateTime.Now.Date) return;
            else
            {
                //get inches
                double numInches = 0.0;
                if (!(inches.Text == "") || Regex.IsMatch(inches.Text, @"^\d.+$")) numInches = Convert.ToDouble(inches.Text);
                TimeSpan span = gDate.Date - DateTime.Now;
                if (span.Days > 0)
                {
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    localSettings.Values["age"] = age.Text;
                    localSettings.Values["feet"] = feet.Text;
                    localSettings.Values["inches"] = inches.Text;
                    localSettings.Values["activityLevel"] = activityLevel.SelectedIndex;
                    localSettings.Values["gender"] = gender.IsOn;
                    localSettings.Values["cWeight"] = cWeight.Text;
                    localSettings.Values["gWeight"] = gWeight.Text;
                    localSettings.Values["gDate"] = gDate.Date.ToString();
                    string newMessage = CalorieIntakeMessage(Convert.ToInt32(age.Text), Convert.ToInt32(feet.Text), numInches, activityLevel.SelectedIndex, gender.IsOn, Convert.ToDouble(cWeight.Text), Convert.ToDouble(gWeight.Text), span.Days);
                    calorieOutput.Text = newMessage;
                    localSettings.Values["calorieOutput"] = newMessage;
                    mpGoal.Text = "Goal: " + localSettings.Values["calorieGoal"] + " Calories Left.";
                }

            }
        }

        private string CalorieIntakeMessage(int age, int feet, double inches, int actIndex, Boolean male, double cWeight, double gWeight, int days)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string result ="";
            //convert height to centimeters
            double cms = ((feet * 12) + inches) * 2.54;
            //convert current weight to killigrams
            double kgs = cWeight / 2.2046226;
            //calculate bmr or basal metabolic rate
            double bmr = 0.0;
            if (male) bmr = 66 + (13.7 * kgs) + (5 * cms) - (6.8 * age);
            else bmr = 665 + (9.6 * kgs) + (1.8 * cms) - (4.7 * age);
            //calculate tdee or total daily energy expenditure
            double tdee = bmr* activityLevels[actIndex];
            //if i'm trying to gain weight
            
            double goal =cWeight - gWeight;
            if (goal < 0)
            {
                //convert goal to calories
                goal = Math.Abs(goal);
                goal *= 3500;
                double calChange = goal / days;
                //saftey warning   
                if (calChange > 1000) return "Attempting to bulk or lose weight this fast is dangerous. Please modify goal weight or date.";
                localSettings.Values["calorieGoal"] = Math.Round((tdee + calChange), 2);
                result = " Calorie Goal is " + localSettings.Values["calorieGoal"] + " calories a day.";
                foreach (MealPlanItem e in MealPlanSource.getCurrentItems()) localSettings.Values["calorieGoal"] = (double)localSettings.Values["calorieGoal"] - e.Calories;    
            }
            else
            {
                goal *= 3500;
                double calChange = goal / days;
                if (calChange > 1000) return "Attempting to bulk or lose weight this fast is dangerous. Please modify goal weight or date.";
                localSettings.Values["calorieGoal"] = Math.Round((tdee - calChange), 2);
                result = " Calorie Goal is " + localSettings.Values["calorieGoal"] + " calories a day.";
                foreach (MealPlanItem e in MealPlanSource.getCurrentItems()) localSettings.Values["calorieGoal"] = (double)localSettings.Values["calorieGoal"] -e.Calories;
            }
            return result;
        }

        private async void MealPlan_Loaded(object sender, RoutedEventArgs e)
        {
            var sampleData = await MealPlanSource.GetItemsAsync();
            this.DefaultViewModel["MealPlan"] = sampleData;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var delete = (AppBarButton)sender;
            MealPlanSource.RemoveMealItem(delete.Label);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            mpGoal.Text = "Goal: " + localSettings.Values["calorieGoal"] + " Calories Left.";
        }

        protected override async void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DataModel//VoiceCommandDefiniton.xml"));
                await Windows.Media.SpeechRecognition.VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(storageFile);
            }
//            base.OnNavigatedTo(e);
        }

    }
}
