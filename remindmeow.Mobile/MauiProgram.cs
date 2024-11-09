using Microsoft.Extensions.Logging;
using remindmeow.core.Interfaces;
using remindmeow.Infrastructure.Data;
using remindmeow.Infrastructure.Services;
using remindmeow.Mobile.Views;
using remindmeow.Mobile.ViewModels;
using remindmeow.Core.Interfaces;

namespace remindmeow.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
            builder.Services.AddSingleton<IRemindersService, RemindersService>();
            builder.Services.AddSingleton<IToastService, ToastService>();
            // Register pages and view models
            builder.Services.AddTransient<RemindersPage>();
            builder.Services.AddTransient<RemindersViewModel>();
            builder.Services.AddTransient<ReminderDetailsPage>();
            builder.Services.AddTransient<ReminderDetailsViewModel>();
            

            return builder.Build();
        }
    }
}