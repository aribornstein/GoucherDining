using GoucherDiningAppPOC1.Common;
using GoucherDiningAppPOC1.Data;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace GoucherDiningAppPOC1
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
    
        private TransitionCollection transitions;

        public static MobileServiceClient MobileService = new MobileServiceClient(
    "https://goucherdiningservice.azure-mobile.net/",
    "dNFeluLccniVJNIwoPeVwydCostJQT18"
);
        
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            AzureDiningHallSource adhs= new AzureDiningHallSource();
            adhs.GetDHData();

            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["MaxMealPlanId"] == null) localSettings.Values["MaxMealPlanId"] = 0;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active.
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page.
                rootFrame = new Frame();

                // Associate the frame with a SuspensionManager key.
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: Change this value to a cache size that is appropriate for your application.
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate.
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue.
                    }
                }

                // Place the frame in the current Window.
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter.
                if (!rootFrame.Navigate(typeof(DiningHalls), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active.
            Window.Current.Activate();
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            // Was the app activated by a voice command?
            if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.VoiceCommand)
            {
                var commandArgs = args as Windows.ApplicationModel.Activation.VoiceCommandActivatedEventArgs;
                Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

                // If so, get the name of the voice command, the actual text spoken, and the value of Command/Navigate@Target.
                string voiceCommandName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;
                string navigationTarget = speechRecognitionResult.SemanticInterpretation.Properties["NavigationTarget"][0];
                Windows.Media.SpeechSynthesis.SpeechSynthesizer x = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
                string response = textSpoken.ToString();
                switch (voiceCommandName)
                {
                    case "isOpen":
                        if (textSpoken.Contains("Stimson")) response = await getIsOpenResponse("Stimson");
                        else if (textSpoken.Contains("Heubeck")) response = await getIsOpenResponse("Heubeck");
                        else if (textSpoken.Contains("Kosher")) response = await getIsOpenResponse("Kosher");
                        else if (textSpoken.Contains("Pearlstone"))response = await getIsOpenResponse("Pearlstone"); 
                        else if (textSpoken.Contains("The Van")) response = await getIsOpenResponse("The Van");
                        else if (textSpoken.Contains("Alice")) response = await getIsOpenResponse("Alice's");
                        break;

                    // Cases for other voice commands.

                    default:
                        // There is no match for the voice command name.
                        break;
                }
                // text to speech
                var stream = await x.SynthesizeTextToStreamAsync(response);
                var mediaElement = new MediaElement();
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
                

             

            }
        }

        private async Task<string> getIsOpenResponse(string diningHall)
        {
            ObservableCollection<DiningHall> diningHalls = (ObservableCollection<DiningHall>)await DiningHallSource.GetDiningHallsAsync();
            //get correct dining hall
            var diningInds = new Dictionary<string, int>() { { "Stimson", 0 }, { "Heubeck", 1 }, { "Pearlstone", 2 }, { "Kosher", 3 }, { "The Van", 4 }, { "Alice's", 5 } };
            //get correct meal index based on current time
            List<Menu> menus = diningHalls[diningInds[diningHall]].Menus;
            int menuIndex = 0;
            while (menuIndex < menus.Count())
            {
                //by date disabled enable when menu pull finalized
                //if (DateTime.Now.Date.Equals(menus[menuIndex].Date) && (DateTime.Now.TimeOfDay > menus[menuIndex].Open) && (DateTime.Now.TimeOfDay < menus[menuIndex].Close)) result = true;

                if ((DateTime.Now.TimeOfDay > menus[menuIndex].Open.TimeOfDay) && (DateTime.Now.TimeOfDay < menus[menuIndex].Close.TimeOfDay)) return diningHall + " is open until " + (menus[menuIndex].Close.ToString("h:mm")) + ".";
                menuIndex++;
            }
            return diningHall + "is closed.";

        }
    }
}
