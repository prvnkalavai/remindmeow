// remindmeow.core/Interfaces/IReminderService.cs
using remindmeow.core.Models;

public interface IRemindersService
{
    Task<IEnumerable<Reminder>> GetActiveRemindersAsync();
    Task<Reminder> GetReminderByIdAsync(string id);
    Task<Reminder> CreateReminderAsync(Reminder reminder);
    Task<Reminder> UpdateReminderAsync(string id, Reminder reminder);
    Task<bool> DeleteReminderAsync(string id);
    Task<IEnumerable<Reminder>> GetDueRemindersAsync();
    DateTime? CalculateNextDueDate(Reminder reminder, DateTime? baseDate = null);
}