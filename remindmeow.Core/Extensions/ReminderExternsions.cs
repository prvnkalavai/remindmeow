// remindmeow.Core/Extensions/ReminderExtensions.cs
using remindmeow.core.Models;

namespace remindmeow.Core.Extensions
{
    public static class ReminderExtensions
    {
        public static DateTime? CalculateNextDueDate(this Reminder reminder)
        {
            if (!reminder.IsActive || reminder.Recurrence == RecurrenceType.None)
                return null;

            var baseDate = reminder.NextDueDate ?? reminder.CreatedAt;

            return reminder.Recurrence switch
            {
                RecurrenceType.Daily => baseDate.AddDays(1),
                RecurrenceType.Weekly => baseDate.AddDays(7),
                RecurrenceType.Monthly => baseDate.AddMonths(1),
                RecurrenceType.Custom => null, // Handle custom recurrence separately
                _ => null
            };
        }
    }
}