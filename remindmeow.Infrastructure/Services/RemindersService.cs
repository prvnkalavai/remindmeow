using remindmeow.core.Interfaces;
using remindmeow.core.Models;

namespace remindmeow.Infrastructure.Services
{
    public class RemindersService : IRemindersService
    {
        private readonly ILocalStorageService _localStorage;

        public RemindersService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            await _localStorage.SaveAsync(reminder);
            return reminder;
        }

        public async Task<bool> DeleteReminderAsync(string id)
        {
            var result = await _localStorage.DeleteAsync(id);
            return result > 0;
        }

        public async Task<IEnumerable<Reminder>> GetActiveRemindersAsync()
        {
            return await _localStorage.GetActiveRemindersAsync();
        }

        public async Task<IEnumerable<Reminder>> GetDueRemindersAsync()
        {
            return await _localStorage.GetDueRemindersAsync();
        }

        public async Task<Reminder> GetReminderByIdAsync(string id)
        {
            return await _localStorage.GetByIdAsync(id);
        }

        public async Task<Reminder> UpdateReminderAsync(string id, Reminder reminder)
        {
            reminder.Id = id;
            await _localStorage.SaveAsync(reminder);
            return reminder;
        }
    }
}