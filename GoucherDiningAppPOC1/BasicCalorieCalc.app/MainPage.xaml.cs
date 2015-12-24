using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BasicCalorieCalc.app
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.LoadColor();
            this.LoadConfig();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private void LoadColor()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["colorIndex"] == null) return;
            if ((int)localSettings.Values["colorIndex"] == 1) this.Background = (SolidColorBrush)Resources["PhoneAccentBrush"];          
            else if ((int)localSettings.Values["colorIndex"] == 2)this.Background = new SolidColorBrush(Windows.UI.Colors.Chocolate);
            else if ((int)localSettings.Values["colorIndex"] == 3)this.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            else if ((int)localSettings.Values["colorIndex"] == 4)this.Background = new SolidColorBrush(Windows.UI.Colors.Green);
        }
        private void LoadConfig()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            //age
            if (localSettings.Values["age"] == null) localSettings.Values["age"] = "";
            age.Text = (string)localSettings.Values["age"];
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
            activityLevel.SelectedIndex = (int)localSettings.Values["activityLevel"];
            //gender
            if (localSettings.Values["gender"] == null) localSettings.Values["gender"] = false;
            gender.IsOn = (Boolean)localSettings.Values["gender"];
            //gDate
            if (localSettings.Values["gDate"] == null) localSettings.Values["gDate"] = DateTime.Now.Date.ToString();
            gDate.Date = DateTime.Parse((string)localSettings.Values["gDate"]);
            //gWeight
            if (localSettings.Values["gWeight"] == null) localSettings.Values["gWeight"] = "";
            gWeight.Text = (string)localSettings.Values["gWeight"];
            //calorieOutput
            if (localSettings.Values["calorieOutput"] == null) localSettings.Values["calorieOutput"] = "Calorie Goal to be calculated.";
            calorieOutput.Text = (string)localSettings.Values["calorieOutput"];
     
           
        }
        private double[] activityLevels = { 1.2, 1.375, 1.55, 1.725, 1.9 };
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void CalculateCalorieIntake(object sender, RoutedEventArgs e)
        {
            if (age.Text == "" ) return;
            else if (feet.Text == "" ) return;// to be fixed
            else if (cWeight.Text == "" ) return;
            else if (gWeight.Text == "" ) return;
            else if (gDate.Date == DateTime.Now.Date) return;
            else
            {
                //get inches
                double numInches = 0.0;
                if (!(inches.Text == "") ) numInches = Convert.ToDouble(inches.Text);
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
                    localSettings.Values["calorieOutput"] = calorieOutput.Text = newMessage;
                   
                }

            }
        }

        private void CalculateCalorieIntake(object sender, DatePickerValueChangedEventArgs e)
        {
            if (age.Text == "" ) return;
            else if (feet.Text == "" ) return;// to be fixed
            else if (cWeight.Text == "" ) return;
            else if (gWeight.Text == "" ) return;
            else if (gDate.Date == DateTime.Now.Date) return;
            else
            {
                //get inches
                double numInches = 0.0;
                if (!(inches.Text == "") ) numInches = Convert.ToDouble(inches.Text);
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
                }

            }
        }

        private string CalorieIntakeMessage(int age, int feet, double inches, int actIndex, Boolean male, double cWeight, double gWeight, int days)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string result = "";
            //convert height to centimeters
            double cms = ((feet * 12) + inches) * 2.54;
            //convert current weight to killigrams
            double kgs = cWeight / 2.2046226;
            //calculate bmr or basal metabolic rate
            double bmr = 0.0;
            if (male) bmr = 66 + (13.7 * kgs) + (5 * cms) - (6.8 * age);
            else bmr = 665 + (9.6 * kgs) + (1.8 * cms) - (4.7 * age);
            //calculate tdee or total daily energy expenditure
            double tdee = bmr * activityLevels[actIndex];
            //if i'm trying to gain weight

            double goal = cWeight - gWeight;
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
            }
            else
            {
                goal *= 3500;
                double calChange = goal / days;
                if (calChange > 1000) return "Attempting to bulk or lose weight this fast is dangerous. Please modify goal weight or date.";
                localSettings.Values["calorieGoal"] = Math.Round((tdee - calChange), 2);
                result = " Calorie Goal is " + localSettings.Values["calorieGoal"] + " calories a day.";
            }
            return result;
        }

        private void ChangeColor(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["colorIndex"] == null || (int)localSettings.Values["colorIndex"] == 0)
            {
                this.Background = (SolidColorBrush)Resources["PhoneAccentBrush"];
                localSettings.Values["colorIndex"]=1;
            }
            else if ((int)localSettings.Values["colorIndex"] == 1)
            {
                this.Background = new SolidColorBrush(Windows.UI.Colors.Chocolate);
                localSettings.Values["colorIndex"] = 2;
            }
            else if ((int)localSettings.Values["colorIndex"] == 2)
            {
                this.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                localSettings.Values["colorIndex"] = 3;
            }
            else if ((int)localSettings.Values["colorIndex"] == 3)
            {
                this.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                localSettings.Values["colorIndex"] = 4;
            }
            else 
            {
                this.Background = new SolidColorBrush(Windows.UI.Colors.Black);
                localSettings.Values["colorIndex"]=0;
            }
 
 
        }

    }
}
