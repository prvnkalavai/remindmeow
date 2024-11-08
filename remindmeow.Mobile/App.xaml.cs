using remindmeow.Mobile.Views;

namespace remindmeow.Mobile
{
    public partial class App : Application
    {        
        public App(IServiceProvider serviceProvider)
        {
            MainPage = new AppShell();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(RemindersPage), typeof(RemindersPage));
        }

        
    }
}
