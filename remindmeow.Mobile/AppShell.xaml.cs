using remindmeow.Mobile.Views;

namespace remindmeow.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(ReminderDetailsPage), typeof(ReminderDetailsPage));
            Routing.RegisterRoute(nameof(RemindersPage), typeof(RemindersPage));
        }
    }
}