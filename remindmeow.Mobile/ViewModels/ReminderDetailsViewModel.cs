// remindmeow.Mobile/ViewModels/ReminderDetailsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using remindmeow.Core.Exceptions;
using remindmeow.core.Models;
using remindmeow.Core.Interfaces;

namespace remindmeow.Mobile.ViewModels
{
    public partial class ReminderDetailsViewModel(
    IRemindersService remindersService,
    IToastService toastService) : ObservableObject
    {
        private readonly IRemindersService _remindersService = remindersService;
        private readonly IToastService _toastService = toastService;

        [ObservableProperty]
        private Reminder? reminder;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand]
        private async Task LoadReminderAsync(string id)
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                Reminder = await _remindersService.GetReminderByIdAsync(id);
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to load reminder";
                _toastService.ShowError("Error loading reminder");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveReminderAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrEmpty(Reminder.Id))
                {
                    await _remindersService.CreateReminderAsync(Reminder);
                }
                else
                {
                    await _remindersService.UpdateReminderAsync(Reminder.Id, Reminder);
                }

                await Shell.Current.GoToAsync("..");
                _toastService.ShowSuccess("Reminder saved successfully");
            }
            catch (ReminderValidationException ex)
            {
                ErrorMessage = ex.Message;
                _toastService.ShowError(ex.Message);
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to save reminder";
                _toastService.ShowError("Error saving reminder");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}