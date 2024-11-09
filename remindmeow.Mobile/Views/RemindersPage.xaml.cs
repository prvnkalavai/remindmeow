//remindmeow.Mobile/Views/RemindersPage.xaml.cs
using remindmeow.Mobile.ViewModels;

namespace remindmeow.Mobile.Views
{
    public partial class RemindersPage : ContentPage
    {
        public RemindersPage(RemindersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ((RemindersViewModel)BindingContext).LoadRemindersCommand.ExecuteAsync(null);
        }
    }
}