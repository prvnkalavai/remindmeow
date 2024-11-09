using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using remindmeow.core.Models;
using remindmeow.Mobile.Views;
using Microsoft.Maui.Controls;

namespace remindmeow.Mobile.ViewModels
{
    public partial class RemindersViewModel : ObservableObject
    {
        private readonly IRemindersService _remindersService;

        [ObservableProperty]
        private ObservableCollection<Reminder> reminders;

        [ObservableProperty]
        private bool isLoading;

        public RemindersViewModel(IRemindersService remindersService)
        {
            _remindersService = remindersService;
            Reminders = new ObservableCollection<Reminder>();
        }

        [RelayCommand]
        private async Task LoadRemindersAsync()
        {
            try
            {
                IsLoading = true;
                var items = await _remindersService.GetActiveRemindersAsync();
                Reminders.Clear();
                foreach (var item in items)
                {
                    Reminders.Add(item);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToDetailsAsync(Reminder reminder)
        {
            if (reminder != null)
            {
                await Shell.Current.GoToAsync($"{nameof(ReminderDetailsPage)}?id={reminder.Id}");
            }
        }

        [RelayCommand]
        private async Task AddReminderAsync(string question)
        {
            var reminder = new Reminder
            {
                Question = question,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _remindersService.CreateReminderAsync(reminder);
            if (result != null)
            {
                Reminders.Add(result);
            }
        }

        [RelayCommand]
        private async Task DeleteReminderAsync(string id)
        {
            if (await _remindersService.DeleteReminderAsync(id))
            {
                var reminderToRemove = Reminders.FirstOrDefault(r => r.Id == id);
                if (reminderToRemove != null)
                    Reminders.Remove(reminderToRemove);
            }
        }

        [RelayCommand]
        private async Task UpdateReminderAsync(Reminder reminder)
        {
            var updated = await _remindersService.UpdateReminderAsync(reminder.Id, reminder);
            if (updated != null)
            {
                var index = Reminders.IndexOf(Reminders.FirstOrDefault(r => r.Id == reminder.Id));
                if (index != -1)
                    Reminders[index] = updated;
            }
        }
    }
}