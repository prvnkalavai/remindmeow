using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using remindmeow.core.Interfaces;
using remindmeow.core.Models;
using remindmeow.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Application = Microsoft.Maui.Controls.Application;

namespace remindmeow.Platforms.Android
{
    [BroadcastReceiver(Label = "Reminder Widget", Exported = true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/reminder_widget_info")]
    public class ReminderWidget : AppWidgetProvider
    {
        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var remindersService = Application.Current?.Handler.MauiContext?.Services.GetService<IRemindersService>();
            if (remindersService == null) return;

            var packageName = context.PackageName;

            // Get the layout ID using Resources instead of Resource
            var widgetLayoutId = context.Resources.GetIdentifier("reminder_widget", "layout", packageName);

            foreach (var widgetId in appWidgetIds)
            {
                var remoteViews = new RemoteViews(packageName, widgetLayoutId);

                var dueReminders = await remindersService.GetDueRemindersAsync();
                var firstDueReminder = dueReminders.FirstOrDefault();

                // Get resource IDs at runtime
                var questionId = context.Resources.GetIdentifier("widget_reminder_question", "id", packageName);
                var yesButtonId = context.Resources.GetIdentifier("widget_yes_button", "id", packageName);
                var noButtonId = context.Resources.GetIdentifier("widget_no_button", "id", packageName);

                if (firstDueReminder != null)
                {
                    remoteViews.SetTextViewText(questionId, firstDueReminder.Question);

                    // Set up Yes button intent
                    var yesIntent = new Intent(context, typeof(ReminderWidget));
                    yesIntent.SetAction("REMINDER_YES");
                    yesIntent.PutExtra("ReminderId", firstDueReminder.Id);
                    var yesPendingIntent = PendingIntent.GetBroadcast(
                        context,
                        0,
                        yesIntent,
                        PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                    );
                    remoteViews.SetOnClickPendingIntent(yesButtonId, yesPendingIntent);

                    // Set up No button intent
                    var noIntent = new Intent(context, typeof(ReminderWidget));
                    noIntent.SetAction("REMINDER_NO");
                    noIntent.PutExtra("ReminderId", firstDueReminder.Id);
                    var noPendingIntent = PendingIntent.GetBroadcast(
                        context,
                        1,
                        noIntent,
                        PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                    );
                    remoteViews.SetOnClickPendingIntent(noButtonId, noPendingIntent);
                }
                else
                {
                    remoteViews.SetTextViewText(questionId, "No reminders due");
                }

                appWidgetManager.UpdateAppWidget(widgetId, remoteViews);
            }
        }

        public override async void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            var remindersService = Application.Current?.Handler.MauiContext?.Services.GetService<IRemindersService>();
            if (remindersService == null) return;

            var reminderId = intent.GetStringExtra("ReminderId");
            if (string.IsNullOrEmpty(reminderId)) return;

            var reminder = await remindersService.GetReminderByIdAsync(reminderId);
            if (reminder != null)
            {
                reminder.Answer = intent.Action == "REMINDER_YES";
                reminder.NextDueDate = remindersService.CalculateNextDueDate(reminder);
                await remindersService.UpdateReminderAsync(reminder.Id, reminder);

                // Update widget
                var appWidgetManager = AppWidgetManager.GetInstance(context);
                var appWidgetIds = appWidgetManager.GetAppWidgetIds(
                    new ComponentName(context, Java.Lang.Class.FromType(typeof(ReminderWidget)))
                );
                OnUpdate(context, appWidgetManager, appWidgetIds);
            }
        }
    }
}