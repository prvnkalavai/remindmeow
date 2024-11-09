//remindmeow.Infrastructure/Services/ToastService.cs
using Microsoft.Maui.Controls;
using remindmeow.Core.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Xamarin.Essentials;
using MainThread = Microsoft.Maui.ApplicationModel.MainThread;

namespace remindmeow.Infrastructure.Services
{
    public class ToastService : IToastService
    {
        public void ShowSuccess(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Success", message, "OK");
            });
        }

        public void ShowError(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", message, "OK");
            });
        }

        public void ShowWarning(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Warning", message, "OK");
            });
        }

        public void ShowInfo(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Info", message, "OK");
            });
        }
    }
}