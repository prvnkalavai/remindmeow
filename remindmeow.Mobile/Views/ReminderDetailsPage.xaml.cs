//remindmeow.mobile/views/ReminderDetailsPage.xaml.cs
using Microsoft.Maui.Controls;
using remindmeow.Mobile.ViewModels;

namespace remindmeow.Mobile.Views
{
    [QueryProperty(nameof(ReminderId), "id")]
    public partial class ReminderDetailsPage : ContentPage
    {
        private readonly ReminderDetailsViewModel _viewModel;

        public required string ReminderId { get; set; }

        public ReminderDetailsPage(ReminderDetailsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (!string.IsNullOrEmpty(ReminderId))
            {
                await _viewModel.LoadReminderCommand.ExecuteAsync(ReminderId);
            }
        }
    }
}